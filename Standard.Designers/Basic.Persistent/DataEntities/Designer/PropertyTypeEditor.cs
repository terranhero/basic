﻿using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Basic.Designer
{
    /// <summary>
    /// 属性类型编辑器
    /// </summary>
    public sealed class PropertyTypeEditor : UITypeEditor
    {
        private PropertyTypeListBox listBox;
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
                    this.listBox = new PropertyTypeListBox();
                }
                this.listBox.BeginEdit(editorService, value);
                editorService.DropDownControl(this.listBox);
                return this.listBox.SelectedItem;
            }
            return value;
            //return base.EditValue(context, provider, value);
        }

        private class PropertyTypeListBox : ListBox
        {
            private IWindowsFormsEditorService _editorService;
            private readonly Type[] primitiveType;
            public PropertyTypeListBox()
            {
                primitiveType = new Type[] { 
					typeof(System.Boolean),
					typeof(System.Byte), 
					typeof(System.Byte[]), 
					typeof(System.Char), 
					typeof(System.DateTime), 
					typeof(System.DateTimeOffset), 
					typeof(System.Decimal),
					typeof(System.Double),
					typeof(System.Guid), 
					typeof(System.Int16),
					typeof(System.Int16[]),
					typeof(System.Int32),	
					typeof(System.Int32[]),
					typeof(System.Int64), 
					typeof(System.Int64[]),
					typeof(System.Object),
					typeof(System.Single),
					typeof(System.String),
					typeof(System.String[]),
					typeof(System.TimeSpan)
				};
                base.Items.AddRange(primitiveType);
                this.IntegralHeight = true;
            }
            internal void BeginEdit(IWindowsFormsEditorService editorService, object value)
            {
                this.Height = this.ItemHeight * 15 + 10;
                _editorService = editorService;
                if (value != null && Items.Contains(value))
                {
                    this.SelectedItem = value;
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
