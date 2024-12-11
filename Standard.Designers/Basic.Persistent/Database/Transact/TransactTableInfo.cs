using Basic.Collections;
using Basic.Designer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Database
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class TransactTableInfo : AbstractNotifyChangedDescriptor
	{
		private readonly TransactSqlResult _Tables;
		private readonly TransactColumnCollection _Columns;
		/// <summary>
		/// 初始化 TransactTableInfo 类实例。
		/// </summary>
		internal TransactTableInfo(TransactSqlResult tables, string tableName) : this(tables, tableName, tableName) { }

		/// <summary>
		/// 初始化 TransactTableInfo 类实例。
		/// </summary>
		internal TransactTableInfo(TransactSqlResult tables, string tableName, string aliasName)
		{
			_Name = tableName;
			_Alias = aliasName;
			SetObjectName(_Name);
			_Tables = tables;
			_Columns = new TransactColumnCollection(tables);
			_Columns.CollectionChanged += _Columns_CollectionChanged;
		}

		private void _Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (TransactColumnInfo column in e.OldItems)
				{
					_Tables.RemoveColumn(column);
				}
			}
		}

		/// <summary>
		/// 添加表和表别名对应项
		/// </summary>
		/// <param name="columnName">一个string类型的值，该值表示数据库表的列名称。</param>
		/// <exception cref="System.ArgumentNullException">tableName 为 null</exception>
		/// <exception cref="System.ArgumentException">已经存在数据库表名称。</exception>
		public TransactColumnInfo AddColumn(string columnName, string sourceName)
		{
			TransactColumnInfo column = new TransactColumnInfo(this);
			column.Name = columnName;
			column.Source = sourceName;
			_Columns.Add(column);
			return column;
		}

		/// <summary>
		/// 添加表和表别名对应项
		/// </summary>
		/// <param name="columnName">一个string类型的值，该值表示数据库表的列名称。</param>
		/// <exception cref="System.ArgumentNullException">tableName 为 null</exception>
		/// <exception cref="System.ArgumentException">已经存在数据库表名称。</exception>
		public TransactColumnInfo AddColumn(string columnName)
		{
			TransactColumnInfo column = new TransactColumnInfo(this);
			column.Name = columnName;
			_Columns.Add(column);
			return column;
		}

		private string _Name = null;
		private string _ObjectName = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		public string Name
		{
			get { return _Name; }
			internal set
			{
				if (_Name != value)
				{
					if (string.IsNullOrWhiteSpace(_Alias)) { Alias = value; }
					_Name = value; OnPropertyChanged("Name");
					SetObjectName(_Name);
				}
			}
		}

		private void SetObjectName(string objectName)
		{
			if (string.IsNullOrWhiteSpace(objectName)) { return; }
			if (objectName.IndexOf('(') >= 0)
				_ObjectName = objectName.Substring(0, objectName.IndexOf('('));
			else
				_ObjectName = objectName;
			OnPropertyChanged("ObjectName");
		}

		/// <summary>
		/// 数据库表/视图/函数名称
		/// </summary>
		public string ObjectName { get { return _ObjectName; } }

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
	}
}
