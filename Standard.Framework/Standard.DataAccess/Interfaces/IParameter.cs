using System.Data;
using System.Xml.Serialization;
using Basic.Enums;

namespace Basic.Interfaces
{
	/// <summary>
	/// 数据库参数集合类
	/// </summary>
	public interface IParameter : IXmlSerializable
	{
		/// <summary>
		/// 获取或设置数据库参数的名称
		/// </summary>
		/// <value>ParameterName 以“@参数名”格式来指定。</value>
		string ParameterName { get; set; }

		/// <summary>
		/// 获取或设置源列的名称，该源列映射到 DataSet 并用于加载或返回 Value
		/// </summary>
		/// <value>映射到 DataSet /DataTable/Entity的源列(属性)的名称。默认值为空字符串。</value>
		string SourceColumn { get; set; }

		/// <summary>
		/// 获取或设置参数的类型。
		/// </summary>
		/// <value>DataTypeEnum 值之一。默认值为 None。</value>
		DataTypeEnum ParameterType { get; set; }

		/// <summary>
		/// 获取或设置一个值，该值指示参数是否接受空值。
		/// </summary>
		/// <value>如果接受 null 值，则为 true；否则为 false。 默认值为 false。</value>
		bool IsNullable { get; set; }

		/// <summary>
		/// 获取或设置列中数据的最大大小（以字节为单位）。
		/// </summary>
		/// <value>列中数据的最大大小（以字节为单位）。默认值是从参数值推导出的。</value>
		int Size { get; set; }

		/// <summary>
		/// 获取或设置用来表示 Value 属性的最大位数。 
		/// </summary>
		/// <value>用于表示 Value 属性的最大位数。 默认值为 0。这指示数据提供程序设置 Value 的精度。 </value>
		byte Precision { get; set; }

		/// <summary>
		/// 获取或设置 Value 解析为的小数位数。
		/// </summary>
		/// <value>要将 Value 解析为的小数位数。默认值为 0。</value>
		byte Scale { get; set; }

		/// <summary>
		/// 获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
		/// </summary>
		/// <value>ParameterDirection 值之一，默认值为 Input。</value>
		ParameterDirection Direction { get; set; }
	}
}
