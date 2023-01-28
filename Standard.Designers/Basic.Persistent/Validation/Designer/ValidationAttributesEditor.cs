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
	/// ��ʾ��֤���ϱ༭��
	/// </summary>
	public class ValidationAttributesListEditor : System.Drawing.Design.UITypeEditor
	{
		private ValidationAttributesListBox listBox;
		/// <summary>
		/// ��ȡ�� EditValue ����ʹ�õı༭����ʽ��
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			//ָ��Ϊģʽ�������Ա༭������
			return UITypeEditorEditStyle.DropDown;
		}

		/// <summary>
		/// ʹ�� System.Drawing.Design.UITypeEditor.GetEditStyle() ������ָʾ�ı༭����ʽ�༭ָ�������ֵ��
		/// </summary>
		/// <param name="context">�����ڻ�ȡ������������Ϣ�� System.ComponentModel.ITypeDescriptorContext��</param>
		/// <param name="provider">System.IServiceProvider���˱༭������������ȡ����</param>
		/// <param name="value">Ҫ�༭�Ķ���</param>
		/// <returns>�µĶ���ֵ����������ֵ��δ���ģ��������صĶ���Ӧ�봫�ݸ����Ķ�����ͬ��</returns>
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
	/// ��ʾ��֤���ϱ༭��
	/// </summary>
	public class ValidationAttributesEditor : CollectionEditor
	{
		/// <summary>
		/// ʹ��ָ���ļ������ͳ�ʼ�� ValidationAttributesEditor �����ʵ��
		/// </summary>
		public ValidationAttributesEditor(Type type) : base(type) { }

		/// <summary>
		/// ��ȡ�˼��ϱ༭���ɰ������������͡�
		/// </summary>
		/// <returns>�˼��Ͽɰ����������������顣</returns>
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
		/// ָʾ�Ƿ��һ��ѡ���������
		/// </summary>
		/// <returns></returns>
		protected override bool CanSelectMultipleInstances()
		{
			return false;
		}
	}
}
