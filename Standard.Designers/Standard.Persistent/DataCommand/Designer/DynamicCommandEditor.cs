using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Basic.Properties;
using Basic.Configuration;
using System.ComponentModel;
using System.Windows.Forms;

namespace Basic.Designer
{
	/// <summary>
	/// 数据源执行的 Transact-SQL 语句、表名或存储过程编辑器
	/// </summary>
	public sealed class DynamicCommandEditor : UITypeEditor
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

				//if (formCommand == null)
				//{
				//	formCommand = new DynamicCommandForm();
				//	formCommand.Height = 550;
				//}
				//if (string.IsNullOrWhiteSpace(editorTitle)) { editorTitle = StringUtils.GetString("PersistentDescription_CommandText_Editor"); }
				DynamicCommandDescriptor dynamicDescriptor = context.Instance as DynamicCommandDescriptor;
				if (dynamicDescriptor == null) { return value; }
				DynamicCommandElement dynamicCommand = dynamicDescriptor.DefinitionInfo;
				DynamicCommandWindow window = new DynamicCommandWindow(dynamicCommand);
				if (window.ShowModal() == true)
				{
					return window.GetDynamicCommand(context.PropertyDescriptor, value);
				}
				//if (context.PropertyDescriptor != null && !string.IsNullOrWhiteSpace(editorTitle))
				//{
				//	formCommand.Text = string.Format(editorTitle, dynamicCommand.Name);
				//}
				//formCommand.SetDynamicCommand(dynamicCommand);
				//if (editorService.ShowDialog(formCommand) == System.Windows.Forms.DialogResult.OK)
				//	return formCommand.GetDynamicCommand(dynamicCommand, context.PropertyDescriptor, value);
			}
			return base.EditValue(context, provider, value);
		}

		/// <summary>
		/// 
		/// </summary>
		private sealed class DynamicCommandForm : System.Windows.Forms.Form
		{
			private readonly System.Windows.Forms.Label lblSelect;
			private readonly System.Windows.Forms.TextBox txtSelect;
			private readonly System.Windows.Forms.Label lblFrom;
			private readonly System.Windows.Forms.TextBox txtFrom;
			private readonly System.Windows.Forms.Label lblWhere;
			private readonly System.Windows.Forms.TextBox txtWhere;
			private readonly System.Windows.Forms.Label lblGroup;
			private readonly System.Windows.Forms.TextBox txtGroup;
			private readonly System.Windows.Forms.Label lblHaving;
			private readonly System.Windows.Forms.TextBox txtHaving;
			private readonly System.Windows.Forms.Label lblOrder;
			private readonly System.Windows.Forms.TextBox txtOrder;

			private readonly System.Windows.Forms.Button btnOk;
			private readonly System.Windows.Forms.Button btnCopy;
			private readonly System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
			/// <summary>
			/// 
			/// </summary>
			public DynamicCommandForm()
			{
				this.ShowInTaskbar = false;
				this.Width = 800;
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				this.Text = "修改 CommandText 属性的值";
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
				tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
				tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;

				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));

				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30f));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30f));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
				tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

				lblSelect = new System.Windows.Forms.Label();
				lblSelect.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				lblSelect.Margin = new System.Windows.Forms.Padding(0);
				lblSelect.Padding = new System.Windows.Forms.Padding(0);
				lblSelect.AutoSize = true;
				tableLayoutPanel.SetColumnSpan(lblSelect, 2);
				tableLayoutPanel.Controls.Add(lblSelect, 0, 0);
				lblSelect.Text = "SELECT";

				txtSelect = new System.Windows.Forms.TextBox();
				tableLayoutPanel.Controls.Add(txtSelect, 0, 1);
				txtSelect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				txtSelect.AcceptsReturn = true;
				txtSelect.Dock = System.Windows.Forms.DockStyle.Fill;
				txtSelect.ScrollBars = System.Windows.Forms.ScrollBars.Both;
				tableLayoutPanel.SetColumnSpan(txtSelect, 2);
				txtSelect.Multiline = true;

				lblFrom = new System.Windows.Forms.Label();
				lblFrom.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				lblFrom.Margin = new System.Windows.Forms.Padding(0);
				lblFrom.Padding = new System.Windows.Forms.Padding(0);
				lblFrom.AutoSize = true;
				tableLayoutPanel.SetColumnSpan(lblFrom, 2);
				tableLayoutPanel.Controls.Add(lblFrom, 0, 2);
				lblFrom.Text = "FROM";

				txtFrom = new System.Windows.Forms.TextBox();
				tableLayoutPanel.SetColumnSpan(txtFrom, 2);
				tableLayoutPanel.Controls.Add(txtFrom, 0, 3);
				txtFrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				txtFrom.AcceptsReturn = true;
				txtFrom.Dock = System.Windows.Forms.DockStyle.Fill;
				txtFrom.ScrollBars = System.Windows.Forms.ScrollBars.Both;
				txtFrom.Multiline = true;

				lblWhere = new System.Windows.Forms.Label();
				lblWhere.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				lblWhere.Margin = new System.Windows.Forms.Padding(0);
				lblWhere.Padding = new System.Windows.Forms.Padding(0);
				lblWhere.AutoSize = true;
				tableLayoutPanel.SetColumnSpan(lblWhere, 2);
				tableLayoutPanel.Controls.Add(lblWhere, 0, 4);
				lblWhere.Text = "WHERE";

				txtWhere = new System.Windows.Forms.TextBox();
				tableLayoutPanel.SetColumnSpan(txtWhere, 2);
				tableLayoutPanel.Controls.Add(txtWhere, 0, 5);
				txtWhere.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				txtWhere.AcceptsReturn = true;
				txtWhere.Height = 54;
				txtWhere.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				txtWhere.ScrollBars = System.Windows.Forms.ScrollBars.Both;
				txtWhere.Multiline = true;

				lblGroup = new System.Windows.Forms.Label();
				lblGroup.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				lblGroup.Margin = new System.Windows.Forms.Padding(0);
				lblGroup.Padding = new System.Windows.Forms.Padding(0);
				lblGroup.AutoSize = true;
				tableLayoutPanel.SetColumnSpan(lblGroup, 2);
				tableLayoutPanel.Controls.Add(lblGroup, 0, 6);
				lblGroup.Text = "GROUP BY";

				txtGroup = new System.Windows.Forms.TextBox();
				tableLayoutPanel.SetColumnSpan(txtGroup, 2);
				tableLayoutPanel.Controls.Add(txtGroup, 0, 7);
				txtGroup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				txtGroup.AcceptsReturn = true;
				txtGroup.Height = 54;
				txtGroup.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				txtGroup.ScrollBars = System.Windows.Forms.ScrollBars.Both;
				txtGroup.Multiline = true;

				lblHaving = new System.Windows.Forms.Label();
				lblHaving.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				lblHaving.Margin = new System.Windows.Forms.Padding(0);
				lblHaving.Padding = new System.Windows.Forms.Padding(0);
				lblHaving.AutoSize = true;
				tableLayoutPanel.SetColumnSpan(lblHaving, 2);
				tableLayoutPanel.Controls.Add(lblHaving, 0, 8);
				lblHaving.Text = "HAVING";

				txtHaving = new System.Windows.Forms.TextBox();
				tableLayoutPanel.SetColumnSpan(txtHaving, 2);
				tableLayoutPanel.Controls.Add(txtHaving, 0, 9);
				txtHaving.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				txtHaving.AcceptsReturn = true;
				txtHaving.Height = 54;
				txtHaving.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				txtHaving.ScrollBars = System.Windows.Forms.ScrollBars.Both;
				txtHaving.Multiline = true;

				lblOrder = new System.Windows.Forms.Label();
				lblOrder.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				lblOrder.Margin = new System.Windows.Forms.Padding(0);
				lblOrder.Padding = new System.Windows.Forms.Padding(0);
				lblOrder.AutoSize = true;
				tableLayoutPanel.SetColumnSpan(lblOrder, 2);
				tableLayoutPanel.Controls.Add(lblOrder, 0, 10);
				lblOrder.Text = "ORDER BY";

				txtOrder = new System.Windows.Forms.TextBox();
				tableLayoutPanel.SetColumnSpan(txtOrder, 2);
				tableLayoutPanel.Controls.Add(txtOrder, 0, 11);
				txtOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				txtOrder.AcceptsReturn = true;
				txtOrder.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

				btnCopy = new System.Windows.Forms.Button();
				btnCopy.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				btnCopy.Cursor = System.Windows.Forms.Cursors.Hand;
				btnCopy.Height = 25;
				btnCopy.Text = "复制到剪贴板";
				btnCopy.Click += new EventHandler(btnCopy_Click);
				tableLayoutPanel.Controls.Add(btnCopy, 0, 12);

				btnOk = new System.Windows.Forms.Button();
				btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
				btnOk.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
				btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
				btnOk.Height = 25;
				btnOk.Text = "确  定";
				tableLayoutPanel.Controls.Add(btnOk, 1, 12);
				this.Controls.Add(tableLayoutPanel);
			}

			private void btnCopy_Click(object sender, EventArgs e)
			{
				Clipboard.Clear();
				StringBuilder text = new StringBuilder();
				if (txtSelect.Visible)
					text.Append("SELECT ").AppendLine(txtSelect.Text);
				if (txtFrom.Visible)
					text.Append("FROM ").AppendLine(txtFrom.Text);
				if (txtWhere.Visible)
					text.Append("WHERE ").AppendLine(txtWhere.Text);
				if (txtGroup.Visible)
					text.Append("GROUP BY ").AppendLine(txtGroup.Text);
				if (txtHaving.Visible)
					text.Append("HAVING ").AppendLine(txtHaving.Text);
				if (txtOrder.Visible)
					text.Append("ORDER BY ").AppendLine(txtOrder.Text);
				Clipboard.SetText(text.ToString());
			}

			/// <summary>
			/// 设置界面文本框值。
			/// </summary>
			/// <param name="value"></param>
			public void SetDynamicCommand(DynamicCommandElement dynamicCommand)
			{
				if (string.IsNullOrWhiteSpace(dynamicCommand.SelectText))
				{
					txtSelect.Visible = lblSelect.Visible = false;
					txtSelect.Text = string.Empty;
				}
				else
				{
					if (dynamicCommand.SelectText.IndexOf(Environment.NewLine) >= 0) { txtSelect.Text = dynamicCommand.SelectText; }
					else { txtSelect.Text = dynamicCommand.SelectText.Replace("\n", Environment.NewLine); }
					txtSelect.Visible = lblSelect.Visible = true;
				}
				if (string.IsNullOrWhiteSpace(dynamicCommand.FromText))
				{
					txtFrom.Visible = lblFrom.Visible = false;
					txtFrom.Text = string.Empty;
				}
				else
				{
					if (dynamicCommand.FromText.IndexOf(Environment.NewLine) >= 0) { txtFrom.Text = dynamicCommand.FromText; }
					else { txtFrom.Text = dynamicCommand.FromText.Replace("\n", Environment.NewLine); }
					txtFrom.Visible = lblFrom.Visible = true;
				}
				if (string.IsNullOrWhiteSpace(dynamicCommand.WhereText))
				{
					txtWhere.Visible = lblWhere.Visible = false;
					txtWhere.Text = string.Empty;
				}
				else
				{
					if (dynamicCommand.WhereText.IndexOf(Environment.NewLine) >= 0) { txtWhere.Text = dynamicCommand.WhereText; }
					else { txtWhere.Text = dynamicCommand.WhereText.Replace("\n", Environment.NewLine); }
					txtWhere.Visible = lblWhere.Visible = true;
				}
				if (string.IsNullOrWhiteSpace(dynamicCommand.GroupText))
				{
					txtGroup.Visible = lblGroup.Visible = false;
					txtGroup.Text = string.Empty;
				}
				else
				{
					if (dynamicCommand.GroupText.IndexOf(Environment.NewLine) >= 0) { txtGroup.Text = dynamicCommand.GroupText; }
					else { txtGroup.Text = dynamicCommand.GroupText.Replace("\n", Environment.NewLine); }
					txtGroup.Visible = lblGroup.Visible = true;
				}
				if (string.IsNullOrWhiteSpace(dynamicCommand.HavingText))
				{
					txtHaving.Visible = lblHaving.Visible = false; txtHaving.Text = string.Empty;
				}
				else
				{
					if (dynamicCommand.HavingText.IndexOf(Environment.NewLine) >= 0) { txtHaving.Text = dynamicCommand.HavingText; }
					else { txtHaving.Text = dynamicCommand.HavingText.Replace("\n", Environment.NewLine); }
					txtHaving.Visible = lblHaving.Visible = true;
				}
				if (string.IsNullOrWhiteSpace(dynamicCommand.OrderText))
				{
					txtOrder.Visible = lblOrder.Visible = false; txtOrder.Text = string.Empty;
				}
				else
				{
					if (dynamicCommand.OrderText.IndexOf(Environment.NewLine) >= 0) { txtOrder.Text = dynamicCommand.OrderText; }
					else { txtOrder.Text = dynamicCommand.OrderText.Replace("\n", Environment.NewLine); }
					txtOrder.Visible = lblOrder.Visible = true;
				}
			}

			/// <summary>
			/// 设置界面文本框值。
			/// </summary>
			/// <param name="value"></param>
			public object GetDynamicCommand(DynamicCommandElement dynamicCommand, PropertyDescriptor propertyDescriptor, object value)
			{
				dynamicCommand.SelectText = txtSelect.Text;
				dynamicCommand.FromText = txtFrom.Text;
				dynamicCommand.WhereText = txtWhere.Text;
				dynamicCommand.GroupText = txtGroup.Text;
				dynamicCommand.HavingText = txtHaving.Text;
				dynamicCommand.OrderText = txtOrder.Text;
				if (propertyDescriptor.Name == "SelectText")
					return dynamicCommand.SelectText;
				else if (propertyDescriptor.Name == "FromText")
					return dynamicCommand.FromText;
				else if (propertyDescriptor.Name == "WhereText")
					return dynamicCommand.WhereText;
				else if (propertyDescriptor.Name == "GroupText")
					return dynamicCommand.GroupText;
				else if (propertyDescriptor.Name == "HavingText")
					return dynamicCommand.HavingText;
				else if (propertyDescriptor.Name == "OrderText")
					return dynamicCommand.OrderText;
				return value;
			}
		}
	}
}
