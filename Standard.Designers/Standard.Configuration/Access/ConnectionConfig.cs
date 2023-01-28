using Basic.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Collections;

namespace Basic.Configuration
{
	/// <summary>
	/// 数据库连接信息
	/// </summary>
	[Serializable, ToolboxItem(false)]
	public sealed class ConnectionConfig : Dictionary<string, string>, ISerializable, IXmlSerializable
	{
		/// <summary>
		/// 使用配置文件信息，初始化一个ConnectinConfig实例。
		/// </summary>
		/// <param name="connection">配置文件信息</param>
		internal ConnectionConfig(ConnectionElement connection)
			: base(10)
		{
			this.Name = connection.Name;
			this.Enabled = connection.Enabled;
			this.ConnectionType = connection.ConnectionType;
			string path = AppDomain.CurrentDomain.BaseDirectory;//.SetupInformation.ApplicationBase;
			foreach (ConnectionItem item in connection.Values)
			{
				if (item.Name == "ConfigFolder")
				{
					if (Directory.Exists(item.Value))
						this.ConfigFolder = Path.GetFullPath(item.Value);
					else
						this.ConfigFolder = Path.Combine(path, item.Value);
				}
				else if (item.Name == "DataSource" || item.Name == "Data Source")
					this.DataSource = item.Value;
				else if (item.Name == "UserID" || item.Name == "User ID")
					this.UserID = item.Value;
				else if (item.Name == "Password")
					this.Password = ConfigurationAlgorithm.Decryption(item.Value);
				else
					base[item.Name] = item.Value;
			}
			if (string.IsNullOrEmpty(this.ConfigFolder))
				this.ConfigFolder = path;
		}

		/// <summary>
		/// 使用不带参数的构造函数初始化一个ConnectinConfig实例。
		/// </summary>
		internal ConnectionConfig()
			: base(10)
		{
			this.Enabled = true;
			this.ConnectionType = ConnectionType.SqlConnection;
			this.DataBaseType = DataBaseType.SqlServer;
		}

		/// <summary>
		/// 使用不带参数的构造函数初始化一个ConnectinConfig实例。
		/// </summary>
		/// <param name="connectionString">数据库连接字符串</param>
		/// <param name="type">连接数据库类型</param>
		public ConnectionConfig(ConnectionType type, string connectionString)
			: base(0)
		{
			ConnectionString = connectionString;
			this.ConnectionType = type;
		}

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// 数据库连接类型
		/// </summary>
		public bool Enabled { get; internal set; }

		/// <summary>
		/// 数据库连接类型
		/// </summary>
		public ConnectionType ConnectionType { get; internal set; }

		/// <summary>
		/// 数据库类型
		/// </summary>
		public DataBaseType DataBaseType { get; internal set; }

		/// <summary>
		/// 配置文件保存为本地时的目录
		/// </summary>
		public string ConfigFolder { get; internal set; }

		/// <summary>
		/// 数据库服务器名
		/// </summary>
		public string DataSource { get; internal set; }

		/// <summary>
		/// 数据库服务器登陆用户
		/// </summary>
		public string UserID { get; internal set; }

		/// <summary>
		/// 数据库服务器登陆用户密码
		/// </summary>
		public string Password { get; internal set; }

		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		public string ConnectionString { get; set; }

		#region ISerializable Members
		/// <summary>
		/// 使用将ConfigInfo对象序列化所需的数据填充 SerializationInfo。
		/// </summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
		internal ConnectionConfig(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			ConfigFolder = info.GetString("ConfigFolder");
			ConnectionType = (ConnectionType)info.GetValue("ConnectionType", typeof(ConnectionType));
			DataBaseType = (DataBaseType)info.GetValue("DataBaseType", typeof(DataBaseType));
			DataSource = info.GetString("DataSource");
			UserID = info.GetString("UserID");
			Password = info.GetString("Password");
		}

		/// <summary>
		/// 使用将ConfigInfo对象序列化所需的数据填充 SerializationInfo。
		/// </summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ConfigFolder", ConfigFolder);
			info.AddValue("ConnectionType", ConnectionType, typeof(ConnectionType));
			info.AddValue("DataBaseType", DataBaseType, typeof(DataBaseType));
			info.AddValue("DataSource", DataSource);
			info.AddValue("UserID", UserID);
			info.AddValue("Password", Password);
		}

