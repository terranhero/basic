using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Basic.Windows
{
	/// <summary>
	/// 表示显示图像的控件(显示资源图像)。
	/// </summary>
	[System.ComponentModel.ToolboxItem(false)]
	public sealed class CommandNode : Control
	{
		static CommandNode() { DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandNode), new FrameworkPropertyMetadata(typeof(CommandNode))); }

		#region 属性 CommandType 定义
		public static readonly DependencyProperty CommandTypeProperty = DependencyProperty.Register("CommandType",
			typeof(CommandType), typeof(CommandNode), new PropertyMetadata(CommandType.Text));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public CommandType CommandType
		{
			get { return (CommandType)base.GetValue(CommandTypeProperty); }
			set { base.SetValue(CommandTypeProperty, value); }
		}
		#endregion
	}


}
