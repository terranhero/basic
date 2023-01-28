using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows;
using System.Xml;
using Basic.Designer;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectItem = EnvDTE.ProjectItem;
using STT = System.Threading.Tasks;

namespace Basic.Localizations
{
	public sealed partial class LocalizationService
	{
		#region 模型命令
		/// <summary>快捷菜单Guid字符串(提取枚举资源)</summary>
		private const string ContextMenuGuidString = "{C12A8C09-CA08-4D26-B859-CF061F7F178E}";

		/// <summary>
		/// 表示数据持久类菜单标识符。
		/// </summary>
		private static readonly Guid guidContextMenu = new Guid(ContextMenuGuidString);

		/// <summary>数据持久类快捷菜单命令ID</summary>
		private static readonly CommandID ContextMenuID = new CommandID(guidContextMenu, 0x2000);

		/// <summary>插入</summary>
		private static readonly CommandID IDB_INSERTITEM = new CommandID(guidContextMenu, 0x2031);
		private static readonly CommandID IDB_APPENDITEM = new CommandID(guidContextMenu, 0x2032);

		private static readonly CommandID IDB_GROUP = new CommandID(guidContextMenu, 0x2041);
		private static readonly CommandID IDB_UNGROUP = new CommandID(guidContextMenu, 0x2042);

		/// <summary>资源添加</summary>
		private const string strAddCultures = "{8ABB68B0-55A2-492A-8842-374099D6D94F}";
		private static readonly Guid guidAddCultures = new Guid(strAddCultures);


		/// <summary>资源删除</summary>
		private const string strRemoveCultures = "{DAD0B384-F058-4962-83DC-C139B757F2F0}";
		private static readonly Guid guidRemoveCultures = new Guid(strRemoveCultures);

		/// <summary>添加区域资源</summary>
		//private static readonly CommandID IDM_ADD_CULTURES = new CommandID(guidContextMenu, 0x2011);
		private static readonly CommandID IDB_ADD_CULTURE_ZH_CN = new CommandID(guidAddCultures, 0x0804);
		private static readonly CommandID IDB_ADD_CULTURE_EN_US = new CommandID(guidAddCultures, 0x0409);
		private static readonly CommandID IDB_ADD_CULTURE_ZH_HANT = new CommandID(guidAddCultures, 0x2026);
		private static readonly CommandID IDB_ADD_CULTURE_ZH_HANS = new CommandID(guidAddCultures, 0x0004);

		/// <summary>删除区域资源</summary>
		//private static readonly CommandID IDM_REMOVE_CULTURES = new CommandID(guidContextMenu, 0x2011);
		private static readonly CommandID IDB_REMOVE_CULTURE_ZH_CN = new CommandID(guidRemoveCultures, 0x0804);
		private static readonly CommandID IDB_REMOVE_CULTURE_EN_US = new CommandID(guidRemoveCultures, 0x0409);
		private static readonly CommandID IDB_REMOVE_CULTURE_ZH_HANT = new CommandID(guidRemoveCultures, 0x2026);
		private static readonly CommandID IDB_REMOVE_CULTURE_ZH_HANS = new CommandID(guidRemoveCultures, 0x0004);

		#endregion

		/// <summary>显示快捷菜单</summary>
		/// <param name="x">菜单显示横坐标</param>
		/// <param name="y">菜单显示纵坐标</param>
		internal void ShowContextMenu(int x, int y)
		{
			if (null != oleMenuService) { oleMenuService.ShowContextMenu(ContextMenuID, x, y); }
		}

