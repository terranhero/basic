using System.Collections.Generic;
using Basic.Database;
using Basic.Enums;
using Basic.Collections;

namespace Basic.DataContexts
{
    /// <summary>
    /// 数据上下文接口
    /// </summary>
    public interface IDataContext : System.IDisposable
    {
        /// <summary>
        /// 获取数据库系统中所有表类型的对象（含Table、View、 Table Function）
        /// </summary>
        /// <param name="tables">需要填充的数据表集合，</param>
        /// <param name="objectType">需要填充的表对象类型</param>
        void GetTableObjects(TableDesignerCollection tables, ObjectTypeEnum objectType);

        /// <summary>
        /// 获取数据库中所有用户表
        /// </summary>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        DesignTableInfo[] GetTables(ObjectTypeEnum objectType);

        /// <summary>
        /// 获取表或视图的列信息
        /// </summary>
        /// <param name="tableInfo">表或视图名称。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        void GetColumns(DesignTableInfo tableInfo);

        /// <summary>
        /// 获取表或视图的列信息
        /// </summary>
        /// <param name="tableInfo">表或视图名称。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        void GetParameters(TableFunctionInfo tableInfo);

        /// <summary>
        /// 获取数据库中所有存储过程
        /// </summary>
        /// <returns>获取获取的存储过程列表。</returns>
        StoreProcedure[] GetProcedures();

        /// <summary>
        /// 获取存储过程参数列表
        /// </summary>
        /// <param name="procedure">存储过程信息</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        bool GetParameters(StoreProcedure procedure);

        /// <summary>
        /// 获取存储过程返回结果列信息
        /// </summary>
        /// <param name="procedure">存储过程信息</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        bool GetColumns(StoreProcedure procedure);

        /// <summary>
        /// 根据自定义 Trancate-SQL 查询表结构信息。
        /// </summary>
        /// <param name="tableCollection">查询表结构定义</param>
        /// <param name="trancateSql">需要查询的 Trancate-Sql 实例。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        bool GetTransactSql(TransactTableCollection tableCollection, string trancateSql);

        /// <summary>
        /// 获取函数的参数信息
        /// </summary>
        /// <param name="tableCollection">查询表结构定义。</param>
        /// <returns>如果获取数据成功则返回True，否则返回False。</returns>
        void GetParameters(TransactTableCollection tableCollection);
    }
}
