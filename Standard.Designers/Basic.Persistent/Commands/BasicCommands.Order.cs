using System.Windows.Input;
using Basic.Designer;
using Basic.Properties;
using System.Globalization;

namespace Basic.Commands
{
	/// <summary>
	/// 提供一组标准的与数据持久类相关的命令。
	/// </summary>
	public static partial class BasicCommands
	{
		private static RoutedUICommand _Order;
		/// <summary>
		/// 表示 Order 命令
		/// </summary>
		public static RoutedUICommand Order
		{
			get
			{
				if (_Order == null)
				{
					string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_OrderCommand", CultureInfo.CurrentUICulture);
					_Order = new RoutedUICommand(commandName, "Order", typeof(BasicCommands));
				}
				return _Order;
			}
		}

		private static RoutedUICommand _BringForward;
		/// <summary>
		/// 表示 BringForward 命令
		/// </summary>
		public static RoutedUICommand BringForward
		{
			get
			{
				if (_BringForward == null)
				{
					string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_BringForward", CultureInfo.CurrentUICulture);
					_BringForward = new RoutedUICommand(commandName, "BringForward", typeof(BasicCommands));
				}
				return _BringForward;
			}
		}

		private static RoutedUICommand _BringToFront;
		/// <summary>
		/// 表示 BringToFront 命令
		/// </summary>
		public static RoutedUICommand BringToFront
		{
			get
			{
				if (_BringToFront == null)
				{
					string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_BringToFront", CultureInfo.CurrentUICulture);
					_BringToFront = new RoutedUICommand(commandName, "BringToFront", typeof(BasicCommands));
				}
				return _BringToFront;
			}
		}

		private static RoutedUICommand _SendBackward;
		/// <summary>
		/// 表示 SendBackward 命令
		/// </summary>
		public static RoutedUICommand SendBackward
		{
			get
			{
				if (_SendBackward == null)
				{
					string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_SendBackward", CultureInfo.CurrentUICulture);
					_SendBackward = new RoutedUICommand(commandName, "SendBackward", typeof(BasicCommands));
				}
				return _SendBackward;
			}
		}

		private static RoutedUICommand _SendToBack;
		/// <summary>
		/// 表示 SendBackward 命令
		/// </summary>
		public static RoutedUICommand SendToBack
		{
			get
			{
				if (_SendToBack == null)
				{
					string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_SendToBack", CultureInfo.CurrentUICulture);
					_SendToBack = new RoutedUICommand(commandName, "SendToBack", typeof(BasicCommands));
				}
				return _SendToBack;
			}
		}
	}
}