		private IVsMonitorSelection monitorSelection;


		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="omc">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		private async STT.Task InitializeContextMenuAsync(OleMenuCommandService omc, IProgress<ServiceProgressData> progress)
		{
			monitorSelection = await asyncServiceProvider.GetServiceAsync(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
			Microsoft.Assumes.Present(monitorSelection);
			progress.Report(new ServiceProgressData("正在加载快捷菜单......"));
			if (null != omc)
			{
				omc.AddCommand(IDB_ADD_CULTURE_ZH_CN, "zh-CN", OnAddCultureExecuted, OnCanAddExecuted);
				omc.AddCommand(IDB_ADD_CULTURE_EN_US, "en-US", OnAddCultureExecuted, OnCanAddExecuted);
				//omc.AddCommand(IDB_ADD_CULTURE_ZH_TW, "zh-TW", OnAddCultureExecuted, OnCanAddExecuted);
				omc.AddCommand(IDB_ADD_CULTURE_ZH_HANT, "zh-Hant", OnAddCultureExecuted, OnCanAddExecuted);
				omc.AddCommand(IDB_ADD_CULTURE_ZH_HANS, "zh-Hans", OnAddCultureExecuted, OnCanAddExecuted);

				omc.AddCommand(IDB_REMOVE_CULTURE_ZH_CN, "zh-CN", OnRemoveCultureExecuted, OnCanRemoveExecuted);
				omc.AddCommand(IDB_REMOVE_CULTURE_EN_US, "en-US", OnRemoveCultureExecuted, OnCanRemoveExecuted);
				//omc.AddCommand(IDB_REMOVE_CULTURE_ZH_TW, "zh-TW", OnRemoveCultureExecuted, OnCanRemoveExecuted);
				omc.AddCommand(IDB_REMOVE_CULTURE_ZH_HANT, "zh-Hant", OnRemoveCultureExecuted, OnCanRemoveExecuted);
				omc.AddCommand(IDB_REMOVE_CULTURE_ZH_HANS, "zh-Hans", OnRemoveCultureExecuted, OnCanRemoveExecuted);

				omc.AddCommand(IDB_INSERTITEM, OnInsertExecuted, OnCanInsertExecuted);
				omc.AddCommand(IDB_APPENDITEM, OnAppendExecuted);

				omc.AddCommand(IDB_GROUP, OnGroupExecuted, OnCanGroupExecuted);
				omc.AddCommand(IDB_UNGROUP, OnUnGroupExecuted, OnCanUnGroupExecuted);

				omc.AddCommand(StandardCommands.Cut, OnCutExecuted, OnCanCutExecuted);
				omc.AddCommand(StandardCommands.Copy, OnCopyExecuted, OnCanCopyExecuted);
				omc.AddCommand(StandardCommands.Paste, OnPasteExecuted, OnCanPasteExecuted);
				omc.AddCommand(StandardCommands.Delete, OnDeleteExecuted, OnCanDeleteExecuted);
				omc.AddCommand(StandardCommands.Properties, OnPropertyWindow);
			}
		}

		#region 添加语言资源
		[SuppressMessage("Usage", "VSTHRD100:避免使用 Async Void 方法", Justification = "<挂起>")]
		private void OnCanAddExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			CommandID id = menu.CommandID; menu.Enabled = menu.Visible = false;
			GetLocalizationPane(out LocalizationPane pane);
			if (pane != null)
			{
				LocalizationCollection localizations = pane.Localizations;
				menu.Enabled = menu.Visible = localizations.EnabledCultureInfos.Any(m => m.LCID == id.ID);
			}
		}

