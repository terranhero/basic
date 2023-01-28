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
		private static RoutedUICommand _CreateResource;
		/// <summary>
		/// 表示 CreateResource 命令
		/// </summary>
		public static RoutedUICommand CreateResource
		{
			get
			{
				if (_CreateResource == null)
				{
					string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_CreateResource", CultureInfo.CurrentUICulture);
					_CreateResource = new RoutedUICommand(commandName, "CreateResource", typeof(BasicCommands));
				}
				return _CreateResource;
			}
		}

		private static RoutedUICommand _ResetResource;
		/// <summary>
		/// 表示 CreateResource 命令
		/// </summary>
		public static RoutedUICommand ResetResource
		{
			get
			{
				if (_ResetResource == null)
				{
					string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_ResetResource", CultureInfo.CurrentUICulture);
					_ResetResource = new RoutedUICommand(commandName, "ResetResource", typeof(BasicCommands));
				}
				return _ResetResource;
			}
		}
	}
}
