using System;

using Basic.Collections;
using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>
	/// ISqlStructBuilder接口的默认实现
	/// </summary>
	public class DataBaseStructBuilder : CommandBuilder
	{
		#region 构造函数
		/// <summary>
		/// 数据库表名称
		/// </summary>
		private readonly string TableName;

		/// <summary>
		/// 数据库表关键字
		/// </summary>
		private readonly int TableKey;

		/// <summary>
		/// 创建DataBaseStructBuilder实例
		/// </summary>
		/// <param name="tabkeKey">数据库表的关键字</param>
		public DataBaseStructBuilder(int tabkeKey) { TableKey = tabkeKey; }

		/// <summary>
		/// 创建DataBaseStructBuilder实例
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		public DataBaseStructBuilder(string tableName) { TableName = tableName; }
		#endregion

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		public override void Dispose()
		{
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="tableCache"></param>
		//public override void CreateDataCommand(DatabaseConfiguration tableCache)
		//{
		//}


		///// <summary>
		///// 根据命令名称创建数据库命令
		///// </summary>
		///// <param name="dbCommands">当前配置文件的缓存</param>
		///// <param name="configFileName">配置文件名称</param>
		//public override void CreateDataCommand(DatabaseCommands dbCommands, string configFileName)
		//{
		//}

		/// <summary>
		/// 根据命令名称创建数据库命令
		/// </summary>
		/// <param name="tableInfo">当前配置文件的缓存</param>
		internal override void CreateDataCommand(TableConfiguration tableInfo)
		{
		}
	}
}
