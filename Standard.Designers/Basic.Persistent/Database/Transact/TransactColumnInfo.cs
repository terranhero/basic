using Basic.Collections;
using Basic.Designer;
using Basic.Enums;

namespace Basic.Database
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TransactColumnInfo : AbstractNotifyChangedDescriptor
    {
        private readonly TransactTableInfo _TableInfo;
        /// <summary>
        /// 初始化 TransactColumnInfo 类实例。
        /// </summary>
        /// <param name="table">需要通知 TableDesignerInfo 类实例当前类的属性已更改。</param>
        public TransactColumnInfo(TransactTableInfo table)
            : base(table) { _TableInfo = table; }

        /// <summary>
        /// 当前列是否是CreatedTime列。
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        internal bool IsCreatedTimeColumn
        {
            get
            {
                return (string.Compare(_Name, DesignTableInfo.CreatedTimeColumn, true) == 0 || string.Compare(_Name, DesignTableInfo.CreateTimeColumn, true) == 0)
                    && _DbType == DbTypeEnum.DateTime;
            }
        }

        /// <summary>
        /// 当前列是否是CreatedTime列。
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        internal bool IsModifiedTimeColumn
        {
            get
            {
                return ((string.Compare(_Name, DesignTableInfo.ModifiedTimeColumn, true) == 0 || string.Compare(_Name, DesignTableInfo.ModifyTimeColumn, true) == 0)
                    && (_DbType == DbTypeEnum.DateTime || _DbType == DbTypeEnum.DateTime2)) || _DbType == DbTypeEnum.Timestamp;
            }
        }

        private bool _PrimaryKey = false;
        /// <summary>
        /// 当前列是否为主键
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnPrimaryKey")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue(false)]
        public bool PrimaryKey
        {
            get { return _PrimaryKey; }
            set { if (_PrimaryKey != value) { _PrimaryKey = value; OnPropertyChanged("PrimaryKey"); } }
        }

        private bool _Computed = false;
        /// <summary>
        /// 当前列是否为计算列
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnComputed")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue(false)]
        public bool Computed
        {
            get { return _Computed; }
            set { if (_Computed != value) { _Computed = value; OnPropertyChanged("Computed"); } }
        }

        /// <summary>
        /// 获取或设置当前数据库列所在表的别名。
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnName")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.Browsable(false)]
        public TransactTableInfo TableInfo { get { return _TableInfo; } }


        /// <summary>
        /// 获取或设置当前数据库列所在表的别名。
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnName")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue("")]
        public string Alias
        {
            get
            {
                if (_TableInfo.Alias == TransactSqlResult.EmptyTableName)
                    return null;
                return _TableInfo.Alias;
            }
        }

        private string _Name = string.Empty;
        /// <summary>
        /// 获取或设置数据库表中的列的名称。
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnName")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue("")]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                    if (string.IsNullOrWhiteSpace(_Source)) { Source = value; }
                }
            }
        }

        private string _Source = string.Empty;
        /// <summary>
        /// 获取或设置数据库表中的列的原名称。
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_SourceName")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue("")]
        public string Source
        {
            get { return _Source; }
            set { if (_Source != value) { _Source = value; OnPropertyChanged("Source"); } }
        }

        private string _PropertyName = string.Empty;
        /// <summary>
        /// 获取或设置数据库表中的列的名称。
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_PropertyName")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue("")]
        public string PropertyName
        {
            get { return _PropertyName; }
            set
            {
                if (_PropertyName != value)
                {
                    _PropertyName = value;
                    OnPropertyChanged("PropertyName");
                }
            }
        }

        private string _Comment;
        /// <summary>
        /// 属性对应数据库字段描述
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_PropertyComment")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue("")]
        public string Comment
        {
            get { return _Comment; }
            set { if (_Comment != value) { _Comment = value; OnPropertyChanged("Comment"); } }
        }

        private DbTypeEnum _DbType = DbTypeEnum.NVarChar;
        /// <summary>
        /// 属性类型名称
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnDbType")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
        [System.ComponentModel.DefaultValue(typeof(DbTypeEnum), "NVarChar")]
        public DbTypeEnum DbType
        {
            get { return _DbType; }
            set { if (_DbType != value) { _DbType = value; OnPropertyChanged("DbType"); } }
        }

        private int _Size = 0;
        /// <summary>
        /// 获取或设置列中数据的最大大小（以字节为单位）。
        /// </summary>
        /// <value>列中数据的最大大小（以字节为单位）。默认值是从参数值推导出的。</value>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnSize")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
        [System.ComponentModel.DefaultValue(0)]
        public int Size
        {
            get { return _Size; }
            set { if (_Size != value) { _Size = value; OnPropertyChanged("Size"); } }
        }

        private byte _Precision = 0;
        /// <summary>
        /// 获取或设置用来表示 Value 属性的最大位数。 
        /// </summary>
        /// <value>用于表示 Value 属性的最大位数。 默认值为 0。这指示数据提供程序设置 Value 的精度。 </value>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnPrecision")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
        [System.ComponentModel.DefaultValue(typeof(byte), "0")]
        public byte Precision
        {
            get { return _Precision; }
            set { if (_Precision != value) { _Precision = value; OnPropertyChanged("Precision"); } }
        }

        private byte _Scale = 0;
        /// <summary>
        /// 获取或设置 Value 解析为的小数位数。
        /// </summary>
        /// <value>要将 Value 解析为的小数位数。默认值为 0。</value>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnScale")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
        [System.ComponentModel.DefaultValue(typeof(byte), "0")]
        public byte Scale
        {
            get { return _Scale; }
            set { if (_Scale != value) { _Scale = value; OnPropertyChanged("Scale"); } }
        }

        private bool _Nullable = false;
        /// <summary>
        /// 属性是否允许为空
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_PropertyNullable")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue(typeof(bool), "false")]
        public bool Nullable
        {
            get { return _Nullable; }
            set { if (_Nullable != value) { _Nullable = value; OnPropertyChanged("Nullable"); } }
        }

        private string _DefaultValue = null;
        /// <summary>
        /// 属性的默认值
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_PropertyDefaultValue")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        public string DefaultValue
        {
            get { return _DefaultValue; }
            set { if (_DefaultValue != value) { _DefaultValue = value; OnPropertyChanged("DefaultValue"); } }
        }
    }
}
