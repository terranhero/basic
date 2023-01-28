using System.Windows.Controls;
using System.Windows.Input;
using Basic.Configuration;
using Basic.Localizations;

namespace Basic.Windows
{
	/// <summary>
	/// CreateResWindow.xaml 的交互逻辑
	/// </summary>
	public partial class CreateResWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly EnvDTE80.DTE2 _DTEClass;
		private readonly LocalizationCollection _ResourceCollection;
		//private readonly ObservableCollection<MessageInfo> _MessageInfos;
		private readonly PersistentService _CommandService;
		private readonly PersistentConfiguration _Persistent;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		public CreateResWindow(PersistentService commandService, PersistentConfiguration persistent,
			EnvDTE80.DTE2 dteClass, LocalizationCollection resources)
			: base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
		{
			_DTEClass = dteClass; _Persistent = persistent;
			_CommandService = commandService; _ResourceCollection = resources;

			InitializeComponent();
			System.IO.FileInfo fileSolution = new System.IO.FileInfo(dteClass.Solution.FullName);
			string localFilePath = string.Concat(fileSolution.DirectoryName, persistent.MessageConverter.FileName);
			_ProjectItem = dteClass.Solution.FindProjectItem(localFilePath);
			lblLocalResx.Content = string.Concat("本地化资源文件：", persistent.MessageConverter.FileName);
			dgEnums.ItemsSource = _ResourceCollection;
			btnOk.IsEnabled = _ResourceCollection.Count > 0;
		}

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_CommandService.SetWaitCursor();
			if (_ProjectItem == null)
			{
				string message = string.Concat("本地化资源文件\"", _Persistent.MessageConverter.ConverterName, "\"不存在，请重新选择。");
				_CommandService.ShowMessage(message); return;
			}

			if (!_ProjectItem.IsOpen) { _ProjectItem.Open(); } _ProjectItem.Document.Activate();
			LocalizationPane pane = _ProjectItem.Document.ActiveWindow.Object as LocalizationPane;
			if (pane == null)
			{
				string message = string.Concat("本地化资源文件\"", _Persistent.MessageConverter.ConverterName,
					"\"打开失败，可能原因是本地化资源编辑器已经更改。");
				_CommandService.ShowMessage(message); return;
			}
			ResourceEditor editor = pane.Content as ResourceEditor;
			foreach (LocalizationItem resx in _ResourceCollection)
			{
				if (resx.Created) { editor.Add(resx); }
			}
			_ProjectItem.Save();
			this.DialogResult = true;
			base.Close();
		}

		private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			CheckBox chk = sender as CheckBox;
			if (chk != null)
			{
				foreach (LocalizationItem resx in _ResourceCollection)
				{
					resx.Created = chk.IsChecked ?? false;
				}
			}
		}
	}
}