		[SuppressMessage("Usage", "VSTHRD102:异步实现内部逻辑", Justification = "<挂起>")]
		[SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
		private void OnAddCultureExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			GetLocalizationPane(out LocalizationPane pane);
			Microsoft.Assumes.Present(pane);
			LocalizationCollection localizations = pane.Localizations;
			CultureInfo cultureInfo = new CultureInfo(menu.CommandID.ID);
			if (localizations.AddCultureInfo(cultureInfo))
			{
				FileInfo itemInfo = new FileInfo(pane.FileName);
				EnvDTE.ProjectItem fileItem = pane.ProjectItem;
				EnvDTE.Solution solution = fileItem.DTE.Solution;
				if (fileItem != null && itemInfo.Exists)
				{
					string cultureFileName = string.Concat(itemInfo.FullName.Replace(itemInfo.Extension, ""), ".", cultureInfo.Name, ".resx");
					EnvDTE.ProjectItem cultureItem = solution.FindProjectItem(cultureFileName);
					FileInfo cultureFileInfo = new FileInfo(cultureFileName);
					if (cultureItem == null && cultureFileInfo.Exists)
					{
						_ = fileItem.ProjectItems.AddFromFile(cultureFileName);
						SortedDictionary<string, string> cultureNodes = new SortedDictionary<string, string>();
						using (FileStream stream = cultureFileInfo.Open(FileMode.Open, FileAccess.Read))
						{
							using (ResXResourceReader reader = new ResXResourceReader(stream))
							{
								foreach (DictionaryEntry node in reader)
								{
									cultureNodes[Convert.ToString(node.Key)] = Convert.ToString(node.Value);
								}
							}
						}
						foreach (LocalizationItem localResx in localizations)
						{
							if (cultureNodes.ContainsKey(localResx.Name))
								localResx[cultureInfo.Name] = cultureNodes[localResx.Name];
							else
								localResx[cultureInfo.Name] = localResx.Value;
						}
					}
					else if (cultureItem == null && cultureFileInfo.Exists == false)
					{
						using (FileStream stream = cultureFileInfo.Create())
						{
							using (ResXResourceWriter writer = new ResXResourceWriter(stream))
							{
								foreach (LocalizationItem localResx in localizations)
								{
									ResXDataNode node = new ResXDataNode(localResx.Name, localResx.Value)
									{
										Comment = localResx.Comment
									};
									localResx[cultureInfo.Name] = localResx.Value;
									writer.AddResource(node);
								}
							}
						}
						ProjectItem projectItem = fileItem.ProjectItems.AddFromFile(cultureFileName);
						GetGeneratorInfo(projectItem, fileItem.Name);
						fileItem.ContainingProject.Save();
					}
				}
			}
		}
		private void GetGeneratorInfo(ProjectItem item, string dependentUpon)
		{
			foreach (EnvDTE.Property property in item.Properties)
			{
				if (property.Name == "AutoGen") { property.Value = "True"; }
				else if (property.Name == "DependentUpon")
				{
					property.Value = dependentUpon;
				}
			}
		}

		#endregion

		#region 删除语言资源
		[SuppressMessage("Usage", "VSTHRD100:避免使用 Async Void 方法", Justification = "<挂起>")]
		private void OnCanRemoveExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			CommandID id = menu.CommandID; menu.Enabled = menu.Visible = false;
			GetLocalizationPane(out LocalizationPane pane);
			if (pane != null)
			{
				LocalizationCollection localizations = pane.Localizations;
				menu.Enabled = menu.Visible = localizations.CultureInfos.Any(m => m.LCID == id.ID);
			}
		}

		[SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
		private void OnRemoveCultureExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			GetLocalizationPane(out LocalizationPane pane);
			Microsoft.Assumes.Present(pane);
			LocalizationCollection localizations = pane.Localizations;
			CultureInfo cultureInfo = new CultureInfo(menu.CommandID.ID);
			if (localizations.RemoveCultureInfo(cultureInfo))
			{
				FileInfo itemInfo = new FileInfo(pane.FileName);
				if (itemInfo.Exists == false) { return; }
				ProjectItem fileItem = pane.ProjectItem;
				string cultureFileName = string.Concat(pane.FileName.Replace(itemInfo.Extension, ""), ".", cultureInfo.Name, ".resx");
				EnvDTE.Solution solution = fileItem.DTE.Solution;

				FileInfo localFile = new FileInfo(cultureFileName);
				ProjectItem cultureItem = solution.FindProjectItem(cultureFileName);
				if (cultureItem != null) { cultureItem.Remove(); }
				if (localFile.Exists) { localFile.Delete(); }
			}
		}
		#endregion

