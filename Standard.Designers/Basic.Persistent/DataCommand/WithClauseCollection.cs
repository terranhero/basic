using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Basic.Configuration;
using Basic.Enums;

namespace Basic.Designer
{
	/// <summary>
	/// 表示 With 子句中表定义和表查询集合
	/// </summary>
	[Editor(typeof(WithClausesEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public sealed class WithClauseCollection : ObservableCollection<WithClause>, IXmlSerializable
	{
		/// <summary>WithClauses 配置节名称</summary>
		internal const string XmlElementName = "WithClauses";

		private readonly DynamicCommandElement _DynamicCommand;
		private readonly PersistentConfiguration _Persistent;
		internal WithClauseCollection(DynamicCommandElement command) { _DynamicCommand = command; _Persistent = command.Persistent; }

		/// <summary>判断当前With子句中是否存在分页子句</summary>
		/// <returns>如果存在分页子句，则返回true，否则返回false。</returns>
		internal bool AllowPaging()
		{
			return Items.Any(m => m.AllowPaging == true);
		}

		/// <summary>
		/// 引发带有提供的参数的 BaseDictionary&lt;T&gt;.CollectionChanged 事件。
		/// </summary>
		/// <param name="e">要引发的事件的参数。</param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			_Persistent.OnFileContentChanged(EventArgs.Empty);
			base.OnCollectionChanged(e);
		}

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		internal void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlElementName) { break; }
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == WithClause.XmlElementName)
				{
					WithClause with = new WithClause(_DynamicCommand);
					with.ReadXml(reader.ReadSubtree());
					base.Add(with);
				}
			}
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		internal void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(XmlElementName);
			foreach (WithClause child in base.Items)
			{
				child.WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		internal void GenerateConfiguration(System.Xml.XmlWriter writer, ConnectionTypeEnum connectionType)
		{
			writer.WriteStartElement(XmlElementName);
			foreach (WithClause child in base.Items)
			{
				child.GenerateConfiguration(writer, connectionType);
			}
			writer.WriteEndElement();
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
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { WriteXml(writer); }
		#endregion
	}
}
