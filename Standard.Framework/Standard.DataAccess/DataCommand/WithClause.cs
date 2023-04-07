using System.Xml;
using System.Xml.Serialization;

namespace Basic.DataAccess
{
	/// <summary>
	/// 获取或设置 WITH 临时结果集的定义
	/// </summary>
	public sealed class WithClause : IXmlSerializable
	{
		#region Xml 节点名称常量
		/// <summary>表示 Element 属性。</summary>
		internal const string XmlElementName = "WithClause";

		/// <summary>表示当前WITH子句是否允许分页元素名称。</summary>
		internal const string AllowPagingAttribute = "Paging";

		/// <summary>表示 TableDefinition 元素名称。</summary>
		internal const string TableNameAttribute = "TableName";

		/// <summary>表示 TableDefinition 元素名称。</summary>
		internal const string TableDefinitionElement = "Definition";

		/// <summary>表示 TableQuery 元素名称。</summary>
		internal const string TableQueryElement = "Query";
		#endregion

		/// <summary>
		/// 初始化 WithClause 类实例。
		/// </summary>
		/// <param name="nofity"></param>
		public WithClause(DynamicCommand nofity) { }

		/// <summary>
		/// 初始化 WithClause 类实例。
		/// </summary>
		/// <param name="nofity"></param>
		/// <param name="name"></param>
		/// <param name="definition"></param>
		/// <param name="query"></param>
		private WithClause(DynamicCommand nofity, string name, string definition, string query)
		{
			_TableName = name;
			_TableDefinition = definition;
			_TableQuery = query;
		}

		private bool _AllowPaging = false;
		/// <summary>
		/// 获取或设置 WITH 临时结果集的表查询
		/// </summary>
		[System.ComponentModel.Description("获取或设置 WITH 子句是否是整个查询的分页语句"), System.ComponentModel.DefaultValue(false)]
		public bool AllowPaging { get { return _AllowPaging; } set { _AllowPaging = value; } }

		private string _TableName = string.Empty;
		/// <summary>
		/// 获取或设置 WITH 临时结果集的表定义名称
		/// </summary>
		[System.ComponentModel.Description("获取或设置 WITH 临时结果集的表定义名称"), System.ComponentModel.DefaultValue("")]
		public string TableName { get { return _TableName; } set { _TableName = value; } }

		private string _TableDefinition = string.Empty;
		/// <summary>
		/// 获取或设置 WITH 临时结果集的表定义
		/// </summary>
		[System.ComponentModel.Description("获取或设置 WITH 临时结果集的表定义"), System.ComponentModel.DefaultValue("")]
		public string TableDefinition { get { return _TableDefinition; } set { _TableDefinition = value; } }

		private string _TableQuery = string.Empty;
		/// <summary>
		/// 获取或设置 WITH 临时结果集的表查询
		/// </summary>
		[System.ComponentModel.Description("获取或设置 WITH 临时结果集的表查询"), System.ComponentModel.DefaultValue("")]
		public string TableQuery { get { return _TableQuery; } set { _TableQuery = value; } }

		/// <summary>
		/// 创建作为当前实例副本的新对象。
		/// </summary>
		/// <returns>作为此实例副本的新对象。</returns>
		public WithClause Clone(DynamicCommand command)
		{
			return new WithClause(command, _TableName, _TableDefinition, _TableQuery);
		}

		/// <summary>将With子句拼装成可执行的 SQL 语句</summary>
		/// <returns>返回可执行的 SQL 语句</returns>
		public string ToSql()
		{
			if (string.IsNullOrWhiteSpace(_TableDefinition)) { return string.Concat(TableName, " AS (", TableQuery, ")"); }
			return string.Concat(TableName, "(", TableDefinition, ") AS (", TableQuery, ")");
		}

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
					reader.MoveToAttribute(index);
					if (reader.LocalName == TableNameAttribute) { _TableName = reader.GetAttribute(index); }
				}
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				if (reader.NodeType == XmlNodeType.Element && reader.LocalName == TableDefinitionElement)
				{
					_TableDefinition = reader.ReadString();
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == TableQueryElement)
				{
					_TableQuery = reader.ReadString();
				}
			}
		}

		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
		/// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

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
