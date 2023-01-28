using Basic.Collections;
using Basic.Designer;
using Basic.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示验证集合编辑器
	/// </summary>
	public class ValidationAttributesListEditor : System.Drawing.Design.UITypeEditor
	{
		private ValidationAttributesListBox listBox;
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
				if (this.listBox == null) { this.listBox = new ValidationAttributesListBox(); }
				AbstractValidationCollection attributes = (AbstractValidationCollection)value;
				this.listBox.BeginEdit(editorService, provider, attributes);
				editorService.DropDownControl(this.listBox);
				foreach (Type type in listBox.Items)
				{
					bool isSelected = listBox.CheckedItems.Contains(type);
					if (isSelected == true)
					{
						AbstractAttribute aa = attributes.FirstOrDefault(m => m.GetType() == type);
						if (aa == null)
						{
							attributes.Add((AbstractAttribute)TypeDescriptor.CreateInstance(null, type, null, new object[] { attributes.Property }));
						}
					}
					else if (isSelected == false)
					{
						AbstractAttribute aa = attributes.FirstOrDefault(m => m.GetType() == type);
						if (aa != null) { attributes.Remove(aa); }
					}
				}
				return attributes;
			}
			return value;
			//return base.EditValue(context, provider, value);
		}
		private class ValidationAttributesListBox : System.Windows.Forms.CheckedListBox
		{
			private IWindowsFormsEditorService _editorService;
			private AbstractValidationCollection _attributes;
			private readonly List<Type> supportAttributes = new List<Type>(new Type[] { typeof(DisplayFormat),
				typeof(ImportPorpertyAttribute),typeof(RequiredValidation),typeof(BoolRequiredValidation),
				typeof(CompareValidation), typeof(RangeValidation) ,typeof(RegularExpressionValidation),
				typeof(MaxLengthValidation), typeof(StringLengthValidation), typeof(CustomValidationAttribute)
			});
			public ValidationAttributesListBox()
			{
				this.IntegralHeight = true; this.CheckOnClick = true;
				this.Height = 170;
				base.DisplayMember = "Name";
				//base.DataSource = supportAttributes;
				foreach (Type type in supportAttributes) { base.Items.Add(type); }
			}

			internal void BeginEdit(IWindowsFormsEditorService editorService, System.IServiceProvider provider, AbstractValidationCollection attrs)
			{
				_editorService = editorService; _attributes = attrs; int index = -1;
				foreach (Type attr in supportAttributes)
				{
					if (attrs.Any(m => m.GetType() == attr)) { index++; base.SetItemChecked(index, true); }
					else { index++; base.SetItemChecked(index, false); }
				}
			}
		}

	}

	/// <summary>
	/// 表示验证集合编辑器
	/// </summary>
	public class ValidationAttributesEditor : CollectionEditor
	{
		/// <summary>
		/// 使用指定的集合类型初始化 ValidationAttributesEditor 类的新实例
		/// </summary>
		public ValidationAttributesEditor(Type type) : base(type) { }

		/// <summary>
		/// 获取此集合编辑器可包含的数据类型。
		/// </summary>
		/// <returns>此集合可包含的数据类型数组。</returns>
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(DisplayFormat),typeof(ImportPorpertyAttribute),typeof(RequiredValidation),typeof(BoolRequiredValidation),typeof(CompareValidation),
				typeof(RangeValidation) ,typeof(RegularExpressionValidation),typeof(MaxLengthValidation),typeof(StringLengthValidation) };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemType"></param>
		/// <returns></returns>
		protected override object CreateInstance(Type itemType)
		{
			//IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			EntityPropertyDescriptor entityPropertyDescriptor = base.Context.Instance as EntityPropertyDescriptor;
			return TypeDescriptor.CreateInstance(null, itemType, null, new object[] { entityPropertyDescriptor.DefinitionInfo });
		}

		/// <summary>
		/// 指示是否可一次选择多个集合项。
		/// </summary>
		/// <returns></returns>
		protected override bool CanSelectMultipleInstances()
		{
			return false;
		}
	}
}
