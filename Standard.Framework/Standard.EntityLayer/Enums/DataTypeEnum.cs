
namespace Basic.Enums
{
	/// <summary>
	/// 用户自定义数据库类型,动态映射各类数据库参数的数据类型（已过时，）。
	/// </summary>
	/// <see cref="Basic.Enums.DbTypeEnum">请使用此类型替代。</see>
	/// <seealso cref="Basic.Enums.DbTypeEnum"/>
	public enum DataTypeEnum
	{
		/// <summary>
		/// 无效的数据类型
		/// </summary>
		None = 0,
		/// <summary>
		/// 映射到.NetFramework的Guid类型
		/// 映射到SqlDbType枚举的UniqueIdentifier类型，全局唯一标识符（或 GUID）。
		/// 映射到OleDbType枚举的Guid类型，全局唯一标识符（或 GUID） (DBTYPE_GUID)。 
		/// </summary>
		Guid = 1,
		/// <summary>
		/// 映射到.NetFramework的Boolean类型
		/// 映射到SqlDbType枚举的Bit类型,无符号数值，可以是 0、1 或 Nothing。
		/// 映射到OleDbType枚举的Boolean类型，布尔值 (DBTYPE_BOOL)。
		/// </summary>
		Boolean,
		///// <summary>
		///// 映射到.NetFramework的Byte类型
		///// 映射到SqlDbType枚举的TinyInt类型，8 位无符号整数。
		///// 映射到OleDbType枚举的TinyInt类型，8 位带符号的整数 (DBTYPE_I2)。
		///// </summary>
		//Byte,
		/// <summary>
		/// 映射到.NetFramework的Int16类型
		/// 映射到SqlDbType枚举的SmallInt类型,16 位带符号整数。
		/// 映射到OleDbType枚举的SmallInt类型，16 位带符号的整数 (DBTYPE_I2)。
		/// </summary>
		Int16,
		/// <summary>
		/// 映射到.NetFramework的Int32类型
		/// 映射到SqlDbType枚举的Int类型,32 位带符号整数。 
		/// 映射到OleDbType枚举的Integer类型，32 位带符号的整数 (DBTYPE_I4)。
		/// </summary>
		Int32,
		/// <summary>
		/// 映射到.NetFramework的Int64类型
		/// 映射到SqlDbType枚举的BigInt类型，64 位有符号整数。 
		/// 映射到OleDbType枚举的BigInt类型，64 位带符号的整数 (DBTYPE_I8)。 
		/// </summary>
		Int64,
		///// <summary>
		///// 映射到.NetFramework的Byte类型。
		///// 无映射到SqlDbType枚举的类型。
		///// 映射到OleDbType枚举的UnsignedTinyInt类型，64 位无符号整数 (DBTYPE_UI8)。
		///// </summary>
		//UByte,
		///// <summary>
		///// 映射到.NetFramework的UInt16类型。
		///// 无映射到SqlDbType枚举的类型。
		///// 映射到OleDbType枚举的UnsignedSmallInt类型，8 位无符号整数 (DBTYPE_UI8)。
		///// </summary>
		//UInt16,
		///// <summary>
		///// 映射到.NetFramework的UInt32类型。
		///// 无映射到SqlDbType枚举的类型。
		///// 映射到OleDbType枚举的UnsignedInt类型，16 位无符号整数 (DBTYPE_UI8)。
		///// </summary>
		//UInt32,
		///// <summary>
		///// 映射到.NetFramework的UInt64类型。
		///// 无映射到SqlDbType枚举的类型。
		///// 映射到OleDbType枚举的UnsignedBigInt类型，32 位无符号整数 (DBTYPE_UI8)。
		///// </summary>
		//UInt64,
		/// <summary>
		/// Decimal，具有定点精度和小数位数的精确数值
		/// 映射到SqlDbType枚举的Decimal类型，固定精度和小数位数数值，在 -10^38 -1 和 10^38 -1之间。
		/// 映射到OleDbType枚举的Decimal类型，定点精度和小数位数数值，范围在 -10^38 -1 和 10^38 -1 之间 (DBTYPE_DECIMAL)。 
		/// </summary>
		Decimal,
		///// <summary>
		///// Decimal，具有定点精度和小数位数的精确数值
		///// 映射到SqlDbType枚举的Decimal类型，固定精度和小数位数数值，在 -10 38 -1 和 10 38 -1 之间。
		///// 映射到OleDbType枚举的Numeric类型，具有定点精度和小数位数的精确数值 (DBTYPE_NUMERIC)。
		///// </summary>
		//Numeric,
		///// <summary>
		///// Decimal，变长数值
		///// 映射到SqlDbType枚举的Decimal类型，固定精度和小数位数数值，在 -10 38 -1 和 10 38 -1 之间。
		///// 映射到OleDbType枚举的VarNumeric类型，具有定点精度和小数位数的精确数值 (DBTYPE_NUMERIC)。
		///// </summary>
		//VarNumeric,
		/// <summary>
		/// Single，单精度浮点型
		/// 映射到SqlDbType枚举的Real类型，-3.40E +38 到 3.40E +38 范围内的浮点数。
		/// 映射到OleDbType枚举的Numeric类型，范围在 -3.40E +38 到 3.40E +38 之间 (DBTYPE_R4)。 
		/// </summary>
		Single,
		/// <summary>
		/// Double，双精度浮点型
		/// 映射到SqlDbType枚举的Float类型，-1.79E +308 到 1.79E +308 范围内的浮点数。
		/// 映射到OleDbType枚举的Double类型，范围在-1.79E +308 到 1.79E +308之间 (DBTYPE_R4)。 
		/// </summary>
		Double,
		///// <summary>
		///// Decimal,货币值
		///// 映射到SqlDbType枚举的SmallMoney类型，范围在 -214,748.3648 到 +214,748.3647 之间，精度为千分之十个货币单位。
		///// 映射到OleDbType枚举的Double类型，范围在-1.79E +308 到 1.79E +308之间 (DBTYPE_R4)。 
		///// </summary>
		//SmallMoney,
		///// <summary>
		///// Decimal,货币值
		///// 映射到SqlDbType枚举的Money类型，范围在 -2 63（即 -922,337,203,685,477.5808）到 2 63 -1（即 +922,337,203,685,477.5807）之间，精度为千分之十个货币单位
		///// 映射到OleDbType枚举的Currency类型，一个货币值，范围在 -2 63（或 -922,337,203,685,477.5808）到 2 63 -1（或 +922,337,203,685,477.5807）之间，精度为千分之十个货币单位 (DBTYPE_CY)。 
		///// </summary>
		//Money,
		/// <summary>
		/// Byte[],二进制数据的固定长度流
		/// 映射到SqlDbType枚举的Binary类型，范围在 1 到 8,000 个字节之间。 
		/// 映射到OleDbType枚举的Binary类型，二进制数据流 (DBTYPE_BYTES)。 
		/// </summary>
		Binary,
		/// <summary>
		/// Byte[],二进制数据的可变长度流
		/// 映射到SqlDbType枚举的VarBinary类型，范围在 1 到 8,000 个字节之间。 如果字节数组大于 8,000 个字节，隐式转换会失败。
		/// 在使用比 8,000 个字节大的字节数组时，请显式设置对象。 
		/// 映射到OleDbType枚举的Binary类型,二进制数据的变长流。 
		/// </summary>
		VarBinary,
		/// <summary>
		/// Byte[],二进制数据的可变长度流
		/// 映射到SqlDbType枚举的Image类型，二进制数据的可变长度流，范围在 0 到 2 31 -1（即 2,147,483,647）字节之间。 
		/// 映射到OleDbType枚举的LongVarBinary类型,二进制数据的变长流。 
		/// </summary>
		Image,
		/// <summary>
		/// String，非 Unicode 字符的固定长度流，范围在 1 到 8,000 个字符之间。
		/// 映射到SqlDbType枚举的Char类型，非 Unicode 字符的固定长度流，范围在 1 到 8,000 个字符之间。
		/// 映射到OleDbType枚举的Char类型，字符串 (DBTYPE_STR)。 
		/// </summary>
		Char,
		/// <summary>
		/// String，非 Unicode 字符的固定长度流，范围在 1 到 8,000 个字符之间。
		/// 映射到SqlDbType枚举的VarChar类型，非 Unicode 字符的可变长度流，范围在 1 到 8,000 个字符之间。当数据库列为 varchar(max) 时，使用 VarChar 。 
		/// 映射到OleDbType枚举的VarChar类型，非 Unicode 字符的变长流。
		/// </summary>
		VarChar,
		/// <summary>
		/// String
		/// 映射到SqlDbType枚举的Text类型，非 Unicode 数据的可变长度流，最大长度为 2 31 -1（即 2,147,483,647）个字符。
		/// 映射到OleDbType枚举的LongVarChar类型，长的字符串值。 
		/// </summary>
		Text,
		/// <summary>
		/// String，非 Unicode 字符的固定长度流，范围在 1 到 4,000 个字符之间。
		/// 映射到SqlDbType枚举的NChar类型，Unicode 字符的固定长度流，范围在 1 到 4,000 个字符之间。 
		/// 映射到OleDbType枚举的WChar类型，以 null 终止的 Unicode 字符流 (DBTYPE_WSTR)。
		/// </summary>
		NChar,
		/// <summary>
		/// String，Unicode 字符的可变长度流，范围在 1 到 4,000 个字符之间。
		/// 映射到SqlDbType枚举的NVarChar类型， 如果字符串大于 4,000 个字符，隐式转换会失败。 在使用比 4,000 个字符更长的字符串时，请显式设置对象。 当数据库列为 nvarchar(max) 时，使用 NVarChar 。 
		/// 映射到OleDbType枚举的VarWChar类型，长可变、以 null 终止的 Unicode 字符流。 
		/// </summary>
		NVarChar,
		/// <summary>
		/// String，Unicode 数据的可变长度流，最大长度为 2^30 - 1（即 1,073,741,823）个字符。
		/// 映射到SqlDbType枚举的NText类型，Unicode 数据的可变长度流，最大长度为 2^30 - 1（即 1,073,741,823）个字符。
		/// 映射到OleDbType枚举的LongVarWChar类型，长的以 null 终止的 Unicode 字符串值。
		/// </summary>
		NText,
		/// <summary>
		/// DateTime，值范围从公元 1 年 1 月 1 日到公元 9999 年 12 月 31 日。
		/// 映射到SqlDbType枚举的Date类型，日期数据，值范围从公元 1 年 1 月 1 日到公元 9999 年 12 月 31 日。
		/// 映射到OleDbType枚举的DBDate类型，格式为 yyyymmdd 的日期数据 (DBTYPE_DBDATE)。
		/// </summary>
		Date,
		/// <summary>
		/// TimeSpan，基于 24 小时制的时间数据。
		/// 映射到SqlDbType枚举的Time类型，时间值范围从 00:00:00 到 23:59:59.9999999，精度为 100 毫微秒。
		/// 映射到OleDbType枚举的DBTime类型，格式为 hhmmss 的时间数据 (DBTYPE_DBTIME)。
		/// </summary>
		Time,
		/// <summary>
		/// Byte[]，timestamp 通常用作为表行添加版本戳的机制。 存储大小为 8 字节。 
		/// 映射到SqlDbType枚举的Timestamp类型，自动生成的二进制数字，它们保证在数据库中是唯一的。 
		/// 映射到OleDbType枚举的Binary(8)类型，格式为 hhmmss 的时间数据 (DBTYPE_DBTIME)。
		/// </summary>
		Timestamp,
		/// <summary>
		/// DateTime，日期和时间数据。
		/// 映射到SqlDbType枚举的DateTime类型，值范围从 1753 年 1 月 1 日到 9999 年 12 月 31 日，精度为 3.33 毫秒。 
		/// 映射到OleDbType枚举的Date类型，日期数据，存储为双精度型 (DBTYPE_DATE)。 整数部分是自 1899 年 12 月 30 日以来的天数，而小数部分是不足一天的部分。
		/// </summary>
		DateTime,
		/// <summary>
		/// DateTime，日期和时间数据。
		/// 映射到SqlDbType枚举的DateTime2类型，日期值范围从公元 1 年 1 月 1 日到公元 9999 年 12 月 31 日。
		/// 时间值范围从 00:00:00 到 23:59:59.9999999，精度为 100 毫微秒。
		/// 无映射到OleDbType枚举的类型。
		/// </summary>
		DateTime2,
		/// <summary>
		/// DateTime，日期和时间数据。
		/// 映射到SqlDbType枚举的DateTimeOffset类型，显示时区的日期和时间数据。 日期值范围从公元 1 年 1 月 1 日到公元 9999 年 12 月 31 日。 
		/// 时间值范围从 00:00:00 到 23:59:59.9999999，精度为 100 毫微秒。 时区值范围从 -14:00 到 +14:00。 
		/// 无映射到OleDbType枚举的类型。
		/// </summary>
		DateTimeOffset,
		///// <summary>
		///// DateTime，日期和时间数据。
		///// 映射到SqlDbType枚举的SmallDateTime类型，日期和时间数据，值范围从 1900 年 1 月 1 日到 2079 年 6 月 6 日，精度为 1 分钟。
		///// 无映射到OleDbType枚举的类型。
		///// </summary>
		//SmallDateTime,
		///// <summary>
		///// 自定义表类型，必须要SqlServer 2008以上的版本使用。
		///// 映射到SqlDbType枚举的Structured类型，指定表值参数中包含的构造数据的特殊数据类型。 
		///// </summary>
		//Structured,
		///// <summary>
		///// 用户自定义类型，必须要SqlServer 2005以上的版本使用。
		///// 映射到SqlDbType枚举的Udt类型，SQL Server 2005 用户定义的类型 (UDT)。 
		///// </summary>
		//UserDefineType,
		///// <summary>
		///// 特殊数据类型，可以包含数值、字符串、二进制或日期数据，以及 SQL Server 值 Empty 和 Null，后两个值在未声明其他类型的情况下采用。 
		///// 映射到SqlDbType枚举的Variant类型，SQL Server 2005 用户定义的类型 (UDT)。 
		///// </summary>
		//Variant,
		///// <summary>
		///// XML 值。可以包含数值、字符串、二进制或日期数据，以及 SQL Server 值 Empty 和 Null，后两个值在未声明其他类型的情况下采用。 
		///// 映射到SqlDbType枚举的Variant类型，使用 GetValue 方法或 Value 属性获取字符串形式的 XML，或通过调用 CreateReader 方法获取 XmlReader 形式的 XML。
		///// </summary>
		//Xml,
		/// <summary>
		/// 包含可返回结果集的游标。 
		/// 无法正确映射到SqlDbType，SqlServer将忽略此参数
		/// 映射到OracleDbType枚举的RefCursor类型。
		/// </summary>
		RefCursor
	}
}
