using System;
using Basic.Enums;

namespace Basic.EntityLayer
{
	/// <summary>表示数据库 JOIN 连接操作所需参数</summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public sealed class JoinParameterAttribute : Attribute
	{
		/// <summary>初始化 JoinParameterAttribute 类实例</summary>
		/// <param name="whereClause">当前字段所属表名称或别名</param>
		/// <param name="fieldName">参数名称</param>
		/// <param name="dataType">数据库字段类型</param>
		public JoinParameterAttribute(string whereClause, string fieldName, DbTypeEnum dataType)
			  : this(whereClause, fieldName, dataType, 0) { }

		/// <summary>初始化 JoinParameterAttribute 类实例</summary>
		/// <param name="whereClause">当前字段所属表名称或别名</param>
		/// <param name="fieldName">数据库字段原名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		public JoinParameterAttribute(string whereClause, string fieldName, DbTypeEnum dataType, int size)
		{
			WhereClause = whereClause;
			FieldName = fieldName;
			DataType = dataType;
			Size = size;
		}

		/// <summary>
		/// 表示where子句
		/// </summary>
		public string WhereClause { get; private set; }

		/// <summary>
		/// 数据库列名称
		/// </summary>
		public string FieldName { get; private set; }

		/// <summary>
		/// 数据库列类型
		/// </summary>
		public DbTypeEnum DataType { get; private set; }

		/// <summary>
		/// 数据库字段长度
		/// </summary>
		public int Size { get; private set; }
	}
}
