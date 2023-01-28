
using Basic.Builders;
using Basic.Configuration;
using Microsoft.VisualStudio.TextTemplating;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace Basic.Windows
{
	/// <summary>
	/// CommandWindow.xaml 的交互逻辑
	/// </summary>
	public partial class CreateController : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly ControllerBuilder _Builder;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		private readonly EnvDTE.Project _Project;
		private readonly PersistentService _CommandService;
		public CreateController(PersistentService commandService, EnvDTE.Project project, EnvDTE.ProjectItem item)
			: base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
		{
			_ProjectItem = item; _Project = project;
			_CommandService = commandService;
			_Builder = new ControllerBuilder(commandService, project, item);
			InitializeComponent();
			DataContext = _Builder;
		}

		private void DialogWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_ = Dispatcher.BeginInvoke(new System.Action(() =>
			  {
				  _Builder.InitlizeFiles();
			  }), null);
		}

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_CommandService.SetWaitCursor();
			CodeDomProvider provider = _CommandService.CreateCodeProvider(_Project);
			EnvDTE.Property propertyNamespace = _Project.Properties.Item("DefaultNamespace");
			EnvDTE.Property propertyFullPath = _Project.Properties.Item("FullPath");
			if (_ProjectItem != null)
			{
				propertyNamespace = _ProjectItem.Properties.Item("DefaultNamespace");
				propertyFullPath = _ProjectItem.Properties.Item("FullPath");
			}
			string fullPath = (string)propertyFullPath.Value;
			string defaultNamespace = (string)propertyNamespace.Value;
			string fullName = string.Concat(fullPath, _Builder.ControllerClass, ".", provider.FileExtension);
			if (File.Exists(fullName))
			{
				string message = string.Format("文件\"{0}\"已经存在，是否覆盖？", fullName);
				if (!_CommandService.Confirm(message)) { return; }
			}
			using (StreamWriter writer = new StreamWriter(fullName, false, Encoding.Unicode))
			{
				_Builder.GenerateCode(writer, defaultNamespace);
			}


			//CodeCompileUnit codeComplieUnit = new CodeCompileUnit();
			//_Builder.GenerateCode(codeComplieUnit, defaultNamespace);
			//CodeGeneratorOptions options = new CodeGeneratorOptions();
			//options.BlankLinesBetweenMembers = true;
			//options.BracingStyle = "C";// "C";
			//if (File.Exists(fullName))
			//{
			//	string message = string.Format("文件\"{0}\"已经存在，是否覆盖？", fullName);
			//	if (!_CommandService.Confirm(message)) { return; }
			//}
			//using (StreamWriter writer = new StreamWriter(fullName, false, Encoding.Unicode))
			//{
			//	provider.GenerateCodeFromCompileUnit(codeComplieUnit, writer, options);
			//}
			EnvDTE.ProjectItem itemController = null;
			EnvDTE.ProjectItems projectItems = _Project.ProjectItems;
			if (_ProjectItem != null) { projectItems = _ProjectItem.ProjectItems; }
			foreach (EnvDTE.ProjectItem item in projectItems)
			{
				propertyFullPath = item.Properties.Item("FullPath");
				if (fullName == (string)propertyFullPath.Value) { itemController = item; }
			}
			if (itemController == null) { itemController = projectItems.AddFromFile(fullName); }
			base.Close();
		}
	}
}
