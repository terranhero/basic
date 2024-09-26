using System;
using System.CodeDom.Compiler;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using Basic.Database;
using Basic.DataEntities;
using Basic.Enums;
using Basic.Localizations;
using Basic.Options;
using Basic.Properties;
using Basic.Windows;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MDC = Microsoft.Data.ConnectionUI;
using MVS = Microsoft.VisualStudio.Shell;
using SC = System.Configuration;
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

namespace Basic.Configuration
{
	/// <summary>数据持久命令服务类</summary>
	[System.Runtime.InteropServices.Guid(PersistentGuidString)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0019:使用模式匹配", Justification = "<挂起>")]
	public sealed partial class PersistentService
	{
		private readonly AsyncPackage asyncPackage;
		private readonly ErrorListProvider errorListProvider;
		/// <summary>初始化 PersistentService 类实例。</summary>
		/// <param name="package"></param>
		public PersistentService(AsyncPackage package)
		{
			asyncPackage = package;
			errorListProvider = new ErrorListProvider(package);
			callback = new ServiceCreatorCallback(CreateService);
			asyncCallback = new AsyncServiceCreatorCallback(CreateServiceAsync);
		}

		public MVS.AsyncPackage AsyncPackage { get { return asyncPackage; } }

		public STT.Task<object> GetServiceAsync(Type serviceType)
		{
			return asyncPackage.GetServiceAsync(serviceType);
		}

		public STT.Task<TI> GetServiceAsync<TS, TI>() where TI : class
		{
			return asyncPackage.GetServiceAsync<TS, TI>();
		}

		public STT.Task<TI> GetServiceAsync<TI>() where TI : class
		{
			return asyncPackage.GetServiceAsync<TI, TI>();
		}

		public TI GetService<TI>() where TI : class
		{
			return asyncPackage.GetService<TI, TI>(true);
		}

		public TI GetService<TS, TI>() where TI : class
		{
			return asyncPackage.GetService<TS, TI>(true);
		}

		#region PersistentService 类全局变量
		private const string pattern = "*.sqlf;*.oraf;*.olef;*.odbcf;*.db2f;*.litf";
		private const string fileExtension = ".sqlf;.oraf;.olef;.odbcf;.db2f;.litf";

		private OleMenuCommandService _oleMenuService;
		private IVsUIShell _IVsUIShell;
		private IVsSolution _VsSolution;
		private EnvDTE80.DTE2 _DteClass;
		#endregion


		#region Visual Studio 常用命令及方法
		/// <summary>设置当前鼠标为等待光标</summary>
		public void SetWaitCursor() { if (_IVsUIShell != null) { _IVsUIShell.SetWaitCursor(); } }

		/// <summary>
		/// 显示快捷菜单
		/// </summary>
		/// <param name="x">菜单显示横坐标</param>
		/// <param name="y">菜单显示纵坐标</param>
		public void ShowContextMenu(int x, int y)
		{
			//OleMenuCommandService _oleMenuService = CommandService;
			if (null != _oleMenuService) { _oleMenuService.ShowContextMenu(ContextMenuID, x, y); }
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="message">一个 String，用于指定要显示的文本。</param>
		/// <param name="title">一个 String，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 int 值，用于指定用户单击了哪个消息框按钮。</returns>
		public int ShowMessage(string message, string title = "Basic.Persistent")
		{
			int result = 0; Guid tempGuid = Guid.Empty;
			IVsUIShell uiShell = GetVsUIShell();
			if (uiShell != null)
			{
				uiShell.ShowMessageBox(0, ref tempGuid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK,
					 OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 0, out result);
			}
			return result;
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="ex">一个 String，用于指定要显示的文本。</param>
		/// <param name="title">一个 String，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 int 值，用于指定用户单击了哪个消息框按钮。</returns>
		public int ShowMessage(Exception ex, string title = "Basic.Persistent")
		{
			int result = 0; Guid tempGuid = Guid.Empty;
			IVsUIShell uiShell = GetVsUIShell();
			if (uiShell != null)
			{
				uiShell.ShowMessageBox(0, ref tempGuid, title, ex.Message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK,
					 OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 0, out result);
			}
			return result;
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="message">一个 String，用于指定要显示的文本。</param>
		/// <param name="title">一个 String，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 int 值，用于指定用户单击了哪个消息框按钮。</returns>
		public bool Confirm(string message, string title = "Basic.Persistent")
		{
			if (string.IsNullOrWhiteSpace(title))
				title = DesignerStrings.ResourceManager.GetString("Package_Description");
			int result = 0; Guid tempGuid = Guid.Empty;
			IVsUIShell uiShell = GetVsUIShell();
			if (uiShell != null)
			{
				uiShell.ShowMessageBox(0, ref tempGuid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
					 OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_QUERY, 0, out result);
			}
			return result == 1;//
		}

		/// <summary>
		/// 显示属性窗口
		/// </summary>
		public void ShowPropertyWindow()
		{
			IVsUIShell uiShell = GetVsUIShell();
			Guid guid = StandardToolWindows.PropertyBrowser;
			uiShell.FindToolWindow(0x80000, ref guid, out IVsWindowFrame ppWindowFrame);
			ppWindowFrame.Show();
		}

		private EnvDTE80.DTE2 GetDTE()
		{
			if (_DteClass == null)
				_DteClass = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
			return _DteClass;
		}

		private IVsUIShell GetVsUIShell() { return _IVsUIShell; }

		private IVsSolution GetVsSolution() { return _VsSolution; }

		#endregion
		public string[] GetBaseAccesses()
		{
			try
			{
				return baseClassesOptions.BaseAccess.ToArray();
			}
			catch (Exception ex)
			{
				WriteToOutput(ex.Message);
				return Array.Empty<string>();
			}
		}

		public string[] GetBaseEntities()
		{
			try
			{
				return baseClassesOptions.BaseEntities.ToArray();
			}
			catch (Exception ex)
			{
				WriteToOutput(ex.Message);
				return Array.Empty<string>();
			}
		}

		public string[] GetBaseConditions()
		{
			try
			{
				return (baseClassesOptions.BaseConditions.ToArray());
			}
			catch (Exception ex)
			{
				WriteToOutput(ex.Message);
				return Array.Empty<string>();
			}
		}
		private AClassesOptions baseClassesOptions;
		/// <summary>读取系统配置信息</summary>
		public Task InitializeOptionsAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			try
			{
				baseClassesOptions = asyncPackage.GetDialogPage(typeof(AClassesOptions)) as AClassesOptions;
				//asyncPackage.AddService(typeof(IClassesOptions), asyncCallback, true);
				//((IServiceContainer)asyncPackage).AddService(typeof(IClassesOptions), callback, true);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				WriteToOutput(ex.Message);
				return Task.CompletedTask;
			}
		}
		private readonly AsyncServiceCreatorCallback asyncCallback;
		public Task<object> CreateServiceAsync(IAsyncServiceContainer container, CancellationToken cancellationToken, Type serviceType)
		{
			if (typeof(IClassesOptions) == serviceType) { return Task.FromResult<object>(baseClassesOptions); }
			return Task.FromResult<object>(null);
		}

		private readonly ServiceCreatorCallback callback;
		private object CreateService(IServiceContainer container, Type serviceType)
		{
			if (typeof(IClassesOptions) == serviceType) { return baseClassesOptions; }
			return null;
		}

		/// <summary>模版文件夹</summary>
		internal const string TemplateFolder = "Templates";
		public Task InitializeTemplateAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			progress.Report(new ServiceProgressData(nameof(InitializeTemplateAsync), "加载模板文件"));
			try
			{
				//string assFilePath = typeof(PersistentService).Assembly.Location;
				//string assPath = Path.GetDirectoryName(assFilePath);
				//string tempPath = Path.Combine(assPath, TemplateFolder);
				//DirectoryInfo folder = new DirectoryInfo(tempPath);
				//tempPath += "\\";

				//TemplateServiceConfiguration config = new TemplateServiceConfiguration
				//{
				//	CompilerServiceFactory = new RazorEngine.Roslyn.RoslynCompilerServiceFactory(),
				//	BaseTemplateType = typeof(RazorEngine.Templating.HtmlTemplateBase<>)
				//};
				//config.Namespaces.Add("System.Reflection");
				//config.Namespaces.Add("RazorEngine.Text");
				//Engine.Razor = RazorEngineService.Create(config);
				//foreach (FileInfo file in folder.GetFiles("*.razor", SearchOption.AllDirectories))
				//{
				//	progress.Report(new ServiceProgressData(nameof(InitializeTemplatesAsync), file.Name));
				//	using (StreamReader reader = new StreamReader(file.FullName))
				//	{
				//		string content = await reader.ReadToEndAsync();
				//		ITemplateKey key = Engine.Razor.GetKey(file.FullName);
				//		Engine.Razor.Compile(content, key, typeof(Type));
				//	}
				//}

				//foreach (FileInfo file in folder.GetFiles("*.cshtml", SearchOption.AllDirectories))
				//{
				//	progress.Report(new ServiceProgressData(nameof(InitializeTemplatesAsync), file.Name));
				//	using (StreamReader reader = new StreamReader(file.FullName))
				//	{
				//		string content = await reader.ReadToEndAsync();
				//		ITemplateKey key = Engine.Razor.GetKey(file.FullName);
				//		Engine.Razor.Compile(content, key, typeof(Type));
				//	}
				//}
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				progress.Report(new ServiceProgressData(nameof(InitializeTemplateAsync), ex.Message));
				return Task.CompletedTask;
			}
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		public async STT.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			progress.Report(new ServiceProgressData("正在加载菜单......"));
			_oleMenuService = await asyncPackage.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Assumes.Present(_oleMenuService);
			_IVsUIShell = await asyncPackage.GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
			Assumes.Present(_IVsUIShell);
			_VsSolution = await asyncPackage.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
			Assumes.Present(_VsSolution);
			_DteClass = await asyncPackage.GetServiceAsync(typeof(SDTE)) as EnvDTE80.DTE2;
			Assumes.Present(_DteClass);
			//await serviceProvider.GetServiceAsync() as OleMenuCommandService
			progress.Report(new ServiceProgressData("菜单加载完成......"));
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		public STT.Task InitializeMenuAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			progress.Report(new ServiceProgressData("正在加载菜单......"));
			if (null != _oleMenuService)
			{
				_oleMenuService.AddCommand(ConverterID, OnConvert, OnCanConvert);
				_oleMenuService.AddCommand(ConverterAllID, OnConvertAll, OnCanConvertAll);
				_oleMenuService.AddCommand(AddClassicViewID, OnAddClassicView, OnCanAddClassicView);
				_oleMenuService.AddCommand(AddMvcViewID, OnAddMvcView, OnCanAddMvcView);
				_oleMenuService.AddCommand(AddControllerID, OnAddController, OnCanAddController);
				_oleMenuService.AddCommand(AddWpfFormID, OnAddWpfForm, OnCanAddWpfForm);
				_oleMenuService.AddCommand(AddPersistentID, OnAddPersistent, OnCanAddPersistent);
				_oleMenuService.AddCommand(AddLocalizationID, OnAddLocalization, OnCanAddLocalization);

				_oleMenuService.AddCommand(ShowAccessCodeID, OnShowAccessCode, OnCanShowCode);
				_oleMenuService.AddCommand(ShowContextCodeID, OnShowContextCode, OnCanShowCode);
				_oleMenuService.AddCommand(ConnectionGroupID, null, OnShowConfiguration);
				_oleMenuService.AddCommand(AddConnectionID, OnAddConnection, OnCanAddConnection);
				_oleMenuService.AddCommand(ResetConnectionID, OnResetConnection, OnCanResetConnection);

				_oleMenuService.AddCommand(CreateResourceID, OnCreateResource, OnCanExecuteResource);
				_oleMenuService.AddCommand(ResetResourceID, OnResetResource, OnCanExecuteResource);
				_oleMenuService.AddCommand(CreatePropertyResourceID, OnCreatePropertyResource, OnCanExecuteResource);
				_oleMenuService.AddCommand(CreateCommandResourceID, OnCreateCommandResource, OnCanExecuteResource);

				_oleMenuService.AddCommand(EditCodeID, OnEditCode, OnCanEditCode);
				_oleMenuService.AddCommand(CopyCodeID, OnCopyCode, OnCanCopyCode);
				_oleMenuService.AddCommand(CopySqlID, OnCopySql, OnCanCopySql);
				_oleMenuService.AddCommand(UpdateTableID, OnUpdateTable, OnCanUpdateTable);
				_oleMenuService.AddCommand(UpdateEntitiesID, OnUpdateEntities, OnCanUpdateEntities);
				_oleMenuService.AddCommand(UpdateEntityID, OnUpdateEntity, OnCanUpdateEntity);
				_oleMenuService.AddCommand(UpdateConditionID, OnUpdateCondition, OnCanUpdateCondition);

				_oleMenuService.AddCommand(UpdateCommandID, OnUpdateCommand, OnCanUpdateCommand);
				_oleMenuService.AddCommand(CreateCommandID, OnCreateCommand, OnCanCreateCommand);
				_oleMenuService.AddCommand(EditCommandID, OnEditCommand, OnCanEditCommand);

				_oleMenuService.AddCommand(PasteStaticCommandID, OnPasteStaticCommand, OnCanPasteCommand);
				_oleMenuService.AddCommand(PasteDynamicCommandID, OnPasteDynamicCommand, OnCanPasteCommand);

				_oleMenuService.AddCommand(InsertPropertyCommandID, OnInsertProperty, OnCanInsertProperty);
				_oleMenuService.AddCommand(CreatePropertyCommandID, OnCreateProperty, OnCanCreateProperty);

				_oleMenuService.AddCommand(StandardCommands.BringForward, OnOrderItem, OnSelectedEnabled);
				_oleMenuService.AddCommand(StandardCommands.BringToFront, OnOrderItem, OnSelectedEnabled);
				_oleMenuService.AddCommand(StandardCommands.SendBackward, OnOrderItem, OnSelectedEnabled);
				_oleMenuService.AddCommand(StandardCommands.SendToBack, OnOrderItem, OnSelectedEnabled);
				_oleMenuService.AddCommand(StandardCommands.SizeToFit, OnSizeToFit, OnCanSizeToFit);

				_oleMenuService.AddCommand(StandardCommands.Cut, OnCut, OnSelectedEnabled);
				_oleMenuService.AddCommand(StandardCommands.Copy, OnCopy, OnSelectedEnabled);
				_oleMenuService.AddCommand(StandardCommands.Paste, OnPaste, OnCanPaste);
				_oleMenuService.AddCommand(StandardCommands.Delete, OnDelete, OnSelectedEnabled);

				_oleMenuService.AddCommand(Cut, OnCut, OnSelectedEnabled);
				_oleMenuService.AddCommand(Copy, OnCopy, OnSelectedEnabled);
				_oleMenuService.AddCommand(Paste, OnPaste, OnCanPaste);
				_oleMenuService.AddCommand(Delete, OnDelete, OnSelectedEnabled);

				_oleMenuService.AddCommand(tableColumnsID, OnShowColumns, OnCanShowColumns);
				_oleMenuService.AddCommand(StandardCommands.Properties, OnProperties);
			}
			progress.Report(new ServiceProgressData("菜单加载完成......"));
			return STT.Task.CompletedTask;
		}

		#region 创建或设置资源命令
		private void OnCanExecuteResource(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			PersistentConfiguration _Persistent = GetPersistentConfiguration();
			if (_Persistent == null) { return; }
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			if (_Persistent.MessageConverter.IsEmpty) { return; }
			System.IO.FileInfo fileSolution = new System.IO.FileInfo(dteClass.Solution.FullName);
			string fullName = string.Concat(fileSolution.DirectoryName, _Persistent.MessageConverter.FileName);
			menu.Enabled = menu.Visible = dteClass.Solution.FindProjectItem(fullName) != null;
		}

		private void OnCreateResource(object sender, EventArgs e)
		{
			try
			{
				PersistentConfiguration _Persistent = GetPersistentConfiguration();
				if (_Persistent == null) { return; }
				if (_Persistent.MessageConverter.IsEmpty) { ShowMessage("本地化资源文件没有选择，请选择"); return; }
				EnvDTE80.DTE2 dteClass = GetDTE();
				System.IO.FileInfo fileSolution = new System.IO.FileInfo(dteClass.Solution.FullName);
				string fullName = string.Concat(fileSolution.DirectoryName, _Persistent.MessageConverter.FileName);
				EnvDTE.ProjectItem resourceItem = dteClass.Solution.FindProjectItem(fullName);
				if (resourceItem == null)
				{
					string message = string.Concat("本地化资源文件\"", _Persistent.MessageConverter, "\"已经不存在，请重新选择。");
					ShowMessage(message);
					return;
				}
				LocalizationCollection existResourceCollection = new LocalizationCollection();
				string resourceFileName = System.IO.Path.GetFileName(fullName);
				existResourceCollection.Load(fullName, m => File.OpenRead(m));

				LocalizationCollection resourceCollection = new LocalizationCollection();
				resourceCollection.AddEnabledCultureInfos();
				CultureInfo cultureInfo = resourceCollection.GetCultureInfo(1033);
				string groupName = _Persistent.GroupName;
				string publicGroupName = _Persistent.PublicGroupName;

				foreach (DataEntityElement entity in _Persistent.DataEntities)
				{
					#region 添加实体模型属性本地化资源：属性，验证，布尔可选
					foreach (DataEntityPropertyElement property in entity.Properties)
					{
						string resName = property.DisplayName.DisplayName;
						string pubName = string.Concat(publicGroupName, "_", property.Name);

						if (string.IsNullOrWhiteSpace(resName)) { resName = string.Concat(groupName, "_", property.Name); }

						if (!Regex.IsMatch(resName, "^[A-Za-z0-9_]+$")) { continue; }
						if (existResourceCollection.ContainsName(resName, pubName)) { continue; }

						if (!resourceCollection.ContainsName(resName))
						{
							LocalizationItem resx = resourceCollection.Add(groupName, resName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}
						if (!resourceCollection.ContainsName(pubName))
						{
							LocalizationItem resx = resourceCollection.Add(publicGroupName, pubName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}
						if (property.Type != null && property.Type == typeof(bool))
						{
							resName = string.Concat(groupName, "_", property.Name, "_TrueText");
							pubName = string.Concat(publicGroupName, "_", property.Name, "_TrueText");
							if (existResourceCollection.ContainsName(resName, pubName)) { continue; }

							if (!resourceCollection.ContainsName(resName))
							{
								LocalizationItem resx = resourceCollection.Add(groupName, resName, "是");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "True"; }
							}
							if (!resourceCollection.ContainsName(pubName))
							{
								LocalizationItem resx = resourceCollection.Add(groupName, pubName, "是");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "True"; }
							}

							resName = string.Concat(groupName, "_", property.Name, "_FalseText");
							pubName = string.Concat(publicGroupName, "_", property.Name, "_FalseText");
							if (existResourceCollection.ContainsName(resName, pubName)) { continue; }

							if (!resourceCollection.ContainsName(resName))
							{
								LocalizationItem resx = resourceCollection.Add(groupName, resName, "否");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "False"; }
							}
							if (!resourceCollection.ContainsName(pubName))
							{
								LocalizationItem resx = resourceCollection.Add(groupName, pubName, "否");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "False"; }
							}
						}
						string promptName = property.DisplayName.Prompt;
						if (!string.IsNullOrWhiteSpace(promptName) && !resourceCollection.ContainsName(promptName))
						{
							LocalizationItem resx = resourceCollection.Add(groupName, promptName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}
						#region 添加验证资源
						foreach (AbstractAttribute attribute in property.Attributes)
						{
							if (attribute is RequiredValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_Required");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "必须输入");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, " must enter");
								}
							}
							else if (attribute is BoolRequiredValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_Required");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "必须输入");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, " must enter");
								}
							}
							else if (attribute is CompareValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_Compare");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "输入数据错误");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, "'s data is error");
								}
							}
							else if (attribute is MaxLengthValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_MaxLength");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "输入长度必须小于\"{0}\"");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, "'s length must be less than or equal to {0} characters");
								}
							}
							else if (attribute is RangeValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_Range");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "必须输入\"{0}\"至\"{1}\"范围内的数据");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, "'s data must be between \"{0}\" to \"{1}\"");
								}
							}
							else if (attribute is RegularExpressionValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_RegularExpression");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "格式输入错误");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, "'s format is error");
								}
							}
							else if (attribute is StringLengthValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_StringLengthIncludingMinimum");

								StringLengthValidation stringLengthAttribute = attribute as StringLengthValidation;
								if (stringLengthAttribute.MinimumLength > 0 && !resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "输入长度必须是{1}至{0}个字符");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, "'s length is between {1} to {0} characters");
								}
								resName = string.Concat(groupName, "_", property.Name, "_StringLength");
								if (stringLengthAttribute.MinimumLength == 0 && !resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "输入长度必须小于{0}字符");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, "'s length is smaller than {0} characters");
								}
							}
						}
						#endregion
					}
					#endregion

					#region 添加条件模型属性本地化资源：属性
					foreach (DataConditionPropertyElement property in entity.Condition.Arguments)
					{
						string resName = property.DisplayName.DisplayName;
						if (string.IsNullOrWhiteSpace(resName)) { resName = string.Concat(groupName, "_", property.Name); }
						if (existResourceCollection.ContainsName(resName)) { continue; }
						if (!resourceCollection.ContainsName(resName))
						{
							LocalizationItem resx = resourceCollection.Add(groupName, resName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}

						resName = string.Concat(publicGroupName, "_", property.Name);
						if (existResourceCollection.ContainsName(resName)) { continue; }
						if (!resourceCollection.ContainsName(resName))
						{
							LocalizationItem resx = resourceCollection.Add(publicGroupName, resName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}

						string promptName = property.DisplayName.Prompt;
						if (!string.IsNullOrWhiteSpace(promptName) && !resourceCollection.ContainsName(promptName))
						{
							LocalizationItem resx = resourceCollection.Add(groupName, promptName, property.Comment);
							if (cultureInfo != null)
								resx[cultureInfo.Name] = property.Name;
						}
					}
					#endregion

					#region 添加命令错误返回资源
					string entityPrefixion = string.Concat(groupName, "_");
					foreach (DataCommandElement dataCommand in entity.DataCommands)
					{
						if (dataCommand is StaticCommandElement)
						{
							StaticCommandElement staticCommand = dataCommand as StaticCommandElement;
							foreach (CheckedCommandElement checkCommand in staticCommand.CheckCommands)
							{
								string resName = checkCommand.ErrorCode;
								if (resName != null && resName.StartsWith(entityPrefixion) && !resourceCollection.ContainsName(resName))
								{
									if (!string.IsNullOrWhiteSpace(checkCommand.PropertyName))
									{
										DesignColumnInfo column = entity.Persistent.TableInfo.Columns[checkCommand.PropertyName];
										if (column != null && column.DbType == DbTypeEnum.Timestamp)
										{
											LocalizationItem resx = resourceCollection.Add(groupName, resName, "并发错误，因修改的数据不是最新数据");
											if (cultureInfo != null) { resx[cultureInfo.Name] = "Concurrent error, because the modified data is'nt the latest data"; }
										}
										else
										{
											LocalizationItem resx = resourceCollection.Add(groupName, resName, "值'{0}' 已经存在，请重新输入");
											if (cultureInfo != null) { resx[cultureInfo.Name] = "Value '{0}' is exists. Please enter again"; }
										}
									}
									else
									{
										LocalizationItem resx = resourceCollection.Add(groupName, resName, "值'{0}' 已经存在，请重新输入");
										if (cultureInfo != null) { resx[cultureInfo.Name] = "Value '{0}' is exists. Please enter again"; }
									}
								}
							}
						}
					}
					#endregion
				}
				CreateResWindow resourceView = new CreateResWindow(this, _Persistent, dteClass, resourceCollection);
				if (resourceView.ShowModal() == true)
				{
					string resInfo = StringUtils.GetString("Package_ResourceDescription");
					string successInfo = StringUtils.GetString("Package_CreateResourceSuccessful"); //当前数据持久类资源已经创建成功。
					ShowMessage(successInfo, resInfo);
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}

		private void OnCreatePropertyResource(object sender, EventArgs e)
		{
			try
			{
				PersistentConfiguration _Persistent = GetPersistentConfiguration();
				if (_Persistent == null) { return; }
				if (_Persistent.MessageConverter.IsEmpty) { ShowMessage("本地化资源文件没有选择，请选择"); return; }
				EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
				System.IO.FileInfo fileSolution = new System.IO.FileInfo(dteClass.Solution.FullName);
				string fullName = string.Concat(fileSolution.DirectoryName, _Persistent.MessageConverter.FileName);
				EnvDTE.ProjectItem resourceItem = dteClass.Solution.FindProjectItem(fullName);
				if (resourceItem == null)
				{
					string message = string.Concat("本地化资源文件\"", _Persistent.MessageConverter, "\"已经不存在，请重新选择。");
					ShowMessage(message);
					return;
				}
				LocalizationCollection existResourceCollection = new LocalizationCollection();
				string resourceFileName = System.IO.Path.GetFileName(fullName);
				existResourceCollection.Load(fullName, m => File.OpenRead(m));

				LocalizationCollection resourceCollection = new LocalizationCollection();
				resourceCollection.AddEnabledCultureInfos();
				CultureInfo cultureInfo = resourceCollection.GetCultureInfo(1033);
				string groupName = _Persistent.GroupName;
				string publicGroupName = _Persistent.PublicGroupName;

				foreach (DataEntityElement entity in _Persistent.DataEntities)
				{
					#region 添加实体模型属性本地化资源：属性，验证，布尔可选
					foreach (DataEntityPropertyElement property in entity.Properties)
					{
						string resName = property.DisplayName.DisplayName;
						string pubName = string.Concat(publicGroupName, "_", property.Name);

						if (string.IsNullOrWhiteSpace(resName)) { resName = string.Concat(groupName, "_", property.Name); }

						if (!Regex.IsMatch(resName, "^[A-Za-z0-9_]+$")) { continue; }
						if (existResourceCollection.ContainsName(resName, pubName)) { continue; }

						if (!resourceCollection.ContainsName(resName))
						{
							LocalizationItem resx = resourceCollection.Add(groupName, resName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}
						if (!resourceCollection.ContainsName(pubName))
						{
							LocalizationItem resx = resourceCollection.Add(publicGroupName, pubName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}
						if (property.Type != null && property.Type == typeof(bool))
						{
							resName = string.Concat(groupName, "_", property.Name, "_TrueText");
							pubName = string.Concat(publicGroupName, "_", property.Name, "_TrueText");
							if (existResourceCollection.ContainsName(resName, pubName)) { continue; }

							if (!resourceCollection.ContainsName(resName))
							{
								LocalizationItem resx = resourceCollection.Add(groupName, resName, "是");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "True"; }
							}
							if (!resourceCollection.ContainsName(pubName))
							{
								LocalizationItem resx = resourceCollection.Add(groupName, pubName, "是");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "True"; }
							}

							resName = string.Concat(groupName, "_", property.Name, "_FalseText");
							pubName = string.Concat(publicGroupName, "_", property.Name, "_FalseText");
							if (existResourceCollection.ContainsName(resName, pubName)) { continue; }

							if (!resourceCollection.ContainsName(resName))
							{
								LocalizationItem resx = resourceCollection.Add(groupName, resName, "否");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "False"; }
							}
							if (!resourceCollection.ContainsName(pubName))
							{
								LocalizationItem resx = resourceCollection.Add(groupName, pubName, "否");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "False"; }
							}
						}
						string promptName = property.DisplayName.Prompt;
						if (!string.IsNullOrWhiteSpace(promptName) && !resourceCollection.ContainsName(promptName))
						{
							LocalizationItem resx = resourceCollection.Add(groupName, promptName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}
						#region 添加验证资源
						foreach (AbstractAttribute attribute in property.Attributes)
						{
							if (attribute is RequiredValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_Required");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "必须输入");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null) { resx[cultureInfo.Name] = string.Concat(property.Name, " must input"); }
								}
							}
							else if (attribute is BoolRequiredValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_Required");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "必须输入");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null) { resx[cultureInfo.Name] = string.Concat(property.Name, " must input"); }
								}
							}
							else if (attribute is CompareValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_Compare");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "输入数据错误");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null) { resx[cultureInfo.Name] = string.Concat(property.Name, "'s data is error"); }
								}
							}
							else if (attribute is MaxLengthValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_MaxLength");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "输入长度必须小于\"{0}\"");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null) { resx[cultureInfo.Name] = string.Concat(property.Name, "'s length must be less than or equal to {0} characters"); }
								}
							}
							else if (attribute is RangeValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_Range");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "必须输入\"{0}\"至\"{1}\"范围内的数据");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null) { resx[cultureInfo.Name] = string.Concat(property.Name, "'s data must be between \"{0}\" to \"{1}\""); }
								}
							}
							else if (attribute is RegularExpressionValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_RegularExpression");
								if (!resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "格式输入错误");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null) { resx[cultureInfo.Name] = string.Concat(property.Name, "'s format is error"); }
								}
							}
							else if (attribute is StringLengthValidation)
							{
								resName = string.Concat(groupName, "_", property.Name, "_StringLengthIncludingMinimum");

								StringLengthValidation stringLengthAttribute = attribute as StringLengthValidation;
								if (stringLengthAttribute.MinimumLength > 0 && !resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "输入长度必须是{1}至{0}个字符");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null) { resx[cultureInfo.Name] = string.Concat(property.Name, "'s length is between {1} to {0} characters"); }
								}
								resName = string.Concat(groupName, "_", property.Name, "_StringLength");
								if (stringLengthAttribute.MinimumLength == 0 && !resourceCollection.ContainsName(resName))
								{
									string value = string.Concat(property.Comment, "输入长度必须小于{0}字符");
									LocalizationItem resx = resourceCollection.Add(groupName, resName, value);
									if (cultureInfo != null)
										resx[cultureInfo.Name] = string.Concat(property.Name, "'s length is smaller than {0} characters");
								}
							}
						}
						#endregion
					}
					#endregion

					#region 添加条件模型属性本地化资源：属性
					foreach (DataConditionPropertyElement property in entity.Condition.Arguments)
					{
						string resName = property.DisplayName.DisplayName;
						if (string.IsNullOrWhiteSpace(resName)) { resName = string.Concat(groupName, "_", property.Name); }
						if (!resourceCollection.ContainsName(resName))
						{
							LocalizationItem resx = resourceCollection.Add(groupName, resName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}

						string promptName = property.DisplayName.Prompt;
						if (!string.IsNullOrWhiteSpace(promptName) && !resourceCollection.ContainsName(promptName))
						{
							LocalizationItem resx = resourceCollection.Add(groupName, promptName, property.Comment);
							if (cultureInfo != null) { resx[cultureInfo.Name] = property.Name; }
						}
					}
					#endregion
				}
				CreateResWindow resourceView = new CreateResWindow(this, _Persistent, dteClass, resourceCollection);
				if (resourceView.ShowModal() == true)
				{
					string resInfo = StringUtils.GetString("Package_ResourceDescription");
					string successInfo = StringUtils.GetString("Package_CreateResourceSuccessful"); //当前数据持久类资源已经创建成功，且已经复制进系统剪贴板。
					ShowMessage(successInfo, resInfo);
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}

		private void OnCreateCommandResource(object sender, EventArgs e)
		{
			try
			{
				PersistentConfiguration _Persistent = GetPersistentConfiguration();
				if (_Persistent == null) { return; }
				if (_Persistent.MessageConverter.IsEmpty) { ShowMessage("本地化资源文件没有选择，请选择"); return; }
				EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
				System.IO.FileInfo fileSolution = new System.IO.FileInfo(dteClass.Solution.FullName);
				string fullName = string.Concat(fileSolution.DirectoryName, _Persistent.MessageConverter.FileName);
				EnvDTE.ProjectItem resourceItem = dteClass.Solution.FindProjectItem(fullName);
				if (resourceItem == null)
				{
					string message = string.Concat("本地化资源文件\"", _Persistent.MessageConverter, "\"已经不存在，请重新选择。");
					ShowMessage(message);
					return;
				}
				LocalizationCollection notExistsCollection = new LocalizationCollection();
				notExistsCollection.AddEnabledCultureInfos();
				CultureInfo cultureInfo = notExistsCollection.GetCultureInfo(1033);
				string groupName = _Persistent.GroupName;
				foreach (DataEntityElement entity in _Persistent.DataEntities)
				{
					#region 添加命令错误返回资源
					string entityPrefixion = string.Concat(groupName, "_");
					foreach (DataCommandElement dataCommand in entity.DataCommands)
					{
						if (!(dataCommand is StaticCommandElement)) { continue; }
						if (!(dataCommand is StaticCommandElement staticCommand)) { continue; }
						foreach (CheckedCommandElement checkCommand in staticCommand.CheckCommands)
						{
							string resName = checkCommand.ErrorCode;
							if (string.IsNullOrWhiteSpace(resName)) { continue; }
							if (notExistsCollection.ContainsName(resName)) { continue; }
							if (!string.IsNullOrWhiteSpace(checkCommand.PropertyName))
							{
								DesignColumnInfo column = entity.Persistent.TableInfo.Columns[checkCommand.PropertyName];
								if (column != null && column.DbType == DbTypeEnum.Timestamp)
								{
									LocalizationItem resx = notExistsCollection.Add(groupName, resName, "并发错误，因修改的数据不是最新数据");
									if (cultureInfo != null) { resx[cultureInfo.Name] = "Concurrent error, because the modified data is'nt the latest data"; }
								}
								else
								{
									LocalizationItem resx = notExistsCollection.Add(groupName, resName, "值'{0}' 已经存在，请重新输入");
									if (cultureInfo != null) { resx[cultureInfo.Name] = "Value '{0}' is exists. Please enter again"; }
								}
							}
							else
							{
								LocalizationItem resx = notExistsCollection.Add(groupName, resName, "值'{0}' 已经存在，请重新输入");
								if (cultureInfo != null) { resx[cultureInfo.Name] = "Value '{0}' is exists. Please enter again"; }
							}
						}
					}
					#endregion
				}
				CreateResWindow resourceView = new CreateResWindow(this, _Persistent, dteClass, notExistsCollection);
				if (resourceView.ShowModal() == true)
				{
					string resInfo = StringUtils.GetString("Package_ResourceDescription");
					string successInfo = StringUtils.GetString("Package_CreateResourceSuccessful"); //当前数据持久类资源已经创建成功，且已经复制进系统剪贴板。
					ShowMessage(successInfo, resInfo);
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}

		private void OnResetResource(object sender, EventArgs e)
		{
			try
			{
				PersistentConfiguration _Persistent = GetPersistentConfiguration();
				if (_Persistent == null) { return; }
				if (_Persistent.MessageConverter.IsEmpty) { ShowMessage("本地化资源文件没有选择，请选择"); return; }
				EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
				System.IO.FileInfo fileSolution = new System.IO.FileInfo(dteClass.Solution.FullName);
				string fullName = string.Concat(fileSolution.DirectoryName, _Persistent.MessageConverter.FileName);
				EnvDTE.ProjectItem resourceItem = dteClass.Solution.FindProjectItem(fullName);
				if (resourceItem == null)
				{
					string message = string.Concat("本地化资源文件\"", _Persistent.MessageConverter, "\"已经不存在，请重新选择。");
					ShowMessage(message);
					return;
				}

				LocalizationCollection resourceCollection = new LocalizationCollection();
				string resourceFileName = System.IO.Path.GetFileName(fullName);
				resourceCollection.Load(fullName, m => File.OpenRead(m));

				System.Collections.Generic.List<string> notExists = new System.Collections.Generic.List<string>(200);
				#region 设置实体属性本地化资源信息
				string groupName = _Persistent.GroupName; string publicGroup = _Persistent.PublicGroupName;
				foreach (DataEntityElement entity in _Persistent.DataEntities)
				{
					#region 设置实体属性本地化资源信息
					foreach (DataEntityPropertyElement property in entity.Properties)
					{
						if (string.IsNullOrWhiteSpace(property.DisplayName.DisplayName))
						{
							string resName = string.Concat(groupName, "_", property.Name);
							string resKeyName = "", pubKeyName = "";
							if (property.Name.EndsWith("Name", StringComparison.OrdinalIgnoreCase) == true)
							{
								resKeyName = string.Concat(groupName, "_", property.Name.TrimSuffix("Name"), "Key");
								pubKeyName = string.Concat(publicGroup, "_", property.Name.TrimSuffix("Name"), "Key");
							}
							else if (property.Name.EndsWith("Text", StringComparison.OrdinalIgnoreCase) == true)
							{
								resKeyName = string.Concat(groupName, "_", property.Name.TrimSuffix("Text"), "Key");
								pubKeyName = string.Concat(publicGroup, "_", property.Name.TrimSuffix("Text"), "Key");
							}

							string respubName = string.Concat(publicGroup, "_", property.Name);
							if (resourceCollection.ContainsName(resName))
							{
								property.DisplayName.ConverterName = resourceCollection.FileName;
								property.DisplayName.DisplayName = resName;
							}
							else if (resourceCollection.ContainsName(respubName))
							{
								property.DisplayName.ConverterName = resourceCollection.FileName;
								property.DisplayName.DisplayName = respubName;
							}
							else if (resourceCollection.ContainsName(resKeyName))
							{
								property.DisplayName.ConverterName = resourceCollection.FileName;
								property.DisplayName.DisplayName = resKeyName;
							}
							else if (resourceCollection.ContainsName(pubKeyName))
							{
								property.DisplayName.ConverterName = resourceCollection.FileName;
								property.DisplayName.DisplayName = pubKeyName;
							}
							else
							{
								notExists.Add(resName);
								//resBuilder.AppendLine().Append(resName);
							}
						}
					}
					#endregion

					#region 设置条件模型属性本地化资源：属性
					foreach (DataConditionPropertyElement property in entity.Condition.Arguments)
					{
						if (string.IsNullOrWhiteSpace(property.DisplayName.DisplayName))
						{
							string resName = string.Concat(groupName, "_", property.Name);
							string respubName = string.Concat(publicGroup, "_", property.Name);
							string resKeyName = "", pubKeyName = "";
							if (property.Name.EndsWith("Name", StringComparison.OrdinalIgnoreCase) == true)
							{
								resKeyName = string.Concat(groupName, "_", property.Name.TrimSuffix("Name"), "Key");
								pubKeyName = string.Concat(publicGroup, "_", property.Name.TrimSuffix("Name"), "Key");
							}
							else if (property.Name.EndsWith("Text", StringComparison.OrdinalIgnoreCase) == true)
							{
								resKeyName = string.Concat(groupName, "_", property.Name.TrimSuffix("Text"), "Key");
								pubKeyName = string.Concat(publicGroup, "_", property.Name.TrimSuffix("Text"), "Key");
							}
							if (resourceCollection.ContainsName(resName))
							{
								property.DisplayName.ConverterName = resourceCollection.FileName;
								property.DisplayName.DisplayName = resName;
							}
							else if (resourceCollection.ContainsName(respubName))
							{
								property.DisplayName.ConverterName = resourceCollection.FileName;
								property.DisplayName.DisplayName = respubName;
							}
							else if (resourceCollection.ContainsName(resKeyName))
							{
								property.DisplayName.ConverterName = resourceCollection.FileName;
								property.DisplayName.DisplayName = resKeyName;
							}
							else if (resourceCollection.ContainsName(pubKeyName))
							{
								property.DisplayName.ConverterName = resourceCollection.FileName;
								property.DisplayName.DisplayName = pubKeyName;
							}
							else
							{
								notExists.Add(resName);
							}
						}
					}
					#endregion

					#region 设置实体属性本地化资源信息
					//foreach (DataCommandElement dataCommand in entity.DataCommands)
					//{
					//	if (dataCommand is StaticCommandElement)
					//	{
					//		StaticCommandElement staticCommand = dataCommand as StaticCommandElement;
					//		foreach (CheckedCommandElement checkCommand in staticCommand.CheckCommands)
					//		{
					//			string resName = checkCommand.ErrorCode;
					//			if (resName != null && resourceCollection.ContainsName(resName))
					//			{
					//				checkCommand.Converter = resourceCollection.FileName;
					//			}
					//		}
					//	}
					//}
					#endregion
				}
				#endregion
				string resInfo = StringUtils.GetString("Package_ResourceIsNotExists", resourceFileName);
				if (notExists.Count > 0)
				{
					ShowMessage(string.Concat(resInfo, Environment.NewLine, string.Join(Environment.NewLine, notExists.Distinct())));
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}
		#endregion

		#region 修改项目配置文件信息(添加/更新数据库连接)
		private void OnShowConfiguration(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			OleMenuCommand menu = sender as OleMenuCommand;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			menu.Enabled = menu.Visible = false;
			if (!(dteClass.ToolWindows.SolutionExplorer.SelectedItems is Array array) || array.Length == 0) { return; }
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				if (item.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
					if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".config"))
					{
						menu.Enabled = menu.Visible = true; return;
					}
				}
			}
		}

		private void OnCanResetConnection(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			menu.Enabled = menu.Visible = false;
			if (array == null || array.Length == 0) { return; }
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				if (item.Object is EnvDTE.ProjectItem pItem)
				{
					if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".config"))
					{
						menu.Enabled = menu.Visible = true; return;
					}
				}
			}
		}

		private void OnResetConnection(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array == null || array.Length == 0) { return; }
			string kindPhysicalFile = EnvDTE.Constants.vsProjectItemKindPhysicalFile;
			foreach (EnvDTE.UIHierarchyItem uihItem in array)
			{
				if (uihItem.Object is EnvDTE.ProjectItem projectItem)
				{
					if (projectItem.Kind == kindPhysicalFile && projectItem.Name.EndsWith(".config", StringComparison.CurrentCultureIgnoreCase))
					{
						EnvDTE.Property pFullPath = projectItem.Properties.Item("FullPath");
						ConnectionContext.InitializeConfiguration((string)pFullPath.Value);
					}
				}
			}
		}

		private void OnCanAddConnection(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			menu.Enabled = menu.Visible = false;
			if (array == null || array.Length == 0) { return; }
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				if (item.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
					if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".config"))
					{
						menu.Enabled = menu.Visible = true; return;
					}
				}
			}
		}

		private void OnAddConnection(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array == null || array.Length == 0) { return; }
			string kindPhysicalFile = EnvDTE.Constants.vsProjectItemKindPhysicalFile;
			foreach (EnvDTE.UIHierarchyItem uihItem in array)
			{
				if (uihItem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem projectItem = uihItem.Object as EnvDTE.ProjectItem;
					if (projectItem.Kind == kindPhysicalFile && projectItem.Name.EndsWith(".config", StringComparison.CurrentCultureIgnoreCase))
					{
						MDC.DataConnectionDialog dlgDataConnection = new MDC.DataConnectionDialog();
						dlgDataConnection.DataSources.Add(MDC.DataSource.AccessDataSource); // Access 
						dlgDataConnection.DataSources.Add(MDC.DataSource.OracleDataSource); // Oracle 
						dlgDataConnection.DataSources.Add(MDC.DataSource.SqlDataSource); // Sql Server
						dlgDataConnection.DataSources.Add(MDC.DataSource.SqlFileDataSource); // Sql File Server
																							 // 初始化
						dlgDataConnection.SelectedDataSource = MDC.DataSource.SqlDataSource;
						dlgDataConnection.SelectedDataProvider = MDC.DataProvider.SqlDataProvider;
						if (MDC.DataConnectionDialog.Show(dlgDataConnection) == System.Windows.Forms.DialogResult.OK)
						{
							EnvDTE.Property pFullPath = projectItem.Properties.Item("FullPath");
							//Type type = dlgDataConnection.SelectedDataProvider.TargetConnectionType;
							//string name = dlgDataConnection.SelectedDataSource.Name;
							string connection = dlgDataConnection.ConnectionString;
							SaveConfiguration((string)pFullPath.Value, connection, dlgDataConnection.SelectedDataProvider);
						}
					}
				}
			}
		}

		private void SaveConfiguration(string fullName, string connection, MDC.DataProvider dataProvider)
		{
			SC.Configuration configuration = ReadConfigurationFile(fullName);
			SC.ConfigurationSectionGroup sectionGroup = GetSectionGroup(configuration);
			ConnectionsSection sectionConnections = GetConnectionsSection(sectionGroup);
			ConnectionElement element = new ConnectionElement
			{
				Name = string.Concat("Connection_", sectionConnections.Connections.Count)
			};
			if (string.IsNullOrEmpty(sectionConnections.DefaultName))
				sectionConnections.DefaultName = element.Name;
			if (dataProvider == MDC.DataProvider.OracleDataProvider)
				element.ConnectionType = ConnectionType.OracleConnection;
			else if (dataProvider == MDC.DataProvider.OdbcDataProvider)
				element.ConnectionType = ConnectionType.OdbcConnection;
			else if (dataProvider == MDC.DataProvider.OleDBDataProvider)
				element.ConnectionType = ConnectionType.OleDbConnection;
			else { element.ConnectionType = ConnectionType.SqlConnection; }

			DbConnectionBuilder builder = new DbConnectionBuilder(connection);
			foreach (string key in builder.Keys)
			{
				ConnectionItem item = new ConnectionItem() { Name = key, Value = builder[key] };
				if (string.Compare(item.Name, "Password", true) == 0) { item.Value = ConfigurationAlgorithm.Encryption(item.Value); }
				element.Values.Add(item);
			}

			sectionConnections.Connections.Add(element);
			configuration.Save(SC.ConfigurationSaveMode.Modified);
		}

		/// <summary>
		/// 读取配置文件信息。
		/// </summary>
		/// <param name="fileConfiguration"></param>
		private SC.Configuration ReadConfigurationFile(string fileConfiguration)
		{
			SC.ConfigurationFileMap fileMap = new SC.ConfigurationFileMap(fileConfiguration);
			return SC.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
		}

		/// <summary>从配置文件中读取 ConnectionsSection 节</summary>
		/// <param name="configuration">项目配置文件</param>
		/// <returns>返回 ConnectionsSection 配置节</returns>
		private ConfigurationGroup GetSectionGroup(SC.Configuration configuration)
		{
			SC.ConfigurationSectionGroup group = configuration.GetSectionGroup(ConfigurationGroup.ElementName);
			if (group is ConfigurationGroup groupSection)
			{
				return groupSection;
			}
			groupSection = new ConfigurationGroup();
			configuration.SectionGroups.Add(ConfigurationGroup.ElementName, groupSection);
			return groupSection;
		}

		/// <summary>
		/// 读取配置文件信息。
		/// </summary>
		/// <param name="groupConfiguration"></param>
		private ConnectionsSection GetConnectionsSection(SC.ConfigurationSectionGroup group)
		{
			SC.ConfigurationSection section = group.Sections[ConnectionsSection.ElementName];
			ConnectionsSection connections = section as ConnectionsSection;
			if (section == null)
			{
				connections = new ConnectionsSection();
				group.Sections.Add(ConnectionsSection.ElementName, connections);
			}
			return connections;
		}
		#endregion

		/// <summary>Returns the project in the solution, given a unique name.</summary>
		/// <param name="uniqueName"> [in] Unique name for the project.</param>
		/// <param name="hierarchy">
		/// Pointer to the Microsoft.VisualStudio.Shell.Interop.IVsHierarchy 
		/// interface of the project referred to by pszUniqueName.
		///</param>
		///<returns><![CDATA[If the method succeeds, it returns Microsoft.VisualStudio.VSConstants.S_OK.
		///If it fails, it returns an error code.]]></returns>
		internal int GetProjectOfUniqueName(string uniqueName, out IVsHierarchy hierarchy)
		{
			return _VsSolution.GetProjectOfUniqueName(uniqueName, out hierarchy);
		}

		internal void WriteError(EnvDTE.ProjectItem item, string msg, string helpLink, int column = 0, int line = 0)
		{
			_VsSolution.GetProjectOfUniqueName(item.ContainingProject.UniqueName, out IVsHierarchy hierarchy);
			errorListProvider.Tasks.Add(new ErrorTask
			{
				Category = TaskCategory.User,
				ErrorCategory = TaskErrorCategory.Error,
				Text = msg,
				HelpKeyword = helpLink,
				Document = item.Name,
				Column = column,
				Line = line,
				CanDelete = true,
				HierarchyItem = hierarchy
			});
			errorListProvider.Show();
		}

		internal void WriteError(EnvDTE.ProjectItem item, Exception ex)
		{
			_VsSolution.GetProjectOfUniqueName(item.ContainingProject.UniqueName, out IVsHierarchy hierarchy);
			errorListProvider.Tasks.Add(new ErrorTask
			{
				Category = TaskCategory.User,
				ErrorCategory = TaskErrorCategory.Error,
				Text = ex.Message,
				HelpKeyword = ex.HelpLink,
				Document = item.Name,
				Column = 0,
				Line = 0,
				CanDelete = true,
				HierarchyItem = hierarchy
			});
			errorListProvider.Show();
		}

		private const string outputTitle = "Asp.Net Mvc Persistents";
		private static string outputGuid;
		internal void WriteToOutput(string message)
		{
			EnvDTE.OutputWindow window = _DteClass.ToolWindows.OutputWindow;
			window.Parent.Activate();
			foreach (EnvDTE.OutputWindowPane pane in window.OutputWindowPanes)
			{
				if (pane.Guid == outputGuid) { pane.Activate(); pane.OutputString(message); pane.OutputString("\r\n"); return; }
			}
			EnvDTE.OutputWindowPane newPane = window.OutputWindowPanes.Add(outputTitle);
			outputGuid = newPane.Guid;
			newPane.Activate(); newPane.OutputString(message); newPane.OutputString("\r\n");
		}

		internal void WriteToOutput(string format, params object[] args)
		{
			WriteToOutput(string.Format(format, args));
		}

		/// <summary>根据</summary>
		/// <param name="project"></param>
		/// <returns></returns>
		public CodeDomProvider CreateCodeProvider(EnvDTE.Project project)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			string language = project.CodeModel.Language;
			if (language == EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB)
				return CodeDomProvider.CreateProvider("VB");
			else if (language == EnvDTE.CodeModelLanguageConstants.vsCMLanguageVC)
				return CodeDomProvider.CreateProvider("C++");
			else if (language == EnvDTE.CodeModelLanguageConstants.vsCMLanguageMC)
				return CodeDomProvider.CreateProvider("MC");
			else if (language == EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp)
				return CodeDomProvider.CreateProvider("C#");
			return CodeDomProvider.CreateProvider("C#");
		}

		/// <summary>
		/// 执行菜单 Properties 命令  
		/// </summary>
		private void OnProperties(object sender, EventArgs e)
		{
			ShowPropertyWindow();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD102:异步实现内部逻辑", Justification = "<挂起>")]
		private IVsMonitorSelection GetMonitorSelection()
		{
			return ThreadHelper.JoinableTaskFactory.Run(async () =>
			{
				return await asyncPackage.GetServiceAsync(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
			});
		}

		private DesignerEntitiesCanvas GetItemsCanvas()
		{
			IVsMonitorSelection monitorSelection = GetMonitorSelection();
			//monitorSelection.GetCurrentElementValue(0, out value0);
			monitorSelection.GetCurrentElementValue(1, out object objectFrame);
			// monitorSelection.GetCurrentElementValue(2, out value2);
			if (objectFrame is IVsWindowFrame frame)
			{
				frame.GetProperty(-3001, out object pane);//获取 WindowPane
				if (pane is PersistentPane modelPane) { return modelPane.Content as DesignerEntitiesCanvas; }
			}
			return null;
		}

		private PersistentConfiguration GetPersistentConfiguration()
		{
			IVsMonitorSelection monitorSelection = GetMonitorSelection();
			//monitorSelection.GetCurrentElementValue(0, out value0);
			monitorSelection.GetCurrentElementValue(1, out object objectFrame);
			// monitorSelection.GetCurrentElementValue(2, out value2);
			if (objectFrame is IVsWindowFrame frame)
			{
				frame.GetProperty(-3001, out object pane);//获取 WindowPane
				if (pane is PersistentPane modelPane) { return modelPane.GetPersistent(); }
			}
			return null;
		}

		private async STT.Task<PersistentPane> GetPersistentPaneAsync()
		{
			IVsMonitorSelection monitorSelection = await asyncPackage.GetServiceAsync(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
			Assumes.Present(monitorSelection);
			monitorSelection.GetCurrentElementValue(1, out object objFrame);
			if (objFrame is IVsWindowFrame frame)
			{
				frame.GetProperty(-3001, out object pane);//获取 WindowPane
				if (pane != null) { return pane as PersistentPane; }
			}
			return null;
		}

		private PersistentPane GetPersistentPane()
		{
			IVsMonitorSelection monitorSelection = GetMonitorSelection();
			//monitorSelection.GetCurrentElementValue(0, out object value0);
			monitorSelection.GetCurrentElementValue(1, out object objectFrame);
			//monitorSelection.GetCurrentElementValue(2, out object value2);
			if (objectFrame is IVsWindowFrame frame)
			{
				frame.GetProperty(-3001, out object pane);//获取 WindowPane
				if (pane != null) { return pane as PersistentPane; }
			}
			return null;
		}

		private void OnCanSizeToFit(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			menu.Enabled = menu.Visible = canvas != null;
		}

		private void OnSizeToFit(object sender, EventArgs e)
		{
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			if (canvas == null) { return; }
			if (canvas.SelectedItem != null)
			{
				canvas.SelectedItem.Height = double.NaN;
				canvas.SelectedItem.Width = double.NaN;
			}
			else
			{
				foreach (DataEntityElement entity in canvas.Items)
				{
					DesignerEntity item = canvas.ItemContainerGenerator.ContainerFromItem(entity) as DesignerEntity;
					item.Height = double.NaN; item.Width = double.NaN;
				}
			}
		}

		private void OnOrderItem(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			PersistentConfiguration persistent = GetPersistentConfiguration();
			if (canvas != null && canvas.SelectedItem != null)
			{
				DataEntityElement entity = canvas.SelectedItem.DataContext as DataEntityElement;

				//UIElement element = canvas.SelectedItem as UIElement;
				if (menu.CommandID == StandardCommands.SendToBack)
				{
					persistent.DataEntities.MoveToLast(entity);
				}
				else if (menu.CommandID == StandardCommands.SendBackward)
				{
					persistent.DataEntities.Move(entity, 1);
				}
				else if (menu.CommandID == StandardCommands.BringForward)
				{
					persistent.DataEntities.Move(entity, -1);
				}
				else if (menu.CommandID == StandardCommands.BringToFront)
				{
					persistent.DataEntities.MoveToFirst(entity);
					//FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
					//int count = VisualTreeHelper.GetChildrenCount(parent), zIndex = 0;
					//for (int index = 0; index < count; index++)
					//	zIndex = Math.Max(zIndex, Canvas.GetZIndex(VisualTreeHelper.GetChild(parent, index) as UIElement));
					//Canvas.SetZIndex(element, zIndex + 1);
				}
			}
		}

		/// <summary>
		/// 查询执行菜单 StandardCommands.Cut 命令 
		/// </summary>
		private void OnSelectedVisibled(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem != null;
		}
	}

	internal static class OleMenuCommandServiceExtension
	{
		internal static void AddCommand(this OleMenuCommandService _oleMenuService, CommandID id, EventHandler invokeHandler, EventHandler beforeQueryStatus, EventHandler changeHandler)
		{
			_oleMenuService.AddCommand(new OleMenuCommand(invokeHandler, changeHandler, beforeQueryStatus, id));
		}

		internal static void AddCommand(this OleMenuCommandService _oleMenuService, CommandID id, EventHandler invokeHandler, EventHandler beforeQueryStatus)
		{
			_oleMenuService.AddCommand(new OleMenuCommand(invokeHandler, null, beforeQueryStatus, id));
		}

		internal static void AddCommand(this OleMenuCommandService _oleMenuService, CommandID id, EventHandler invokeHandler)
		{
			_oleMenuService.AddCommand(new OleMenuCommand(invokeHandler, id));
		}
	}
}
