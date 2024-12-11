using System.Collections.Generic;
using Basic.Collections;
using Basic.Database;
using Basic.Enums;
using Oracle.ManagedDataAccess.Client;

namespace Basic.DataContexts
{
    /// <summary>
    /// ORACLE 数据上下文接口
    /// </summary>
    public sealed class OracleDataContext : IDataContext
    {
        private readonly OracleCommand oracleCommand;
        private readonly OracleConnection oracleConnection;
        internal OracleDataContext(string connectionString)
        {
            oracleConnection = new OracleConnection(connectionString);
            oracleConnection.Open();
            oracleCommand = new OracleCommand(string.Empty, oracleConnection);
        }

		/// <summary>获取特定数据库参数带符号的名称</summary>
		/// <param name="parameterName">不带参数符号的参数名称</param>
		/// <returns>返回特定数据库参数带符号的名称。</returns>
		public string GetParameterName(string parameterName) { return string.Concat("@", parameterName); }

		/// <summary>
		/// 获取数据库系统中所有表类型的对象（含Table、View、 Table Function）
		/// </summary>
		/// <param name="tables">需要填充的数据表集合，</param>
		/// <param name="objectType">需要填充的表对象类型</param>
		public void GetTableObjects(TableDesignerCollection tables, ObjectTypeEnum objectType)
        {
        }

        /// <summary>
        /// 获取数据库中所有用户表
        /// </summary>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public DesignTableInfo[] GetTables(ObjectTypeEnum objectType)
        {
            return null;
        }

        /// <summary>
        /// 获取表或视图的列信息
        /// </summary>
        /// <param name="tableInfo">表或视图名称。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public void GetColumns(DesignTableInfo tableInfo)
        {
        }

        /// <summary>
        /// 获取表或视图的列信息
        /// </summary>
        /// <param name="tableInfo">表或视图名称。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public void GetColumns(TableDesignerInfo tableInfo)
        {
        }

        /// <summary>
        /// 获取表或视图的列信息
        /// </summary>
        /// <param name="tableInfo">表或视图名称。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public void GetParameters(TableFunctionInfo tableInfo) { }

        /// <summary>
        /// 获取数据库中所有存储过程
        /// </summary>
        /// <returns>获取获取的存储过程列表。</returns>
        public StoreProcedure[] GetProcedures() { return null; }

        /// <summary>
        /// 获取存储过程参数列表
        /// </summary>
        /// <param name="procedure">存储过程信息</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public bool GetParameters(StoreProcedure procedure) { return true; }

        /// <summary>
        /// 获取存储过程返回结果列信息
        /// </summary>
        /// <param name="procedure">存储过程信息</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public bool GetColumns(StoreProcedure procedure)
        {
            return true;
        }

        /// <summary>
        /// 获取命令文本中返回结果的列信息。
        /// </summary>
        /// <param name="columns">待填充 DataTable 实例。</param>
        /// <param name="trancateSql">需要查询的 Trancate-Sql 实例。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public bool GetTransactSql(IList<DesignColumnInfo> columns, string trancateSql)
        {
            return true;
        }

        /// <summary>
        /// 根据自定义 Trancate-SQL 查询表结构信息。
        /// </summary>
        /// <param name="tableCollection">查询表结构定义</param>
        /// <param name="trancateSql">需要查询的 Trancate-Sql 实例。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public bool GetTransactSql(TransactSqlResult tableCollection, string trancateSql)
        {
            return true;
        }

        /// <summary>
        /// 获取函数的参数信息
        /// </summary>
        /// <param name="tableCollection">查询表结构定义。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        public void GetParameters(TransactSqlResult tableCollection)
        {

        }

        /// <summary>
        /// 释放数据库关键字
        /// </summary>
        public void Dispose()
        {
            oracleConnection.Close();
            oracleConnection.Dispose();
            oracleCommand.Dispose();
        }
    }
}
