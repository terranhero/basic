using Basic.Enums;
using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示数据库列信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class JoinFieldAttribute : Attribute
	{
		/// <summary>
		/// 初始化 JoinFieldAttribute 类实例, 设置字符类型数据列
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="fieldName">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		public JoinFieldAttribute(string tableAlias, string fieldName, DbTypeEnum dataType)
			 : this(tableAlias, fieldName, fieldName, dataType, 0, 0, 0) { }

		/// <summary>
		/// 初始化 JoinFieldAttribute 类实例, 设置字符类型数据列
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="fieldName">数据库字段名称</param>
		/// <param name="fieldAlias">数据库字段原名称</param>
		/// <param name="dataType">数据库字段类型</param>
		public JoinFieldAttribute(string tableAlias, string fieldName, string fieldAlias, DbTypeEnum dataType)
			 : this(tableAlias, fieldName, fieldAlias, dataType, 0, 0, 0) { }

		/// <summary>
		/// 初始化JoinFieldAttribute类实例
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="fieldName">数据库字段返回名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		public JoinFieldAttribute(string tableAlias, string fieldName, DbTypeEnum dataType, int size)
		 : this(tableAlias, fieldName, fieldName, dataType, size, 0, 0) { }

		/// <summary>
		/// 初始化JoinFieldAttribute类实例
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="fieldName">数据库字段返回名称</param>
		/// <param name="fieldAlias">数据库字段原名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		public JoinFieldAttribute(string tableAlias, string fieldName, string fieldAlias, DbTypeEnum dataType, int size)
		 : this(tableAlias, fieldName, fieldAlias, dataType, size, 0, 0) { }

		/// <summary>
		/// 初始化JoinFieldAttribute类实例, 设置Decimal类型数据列
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="fieldName">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">数据库字段长度(decimal类型的精度)</param>
		/// <param name="scale">数据库字段的小数位数</param>
		public JoinFieldAttribute(string tableAlias, string fieldName, DbTypeEnum dataType, byte precision, byte scale)
			  : this(tableAlias, fieldName, fieldName, dataType, 0, precision, scale) { }

		/// <summary>
		/// 初始化 JoinFieldAttribute 类实例
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="fieldName">数据库字段名称</param>
		/// <param name="fieldAlias">数据库字段原名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="precision">表示 Value 属性的最大位数。</param>
		/// <param name="scale">数据库字段的小数位数</param>
		public JoinFieldAttribute(string tableAlias, string fieldName, string fieldAlias, DbTypeEnum dataType, int size, byte precision, byte scale)
		{
			if (string.IsNullOrWhiteSpace(fieldName)) { throw new ArgumentNullException(nameof(fieldName)); }
			TableAlias = tableAlias;
			FieldName = fieldName;
			FieldAlias = fieldAlias ?? fieldName;
			DataType = dataType;
			Size = size;
			Precision = precision;
			Scale = scale;
		}

		/// <summary>数据库表名称或别名</summary>
		public string TableAlias { get; private set; }

		/// <summary>数据库列名称</summary>
		public string FieldName { get; private set; }

		/// <summary>列别名</summary>
		public string FieldAlias { get; private set; }

		/// <summary>数据库列类型</summary>
		public DbTypeEnum DataType { get; private set; }

		/// <summary>数据库字段的小数位数</summary>
		public byte Scale { get; private set; }

		/// <summary>数据库字段长度</summary>
		public int Size { get; private set; }

		/// <summary>数据库字段长度</summary>
		public byte Precision { get; private set; }

		/// <summary>根据参数获取JOIN操作SQL语句</summary>
		/// <returns></returns>
		internal string Script
		{
			get
			{
				if (FieldName == FieldAlias) { return string.Concat(TableAlias, ".", FieldName); }
				else { return string.Concat(TableAlias, ".", FieldName, " AS ", FieldAlias); }
			}
		}
	}
}
