using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Basic.Collections;
using Basic.Localizations;

namespace Basic.Windows
{
	/// <summary>
	/// ExtractEnumDialog.xaml 的交互逻辑
	/// </summary>
	public partial class ExtractEnumDialog : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		static ExtractEnumDialog()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtractEnumDialog),
				new FrameworkPropertyMetadata(typeof(ExtractEnumDialog)));
		}

		private readonly EnvDTE80.DTE2 _DTEClass;
		private readonly LocalizationCollection _ResourceCollection;
		private readonly Pagination<MessageInfo> _MessageInfos = new Pagination<MessageInfo>();
		private readonly LocalizationService _CommandService;
		public ExtractEnumDialog(LocalizationService commandService, EnvDTE80.DTE2 dteClass, LocalizationCollection resources)
		{
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSaveExecuted));
			_DTEClass = dteClass; _ResourceCollection = resources;
			_CommandService = commandService;
			this.Loaded += new RoutedEventHandler(DialogWindow_Loaded);
		}
		/// <summary></summary>
		protected override void InvokeDialogHelp() { }

		private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler((ss, se) =>
			{
				EnvDTE.Solution solutionClass = _DTEClass.Solution;
				System.IO.FileInfo fileSolution = new System.IO.FileInfo(solutionClass.FullName);
				System.IO.FileInfo[] fileArray = fileSolution.Directory.GetFiles("*.localresx", System.IO.SearchOption.AllDirectories);
				char separator = System.IO.Path.PathSeparator;
				_MessageInfos.BeginCollectionChanged();
				foreach (System.IO.FileInfo fileLocalResx in fileArray)
				{
					if (solutionClass.FindProjectItem(fileLocalResx.FullName) != null)
					{
						string fileName = Path.GetFileNameWithoutExtension(fileLocalResx.FullName);
						_MessageInfos.Add(new MessageInfo(fileName, fileLocalResx.FullName));
					}
				}
			});
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((ss, se) =>
			{
				_MessageInfos.EndCollectionChanged();
				//cmbLocalizations.ItemsSource = _MessageInfos;
			});
			worker.RunWorkerAsync();
		}

		#region 属性 CanSave 定义
		public static readonly DependencyProperty CanSaveProperty = DependencyProperty.Register("CanSave",
			typeof(bool), typeof(ExtractEnumDialog), new PropertyMetadata(false));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool CanSave
		{
			get { return (bool)base.GetValue(CanSaveProperty); }
			set { base.SetValue(CanSaveProperty, value); }
		}
		#endregion

		#region 属性 ConnectionInfo 定义
		internal static readonly DependencyProperty MessageConverterProperty = DependencyProperty.Register("MessageConverter",
			typeof(MessageInfo), typeof(ExtractEnumDialog), new PropertyMetadata(null));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		internal MessageInfo MessageConverter
		{
			get { return (MessageInfo)base.GetValue(MessageConverterProperty); }
			set { base.SetValue(MessageConverterProperty, value); }
		}
		#endregion

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (MessageConverter != null)
			{
				MessageInfo info = MessageConverter;
				string fullName = info.FileName;
				EnvDTE.ProjectItem item = _DTEClass.Solution.FindProjectItem(fullName);
				if (item == null)
				{
					string message = string.Concat("本地化资源文件\"", info.ConverterName, "\"已经不存在，请重新选择。");
					_CommandService.ShowMessage(message); return;
				}
				if (item.IsOpen == true) { item.Document.Activate(); }
				else { item.Open(); }
				if (!(item.Document.ActiveWindow.Object is LocalizationPane pane))
				{
					string message = string.Concat("本地化资源文件\"", info.ConverterName, "\"打开失败，可能原因是本地化资源编辑器已经更改。");
					_CommandService.ShowMessage(message); return;
				}
				ResourceEditor editor = pane.Content as ResourceEditor;
				foreach (LocalizationItem resx in _ResourceCollection)
				{
					if (resx.Created) { editor.Add(resx); }
				}			
				this.DialogResult = true;
				base.Close();
			}
		}

		#region 重载 OnApplyTemplate 方法，绑定 PART_TREEVIEW 事件
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			// 查找模板中的控件
			if (GetTemplateChild("PART_LOCALIZATIONS") is ComboBox cmbLocalizations)
			{
				cmbLocalizations.ItemsSource = _MessageInfos;
				cmbLocalizations.SelectionChanged += new SelectionChangedEventHandler((sender, args) =>
				{
					CanSave = (cmbLocalizations.SelectedIndex >= 0);
					MessageConverter = (MessageInfo)cmbLocalizations.SelectedItem;
				});
			}
			if (GetTemplateChild("PART_ENUMS") is DataGrid dgEnums)
			{
				dgEnums.ItemsSource = _ResourceCollection;
			}
		}
		#endregion
	}
}
