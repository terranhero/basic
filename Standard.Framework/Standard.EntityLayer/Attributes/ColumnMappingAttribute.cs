using System;
using System.Data.SqlTypes;
using Basic.Enums;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示数据库列信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ColumnMappingAttribute : Attribute
	{
		#region 使用 DbTypeEnum 数据类型的构造函数，仅支持文本/Guid/Int等不需要精度的类型
		/// <summary>初始化 <see cref="ColumnMappingAttribute"/> 类的新实例（字符串类型专用构造函数）。</summary>
		/// <param name="tableAlias">表别名，用于在多表关联查询时标识字段所属的表。</param>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		/// <param name="dynamics">指示该字段是否为动态字段，动态字段可能在数据库表中不存在实际列。</param>
		public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, bool nullable, bool dynamics)
			 : this(tableAlias, name, null, dataType, 0, nullable, dynamics) { }

		/// <summary>
		/// 初始化 <see cref="ColumnMappingAttribute"/> 类的新实例（字符串类型专用构造函数）。
		/// </summary>
		/// <param name="tableAlias">表别名，用于在多表关联查询时标识字段所属的表。</param>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, bool nullable)
			 : this(tableAlias, name, null, dataType, 0, nullable, false) { }

		/// <summary>
		/// 初始化ColumnAttribute类实例, 设置字符类型数据列
		/// </summary>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="nullable">是否允许为空</param>
		public ColumnMappingAttribute(string name, DbTypeEnum dataType, bool nullable)
			  : this(null, name, null, dataType, 0, nullable, false) { }

		/// <summary>初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。</summary>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据库字段长度</param>
		public ColumnMappingAttribute(string name, DbTypeEnum dataType, int size)
			  : this(null, name, null, dataType, size, false, false) { }

		/// <summary>初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。</summary>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public ColumnMappingAttribute(string name, DbTypeEnum dataType, int size, bool nullable)
			  : this(null, name, null, dataType, size, false, false) { }

		/// <summary>初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。</summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable)
			: this(tableAlias, name, null, dataType, size, nullable, false) { }

		/// <summary>初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。</summary>
		/// <param name="tableAlias">表别名，用于在多表关联查询时标识字段所属的表。</param>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="size">字段的长度或大小。对于字符串类型表示最大字符数，对于二进制类型表示最大字节数。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		/// <param name="dynamics">指示该字段是否为动态字段，动态字段可能在数据库表中不存在实际列。</param>
		public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable, bool dynamics)
			 : this(tableAlias, name, null, dataType, 0, nullable, dynamics) { }

		/// <summary>初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。</summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段返回名称</param>
		/// <param name="source">数据库字段原名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public ColumnMappingAttribute(string tableAlias, string name, string source, DbTypeEnum dataType, bool nullable)
			  : this(tableAlias, name, source, dataType, 0, nullable, false) { }
		/// <summary>
		/// 初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。
		/// </summary>
		/// <param name="tableAlias">表别名，用于在多表关联查询时标识字段所属的表。</param>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="source">数据源名称或源表名称。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="size">字段的长度或大小。对于字符串类型表示最大字符数，对于二进制类型表示最大字节数。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		public ColumnMappingAttribute(string tableAlias, string name, string source, DbTypeEnum dataType, int size, bool nullable)
		 : this(tableAlias, name, source, dataType, 0, nullable, false) { }

		/// <summary>
		/// 初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。
		/// </summary>
		/// <param name="tableAlias">表别名，用于在多表关联查询时标识字段所属的表。</param>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="source">数据源名称或源表名称。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="size">字段的长度或大小。对于字符串类型表示最大字符数，对于二进制类型表示最大字节数。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		/// <param name="dynamics">指示该字段是否为动态字段，动态字段可能在数据库表中不存在实际列。</param>
		/// <remarks>
		/// 该构造函数适用于需要指定长度/大小的字段类型（如 <c>varchar</c>、<c>char</c>、<c>nvarchar</c>、<c>varbinary</c> 等）。
		/// <para>
		/// <list type="bullet">
		/// <item><description><paramref name="dynamics"/> 参数为 <c>true</c> 时，表示该字段不参与数据库架构验证，通常用于扩展属性或运行时附加数据。</description></item>
		/// <item><description><paramref name="size"/> 设置为 <c>-1</c> 或 <c>max</c> 通常表示最大长度（如 <c>varchar(max)</c>）。</description></item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// 当 <paramref name="tableAlias"/>、<paramref name="name"/> 或 <paramref name="source"/> 为 <c>null</c> 时抛出。
		/// </exception>
		/// <exception cref="ArgumentException">
		/// 当 <paramref name="tableAlias"/>、<paramref name="name"/> 或 <paramref name="source"/> 为空白字符串，或 <paramref name="size"/> 无效时抛出。
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 当 <paramref name="size"/> 小于 <c>-1</c> 或等于 <c>0</c> 时抛出（<c>-1</c> 通常表示 max）。
		/// </exception>
		public ColumnMappingAttribute(string tableAlias, string name, string source, DbTypeEnum dataType, int size, bool nullable, bool dynamics)
		{
			TableAlias = tableAlias;
			ColumnName = name;
			SourceColumn = source ?? name;
			DataType = dataType;
			Size = size;
			Precision = 0;
			Scale = 0;
			Nullable = nullable;
			Dynamics = dynamics;
		}
		#endregion

		#region 初始化ColumnMappingAttribute类实例, 设置Decimal类型数据列
		/// <summary>
		/// 初始化ColumnMappingAttribute类实例, 设置Decimal类型数据列
		/// </summary>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">数据库字段长度(decimal类型的精度)</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public ColumnMappingAttribute(string name, DbTypeEnum dataType, byte precision, bool nullable)
			  : this(null, name, null, dataType, precision, 0, nullable, false) { }

		/// <summary>
		/// 初始化ColumnAttribute类实例, 设置Decimal类型数据列
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">数据库字段长度(decimal类型的精度)</param>
		/// <param name="scale">数据库字段的小数位数</param>
		public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale)
			  : this(tableAlias, name, null, dataType, precision, scale, false, false) { }

		/// <summary>
		/// 初始化ColumnAttribute类实例, 设置Decimal类型数据列
		/// </summary>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">数据库字段长度(decimal类型的精度)</param>
		/// <param name="scale">数据库字段的小数位数</param>
		public ColumnMappingAttribute(string name, DbTypeEnum dataType, byte precision, byte scale)
			  : this(null, name, null, dataType, precision, scale, false, false) { }

		/// <summary>
		/// 初始化ColumnAttribute类实例
		/// </summary>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">表示 Value 属性的最大位数。</param>
		/// <param name="scale">数据库字段的小数位数</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public ColumnMappingAttribute(string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
			  : this(null, name, null, dataType, precision, scale, false, false) { }

		/// <summary>初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。</summary>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="precision">数值字段的精度（总位数），仅对 decimal/numeric 类型有效。</param>
		/// <param name="scale">数值字段的小数位数，仅对 decimal/numeric 类型有效。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		/// <param name="dynamics">指示该字段是否为动态字段，动态字段可能在数据库表中不存在实际列。</param>
		public ColumnMappingAttribute(string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable, bool dynamics)
			 : this(null, name, null, dataType, precision, scale, nullable, dynamics) { }

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
			: this(tableAlias, name, null, dataType, precision, scale, nullable, false) { }

		/// <summary>初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。</summary>
		/// <param name="tableAlias">表别名，用于在多表关联查询时标识字段所属的表。</param>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="precision">数值字段的精度（总位数），仅对 decimal/numeric 类型有效。</param>
		/// <param name="scale">数值字段的小数位数，仅对 decimal/numeric 类型有效。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		/// <param name="dynamics">指示该字段是否为动态字段，动态字段可能在数据库表中不存在实际列。</param>
		public ColumnMappingAttribute(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable, bool dynamics)
			 : this(tableAlias, name, null, dataType, precision, scale, nullable, dynamics) { }

		/// <summary>
		/// 初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。
		/// </summary>
		/// <param name="tableAlias">表别名，用于在多表关联查询时标识字段所属的表。</param>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="source">数据源名称或源表名称。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="precision">数值字段的精度（总位数），仅对 decimal/numeric 类型有效。</param>
		/// <param name="scale">数值字段的小数位数，仅对 decimal/numeric 类型有效。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		public ColumnMappingAttribute(string tableAlias, string name, string source, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
					 : this(tableAlias, name, null, dataType, precision, scale, nullable, false) { }

		/// <summary>
		/// 初始化 <see cref="ColumnMappingAttribute"/> 类的新实例。
		/// </summary>
		/// <param name="tableAlias">表别名，用于在多表关联查询时标识字段所属的表。</param>
		/// <param name="name">字段名称，对应数据库中的列名。</param>
		/// <param name="source">数据源名称或源表名称。</param>
		/// <param name="dataType">字段的数据类型，<see cref="DbTypeEnum"/> 枚举值之一。</param>
		/// <param name="precision">数值字段的精度（总位数），仅对 decimal/numeric 类型有效。</param>
		/// <param name="scale">数值字段的小数位数，仅对 decimal/numeric 类型有效。</param>
		/// <param name="nullable">指示该字段在数据库中是否可为 null。</param>
		/// <param name="dynamics">指示该字段是否为动态字段，动态字段可能在数据库表中不存在实际列。</param>
		/// <remarks>该构造函数用于创建列映射特性的完整配置实例。
		/// <para>
		/// <list type="bullet">
		/// <item><description><paramref name="precision"/> 和 <paramref name="scale"/> 参数通常用于 <c>decimal</c> 或 <c>numeric</c> 类型的字段；对于其他类型，这两个参数可能被忽略。</description></item>
		/// <item><description><paramref name="dynamics"/> 参数为 <c>true</c> 时，表示该字段不参与数据库架构验证，通常用于扩展属性或运行时附加数据。</description></item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// 当 <paramref name="tableAlias"/>、<paramref name="name"/> 或 <paramref name="source"/> 为 <c>null</c> 时抛出。
		/// </exception>
		/// <exception cref="ArgumentException">
		/// 当 <paramref name="tableAlias"/>、<paramref name="name"/> 或 <paramref name="source"/> 为空白字符串时抛出。
		/// </exception>
		public ColumnMappingAttribute(string tableAlias, string name, string source, DbTypeEnum dataType, byte precision, byte scale, bool nullable, bool dynamics)
		{
			TableAlias = tableAlias;
			ColumnName = name;
			SourceColumn = source ?? name;
			DataType = dataType;
			Size = 0;
			Precision = precision;
			Scale = scale;
			Nullable = nullable;
			Dynamics = dynamics;
		}
		#endregion


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
		/// 获取一个值，该值指示当前属性是否可为 null。
		/// </summary>
		/// <value>
		/// 如果属性允许为 null 值，则为 <see cref="bool">true</see>；否则为 <see cref="bool">false</see>。
		/// </value>
		/// <remarks>
		/// <para>该属性影响序列化行为和数据验证规则。</para>
		/// <para>对于值类型（如 <c>int</c>、<c>DateTime</c> 等），当 <c>Nullable = true</c> 时，
		/// 在序列化或数据处理过程中会使用对应的可空类型（如 <c>int?</c>、<c>DateTime?</c>）。</para>
		/// <para>对于引用类型（如 <c>string</c>、<c>List&lt;T&gt;</c> 等），此设置主要影响数据验证，
		/// 因为引用类型本身就可以为 null。</para>
		/// <para>默认值通常由系统根据属性类型自动推断，或通过特性显式指定。</para>
		/// </remarks>
		public bool Nullable { get; private set; }

		/// <summary>
		/// 获取或设置一个值，该值指示当前字段是否为动态字段。
		/// 动态字段可能在对应的数据库表中不存在实际列，通常用于扩展属性或运行时附加数据。
		/// </summary>
		/// <value>
		/// 如果该字段是动态字段，则为 <c>true</c>；否则为 <c>false</c>。
		/// 默认值为 <c>false</c>。
		/// </value>
		/// <remarks>
		/// 动态字段不会参与数据库的 CRUD 操作中的架构验证。
		/// 该属性为只读，只能在类内部通过构造函数或初始化逻辑进行设置。
		/// </remarks>
		public bool Dynamics { get; private set; } = false;

		/// <summary>
		/// 获取当前字段的数据类型。
		/// </summary>
		/// <value>
		/// <see cref="DbTypeEnum"/> 枚举值之一，表示该字段在数据库中的存储类型。
		/// </value>
		/// <remarks>
		/// 该属性用于定义字段映射到数据库时的列数据类型，如整数、字符串、日期时间等。
		/// 该属性为只读，只能在类内部通过构造函数或初始化逻辑进行设置。
		/// </remarks>
		/// <example>
		/// 以下示例演示如何使用该属性：
		/// <code>
		/// var field = new TableField();
		/// // DataType 在对象构造或初始化时设置
		/// if (field.DataType == DbTypeEnum.VarChar)
		/// {
		///     // 处理字符串类型字段
		/// }
		/// </code>
		/// </example>
		public DbTypeEnum DataType { get; private set; }

		/// <summary>
		/// 获取数据库字段的小数位数（Scale）。
		/// </summary>
		/// <value>
		/// 小数部分允许的最大位数。仅对数值类型（如 decimal、numeric）有效。
		/// </value>
		/// <remarks>
		/// 例如，字段类型为 <c>decimal(18, 4)</c> 时，<see cref="Scale"/> 为 4。
		/// 该属性为只读，只能在类内部通过构造函数或初始化逻辑进行设置。
		/// </remarks>
		public byte Scale { get; private set; }

		/// <summary>
		/// 获取数据库字段的长度（Size）。
		/// </summary>
		/// <value>
		/// 字段的最大存储长度或字符数。对于字符串类型表示最大字符数，对于二进制类型表示最大字节数。
		/// </value>
		/// <remarks>
		/// 例如，<c>varchar(255)</c> 的 <see cref="Size"/> 为 255，<c>char(10)</c> 的 <see cref="Size"/> 为 10。
		/// 该属性为只读，只能在类内部通过构造函数或初始化逻辑进行设置。
		/// </remarks>
		public int Size { get; private set; }

		/// <summary>
		/// 获取数据库字段的精度（Precision）。
		/// </summary>
		/// <value>
		/// 数值类型字段的总位数（包括整数部分和小数部分）。仅对数值类型（如 decimal、numeric）有效。
		/// </value>
		/// <remarks>
		/// 例如，字段类型为 <c>decimal(18, 4)</c> 时，<see cref="Precision"/> 为 18。
		/// 注意：<see cref="Precision"/> 表示总位数，而 <see cref="Scale"/> 表示小数位数。
		/// 该属性为只读，只能在类内部通过构造函数或初始化逻辑进行设置。
		/// </remarks>
		public byte Precision { get; private set; }
	}
}
