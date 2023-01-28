using System;
using System.Xml;
using System.Xml.Serialization;
using Basic.Enums;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示数据库列信息
	/// </summary>
	public sealed class ColumnInfo : System.Xml.Serialization.IXmlSerializable
	{
		#region 实体定义字段
		internal const string XmlElementName = "TableColumn";
		internal const string NameAttribute = "Name";
		internal const string DbTypeAttribute = "DbType";
		internal const string NullableAttribute = "Nullable";
		internal const string SizeAttribute = "Size";
		internal const string PrecisionAttribute = "Precision";
		internal const string ScaleAttribute = "Scale";
		internal const string PrimaryKeyAttribute = "PKey";
		internal const string PropertyAttribute = "Property";
		internal const string DefaultValueAttribute = "Default";
		internal const string ComputedAttribute = "Computed";

		private readonly ColumnInfoCollection _Columns;
		#endregion

		/// <summary>初始化 ColumnInfo 类实例</summary>
		/// <param name="columns"></param>
		internal ColumnInfo(ColumnInfoCollection columns) { _Columns = columns; }

		private bool _PrimaryKey = false;
		/// <summary>
		/// 当前列是否为主键
		/// </summary>
		public bool PrimaryKey { get { return _PrimaryKey; } }

		private bool _Computed = false;
		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		public bool Computed { get { return _Computed; } }

		private string _Name = string.Empty;
		/// <summary>
		/// 获取或设置数据库表中的列的名称。
		/// </summary>
		public string Name { get { return _Name; } }

		private string _Comment = null;
		/// <summary>
		/// 属性对应数据库字段描述
		/// </summary>
		public string Comment { get { return _Comment; } }

		private DbTypeEnum _DbType = DbTypeEnum.NVarChar;
		/// <summary>
		/// 属性类型名称
		/// </summary>
		public DbTypeEnum DbType { get { return _DbType; } }

		private int _Size = 0;
		/// <summary>
		/// 获取或设置列中数据的最大大小（以字节为单位）。
		/// </summary>
		/// <value>列中数据的最大大小（以字节为单位）。默认值是从参数值推导出的。</value>
		public int Size { get { return _Size; } }

		private byte _Precision = 0;
		/// <summary>
		/// 获取或设置用来表示 Value 属性的最大位数。 
		/// </summary>
		/// <value>用于表示 Value 属性的最大位数。 默认值为 0。这指示数据提供程序设置 Value 的精度。 </value>
		public byte Precision { get { return _Precision; } }

		private byte _Scale = 0;
		/// <summary>
		/// 获取或设置 Value 解析为的小数位数。
		/// </summary>
		/// <value>要将 Value 解析为的小数位数。默认值为 0。</value>
		public byte Scale { get { return _Scale; } }

		private bool _Nullable = false;
		/// <summary>
		/// 属性是否允许为空
		/// </summary>
		public bool Nullable { get { return _Nullable; } }

		private string _DefaultValue = null;
		/// <summary>
		/// 属性的默认值
		/// </summary>
		public string DefaultValue { get { return _DefaultValue; } }

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		internal void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			if (reader.HasAttributes)
			{
				for (int index = 0; index < reader.AttributeCount; index++)
				{
					reader.MoveToAttribute(index); string value = reader.GetAttribute(index);
					if (reader.LocalName == NameAttribute) { _Name = value; }
              		else if (reader.LocalName == DbTypeAttribute) { Enum.TryParse<DbTypeEnum>(value, out _DbType); }
					else if (reader.LocalName == NullableAttribute) { _Nullable = Convert.ToBoolean(value); }
					else if (reader.LocalName == SizeAttribute) { _Size = Convert.ToInt32(value); }
					else if (reader.LocalName == PrecisionAttribute) { _Precision = Convert.ToByte(value); }
					else if (reader.LocalName == ScaleAttribute) { _Scale = Convert.ToByte(value); }
					else if (reader.LocalName == PrimaryKeyAttribute) { _PrimaryKey = Convert.ToBoolean(value); }
					else if (reader.LocalName == DefaultValueAttribute) { _DefaultValue = value; }
					else if (reader.LocalName == ComputedAttribute) { _Computed = Convert.ToBoolean(value); }
				}
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == XmlElementName)
				{
					_Comment = reader.ReadString();
				}
			}
		}

		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
		/// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { }
		#endregion
	}
}