		/// <summary>
		/// 使用将ConfigInfo对象序列化所需的数据填充 SerializationInfo。
		/// </summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}
		#endregion

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 数据库连接配置信息顶级 元素名称
		/// </summary>
		internal const string XmlElementName = "ConnectinConfig";
		/// <summary>
		/// 数据库连接名称 属性名称
		/// </summary>
		internal const string NameAttribute = "Name";
		/// <summary>
		/// 数据库连接名称 属性名称
		/// </summary>
		internal const string ValueAttribute = "Value";
		/// <summary>
		/// 数据库连接类型 属性名称
		/// </summary>
		internal const string ConnectionTypeAttribute = "Type";
		/// <summary>
		/// 目标数据库类型 属性名称
		/// </summary>
		internal const string DataBaseTypeAttribute = "DataBase";
		/// <summary>
		/// 数据库连接配置信息顶级 元素名称
		/// </summary>
		internal const string ChildElementName = "KeyPair";
		/// <summary>
		/// 配置文件保存为本地时的目录 属性名称
		/// </summary>
		internal const string ConfigFolderElement = "Folder";
		/// <summary>
		/// 数据库服务器名 属性名称
		/// </summary>
		internal const string DataSourceElement = "DataSource";
		/// <summary>
		/// 数据库服务器登陆用户 属性名称
		/// </summary>
		internal const string UserIDElement = "UserID";

		/// <summary>
		/// 数据库服务器登陆用户密码 属性名称
		/// </summary>
		internal const string PasswordElement = "Password";
		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		internal bool ReadAttribute(string name, string value)
		{
			if (name == NameAttribute) { Name = value; }
			else if (name == ConnectionTypeAttribute)
			{
#if(NET35)
                 if (Enum.IsDefined(typeof(ConnectionType), value))
                {
                    ConnectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), value, true);
                }
#elif(NET40)
				ConnectionType connectionType = ConnectionType.SqlConnection;