		#region 标准操作(剪切)
		private void OnCanCutExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			GetLocalizationPane(out LocalizationPane pane);
			if (pane != null)
			{
				ResourceEditor editor = pane.Editor;
				menu.Enabled = menu.Visible = editor.SelectedItems != null && editor.SelectedItems.Count > 0;
			}
		}

		private void OnCutExecuted(object sender, EventArgs e)
		{
			GetLocalizationPane(out LocalizationPane pane);
			if (pane != null)
			{
				LocalizationCollection localizations = pane.Localizations;
				ResourceEditor editor = pane.Editor;
				if (editor.SelectedItems == null || editor.SelectedItems.Count == 0) { return; }
				StringBuilder builder = new StringBuilder(ClipboardFormat.LocalizationFormat, 5000);
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					writer.WriteStartElement(LocalizationCollection.ResxDatasElementName);
					localizations.BeginChanging();
					foreach (LocalizationItem res in editor.SelectedItems)
					{
						res.WriteXml(writer, true); _ = localizations.Remove(res);
					}
					localizations.EndChanged();
					writer.WriteEndElement();
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.CommaSeparatedValue);
			}
		}
		#endregion

		#region 标准操作(复制)
		private void OnCanCopyExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			GetLocalizationPane(out LocalizationPane pane);
			if (pane != null)
			{
				ResourceEditor editor = pane.Editor;
				menu.Enabled = menu.Visible = editor.SelectedItems != null && editor.SelectedItems.Count > 0;
			}
		}

		private void OnCopyExecuted(object sender, EventArgs e)
		{
			GetLocalizationPane(out LocalizationPane pane);
			if (pane != null)
			{
				ResourceEditor editor = pane.Editor;
				if (editor.SelectedItems == null || editor.SelectedItems.Count == 0) { return; }
				StringBuilder builder = new StringBuilder(ClipboardFormat.LocalizationFormat, 5000);
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					writer.WriteStartElement(LocalizationCollection.ResxDatasElementName);
					foreach (LocalizationItem res in editor.SelectedItems)
					{
						res.WriteXml(writer, true);
					}
					writer.WriteEndElement();
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.CommaSeparatedValue);
			}
		}
		#endregion

		#region 标准操作(粘贴)

		private void OnCanPasteExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			if (Clipboard.ContainsText(TextDataFormat.CommaSeparatedValue))
			{
				string text = Clipboard.GetText(TextDataFormat.CommaSeparatedValue);
				int length = ClipboardFormat.ClipboardFormatLength;
				menu.Enabled = menu.Visible = text.IndexOf(ClipboardFormat.LocalizationFormat, 0, length) >= 0;
			}
		}

		private void OnPasteExecuted(object sender, EventArgs e)
		{
			GetLocalizationPane(out LocalizationPane pane);
			if (pane == null) { return; }
			LocalizationCollection localizations = pane.Localizations;
			string text = Clipboard.GetText(TextDataFormat.CommaSeparatedValue);
			if (!string.IsNullOrWhiteSpace(text))
			{
				text = text.Remove(0, ClipboardFormat.ClipboardFormatLength);
				using (StringReader stringReader = new StringReader(text))
				{
					using (XmlReader reader = XmlReader.Create(stringReader))
					{
						reader.MoveToContent(); int index = 0;
						while (reader.Read())
						{
							if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
							else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == LocalizationItem.XmlElementName)
							{
								LocalizationItem localResx = new LocalizationItem(localizations);
								localResx.ReadXml(reader.ReadSubtree());
								if (localizations.ContainsName(localResx.Name))
								{
									index = 0;
									while (true)
									{
										index++;
										string name = string.Concat(localResx.Name, index);
										if (localizations.ContainsName(name))
											continue;
										localResx.Name = name;
										break;
									}
								}
								localizations.Add(localResx);
							}
						}
					}
				}
				Clipboard.Clear();
			}
		}
		#endregion

		#region 标准操作(删除)
		private void OnCanDeleteExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			GetLocalizationPane(out LocalizationPane pane);
			if (pane != null)
			{
				ResourceEditor editor = pane.Editor;
				menu.Enabled = menu.Visible = editor.SelectedItems != null && editor.SelectedItems.Count > 0;
			}
		}

		private void OnDeleteExecuted(object sender, EventArgs e)
		{
			GetLocalizationPane(out LocalizationPane pane);
			if (pane == null) { return; }
			LocalizationCollection localizations = pane.Localizations;
			ResourceEditor editor = pane.Editor;
			if (editor.SelectedItems == null || editor.SelectedItems.Count == 0) { return; }
			localizations.BeginChanging();
			foreach (LocalizationItem res in editor.SelectedItems)
			{
				_ = localizations.Remove(res);
			}
			localizations.EndChanged();
		}
		#endregion

		#region 分组操作(分组/取消分组)
		private void OnCanUnGroupExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			GetLocalizations(out LocalizationPane pane, out ResourceEditor editor);
			if (pane != null && editor != null)
			{
				menu.Enabled = menu.Visible = editor.IsGrouping == true;
			}
		}
		private void OnUnGroupExecuted(object sender, EventArgs e)
		{
			GetLocalizationPane(out LocalizationPane pane);
			if (pane == null) { return; }
			//LocalizationCollection localizations = pane.Localizations;
			//ResourceEditor editor = pane.Editor;
		}

		private void OnCanGroupExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			GetLocalizations(out LocalizationPane pane, out ResourceEditor editor);
			if (pane != null && editor != null)
			{
				menu.Enabled = menu.Visible = editor.IsGrouping == false;
			}
		}
		private void OnGroupExecuted(object sender, EventArgs e)
		{
			GetLocalizations(out LocalizationPane pane, out ResourceEditor editor);
			if (pane == null || editor == null) { return; }
			editor.Grouping();
		}
		#endregion

		#region 数据操作(插入/追加)
		private void OnAppendExecuted(object sender, EventArgs e)
		{
			GetLocalizations(out LocalizationPane pane, out ResourceEditor editor);
			if (pane == null || editor == null) { return; }
			LocalizationCollection localizations = pane.Localizations;
			int count = localizations.Count + 1;
			if (editor.SelectedItem != null)
			{
				LocalizationItem item = editor.SelectedItem as LocalizationItem;
				editor.SelectedItem = localizations.Add(item.Group, string.Concat("String", count), string.Concat("String", count));
			}
			else
			{
				editor.SelectedItem = localizations.Add("Group", string.Concat("String", count), string.Concat("String", count));
			}
		}

		private void OnCanInsertExecuted(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			GetLocalizationPane(out LocalizationPane pane);
			if (pane != null && pane.Editor != null)
			{
				menu.Enabled = menu.Visible = pane.Editor.SelectedItem != null;
			}
		}
		private void OnInsertExecuted(object sender, EventArgs e)
		{
			GetLocalizations(out LocalizationPane pane, out ResourceEditor editor);
			if (pane == null || editor == null) { return; }
			LocalizationCollection localizations = pane.Localizations;
			if (editor.SelectedItem != null && editor.SelectedIndex >= 0)
			{
				int count = localizations.Count;
				LocalizationItem sitem = editor.SelectedItem as LocalizationItem;
				LocalizationItem nitem = new LocalizationItem(localizations, sitem.Group,
						string.Concat("String", count), string.Concat("String", count));
				int index = localizations.IndexOf(sitem);
				if (index >= 0) { localizations.Insert(index, nitem); }
				else { localizations.Insert(0, nitem); }
				editor.SelectedItem = nitem;
			}
		}
		#endregion

		#region 获取当前编辑的
		[SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
		private void GetLocalizationPane(out LocalizationPane localPane)
		{
			Microsoft.Assumes.Present(monitorSelection);
			//_ = ErrorHandler.ThrowOnFailure(monitorSelection.GetCurrentSelection(out IntPtr hierarchyPtr, out uint itemId, out _, out _));
			//IVsHierarchy hierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
			//hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out projectItem);
			monitorSelection.GetCurrentElementValue(1, out object objectFrame);
			if (objectFrame is IVsWindowFrame frame)
			{
				frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object pane);//获取 WindowPane
				if (pane != null) { localPane = pane as LocalizationPane; return; }
			}
			localPane = null;
		}
		[SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
		private void GetLocalizations(out LocalizationPane localPane, out ResourceEditor editor)
		{
			Microsoft.Assumes.Present(monitorSelection);
			monitorSelection.GetCurrentElementValue(1, out object objectFrame);
			if (objectFrame is IVsWindowFrame frame)
			{
				frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object pane);//获取 WindowPane
				if (pane != null) { localPane = pane as LocalizationPane; editor = localPane.Editor; return; }
			}
			localPane = null; editor = null;
		}
		#endregion

		/// <summary>
		/// 显示属性窗口
		/// </summary>
		public void OnPropertyWindow(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Guid guid = StandardToolWindows.PropertyBrowser;
			iVsUIShell.FindToolWindow(0x80000, ref guid, out IVsWindowFrame ppWindowFrame);
			ppWindowFrame.Show();
		}
	}
}
