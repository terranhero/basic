using EnvDTE;
using Basic.DataEntities;
using Basic.Designer;
using Basic.EntityLayer;
using BE = Basic.EntityLayer;
using Basic.Enums;
using Basic.Windows;
using Microsoft.CSharp;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Transactions;
using System.Xml;
using CD = System.CodeDom;
using Microsoft.VisualStudio.OLE.Interop;
using System.Globalization;
using System.Diagnostics;

namespace Basic.Configuration
{
	/// <summary>
	/// This control host the editor (an extended RichTextBox) and is responsible for
	/// handling the commands targeted to the editor as well as saving and loading
	/// the document. This control also implement the search and replace functionalities.
	/// IVsPersistDocData,  to Enable persistence functionality for document data
	/// IPersistFileFormat,  to enable the programmatic loading or saving of an object 
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD001", Justification = "<挂起>")]
	[System.Runtime.InteropServices.ComVisible(true)]
	public sealed class PersistentPane : WindowPane, IVsPersistDocData, IPersistFileFormat,
		IVsFileChangeEvents, IVsDocDataFileChangeControl, IWindowUICommands
	{
		#region Fields
		private bool loading = false;
		private bool isDirty;
		private string fileName = string.Empty;

		//private Microsoft.VisualStudio.Shell.Interop.ISelectionContainer selContainer;
		private ITrackSelection trackSelection;
		private IVsFileChangeEx vsFileChangeEx;
		private uint vsFileChangeCookie;

		private readonly PersistentService _CommandService;
		private const uint PersistentFormat = 0;
		private const string PersistentExtension = ".dpdl";
		private DesignerEntitiesCanvas editorControl;
		private readonly PersistentConfiguration _Persistent;
		private readonly AsyncPackage _PersistentPackage;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		private readonly IVsHierarchy vsHierarchy;
		private readonly ArrayList selectableList = new ArrayList();
		private readonly EnvDTE.DTE dteClass;
		private readonly string _FileName;
		public EnvDTE.ProjectItem ProjectItem { get { return _ProjectItem; } }
		public string FileName { get { return _FileName; } }

		/// <summary>Vs 属性窗口可选择对象</summary>
		private readonly Microsoft.VisualStudio.Shell.SelectionContainer selectionContainer;
		#endregion

		#region "Window.Pane Overrides"
		/// <summary>
		/// Constructor that calls the Microsoft.VisualStudio.Shell.WindowPane constructor then
		/// our initialization functions.
		/// </summary>
		/// <param name="package">Our Package instance.</param>
		public PersistentPane(PersistentPackage package, PersistentService service, IVsHierarchy pvHier, EnvDTE.ProjectItem item, string pszMkDocument)
			: base(package)
		{
			_CommandService = service;
			_FileName = pszMkDocument;
			vsHierarchy = pvHier;
			_ProjectItem = item;
			_PersistentPackage = package;
			dteClass = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
			_Persistent = new PersistentConfiguration();
			_Persistent.FileContentChanged += new EventHandler(OnPersistentContentChanged);
			_Persistent.CollectionChanged += new NotifyCollectionChangedEventHandler(OnPersistentCollectionChanged);
			editorControl = new DesignerEntitiesCanvas(this, _Persistent);
			selectionContainer = new Microsoft.VisualStudio.Shell.SelectionContainer(false, false);
			selectionContainer.SelectedObjectsChanged += new EventHandler(OnSelectionChanged);
			this.Content = editorControl;
		}

		/// <summary>
		/// returns an instance of the ITrackSelection service object
		/// </summary>
		private ITrackSelection TrackSelection
		{
			get
			{
				ThreadHelper.ThrowIfNotOnUIThread();
				if (trackSelection == null)
				{
					trackSelection = (ITrackSelection)GetService(typeof(ITrackSelection));
				}
				return trackSelection;
			}
		}

		/// <summary>
		/// This is an added command handler that will make it so the ITrackSelection.OnSelectChange
		/// function gets called whenever the cursor position is changed and also so the position 
		/// displayed on the status bar will update whenever the cursor position changes.
		/// </summary>
		/// <param name="sender"> Not used.</param>
		/// <param name="e"> Not used.</param>
		void OnSelectionChanged(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			// selContainer variables.
			ITrackSelection track = TrackSelection;
			if (null != track)
			{
				ErrorHandler.ThrowOnFailure(track.OnSelectChange(selectionContainer));
			}
		}

		private void OnPersistentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			selectableList.Clear();
			SetSelectableObjects(_Persistent.SetSelectedObjects(selectableList));
		}

