using Basic.Configuration;
using Basic.Properties;

namespace Basic.Localizations
{
	/// <summary>
	/// CreateLocalizationDialog.xaml 的交互逻辑
	/// </summary>
	public partial class CreateLocalizationDialog : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly string _folderPath;

		public CreateLocalizationDialog(string folderPath, string defaultName)
		{
			InitializeComponent(); _folderPath = folderPath;
			if (defaultName == "") { defaultName = "Strings"; }
			if (defaultName.EndsWith(PersistentService.LocalizedExtension) == false)
			{
				defaultName = string.Concat(defaultName, PersistentService.LocalizedExtension);
			}
			txtName.Text = defaultName;
			Title = StringUtils.GetString("Package_CreateLocalizationDialog_Title");
		}

		/// <summary>新建的文件名称</summary>
		public string FileName { get { return txtName.Text.Trim(); } }

		private void OnSaveExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtName.Text) == false)
			{
				string fileName = txtName.Text.Trim();
				if (fileName.EndsWith(PersistentService.LocalizedExtension) == false)
				{
					fileName = string.Concat(fileName, PersistentService.LocalizedExtension);
				}

				string filePath = string.Concat(_folderPath, "\\", fileName);
				if (System.IO.File.Exists(filePath))
				{
					lblErrors.Text = string.Concat("文件\"", filePath, "\"已经存在，请修改文件名");
				}
				else { this.DialogResult = true; base.Close(); }
			}
		}

		private void txtName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtName.Text) == false)
			{
				string fileName = txtName.Text.Trim();
				if (fileName.EndsWith(PersistentService.LocalizedExtension))
				{
					txtName.Select(0, fileName.Length - 10);
				}
				else { txtName.Select(0, fileName.Length); }
			}
		}

		private void txtName_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			lblErrors.Text = "";
		}
	}
}
