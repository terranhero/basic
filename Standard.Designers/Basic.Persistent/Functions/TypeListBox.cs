using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms.Design;

namespace Basic.Functions
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class TypeListBox : System.Windows.Forms.ListBox
	{
		private IWindowsFormsEditorService _editorService;
		private readonly Type[] primitiveType;
		public TypeListBox(Type[] typeArray)
		{
			primitiveType = typeArray;
			base.Items.AddRange(typeArray);
			this.IntegralHeight = true;
		}

		internal void BeginEdit(IWindowsFormsEditorService editorService, object value)
		{
			if (Items.Count >= 10)
				this.Height = this.ItemHeight * 10 + 10;
			_editorService = editorService;
			if (Items.Contains(value))
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
