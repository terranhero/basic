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
	/// CommandWindow.xaml 的交互逻辑
	/// </summary>
	public partial class WithClauseWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly WithClause _WithClause;
		public WithClauseWindow(WithClause clause)
			: base()
		{
			InitializeComponent();
			this.DataContext = _WithClause = clause;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (sender == btnDelete) { this.DialogResult = true; this.Close(); }
			else if (sender == btnClose) { this.Close(); }
		}
	}
}
