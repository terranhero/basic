using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Designer;
using Basic.Collections;
using Basic.Enums;

namespace Basic.Database
{
	/// <summary>
	/// 表示数据库表对象的设计时类型
	/// </summary>
	public class TableDesignerInfo : AbstractNotifyChangedDescriptor
	{
		#region 实体定义字段
		internal const string CreatedTimeColumn = "CREATEDTIME";
		internal const string CreateTimeColumn = "CREATETIME";
		internal const string ModifiedTimeColumn = "MODIFIEDTIME";
		internal const string ModifyTimeColumn = "MODIFYTIME";
		#endregion
		private readonly TableDesignerCollection _Tables;
		private readonly ColumnDesignerCollection _Columns;

		/// <summary>
		/// 初始化 TableDesignerInfo 类实例。
		/// </summary>
		internal TableDesignerInfo(TableDesignerCollection tables) : this(tables, null, false) { }

		/// <summary>
		/// 初始化 TableDesignerInfo 类实例。
		/// </summary>
		internal TableDesignerInfo(TableDesignerCollection tables, string aliasName) : this(tables, aliasName, false) { }

		/// <summary>
		/// 初始化 TableDesignerInfo 类实例。
		/// </summary>
		/// <param name="tables">需要通知 AbstractCustomTypeDescriptor 类实例当前类的属性已更改。</param>
		/// <param name="aliasName"></param>
		/// <param name="mainTable"></param>
		internal TableDesignerInfo(TableDesignerCollection tables, string aliasName, bool mainTable)
		{
			_MainTable = mainTable;
			_Alias = aliasName;
			_Tables = tables;
			_Columns = new ColumnDesignerCollection(this);
		}

		/// <summary>
		/// 拥有此表对象的集合。
		/// </summary>
		public TableDesignerCollection Tables { get { return _Tables; } }

		/// <summary>
		/// 将 TableInfoElement 对象的内容复制到当前实例。
		/// </summary>
		/// <param name="tableInfo">标识需要复制的 TableInfoElement 类实例。</param>
		public void CopyFrom(DesignTableInfo tableInfo)
		{
			if (string.IsNullOrWhiteSpace(_Alias))
				_Alias = tableInfo.TableName;
			_Name = tableInfo.TableName;
			_Common = tableInfo.Description;
			_Owner = tableInfo.Owner;
			_Columns.Clear();
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				ColumnDesignerInfo columnInfo = _Columns.CreateColumn();
				columnInfo.CopyFrom(column);
				_Columns.Add(columnInfo);
			}
		}

		/// <summary>
		/// 将 TableInfoElement 对象的内容复制到当前实例。
		/// </summary>
		/// <param name="tableInfo">标识需要复制的 TableInfoElement 类实例。</param>
		public void CopyFrom(TableDesignerInfo tableInfo)
		{
			if (string.IsNullOrWhiteSpace(_Alias))
				_Alias = tableInfo.Alias;
			_MainTable = tableInfo.MainTable;
			_Name = tableInfo.Name;
			_Common = tableInfo.Common;
			_Owner = tableInfo.Owner;
			_Columns.Clear();
			foreach (ColumnDesignerInfo column in tableInfo.Columns)
			{
				ColumnDesignerInfo columnInfo = _Columns.CreateColumn();
				columnInfo.CopyFrom(column);
				_Columns.Add(columnInfo);
			}
		}

		/// <summary>
		/// 数据库列集合
		/// </summary>
		public ColumnDesignerCollection Columns { get { return _Columns; } }

		private bool _MainTable = false;
		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnChecked")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue(false)]
		public bool MainTable
		{
			get { return _MainTable; }
		}

		private bool _Selected = false;
		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnChecked")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Selected
		{
			get { return _Selected; }
			set
			{
				if (_Selected != value)
				{
					_Selected = value;
					OnPropertyChanged("Selected");
				}
			}
		}

		private int _ObjectId = -1;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[System.ComponentModel.Browsable(false)]
		public int ObjectId
		{
			get { return _ObjectId; }
			internal set { if (_ObjectId != value) { _ObjectId = value; OnPropertyChanged("ObjectId"); } }
		}

		private ObjectTypeEnum _ObjectType = ObjectTypeEnum.UserTable;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[System.ComponentModel.Browsable(false)]
		public ObjectTypeEnum ObjectType
		{
			get { return _ObjectType; }
			internal set { if (_ObjectType != value) { _ObjectType = value; OnPropertyChanged("ObjectType"); } }
		}

		private string _Alias = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		public string Alias
		{
			get { return _Alias; }
			internal set { if (_Alias != value) { _Alias = value; OnPropertyChanged("Alias"); } }
		}

		private string _Owner = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[System.ComponentModel.Browsable(false)]
		public string Owner
		{
			get { return _Owner; }
			internal set { if (_Owner != value) { _Owner = value; OnPropertyChanged("Owner"); } }
		}

		private string _Name = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_TableName")]
		public string Name
		{
			get { return _Name; }
			internal set
			{
				if (_Name != value)
				{
					if (string.IsNullOrWhiteSpace(_Alias)) { Alias = value; }
					_Name = value; OnPropertyChanged("Name");
				}
			}
		}

		/// <summary>
		/// 获取或设置数据库表对象在FROM子句中显示的名称。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_TableName")]
		public virtual string FromName
		{
			get
			{
				if (_Alias == _Name)
					return _Name;
				return string.Concat(_Name, " ", _Alias);
			}
		}

		private string _Common = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_Description")]
		public string Common
		{
			get { return _Common; }
			set { if (_Common != value) { _Common = value; OnPropertyChanged("Common"); } }
		}

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return _Name; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return typeof(TableDesignerInfo).Name; }
	}
}
