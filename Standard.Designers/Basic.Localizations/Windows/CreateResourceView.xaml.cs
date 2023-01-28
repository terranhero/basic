
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;
using Basic.Localizations;
using Microsoft.VisualStudio.Shell;

namespace Basic.Windows
{
	/// <summary>
	/// CreateResourceView.xaml 的交互逻辑
	/// </summary>
	public partial class CreateResourceView : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly EnvDTE80.DTE2 _DTEClass;
		private readonly LocalizationCollection _ResourceCollection;
		private readonly ObservableCollection<MessageInfo> _MessageInfos;
		private readonly LocalizationService _CommandService;
		public CreateResourceView(LocalizationService commandService, EnvDTE80.DTE2 dteClass, LocalizationCollection resources)
			: base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
		{
			_DTEClass = dteClass;
			_CommandService = commandService;
			_ResourceCollection = resources;
			_MessageInfos = new ObservableCollection<MessageInfo>();
			InitializeComponent();
			dgEnums.ItemsSource = _ResourceCollection;
		}

		[SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
		private void DialogWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler((ss, se) =>
			{
				EnvDTE.Solution solutionClass = _DTEClass.Solution;
				System.IO.FileInfo fileSolution = new System.IO.FileInfo(solutionClass.FullName);
				System.IO.FileInfo[] fileArray = fileSolution.Directory.GetFiles("*.localresx", System.IO.SearchOption.AllDirectories);
				char separator = System.IO.Path.PathSeparator;
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
				cmbLocalizations.ItemsSource = _MessageInfos;
			});
			worker.RunWorkerAsync();
		}

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_CommandService.SetWaitCursor();
			if (cmbLocalizations.SelectedIndex >= 0)
			{
				MessageInfo info = (MessageInfo)cmbLocalizations.SelectedItem;
				string fullName = (string)cmbLocalizations.SelectedValue;
				EnvDTE.ProjectItem item = _DTEClass.Solution.FindProjectItem(fullName);
				if (item == null)
				{
					string message = string.Concat("本地化资源文件\"", info.ConverterName, "\"已经不存在，请重新选择。");
					_CommandService.ShowMessage(message); return;
				}

				if (!item.IsOpen) { item.Open(); }
				item.Document.Activate();
				LocalizationPane pane = item.Document.ActiveWindow.Object as LocalizationPane;
				if (pane == null)
				{
					string message = string.Concat("本地化资源文件\"", info.ConverterName, "\"打开失败，可能原因是本地化资源编辑器已经更改。");
					_CommandService.ShowMessage(message); return;
				}
				ResourceEditor editor = pane.Content as ResourceEditor;
				foreach (LocalizationItem resx in _ResourceCollection)
				{
					if (resx.Created) { editor.Add(resx); }
				}
				item.Save();
				this.DialogResult = true;
				base.Close();
			}
		}

		private void cmbLocalizations_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (cmbLocalizations.SelectedIndex >= 0)
			{
				btnOk.IsEnabled = true;
			}
		}
	}
}
