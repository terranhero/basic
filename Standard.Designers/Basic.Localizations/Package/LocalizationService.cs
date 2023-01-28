using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Basic.Windows;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;
using STT = System.Threading.Tasks;
//VSLangProj.prjKindVBProject for VB.NET projects.
//VSLangProj.prjKindCSharpProject for C# projects.
//VSLangProj.prjKindVSAProject for VSA projects (macros projects).
//VSLangProj2.prjKindVJSharpProject for Visual J# projects.
//VSLangProj2.prjKindSDEVBProject for VB.NET Smart Device projects (Visual Studio .NET 2002/2003 only, see below for Visual Studio 2005).
//VSLangProj2.prjKindSDECSharpProject for C# projects  (Visual Studio .NET 2002/2003 only, see below for Visual Studio 2005).
//{7D353B21-6E36-11D2-B35A-0000F81F0C06} for "Enterprise Projects" (Visual Studio .NET 2002/2003 only).
//{54435603-DBB4-11D2-8724-00A0C9A8B90C} for Setup projects.
//EnvDTE.Constants.vsProjectKindSolutionItems for the Solution Items folder of the Solution Explorer.
//EnvDTE.Constants.vsProjectKindMisc for the Miscellaneous Files folder of the Solution Explorer.

namespace Basic.Localizations
{
	/// <summary表示本地化资源服务</summary>
	[System.Runtime.InteropServices.Guid(MenuGuidString)]
	public sealed partial class LocalizationService
	{
		#region 模型命令
		/// <summary>快捷菜单Guid字符串(提取枚举资源)</summary>
		public const string MenuGuidString = "81B3CD54-E17E-46A0-87EF-988C25A0FDD0";

		/// <summary>
		/// 表示数据持久类菜单标识符。
		/// </summary>
		public static readonly Guid MenuGuid = new Guid(MenuGuidString);

		/// <summary>表示数据持久类菜单ID</summary>
		public static readonly CommandID MenuID = new CommandID(MenuGuid, 0x1000);

		/// <summary>表示数据持久类菜单ID</summary>
		public static readonly CommandID ExtractEnumID = new CommandID(MenuGuid, 0x1011);

		/// <summary>表示数据持久类菜单ID</summary>
		public static readonly CommandID IDC_SVN_VERSION = new CommandID(MenuGuid, 0x1021);
		#endregion

		#region PersistentCommandService 类全局变量
		//private const string pattern = "*.sqlf;*.oraf;*.olef;*.odbcf;*.db2f;*.litf";
		//private const string fileExtension = ".sqlf;.oraf;.olef;.odbcf;.db2f;.litf";

		private readonly AsyncPackage asyncServiceProvider;
		private OleMenuCommandService oleMenuService;
		private IVsUIShell iVsUIShell;
		private EnvDTE.DTE dteClass;
		#endregion

		/// <summary>初始化 ResourceCommandService 类实例。</summary>
		/// <param name="package"></param>
		public LocalizationService(AsyncPackage package) { asyncServiceProvider = package; }

		internal void SetWaitCursor()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (iVsUIShell != null) { iVsUIShell.SetWaitCursor(); }
		}

