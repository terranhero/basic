using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using Basic.Designer;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Basic.Designer
{
	/// <summary>
	/// 表示验证集合编辑器
	/// </summary>
	public class WithClauseEditor : System.Drawing.Design.UITypeEditor
	{
		/// <summary>
		/// 使用指定的集合类型初始化 WithClauseEditor 类的新实例
		/// </summary>
		public WithClauseEditor() : base() { }

		/// <summary>
		/// 获取由 EditValue 方法使用的编辑器样式。
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			//指定为模式窗体属性编辑器类型
			return UITypeEditorEditStyle.Modal;
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
				if (value is WithClause && context.Instance is DynamicCommandDescriptor) //用户选中了WithClause类型单元格
				{
					WithClauseWindow window = new WithClauseWindow(value as WithClause);
					if (window.ShowModal() == true)
					{
						DynamicCommandDescriptor dcd = context.Instance as DynamicCommandDescriptor;
						dcd.DefinitionInfo.WithClauses.Remove(value as WithClause);
					}
				}
				else if (value is string && context.Instance is WithClause)
				{
					WithClauseWindow window = new WithClauseWindow(context.Instance as WithClause);
					window.ShowModal();
				}
			}
			return base.EditValue(context, provider, value);
		}

	}
}
