using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basic.Options
{
	public partial class AbstractClassesControl : UserControl
	{
		public AbstractClassesControl(AbstractClassesOptions opts)
		{
			InitializeComponent();
			options = opts;
		}
		private TableLayoutPanel tableLayoutPanel1;
		private TextBox txtClasses;
		private Button btnConditions;
		private Button btnRemoveCondiitons;
		private TextBox textBox1;
		private TextBox textBox2;
		private Button btnEntities;
		private Button btnRemoveEntities;
		private Button btnAccesses;
		private Button btnRemoveAccesses;
		private ListBox lstConditions;
		private ListBox lstEntities;
		private ListBox lstAccesses;
		private readonly AbstractClassesOptions options;

		public void Initialize()
		{
			lstConditions.Items.AddRange(options.BaseConditions.ToArray());
			lstEntities.Items.AddRange(options.BaseEntities.ToArray());
			lstAccesses.Items.AddRange(options.BaseAccess.ToArray());
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.txtClasses = new System.Windows.Forms.TextBox();
			this.btnConditions = new System.Windows.Forms.Button();
			this.btnRemoveCondiitons = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.btnEntities = new System.Windows.Forms.Button();
			this.btnRemoveEntities = new System.Windows.Forms.Button();
			this.btnAccesses = new System.Windows.Forms.Button();
			this.btnRemoveAccesses = new System.Windows.Forms.Button();
			this.lstConditions = new System.Windows.Forms.ListBox();
			this.lstEntities = new System.Windows.Forms.ListBox();
			this.lstAccesses = new System.Windows.Forms.ListBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.txtClasses, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnConditions, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnRemoveCondiitons, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.textBox2, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.btnEntities, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.btnRemoveEntities, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.btnAccesses, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.btnRemoveAccesses, 2, 4);
			this.tableLayoutPanel1.Controls.Add(this.lstConditions, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.lstEntities, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.lstAccesses, 0, 5);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 400);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// txtClasses
			// 
			this.txtClasses.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtClasses.Location = new System.Drawing.Point(3, 3);
			this.txtClasses.Name = "txtClasses";
			this.txtClasses.Size = new System.Drawing.Size(432, 21);
			this.txtClasses.TabIndex = 1;
			// 
			// btnConditions
			// 
			this.btnConditions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnConditions.Location = new System.Drawing.Point(441, 3);
			this.btnConditions.Name = "btnConditions";
			this.btnConditions.Size = new System.Drawing.Size(75, 23);
			this.btnConditions.TabIndex = 2;
			this.btnConditions.Text = "添加/修改";
			this.btnConditions.UseVisualStyleBackColor = true;
			this.btnConditions.Click += new System.EventHandler(this.btnConditions_Click);
			// 
			// btnRemoveCondiitons
			// 
			this.btnRemoveCondiitons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnRemoveCondiitons.Enabled = false;
			this.btnRemoveCondiitons.Location = new System.Drawing.Point(522, 3);
			this.btnRemoveCondiitons.Name = "btnRemoveCondiitons";
			this.btnRemoveCondiitons.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveCondiitons.TabIndex = 6;
			this.btnRemoveCondiitons.Text = "删除";
			this.btnRemoveCondiitons.UseVisualStyleBackColor = true;
			this.btnRemoveCondiitons.Click += new System.EventHandler(this.btnRemoveCondiitons_Click);
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(3, 136);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(432, 21);
			this.textBox1.TabIndex = 7;
			// 
			// textBox2
			// 
			this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox2.Location = new System.Drawing.Point(3, 269);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(432, 21);
			this.textBox2.TabIndex = 8;
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
			// btnRemoveEntities
			// 
			this.btnRemoveEntities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnRemoveEntities.Enabled = false;
			this.btnRemoveEntities.Location = new System.Drawing.Point(522, 136);
			this.btnRemoveEntities.Name = "btnRemoveEntities";
			this.btnRemoveEntities.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveEntities.TabIndex = 10;
			this.btnRemoveEntities.Text = "删除";
			this.btnRemoveEntities.UseVisualStyleBackColor = true;
			this.btnRemoveEntities.Click += new System.EventHandler(this.btnRemoveEntities_Click);
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
			this.tableLayoutPanel1.SetColumnSpan(this.lstConditions, 3);
			this.lstConditions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstConditions.FormattingEnabled = true;
			this.lstConditions.ItemHeight = 12;
			this.lstConditions.Location = new System.Drawing.Point(3, 32);
			this.lstConditions.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this.lstConditions.Name = "lstConditions";
			this.lstConditions.Size = new System.Drawing.Size(594, 91);
			this.lstConditions.Sorted = true;
			this.lstConditions.TabIndex = 13;
			this.lstConditions.SelectedIndexChanged += new System.EventHandler(this.lstConditions_SelectedIndexChanged);
			// 
			// lstEntities
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.lstEntities, 3);
			this.lstEntities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstEntities.FormattingEnabled = true;
			this.lstEntities.ItemHeight = 12;
			this.lstEntities.Location = new System.Drawing.Point(3, 165);
			this.lstEntities.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this.lstEntities.Name = "lstEntities";
			this.lstEntities.Size = new System.Drawing.Size(594, 91);
			this.lstEntities.TabIndex = 14;
			this.lstEntities.SelectedIndexChanged += new System.EventHandler(this.lstEntities_SelectedIndexChanged);
			// 
			// lstAccesses
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.lstAccesses, 3);
			this.lstAccesses.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstAccesses.FormattingEnabled = true;
			this.lstAccesses.ItemHeight = 12;
			this.lstAccesses.Location = new System.Drawing.Point(3, 298);
			this.lstAccesses.Name = "lstAccesses";
			this.lstAccesses.Size = new System.Drawing.Size(594, 99);
			this.lstAccesses.TabIndex = 15;
			this.lstAccesses.SelectedIndexChanged += new System.EventHandler(this.lstAccesses_SelectedIndexChanged);
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

		private void btnConditions_Click(object sender, EventArgs e)
		{
			options.BaseConditions.Add(txtClasses.Text.Trim());
			lstConditions.Items.Add(txtClasses.Text.Trim());
			options.SaveSettingsToStorage();
		}

		private void lstConditions_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstConditions.SelectedIndex >= 0) { btnRemoveCondiitons.Enabled = true; }
		}

		private void btnRemoveCondiitons_Click(object sender, EventArgs e)
		{
			options.SaveSettingsToStorage();
		}

		private void btnEntities_Click(object sender, EventArgs e)
		{
			options.BaseConditions.Add(txtClasses.Text.Trim());
			lstConditions.Items.Add(txtClasses.Text.Trim());
		}
		private void lstEntities_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstEntities.SelectedIndex >= 0)
			{
				btnRemoveEntities.Enabled = true;
			}
		}
		private void btnAccesses_Click(object sender, EventArgs e)
		{
			options.BaseConditions.Add(txtClasses.Text.Trim());
			lstConditions.Items.Add(txtClasses.Text.Trim());
		}
		private void lstAccesses_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstAccesses.SelectedIndex >= 0)
			{
				btnRemoveAccesses.Enabled = true;
			}
		}


		private void btnRemoveEntities_Click(object sender, EventArgs e)
		{

		}

		private void btnRemoveAccesses_Click(object sender, EventArgs e)
		{

		}
	}
}
