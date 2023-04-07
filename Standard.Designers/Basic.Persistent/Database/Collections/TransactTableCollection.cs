using Basic.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Basic.Collections
{
	/// <summary>
	/// 表示 Transact-SQL 解析成功后的结果
	/// </summary>
	public sealed class TransactTableCollection : BaseCollection<TransactTableInfo>, INotifyCollectionChanged
	{
		private readonly SortedDictionary<string, string> _PropertyMapping;
		internal const string EmptyTableName = "E932C150946343158DD71D30BEF75E41";
		private readonly TransactTableInfo emptyNameTable;
		private bool m_Successful = false;
		private readonly TransactColumnCollection m_Columns;
		/// <summary>
		/// 使用是否成功解析作为参数，初始化 TransactTableCollection 类实例。
		/// </summary>
		/// <param name="successful">当前 Transact-SQL 是否解析成功，如果成功则为 True，否则为 False。</param>
		public TransactTableCollection(bool successful)
		{
			_PropertyMapping = new SortedDictionary<string, string>();
			m_Successful = successful;
			m_Columns = new TransactColumnCollection(this);
			emptyNameTable = new TransactTableInfo(this, EmptyTableName);
		}

		/// <summary>
		/// 当前设计文件已经存在的属性名称映射信息。
		/// </summary>
		public SortedDictionary<string, string> PropertyMapping { get { return _PropertyMapping; } }
		/// <summary>
		/// 数据库列集合
		/// </summary>
		public TransactColumnCollection Columns { get { return m_Columns; } }

		private string _TableName = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_TableName")]
		public string TableName
		{
			get { return _TableName; }
			internal set
			{
				if (_TableName != value)
				{
					_TableName = value;
				}
			}
		}

		private string _EntityName = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_EntityName")]
		public string EntityName
		{
			get { return _EntityName; }
			set
			{
				if (_EntityName != value)
				{
					_EntityName = value;
				}
			}
		}

		private string _Description = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_Description")]
		public string Description
		{
			get { return _Description; }
			set
			{
				if (_Description != value)
				{
					_Description = value;
				}
			}
		}

		/// <summary>
		/// 添加表和表别名对应项
		/// </summary>
		/// <param name="tableName">一个string类型的值，该值表示数据库表的名称。</param>
		/// <param name="aliasName">一个string类型的值，该值表示数据库表的别名。</param>
		/// <exception cref="System.ArgumentNullException">tableName 为 null</exception>
		/// <exception cref="System.ArgumentException">已经存在数据库表名称。</exception>
		public void AddTable(string tableName, string aliasName)
		{
			if (ContainsKey(tableName) == false)
			{
				base.Add(new TransactTableInfo(this, tableName, aliasName));
			}
		}
		/// <summary>
		/// 从 TransactColumnCollection 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="column">要从 TransactColumnCollection 中移除的对象。对于引用类型，该值可以为 null。</param>
		/// <returns>如果成功移除 item，则为 true；否则为 false。如果在原始 TransactColumnCollection中未找到 item，此方法也会返回 false。</returns>
		internal bool RemoveColumn(TransactColumnInfo column)
		{
			return m_Columns.Remove(column);
		}

		/// <summary>
		/// 添加表和表别名对应项
		/// </summary>
		/// <param name="tableName">一个string类型的值，该值表示数据库表的名称。</param>
		/// <param name="columnName">一个string类型的值，该值表示数据库表的列名称。</param>
		/// <exception cref="System.ArgumentNullException">tableName 为 null</exception>
		/// <exception cref="System.ArgumentException">已经存在数据库表名称。</exception>
		public TransactColumnInfo AddColumn(string tableName, string columnName)
		{
			TransactTableInfo tableInfo = null;
			if (base.TryGetValue(tableName, out tableInfo))
			{
				TransactColumnInfo column = tableInfo.AddColumn(columnName);
				if (_PropertyMapping.ContainsKey(columnName))
					column.PropertyName = _PropertyMapping[columnName];
				m_Columns.Add(column);
				return column;
			}
			TransactColumnInfo column1 = emptyNameTable.AddColumn(columnName);
			if (_PropertyMapping.ContainsKey(columnName))
				column1.PropertyName = _PropertyMapping[columnName];
			m_Columns.Add(column1);
			return column1;
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(TransactTableInfo item) { return item.ObjectName; }

		/// <summary>
		/// 当前 Transact-SQL 是否解析成功。
		/// </summary>
		/// <value>如果成功则为 True，否则为 False。</value>
		public bool Successful { get { return m_Successful; } set { m_Successful = value; } }

		/// <summary>
		/// Transact-SQL 语句中 WITH 部分
		/// </summary>
		public string WithText { get { return m_WithText.ToString(); } }
		/// <summary>
		/// Transact-SQL 语句中 WITH 部分
		/// </summary>
		public StringBuilder WithBuilder { get { return m_WithText; } }
		private readonly StringBuilder m_WithText = new StringBuilder(1000);

		/// <summary>
		/// Transact-SQL 语句中 SELECT 部分
		/// </summary>
		public string SelectText { get { return m_SelectBuilder.ToString(); } }

		/// <summary>
		/// Transact-SQL 语句中 SELECT 部分
		/// </summary>
		public StringBuilder SelectBuilder { get { return m_SelectBuilder; } }
		private readonly StringBuilder m_SelectBuilder = new StringBuilder(1000);

		/// <summary>
		/// Transact-SQL 语句中 FROM 部分
		/// </summary>
		public string FromText { get { return m_FromBuilder.ToString(); } }
		/// <summary>
		/// Transact-SQL 语句中 FROM 部分
		/// </summary>
		public StringBuilder FromBuilder { get { return m_FromBuilder; } }
		private readonly StringBuilder m_FromBuilder = new StringBuilder(1000);

		/// <summary>
		/// Transact-SQL 语句中 WHERE 部分
		/// </summary>
		public string WhereText { get { return m_WhereBuilder.ToString(); } }
		/// <summary>
		/// Transact-SQL 语句中 WHERE 部分
		/// </summary>
		public StringBuilder WhereBuilder { get { return m_WhereBuilder; } }
		private readonly StringBuilder m_WhereBuilder = new StringBuilder(1000);

		/// <summary>
		/// Transact-SQL 语句中 GROUP BY 部分
		/// </summary>
		public string GroupText { get { return m_GroupBuilder.ToString(); } }
		/// <summary>
		/// Transact-SQL 语句中 GROUP BY 部分
		/// </summary>
		public StringBuilder GroupBuilder { get { return m_GroupBuilder; } }
		private readonly StringBuilder m_GroupBuilder = new StringBuilder(1000);

		/// <summary>
		/// Transact-SQL 语句中 HAVING 部分
		/// </summary>
		public string HavingText { get { return m_HavingBuilder.ToString(); } }
		/// <summary>
		/// Transact-SQL 语句中 HAVING 部分
		/// </summary>
		public StringBuilder HavingBuilder { get { return m_HavingBuilder; } }
		private readonly StringBuilder m_HavingBuilder = new StringBuilder(1000);

		/// <summary>
		/// Transact-SQL 语句中 ORDER BY 部分
		/// </summary>
		public string OrderText { get { return m_OrderBuilder.ToString(); } }
		/// <summary>
		/// Transact-SQL 语句中 ORDER BY 部分
		/// </summary>
		public StringBuilder OrderBuilder { get { return m_OrderBuilder; } }
		private readonly StringBuilder m_OrderBuilder = new StringBuilder(1000);
	}
}
