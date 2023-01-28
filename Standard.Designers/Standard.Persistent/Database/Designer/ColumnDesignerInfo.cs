using Basic.Designer;
using Basic.EntityLayer;
using Basic.Collections;
using Basic.Enums;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows;

namespace Basic.Database
{
	/// <summary>
	/// 表示数据库表对象的设计时类型
	/// </summary>
	public sealed class ColumnDesignerInfo : AbstractNotifyChangedDescriptor
	{
		private readonly TableDesignerInfo tableInfo;
		private readonly TableDesignerCollection tableDesigners;
		/// <summary>
		/// 初始化 TableDesignerInfo 类实例。
		/// </summary>
		public ColumnDesignerInfo() : base() { }

		/// <summary>
		/// 初始化 TableDesignerInfo 类实例。
		/// </summary>
		/// <param name="table">需要通知 TableDesignerInfo 类实例当前类的属性已更改。</param>
		public ColumnDesignerInfo(TableDesignerInfo table)
			: base(table)
		{
			tableInfo = table;
			tableDesigners = table.Tables;
			tableDesigners.PropertyChanged += new PropertyChangedEventHandler(tableDesigners_PropertyChanged);
		}

		private void tableDesigners_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Group")
			{
				base.OnPropertyChanged(e.PropertyName);
				tableDesigners.OnColumnChanged(this);
			}
		}

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


		/// <summary>
		/// 将 TableInfoElement 对象的内容复制到当前实例。
		/// </summary>
		/// <param name="column">标识需要复制的 TableColumnElement 类实例。</param>
		public void CopyFrom(DesignColumnInfo column)
		{
			_PrimaryKey = column.PrimaryKey;
			_Comment = column.Comment;
			_Computed = column.Computed;
			_DbType = column.DbType;
			_DefaultValue = column.DefaultValue;
			_Name = column.Name;
			_PropertyName = column.PropertyName;
			_Nullable = column.Nullable;
			_Precision = column.Precision;
			_Scale = column.Scale;
			_Size = column.Size;
		}

		/// <summary>
		/// 将 TableInfoElement 对象的内容复制到当前实例。
		/// </summary>
		/// <param name="column">标识需要复制的 TableColumnElement 类实例。</param>
		public void CopyFrom(ColumnDesignerInfo column)
		{
			_PrimaryKey = column.PrimaryKey;
			_Comment = column.Comment;
			_Computed = column.Computed;
			_DbType = column.DbType;
			_DefaultValue = column.DefaultValue;
			_Name = column.Name;
			_PropertyName = column.PropertyName;
			_Nullable = column.Nullable;
			_Precision = column.Precision;
			_Scale = column.Scale;
			_Size = column.Size;
		}

