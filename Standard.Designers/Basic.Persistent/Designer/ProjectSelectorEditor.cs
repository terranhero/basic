using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Basic.Configuration;
using Microsoft;
using System.Collections.Generic;
using System.Linq;

namespace Basic.Designer
{
	/// <summary>
	/// 属性类型编辑器
	/// </summary>
	public sealed class ProjectSelectorEditor : UITypeEditor
	{
		private ProjectListBox listBox;
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
				if (editorService == null) { return value; }
				ProjectInfo projectInfo = value as ProjectInfo;
				if (projectInfo == null) { return value; }
				if (this.listBox == null)
				{
					EnvDTE.DTE dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
					this.listBox = new ProjectListBox(dteClass);
				}
				PersistentDescriptor objectDescriptor = context.Instance as PersistentDescriptor;
				PersistentDesigner persistet = objectDescriptor.DefinitionInfo;
				this.listBox.BeginEdit(editorService, provider, persistet, projectInfo.ProjectGuid);
				editorService.DropDownControl(this.listBox);
				ProjectInfo info = (ProjectInfo)listBox.SelectedItem;
				if (info == null) { return value; }
				projectInfo.ProjectGuid = info.ProjectGuid;
				projectInfo.UniqueName = info.UniqueName;
				projectInfo.ProjectName = info.ProjectName;
				return projectInfo;
			}
			return base.EditValue(context, provider, value);
		}

		private class ProjectListBox : ListBox
		{
			private IWindowsFormsEditorService _editorService;
			private PersistentDesigner persistentConfiguration;
			private readonly EnvDTE.DTE dteClass;
			public ProjectListBox(EnvDTE.DTE dte)
			{
				dteClass = dte;
			}
			internal void BeginEdit(IWindowsFormsEditorService editorService, IServiceProvider provider, PersistentDesigner persistent, Guid value)
			{
				persistentConfiguration = persistent;
				_editorService = editorService;
				this.Items.Clear();
				List<ProjectInfo> projects = new List<ProjectInfo>
				{
					new ProjectInfo(persistent, Guid.Empty, null, null)
				};
				IVsSolution vsSolution = (IVsSolution)provider.GetService(typeof(IVsSolution));
				Assumes.Present(vsSolution);
				EnvDTE.ProjectItem projectItem = dteClass.Solution.FindProjectItem(dteClass.ActiveDocument.FullName);
				EnvDTE.Project itemProject = null;
				if (projectItem != null) { itemProject = projectItem.ContainingProject; }
				this.DisplayMember = "ProjectName";
				this.ValueMember = "ProjectGuid";

				if (itemProject != null)
				{
							// 首先添加当前项目
					int hr = vsSolution.GetProjectOfUniqueName(itemProject.UniqueName, out IVsHierarchy currentHierarchy);
					if (hr == 0 && currentHierarchy != null)
					{
						hr = vsSolution.GetGuidOfProject(currentHierarchy, out Guid currentProjectGuid);
						if (hr == 0 && currentProjectGuid != Guid.Empty)
						{
							projects.Add(new ProjectInfo(persistent, currentProjectGuid, itemProject.Name, itemProject.UniqueName));
							//if (currentProjectGuid == value) { this.SelectedIndex = projects.Count - 1; }
						}
					}

					// 获取当前项目的 VSProject 对象以获取引用列表
					if (itemProject.Object is VSLangProj.VSProject vsProject)
					{
						foreach (VSLangProj.Reference reference in vsProject.References)
						{
							// 只处理项目引用（SourceProject 不为 null 表示是项目到项目的引用）
							if (reference.SourceProject != null)
							{
								EnvDTE.Project referencedProject = reference.SourceProject;
								int hrRef = vsSolution.GetProjectOfUniqueName(referencedProject.UniqueName, out IVsHierarchy refHierarchy);
								if (hrRef == 0 && refHierarchy != null)
								{
									hrRef = vsSolution.GetGuidOfProject(refHierarchy, out Guid refProjectGuid);
									if (hrRef == 0 && refProjectGuid != Guid.Empty)
									{
										projects.Add(new ProjectInfo(persistent, refProjectGuid, referencedProject.Name, referencedProject.UniqueName));
									}
								}
							}
						}
					}
				}
				this.Items.AddRange(projects.OrderBy(m => m.ProjectName).ToArray());
				if (value != null) { SelectedItem = projects.Find(m => m.ProjectGuid == value); }
				this.Height = this.ItemHeight * 10;
				if (this.PreferredHeight <= this.Height)
					this.Height = this.PreferredHeight;
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
