using Basic.Database;
using Basic.Properties;

namespace Basic.Collections
{
	/// <summary>
	/// 表示数据库表结构定义的集合
	/// </summary>
	public sealed class TableDesignerCollection : BaseCollection<TableDesignerInfo>
	{
		private readonly CheckedColumnCollection _CheckedColumns;
		private readonly RelationDesignerCollection _TableRelations;
		/// <summary>
		/// 初始化 TableDesignerCollection 类的新实例。
		/// </summary>
		internal TableDesignerCollection(string resourcesName)
			: base()
		{
			_Name = DesignerStrings.ResourceManager.GetString(resourcesName);
			_CheckedColumns = new CheckedColumnCollection(this);
			_TableRelations = new RelationDesignerCollection(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			base.OnPropertyChanged("ShowFrom");
		}

		/// <summary>
		/// 添加关系
		/// </summary>
		/// <param name="parentColumn"></param>
		/// <param name="childColumn"></param>
		public void AddRelation(ColumnDesignerInfo parentColumn, ColumnDesignerInfo childColumn)
		{
			foreach (RelationDesignerInfo relation in _TableRelations)
			{
				if (relation.Parent == parentColumn.Table && relation.Child == childColumn.Table)
				{
					relation.Add(parentColumn, childColumn); return;
				}
			}
			RelationDesignerInfo relation1 = new RelationDesignerInfo(_TableRelations, parentColumn.Table, childColumn.Table);
			_TableRelations.Add(relation1);
			relation1.Add(parentColumn, childColumn);
		}

		/// <summary>
		/// 
		/// </summary>
		public CheckedColumnCollection CheckedColumns { get { return _CheckedColumns; } }

		/// <summary>
		/// 
		/// </summary>
		public RelationDesignerCollection Relations { get { return _TableRelations; } }

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(TableDesignerInfo item) { return item.Name; }

		#region Checked ,IsWhere 事件
		/// <summary>
		/// 表示数据列中 Checked 属性已更改的事件。
		/// </summary>
		public event ColumnCheckedHandler CheckedChanged;
		/// <summary>
		/// 引发 CheckedChanged 事件的受保护方法。
		/// </summary>
		/// <param name="column">引发事件的 ColumnDesignerInfo 类实例。</param>
		internal void OnCheckedChanged(ColumnDesignerInfo column)
		{
			if (column.Checked)
				_CheckedColumns.Add(column);
			else
				_CheckedColumns.Remove(column);
			if (CheckedChanged != null)
				CheckedChanged(this, new ColumnCheckedEventArgs(column));
		}

		/// <summary>
		/// 表示数据列中 Checked 属性已更改的事件。
		/// </summary>
		public event ColumnChangedHandler ColumnChanged;
		/// <summary>
		/// 引发 CheckedChanged 事件的受保护方法。
		/// </summary>
		/// <param name="column">引发事件的 ColumnDesignerInfo 类实例。</param>
		internal void OnColumnChanged(ColumnDesignerInfo column)
		{
			if (ColumnChanged != null)
				ColumnChanged(this, new ColumnChangedEventArgs(column));
		}

		/// <summary>
		/// 表示数据列中 RelationDesignerInfo 属性已更改的事件。
		/// </summary>
		public event RelationChangedHandler RelationChanged;
		/// <summary>
		/// 引发 RelationChanged 事件的受保护方法。
		/// </summary>
		/// <param name="relationInfo">引发事件的 RelationDesignerInfo 类实例。</param>
		internal void OnRelationChanged(RelationDesignerInfo relationInfo)
		{
			if (RelationChanged != null)
				RelationChanged(this, new RelationChangedEventArgs(relationInfo));
		}


		/// <summary>
		/// 表示数据列中 IsFrom 属性已更改的事件。
		/// </summary>
		public event ColumnCheckedHandler IsFromChanged;
		/// <summary>
		/// 引发 IsFromChanged 事件的受保护方法。
		/// </summary>
		/// <param name="column">引发事件的 ColumnDesignerInfo 类实例。</param>
		internal void OnIsFromChanged(ColumnDesignerInfo column)
		{
			if (IsFromChanged != null)
				IsFromChanged(this, new ColumnCheckedEventArgs(column));
		}

		/// <summary>
		/// 表示数据列中 IsWhere 属性已更改的事件。
		/// </summary>
		public event ColumnIsWhereChangedHandler IsWhereChanged;
		/// <summary>
		/// 引发 IsWhereChanged 事件的受保护方法。
		/// </summary>
		/// <param name="column">引发事件的 ColumnDesignerInfo 类实例。</param>
		internal void OnIsWhereChanged(ColumnDesignerInfo column)
		{
			if (IsWhereChanged != null)
				IsWhereChanged(this, new ColumnIsWhereEventArgs(column));
		}

		/// <summary>
		/// 表示数据列中 UseDefault 属性已更改的事件。
		/// </summary>
		public event ColumnUseDefaultHandler UseDefaultChanged;
		/// <summary>
		/// 引发 UseDefaultChanged 事件的受保护方法。
		/// </summary>
		/// <param name="column">引发事件的 ColumnDesignerInfo 类实例。</param>
		internal void OnUseDefaultChanged(ColumnDesignerInfo column)
		{
			if (UseDefaultChanged != null)
				UseDefaultChanged(this, new ColumnUseDefaultEventArgs(column));
		}
		#endregion

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		public TableDesignerInfo CreateTable()
		{
			return new TableDesignerInfo(this);
		}

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		public TableDesignerInfo CreateTable(string aliasName)
		{
			return new TableDesignerInfo(this, aliasName);
		}

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		public TableDesignerInfo CreateTable(string aliasName, bool mainTable)
		{
			return new TableDesignerInfo(this, aliasName, mainTable);
		}

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		public TableFunctionInfo CreateTableFunction()
		{
			return new TableFunctionInfo(this);
		}

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		public TableFunctionInfo CreateTableFunction(string aliasName)
		{
			return new TableFunctionInfo(this, aliasName);
		}

		private bool _Group = false;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		public bool Group
		{
			get { return _Group; }
			set
			{
				if (_Group != value)
				{
					_Group = value;
					base.OnPropertyChanged("Group");
				}
			}
		}

		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		public bool ShowFrom
		{
			get { return Count >= 2; }
		}

		private bool _ShowColumns = true;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		public bool ShowColumns
		{
			get { return _ShowColumns; }
			set
			{
				if (_ShowColumns != value)
				{
					_ShowColumns = value;
					base.OnPropertyChanged("ShowColumns");
				}
			}
		}

		private string _Name = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_TableName")]
		public string Name { get { return _Name; } }
	}
}
