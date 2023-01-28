using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Threading;
using Basic.Builders;
using Basic.Configuration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;

namespace Basic.Windows
{
	/// <summary>
	/// MvcViewWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MvcViewWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		public delegate void GeneratCode(string mode, string tempFile);
		private readonly MvcViewBuilder _Builder;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		private readonly PersistentService _CommandService;
		private System.Collections.Generic.IEnumerable<Type> types;
		public MvcViewWindow(PersistentService commandService, EnvDTE.ProjectItem item)
			: base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
		{
			_ProjectItem = item; _CommandService = commandService;
			_Builder = new MvcViewBuilder(commandService, item);
			InitializeComponent();
			DataContext = _Builder;
		}

		private void DialogWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_Builder.InitlizeTemplates();
			_Builder.InitlizeFiles();
			Dispatcher.BeginInvoke(new EventHandler((se, ss) =>
			{
				EnvDTE.Project project = _ProjectItem.ContainingProject;
				_CommandService.SetWaitCursor();
				IVsSolution2 vsSolution = _CommandService.GetService<SVsSolution, IVsSolution2>();
				vsSolution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
				vsSolution.GetGuidOfProject(hierarchy, out Guid projectGuid);
				IVsHierarchy ivsh = VsShellUtilities.GetHierarchy(_CommandService.AsyncPackage, projectGuid);
				DynamicTypeService typeService = _CommandService.GetService<DynamicTypeService>();
				ITypeDiscoveryService discovery1 = typeService.GetTypeDiscoveryService(ivsh);
				types = discovery1.GetTypes(typeof(Basic.EntityLayer.AbstractEntity), true).Cast<Type>();
				//VSProject vsProject = project.Object as VSProject;
				//foreach (Reference reference in vsProject.References)
				//{
				//	if (reference.Type == prjReferenceType.prjReferenceTypeAssembly && reference.Name == "Basic.EntityLayer")
				//	{
				//		Assembly ass = Assembly.ReflectionOnlyLoadFrom(reference.Path);
				//		Type entityType = ass.GetType("Basic.EntityLayer.AbstractEntity", false, true);

				//		break;
				//	}
				//}
			}), DispatcherPriority.Background, sender, e);
		}

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_CommandService.SetWaitCursor();
			EnvDTE.ProjectItem itemFolder = _ProjectItem;
			EnvDTE.Property propertyFullPath = itemFolder.Properties.Item("FullPath");
			string fullPath = (string)propertyFullPath.Value;
			fullPath = string.Concat(fullPath, txtViewName.Text);
			if (File.Exists(fullPath))
			{
				if (_CommandService.Confirm(string.Format("文件\"{0}\"已经存在，是否需要覆盖？", fullPath)) == false)
				{
					base.Close(); return;
				}
			}
			using (FileStream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.Write(txtCode.Text);
				}
			}
			EnvDTE.ProjectItem itemFile = null;
			foreach (EnvDTE.ProjectItem item in itemFolder.ProjectItems)
			{
				if (item.Name == txtViewName.Text) { itemFile = item; }
			}
			if (itemFile == null) { itemFolder.ProjectItems.AddFromFile(fullPath); }
			base.Close();
		}

		private void cmbModels_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			Dispatcher.BeginInvoke(new GeneratCode((model, template) =>
			{
				if (string.IsNullOrWhiteSpace(model) == true) { return; }
				if (string.IsNullOrWhiteSpace(template) == true) { return; }
				if (types == null) { return; }
				_CommandService.SetWaitCursor();

				Type type = types.FirstOrDefault(m => m.FullName == model);
				if (type == null) { txtCode.Text = string.Concat("类型: ", model, "当前解决方案不存在"); return; }
				txtCode.Text = _Builder.GetRazorContent(template, type);
			}), DispatcherPriority.Background, _Builder.ModelName, _Builder.SelectedTemplate);
		}

		private void cmbTemplates_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			FileInfo tempFile = new FileInfo(_Builder.SelectedTemplate);
			txtViewName.Text = tempFile.Name.Replace(tempFile.Extension, "");
			Dispatcher.BeginInvoke(new GeneratCode((model, template) =>
			{
				if (string.IsNullOrWhiteSpace(model) == true) { return; }
				if (string.IsNullOrWhiteSpace(template) == true) { return; }
				if (types == null) { return; }

				_CommandService.SetWaitCursor();
				Type type = types.FirstOrDefault(m => m.FullName == model);
				if (type == null) { txtCode.Text = string.Concat("类型: ", model, "当前解决方案不存在"); return; }
				txtCode.Text = _Builder.GetRazorContent(template, type);
			}), DispatcherPriority.Background, _Builder.ModelName, _Builder.SelectedTemplate);
		}
	}
}
