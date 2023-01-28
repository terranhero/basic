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
        private static RoutedUICommand _InsertProperty;
        /// <summary>
        /// 表示 Insert 命令
        /// </summary>
        public static RoutedUICommand InsertProperty
        {
            get
            {
                if (_InsertProperty == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_InsertProperty", CultureInfo.CurrentUICulture);
                    _InsertProperty = new RoutedUICommand(commandName, "InsertProperty", typeof(BasicCommands));
                    _InsertProperty.InputGestures.Add(new KeyGesture(Key.Insert));
                }
                return _InsertProperty;
            }
        }

        private static RoutedUICommand _NewProperty;
        /// <summary>
        /// 表示 Insert 命令
        /// </summary>
        public static RoutedUICommand NewProperty
        {
            get
            {
                if (_NewProperty == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_NewProperty", CultureInfo.CurrentUICulture);
                    _NewProperty = new RoutedUICommand(commandName, "NewProperty", typeof(BasicCommands));
                    _NewProperty.InputGestures.Add(new KeyGesture(Key.Add, ModifierKeys.Control));
                }
                return _NewProperty;
            }
        }

        private static RoutedUICommand _ResetWidth;
        /// <summary>
        /// 表示重置形状宽度 命令
        /// </summary>
        public static RoutedUICommand ResetWidth
        {
            get
            {
                if (_ResetWidth == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_ResetWidth", CultureInfo.CurrentUICulture);
                    _ResetWidth = new RoutedUICommand(commandName, "ResetWidth", typeof(BasicCommands));
                }
                return _ResetWidth;
            }
        }

        private static RoutedUICommand _RefreshConnection;
        /// <summary>
        /// 表示 重置数据库连接 命令
        /// </summary>
        public static RoutedUICommand RefreshConnection
        {
            get
            {
                if (_RefreshConnection == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_RefreshConnection", CultureInfo.CurrentUICulture);
                    _RefreshConnection = new RoutedUICommand(commandName, "RefreshConnection", typeof(BasicCommands));
                }
                return _RefreshConnection;
            }
        }

        private static RoutedUICommand _EditCode;
        /// <summary>
        /// 表示 AutoSize 命令
        /// </summary>
        public static RoutedUICommand EditCode
        {
            get
            {
                if (_EditCode == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_EditCode", CultureInfo.CurrentUICulture);
                    _EditCode = new RoutedUICommand(commandName, "EditCode", typeof(BasicCommands));
                    _EditCode.InputGestures.Add(new KeyGesture(Key.F7));
                }
                return _EditCode;
            }
        }

        private static RoutedUICommand _EditCommand;
        /// <summary>
        /// 表示 AutoSize 命令
        /// </summary>
        public static RoutedUICommand EditCommand
        {
            get
            {
                if (_EditCommand == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_EditCommand", CultureInfo.CurrentUICulture);
                    _EditCommand = new RoutedUICommand(commandName, "EditCommand", typeof(BasicCommands));
                }
                return _EditCommand;
            }
        }

        private static RoutedUICommand _Remove;
        /// <summary>
        /// 表示 SendBackward 命令
        /// </summary>
        public static RoutedUICommand Remove
        {
            get
            {
                if (_Remove == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_Remove", CultureInfo.CurrentUICulture);
                    _Remove = new RoutedUICommand(commandName, "Remove", typeof(BasicCommands));
                    _Remove.InputGestures.Add(new KeyGesture(Key.Delete));
                }
                return _Remove;
            }
        }

        private static RoutedUICommand _Properties;
        /// <summary>
        /// 表示 SendBackward 命令
        /// </summary>
        public static RoutedUICommand Properties
        {
            get
            {
                if (_Properties == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_Properties", CultureInfo.CurrentUICulture);
                    _Properties = new RoutedUICommand(commandName, "Properties", typeof(BasicCommands));
                    _Properties.InputGestures.Add(new KeyGesture(Key.F4));
                }
                return _Properties;
            }
        }

        private static RoutedUICommand _Create;
        /// <summary>
        /// 表示 SendBackward 命令
        /// </summary>
        public static RoutedUICommand CreateCommand
        {
            get
            {
                if (_Create == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_CreateCommand", CultureInfo.CurrentUICulture);
                    _Create = new RoutedUICommand(commandName, "CreateCommand", typeof(BasicCommands));
                }
                return _Create;
            }
        }

        private static RoutedUICommand _GroupCommand;
        /// <summary>
        /// 表示 GroupCommand 命令
        /// </summary>
        public static RoutedUICommand Group
        {
            get
            {
                if (_GroupCommand == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_AddGroup", CultureInfo.CurrentUICulture);
                    _GroupCommand = new RoutedUICommand(commandName, "GroupCommand", typeof(BasicCommands));
                }
                return _GroupCommand;
            }
        }

        private static RoutedUICommand _PasteStaticCommand;
        /// <summary>
        /// 表示 PasteStaticCommand 命令
        /// </summary>
        public static RoutedUICommand PasteStaticCommand
        {
            get
            {
                if (_PasteStaticCommand == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_PasteStaticCommand", CultureInfo.CurrentUICulture);
                    _PasteStaticCommand = new RoutedUICommand(commandName, "GroupCommand", typeof(BasicCommands));
                }
                return _PasteStaticCommand;
            }
        }

        private static RoutedUICommand _PasteDynamicCommand;
        /// <summary>
        /// 表示 PasteDynamicCommand 命令
        /// </summary>
        public static RoutedUICommand PasteDynamicCommand
        {
            get
            {
                if (_PasteDynamicCommand == null)
                {
                    string commandName = DesignerStrings.ResourceManager.GetString("BasicCommands_PasteDynamicCommand", CultureInfo.CurrentUICulture);
                    _PasteDynamicCommand = new RoutedUICommand(commandName, "GroupCommand", typeof(BasicCommands));
                }
                return _PasteDynamicCommand;
            }
        }
    }
}
