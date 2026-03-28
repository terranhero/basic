using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Basic.Configuration;
using Basic.Properties;
using Basic.Windows;

namespace Basic.Localizations
{
	/// <summary>
	/// LocalizationDialog.xaml 的交互逻辑
	/// </summary>
	public partial class LocalizationDialog : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		//static LocalizationDialog()
		//{
		//	DefaultStyleKeyProperty.OverrideMetadata(typeof(LocalizationDialog),
		//		new FrameworkPropertyMetadata(typeof(LocalizationDialog)));
		//}
		private readonly string _folderPath;

		public LocalizationDialog(string folderPath, string defaultName)
		{
			InitializeContent();
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSaveExecuted));

			_folderPath = folderPath;
			if (defaultName == "") { defaultName = "Strings"; }
			if (defaultName.EndsWith(PersistentService.LocalizedExtension) == false)
			{
				defaultName = string.Concat(defaultName, PersistentService.LocalizedExtension);
			}
			FileName = defaultName;
			Title = StringUtils.GetString("Package_CreateLocalizationDialog_Title");
		}

		#region 属性 FileName 定义
		public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName",
			typeof(string), typeof(LocalizationDialog), new PropertyMetadata(null));
		/// <summary>文件名称</summary>
		public string FileName
		{
			get { return (string)base.GetValue(FileNameProperty); }
			set { base.SetValue(FileNameProperty, value); }
		}
		#endregion

		#region 属性 ErrorText 定义
		public static readonly DependencyProperty ErrorTextProperty = DependencyProperty.Register("ErrorText",
			typeof(string), typeof(LocalizationDialog), new PropertyMetadata(null));
		/// <summary>错误消息</summary>
		public string ErrorText
		{
			get { return (string)base.GetValue(ErrorTextProperty); }
			set { base.SetValue(ErrorTextProperty, value); }
		}
		#endregion

		private void InitializeContent()
		{
			var stackPanel = new StackPanel { Margin = new Thickness(30) };

			var textBox = new TextBox { Padding = new Thickness(8), MinWidth = 300 };
			textBox.SetBinding(TextBox.TextProperty, new Binding("FileName") { Source = this });

			var errorBlock = new TextBlock
			{
				Foreground = Brushes.Red,
				Margin = new Thickness(0, 5, 0, 0),
				TextWrapping = TextWrapping.WrapWithOverflow
			};
			errorBlock.SetBinding(TextBlock.TextProperty, new Binding("ErrorText") { Source = this });

			var button = new Button
			{
				Content = "确定",
				Margin = new Thickness(0, 20, 0, 0),
				Padding = new Thickness(40, 10, 40, 10),
				HorizontalAlignment = HorizontalAlignment.Right,
				Command = ApplicationCommands.Save
			};

			stackPanel.Children.Add(textBox);
			stackPanel.Children.Add(errorBlock);
			stackPanel.Children.Add(button);

			this.Content = stackPanel;
		}
		#region 重载 OnApplyTemplate 方法，绑定 PART_TREEVIEW 事件
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			// 查找模板中的控件
			//if (GetTemplateChild("PART_NAME") is TextBox txtName)
			//{
			//	txtName.PreviewKeyDown += new KeyEventHandler((sender, args) => { ErrorText = ""; });
			//	txtName.MouseLeftButtonDown += new MouseButtonEventHandler((sender, args) =>
			//	{
			//		if (string.IsNullOrWhiteSpace(txtName.Text) == false)
			//		{
			//			string fileName = txtName.Text.Trim();
			//			if (fileName.EndsWith(PersistentService.LocalizedExtension))
			//			{
			//				txtName.Select(0, fileName.Length - 10);
			//			}
			//			else { txtName.Select(0, fileName.Length); }
			//		}
			//	});
			//}
		}

		private void TxtName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			throw new System.NotImplementedException();
		}
		#endregion

		private void OnSaveExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(FileName) == false)
			{
				string fileName = FileName.Trim();
				if (fileName.EndsWith(PersistentService.LocalizedExtension) == false)
				{
					fileName = string.Concat(fileName, PersistentService.LocalizedExtension);
				}

				string filePath = string.Concat(_folderPath, "\\", fileName);
				if (System.IO.File.Exists(filePath))
				{
					ErrorText = string.Concat("文件\"", filePath, "\"已经存在，请修改文件名");
				}
				else { this.DialogResult = true; base.Close(); }
			}
		}
	}
}
