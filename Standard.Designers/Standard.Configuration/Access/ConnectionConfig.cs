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
	/// ���ݿ�������Ϣ
	/// </summary>
	[Serializable, ToolboxItem(false)]
	public sealed class ConnectionConfig : Dictionary<string, string>, ISerializable, IXmlSerializable
	{
		/// <summary>
		/// ʹ�������ļ���Ϣ����ʼ��һ��ConnectinConfigʵ����
		/// </summary>
		/// <param name="connection">�����ļ���Ϣ</param>
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
		/// ʹ�ò��������Ĺ��캯����ʼ��һ��ConnectinConfigʵ����
		/// </summary>
		internal ConnectionConfig()
			: base(10)
		{
			this.Enabled = true;
			this.ConnectionType = ConnectionType.SqlConnection;
			this.DataBaseType = DataBaseType.SqlServer;
		}

		/// <summary>
		/// ʹ�ò��������Ĺ��캯����ʼ��һ��ConnectinConfigʵ����
		/// </summary>
		/// <param name="connectionString">���ݿ������ַ���</param>
		/// <param name="type">�������ݿ�����</param>
		public ConnectionConfig(ConnectionType type, string connectionString)
			: base(0)
		{
			ConnectionString = connectionString;
			this.ConnectionType = type;
		}

		/// <summary>
		/// ���ݿ���������
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// ���ݿ���������
		/// </summary>
		public bool Enabled { get; internal set; }

		/// <summary>
		/// ���ݿ���������
		/// </summary>
		public ConnectionType ConnectionType { get; internal set; }

		/// <summary>
		/// ���ݿ�����
		/// </summary>
		public DataBaseType DataBaseType { get; internal set; }

		/// <summary>
		/// �����ļ�����Ϊ����ʱ��Ŀ¼
		/// </summary>
		public string ConfigFolder { get; internal set; }

		/// <summary>
		/// ���ݿ��������
		/// </summary>
		public string DataSource { get; internal set; }

		/// <summary>
		/// ���ݿ��������½�û�
		/// </summary>
		public string UserID { get; internal set; }

		/// <summary>
		/// ���ݿ��������½�û�����
		/// </summary>
		public string Password { get; internal set; }

		/// <summary>
		/// ���ݿ������ַ���
		/// </summary>
		public string ConnectionString { get; set; }

		#region ISerializable Members
		/// <summary>
		/// ʹ�ý�ConfigInfo�������л������������� SerializationInfo��
		/// </summary>
		/// <param name="info">Ҫ������ݵ� SerializationInfo��</param>
		/// <param name="context">�����л���Ŀ�꣨��μ� StreamingContext����</param>
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
		/// ʹ�ý�ConfigInfo�������л������������� SerializationInfo��
		/// </summary>
		/// <param name="info">Ҫ������ݵ� SerializationInfo��</param>
		/// <param name="context">�����л���Ŀ�꣨��μ� StreamingContext����</param>
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
		/// ʹ�ý�ConfigInfo�������л������������� SerializationInfo��
		/// </summary>
		/// <param name="info">Ҫ������ݵ� SerializationInfo��</param>
		/// <param name="context">�����л���Ŀ�꣨��μ� StreamingContext����</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}
		#endregion

		#region �ӿ� IXmlSerializable Ĭ��ʵ��
		/// <summary>
		/// ���ݿ�����������Ϣ���� Ԫ������
		/// </summary>
		internal const string XmlElementName = "ConnectinConfig";
		/// <summary>
		/// ���ݿ��������� ��������
		/// </summary>
		internal const string NameAttribute = "Name";
		/// <summary>
		/// ���ݿ��������� ��������
		/// </summary>
		internal const string ValueAttribute = "Value";
		/// <summary>
		/// ���ݿ��������� ��������
		/// </summary>
		internal const string ConnectionTypeAttribute = "Type";
		/// <summary>
		/// Ŀ�����ݿ����� ��������
		/// </summary>
		internal const string DataBaseTypeAttribute = "DataBase";
		/// <summary>
		/// ���ݿ�����������Ϣ���� Ԫ������
		/// </summary>
		internal const string ChildElementName = "KeyPair";
		/// <summary>
		/// �����ļ�����Ϊ����ʱ��Ŀ¼ ��������
		/// </summary>
		internal const string ConfigFolderElement = "Folder";
		/// <summary>
		/// ���ݿ�������� ��������
		/// </summary>
		internal const string DataSourceElement = "DataSource";
		/// <summary>
		/// ���ݿ��������½�û� ��������
		/// </summary>
		internal const string UserIDElement = "UserID";

		/// <summary>
		/// ���ݿ��������½�û����� ��������
		/// </summary>
		internal const string PasswordElement = "Password";
		/// <summary>
		/// �Ӷ���� XML ��ʾ��ʽ��ȡ���ԡ�
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="value">����ֵ</param>
		/// <returns>������Դ��ڶ�ȡ�ɹ��򷵻�true�����򷵻�false���������ȡ��</returns>
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
		/// �Ӷ���� XML ��ʾ��ʽ���ɸö�����չ��Ϣ��
		/// </summary>
		/// <param name="reader">������н��з����л��� XmlReader ����</param>
		/// <returns>�жϵ�ǰ�����Ƿ��Ѿ���ȡ��ɣ������ȡ����򷵻�true�����򷵻�false��</returns>
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
		/// �Ӷ���� XML ��ʾ��ʽ���ɸö���
		/// </summary>
		/// <param name="reader">������н��з����л��� XmlReader ����</param>
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
		/// ������ת��Ϊ�� XML ��ʾ��ʽ��
		/// </summary>
		/// <param name="writer">����Ҫ���л�Ϊ�� XmlWriter ����</param>
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
		/// �˷����Ǳ����������벻Ҫʹ�á���ʵ�� IXmlSerializable �ӿ�ʱ��
		/// Ӧ�Ӵ˷������� null���� Visual Basic ��Ϊ Nothing����
		/// �����Ҫָ���Զ���ܹ���Ӧ�����Ӧ�� XmlSchemaProviderAttribute��
		/// </summary>
		/// <returns>XmlSchema�������� WriteXml ������������ ReadXml ����ʹ�õĶ���� XML ��ʾ��ʽ��</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

		/// <summary>
		/// �Ӷ���� XML ��ʾ��ʽ���ɸö���
		/// </summary>
		/// <param name="reader">������н��з����л��� XmlReader ����</param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

		/// <summary>
		/// ������ת��Ϊ�� XML ��ʾ��ʽ��
		/// </summary>
		/// <param name="writer">����Ҫ���л�Ϊ�� XmlWriter ����</param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { WriteXml(writer); }
		#endregion
	}

	/// <summary>
	/// ��ʾ Basic.Configuration.ConnectinConfig �༯��
	/// </summary>
	[Serializable, ToolboxItem(false), System.Xml.Serialization.XmlRoot(XmlElementName)]
	public class ConnectionConfigCollection : IEnumerable<ConnectionConfig>, IXmlSerializable
	{
		private ConnectionConfig _DefaultConnection;
		/// <summary>
		/// Ĭ�����ݿ�����������Ϣ
		/// </summary>
		public ConnectionConfig DefaultConnection { get { return _DefaultConnection; } }
		private readonly string _DefaultName;
		private readonly SortedList<string, ConnectionConfig> dictonary;

		#region ���캯��
		/// <summary>
		/// ��ʼ�� Basic.Configuration.ConnectionConfigCollection �����ʵ����
		/// </summary>
		/// <param name="defaultName">һ�� string ���͵Ĳ�������ʾϵͳ������Ĭ�����ݿ����ӵ����ơ�</param>
		public ConnectionConfigCollection(string defaultName)
		{
			_DefaultName = defaultName;
			dictonary = new SortedList<string, ConnectionConfig>();
		}

		/// <summary>
		/// ��ʼ�� Basic.Configuration.ConnectionConfigCollection �����ʵ�������������ָ���б��и��Ƶ�Ԫ�ء�
		/// </summary>
		/// <param name="defaultName">һ�� string ���͵Ĳ�������ʾϵͳ������Ĭ�����ݿ����ӵ����ơ�</param>
		/// <param name="collection">���и���Ԫ�صļ��ϡ�</param>
		/// <exception cref="System.ArgumentNullException">collection ��������Ϊ null��</exception>
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
		/// ������ָ������ֵ��Ԫ����ӵ� ConnectionConfigCollection �С�
		/// </summary>
		/// <param name="item">Ҫ��ӵ�Ԫ�ص�ֵ��</param>
		/// <returns>������ӳɹ���Ԫ�ء�</returns>
		public ConnectionConfig Add(ConnectionConfig item)
		{
			if (item == null) { throw new System.ArgumentNullException("item"); }
			if (dictonary.ContainsKey(item.Name)) { dictonary[item.Name] = item; }
			else { dictonary.Add(item.Name, item); }
			if (item.Name == _DefaultName) { _DefaultConnection = item; }
			return item;
		}

		/// <summary>
		///  �� ConnectionConfigCollection ���Ƴ�����Ԫ�ء�
		/// </summary>
		public void Clear() { dictonary.Clear(); }

		/// <summary>
		/// ȷ�� Basic.Configuration.ConnectionConfigCollection �Ƿ��������ָ������Ԫ�ء�
		/// </summary>
		/// <param name="connectionName">Ҫ�� Basic.Configuration.ConnectionConfigCollection �ж�λ�ļ���</param>
		/// <returns>��� Basic.Configuration.ConnectionConfigCollection ��������ָ������Ԫ�أ���Ϊ true������Ϊ false��</returns>
		/// <exception cref="System.ArgumentNullException">key Ϊ null��</exception>
		public bool ContainsKey(string connectionName)
		{
			return dictonary.ContainsKey(connectionName);
		}

		/// <summary>
		/// ��ȡ��������ָ���ļ��������ֵ��
		/// </summary>
		/// <param name="key">Ҫ��ȡ�����õ�ֵ�ļ���</param>
		/// <exception cref="System.ArgumentNullException">key Ϊ null��</exception>
		/// <returns>��ָ���ļ��������ֵ��</returns>
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
		/// ��ȡ��ָ���ļ��������ֵ��
		/// </summary>
		/// <param name="key">Ҫ��ȡ��ֵ�ļ���</param>
		/// <param name="value">���˷�������ʱ������ҵ�ָ�������򷵻���ü��������ֵ�����򣬽����� value ���������͵�Ĭ��ֵ��</param>
		/// <exception cref="System.ArgumentNullException">key Ϊ null��</exception>
		/// <returns>��� Basic.Configuration.ConnectionConfigCollection ��������ָ������Ԫ�أ���Ϊ true������Ϊ false��</returns>
		public bool TryGetValue(string key, out ConnectionConfig value)
		{
			return dictonary.TryGetValue(key, out value);
		}

		#region �ӿ� IXmlSerializable Ĭ��ʵ��
		/// <summary>
		/// ���ݿ�����������Ϣ���� Ԫ������
		/// </summary>
		internal const string XmlElementName = "ConnectinConfigs";
		/// <summary>
		/// ���ݿ�����������Ϣ���� Ԫ������
		/// </summary>
		internal const string DefaultElement = "Default";
		/// <summary>
		/// �Ӷ���� XML ��ʾ��ʽ���ɸö���
		/// </summary>
		/// <param name="reader">������н��з����л��� XmlReader ����</param>
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
		/// ������ת��Ϊ�� XML ��ʾ��ʽ��
		/// </summary>
		/// <param name="writer">����Ҫ���л�Ϊ�� XmlWriter ����</param>
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
		/// �˷����Ǳ����������벻Ҫʹ�á���ʵ�� IXmlSerializable �ӿ�ʱ��
		/// Ӧ�Ӵ˷������� null���� Visual Basic ��Ϊ Nothing����
		/// �����Ҫָ���Զ���ܹ���Ӧ�����Ӧ�� XmlSchemaProviderAttribute��
		/// </summary>
		/// <returns>XmlSchema�������� WriteXml ������������ ReadXml ����ʹ�õĶ���� XML ��ʾ��ʽ��</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

		/// <summary>
		/// �Ӷ���� XML ��ʾ��ʽ���ɸö���
		/// </summary>
		/// <param name="reader">������н��з����л��� XmlReader ����</param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

		/// <summary>
		/// ������ת��Ϊ�� XML ��ʾ��ʽ��
		/// </summary>
		/// <param name="writer">����Ҫ���л�Ϊ�� XmlWriter ����</param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { WriteXml(writer); }
		#endregion

		/// <summary>
		/// ����һ��ѭ�����ʼ��ϵ�ö������
		/// </summary>
		/// <returns><![CDATA[������ѭ�����ʼ��ϵ� System.Collections.Generic.IEnumerator<ConnectionConfig>��]]></returns>
		IEnumerator<ConnectionConfig> IEnumerable<ConnectionConfig>.GetEnumerator() { return dictonary.Values.GetEnumerator(); }

		/// <summary>
		/// ����һ��ѭ�����ʼ��ϵ�ö������
		/// </summary>
		/// <returns>������ѭ�����ʼ��ϵ� System.Collections.IEnumerator ����</returns>
		IEnumerator IEnumerable.GetEnumerator() { return dictonary.Values.GetEnumerator(); }
	}
}
