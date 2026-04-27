using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Basic.Configuration;
using Basic.Localizations;

namespace Basic.Windows
{
	/// <summary>
	/// AddResourceDialog.xaml 的交互逻辑
	/// </summary>
	public partial class AddResourceDialog : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		static AddResourceDialog()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AddResourceDialog), new FrameworkPropertyMetadata(typeof(AddResourceDialog)));
		}

		//private readonly EnvDTE80.DTE2 _DTEClass;
		private readonly LocalizationCollection _ResourceCollection;
		//private readonly ObservableCollection<MessageInfo> _MessageInfos;
		//private readonly PersistentService _CommandService;
		//private readonly PersistentDesigner _Persistent;
		//private readonly EnvDTE.ProjectItem _ProjectItem;
		public AddResourceDialog(PersistentDesigner persistent, LocalizationCollection resources)
			: base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
		{
			_ResourceCollection = resources; CanSave = resources.Count > 0;
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSaveExecuted));
			ResourceFile = string.Concat("本地化资源文件：", persistent.MessageConverter.FileName);
			//_DTEClass = dteClass; _Persistent = persistent;
			//_CommandService = commandService; 

			//System.IO.FileInfo fileSolution = new System.IO.FileInfo(dteClass.Solution.FullName);
			//string localFilePath = string.Concat(fileSolution.DirectoryName, persistent.MessageConverter.FileName);
			//_ProjectItem = dteClass.Solution.FindProjectItem(localFilePath);
			//dgEnums.ItemsSource = _ResourceCollection;
			//btnOk.IsEnabled = _ResourceCollection.Count > 0;
		}

		#region 属性 AllChecked 定义
		public static readonly DependencyProperty AllCheckedProperty = DependencyProperty.Register("AllChecked",
			typeof(bool), typeof(AddResourceDialog), new PropertyMetadata(false, AllCheckedChanged));
		/// <summary>是否选择</summary>
		public bool AllChecked
		{
			get { return (bool)base.GetValue(AllCheckedProperty); }
			set { base.SetValue(AllCheckedProperty, value); }
		}
		private static void AllCheckedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			AddResourceDialog dlg = sender as AddResourceDialog;
			if (dlg != null) { dlg.CheckedChanged((bool)e.NewValue); }
		}
		private void CheckedChanged(bool isChecked)
		{
			foreach (LocalizationItem resx in _ResourceCollection)
			{
				resx.Created = isChecked;
			}

		}
		#endregion

		#region 属性 CanSave 定义
		public static readonly DependencyProperty ResourceFileProperty = DependencyProperty.Register("ResourceFile",
			typeof(string), typeof(AddResourceDialog), new PropertyMetadata(null));
		/// <summary>当前项目资源文件</summary>
		public string ResourceFile
		{
			get { return (string)base.GetValue(ResourceFileProperty); }
			set { base.SetValue(ResourceFileProperty, value); }
		}
		#endregion

		#region 属性 CanSave 定义
		public static readonly DependencyProperty CanSaveProperty = DependencyProperty.Register("CanSave",
			typeof(bool), typeof(AddResourceDialog), new PropertyMetadata(false));
		/// <summary>判断当前列表是否允许保存</summary>
		public bool CanSave
		{
			get { return (bool)base.GetValue(CanSaveProperty); }
			set { base.SetValue(CanSaveProperty, value); }
		}

		#endregion

		#region 重载 OnApplyTemplate 方法，绑定 PART_TREEVIEW 事件
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (GetTemplateChild("PART_ITEMS") is DataGrid dgEnums)
			{
				dgEnums.ItemsSource = _ResourceCollection;
			}
		}
		#endregion

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			//_CommandService.SetWaitCursor();
			//if (_ProjectItem == null)
			//{
			//	string message = string.Concat("本地化资源文件\"", _Persistent.MessageConverter.ConverterName, "\"不存在，请重新选择。");
			//	_CommandService.ShowMessage(message); return;
			//}

			//if (!_ProjectItem.IsOpen) { _ProjectItem.Open(); }
			//_ProjectItem.Document.Activate();
			//LocalizationPane pane = _ProjectItem.Document.ActiveWindow.Object as LocalizationPane;
			//if (pane == null)
			//{
			//	string message = string.Concat("本地化资源文件\"", _Persistent.MessageConverter.ConverterName,
			//		"\"打开失败，可能原因是本地化资源编辑器已经更改。");
			//	_CommandService.ShowMessage(message); return;
			//}
			//ResourceEditor editor = pane.Content as ResourceEditor;
			//foreach (LocalizationItem resx in _ResourceCollection)
			//{
			//	if (resx.Created) { editor.Add(resx); }
			//}
			//_ProjectItem.Save();
			this.DialogResult = _ResourceCollection.Any(m => m.Created);
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
