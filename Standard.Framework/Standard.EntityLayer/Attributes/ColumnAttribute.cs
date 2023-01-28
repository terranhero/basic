using Basic.Enums;
using System;

namespace Basic.EntityLayer
{
    /// <summary>
    /// 表示数据库列信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置字符类型数据列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="nullable">是否允许为空</param>
        public ColumnAttribute(string name, DataTypeEnum dataType, bool nullable)
            : this(name, false, dataType, 0, 0, 0, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置字符类型数据列
        /// </summary>
        /// <param name="tabName">数据库表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="nullable">是否允许为空</param>
        public ColumnAttribute(string tabName, string name, DataTypeEnum dataType, bool nullable)
            : this(tabName, name, false, dataType, 0, 0, 0, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置字符类型数据列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据库字段长度(decimal类型的精度)</param>
        /// <param name="nullable">是否允许为空</param>
        public ColumnAttribute(string name, DataTypeEnum dataType, int size, bool nullable)
            : this(name, false, dataType, size, 0, 0, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置字符类型数据列
        /// </summary>
        /// <param name="tabName">数据库表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据库字段长度(decimal类型的精度)</param>
        /// <param name="nullable">是否允许为空</param>
        public ColumnAttribute(string tabName, string name, DataTypeEnum dataType, int size, bool nullable)
            : this(tabName, name, false, dataType, size, 0, 0, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置Decimal类型数据列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="precision">数据库字段长度(decimal类型的精度)</param>
        /// <param name="scale">数据库字段的小数位数</param>
        /// <param name="nullable">是否允许为空</param>
        public ColumnAttribute(string name, DataTypeEnum dataType, byte precision, byte scale, bool nullable)
            : this(name, false, dataType, 0, precision, scale, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例, 设置Decimal类型数据列
        /// </summary>
        /// <param name="tabName">数据库表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="precision">数据库字段长度(decimal类型的精度)</param>
        /// <param name="scale">数据库字段的小数位数</param>
        /// <param name="nullable">是否允许为空</param>
        public ColumnAttribute(string tabName, string name, DataTypeEnum dataType, byte precision, byte scale, bool nullable)
            : this(tabName, name, false, dataType, 0, precision, scale, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例，提供整型和GUID类型的关键字列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="primaryKey">是否为表关键字</param>
        public ColumnAttribute(string name, bool primaryKey, DataTypeEnum dataType)
            : this(name, primaryKey, dataType, 0, 0, 0, false) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例，提供整型和GUID类型的关键字列
        /// </summary>
        /// <param name="tabName">数据库表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="primaryKey">是否为表关键字</param>
        public ColumnAttribute(string tabName, string name, bool primaryKey, DataTypeEnum dataType)
            : this(tabName, name, primaryKey, dataType, 0, 0, 0, false) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例，提供字符类型和Decimal类型的关键字列
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据库字段长度</param>
        /// <param name="primaryKey">是否为表关键字</param>
        public ColumnAttribute(string name, bool primaryKey, DataTypeEnum dataType, int size)
            : this(null, name, primaryKey, dataType, size, 0, 0, false) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例，提供字符类型和Decimal类型的关键字列
        /// </summary>
        /// <param name="tabName">数据库表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="primaryKey">是否为表关键字</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据库字段长度</param>
        public ColumnAttribute(string tabName, string name, bool primaryKey, DataTypeEnum dataType, int size)
            : this(tabName, name, primaryKey, dataType, size, 0, 0, false) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例
        /// </summary>
        /// <param name="name">数据库字段名称</param>
        /// <param name="primaryKey">是否为表关键字</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据的最大大小，以字节为单位。</param>
        /// <param name="precision">表示 Value 属性的最大位数。</param>
        /// <param name="scale">数据库字段的小数位数</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnAttribute(string name, bool primaryKey, DataTypeEnum dataType, int size, byte precision, byte scale, bool nullable)
            : this(null, name, primaryKey, dataType, size, precision, scale, nullable) { }

        /// <summary>
        /// 初始化ColumnAttribute类实例
        /// </summary>
        /// <param name="tabName">数据库表名称或别名</param>
        /// <param name="name">数据库字段名称</param>
        /// <param name="primaryKey">是否为表关键字</param>
        /// <param name="dataType">数据库字段类型</param>
        /// <param name="size">数据的最大大小，以字节为单位。</param>
        /// <param name="precision">表示 Value 属性的最大位数。</param>
        /// <param name="scale">数据库字段的小数位数</param>
        /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
        public ColumnAttribute(string tabName, string name, bool primaryKey, DataTypeEnum dataType, int size, byte precision, byte scale, bool nullable)
        {
            TableName = tabName;
            ColumnName = name;
            PrimaryKey = primaryKey;
            DataType = dataType;
            Size = size;
            Precision = precision;
            Scale = scale;
            Nullable = nullable;
        }
        /// <summary>
        /// 数据库列所属表名称
        /// </summary>
        public string TableName { get; private set; }
        /// <summary>
        /// 数据库列名称
        /// </summary>
        public string ColumnName { get; private set; }
        /// <summary>
        /// 是否为主键列
        /// </summary>
        public bool PrimaryKey { get; private set; }
        /// <summary>
        /// 是否是否允许为空
        /// </summary>
        public bool Nullable { get; private set; }
        /// <summary>
        /// 数据库列类型
        /// </summary>
        public DataTypeEnum DataType { get; private set; }
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