		/// <summary>
		/// 拥有此列的 TableDesignerInfo 类实例。
		/// </summary>
		public TableDesignerInfo Table { get { return tableInfo; } }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return _Name; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return typeof(ColumnDesignerInfo).Name; }

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		public bool Group { get { return tableInfo.Tables.Group; } }

		private string _Aggregate = string.Empty;
		/// <summary>
		/// 获取或设置数据库表中的列的名称。
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnName")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue("")]
		public string Aggregate
		{
			get { return _Aggregate; }
			set
			{
				if (_Aggregate != value)
				{
					_Aggregate = value;
					base.OnPropertyChanged("Aggregate");
					base.OnPropertyChanged("IsAggregate");
					tableDesigners.OnColumnChanged(this);
				}
			}
		}

		/// <summary>
		/// 获取 SELECT GROUP BY 语句中列限定全名。
		/// </summary>
		public string AggregateName
		{
			get
			{
				if (tableInfo.Alias == tableInfo.Name)
				{
					if (string.IsNullOrWhiteSpace(_Aggregate))
						return _Name;
					else
						return string.Concat(_Aggregate, "(", _Name, ") ");
				}
				if (string.IsNullOrWhiteSpace(_Aggregate))
					return string.Concat(tableInfo.Alias, ".", _Name);
				return string.Concat(_Aggregate, "(", tableInfo.Alias, ".", _Name, ")");
			}
		}

		/// <summary>
		/// 获取当前列是否为聚合列。
		/// </summary>
		public bool HasAggregate { get { return !string.IsNullOrWhiteSpace(_Aggregate); } }

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

		private string _Alias = null;
		/// <summary>
		/// 获取或设置当前数据库列所在表的别名。
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnName")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue("")]
		public string Alias
		{
			get { if (string.IsNullOrWhiteSpace(_Alias)) { return tableInfo.Alias; } else { return _Alias; } }
			set { if (_Alias != value) { _Alias = value; OnPropertyChanged("Alias"); } }
		}

		private OrderEnum _SortOrder = OrderEnum.None;
		/// <summary>
		/// 获取或设置当前列的排序方式。
		/// </summary>
		public OrderEnum SortOrder
		{
			get { return _SortOrder; }
			set
			{
				if (_SortOrder != value)
				{
					_SortOrder = value;
					base.OnPropertyChanged("SortOrder");
					tableDesigners.OnColumnChanged(this);
				}
			}
		}

		/// <summary>
		/// 获取 SELECT GROUP BY 语句中列限定全名。
		/// </summary>
		public string GroupName
		{
			get
			{
				if (tableInfo.Alias == tableInfo.Name)
				{
					if (string.IsNullOrWhiteSpace(_Aggregate))
						return _Name;
					else
						return string.Concat(_Aggregate, "(", _Name, ") AS ", _Name);
				}
				if (string.IsNullOrWhiteSpace(_Aggregate))
					return string.Concat(tableInfo.Alias, ".", _Name);
				return string.Concat(_Aggregate, "(", tableInfo.Alias, ".", _Name, ") AS ", _Name);
			}
		}

		/// <summary>
		/// 获取 SELECT 语句中列限定全名。
		/// </summary>
		public string SelectName
		{
			get
			{
				if (tableInfo.Alias == tableInfo.Name)
					return _Name;
				return string.Concat(tableInfo.Alias, ".", _Name);
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
			set { if (_Name != value) { _Name = value; OnPropertyChanged("Name"); } }
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

		/// <summary>
		/// 判断当前列是否允许使用默认值。
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnIsWhere")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue(false)]
		public bool CanUseDefault
		{
			get { return !string.IsNullOrWhiteSpace(_DefaultValue) && !_IsWhere; }
		}

		private bool _UseDefault = false;
		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnIsWhere")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue(false)]
		public bool UseDefault
		{
			get { return _UseDefault; }
			set
			{
				if (_UseDefault != value)
				{
					_UseDefault = value; OnPropertyChanged("UseDefault");
					tableDesigners.OnUseDefaultChanged(this);
				}
			}
		}

		private bool _Checked = false;
		/// <summary>
		/// 当前列是否已选择
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnChecked")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Checked
		{
			get { return _Checked; }
			set
			{
				if (_Checked != value)
				{
					_Checked = value;
					OnPropertyChanged("Checked");
					tableDesigners.OnCheckedChanged(this);
				}
			}
		}

		private bool _Selected = true;
		/// <summary>
		/// 当前列是否选择输出
		/// </summary>
		public bool Selected
		{
			get { return _Selected; }
			set
			{
				if (_Selected != value)
				{
					_Selected = value;
					OnPropertyChanged("Selected");
					tableDesigners.OnColumnChanged(this);
				}
			}
		}

		private bool _IsWhere = false;
		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnIsWhere")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue(false)]
		public bool IsWhere
		{
			get { return _IsWhere; }
			set
			{
				if (_IsWhere != value)
				{
					_IsWhere = value; OnPropertyChanged("IsWhere");
					tableDesigners.OnIsWhereChanged(this);
				}
			}
		}

		private bool _IsFrom = false;
		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnIsWhere")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue(false)]
		public bool IsFrom
		{
			get { return _IsFrom; }
			set
			{
				if (_IsFrom != value)
				{
					_IsFrom = value; OnPropertyChanged("IsFrom");
					tableDesigners.OnIsFromChanged(this);
				}
			}
		}
	}
}
