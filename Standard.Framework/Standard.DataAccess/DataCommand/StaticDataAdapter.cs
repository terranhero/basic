using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using Basic.Enums;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示用于填充 DataSet 和更新数据库的一组数据命令和一个数据库连接。 
	/// </summary>
	[System.Serializable(), System.ComponentModel.ToolboxItem(false)]
	public abstract class StaticDataAdapter : Component, IDbDataAdapter, IDataAdapter, ICloneable
	{
		/// <summary>
		/// 帮助实现 System.Data.IDbDataAdapter 接口。
		/// System.Data.Common.DbDataAdapter 的继承者实现一组函数以提供强类型，
		/// 但是继承了完全实现 DataAdapter 所需的大部分功能。
		/// </summary>
		protected internal readonly DbDataAdapter dbDataAdapter;
		/// <summary>
		/// 初始化 StaticDataAdapter 类实例
		/// </summary>
		/// <param name="dataAdapter">用于创建新 StaticDataAdapter 的 DbDataAdapter 对象。</param>
		protected StaticDataAdapter(DbDataAdapter dataAdapter) { dbDataAdapter = dataAdapter; }
	
		/// <summary>当前命令的数据库类型</summary>
		public abstract ConnectionType ConnectionType { get; }

		#region 接口 IDataAdapter 的显式实现
		int IDataAdapter.Fill(DataSet dataSet)
		{
			return dbDataAdapter.Fill(dataSet);
		}

		DataTable[] IDataAdapter.FillSchema(DataSet dataSet, SchemaType schemaType)
		{
			return dbDataAdapter.FillSchema(dataSet, schemaType);
		}

		IDataParameter[] IDataAdapter.GetFillParameters()
		{
			return dbDataAdapter.GetFillParameters();
		}

		MissingMappingAction IDataAdapter.MissingMappingAction
		{
			get { return dbDataAdapter.MissingMappingAction; }
			set { dbDataAdapter.MissingMappingAction = value; }
		}

		MissingSchemaAction IDataAdapter.MissingSchemaAction
		{
			get { return dbDataAdapter.MissingSchemaAction; }
			set { dbDataAdapter.MissingSchemaAction = value; }
		}

		ITableMappingCollection IDataAdapter.TableMappings
		{
			get { return dbDataAdapter.TableMappings; }
		}

		int IDataAdapter.Update(DataSet dataSet)
		{
			return dbDataAdapter.Update(dataSet);
		}
		#endregion

		#region 接口 ICloneable 的显式实现
		object ICloneable.Clone()
		{
			return this;
		}
		#endregion

		#region 接口 IDbDataAdapter 的显式实现
		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get { return dbDataAdapter.DeleteCommand; }
			set { dbDataAdapter.DeleteCommand = value as DbCommand; }
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get { return dbDataAdapter.InsertCommand; }
			set { dbDataAdapter.InsertCommand = value as DbCommand; }
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get { return dbDataAdapter.SelectCommand; }
			set { dbDataAdapter.SelectCommand = value as DbCommand; }
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get { return dbDataAdapter.UpdateCommand; }
			set { dbDataAdapter.UpdateCommand = value as DbCommand; }
		}
		#endregion
	}
}
