using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Basic.Properties;
using System.Text.RegularExpressions;

namespace Basic.Designer
{
	/// <summary>
	/// 数据源执行的 Transact-SQL 语句、表名或存储过程编辑器
	/// </summary>
	public sealed class CommandTextEditor : UITypeEditor
	{
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

		private string editorTitle;
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

				CommandTextWindow window = new CommandTextWindow(value);
				if (string.IsNullOrWhiteSpace(editorTitle)) { editorTitle = StringUtils.GetString("PersistentDescription_CommandText_Editor"); }
				if (context.PropertyDescriptor != null && !string.IsNullOrWhiteSpace(editorTitle))
				{
					window.Title = string.Format(editorTitle, context.PropertyDescriptor.Name);
				}
				if (window.ShowModal() == true)
				{
					return window.CommandText;
				}
			}
			return base.EditValue(context, provider, value);
		}
	}
}
