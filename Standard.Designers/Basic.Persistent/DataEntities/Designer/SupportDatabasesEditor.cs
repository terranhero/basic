using Basic.Enums;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms.Design;

namespace Basic.Designer
{
	/// <summary>
	/// 提供支持数据的可视化编辑
	/// </summary>
	public sealed class SupportDatabasesEditor : System.Drawing.Design.UITypeEditor
	{
		private DatabaseListBox listBox;
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
					this.listBox = new DatabaseListBox();
				}
				ConnectionTypeEnum[] types = (ConnectionTypeEnum[])value;
				this.listBox.BeginEdit(editorService, provider, types);
				editorService.DropDownControl(this.listBox);
				ConnectionTypeEnum[] result = new ConnectionTypeEnum[listBox.CheckedItems.Count];
				listBox.CheckedItems.CopyTo(result, 0);
				return result;
			}
			return value;
			//return base.EditValue(context, provider, value);
		}

		private class DatabaseListBox : System.Windows.Forms.CheckedListBox
		{
			private IWindowsFormsEditorService _editorService;
			private readonly List<ConnectionTypeEnum> dbs = new List<ConnectionTypeEnum>();
			private readonly SortedList<ConnectionTypeEnum, int> indexList = new SortedList<ConnectionTypeEnum, int>();
			public DatabaseListBox()
			{
				this.IntegralHeight = true; this.CheckOnClick = true;
				foreach (ConnectionTypeEnum element in Enum.GetValues(typeof(ConnectionTypeEnum)))
				{
					if (element != ConnectionTypeEnum.Default)
					{
						base.Items.Add(element); dbs.Add(element);
					}
				}
			}

			internal void BeginEdit(IWindowsFormsEditorService editorService, System.IServiceProvider provider, ConnectionTypeEnum[] types)
			{
				_editorService = editorService; int index = -1;
				List<ConnectionTypeEnum> selecteds = new List<ConnectionTypeEnum>(types);
				foreach (ConnectionTypeEnum element in dbs)
				{
					if (selecteds.Contains(element)) { index++; base.SetItemChecked(index, true); }
					else { index++; base.SetItemChecked(index, false); }
				}
			}

			///// <summary>
			///// 
			///// </summary>
			///// <param name="e"></param>
			//protected override void OnClick(EventArgs e)
			//{
			//	base.OnClick(e);
			//	if (this.SelectedIndex >= 0)
			//	{
			//		bool itemChecked = this.GetItemChecked(this.SelectedIndex);
			//		this.SetItemChecked(this.SelectedIndex, !itemChecked);
			//	}
			//}

			//protected override void OnSelectedIndexChanged(EventArgs e)
			//{
			//	base.OnSelectedIndexChanged(e);
			//	//if (this.SelectedIndex >= 0)
			//	//{
			//	//    bool itemChecked = this.GetItemChecked(this.SelectedIndex);
			//	//    this.SetItemChecked(this.SelectedIndex, !itemChecked);
			//	//}
			//}
		}
	}
}
