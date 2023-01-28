using System;
using System.Linq;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Basic.DataEntities;
using Microsoft.VisualStudio.Shell.Design;
using System.ComponentModel.Design;
using System.Collections;
using Basic.EntityLayer;
using System.Collections.Generic;
using Microsoft;

namespace Basic.Designer
{
	/// <summary>
	/// 条件类基类选择器
	/// </summary>
	public sealed class BaseConditionSelector : UITypeEditor
	{
		private BaseClassListBox listBox;
		/// <summary>
		/// 初始化 BaseClassSelector 类实例。
		/// </summary>
		public BaseConditionSelector() : base() { }

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
					this.listBox = new BaseClassListBox();
				}
				this.listBox.BeginEdit(editorService, provider, value);
				editorService.DropDownControl(this.listBox);
				return listBox.SelectedItem;
			}
			return base.EditValue(context, provider, value);
		}

		private class BaseClassListBox : ListBox
		{
			private IWindowsFormsEditorService _editorService;
			internal BaseClassListBox()
			{
				base.Dock = DockStyle.Fill;
				base.DisplayMember = "FullName";
				base.IntegralHeight = true;
			}

			internal void BeginEdit(IWindowsFormsEditorService editorService, System.IServiceProvider provider, object value)
			{
				_editorService = editorService;
				base.Items.Clear();
				base.Items.Add(typeof(AbstractCondition).FullName);
				EnvDTE.DTE dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
				Assumes.Present(dteClass);
				EnvDTE.ProjectItem projectItem = dteClass.Solution.FindProjectItem(dteClass.ActiveDocument.FullName);
				if (projectItem != null)
				{
					EnvDTE.Project project = projectItem.ContainingProject;
					IVsSolution2 vsSolution = (IVsSolution2)provider.GetService(typeof(SVsSolution));
					Assumes.Present(vsSolution);
					vsSolution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
					vsSolution.GetGuidOfProject(hierarchy, out Guid projectGuid);
					IVsHierarchy ivsh = VsShellUtilities.GetHierarchy(provider, projectGuid);
					DynamicTypeService typeService = (DynamicTypeService)provider.GetService(typeof(DynamicTypeService));
					Assumes.Present(typeService); ITypeDiscoveryService discovery = typeService.GetTypeDiscoveryService(ivsh);
					ICollection types = discovery.GetTypes(typeof(Basic.EntityLayer.AbstractEntity), true);
					foreach (Type type in types)
					{
						if (type.IsSubclassOf(typeof(AbstractCondition)))
						{
							if (type.IsPublic && !type.IsGenericType && type.IsAbstract)
								base.Items.Add(type.FullName);
						}
					}
				}
				if (value != null) { SelectedItem = value; }
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

	/// <summary>
	/// 条件类基类选择器
	/// </summary>
	public sealed class ConditionSelector : UITypeEditor
	{
		private ClassListBox listBox;
		/// <summary>
		/// 初始化 ConditionSelector 类实例。
		/// </summary>
		public ConditionSelector() : base() { }

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
					this.listBox = new ClassListBox();
				}
				this.listBox.BeginEdit(editorService, provider, value);
				editorService.DropDownControl(this.listBox);
				return listBox.SelectedItem;
			}
			return base.EditValue(context, provider, value);
		}

		private class ClassListBox : ListBox
		{
			private IWindowsFormsEditorService _editorService;
			internal ClassListBox()
			{
				base.Dock = DockStyle.Fill;
				base.DisplayMember = "Name";
				base.IntegralHeight = true;
			}

			internal void BeginEdit(IWindowsFormsEditorService editorService, System.IServiceProvider provider, object value)
			{
				_editorService = editorService;
				base.Items.Clear();
				base.Items.Add(typeof(AbstractCondition).Name);
				EnvDTE.DTE dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
				Assumes.Present(dteClass);
				EnvDTE.ProjectItem projectItem = dteClass.Solution.FindProjectItem(dteClass.ActiveDocument.FullName);
				if (projectItem != null)
				{
					SortedSet<string> list = new SortedSet<string>();
					EnvDTE.Project project = projectItem.ContainingProject;
					IVsSolution2 vsSolution = (IVsSolution2)provider.GetService(typeof(SVsSolution));
					Assumes.Present(vsSolution);
					vsSolution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
					vsSolution.GetGuidOfProject(hierarchy, out Guid projectGuid);
					IVsHierarchy ivsh = VsShellUtilities.GetHierarchy(provider, projectGuid);
					DynamicTypeService typeService = (DynamicTypeService)provider.GetService(typeof(DynamicTypeService));
					Assumes.Present(typeService);
					ITypeDiscoveryService discovery = typeService.GetTypeDiscoveryService(ivsh);
					ICollection types = discovery.GetTypes(typeof(Basic.EntityLayer.AbstractEntity), true);
					foreach (Type type in types)
					{
						if (type.IsSubclassOf(typeof(AbstractCondition)))
						{
							if (type.IsPublic && !type.IsGenericType)
								list.Add(type.Name);
						}
					}
					base.Items.AddRange(list.ToArray());
				}
				if (value != null) { SelectedItem = value; }
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