		#region Visual Studio 常用命令及方法
		/// <summary>
		/// 显示快捷菜单
		/// </summary>
		/// <param name="x">菜单显示横坐标</param>
		/// <param name="y">菜单显示纵坐标</param>
		public void ShowEnumContextMenu(int x, int y)
		{
			if (null != oleMenuService) { oleMenuService.ShowContextMenu(MenuID, x, y); }
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="message">一个 String，用于指定要显示的文本。</param>
		/// <param name="title">一个 String，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 int 值，用于指定用户单击了哪个消息框按钮。</returns>
		public int ShowMessage(string message, string title = "Basic.Localizations")
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (iVsUIShell != null)
			{
				Guid tempGuid = Guid.Empty;
				iVsUIShell.ShowMessageBox(0, ref tempGuid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK,
				   OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 0, out int result);
				return result;
			}
			return 0;
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="ex">一个 String，用于指定要显示的文本。</param>
		/// <param name="title">一个 String，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 int 值，用于指定用户单击了哪个消息框按钮。</returns>
		public int ShowMessage(Exception ex, string title = "Basic.Localizations")
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return ShowMessage(ex.Message, title);
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="message">一个 String，用于指定要显示的文本。</param>
		/// <param name="title">一个 String，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 int 值，用于指定用户单击了哪个消息框按钮。</returns>
		public bool Confirm(string message, string title = "Basic.Localizations")
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (string.IsNullOrWhiteSpace(title) && dteClass.LocaleID == 2052) { title = Strings.Package_Description_zh_CN; }
			else if (string.IsNullOrWhiteSpace(title)) { title = Strings.Package_Description_en_US; }
			if (iVsUIShell != null)
			{
				Guid tempGuid = Guid.Empty;
				iVsUIShell.ShowMessageBox(0, ref tempGuid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_QUERY, 0, out int result);
				return result == 1;
			}
			return false;//
		}


		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		internal async STT.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			progress.Report(new ServiceProgressData("请稍等", "加载菜单"));
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			dteClass = await asyncServiceProvider.GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
			Assumes.Present(dteClass);
			iVsUIShell = await asyncServiceProvider.GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
			Assumes.Present(iVsUIShell);
			oleMenuService = await asyncServiceProvider.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Assumes.Present(oleMenuService);
			if (null != oleMenuService)
			{
				oleMenuService.AddCommand(ExtractEnumID, OnExtractEnum, OnCanExtractEnum);
				oleMenuService.AddCommand(IDC_SVN_VERSION, OnSvnVersion, OnCanSvnVersion);
				await InitializeContextMenuAsync(oleMenuService, progress);
				//mcs.AddCommand( ConverterAllID, OnConvertAll, OnCanConvertAll);
				//mcs.AddCommand( AddClassicViewID, OnAddClassicView, OnCanAddClassicView);
				//mcs.AddCommand( AddControllerID, OnAddController, OnCanAddController);
				//mcs.AddCommand( AddWpfFormID, OnAddWpfForm, OnCanAddWpfForm);
				//mcs.AddCommand( AddPersistentID, OnAddPersistent, OnCanAddPersistent);
				//mcs.AddCommand( AddConnectionID, OnAddConnection, OnCanAddConnection);
			}
		}


		#endregion

		private void OnCanSvnVersion(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dte2Class = dteClass as EnvDTE80.DTE2;
			if (dte2Class == null) { return; }
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(dte2Class.ActiveDocument.FullName);
			if (fileInfo.Extension == ".targets") { menu.Visible = menu.Enabled = true; }
			else if (fileInfo.Extension == ".cs") { menu.Visible = menu.Enabled = true; }
			//menu.Visible = menu.Enabled = element != null;
		}

		/// <summary>提取Svn最新提交日志版本号</summary>
		private void OnSvnVersion(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			EnvDTE80.DTE2 dte2Class = dteClass as EnvDTE80.DTE2;
			if (dte2Class == null) { return; }
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(dte2Class.ActiveDocument.FullName);
			using (SharpSvn.SvnClient client = new SharpSvn.SvnClient())
			{
				SvnPathTarget local = new SvnPathTarget(fileInfo.FullName);
				client.GetInfo(local, out SvnInfoEventArgs info);
				MessageBox.Show(Convert.ToString(info.LastChangeRevision));
			}
			//EnvDTE.TextSelection textSelection = (EnvDTE.TextSelection)dte.ActiveDocument.Selection;
			//EnvDTE.VirtualPoint point = textSelection.ActivePoint;
			//EnvDTE.CodeVariable element = (EnvDTE.CodeVariable)point.get_CodeElement(EnvDTE.vsCMElement.vsCMElementVariable);
			//EnvDTE.CodeEnum codeEnum = (EnvDTE.CodeEnum)textSelection.ActivePoint.get_CodeElement(EnvDTE.vsCMElement.vsCMElementEnum);
			//LocalizationCollection resourceCollection = new LocalizationCollection();
			//resourceCollection.AddEnabledCultureInfos();
			//Regex reg = new Regex(@"(?<=<summary>)(.*?)(?=</summary>)", RegexOptions.Singleline);//[^(<summary>))] 
			//if (element != null && codeEnum != null)
			//{
			//	string itemName = string.Concat(codeEnum.Name, "_", element.Name);
			//	string elementName = element.Name, comment = element.Name;
			//	if (!string.IsNullOrWhiteSpace(element.DocComment))
			//		comment = reg.Match(element.DocComment).Value.Trim();
			//	string groupName = codeEnum.Name;
			//	if (!resourceCollection.ContainsName(itemName))
			//	{
			//		LocalizationItem resx = resourceCollection.Add(groupName, itemName, comment);
			//		foreach (CultureInfo culture in resourceCollection.CultureInfos)
			//		{
			//			if (culture.LCID == 1033) { resx[culture.Name] = elementName; }//en-US
			//			else if (culture.LCID == 0x0004) { resx[culture.Name] = comment; }//zh-Hans
			//			else if (culture.LCID == 0x7C04) { resx[culture.Name] = comment; }//zh-Hant
			//			else if (culture.LCID == 2052) { resx[culture.Name] = comment; }//zh-CN
			//			else if (culture.LCID == 1028) { resx[culture.Name] = comment; }//zh-TW
			//			else { resx[culture.Name] = elementName; }
			//		}
			//	}
			//}
			//else if (codeEnum != null)
			//{
			//	foreach (EnvDTE.CodeVariable variable in codeEnum.Members)
			//	{
			//		string itemName = string.Concat(codeEnum.Name, "_", variable.Name);
			//		string elementName = variable.Name, comment = variable.Name;
			//		if (!string.IsNullOrWhiteSpace(variable.DocComment))
			//			comment = reg.Match(variable.DocComment).Value.Trim();
			//		string groupName = codeEnum.Name;
			//		if (!resourceCollection.ContainsName(itemName))
			//		{
			//			LocalizationItem resx = resourceCollection.Add(groupName, itemName, comment);
			//			foreach (CultureInfo culture in resourceCollection.CultureInfos)
			//			{
			//				if (culture.LCID == 1033) { resx[culture.Name] = elementName; }//en-US
			//				else if (culture.LCID == 0x0004) { resx[culture.Name] = comment; }//zh-Hans
			//				else if (culture.LCID == 0x7C04) { resx[culture.Name] = comment; }//zh-Hant
			//				else if (culture.LCID == 2052) { resx[culture.Name] = comment; }//zh-CN
			//				else if (culture.LCID == 1028) { resx[culture.Name] = comment; }//zh-TW
			//				else { resx[culture.Name] = elementName; }
			//			}
			//		}
			//	}
			//}
			//CreateResourceView resourceView = new CreateResourceView(this, dte, resourceCollection);
			//if (resourceView.ShowModal() == true)
			//{
			//	string resInfo = Strings.Package_ResourceDescription;
			//	string successInfo = Strings.Package_CreateResourceSuccessful; //当前数据持久类资源已经创建成功，且已经复制进系统剪贴板。
			//	ShowMessage(successInfo, resInfo);
			//}
		}

		private void OnCanExtractEnum(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dte = dteClass as EnvDTE80.DTE2;
			if (dteClass == null) { return; }
			EnvDTE.TextSelection textSelection = (EnvDTE.TextSelection)dte.ActiveDocument.Selection;
			if (textSelection == null) { return; }
			EnvDTE.VirtualPoint point = textSelection.ActivePoint;
			if (point == null) { return; }
			EnvDTE.CodeElement element = point.get_CodeElement(EnvDTE.vsCMElement.vsCMElementEnum);
			if (element == null) { return; }
			menu.Visible = menu.Enabled = element != null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExtractEnum(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			EnvDTE80.DTE2 dte = dteClass as EnvDTE80.DTE2;
			EnvDTE.TextSelection textSelection = (EnvDTE.TextSelection)dte.ActiveDocument.Selection;
			EnvDTE.VirtualPoint point = textSelection.ActivePoint;
			EnvDTE.CodeVariable element = (EnvDTE.CodeVariable)point.get_CodeElement(EnvDTE.vsCMElement.vsCMElementVariable);
			EnvDTE.CodeEnum codeEnum = (EnvDTE.CodeEnum)textSelection.ActivePoint.get_CodeElement(EnvDTE.vsCMElement.vsCMElementEnum);
			LocalizationCollection resourceCollection = new LocalizationCollection();
			resourceCollection.AddEnabledCultureInfos();
			Regex reg = new Regex(@"(?<=<summary>)(.*?)(?=</summary>)", RegexOptions.Singleline);//[^(<summary>))] 
			if (element != null && codeEnum != null)
			{
				string itemName = string.Concat(codeEnum.Name, "_", element.Name);
				string elementName = element.Name, comment = element.Name;
				if (!string.IsNullOrWhiteSpace(element.DocComment))
					comment = reg.Match(element.DocComment).Value.Trim();
				string groupName = codeEnum.Name;
				if (!resourceCollection.ContainsName(itemName))
				{
					LocalizationItem resx = resourceCollection.Add(groupName, itemName, comment);
					foreach (CultureInfo culture in resourceCollection.CultureInfos)
					{
						if (culture.LCID == 1033) { resx[culture.Name] = elementName; }//en-US
						else if (culture.LCID == 0x0004) { resx[culture.Name] = comment; }//zh-Hans
						else if (culture.LCID == 0x7C04) { resx[culture.Name] = comment; }//zh-Hant
						else if (culture.LCID == 2052) { resx[culture.Name] = comment; }//zh-CN
						else if (culture.LCID == 1028) { resx[culture.Name] = comment; }//zh-TW
						else { resx[culture.Name] = elementName; }
					}
				}
			}
			else if (codeEnum != null)
			{
				foreach (EnvDTE.CodeVariable variable in codeEnum.Members)
				{
					string itemName = string.Concat(codeEnum.Name, "_", variable.Name);
					string elementName = variable.Name, comment = variable.Name;
					if (!string.IsNullOrWhiteSpace(variable.DocComment))
						comment = reg.Match(variable.DocComment).Value.Trim();
					string groupName = codeEnum.Name;
					if (!resourceCollection.ContainsName(itemName))
					{
						LocalizationItem resx = resourceCollection.Add(groupName, itemName, comment);
						foreach (CultureInfo culture in resourceCollection.CultureInfos)
						{
							if (culture.LCID == 1033) { resx[culture.Name] = elementName; }//en-US
							else if (culture.LCID == 0x0004) { resx[culture.Name] = comment; }//zh-Hans
							else if (culture.LCID == 0x7C04) { resx[culture.Name] = comment; }//zh-Hant
							else if (culture.LCID == 2052) { resx[culture.Name] = comment; }//zh-CN
							else if (culture.LCID == 1028) { resx[culture.Name] = comment; }//zh-TW
							else { resx[culture.Name] = elementName; }
						}
					}
				}
			}
			CreateResourceView resourceView = new CreateResourceView(this, dte, resourceCollection);
			if (resourceView.ShowModal() == true)
			{
				string resInfo = Strings.Package_ResourceDescription;
				string successInfo = Strings.Package_CreateResourceSuccessful; //当前数据持久类资源已经创建成功，且已经复制进系统剪贴板。
				ShowMessage(successInfo, resInfo);
			}
		}
	}
}
