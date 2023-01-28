using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Basic.DataEntities
{
	/// <summary>
	/// 属性类型编辑器
	/// </summary>
	public sealed class OtherPropertyEditor : UITypeEditor
	{
		private OtherPropertyListBox listBox;
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
				AbstractAttribute validation = context.Instance as AbstractAttribute;
				ICompareProperty compareProperty = validation as ICompareProperty;
				if (validation == null) { return value; }
				IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (editorService == null) { return value; }
				if (listBox == null)
				{
					listBox = new OtherPropertyListBox();
					listBox.SelectedIndexChanged += (sender, e) =>
					{
						editorService.CloseDropDown();
						if (compareProperty != null)
							compareProperty.SetOtherProperty(listBox.SelectedItem as DataEntityPropertyElement);
					};
				}
				listBox.BeginEdit(editorService, validation, value);
				editorService.DropDownControl(listBox);
				return listBox.EndEdit(value);
			}
			return base.EditValue(context, provider, value);
		}

		private class OtherPropertyListBox : ListBox
		{
			private IWindowsFormsEditorService _editorService;
			public OtherPropertyListBox()
			{
				this.IntegralHeight = true;
				this.DisplayMember = "Name";
			}

			internal void BeginEdit(IWindowsFormsEditorService editorService, AbstractAttribute validation, object value)
			{
				_editorService = editorService;
				this.Items.Clear();
				DataEntityElement entity = validation.Property.Owner as DataEntityElement;
				ICompareProperty compareProperty = validation as ICompareProperty;
				//DataEntityElement entity = property.Owner as DataEntityElement;
				if (entity == null) { return; }
				this.Items.Add(new DataEntityPropertyElement(entity));
				foreach (DataEntityPropertyElement property in entity.Properties)
				{
					if (property != validation.Property)
					{
						this.Items.Add(property);
						if (property.Name == (value as string)) { this.SelectedItem = property; }
					}
				}
			}

			internal object EndEdit(object value)
			{
				if (this.SelectedItem != null)
					return (this.SelectedItem as DataEntityPropertyElement).Name;
				return null;
			}
		}
	}
}
