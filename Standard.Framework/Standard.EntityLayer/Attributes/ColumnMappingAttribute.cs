using Basic.Enums;
using System;

namespace Basic.EntityLayer
{
    /// <summary>
    /// 表示数据库列信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ColumnMappingAttribute : Attribute
    {
        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置字符类型数据列
        /// </summary>
        /// <param name="tableAlias">当前字段所属表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="nullable">是否允许为空</param>
        public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, bool nullable)
			 : this(tableAlias, name, null, dataType, 0, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置字符类型数据列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="nullable">是否允许为空</param>
        public ColumnMappingAttribute(string name, DbTypeEnum dataType, bool nullable)
			  : this(null, name, null, dataType, 0, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例，提供字符类型和Decimal类型的关键字列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据库字段长度</param>
        public ColumnMappingAttribute(string name, DbTypeEnum dataType, int size)
			  : this(null, name, null, dataType, size, false) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据的最大大小，以字节为单位。</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnMappingAttribute(string name, DbTypeEnum dataType, int size, bool nullable)
			  : this(null, name, null, dataType, size, false) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例
        /// </summary>
        /// <param name="tableAlias">当前字段所属表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据的最大大小，以字节为单位。</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable)
            : this(tableAlias, name, null, dataType, size, nullable) { }

        /// <summary>
        /// 初始化ColumnMappingAttribute类实例
        /// </summary>
        /// <param name="tableAlias">当前字段所属表名称或别名</param>
        /// <param name="name">数据库字段返回名称</param>
        /// <param name="source">数据库字段原名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnMappingAttribute(string tableAlias, string name, string source, DbTypeEnum dataType, bool nullable)
			  : this(tableAlias, name, source, dataType, 0, nullable) { }

        /// <summary>
        /// 初始化ColumnMappingAttribute类实例
        /// </summary>
        /// <param name="tableAlias">当前字段所属表名称或别名</param>
        /// <param name="name">数据库字段返回名称</param>
        /// <param name="source">数据库字段原名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据的最大大小，以字节为单位。</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnMappingAttribute(string tableAlias, string name, string source, DbTypeEnum dataType, int size, bool nullable)
        {
            TableAlias = tableAlias;
            ColumnName = name;
            SourceColumn = source ?? name;
            DataType = dataType;
            Size = size;
            Precision = 0;
            Scale = 0;
            Nullable = nullable;
        }

        /// <summary>
        /// 初始化ColumnMappingAttribute类实例, 设置Decimal类型数据列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="precision">数据库字段长度(decimal类型的精度)</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnMappingAttribute(string name, DbTypeEnum dataType, byte precision, bool nullable)
			  : this(null, name, null, dataType, precision, 0, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置Decimal类型数据列
        /// </summary>
        /// <param name="tableAlias">当前字段所属表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="precision">数据库字段长度(decimal类型的精度)</param>
        /// <param name="scale">数据库字段的小数位数</param>
        public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale)
			  : this(tableAlias, name, null, dataType, precision, scale, false) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置Decimal类型数据列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="precision">数据库字段长度(decimal类型的精度)</param>
        /// <param name="scale">数据库字段的小数位数</param>
        public ColumnMappingAttribute(string name, DbTypeEnum dataType, byte precision, byte scale)
			  : this(null, name, null, dataType, precision, scale, false) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="precision">表示 Value 属性的最大位数。</param>
        /// <param name="scale">数据库字段的小数位数</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnMappingAttribute(string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
			  : this(null, name, null, dataType, precision, scale, false) { }

        /// <summary>
        /// 初始化 ColumnMappingAttribute 类实例
        /// </summary>
        /// <param name="tableAlias">当前字段所属表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="precision">表示 Value 属性的最大位数。</param>
        /// <param name="scale">数据库字段的小数位数</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
            : this(tableAlias, name, null, dataType, precision, scale, nullable) { }

        /// <summary>
        /// 初始化 ColumnMappingAttribute 类实例
        /// </summary>
        /// <param name="tableAlias">当前字段所属表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="source">数据库字段原名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="precision">表示 Value 属性的最大位数。</param>
        /// <param name="scale">数据库字段的小数位数</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnMappingAttribute(string tableAlias, string name, string source, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
        {
            TableAlias = tableAlias;
            ColumnName = name;
            SourceColumn = source ?? name;
            DataType = dataType;
            Size = 0;
            Precision = precision;
            Scale = scale;
            Nullable = nullable;
        }

        /// <summary>
        /// 数据库表名称或别名
        /// </summary>
        public string TableAlias { get; private set; }

        /// <summary>
        /// 数据库列名称
        /// </summary>
        public string ColumnName { get; private set; }

        private string _SourceColumn = null;
        /// <summary>
        /// 数据库列名称
        /// </summary>
        public string SourceColumn
        {
            get { return string.IsNullOrEmpty(_SourceColumn) ? ColumnName : _SourceColumn; }
            set { _SourceColumn = value; }
        }

        /// <summary>
        /// 是否是否允许为空
        /// </summary>
        public bool Nullable { get; private set; }

        /// <summary>
        /// 数据库列类型
        /// </summary>
        public DbTypeEnum DataType { get; private set; }

        /// <summary>
        /// 数据库字段的小数位数
        /// </summary>
        public byte Scale { get; private set; }

        /// <summary>
        /// 数据库字段长度
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// 数据库字段长度
        /// </summary>
        public byte Precision { get; private set; }
    }
}
