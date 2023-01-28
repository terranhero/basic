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
        private static RoutedUICommand _UpdateTable;
        /// <summary>
        /// 表示 刷新表结构 命令
        /// </summary>
        public static RoutedUICommand UpdateTable
        {
            get
            {
                if (_UpdateTable == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_UpdateTable");
                    _UpdateTable = new RoutedUICommand(commandName, "UpdateTable", typeof(BasicCommands));
                }
                return _UpdateTable;
            }
        }

        private static RoutedUICommand _InitializeTable;
        /// <summary>
        /// 表示 刷新表结构 命令
        /// </summary>
        public static RoutedUICommand InitializeTable
        {
            get
            {
                if (_InitializeTable == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_InitializeTable", CultureInfo.CurrentUICulture);
                    _InitializeTable = new RoutedUICommand(commandName, "RefreshTable", typeof(BasicCommands));
                }
                return _InitializeTable;
            }
        }

        private static RoutedUICommand _UpdateCommand;
        /// <summary>
        /// 表示 刷新实体类 命令
        /// </summary>
        public static RoutedUICommand UpdateCommand
        {
            get
            {
                if (_UpdateCommand == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_UpdateCommand", CultureInfo.CurrentUICulture);
                    _UpdateCommand = new RoutedUICommand(commandName, "UpdateCommand", typeof(BasicCommands));
                }
                return _UpdateCommand;
            }
        }

        private static RoutedUICommand _UpdateCondition;
        /// <summary>
        /// 表示 刷新实体类 命令
        /// </summary>
        public static RoutedUICommand UpdateCondition
        {
            get
            {
                if (_UpdateCondition == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_UpdateCondition", CultureInfo.CurrentUICulture);
                    _UpdateCondition = new RoutedUICommand(commandName, "UpdateCondition", typeof(BasicCommands));
                }
                return _UpdateCondition;
            }
        }

        private static RoutedUICommand _UpdateEntity;
        /// <summary>
        /// 表示 刷新实体类 命令
        /// </summary>
        public static RoutedUICommand UpdateEntity
        {
            get
            {
                if (_UpdateEntity == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_UpdateEntity", CultureInfo.CurrentUICulture);
                    _UpdateEntity = new RoutedUICommand(commandName, "UpdateEntity", typeof(BasicCommands));
                }
                return _UpdateEntity;
            }
        }

        private static RoutedUICommand _AddCommand;
        /// <summary>
        /// 表示 刷新实体类 命令
        /// </summary>
        public static RoutedUICommand Add
        {
            get
            {
                if (_AddCommand == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_AddCommand", CultureInfo.CurrentUICulture);
                    _AddCommand = new RoutedUICommand(commandName, "AddCommand", typeof(BasicCommands));
                }
                return _AddCommand;
            }
        }

        private static RoutedUICommand _AddTableObject;
        /// <summary>
        /// 表示 刷新实体类 命令
        /// </summary>
        public static RoutedUICommand AddTable
        {
            get
            {
                if (_AddTableObject == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_AddTable", CultureInfo.CurrentUICulture);
                    _AddTableObject = new RoutedUICommand(commandName, "AddTable", typeof(BasicCommands));
                }
                return _AddTableObject;
            }
        }

        private static RoutedUICommand _AddRelationObject;
        /// <summary>
        /// 表示 刷新实体类 命令
        /// </summary>
        public static RoutedUICommand AddRelation
        {
            get
            {
                if (_AddRelationObject == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_AddRelation", CultureInfo.CurrentUICulture);
                    _AddRelationObject = new RoutedUICommand(commandName, "AddRelation", typeof(BasicCommands));
                }
                return _AddRelationObject;
            }
        }
    }
}
