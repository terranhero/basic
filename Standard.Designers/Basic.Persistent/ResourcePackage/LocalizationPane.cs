using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Basic.Designer;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Point = System.Windows.Point;

namespace Basic.Localizations
{
	/// <summary>
	/// 
	/// </summary>
	[System.Runtime.InteropServices.ComSourceInterfaces(typeof(IVsTextViewEvents))]
	[System.Runtime.InteropServices.ComVisible(true)]
	[System.Runtime.InteropServices.Guid(Consts.guidPaneString)]
	[SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
	public sealed class LocalizationPane : WindowPane, IVsPersistDocData, IPersistFileFormat,
		 IVsFileChangeEvents, IVsDocDataFileChangeControl
	{
		//private readonly LocalizationPackage asyncPackage;
		private readonly LocalizationService _Commands;

		private const string FileExtension = ".localresx";
		private bool isDirty = false, loading = false;
		private uint vsFileChangeCookie;
		private IVsFileChangeEx vsFileChangeEx;
		private ITrackSelection trackSelection;
		private readonly LocalizationPackage asyncPackage;
		/// <summary>Vs 属性窗口可选择对象</summary>
		private readonly Microsoft.VisualStudio.Shell.SelectionContainer selectionContainer;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="package"></param>
		/// <param name="commands"></param>
		/// <param name="item"></param>
		public LocalizationPane(LocalizationPackage package, LocalizationService commands, EnvDTE.ProjectItem item)
			: base(package)
		{
			asyncPackage = package; ProjectItem = item;
			_Commands = commands;
			selectionContainer = new Microsoft.VisualStudio.Shell.SelectionContainer(false, false);
			selectionContainer.SelectedObjectsChanged += new EventHandler(OnSelectionChanged);
			Localizations.ContentChanged += new EventHandler(RaiseChanged);
			Content = Editor = new ResourceEditor(Localizations);
			Editor.ContextMenuOpening += new ContextMenuEventHandler(OnContextMenuOpening);
			Editor.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
			Editor.SelectedCellsChanged += new SelectedCellsChangedEventHandler(OnSelectedCellsChanged);
		}

		/// <summary></summary>
		public ResourceEditor Editor { get; }

		#region Editor_Events
		private void SetSelectedObjects(LocalizationItem[] items)
		{
			if (items == null || items.Length == 0) { return; }
			selectionContainer.SelectableObjects = items.Select(m => new ObjectDescriptor(m)).ToArray();
			LocalizationItem item = items.LastOrDefault();
			selectionContainer.SelectedObjects = new ObjectDescriptor[] { new ObjectDescriptor(item) };
			ITrackSelection track = GetITrackSelection();
			if (track != null) { track.OnSelectChange(selectionContainer); }
		}

		/// <summary>
		/// 引发 System.Windows.Controls.DataGrid.SelectedCellsChanged 事件。
		/// </summary>
		/// <param name="e"> 事件的相关数据。</param>
		private void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			if (e.AddedCells != null && e.AddedCells.Count > 0)
			{
				LocalizationItem[] items = e.AddedCells.Select(m => m.Item).Cast<LocalizationItem>().Distinct().ToArray();
				SetSelectedObjects(items);
			}
		}

		/// <summary>
		/// 当选择更改时调用。
		/// </summary>
		/// <param name="e">事件的相关数据。</param>
		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems != null && e.AddedItems.Count > 0)
			{
				LocalizationItem[] items = e.AddedItems.Cast<LocalizationItem>().ToArray();
				SetSelectedObjects(items);
			}
		}

		private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			try
			{
				ThreadHelper.ThrowIfNotOnUIThread(); e.Handled = true;
				Point point = Editor.PointToScreen(Mouse.GetPosition(Editor));
				_Commands.ShowContextMenu((int)point.X, (int)point.Y);
			}
			catch (Exception ex)
			{
				_Commands.ShowMessage(ex);
			}
		}
		#endregion

		/// <summary></summary>
		public ProjectItem ProjectItem { get; }

		/// <summary></summary>
		private ITrackSelection GetITrackSelection()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (trackSelection == null) { trackSelection = (ITrackSelection)GetService(typeof(ITrackSelection)); }
			return trackSelection;
		}

		/// <summary></summary>
		public LocalizationCollection Localizations { get; } = new LocalizationCollection();

		/// <summary>
		/// This is an added command handler that will make it so the ITrackSelection.OnSelectChange
		/// function gets called whenever the cursor position is changed and also so the position 
		/// displayed on the status bar will update whenever the cursor position changes.
		/// </summary>
		/// <param name="sender"> Not used.</param>
		/// <param name="e"> Not used.</param>
		internal void OnSelectionChanged(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			ITrackSelection track = GetITrackSelection();
			if (track != null) { track.OnSelectChange(selectionContainer); }
		}

		/// <summary>
		/// 当前打开的文件名
		/// </summary>
		internal string FileName { get; private set; } = string.Empty;

		/// <summary>
		/// 
		/// </summary>
		internal void RaiseChanged(object sender, EventArgs args)
		{
			//ThreadHelper.ThrowIfNotOnUIThread();
			if (loading == false) { isDirty = true; OnSelectionChanged(sender, args); }
		}

		#region 接口 IVsPersistDocData 的默认实现
		/// <summary>
		/// Close the IVsPersistDocData object
		/// </summary>
		/// <returns>S_OK if the function succeeds</returns>
		int IVsPersistDocData.Close() { return VSConstants.S_OK; }


		/// <summary>
		/// Returns the Guid of the editor factory that created the IVsPersistDocData object
		/// </summary>
		/// <param name="pClassID">Pointer to the class identifier of the editor type</param>
		/// <returns>S_OK if the method succeeds</returns>
		[SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
		int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
		{
			return ((IPersistFileFormat)this).GetClassID(out pClassID);
		}

		/// <summary>
		/// Used to determine if the document data has changed since the last time it was saved
		/// </summary>
		/// <param name="pfDirty">Will be set to 1 if the data has changed</param>
		/// <returns>S_OK if the function succeeds</returns>
		int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
		{
			return ((IPersistFileFormat)this).IsDirty(out pfDirty);
		}

		/// <summary>
		/// Determines if it is possible to reload the document data
		/// </summary>
		/// <param name="pfReloadable">set to 1 if the document can be reloaded</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
		{
			pfReloadable = 1; return VSConstants.S_OK;
		}

		/// <summary>
		/// Loads the document data from the file specified
		/// </summary>
		/// <param name="pszMkDocument">Path to the document file which needs to be loaded</param>
		/// <returns>S_Ok if the method succeeds</returns>
		int IVsPersistDocData.LoadDocData(string pszMkDocument)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return ((IPersistFileFormat)this).Load(pszMkDocument, 0, 0);
		}

		/// <summary>
		/// Called by the Running Document Table when it registers the document data. 
		/// </summary>
		/// <param name="docCookie">Handle for the document to be registered</param>
		/// <param name="pHierNew">Pointer to the IVsHierarchy interface</param>
		/// <param name="itemidNew">Item identifier of the document to be registered from VSITEM</param>
		/// <returns></returns>
		int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
		{
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Reloads the document data
		/// </summary>
		/// <param name="grfFlags">Flag indicating whether to ignore the next file change when reloading the document data.
		/// This flag should not be set for us since we implement the "IVsDocDataFileChangeControl" interface in order to 
		/// indicate ignoring of file changes
		/// </param>
		/// <returns>S_OK if the mthod succeeds</returns>
		int IVsPersistDocData.ReloadDocData(uint grfFlags)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return ((IPersistFileFormat)this).Load(FileName, grfFlags, 0);
		}

		/// <summary>
		/// Renames the document data
		/// </summary>
		/// <param name="grfAttribs"></param>
		/// <param name="pHierNew"></param>
		/// <param name="itemidNew"></param>
		/// <param name="pszMkDocumentNew"></param>
		/// <returns></returns>
		int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
		{
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Saves the document data. Before actually saving the file, we first need to indicate to the environment
		/// that a file is about to be saved. This is done through the "SVsQueryEditQuerySave" service. We call the
		/// "QuerySaveFile" function on the service instance and then proceed depending on the result returned as follows:
		/// If result is QSR_SaveOK - We go ahead and save the file and the file is not read only at this point.
		/// If result is QSR_ForceSaveAs - We invoke the "Save As" functionality which will bring up the Save file name 
		///                                dialog 
		/// If result is QSR_NoSave_Cancel - We cancel the save operation and indicate that the document could not be saved
		///                                by setting the "pfSaveCanceled" flag
		/// If result is QSR_NoSave_Continue - Nothing to do here as the file need not be saved
		/// </summary>
		/// <param name="dwSave">Flags which specify the file save options:
		/// VSSAVE_Save        - Saves the current file to itself.
		/// VSSAVE_SaveAs      - Prompts the User for a filename and saves the file to the file specified.
		/// VSSAVE_SaveCopyAs  - Prompts the user for a filename and saves a copy of the file with a name specified.
		/// VSSAVE_SilentSave  - Saves the file without prompting for a name or confirmation.  
		/// </param>
		/// <param name="pbstrMkDocumentNew">Pointer to the path to the new document</param>
		/// <param name="pfSaveCanceled">value 1 if the document could not be saved</param>
		/// <returns></returns>
		int IVsPersistDocData.SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			pbstrMkDocumentNew = null; pfSaveCanceled = 0;
			if (!isDirty) { return VSConstants.S_OK; }
			int hr;
			switch (dwSave)
			{
				case VSSAVEFLAGS.VSSAVE_Save:
				case VSSAVEFLAGS.VSSAVE_SilentSave:
					IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2)GetService(typeof(SVsQueryEditQuerySave));
					// Call QueryEditQuerySave
					uint result;
					hr = queryEditQuerySave.QuerySaveFile(FileName, 0, null, out result);
					if (ErrorHandler.Failed(hr))
						return hr;

					// Process according to result from QuerySave
					switch ((tagVSQuerySaveResult)result)
					{
						case tagVSQuerySaveResult.QSR_NoSave_Cancel:
							// Note that this is also case tagVSQuerySaveResult.QSR_NoSave_UserCanceled because these
							// two tags have the same value.
							pfSaveCanceled = ~0;
							break;

						case tagVSQuerySaveResult.QSR_SaveOK:
							{
								// Call the shell to do the save for us
								IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
								hr = uiShell.SaveDocDataToFile(dwSave, (IPersistFileFormat)this, FileName, out pbstrMkDocumentNew, out pfSaveCanceled);
								if (ErrorHandler.Failed(hr))
									return hr;
							}
							break;
						case tagVSQuerySaveResult.QSR_ForceSaveAs:
							{
								// Call the shell to do the SaveAS for us
								IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
								hr = uiShell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SaveAs, (IPersistFileFormat)this, FileName, out pbstrMkDocumentNew, out pfSaveCanceled);
								if (ErrorHandler.Failed(hr))
									return hr;
							}
							break;

						case tagVSQuerySaveResult.QSR_NoSave_Continue:
							// In this case there is nothing to do.
							break;

						default:
							throw new NotSupportedException("Unsupported result from QEQS");
					}
					break;
				case VSSAVEFLAGS.VSSAVE_SaveAs:
				case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
					// Make sure the file name as the right extension
					if (String.Compare(FileExtension, Path.GetExtension(FileName), true, CultureInfo.CurrentCulture) != 0)
					{
						FileName += FileExtension;
					}
					// Call the shell to do the save for us
					IVsUIShell vsUIShell = (IVsUIShell)GetService(typeof(SVsUIShell));
					hr = vsUIShell.SaveDocDataToFile(dwSave, (IPersistFileFormat)this, FileName, out pbstrMkDocumentNew, out pfSaveCanceled);
					if (ErrorHandler.Failed(hr))
						return hr;
					break;
				default:
					throw new ArgumentException("Unsupported Save flag");
			};

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Used to set the initial name for unsaved, newly created document data
		/// </summary>
		/// <param name="pszDocDataPath">String containing the path to the document. We need to ignore this parameter
		/// </param>
		/// <returns>S_OK if the mthod succeeds</returns>
		int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return ((IPersistFileFormat)this).InitNew(Consts.FileFormat);
		}
		#endregion

		#region 接口 IPersistFileFormat 的默认实现
		/// <summary>
		/// Returns the class identifier of the editor type
		/// </summary>
		/// <param name="pClassID">pointer to the class identifier</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.GetClassID(out Guid pClassID)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			ErrorHandler.ThrowOnFailure(((Microsoft.VisualStudio.OLE.Interop.IPersist)this).GetClassID(out pClassID));
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Returns the path to the object's current working file 
		/// </summary>
		/// <param name="ppszFilename">Pointer to the file name</param>
		/// <param name="pnFormatIndex">Value that indicates the current format of the file as a zero based index
		/// into the list of formats. Since we support only a single format, we need to return zero. 
		/// Subsequently, we will return a single element in the format list through a call to GetFormatList.</param>
		/// <returns></returns>
		int IPersistFileFormat.GetCurFile(out string ppszFilename, out uint pnFormatIndex)
		{
			ppszFilename = FileName;
			pnFormatIndex = Consts.FileFormat;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Provides the caller with the information necessary to open the standard common "Save As" dialog box. 
		/// This returns an enumeration of supported formats, from which the caller selects the appropriate format. 
		/// Each string for the format is terminated with a newline (\n) character. 
		/// The last string in the buffer must be terminated with the newline character as well. 
		/// The first string in each pair is a display string that describes the filter, such as "Text Only 
		/// (*.txt)". The second string specifies the filter pattern, such as "*.txt". To specify multiple filter 
		/// patterns for a single display string, use a semicolon to separate the patterns: "*.htm;*.html;*.asp". 
		/// A pattern string can be a combination of valid file name characters and the asterisk (*) wildcard character. 
		/// Do not include spaces in the pattern string. The following string is an example of a file pattern string: 
		/// "HTML File (*.htm; *.html; *.asp)\n*.htm;*.html;*.asp\nText File (*.txt)\n*.txt\n."
		/// </summary>
		/// <param name="ppszFormatList">Pointer to a string that contains pairs of format filter strings</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.GetFormatList(out string ppszFormatList)
		{
			char Endline = (char)'\n';
			ppszFormatList = string.Concat(Consts.FileFormatName, "(*", FileExtension, ")",
				 Endline, "*", FileExtension, Endline, Endline);
			//string.Format(CultureInfo.InvariantCulture, "{2} (*{0}){1}*{0}{1}{1}",
			//Consts.FileExtension, Endline, Consts.FileFormatName);
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Initialization for the object 
		/// </summary>
		/// <param name="nFormatIndex">Zero based index into the list of formats that indicates the current format 
		/// of the file</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.InitNew(uint nFormatIndex)
		{
			if (nFormatIndex != Consts.FileFormat)
			{
				return VSConstants.E_INVALIDARG;
			}
			// until someone change the file, we can consider it not dirty as
			// the user would be annoyed if we prompt him to save an empty file
			isDirty = false;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Determines whether an object has changed since being saved to its current file
		/// </summary>
		/// <param name="pfIsDirty">true if the document has changed</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.IsDirty(out int pfIsDirty)
		{
			pfIsDirty = isDirty ? 1 : 0;
			return VSConstants.S_OK;
		}


		/// <summary>
		/// Loads the file content into the textbox
		/// </summary>
		/// <param name="pszFilename">Pointer to the full path name of the file to load</param>
		/// <param name="grfMode">file format mode</param>
		/// <param name="fReadOnly">determines if teh file should be opened as read only</param>
		/// <returns>S_OK if the method succeeds</returns>
		[SuppressMessage("Usage", "VSTHRD104:提供异步方法", Justification = "<挂起>")]
		int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
		{
			if (pszFilename == null) { return VSConstants.E_INVALIDARG; }
			try
			{
				ThreadHelper.JoinableTaskFactory.Run(async () =>
				{
					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
					loading = true; _Commands.SetWaitCursor();
					Localizations.Clear();
					_ = Localizations.Load(pszFilename, m =>
					{
						if (File.Exists(m)) { return File.OpenRead(m); }
						else { return null; }
					});

					isDirty = false; int hr = 0;
					ITrackSelection track = GetITrackSelection();
					if (track != null) { hr = track.OnSelectChange(selectionContainer); }
					if (ErrorHandler.Failed(hr)) { return; }

					// Hook up to file change notifications
					if (string.IsNullOrEmpty(FileName) || 0 != string.Compare(FileName, pszFilename, true, CultureInfo.CurrentCulture))
					{
						FileName = pszFilename;
						SetFileChangeNotification(pszFilename, true);
						// Notify the load or reload
						NotifyDocChanged();
					}
				});
			}
			catch (Exception ex)
			{
				_Commands.ShowMessage(ex);
			}
			finally
			{
				loading = false;
			}
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Save the contents of the textbox into the specified file. If doing the save on the same file, we need to
		/// suspend notifications for file changes during the save operation.
		/// </summary>
		/// <param name="pszFilename">Pointer to the file name. If the pszFilename parameter is a null reference 
		/// we need to save using the current file
		/// </param>
		/// <param name="fRemember">Boolean value that indicates whether the pszFileName parameter is to be used 
		/// as the current working file.
		/// If remember != 0, pszFileName needs to be made the current file and the dirty flag needs to be cleared after the save.
		///                   Also, file notifications need to be enabled for the new file and disabled for the old file 
		/// If remember == 0, this save operation is a Save a Copy As operation. In this case, 
		///                   the current file is unchanged and dirty flag is not cleared
		/// </param>
		/// <param name="nFormatIndex">Zero based index into the list of formats that indicates the format in which 
		/// the file will be saved</param>
		/// <returns>S_OK if the method succeeds</returns>
		[SuppressMessage("Usage", "VSTHRD104:提供异步方法", Justification = "<挂起>")]
		int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			int hr = VSConstants.S_OK;
			bool doingSaveOnSameFile = false;
			// If file is null or same --> SAVE
			if (pszFilename == null || pszFilename == FileName)
			{
				fRemember = 1;
				doingSaveOnSameFile = true;
			}

			//Suspend file change notifications for only Save since we don't have notifications setup
			//for SaveAs and SaveCopyAs (as they are different files)
			if (doingSaveOnSameFile)
				this.SuspendFileChangeNotification(pszFilename, 1);

			try
			{
				ProjectItem item = ProjectItem;
				EnvDTE.Solution solution = item.DTE.Solution;
				string[] files = Localizations.Save(pszFilename);
				foreach (string filePath in files)
				{
					ProjectItem sitem = solution.FindProjectItem(filePath);
					if (item == null) { _ = item.ProjectItems.AddFromFile(filePath); }
				}
			}
			catch (ArgumentException)
			{
				hr = VSConstants.E_FAIL;
			}
			catch (IOException)
			{
				hr = VSConstants.E_FAIL;
			}
			finally
			{
				//restore the file change notifications
				if (doingSaveOnSameFile)
					this.SuspendFileChangeNotification(pszFilename, 0);
			}

			if (VSConstants.E_FAIL == hr)
				return hr;

			//Save and Save as
			if (fRemember != 0)
			{
				//Save as
				if (null != pszFilename && !FileName.Equals(pszFilename))
				{
					SetFileChangeNotification(FileName, false); //remove notification from old file
					SetFileChangeNotification(pszFilename, true); //add notification for new file
					FileName = pszFilename;     //cache the new file name
				}
				isDirty = false;
			}
			ITrackSelection track = GetITrackSelection();
			if (track != null) { hr = track.OnSelectChange(selectionContainer); }
			return hr;
		}

		/// <summary>
		/// Notifies the object that it has concluded the Save transaction
		/// </summary>
		/// <param name="pszFilename">Pointer to the file name</param>
		/// <returns>S_OK if the funtion succeeds</returns>
		int IPersistFileFormat.SaveCompleted(string pszFilename)
		{
			return VSConstants.S_OK;
		}
		#endregion

		#region 接口 Microsoft.VisualStudio.OLE.Interop.IPersist 的默认实现
		int Microsoft.VisualStudio.OLE.Interop.IPersist.GetClassID(out Guid pClassID)
		{
			pClassID = GetType().GUID;
			return VSConstants.S_OK;
		}
		#endregion

		#region 接口 IVsFileChangeEvents 的默认实现
		/// <summary>
		/// Notify the editor of the changes made to one or more files
		/// </summary>
		/// <param name="cChanges">Number of files that have changed</param>
		/// <param name="rgpszFile">array of the files names that have changed</param>
		/// <param name="rggrfChange">Array of the flags indicating the type of changes</param>
		/// <returns></returns>
		int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
		{
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Notify the editor of the changes made to one or more files
		/// </summary>
		/// <param name="cChanges">Number of files that have changed</param>
		/// <param name="rgpszFile">array of the files names that have changed</param>
		/// <param name="rggrfChange">Array of the flags indicating the type of changes</param>
		/// <returns></returns>
		int IVsFileChangeEvents.FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t**** Inside FilesChanged ****"));

			//check the different parameters
			if (0 == cChanges || null == rgpszFile || null == rggrfChange)
				return VSConstants.E_INVALIDARG;

			//ignore file changes if we are in that mode
			//for (uint i = 0; i < cChanges; i++)
			//{
			//    if (!String.IsNullOrEmpty(rgpszFile[i]) && String.Compare(rgpszFile[i], fileName, true, CultureInfo.CurrentCulture) == 0)
			//    {
			//        // if the readonly state (file attributes) have changed we can immediately update
			//        // the editor to match the new state (either readonly or not readonly) immediately
			//        // without prompting the user.
			//        if (0 != (rggrfChange[i] & (int)_VSFILECHANGEFLAGS.VSFILECHG_Attr))
			//        {
			//            FileAttributes fileAttrs = File.GetAttributes(fileName);
			//            int isReadOnly = (int)fileAttrs & (int)FileAttributes.ReadOnly;
			//        }
			//    }
			//}
			return VSConstants.S_OK;
		}
		#endregion

		#region 接口 IVsDocDataFileChangeControl 的默认实现
		/// <summary>
		/// Used to determine whether changes to DocData in files should be ignored or not
		/// </summary>
		/// <param name="fIgnore">a non zero value indicates that the file changes should be ignored
		/// </param>
		/// <returns></returns>
		int IVsDocDataFileChangeControl.IgnoreFileChanges(int fIgnore)
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t **** Inside IgnoreFileChanges ****"));
			if (fIgnore == 0)
			{
				// We need to check here if our file has changed from "Read Only"
				// to "Read/Write" or vice versa while the ignore level was non-zero.
				// This may happen when a file is checked in or out under source
				// code control. We need to check here so we can update our caption.
				//FileAttributes fileAttrs = File.GetAttributes(fileName);
				//int isReadOnly = (int)fileAttrs & (int)FileAttributes.ReadOnly;
				//SetReadOnly(isReadOnly != 0);
			}
			return VSConstants.S_OK;
		}
		#endregion

		#region File Change Notification Helpers
		/// <summary>
		/// Gets an instance of the RunningDocumentTable (RDT) service which manages the set of currently open 
		/// documents in the environment and then notifies the client that an open document has changed
		/// </summary>
		private void NotifyDocChanged()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			// Make sure that we have a file name
			if (FileName.Length == 0)
				return;

			// Get a reference to the Running Document Table
			IVsRunningDocumentTable runningDocTable = (IVsRunningDocumentTable)GetService(typeof(SVsRunningDocumentTable));

			// Lock the document
			int hr = runningDocTable.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, FileName, out _,
				 out _, out _, out uint docCookie);
			ErrorHandler.ThrowOnFailure(hr);

			// Send the notification
			hr = runningDocTable.NotifyDocumentChanged(docCookie, (uint)__VSRDTATTRIB.RDTA_DocDataReloaded);

			// Unlock the document.
			// Note that we have to unlock the document even if the previous call failed.
			ErrorHandler.ThrowOnFailure(runningDocTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, docCookie));

			// Check ff the call to NotifyDocChanged failed.
			ErrorHandler.ThrowOnFailure(hr);
		}

		/// <summary>
		/// In this function we inform the shell when we wish to receive 
		/// events when our file is changed or we inform the shell when 
		/// we wish not to receive events anymore.
		/// </summary>
		/// <param name="pszFileName">File name string</param>
		/// <param name="fStart">TRUE indicates advise, FALSE indicates unadvise.</param>
		/// <returns>Result of teh operation</returns>
		private int SetFileChangeNotification(string pszFileName, bool fStart)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t **** Inside SetFileChangeNotification ****"));

			int result = VSConstants.E_FAIL;

			//Get the File Change service
			if (null == vsFileChangeEx) { vsFileChangeEx = (IVsFileChangeEx)GetService(typeof(SVsFileChangeEx)); }
			if (null == vsFileChangeEx) { return VSConstants.E_UNEXPECTED; }

			// Setup Notification if fStart is TRUE, Remove if fStart is FALSE.
			if (fStart)
			{
				if (vsFileChangeCookie == VSConstants.VSCOOKIE_NIL)
				{
					//Receive notifications if either the attributes of the file change or 
					//if the size of the file changes or if the last modified time of the file changes
					uint grfFilter = (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Attr | _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time);
					result = vsFileChangeEx.AdviseFileChange(pszFileName, grfFilter, this, out vsFileChangeCookie);
					if (vsFileChangeCookie == VSConstants.VSCOOKIE_NIL)
						return VSConstants.E_FAIL;
				}
			}
			else
			{
				if (vsFileChangeCookie != VSConstants.VSCOOKIE_NIL)
				{
					result = vsFileChangeEx.UnadviseFileChange(vsFileChangeCookie);
					vsFileChangeCookie = VSConstants.VSCOOKIE_NIL;
				}
			}
			return result;
		}

		/// <summary>
		/// In this function we suspend receiving file change events for
		/// a file or we reinstate a previously suspended file depending
		/// on the value of the given fSuspend flag.
		/// </summary>
		/// <param name="pszFileName">File name string</param>
		/// <param name="fSuspend">TRUE indicates that the events needs to be suspended</param>
		/// <returns></returns>
		private int SuspendFileChangeNotification(string pszFileName, int fSuspend)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t **** Inside SuspendFileChangeNotification ****"));

			if (null == vsFileChangeEx)
				vsFileChangeEx = (IVsFileChangeEx)GetService(typeof(SVsFileChangeEx));
			if (null == vsFileChangeEx)
				return VSConstants.E_UNEXPECTED;

			if (0 == fSuspend)
			{
				// we are transitioning from suspended to non-suspended state - so force a
				// sync first to avoid asynchronous notifications of our own change
				if (vsFileChangeEx.SyncFile(pszFileName) == VSConstants.E_FAIL)
					return VSConstants.E_FAIL;
			}

			//If we use the VSCOOKIE parameter to specify the file, then pszMkDocument parameter 
			//must be set to a null reference and vice versa 
			return vsFileChangeEx.IgnoreFile(vsFileChangeCookie, null, fSuspend);
		}
		#endregion
	}
}
