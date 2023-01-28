using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Basic.Designer;

namespace Basic.Localizations
{
	/// <summary>
	/// 资源本地化信息
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(ResourceConverter))]
	public sealed class LocalizationItem : SortedList<string, string>, IXmlSerializable, INotifyPropertyChanged,
		 IComparable<LocalizationItem>, IEquatable<LocalizationItem>
	{
		#region Xml Element Definition
		private readonly LocalizationCollection resourceCollection;
		internal const string XmlElementName = "resxData";
		private const string ValueElementName = "value";
		private const string CommentElementName = "comment";
		private const string NameAttribute = "name";
		private const string GroupAttribute = "group";
		#endregion

		/// <summary>
		/// 初始化 LocalizationResxResource 类实例
		/// </summary>
		/// <param name="localization">表示拥有此对象的 LocalizationCollection 类实例。</param>
		public LocalizationItem(LocalizationCollection localization) : this(localization, null, null, null) { }

		/// <summary>
		/// 初始化 LocalizationResxResource 类实例
		/// </summary>
		/// <param name="localization">表示拥有此对象的 LocalizationCollection 类实例。</param>
		/// <param name="name">资源名称</param>
		/// <param name="value">资源值</param>
		internal LocalizationItem(LocalizationCollection localization, string group, string name, string value)
		{
			resourceCollection = localization;
			_Group = group;
			_Name = name;
			_Value = value;
		}

		/// <summary>
		/// 判断当前本地化资源是否为空。
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrWhiteSpace(_Name) ||
					 string.IsNullOrWhiteSpace(_Group) || string.IsNullOrWhiteSpace(_Value);
			}
		}


		private bool _Created = true;
		/// <summary>
		/// 是否创建
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.Bindable(true)]
		public bool Created
		{
			get { return _Created; }
			set { if (_Created != value) { _Created = value; OnPropertyChanged("Created"); } }
		}

		private string _Name = null;
		/// <summary>
		/// 资源名称
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.Bindable(true)]
		[ResourceDescription("LocalizationResource_Name")]
		[ResourceCategory("LocalizationResource_Category")]
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					if (resourceCollection.ContainsName(value))
					{
						throw new Exception(string.Format("已经存在使用名称\"{0}\"的其他资源", value));
					}
					_Name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		private string _Group = null;
		/// <summary>
		/// 资源分组名称
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.Bindable(true)]
		[ResourceDescription("LocalizationResource_Group")]
		[ResourceCategory("LocalizationResource_Category")]
		public string Group
		{
			get { return _Group; }
			set
			{
				if (_Group != value)
				{
					_Group = value;
					OnPropertyChanged("Group");
				}
			}
		}

		private string _Value = null;
		/// <summary>
		/// 值
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.Bindable(true)]
		[ResourceDescription("LocalizationResource_Value")]
		[ResourceCategory("LocalizationResource_Category")]
		public string Value
		{
			get { return _Value; }
			set
			{
				if (_Value != value)
				{
					foreach (CultureInfo cultureInfo in resourceCollection.CultureInfos)
					{
						if (ContainsKey(cultureInfo.Name) && string.IsNullOrWhiteSpace(base[cultureInfo.Name]))
							base[cultureInfo.Name] = value;
						else if (ContainsKey(cultureInfo.Name) && base[cultureInfo.Name] == _Value)
							base[cultureInfo.Name] = value;
						else if (!ContainsKey(cultureInfo.Name))
							base[cultureInfo.Name] = value;
					}
					_Value = value;
					OnPropertyChanged("Value");
				}
			}
		}

		private string _Comment = null;
		/// <summary>
		/// 注释
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.Bindable(true)]
		[ResourceDescription("LocalizationResource_Comment")]
		[ResourceCategory("LocalizationResource_Category")]
		public string Comment
		{
			get { return _Comment; }
			set
			{
				if (_Comment != value)
				{
					_Comment = value;
					OnPropertyChanged("Comment");
				}
			}
		}

		/// <summary>
		/// 获取或设置与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取或设置其值的键。</param>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">已检索该属性，并且集合中不存在 key。</exception>
		/// <returns>与指定的键相关联的值。
		/// 如果找不到指定的键，则 get 操作会引发 System.Collections.Generic.KeyNotFoundException，
		/// 而 set 操作会创建一个使用指定键的新元素。</returns>
		public new string this[string key]
		{
			get
			{
				if (base.ContainsKey(key)) { return base[key]; }
				return null;
			}
			set
			{
				if (!base.ContainsKey(key))
				{
					base[key] = value; OnPropertyChanged(key);
				}
				else if (base.ContainsKey(key) && base[key] != value)
				{
					base[key] = value;
					OnPropertyChanged(key);
				}
			}
		}

		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
		/// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema() { return null; }

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
					string name = reader.LocalName;
					if (name == NameAttribute) { _Name = reader.GetAttribute(index); continue; }
					else if (name == GroupAttribute) { _Group = reader.GetAttribute(index); continue; }
				}
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == ValueElementName)
				{
					_Value = reader.ReadString(); continue;
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == CommentElementName)
				{
					_Comment = reader.ReadString(); continue;
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					foreach (CultureInfo cultureInfo in resourceCollection.CultureInfos)
					{
						if (cultureInfo.Name == reader.LocalName)
						{
							this[cultureInfo.Name] = reader.ReadString();
						}
					}
					continue;
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == XmlElementName)
				{
					break;
				}
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		internal void WriteXml(System.Xml.XmlWriter writer)
		{
			WriteXml(writer, false);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="writelocalation">是否要将本地化的资源一同写入。</param>
		internal void WriteXml(System.Xml.XmlWriter writer, bool writelocalation)
		{
			writer.WriteStartElement(XmlElementName);
			writer.WriteAttributeString(NameAttribute, _Name);
			writer.WriteAttributeString(GroupAttribute, _Group);
			if (!string.IsNullOrWhiteSpace(_Value))
				writer.WriteElementString(ValueElementName, _Value);
			if (!string.IsNullOrWhiteSpace(_Comment))
				writer.WriteElementString(CommentElementName, _Comment);
			if (writelocalation)
			{
				foreach (CultureInfo cultureInfo in resourceCollection.CultureInfos)
				{
					if (ContainsKey(cultureInfo.Name))
						writer.WriteElementString(cultureInfo.Name, base[cultureInfo.Name]);
				}
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { WriteXml(writer); }

		/// <summary>
		/// 在更改属性值时发生。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 引发 PropertyChanged 事件
		/// </summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal void OnPropertyChanged(string propertyName)
		{
			resourceCollection.OnContentChanged(EventArgs.Empty);
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 other 参数，则为 true；否则为 false。</returns>
		public bool Equals(LocalizationItem other)
		{
			return (other.Group == _Group && other.Name == _Name);
		}

		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。 
		/// 返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 
		/// 零此对象等于 other。 大于零此对象大于 other。</returns>
		public int CompareTo(LocalizationItem other)
		{
			if (other.Group == _Group) { return string.Compare(_Name, other.Name, true); }
			return string.Compare(_Group, other.Group, true);
		}
	}
}
