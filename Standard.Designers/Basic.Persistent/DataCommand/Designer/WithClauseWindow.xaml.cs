using System.Windows;
using System.Windows.Input;

namespace Basic.Designer
{
	/// <summary>
	/// WithClauseDialog.xaml 的交互逻辑
	/// </summary>
	public partial class WithClauseDialog : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		static WithClauseDialog()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(WithClauseDialog),
				new FrameworkPropertyMetadata(typeof(WithClauseDialog)));
		}
		private readonly WithClause _WithClause;
		public WithClauseDialog(WithClause clause) : base()
		{
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDeleteExecuted));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnCloseExecuted));
			this.DataContext = _WithClause = clause;
		}

		private void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}
	}
}
