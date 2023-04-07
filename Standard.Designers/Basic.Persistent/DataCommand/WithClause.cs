using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Basic.Configuration;
using Basic.Designer;
using Basic.Enums;

namespace Basic.Designer
{
	/// <summary>
	/// 获取或设置 WITH 临时结果集的定义
	/// </summary>
	[PersistentDescriptionAttribute("获取或设置 Transact-SQL 语句的 WITH 子句")]
	[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
	[Editor(typeof(WithClauseEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public sealed class WithClause : AbstractCustomTypeDescriptor
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

		private readonly DynamicCommandElement _DynamicCommand;
		private readonly PersistentConfiguration _Persistent;
		internal WithClause(DynamicCommandElement command)
			: base(command) { _DynamicCommand = command; _Persistent = command.Persistent; }

		private bool _AllowPaging = false;
		/// <summary>
		/// 获取或设置 WITH 临时结果集的表查询
		/// </summary>
		[System.ComponentModel.Description("获取或设置 WITH 子句是否是整个查询的分页语句"), System.ComponentModel.DefaultValue(false)]
		public bool AllowPaging
		{
			get { return _AllowPaging; }
			set { if (_AllowPaging != value) { _AllowPaging = value; base.RaisePropertyChanged("AllowPaging"); } }
		}

		private string _TableName = string.Empty;
		/// <summary>
		/// 获取或设置 WITH 临时结果集的表定义名称
		/// </summary>
		[System.ComponentModel.Description("获取或设置 WITH 临时结果集的表定义名称"), System.ComponentModel.DefaultValue("")]
		[Editor(typeof(WithClauseEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string TableName
		{
			get { return _TableName; }
			set
			{
				if (_TableName != value)
				{
					_TableName = value;
					base.RaisePropertyChanged("TableName");
				}
			}
		}

		private string _TableDefinition = string.Empty;
		/// <summary>
		/// 获取或设置 WITH 临时结果集的表定义
		/// </summary>
		[System.ComponentModel.Description("获取或设置 WITH 临时结果集的表定义"), System.ComponentModel.DefaultValue("")]
		[Editor(typeof(WithClauseEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string TableDefinition
		{
			get { return _TableDefinition; }
			set
			{
				if (_TableDefinition != value)
				{
					_TableDefinition = value;
					base.RaisePropertyChanged("TableDefinition");
				}
			}
		}

		private string _TableQuery = string.Empty;
		/// <summary>
		/// 获取或设置 WITH 临时结果集的表查询
		/// </summary>
		[System.ComponentModel.Description("获取或设置 WITH 临时结果集的表查询"), System.ComponentModel.DefaultValue("")]
		[Editor(typeof(WithClauseEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string TableQuery
		{
			get { return _TableQuery; }
			set
			{
				if (_TableQuery != value)
				{
					_TableQuery = value;
					base.RaisePropertyChanged("TableDefinition");
				}
			}
		}

		/// <summary>将With子句拼装成可执行的 SQL 语句</summary>
		/// <returns>返回可执行的 SQL 语句</returns>
		public string ToSql()
		{
			if (string.IsNullOrWhiteSpace(_TableDefinition)) { return string.Concat(TableName, " AS (", TableQuery, ")"); }
			return string.Concat(TableName, "(", TableDefinition, ") AS (", TableQuery, ")");
		}

		/// <summary>表示 WithClause 类型的字符串表示形式</summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(_TableName)) { return typeof(WithClause).Name; }
			return _TableName;
		}

		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return XmlElementName; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return _TableName; }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return null; } }

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		internal protected override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == TableDefinitionElement)
			{
				_TableDefinition = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == TableQueryElement)
			{
				_TableQuery = reader.ReadString();
			}
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString(TableDefinitionElement, _TableDefinition);
			writer.WriteStartElement(TableQueryElement);
			writer.WriteCData(_TableQuery);//写CData
			writer.WriteEndElement();
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
			writer.WriteStartElement(XmlElementName);

			writer.WriteAttributeString(TableNameAttribute, _TableName);
			writer.WriteElementString(TableDefinitionElement, _TableDefinition);

			writer.WriteStartElement(TableQueryElement);
			writer.WriteCData(_DynamicCommand.CreateCommandText(_TableQuery, connectionType));//写CData
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == TableNameAttribute) { _TableName = value; }
			else if (name == AllowPagingAttribute) { _AllowPaging = Convert.ToBoolean(value); }
			return true;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(XmlWriter writer)
		{
			writer.WriteAttributeString(TableNameAttribute, _TableName);
			writer.WriteAttributeString(AllowPagingAttribute, Convert.ToString(_AllowPaging));
		}

	}
}
