using Basic.Configuration;
using Basic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Designer
{
	/// <summary>
	/// 资源转换器名称
	/// </summary>
	public sealed class MessageInfo : AbstractCustomTypeDescriptor
	{
		private readonly PersistentConfiguration _Persistent;
		/// <summary>
		/// 采用项目信息初始化 MessageInfo 类实例。
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fileName"></param>
		public MessageInfo(PersistentConfiguration persistent, string name, string fileName)
			: base(persistent)
		{
			_Persistent = persistent;
			_ConverterName = name;
			_FileName = fileName;
		}

		private string _ConverterName = null;

		/// <summary>
		/// 获取或设置资源转换器名称
		/// </summary>
		public string ConverterName
		{
			get { return _ConverterName; }
			set
			{
				_ConverterName = value;
				RaisePropertyChanged("ConverterName");
			}
		}

		private string _GroupName = null;

		/// <summary>获取或设置资源组名称</summary>
		public string GroupName
		{
			get { return _GroupName; }
			set
			{
				_GroupName = value;
				RaisePropertyChanged("GroupName");
			}
		}

		private string _PublicGroupName = null;

		/// <summary>获取或设置资源组名称</summary>
		public string PublicGroupName
		{
			get { return _PublicGroupName; }
			set { _PublicGroupName = value; RaisePropertyChanged("PublicGroupName"); }
		}

		private string _FileName = null;
		/// <summary>
		/// 获取或设置本地化资源管理文件相对名称
		/// </summary>
		public string FileName
		{
			get { return _FileName; }
			set
			{
				_FileName = value;
				RaisePropertyChanged("FileName");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _ConverterName;
		}

		/// <summary>
		/// 判断当前项目信息是否为空。
		/// </summary>
		public bool IsEmpty { get { return string.IsNullOrWhiteSpace(_ConverterName); } }

		/// <summary>
		/// 判断当前项目信息是否为空。
		/// </summary>
		public bool NotEmpty { get { return !string.IsNullOrWhiteSpace(_ConverterName); } }

		internal const string XmlElementName = "MessageConverter";
		internal const string NameAttribute = "Name";
		internal const string GroupAttribute = "Group";
		internal const string PublicAttribute = "Public";
		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return XmlElementName; }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return _Persistent.ElementNamespace; } }

		/// <summary>
		/// 获取当前节点元素前缀
		/// </summary>
		protected internal override string ElementPrefix { get { return _Persistent.ElementPrefix; } }

		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return typeof(MessageInfo).Name; }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == NameAttribute) { _ConverterName = value; return true; }
			else if (name == PublicAttribute) { _PublicGroupName = value; return true; }
			else if (name == GroupAttribute) { _GroupName = value; if (string.IsNullOrWhiteSpace(_PublicGroupName)) { _PublicGroupName = value; } return true; }
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			_FileName = reader.ReadString();
			return true;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString(NameAttribute, _ConverterName);
			writer.WriteAttributeString(GroupAttribute, _GroupName);
			writer.WriteAttributeString(PublicAttribute, _PublicGroupName);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			writer.WriteValue(_FileName);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(System.Xml.XmlWriter writer, ConnectionTypeEnum connectionType) { }
	}
}