				if (Enum.TryParse<ConnectionType>(value, true, out connectionType))
					ConnectionType = connectionType;
#endif
				return true;
			}
			else if (name == DataBaseTypeAttribute)
			{
#if(NET35)
                 if (Enum.IsDefined(typeof(DataBaseType), value))
                {
                    DataBaseType = (DataBaseType)Enum.Parse(typeof(DataBaseType), value, true);
                }
#elif (Net40)
                    DataBaseType databaseType = DataBaseType.SqlServer;
              if (Enum.TryParse<DataBaseType>(value, true, out databaseType))
                    DataBaseType = databaseType;
#endif

				return true;
			}
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		internal bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ChildElementName)
			{
				System.Xml.XmlReader subReader = reader.ReadSubtree();
				subReader.MoveToContent();
				if (subReader.HasAttributes)
				{
					string attributeName = null;
					for (int index = 0; index < subReader.AttributeCount; index++)
					{
						subReader.MoveToAttribute(index);
						string name = reader.LocalName, value = reader.GetAttribute(index);
						if (name == NameAttribute) { attributeName = value; }
						else if (attributeName != null && name == ValueAttribute)
						{
							if (attributeName == ConfigFolderElement) { ConfigFolder = value; }
							else if (attributeName == DataSourceElement) { DataSource = value; }
							else if (attributeName == UserIDElement) { UserID = value; }
							else if (attributeName == PasswordElement) { Password = value; }
							else { base[attributeName] = value; }
						}
					}
				}
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlElementName)
			{
				return true;
			}
			return false;
		}

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
					ReadAttribute(reader.LocalName, reader.GetAttribute(index));
				}
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				if (ReadContent(reader)) { break; }
			}
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		internal void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(XmlElementName);
			if (!string.IsNullOrEmpty(Name))
				writer.WriteAttributeString(NameAttribute, Name);
			writer.WriteAttributeString(ConnectionTypeAttribute, ConnectionType.ToString());
			writer.WriteAttributeString(DataBaseTypeAttribute, DataBaseType.ToString());
			if (!string.IsNullOrEmpty(ConfigFolder))
			{
				writer.WriteStartElement(ChildElementName);
				writer.WriteAttributeString(NameAttribute, ConfigFolderElement);
				writer.WriteAttributeString(ValueAttribute, ConfigFolder);
				writer.WriteEndElement();
			}
			if (string.IsNullOrEmpty(DataSource))
			{
				writer.WriteStartElement(ChildElementName);
				writer.WriteAttributeString(NameAttribute, DataSourceElement);
				writer.WriteAttributeString(ValueAttribute, DataSource);
				writer.WriteEndElement();
			}
			if (string.IsNullOrEmpty(UserID))
			{
				writer.WriteStartElement(ChildElementName);
				writer.WriteAttributeString(NameAttribute, UserIDElement);
				writer.WriteAttributeString(ValueAttribute, UserID);
				writer.WriteEndElement();
			}
			if (string.IsNullOrEmpty(Password))
			{
				writer.WriteStartElement(ChildElementName);
				writer.WriteAttributeString(NameAttribute, PasswordElement);
				writer.WriteAttributeString(ValueAttribute, Password);
				writer.WriteEndElement();
			}
			foreach (var keyPair in this)
			{
				writer.WriteStartElement(ChildElementName);
				writer.WriteAttributeString(NameAttribute, keyPair.Key);
				writer.WriteAttributeString(ValueAttribute, keyPair.Value);
				writer.WriteEndElement();
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

	/// <summary>
	/// 表示 Basic.Configuration.ConnectinConfig 类集合
	/// </summary>
	[Serializable, ToolboxItem(false), System.Xml.Serialization.XmlRoot(XmlElementName)]
	public class ConnectionConfigCollection : IEnumerable<ConnectionConfig>, IXmlSerializable
	{
		private ConnectionConfig _DefaultConnection;
		/// <summary>
		/// 默认数据库连接配置信息
		/// </summary>
		public ConnectionConfig DefaultConnection { get { return _DefaultConnection; } }
		private readonly string _DefaultName;
		private readonly SortedList<string, ConnectionConfig> dictonary;

		#region 构造函数
		/// <summary>
		/// 初始化 Basic.Configuration.ConnectionConfigCollection 类的新实例。
		/// </summary>
		/// <param name="defaultName">一个 string 类型的参数，表示系统配置中默认数据库连接的名称。</param>
		public ConnectionConfigCollection(string defaultName)
		{
			_DefaultName = defaultName;
			dictonary = new SortedList<string, ConnectionConfig>();
		}

		/// <summary>
		/// 初始化 Basic.Configuration.ConnectionConfigCollection 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="defaultName">一个 string 类型的参数，表示系统配置中默认数据库连接的名称。</param>
		/// <param name="collection">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		public ConnectionConfigCollection(string defaultName, IEnumerable<ConnectionConfig> collection)
		{
			_DefaultName = defaultName;
			dictonary = new SortedList<string, ConnectionConfig>();
			foreach (ConnectionConfig config in collection)
			{

				dictonary[config.Name] = config;
			}

		}
		#endregion

		/// <summary>
		/// 将带有指定键和值的元素添加到 ConnectionConfigCollection 中。
		/// </summary>
		/// <param name="item">要添加的元素的值。</param>
		/// <returns>返回添加成功的元素。</returns>
		public ConnectionConfig Add(ConnectionConfig item)
		{
			if (item == null) { throw new System.ArgumentNullException("item"); }
			if (dictonary.ContainsKey(item.Name)) { dictonary[item.Name] = item; }
			else { dictonary.Add(item.Name, item); }
			if (item.Name == _DefaultName) { _DefaultConnection = item; }
			return item;
		}

		/// <summary>
		///  从 ConnectionConfigCollection 中移除所有元素。
		/// </summary>
		public void Clear() { dictonary.Clear(); }

		/// <summary>
		/// 确定 Basic.Configuration.ConnectionConfigCollection 是否包含具有指定键的元素。
		/// </summary>
		/// <param name="connectionName">要在 Basic.Configuration.ConnectionConfigCollection 中定位的键。</param>
		/// <returns>如果 Basic.Configuration.ConnectionConfigCollection 包含具有指定键的元素，则为 true；否则为 false。</returns>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		public bool ContainsKey(string connectionName)
		{
			return dictonary.ContainsKey(connectionName);
		}

		/// <summary>
		/// 获取或设置与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取或设置的值的键。</param>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		/// <returns>与指定的键相关联的值。</returns>
		public ConnectionConfig this[string key]
		{
			get
			{
				if (dictonary.ContainsKey(key))
				{
					return dictonary[key];
				}
				return null;
			}
		}

		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取的值的键。</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。</param>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		/// <returns>如果 Basic.Configuration.ConnectionConfigCollection 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public bool TryGetValue(string key, out ConnectionConfig value)
		{
			return dictonary.TryGetValue(key, out value);
		}

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 数据库连接配置信息顶级 元素名称
		/// </summary>
		internal const string XmlElementName = "ConnectinConfigs";
		/// <summary>
		/// 数据库连接配置信息顶级 元素名称
		/// </summary>
		internal const string DefaultElement = "Default";
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
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ConnectionConfig.XmlElementName)
				{
					ConnectionConfig item = new ConnectionConfig();
					item.ReadXml(reader.ReadSubtree());
					this.Add(item);
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlElementName) { break; }
			}
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		internal void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(XmlElementName);
			writer.WriteAttributeString(DefaultElement, _DefaultName);
			foreach (ConnectionConfig config in dictonary.Values)
			{
				config.WriteXml(writer);
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

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns><![CDATA[可用于循环访问集合的 System.Collections.Generic.IEnumerator<ConnectionConfig>。]]></returns>
		IEnumerator<ConnectionConfig> IEnumerable<ConnectionConfig>.GetEnumerator() { return dictonary.Values.GetEnumerator(); }

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
		IEnumerator IEnumerable.GetEnumerator() { return dictonary.Values.GetEnumerator(); }
	}
}
