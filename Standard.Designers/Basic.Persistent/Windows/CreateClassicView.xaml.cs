
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
	public partial class CreateClassicView : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly ClassicViewBuilder _Builder;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		private readonly PersistentService _CommandService;
		public CreateClassicView(PersistentService commandService, EnvDTE.ProjectItem item)
			: base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
		{
			_ProjectItem = item; _CommandService = commandService;
			_Builder = new ClassicViewBuilder(commandService, item);
			InitializeComponent();
			EnvDTE.Property propertyFullPath = item.Properties.Item("FullPath");
			string fullPath = (string)propertyFullPath.Value;
			_Builder.ControllerVisibled = fullPath.EndsWith(@"\Views\");
			DataContext = _Builder;
		}

		private void DialogWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			// _Builder.InitilizeTemplates();
			_Builder.InitlizeFiles();
			Assembly _Assembly = GetType().Assembly;
			string test = _Assembly.Location;
			string local = _Assembly.CodeBase;
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
			foreach (MvcView view in _Builder.Views)
			{
				if (!view.Created) { continue; }
				fullPath = (string)propertyFullPath.Value;
				string fileName = string.Concat(view.Name, ".cshtml");
				fullPath = string.Concat(fullPath, fileName);
				if (File.Exists(fullPath))
				{
					if (!_CommandService.Confirm(string.Format("文件\"{0}\"已经存在，是否需要覆盖？", fullPath))) { continue; }
				}
				using (FileStream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
				{
					using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
					{
						view.WriteCode(writer);
					}
				}
				EnvDTE.ProjectItem itemFile = null;
				foreach (EnvDTE.ProjectItem item in itemFolder.ProjectItems)
				{
					if (item.Name == fileName) { itemFile = item; }
				}
				if (itemFile == null) { itemFolder.ProjectItems.AddFromFile(fullPath); }
			}
			base.Close();
		}
	}
}
