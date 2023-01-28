using System;
using System.Windows;
using System.Windows.Controls;
using Basic.Collections;
using Basic.Configuration;
using Basic.Database;
using Basic.DataContexts;
using Basic.DataEntities;
using Basic.Enums;

namespace Basic.Designer
{
	/// <summary>
	/// CommandTextWindow.xaml 的交互逻辑
	/// </summary>
	public partial class CommandTextWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		public CommandTextWindow(object value)
			: base() { InitializeComponent(); SetCommandText(value); }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void SetCommandText(object value)
		{
			string text = Convert.ToString(value);
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (text.IndexOf(Environment.NewLine) >= 0) { txtCommandText.Text = text; }
				else { txtCommandText.Text = text.Replace("\n", Environment.NewLine); }
			}
		}

		/// <summary>
		/// 文本框内容。
		/// </summary>
		public string CommandText { get { return txtCommandText.Text; } }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}
	}
}