		private void OnPersistentContentChanged(object sender, EventArgs e)
		{
			RaiseDocumentDataChange();
		}
		/// <summary>
		/// 当文档已更改时，调用此方法
		/// </summary>
		internal void RaiseDocumentDataChange()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (!loading)
			{
				isDirty = true;
				ITrackSelection track = TrackSelection;
				if (null != track) { track.OnSelectChange(selectionContainer); }
			}
		}
		#endregion

		#region IPersistFileFormat Members

		/// <summary>
		/// Notifies the object that it has concluded the Save transaction
		/// </summary>
		/// <param name="pszFilename">Pointer to the file name</param>
		/// <returns>S_OK if the funtion succeeds</returns>
		int IPersistFileFormat.SaveCompleted(string pszFilename)
		{
			// TODO:  Add Editor.SaveCompleted implementation
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
			// We only support 1 format so return its index
			pnFormatIndex = FileFormat;
			ppszFilename = fileName;
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
			if (nFormatIndex != FileFormat)
			{
				return VSConstants.E_INVALIDARG;
			}
			// until someone change the file, we can consider it not dirty as
			// the user would be annoyed if we prompt him to save an empty file
			isDirty = false;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Returns the class identifier of the editor type
		/// </summary>
		/// <param name="pClassID">pointer to the class identifier</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.GetClassID(out Guid pClassID)
		{
			ErrorHandler.ThrowOnFailure(((Microsoft.VisualStudio.OLE.Interop.IPersist)this).GetClassID(out pClassID));
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
			ppszFormatList = string.Format(CultureInfo.InvariantCulture, "{2} (*{0}){1}*{0}{1}{1}", FileExtension, Endline, FileFormatName);
			return VSConstants.S_OK;
		}

		/// <summary>
		/// 设置属性窗口选择对象
		/// </summary>
		/// <param name="listObjects"></param>
		public void SetSelectableObjects(ICollection listObjects)
		{
			selectionContainer.SelectableObjects = listObjects;
			ITrackSelection track = TrackSelection;
			if (null != track)
				track.OnSelectChange(selectionContainer);
		}

		/// <summary>
		/// 设置属性窗口选择对象
		/// </summary>
		/// <param name="listObjects"></param>
		public void SetSelectedObjects(ICollection listObjects)
		{
			selectionContainer.SelectedObjects = listObjects;
			ITrackSelection track = TrackSelection;
			if (null != track)
				track.OnSelectChange(selectionContainer);
		}

		/// <summary>
		/// Loads the file content into the textbox
		/// </summary>
		/// <param name="pszFilename">Pointer to the full path name of the file to load</param>
		/// <param name="grfMode">file format mode</param>
		/// <param name="fReadOnly">determines if teh file should be opened as read only</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
		{
			if (pszFilename == null)
			{
				return VSConstants.E_INVALIDARG;
			}

			try
			{
				loading = true;
				int result = LoadFile(pszFilename, grfMode, fReadOnly);
				isDirty = false;

				//Determine if the file is read only on the file system
				//FileAttributes fileAttrs = File.GetAttributes(pszFilename);

				//int isReadOnly = (int)fileAttrs & (int)FileAttributes.ReadOnly;

				////Set readonly if either the file is readonly for the user or on the file system
				//if (0 == isReadOnly && 0 == fReadOnly)
				//    SetReadOnly(false);
				//else
				//    SetReadOnly(true);


				// Notify to the property window that some of the selected objects are changed
				ITrackSelection track = TrackSelection;
				if (null != track)
				{
					int hr = track.OnSelectChange(selectionContainer);
					if (ErrorHandler.Failed(hr)) { return hr; }
				}

				// Hook up to file change notifications
				if (String.IsNullOrEmpty(fileName) || 0 != String.Compare(fileName, pszFilename, true, CultureInfo.CurrentCulture))
				{
					fileName = pszFilename;
					SetFileChangeNotification(pszFilename, true);

					// Notify the load or reload
					NotifyDocChanged();
				}
			}
			finally
			{
				loading = false;
			}
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Gets an instance of the RunningDocumentTable (RDT) service which manages the set of currently open 
		/// documents in the environment and then notifies the client that an open document has changed
		/// </summary>
		private void NotifyDocChanged()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			// Make sure that we have a file name
			if (fileName.Length == 0)
				return;

			// Get a reference to the Running Document Table
			IVsRunningDocumentTable runningDocTable = (IVsRunningDocumentTable)GetService(typeof(SVsRunningDocumentTable));

			// Lock the document
			int hr = runningDocTable.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, fileName,
				out _, out _, out _, out uint docCookie);
			ErrorHandler.ThrowOnFailure(hr);

			// Send the notification
			_ = ErrorHandler.ThrowOnFailure(runningDocTable.NotifyDocumentChanged(docCookie,
				(uint)__VSRDTATTRIB.RDTA_DocDataReloaded));

			// Unlock the document.
			// Note that we have to unlock the document even if the previous call failed.
			ErrorHandler.ThrowOnFailure(runningDocTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, docCookie));

			// Check ff the call to NotifyDocChanged failed.
		}


		/// <summary>
		/// Determines whether an object has changed since being saved to its current file
		/// </summary>
		/// <param name="pfIsDirty">true if the document has changed</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.IsDirty(out int pfIsDirty)
		{
			if (isDirty)
			{
				pfIsDirty = 1;
			}
			else
			{
				pfIsDirty = 0;
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
		int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
		{
			int hr = VSConstants.S_OK;
			bool doingSaveOnSameFile = false;
			// If file is null or same --> SAVE
			if (pszFilename == null || pszFilename == fileName)
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
				SaveFile(pszFilename, fRemember, nFormatIndex);
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
				if (null != pszFilename && !fileName.Equals(pszFilename))
				{
					SetFileChangeNotification(fileName, false); //remove notification from old file
					SetFileChangeNotification(pszFilename, true); //add notification for new file
					fileName = pszFilename;     //cache the new file name
				}
				isDirty = false;
				SetReadOnly(false);             //set read only to false since you were successfully able
												//to save to the new file                                                    
			}

			ITrackSelection track = TrackSelection;
			if (null != track)
			{
				hr = track.OnSelectChange(selectionContainer);
			}

			// Since all changes are now saved properly to disk, there's no need for a backup.
			return hr;
		}

		/// <summary>
		/// Used to ReadOnly property for the Rich TextBox and correspondingly update the editor caption
		/// </summary>
		/// <param name="_isFileReadOnly">Indicates whether the file loaded is Read Only or not</param>
		private void SetReadOnly(bool _isFileReadOnly)
		{
			//this.editorControl.SetReadOnly(_isFileReadOnly);
			IVsWindowFrame frame = (IVsWindowFrame)GetService(typeof(SVsWindowFrame));
			string editorCaption = "";
			if (_isFileReadOnly)
				editorCaption = this.GetResourceString("@100");
			ErrorHandler.ThrowOnFailure(frame.SetProperty((int)__VSFPROPID.VSFPROPID_EditorCaption, editorCaption));
		}
		/// <summary>
		/// This method loads a localized string based on the specified resource.
		/// </summary>
		/// <param name="resourceName">Resource to load</param>
		/// <returns>String loaded for the specified resource</returns>
		internal string GetResourceString(string resourceName)
		{
			string resourceValue;
			IVsResourceManager resourceManager = (IVsResourceManager)GetService(typeof(SVsResourceManager));
			if (resourceManager == null)
			{
				throw new InvalidOperationException("Could not get SVsResourceManager service. Make sure the package is Sited before calling this method");
			}
			Guid packageGuid = _PersistentPackage.GetType().GUID;
			int hr = resourceManager.LoadResourceString(ref packageGuid, -1, resourceName, out resourceValue);
			ErrorHandler.ThrowOnFailure(hr);
			return resourceValue;
		}
		#endregion

		#region IVsPersistDocData Members

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
			pbstrMkDocumentNew = null;
			pfSaveCanceled = 0;
			int hr = VSConstants.S_OK;
			if (!isDirty) { return VSConstants.S_OK; }
			switch (dwSave)
			{
				case VSSAVEFLAGS.VSSAVE_Save:
				case VSSAVEFLAGS.VSSAVE_SilentSave:
					{
						IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2)GetService(typeof(SVsQueryEditQuerySave));

						// Call QueryEditQuerySave
						uint result = 0;
						hr = queryEditQuerySave.QuerySaveFile(fileName, 0, null, out result);
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
									hr = uiShell.SaveDocDataToFile(dwSave, (IPersistFileFormat)this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
									if (ErrorHandler.Failed(hr))
										return hr;
								}
								break;

							case tagVSQuerySaveResult.QSR_ForceSaveAs:
								{
									// Call the shell to do the SaveAS for us
									IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
									hr = uiShell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SaveAs, (IPersistFileFormat)this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
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
					}
				case VSSAVEFLAGS.VSSAVE_SaveAs:
				case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
					{
						// Make sure the file name as the right extension
						if (String.Compare(FileExtension, System.IO.Path.GetExtension(fileName), true, CultureInfo.CurrentCulture) != 0)
						{
							fileName += FileExtension;
						}
						// Call the shell to do the save for us
						IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
						hr = uiShell.SaveDocDataToFile(dwSave, (IPersistFileFormat)this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
						if (ErrorHandler.Failed(hr))
							return hr;
						break;
					}
				default:
					throw new ArgumentException("Unsupported Save flag");
			};

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Loads the document data from the file specified
		/// </summary>
		/// <param name="pszMkDocument">Path to the document file which needs to be loaded</param>
		/// <returns>S_Ok if the method succeeds</returns>
		int IVsPersistDocData.LoadDocData(string pszMkDocument)
		{
			return ((IPersistFileFormat)this).Load(pszMkDocument, 0, 0);
		}

		/// <summary>
		/// Used to set the initial name for unsaved, newly created document data
		/// </summary>
		/// <param name="pszDocDataPath">String containing the path to the document. We need to ignore this parameter
		/// </param>
		/// <returns>S_OK if the mthod succeeds</returns>
		int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
		{
			return ((IPersistFileFormat)this).InitNew(FileFormat);
		}

		/// <summary>
		/// Returns the Guid of the editor factory that created the IVsPersistDocData object
		/// </summary>
		/// <param name="pClassID">Pointer to the class identifier of the editor type</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
		{
			return ((IPersistFileFormat)this).GetClassID(out pClassID);
		}

		/// <summary>
		/// Close the IVsPersistDocData object
		/// </summary>
		/// <returns>S_OK if the function succeeds</returns>
		int IVsPersistDocData.Close()
		{
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Determines if it is possible to reload the document data
		/// </summary>
		/// <param name="pfReloadable">set to 1 if the document can be reloaded</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
		{
			// Allow file to be reloaded
			pfReloadable = 1;
			return VSConstants.S_OK;
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
			// TODO:  Add EditorPane.RenameDocData implementation
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
			return LoadFile(fileName, grfFlags, 0);
			//return ((IPersistFileFormat)this).Load(fileName, grfFlags, 0);
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
			//Nothing to do here
			return VSConstants.S_OK;
		}

		#endregion

		#region IVsFileChangeEvents Members

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
			for (uint i = 0; i < cChanges; i++)
			{
				if (!String.IsNullOrEmpty(rgpszFile[i]) && String.Compare(rgpszFile[i], fileName, true, CultureInfo.CurrentCulture) == 0)
				{
					// if the readonly state (file attributes) have changed we can immediately update
					// the editor to match the new state (either readonly or not readonly) immediately
					// without prompting the user.
					if (0 != (rggrfChange[i] & (int)_VSFILECHANGEFLAGS.VSFILECHG_Attr))
					{
						FileAttributes fileAttrs = File.GetAttributes(fileName);
						int isReadOnly = (int)fileAttrs & (int)FileAttributes.ReadOnly;
						SetReadOnly(isReadOnly != 0);
					}
				}
			}
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Notify the editor of the changes made to a directory
		/// </summary>
		/// <param name="pszDirectory">Name of the directory that has changed</param>
		/// <returns></returns>
		int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
		{
			//Nothing to do here
			return VSConstants.S_OK;
		}
		#endregion

		#region IVsDocDataFileChangeControl Members

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
				FileAttributes fileAttrs = File.GetAttributes(fileName);
				int isReadOnly = (int)fileAttrs & (int)FileAttributes.ReadOnly;
				SetReadOnly(isReadOnly != 0);
			}
			return VSConstants.S_OK;
		}
		#endregion

		#region File Change Notification Helpers

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
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t **** Inside SetFileChangeNotification ****"));

			int result = VSConstants.E_FAIL;

			//Get the File Change service
			if (null == vsFileChangeEx)
				vsFileChangeEx = (IVsFileChangeEx)GetService(typeof(SVsFileChangeEx));
			if (null == vsFileChangeEx)
				return VSConstants.E_UNEXPECTED;

			// Setup Notification if fStart is TRUE, Remove if fStart is FALSE.
			if (fStart)
			{
				if (vsFileChangeCookie == VSConstants.VSCOOKIE_NIL)
				{
					//Receive notifications if either the attributes of the file change or 
					//if the size of the file changes or if the last modified time of the file changes
					uint grfFilter = (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Attr | _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time);
					result = vsFileChangeEx.AdviseFileChange(pszFileName, grfFilter, (IVsFileChangeEvents)this, out vsFileChangeCookie);
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

		#region EditCode 命令执行和状态查询
		internal CodeDomProvider GetCodeDomProvider()
		{
			if (GetService(typeof(SVSMDCodeDomProvider)) is IVSMDCodeDomProvider provider)
			{
				CodeDomProvider codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
				if (codeDomProvider == null) { codeDomProvider = new CSharpCodeProvider(); }
				else
				{
					string language = CodeDomProvider.GetLanguageFromExtension(codeDomProvider.FileExtension);
					codeDomProvider = CodeDomProvider.CreateProvider(language);
				}
				return codeDomProvider;
			}
			return new CSharpCodeProvider();
		}

		internal void EditCommandCode(DataCommandElement dataCommand, DataEntityElement entity)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (dataCommand.Kind != ConfigurationTypeEnum.Other && dataCommand.Kind != ConfigurationTypeEnum.SearchTable) { return; }
			CodeDomProvider codeDomProvider = GetCodeDomProvider();
			FileInfo dpdlFileInfo = new FileInfo(_FileName);
			string contextName = _Persistent.ContextName;
			string contextFullName = string.Concat(dpdlFileInfo.DirectoryName, @"\", contextName, ".", codeDomProvider.FileExtension);
			ProjectItem projectItem = dteClass.Solution.FindProjectItem(contextFullName);
			if (projectItem == null) { return; }
			EnvDTE80.CodeClass2 codeClass = projectItem.FileCodeModel.CodeElements.FindCodeClass(_Persistent.ContextName);
			if (codeClass != null)
			{
				EditPoint movePoint = dataCommand.WriteContextCode(codeClass, codeDomProvider, entity, _Persistent);
				if (codeClass.ProjectItem.IsOpen == false) { codeClass.ProjectItem.Open(); }
				codeClass.ProjectItem.Document.Activate();
				TextSelection textSelection = (TextSelection)codeClass.ProjectItem.Document.Selection;
				textSelection.MoveToPoint(movePoint);
			}
		}

		internal void EditDataEntityCode(DataEntityElement entity, DataEntityPropertyElement property)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IVSMDCodeDomProvider provider = (IVSMDCodeDomProvider)GetService(typeof(SVSMDCodeDomProvider));
			if (provider != null)
			{
				CodeDomProvider codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
				if (codeDomProvider == null) { codeDomProvider = new CSharpCodeProvider(); }
				else
				{
					string language = CodeDomProvider.GetLanguageFromExtension(codeDomProvider.FileExtension);
					codeDomProvider = CodeDomProvider.CreateProvider(language);
				}
				ProjectItem entityItem = GetEntityProjectItem();
				if (entityItem == null) { return; }
				FileCodeModel codeModel = entityItem.FileCodeModel;
				CodeNamespace codeNs = codeModel.CodeElements.FindNamespace(_Persistent.EntityNamespace);
				if (codeNs == null) { return; }
				CodeClass codeClass = codeNs.Members.FindCodeClass(entity.ClassName);
				if (codeClass == null)
				{
					CodeGeneratorOptions options = new CodeGeneratorOptions();
					options.BlankLinesBetweenMembers = true;
					options.BracingStyle = "C";// "C";
					System.Text.StringBuilder textBuilder = new System.Text.StringBuilder(1500);
					using (StringWriter writer = new StringWriter(textBuilder))
					{
						CD.CodeNamespace codeNamespace = new CD.CodeNamespace();
						if (entity.GeneratorMode == GenerateModeEnum.DataEntity)
						{
							CD.CodeTypeDeclaration entityType = entity.WriteEntityCode(codeNamespace);
							codeDomProvider.GenerateCodeFromType(entityType, writer, options);
						}
						else if (entity.GeneratorMode == GenerateModeEnum.DataTable)
						{
							CD.CodeTypeDeclaration entityType = entity.WriteTableCode(codeNamespace);
							codeDomProvider.GenerateCodeFromType(entityType, writer, options);
						}
						else
						{
							CD.CodeTypeDeclaration entityType = entity.WriteEntityCode(codeNamespace);
							entity.WriteTableCode(codeNamespace);
							codeDomProvider.GenerateCodeFromType(entityType, writer, options);
							CD.CodeTypeDeclaration tableType = entity.WriteTableCode(codeNamespace);
							codeDomProvider.GenerateCodeFromType(tableType, writer, options);
						}
					}
					vsCMAccess access = vsCMAccess.vsCMAccessPublic;
					codeClass = codeNs.AddClass(string.Concat("F", BE.GuidConverter.NewGuid.ToString("N")), -1, null, null, access);
					EditPoint editPoint = codeClass.StartPoint.CreateEditPoint();
					EditPoint movePoint = codeClass.EndPoint.CreateEditPoint();
					editPoint.StartOfLine(); movePoint.EndOfLine();
					editPoint.ReplaceText(movePoint, textBuilder.ToString(), (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
					if (!entityItem.IsOpen) { entityItem.Open(); }
					entityItem.Save(); entityItem.Document.Activate();
					TextSelection textSelection1 = (TextSelection)entityItem.Document.Selection;
					textSelection1.MoveToPoint(editPoint);
					return;
				}
				if (property != null)
				{
					CodeClass metadataClass = codeClass.Members.FindCodeClass(entity.MetadataName);
					if (metadataClass == null) { return; }
					CodeProperty codeProperty = property.WritePropertyCode(metadataClass, codeDomProvider);
					if (codeProperty == null)
					{
						EditPoint editPoint = codeProperty.StartPoint.CreateEditPoint();
						if (!entityItem.IsOpen) { entityItem.Open(); }
						entityItem.Save(); entityItem.Document.Activate();
						TextSelection textSelection1 = (TextSelection)entityItem.Document.Selection;
						textSelection1.MoveToPoint(editPoint);
						return;
					}
					EditPoint propertyPoint = codeProperty.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
					if (!entityItem.IsOpen) { entityItem.Open(); }
					entityItem.Document.Activate();
					TextSelection propertySelection = (TextSelection)entityItem.Document.Selection;
					propertySelection.MoveToPoint(propertyPoint);
					return;
				}
				EditPoint classPoint = codeClass.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
				if (!entityItem.IsOpen) { entityItem.Open(); }
				entityItem.Document.Activate();
				TextSelection classSelection = (TextSelection)entityItem.Document.Selection;
				classSelection.MoveToPoint(classPoint);
			}
		}

		internal void EditConditionCode(DataEntityElement entity, DataConditionPropertyElement property)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IVSMDCodeDomProvider provider = (IVSMDCodeDomProvider)GetService(typeof(SVSMDCodeDomProvider));
			if (provider != null)
			{
				CodeDomProvider codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
				if (codeDomProvider == null) { codeDomProvider = new CSharpCodeProvider(); }
				else
				{
					string language = CodeDomProvider.GetLanguageFromExtension(codeDomProvider.FileExtension);
					codeDomProvider = CodeDomProvider.CreateProvider(language);
				}

				ProjectItem entityItem = GetEntityProjectItem();
				if (entityItem == null) { return; }
				FileCodeModel codeModel = entityItem.FileCodeModel;
				CodeNamespace codeNs = codeModel.CodeElements.FindNamespace(_Persistent.EntityNamespace);
				if (codeNs == null) { return; }
				CodeClass codeClass = codeNs.Members.FindCodeClass(entity.Condition.EntityName);
				if (codeClass == null)
				{
					CodeGeneratorOptions options = new CodeGeneratorOptions();
					options.BlankLinesBetweenMembers = true;
					options.BracingStyle = "C";// "C";
					System.Text.StringBuilder textBuilder = new System.Text.StringBuilder(1500);
					using (StringWriter writer = new StringWriter(textBuilder))
					{
						CD.CodeNamespace codeNamespace = new CD.CodeNamespace();
						CD.CodeTypeDeclaration entityType = entity.Condition.WriteEntityCode(codeNamespace);
						codeDomProvider.GenerateCodeFromType(entityType, writer, options);
					}
					vsCMAccess access = vsCMAccess.vsCMAccessPublic;
					codeClass = codeNs.AddClass(string.Concat("F", BE.GuidConverter.NewGuid.ToString("N")), -1, null, null, access);
					EditPoint editPoint = codeClass.StartPoint.CreateEditPoint();
					EditPoint movePoint = codeClass.EndPoint.CreateEditPoint();
					editPoint.StartOfLine(); movePoint.EndOfLine();
					editPoint.ReplaceText(movePoint, textBuilder.ToString(), (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
					if (!entityItem.IsOpen) { entityItem.Open(); }
					entityItem.Save(); entityItem.Document.Activate();
					TextSelection textSelection1 = (TextSelection)entityItem.Document.Selection;
					textSelection1.MoveToPoint(editPoint);
					return;
				}
				if (property != null)
				{
					CodeClass metadataClass = codeClass.Members.FindCodeClass(entity.Condition.MetadataName);
					CodeProperty codeProperty = property.WritePropertyCode(metadataClass, codeDomProvider);
					if (codeProperty == null)
					{
						EditPoint editPoint = codeProperty.StartPoint.CreateEditPoint();
						if (!entityItem.IsOpen) { entityItem.Open(); }
						entityItem.Save(); entityItem.Document.Activate();
						TextSelection textSelection1 = (TextSelection)entityItem.Document.Selection;
						textSelection1.MoveToPoint(editPoint);
						return;
					}
					EditPoint propertyPoint = codeProperty.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
					if (!entityItem.IsOpen) { entityItem.Open(); }
					entityItem.Document.Activate();
					TextSelection propertySelection = (TextSelection)entityItem.Document.Selection;
					propertySelection.MoveToPoint(propertyPoint);
					return;
				}
				EditPoint classPoint = codeClass.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
				if (!entityItem.IsOpen) { entityItem.Open(); }
				entityItem.Document.Activate();
				TextSelection classSelection = (TextSelection)entityItem.Document.Selection;
				classSelection.MoveToPoint(classPoint);
			}
		}
		#endregion

		public PersistentConfiguration GetPersistent() { return _Persistent; }

		private uint FileFormat
		{
			get { return PersistentFormat; }
		}

		private string FileFormatName
		{
			get { return "Data Persistent Class"; }
		}

		private string FileExtension
		{
			get { return PersistentExtension; }
		}

		private void DisposePane()
		{
			if (editorControl != null)
				editorControl = null;
		}

		internal EnvDTE.ProjectItem GetEntityProjectItem()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			CodeDomProvider provider = GetCodeProvider();
			string entityName = string.Concat(_Persistent.EntityName, ".", provider.FileExtension);
			if (_Persistent.Project.NotEmpty)
			{
				Guid guid = _Persistent.ProjectGuid;
				IVsSolution vsSolution = (IVsSolution)GetService(typeof(SVsSolution));
				IVsHierarchy hierarchy; object outProject = null;
				vsSolution.GetProjectOfGuid(ref guid, out hierarchy);
				uint itemId = (uint)VSConstants.VSITEMID.Root;
				if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out outProject) >= 0)
				{
					EnvDTE.Project project = (EnvDTE.Project)outProject;
					string folderName = _Persistent.EntityFolder;
					FileInfo projectInfo = new FileInfo(project.FullName);
					string entityFullName = string.Concat(projectInfo.DirectoryName, "\\", folderName, "\\", entityName);
					return dteClass.Solution.FindProjectItem(entityFullName);
				}
			}
			EnvDTE.ProjectItem dpdlItem = GetProjectItem();
			if (dpdlItem == null) { return null; }
			foreach (EnvDTE.ProjectItem item in dpdlItem.ProjectItems)
			{
				if (item.ProjectItems.Count > 0)
				{
					foreach (EnvDTE.ProjectItem childItem in item.ProjectItems)
					{
						if (childItem.Name == entityName)
							return childItem;
					}
				}
				if (item.Name == entityName)
					return item;
			}
			return null;
		}
		/// <summary>
		/// Returns the EnvDTE.ProjectItem object that corresponds to the project item the code 
		/// generator was called on
		/// </summary>
		/// <returns>The EnvDTE.ProjectItem of the project item the code generator was called on</returns>
		internal ProjectItem GetProjectItem()
		{
			object p = GetService(typeof(ProjectItem));
			Debug.Assert(p != null, "Unable to get Project Item.");
			return (ProjectItem)p;
		}
		internal EnvDTE.ProjectItem GetAccessProjectItem()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			CodeDomProvider provider = GetCodeProvider();
			string accessName = string.Concat(_Persistent.AccessName, ".", provider.FileExtension);
			EnvDTE.ProjectItem dpdlItem = GetProjectItem();
			if (dpdlItem == null) { return null; }
			foreach (EnvDTE.ProjectItem item in dpdlItem.ProjectItems)
			{
				if (item.ProjectItems.Count > 0)
				{
					foreach (EnvDTE.ProjectItem childItem in item.ProjectItems)
					{
						if (childItem.Name == accessName)
							return childItem;
					}
				}
				if (item.Name == accessName)
					return item;
			}
			return null;
		}

		internal EnvDTE.ProjectItem GetContextProjectItem()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			CodeDomProvider provider = GetCodeProvider();
			string contextName = string.Concat(_Persistent.ContextName, ".", provider.FileExtension);
			EnvDTE.ProjectItem dpdlItem = GetProjectItem();
			if (dpdlItem == null) { return null; }
			foreach (EnvDTE.ProjectItem item in dpdlItem.ProjectItems)
			{
				if (item.ProjectItems.Count > 0)
				{
					foreach (EnvDTE.ProjectItem childItem in item.ProjectItems)
					{
						if (childItem.Name == contextName)
							return childItem;
					}
				}
				if (item.Name == contextName)
					return item;
			}
			return null;
		}

		private CodeDomProvider codeDomProvider = null;
		/// <summary>
		/// Returns a CodeDomProvider object for the language of the project containing
		/// the project item the generator was called on
		/// </summary>
		/// <returns>A CodeDomProvider object</returns>
		private CodeDomProvider GetCodeProvider()
		{
			if (codeDomProvider == null)
			{
				//Query for IVSMDCodeDomProvider/SVSMDCodeDomProvider for this project type
				IVSMDCodeDomProvider provider = GetService(typeof(SVSMDCodeDomProvider)) as IVSMDCodeDomProvider;
				if (provider != null)
				{
					codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
				}
				else
				{
					//In the case where no language specific CodeDom is available, fall back to C#
					codeDomProvider = CodeDomProvider.CreateProvider("C#");
				}
			}
			return codeDomProvider;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pszFilename"></param>
		/// <param name="grfMode"></param>
		/// <param name="fReadOnly"></param>
		private int LoadFile(string pszFilename, uint grfMode, int fReadOnly)
		{
			try
			{
				using (StreamReader reader = new StreamReader(pszFilename))
				{
					_Persistent.ClearContent();
					_Persistent.ReadXml(reader);
					//Guid guid = persistent.ProjectGuid;
					//IVsSolution vsSolution = (IVsSolution)base.GetService(typeof(SVsSolution));
					//if (guid != Guid.Empty)
					//{
					//    IVsHierarchy hierarchy; object outProject = null;
					//    vsSolution.GetProjectOfGuid(ref guid, out hierarchy);
					//    uint itemId = (uint)VSConstants.VSITEMID.Root;
					//    if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out outProject) >= 0)
					//        persistent.Project = new ProjectInfo(guid, (EnvDTE.Project)outProject);
					//}
				}
				return VSConstants.S_OK;
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message);
				return VSConstants.S_FALSE;
			}
		}

		/// <summary>
		/// 判断实体类命名空间和类名称是否有变化，如果有变化则在自定义代码文件中修改。
		/// </summary>
		private void ChangeEntityContent()
		{
			EnvDTE.ProjectItem entityItem = GetEntityProjectItem();
			if (entityItem == null) { return; }
			if (!entityItem.IsOpen) { entityItem.Open(); }
			string oldNamespace = _Persistent.EntityOldNamespace;
			CodeElements elements = entityItem.FileCodeModel.CodeElements;
			CodeNamespace codeNamespace = elements.FindNamespace(oldNamespace);
			if (codeNamespace == null)
				codeNamespace = elements.FindNamespace(_Persistent.EntityNamespace);
			if (codeNamespace == null) { return; }
			if (_Persistent.EntityNamespaceChanged)
				codeNamespace.Name = _Persistent.EntityNamespace;
			foreach (DataEntityElement entity in _Persistent.DataEntities)
			{
				if (entity.NameChanged)
				{
					CodeClass codeClass = codeNamespace.Members.FindCodeClass(entity.OldEntityName);
					if (codeClass != null) { codeClass.Name = entity.EntityName; }

					codeClass = codeNamespace.Members.FindCodeClass(entity.Condition.OldEntityName);
					if (codeClass != null) { codeClass.Name = entity.Condition.EntityName; }

					codeClass = codeNamespace.Members.FindCodeClass(entity.OldDataTableName);
					if (codeClass != null) { codeClass.Name = entity.DataTableName; }

					codeClass = codeNamespace.Members.FindCodeClass(entity.OldDataRowName);
					if (codeClass != null) { codeClass.Name = entity.DataRowName; }
				}
			}
			entityItem.Save();
		}

		private void ChangeAccessContent()
		{
			if (_Persistent.NamespaceChanged)
			{
				EnvDTE.ProjectItem accessItem = GetAccessProjectItem();
				if (accessItem != null)
				{
					if (!accessItem.IsOpen) { accessItem.Open(); }
					CodeNamespace codeNs = accessItem.FileCodeModel.CodeElements.FindNamespace(_Persistent.OldNamespace);
					if (codeNs != null)
					{
						codeNs.Name = _Persistent.Namespace;
						accessItem.Save();
					}
				}
			}
		}

		private void ChangeContextContent()
		{
			if (_Persistent.NamespaceChanged)
			{
				EnvDTE.ProjectItem contextItem = GetContextProjectItem();
				if (contextItem != null)
				{
					if (!contextItem.IsOpen) { contextItem.Open(); }
					CodeNamespace codeNs = contextItem.FileCodeModel.CodeElements.FindNamespace(_Persistent.OldNamespace);
					if (codeNs != null)
					{
						codeNs.Name = _Persistent.Namespace;
						contextItem.Save();
					}
				}
			}
		}

		/// <summary>
		/// 显示快捷菜单
		/// </summary>
		/// <param name="x">菜单显示横坐标</param>
		/// <param name="y">菜单显示纵坐标</param>
		public void ShowContextMenu(int x, int y)
		{
			_CommandService.ShowContextMenu(x, y);
		}

		/// <summary>
		/// 保存当前文件
		/// </summary>
		/// <param name="pszFilename">保存文件路径</param>
		/// <param name="fRemember"></param>
		/// <param name="nFormatIndex"></param>
		/// <returns></returns>
		private int SaveFile(string pszFilename, int fRemember, uint nFormatIndex)
		{
			try
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.IndentChars = "\t";
				string oldEntityNs = _Persistent.EntityOldNamespace;
				bool entityNsChanged = _Persistent.EntityNamespaceChanged;

				string oldNamespace = _Persistent.OldNamespace;
				bool nsChanged = _Persistent.NamespaceChanged;
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					using (XmlWriter writer = XmlWriter.Create(pszFilename, settings))
					{
						_Persistent.WriteXml(writer);
					}
					ChangeEntityContent();
					ChangeAccessContent();
					ChangeContextContent();
				}
				return VSConstants.S_OK;
			}
			catch (Exception ex)
			{
				ShowMessage(string.Concat(ex.Message, "\r\n", ex.StackTrace));
				return VSConstants.S_FALSE;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void ShowProperty()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IVsUIShell globalService = (IVsUIShell)GetService(typeof(SVsUIShell));
			IVsWindowFrame ppWindowFrame = null;
			Guid guid = new Guid("{EEFA5220-E298-11D0-8F78-00A0C9110057}");
			globalService.FindToolWindow(0x80000, ref guid, out ppWindowFrame);
			ppWindowFrame.Show();
		}

		/// <summary>
		/// 显示对话框消息
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		public void ShowMessage(string message, string title = "Basic.DataPersistent")
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IVsUIShell iVsUiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
			int result = 0; Guid tempGuid = Guid.Empty;
			if (iVsUiShell != null)
			{
				iVsUiShell.ShowMessageBox(0, ref tempGuid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 0, out result);
			}
		}

		/// <summary>
		/// 显示对话框消息
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		public void ShowMessage(string message)
		{
			this.ShowMessage(message, "Basic.DataPersistent");
		}

		/// <summary>
		/// 显示对话框消息
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		public int Confirm(string message)
		{
			return this.Confirm(message, "Basic.DataPersistent");
		}

		/// <summary>
		/// 显示对话框消息
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		public int Confirm(string message, string title = "Basic.DataPersistent")
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IVsUIShell iVsUiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
			int result = 0; Guid tempGuid = Guid.Empty;
			if (iVsUiShell != null)
			{
				iVsUiShell.ShowMessageBox(0, ref tempGuid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_QUERY, 0, out result);
			}
			return result;
		}
		int IPersist.GetClassID(out Guid pClassID)
		{
			throw new NotImplementedException();
		}
	}
}
