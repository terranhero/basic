
using Basic.Builders;
using Basic.Configuration;
using Basic.Views;
using System.IO;
using System.Reflection;
using System.Windows.Input;
namespace Basic.Windows
{
	/// <summary>
	/// CommandWindow.xaml 的交互逻辑
	/// </summary>
	public partial class CreateWpfView : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly WpfFormBuilder _Builder;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		private readonly EnvDTE.Project _Project;
		private readonly PersistentService _CommandService;
		public CreateWpfView(PersistentService commandService, EnvDTE.Project project, EnvDTE.ProjectItem item)
			: base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
		{
			_Project = project; _ProjectItem = item; _CommandService = commandService;
			_Builder = new WpfFormBuilder(commandService, project, item);
			InitializeComponent();
			DataContext = _Builder;
		}

		private void DialogWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_Builder.InitlizeFiles();
		}

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_CommandService.SetWaitCursor();
			EnvDTE.ProjectItem itemFolder = _ProjectItem;
			EnvDTE.Property propertyFullPath = itemFolder.Properties.Item("FullPath");
			string fullPath = (string)propertyFullPath.Value;
			if (_Builder.ControllerVisibled)
			{
				foreach (EnvDTE.ProjectItem item in _ProjectItem.ProjectItems)
				{
					if (item.Name == _Builder.Controller) { itemFolder = item; }
				}
			}
			if (itemFolder == _ProjectItem && _Builder.ControllerVisibled)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
				DirectoryInfo[] subArray = directoryInfo.GetDirectories(_Builder.Controller);
				if (subArray != null && subArray.Length > 0)
					itemFolder = _ProjectItem.ProjectItems.AddFromDirectory(subArray[0].FullName);
				else
					itemFolder = _ProjectItem.ProjectItems.AddFolder(_Builder.Controller);
			}
			propertyFullPath = itemFolder.Properties.Item("FullPath");
			fullPath = (string)propertyFullPath.Value;
			base.Close();
		}
	}
}
