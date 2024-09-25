using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Basic.Options
{
	public partial class AbstractClassesControl : UserControl
	{
		public AbstractClassesControl(AbstractClassesOptions opts)
		{
			InitializeComponent(); options = opts;
			lstConditions.DataSource = options.BaseConditions;
			lstEntities.DataSource = options.BaseEntities;
			lstAccesses.DataSource = options.BaseAccess;
		}
		private TableLayoutPanel tableLayoutPanel1;
		private TextBox txtCondition;
		private Button addCondition;
		private Button removeCondiiton;
		private TextBox txtEntity;
		private TextBox txtAccess;
		private Button btnEntities;
		private Button removeEntities;
		private Button btnAccesses;
		private Button btnRemoveAccesses;
		private ListBox lstConditions;
		private ListBox lstEntities;
		private ListBox lstAccesses;
		private Label lblConditions;
		private Label lblEntities;
		private Label lblAccesses;
		private readonly AbstractClassesOptions options;

		public void Initialize()
		{
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.txtCondition = new System.Windows.Forms.TextBox();
			this.addCondition = new System.Windows.Forms.Button();
			this.removeCondiiton = new System.Windows.Forms.Button();
			this.txtEntity = new System.Windows.Forms.TextBox();
			this.txtAccess = new System.Windows.Forms.TextBox();
			this.btnEntities = new System.Windows.Forms.Button();
			this.removeEntities = new System.Windows.Forms.Button();
			this.btnAccesses = new System.Windows.Forms.Button();
			this.btnRemoveAccesses = new System.Windows.Forms.Button();
			this.lstConditions = new System.Windows.Forms.ListBox();
			this.lstEntities = new System.Windows.Forms.ListBox();
			this.lstAccesses = new System.Windows.Forms.ListBox();
			this.lblConditions = new System.Windows.Forms.Label();
			this.lblEntities = new System.Windows.Forms.Label();
			this.lblAccesses = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.txtCondition, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.addCondition, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.removeCondiiton, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.txtEntity, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.txtAccess, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.btnEntities, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.removeEntities, 3, 2);
			this.tableLayoutPanel1.Controls.Add(this.btnAccesses, 2, 4);
			this.tableLayoutPanel1.Controls.Add(this.btnRemoveAccesses, 3, 4);
			this.tableLayoutPanel1.Controls.Add(this.lstConditions, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.lstEntities, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.lstAccesses, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.lblConditions, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblEntities, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblAccesses, 0, 4);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 400);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// txtCondition
			// 
			this.txtCondition.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtCondition.Location = new System.Drawing.Point(62, 3);
			this.txtCondition.Name = "txtCondition";
			this.txtCondition.Size = new System.Drawing.Size(373, 21);
			this.txtCondition.TabIndex = 1;
			// 
			// addCondition
			// 
			this.addCondition.Dock = System.Windows.Forms.DockStyle.Fill;
			this.addCondition.Location = new System.Drawing.Point(441, 3);
			this.addCondition.Name = "addCondition";
			this.addCondition.Size = new System.Drawing.Size(75, 23);
			this.addCondition.TabIndex = 2;
			this.addCondition.Text = "添加/修改";
			this.addCondition.UseVisualStyleBackColor = true;
			this.addCondition.Click += new System.EventHandler(this.AddCondition_Click);
			// 
			// removeCondiiton
			// 
			this.removeCondiiton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.removeCondiiton.Enabled = false;
			this.removeCondiiton.Location = new System.Drawing.Point(522, 3);
			this.removeCondiiton.Name = "removeCondiiton";
			this.removeCondiiton.Size = new System.Drawing.Size(75, 23);
			this.removeCondiiton.TabIndex = 6;
			this.removeCondiiton.Text = "删除";
			this.removeCondiiton.UseVisualStyleBackColor = true;
			this.removeCondiiton.Click += new System.EventHandler(this.RemoveCondiitons_Click);
			// 
			// txtEntity
			// 
			this.txtEntity.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtEntity.Location = new System.Drawing.Point(62, 136);
			this.txtEntity.Name = "txtEntity";
			this.txtEntity.Size = new System.Drawing.Size(373, 21);
			this.txtEntity.TabIndex = 7;
			// 
			// txtAccess
			// 
			this.txtAccess.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtAccess.Location = new System.Drawing.Point(62, 269);
			this.txtAccess.Name = "txtAccess";
			this.txtAccess.Size = new System.Drawing.Size(373, 21);
			this.txtAccess.TabIndex = 8;
			// 
			// btnEntities
			// 
			this.btnEntities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnEntities.Location = new System.Drawing.Point(441, 136);
			this.btnEntities.Name = "btnEntities";
			this.btnEntities.Size = new System.Drawing.Size(75, 23);
			this.btnEntities.TabIndex = 9;
			this.btnEntities.Text = "添加/修改";
			this.btnEntities.UseVisualStyleBackColor = true;
			this.btnEntities.Click += new System.EventHandler(this.btnEntities_Click);
			// 
			// removeEntities
			// 
			this.removeEntities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.removeEntities.Enabled = false;
			this.removeEntities.Location = new System.Drawing.Point(522, 136);
			this.removeEntities.Name = "removeEntities";
			this.removeEntities.Size = new System.Drawing.Size(75, 23);
			this.removeEntities.TabIndex = 10;
			this.removeEntities.Text = "删除";
			this.removeEntities.UseVisualStyleBackColor = true;
			this.removeEntities.Click += new System.EventHandler(this.btnRemoveEntities_Click);
			// 
			// btnAccesses
			// 
			this.btnAccesses.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnAccesses.Location = new System.Drawing.Point(441, 269);
			this.btnAccesses.Name = "btnAccesses";
			this.btnAccesses.Size = new System.Drawing.Size(75, 23);
			this.btnAccesses.TabIndex = 11;
			this.btnAccesses.Text = "添加/修改";
			this.btnAccesses.UseVisualStyleBackColor = true;
			this.btnAccesses.Click += new System.EventHandler(this.btnAccesses_Click);
			// 
			// btnRemoveAccesses
			// 
			this.btnRemoveAccesses.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnRemoveAccesses.Enabled = false;
			this.btnRemoveAccesses.Location = new System.Drawing.Point(522, 269);
			this.btnRemoveAccesses.Name = "btnRemoveAccesses";
			this.btnRemoveAccesses.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveAccesses.TabIndex = 12;
			this.btnRemoveAccesses.Text = "删除";
			this.btnRemoveAccesses.UseVisualStyleBackColor = true;
			this.btnRemoveAccesses.Click += new System.EventHandler(this.btnRemoveAccesses_Click);
			// 
			// lstConditions
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.lstConditions, 4);
			this.lstConditions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstConditions.FormattingEnabled = true;
			this.lstConditions.ItemHeight = 12;
			this.lstConditions.Location = new System.Drawing.Point(3, 32);
			this.lstConditions.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this.lstConditions.Name = "lstConditions";
			this.lstConditions.Size = new System.Drawing.Size(594, 91);
			this.lstConditions.Sorted = true;
			this.lstConditions.TabIndex = 13;
			this.lstConditions.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstConditions_MouseClick);
			this.lstConditions.SelectedIndexChanged += new System.EventHandler(this.lstConditions_SelectedIndexChanged);
			// 
			// lstEntities
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.lstEntities, 4);
			this.lstEntities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstEntities.FormattingEnabled = true;
			this.lstEntities.ItemHeight = 12;
			this.lstEntities.Location = new System.Drawing.Point(3, 165);
			this.lstEntities.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this.lstEntities.Name = "lstEntities";
			this.lstEntities.Size = new System.Drawing.Size(594, 91);
			this.lstEntities.TabIndex = 14;
			this.lstEntities.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstEntities_MouseClick);
			this.lstEntities.SelectedIndexChanged += new System.EventHandler(this.lstEntities_SelectedIndexChanged);
			// 
			// lstAccesses
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.lstAccesses, 4);
			this.lstAccesses.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstAccesses.FormattingEnabled = true;
			this.lstAccesses.ItemHeight = 12;
			this.lstAccesses.Location = new System.Drawing.Point(3, 298);
			this.lstAccesses.Name = "lstAccesses";
			this.lstAccesses.Size = new System.Drawing.Size(594, 99);
			this.lstAccesses.TabIndex = 15;
			this.lstAccesses.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstAccesses_MouseClick);
			this.lstAccesses.SelectedIndexChanged += new System.EventHandler(this.lstAccesses_SelectedIndexChanged);
			// 
			// lblConditions
			// 
			this.lblConditions.AutoSize = true;
			this.lblConditions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblConditions.Location = new System.Drawing.Point(3, 3);
			this.lblConditions.Margin = new System.Windows.Forms.Padding(3);
			this.lblConditions.Name = "lblConditions";
			this.lblConditions.Size = new System.Drawing.Size(53, 23);
			this.lblConditions.TabIndex = 16;
			this.lblConditions.Text = "条件基类";
			this.lblConditions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblEntities
			// 
			this.lblEntities.AutoSize = true;
			this.lblEntities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblEntities.Location = new System.Drawing.Point(3, 136);
			this.lblEntities.Margin = new System.Windows.Forms.Padding(3);
			this.lblEntities.Name = "lblEntities";
			this.lblEntities.Size = new System.Drawing.Size(53, 23);
			this.lblEntities.TabIndex = 17;
			this.lblEntities.Text = "实体基类";
			this.lblEntities.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblAccesses
			// 
			this.lblAccesses.AutoSize = true;
			this.lblAccesses.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblAccesses.Location = new System.Drawing.Point(3, 269);
			this.lblAccesses.Margin = new System.Windows.Forms.Padding(3);
			this.lblAccesses.Name = "lblAccesses";
			this.lblAccesses.Size = new System.Drawing.Size(53, 23);
			this.lblAccesses.TabIndex = 18;
			this.lblAccesses.Text = "持久基类";
			this.lblAccesses.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AbstractClassesControl
			// 
			this.AutoSize = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "AbstractClassesControl";
			this.Size = new System.Drawing.Size(600, 400);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		private void AddCondition_Click(object sender, EventArgs e)
		{
			string text = txtCondition.Text.Trim();
			if (lstConditions.SelectedIndex >= 0)
			{
				options.BaseConditions[lstConditions.SelectedIndex] = text;
				options.SaveSettingsToStorage();
			}
			else if (options.BaseConditions.Contains(text) == false)
			{
				options.BaseConditions.Add(text);
				options.SaveSettingsToStorage();
			}
		}

		private void lstConditions_MouseClick(object sender, MouseEventArgs e)
		{
			if (lstConditions.IndexFromPoint(e.X, e.Y) < 0)
			{
				lstConditions.SelectedIndex = -1; removeCondiiton.Enabled = false;
				if (options.BaseConditions.Contains(txtCondition.Text.Trim()))
				{
					txtCondition.Text = "";
				}
			}
		}

		private void lstConditions_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstConditions.SelectedIndex >= 0)
			{
				txtCondition.Text = options.BaseConditions[lstConditions.SelectedIndex];
				removeCondiiton.Enabled = true;
			}
		}

		private void RemoveCondiitons_Click(object sender, EventArgs e)
		{
			if (lstConditions.SelectedIndex >= 0)
			{
				options.BaseConditions.RemoveAt(lstConditions.SelectedIndex);
				txtCondition.Text = ""; lstConditions.SelectedIndex = -1;
				options.SaveSettingsToStorage();
			}
		}

		private void btnEntities_Click(object sender, EventArgs e)
		{
			string text = txtEntity.Text.Trim();
			if (lstEntities.SelectedIndex >= 0)
			{
				options.BaseEntities[lstEntities.SelectedIndex] = text;
				options.SaveSettingsToStorage();
			}
			else if (options.BaseEntities.Contains(text) == false)
			{
				options.BaseEntities.Add(text);
				options.SaveSettingsToStorage();
			}
		}

		private void lstEntities_MouseClick(object sender, MouseEventArgs e)
		{
			if (lstEntities.IndexFromPoint(e.X, e.Y) < 0)
			{
				lstEntities.SelectedIndex = -1; removeEntities.Enabled = false;
				if (options.BaseEntities.Contains(txtEntity.Text.Trim()))
				{
					txtEntity.Text = "";
				}
			}
		}

		private void btnRemoveEntities_Click(object sender, EventArgs e)
		{
			if (lstEntities.SelectedIndex >= 0)
			{
				options.BaseEntities.RemoveAt(lstEntities.SelectedIndex);
				txtEntity.Text = ""; lstEntities.SelectedIndex = -1;
				options.SaveSettingsToStorage();
			}
		}

		private void lstEntities_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstEntities.SelectedIndex >= 0)
			{
				txtEntity.Text = options.BaseEntities[lstEntities.SelectedIndex];
				removeEntities.Enabled = true;
			}
		}

		private void btnAccesses_Click(object sender, EventArgs e)
		{
			string text = txtAccess.Text.Trim();
			if (lstAccesses.SelectedIndex >= 0)
			{
				options.BaseAccess[lstAccesses.SelectedIndex] = text;
				options.SaveSettingsToStorage();
			}
			else if (options.BaseAccess.Contains(text) == false)
			{
				options.BaseAccess.Add(text);
				options.SaveSettingsToStorage();
			}
		}

		private void lstAccesses_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstAccesses.SelectedIndex >= 0)
			{
				txtAccess.Text = options.BaseAccess[lstAccesses.SelectedIndex];
				btnRemoveAccesses.Enabled = true;
			}
		}

		private void btnRemoveAccesses_Click(object sender, EventArgs e)
		{
			if (lstAccesses.SelectedIndex >= 0)
			{
				options.BaseAccess.RemoveAt(lstAccesses.SelectedIndex);
				txtAccess.Text = ""; lstAccesses.SelectedIndex = -1; 
				options.SaveSettingsToStorage();
			}
		}

		private void lstAccesses_MouseClick(object sender, MouseEventArgs e)
		{
			if (lstAccesses.IndexFromPoint(e.X, e.Y) < 0)
			{
				lstAccesses.SelectedIndex = -1; btnRemoveAccesses.Enabled = false;
				if (options.BaseAccess.Contains(txtAccess.Text.Trim()))
				{
					txtAccess.Text = "";
				}
			}
		}
	}
}
