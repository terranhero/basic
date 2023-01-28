using System.Xml;
using System.Xml.Serialization;

using Basic.Functions;
using Basic.Designer;
using Basic.Enums;
using Basic.DataEntities;
using System.CodeDom;
using System;
using System.Drawing.Design;
using Basic.Configuration;

namespace Basic.Converters
{
	/// <summary>
	/// 表示动态配置命令
	/// </summary>
	internal class ConverterDynamicCommand : ConverterDataCommand, IXmlSerializable
	{
		/// <summary>
		/// 初始化 DynamicCommandElement 类实例
		/// </summary>
		internal protected ConverterDynamicCommand(ConverterConfiguration converter) : base(converter) { }

		private string _SelectText = string.Empty;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 SELECT 部分，默认值为空字符串。</value>
		public virtual string SelectText { get { return _SelectText; } set { _SelectText = value; } }

		private string _FromText = string.Empty;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 FROM 部分，默认值为空字符串。</value>
		public virtual string FromText { get { return _FromText; } set { _FromText = value; } }

		private string _WhereText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 WHERE 部分，默认值为空字符串。</value>
		public virtual string WhereText { get { return _WhereText; } set { _WhereText = value; } }

		private string _GroupText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 GROUP BY 部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 GROUP BY 部分，默认值为空字符串。</value>
		public virtual string GroupText { get { return _GroupText; } set { _GroupText = value; } }

		private string _HavingText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 HANVING 部分，默认值为空字符串。</value>
		public virtual string HavingText { get { return _HavingText; } set { _HavingText = value; } }

		private string _OrderText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 ORDER BY 部分，默认值为空字符串。</value>
		public virtual string OrderText { get { return _OrderText; } set { _OrderText = value; } }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return DynamicCommandElement.XmlElementName; } }

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.Name == DynamicCommandElement.SelectTextElement)
			{
				_SelectText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.Name == DynamicCommandElement.FromTextElement)
			{
				_FromText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.Name == DynamicCommandElement.WhereTextElement)
			{
				_WhereText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.Name == DynamicCommandElement.GroupTextElement)
			{
				_GroupText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.Name == DynamicCommandElement.HavingTextElement)
			{
				_HavingText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.Name == DynamicCommandElement.OrderTextElement)
			{
				_OrderText = reader.ReadString();
			}
			return base.ReadContent(reader);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			base.WriteContent(writer);
			writer.WriteStartElement(DynamicCommandElement.SelectTextElement);
			writer.WriteCData(_SelectText);//写CData
			writer.WriteEndElement();

			writer.WriteStartElement(DynamicCommandElement.FromTextElement);
			writer.WriteCData(_FromText);//写CData
			writer.WriteEndElement();

			if (string.IsNullOrEmpty(_WhereText) == false)
			{
				writer.WriteStartElement(DynamicCommandElement.WhereTextElement);
				writer.WriteCData(_WhereText);//写CData
				writer.WriteEndElement();
			}
			if (string.IsNullOrEmpty(_GroupText) == false)
			{
				writer.WriteStartElement(DynamicCommandElement.GroupTextElement);
				writer.WriteCData(_GroupText);//写CData
				writer.WriteEndElement();
			}

			if (string.IsNullOrEmpty(_HavingText) == false)
			{
				writer.WriteStartElement(DynamicCommandElement.HavingTextElement);
				writer.WriteCData(_HavingText);//写CData
				writer.WriteEndElement();
			}
			writer.WriteElementString(DynamicCommandElement.OrderTextElement, _OrderText);
		}
	}
}
