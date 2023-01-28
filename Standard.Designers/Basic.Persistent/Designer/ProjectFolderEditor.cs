using System;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Basic.Configuration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft;

namespace Basic.Designer
{
	/// <summary>
	/// 属性类型编辑器
	/// </summary>
	public sealed class ProjectFolderEditor : UITypeEditor
	{
		private FolderListBox listBox;
		/// <summary>
		/// 获取由 EditValue 方法使用的编辑器样式。
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			//指定为模式窗体属性编辑器类型
			return UITypeEditorEditStyle.DropDown;
		}

		/// <summary>
		/// 使用 System.Drawing.Design.UITypeEditor.GetEditStyle() 方法所指示的编辑器样式编辑指定对象的值。
		/// </summary>
		/// <param name="context">可用于获取附加上下文信息的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="provider">System.IServiceProvider，此编辑器可用其来获取服务。</param>
		/// <param name="value">要编辑的对象。</param>
		/// <returns>新的对象值。如果对象的值尚未更改，则它返回的对象应与传递给它的对象相同。</returns>
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (editorService == null)
				{
					return value;
				}
				if (this.listBox == null)
				{
					//EnvDTE.DTE dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
					this.listBox = new FolderListBox(provider);
				}
				PersistentDescriptor objectDescriptor = context.Instance as PersistentDescriptor;
				PersistentConfiguration persistet = objectDescriptor.DefinitionInfo;
				if (!persistet.Project.IsEmpty) { this.listBox.BeginEdit(editorService, provider, persistet.ProjectGuid, (string)value); }
				else
				{
					EnvDTE.DTE dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
					Assumes.Present(dteClass);
					EnvDTE.ProjectItem item = dteClass.ActiveDocument.ProjectItem;
					this.listBox.BeginEdit(editorService, provider, item.ContainingProject, (string)value);
				}
				editorService.DropDownControl(this.listBox);
				return listBox.SelectedItem;
			}
			return value;
			//return base.EditValue(context, provider, value);
		}

		private class FolderListBox : ListBox
		{
			private IWindowsFormsEditorService _editorService;
			public FolderListBox(System.IServiceProvider p)
			{
				this.Dock = DockStyle.Fill;
				this.IntegralHeight = true;
			}
			internal bool BeginEdit(IWindowsFormsEditorService editorService, IServiceProvider provider, Guid projectGuid, string value)
			{
				Items.Clear();
				_editorService = editorService;
				Items.Add(string.Empty);
				IVsSolution vsSolution = (IVsSolution)provider.GetService(typeof(SVsSolution));
				Assumes.Present(vsSolution); Guid guid = projectGuid;
				vsSolution.GetProjectOfGuid(ref guid, out IVsHierarchy hierarchy);
				uint itemId = (uint)VSConstants.VSITEMID.Root;
				object outProject;
				if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out outProject) >= 0)
				{
					EnvDTE.Project project = (EnvDTE.Project)outProject;
					FileInfo fileInfo = new FileInfo(project.FullName);
					string directoryName = fileInfo.DirectoryName + "\\";
					SearchProjectSubDirectory(directoryName, project.ProjectItems);
					if (this.Items.Count >= 10)
						this.Height = this.ItemHeight * 10;
					if (value != null) { SelectedItem = value; }
				}
				return true;
			}

			internal bool BeginEdit(IWindowsFormsEditorService editorService, IServiceProvider provider, EnvDTE.Project project, string value)
			{
				Items.Clear();
				_editorService = editorService;
				Items.Add(string.Empty);
				FileInfo fileInfo = new FileInfo(project.FullName);
				string directoryName = fileInfo.DirectoryName + "\\";
				SearchProjectSubDirectory(directoryName, project.ProjectItems);
				if (this.Items.Count >= 10) { this.Height = this.ItemHeight * 10; }
				if (value != null) { SelectedItem = value; }
				return true;
			}

			private void SearchProjectSubDirectory(string directoryName, EnvDTE.ProjectItems itemArray)
			{
				foreach (EnvDTE.ProjectItem item in itemArray)
				{
					if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
					{
						if (item.Name == "Properties") { continue; }
						EnvDTE.Property property = item.Properties.Item("FullPath");
						if (property != null && property.Value is string)
						{
							Items.Add(property.Value.ToString().Replace(directoryName, string.Empty));
							EnvDTE.ProjectItems subItems = item.ProjectItems;
							if (subItems != null && subItems.Count > 0)
								SearchProjectSubDirectory(directoryName, subItems);
						}

					}
				}
			}

			private void SearchSubDirectory(string directoryName, DirectoryInfo[] directoryInfos)
			{
				foreach (DirectoryInfo directoryInfo in directoryInfos)
				{
					Items.Add(directoryInfo.FullName.Replace(directoryName, string.Empty));
					DirectoryInfo[] subFolders = directoryInfo.GetDirectories();
					if (subFolders != null && subFolders.Length > 0)
						SearchSubDirectory(directoryName, subFolders);
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnSelectedIndexChanged(EventArgs e)
			{
				base.OnSelectedIndexChanged(e);
				_editorService.CloseDropDown();
			}
		}
	}
}
