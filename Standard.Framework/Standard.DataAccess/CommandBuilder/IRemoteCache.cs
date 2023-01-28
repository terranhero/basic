using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.DataAccess
{
	/// <summary>
	/// 为远程对象提供接口实现
	/// </summary>
	public interface IRemoteCache
	{
		///// <summary>
		///// 根据数据库命令名称获取数据库执行命令
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <param name="sqlStructName">命令名称</param>
		///// <returns>返回数据库命令</returns>
		//SqlStruct GetSqlStruct(string tableName, string sqlStructName);

		///// <summary>
		///// 根据数据库命令名称获取数据库执行命令
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <param name="sqlStructName">命令名称</param>
		///// <returns>返回数据库命令</returns>
		//SqlStruct GetSqlStruct(int tableKey, string sqlStructName);

		///// <summary>
		///// 根据数据库命令关键字获取数据库执行命令
		///// </summary>
		///// <param name="CmdKey">命令关键字</param>
		///// <returns>返回数据库命令</returns>
		//SqlStruct GetSqlStruct(int CmdKey);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct NewKeySqlStruct(string tableName);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct NewKeySqlStruct(int tableKey);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct AddNewSqlStruct(string tableName);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct AddNewSqlStruct(int tableKey);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct UpdateSqlStruct(string tableName);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct UpdateSqlStruct(int tableKey);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct DeleteSqlStruct(string tableName);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct DeleteSqlStruct(int tableKey);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct DeleteByFKeySqlStruct(string tableName);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct DeleteByFKeySqlStruct(int tableKey);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct SelectByKeySqlStruct(string tableName);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct SelectByKeySqlStruct(int tableKey);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct SelectByFKeySqlStruct(string tableName);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct SelectByFKeySqlStruct(int tableKey);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableName">数据库基表名称</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct SelectAllSqlStruct(string tableName);

		///// <summary>
		///// 根据命令名称创建数据库语句执行体
		///// </summary>
		///// <param name="tableKey">数据库基表关键字</param>
		///// <returns>返回创建成功的数据库命令</returns>
		//SqlStruct SelectAllSqlStruct(int tableKey);
	}
}
