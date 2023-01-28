using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.EntityLayer;
using System.Xml.Serialization;
using System.Xml;

namespace Basic.Validations
{
	/// <summary>
	/// 表示实体属性验证的结果
	/// </summary>
	public sealed class ValidationPropertyResult : System.Collections.ObjectModel.Collection<string>, IXmlSerializable
	{
		private const string ElementName = "ErrorMessage";
		/// <summary>
		/// 在调用 Remove 方法时需要保留
		/// </summary>
		private readonly SortedSet<string> _AllowSaveList;
		/// <summary>
		/// 初始化 ValidationPropertyResult 类的实例。
		/// </summary>
		/// <param name="entity">一个 AbstractEntity 子类的实例，表示当前验证的实体类。</param>
		/// <param name="propertyName">验证属性名称</param>
		internal ValidationPropertyResult(AbstractEntity entity, string propertyName)
		{
			_AllowSaveList = new SortedSet<string>();
			abstractEntity = entity;
			m_PropertyName = propertyName;
		}
		private readonly AbstractEntity abstractEntity;
		private readonly string m_PropertyName;
		/// <summary>
		/// 获取实体属性的名称
		/// </summary>
		public string PropertyName { get { return m_PropertyName; } }

		/// <summary>
		/// 获取一个值，该值指示当前属性是否具有一个失败的验证规则。
		/// </summary>
		public bool HasError { get { return this.Count > 0; } }

		/// <summary>
		/// 当前属性的异常信息
		/// </summary>
		public string ErrorMessage { get { return string.Join(",", this.ToArray()); } }

		/// <summary>
		/// 将对象添加到 ValidationPropertyResult 的结尾处。
		/// </summary>
		/// <param name="item">要添加到 ValidationPropertyResult 结尾处的对象。</param>
		/// <param name="allowSaved">该对象在Remove方法后是否保留。</param>
		public void Add(string item, bool allowSaved)
		{
			base.Add(item);
			if (allowSaved) { _AllowSaveList.Add(item); }
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void ClearItems()
		{
			List<string> list = new List<string>(base.Items.Join(_AllowSaveList, m => m, m => m, (im, om) => { return im; }));
			base.ClearItems(); list.ForEach(m => base.Add(m));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			string item = base.Items[index];
			if (!_AllowSaveList.Contains(item))
				base.RemoveItem(index);
		}

		///// <summary>
		///// 添加异常信息到结果集合的末尾。
		///// </summary>
		///// <param name="collection">需要添加的异常信息</param>
		//public void AddRange(IEnumerable<string> collection)
		//{
		//	foreach (string item in collection)
		//	{
		//		base.Add(item);
		//	}
		//}


		#region 接口 IXmlSerializable 的实现
		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		internal void ReadXml(System.Xml.XmlReader reader)
		{
			Type type = GetType();
			string element = ElementName;
			if (Attribute.IsDefined(type, typeof(XmlRootAttribute), true))
			{
				XmlRootAttribute root = (XmlRootAttribute)Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute), true);
				element = root.ElementName;
			}
			reader.MoveToContent();
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == element)
				{
					string text = reader.ReadString();
					this.Add(text);
				}
			}
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		internal void WriteXml(System.Xml.XmlWriter writer)
		{
			Type type = GetType();
			string element = ElementName;
			if (Attribute.IsDefined(type, typeof(XmlRootAttribute), true))
			{
				XmlRootAttribute root = (XmlRootAttribute)Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute), true);
				element = root.ElementName;
			}
			foreach (string item in base.Items)
			{
				writer.WriteElementString(element, item);
			}
		}

		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 System.Xml.Serialization.XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>System.Xml.Schema.XmlSchema，描述由 System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter) 
		/// 方法产生并由 System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader) 方法使用的对象的 XML 表示形式。</returns>
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
