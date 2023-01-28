using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.DataEntities;
using Basic.Configuration;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace Basic.Collections
{
	/// <summary>
	/// 表示实体类集合
	/// </summary>
	public sealed class DataEntityElementCollection : AbstractCollection<DataEntityElement>, INotifyPropertyChanged,
		IEnumerable<DataEntityElement>, INotifyCollectionChanged, IXmlSerializable
	{
		#region 实体定义字段
		internal const string XmlElementName = "DataEntityElements";
		internal const string NamespaceAttribute = "Namespace";
		#endregion

		private readonly PersistentConfiguration persistentConfiguration;
		/// <summary>
		/// 初始化 AbstractCustomTypeDescriptor 类实例。
		/// </summary>
		internal DataEntityElementCollection(PersistentConfiguration persistent)
			: base(persistent, null, null) { persistentConfiguration = persistent; }

		/// <summary>
		/// 初始化 DesignTableInfo 类实例
		/// </summary>
		/// <param name="persistent">包含此类实例的 PersistentConfiguration 类实例。</param>
		/// <param name="prefix">Xml文档元素前缀。</param>
		/// <param name="elementns">Xml文档元素命名空间。</param>
		internal DataEntityElementCollection(PersistentConfiguration persistent, string prefix, string elementns)
			: base(persistent, prefix, elementns) { persistentConfiguration = persistent; }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>表示将实体模型移动位置</summary>
		/// <param name="item"></param>
		/// <param name="offset"></param>
		internal void Move(DataEntityElement item, int offset)
		{
			int index = IndexOf(item);
			if (index >= 0) { MoveItem(index, index + offset); }
		}

		/// <summary>表示将实体模型移动位置</summary>
		/// <param name="item"></param>
		internal void MoveToFirst(DataEntityElement item)
		{
			int index = IndexOf(item);
			if (index >= 0) { MoveItem(index, 0); }
		}

		/// <summary>表示将实体模型移动位置</summary>
		/// <param name="item"></param>
		internal void MoveToLast(DataEntityElement item)
		{
			int index = IndexOf(item);
			if (index >= 0) { MoveItem(index, Items.Count - 1); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, Basic.DataEntities.DataEntityElement item)
		{
			string name = this.GetKey(item);
			if (base.ContainsKey(name)) { item.Name = string.Concat(item.Name, "_", this.Count); }
			base.InsertItem(index, item);
		}

		private string _Namespace = null;
		/// <summary>
		/// 获取或设置当前配置文件生成代码时使用的命名空间。
		/// </summary>
		/// <value>当前配置文件生成代码时使用的命名空间，默认值为空字符串。</value>
		[Basic.Designer.PersistentDescription("PropertyDescription_EntityNamespace")]
		public string Namespace
		{
			get { return _Namespace; }
			set
			{
				if (_Namespace != value)
				{
					_Namespace = value;
					base.OnPropertyChanged("Namespace");
				}
			}
		}

		private string _OldNamespace = null;
		/// <summary>
		/// 
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public string OldNamespace { get { return _OldNamespace; } }

		/// <summary>
		/// 判断命名空间自上次保存后是否已更改。
		/// </summary>
		public bool NamespaceChanged { get { return _OldNamespace != _Namespace; } }

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(DataEntityElement item) { return item.EntityName; }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteXml(System.Xml.XmlWriter writer)
		{
			base.WriteXml(writer);
			_OldNamespace = _Namespace;
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == NamespaceAttribute) { _Namespace = value; _OldNamespace = _Namespace; return true; }
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionEnum">表示数据库连接类型</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString(NamespaceAttribute, _Namespace);
			base.WriteAttribute(writer);
		}
		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadChildContent(System.Xml.XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == DataEntityElement.XmlElementName)
				{
					DataEntityElement element = new DataEntityElement(persistentConfiguration);
					element.ReadXml(reader.ReadSubtree());
					this.Add(element);
				}
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			DataEntityElement item = base[index];
			item.DataCommands.Clear();
			base.RemoveItem(index);
		}
	}
}
