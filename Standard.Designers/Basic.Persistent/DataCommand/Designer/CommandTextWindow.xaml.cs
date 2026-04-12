using System;
using System.Windows;

namespace Basic.Designer
{
	/// <summary>
	/// CommandTextForm.xaml 的交互逻辑
	/// </summary>
	public partial class CommandTextForm : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		static CommandTextForm()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandTextForm),
				new FrameworkPropertyMetadata(typeof(CommandTextForm)));
		}
		protected override void InvokeDialogHelp()
		{
		}

		public CommandTextForm() : base() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void SetCommandText(object value)
		{
			string text = Convert.ToString(value);
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (text.IndexOf(Environment.NewLine) >= 0) { CommandText = text; }
				else { CommandText = text.Replace("\n", Environment.NewLine); }
			}
		}

		#region 属性 SelectedTable 定义
		public static readonly DependencyProperty CommandTextProperty = DependencyProperty.Register("CommandText",
			typeof(string), typeof(CommandTextForm), new PropertyMetadata(null));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public string CommandText
		{
			get { return (string)base.GetValue(CommandTextProperty); }
			set { base.SetValue(CommandTextProperty, value); }
		}
		#endregion

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}
	}
}
