using System;
using System.Collections.Specialized;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using Basic.Collections;
using Basic.Designer;
using Basic.Enums;
using Basic.Properties;

namespace Basic.Configuration
{
	/// <summary>表示抽象配置命令</summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public abstract class AbstractCommandElement : AbstractCustomTypeDescriptor, IXmlSerializable, INotifyCollectionChanged
	{
		#region Xml 节点名称常量
		/// <summary>
		/// 表示 Name 属性。
		/// </summary>
		protected internal const string NameAttribute = "Name";

		/// <summary>
		/// 表示 CommandType 属性
		/// </summary>
		internal protected const string CommandTypeAttribute = "CommandType";

		/// <summary>
		/// 表示 CommandTimeout 属性
		/// </summary>
		internal protected const string CommandTimeoutAttribute = "CommandTimeout";

		/// <summary>
		/// 表示 Parameters 元素。
		/// </summary>
		internal protected const string ParametersElement = "Parameters";
		#endregion

		/// <summary>获取或设置异步命令名称</summary>
		protected string AsyncName { get { return string.Concat(_Name, "Async"); } }

		private string _Name = string.Empty;
		/// <summary>
		/// 获取或设置命令名称
		/// </summary>
		[System.ComponentModel.Description("获取或设置命令名称"), System.ComponentModel.DefaultValue("")]
		[Basic.Designer.PersistentDisplay("DisplayName_CommandName")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		public virtual string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					base.RaisePropertyChanged("Name");
					base.RaisePropertyChanged("ConfigurationName");
				}
			}
		}

		/// <summary>
		/// 获取命令在配置文件中的名称
		/// </summary>
		[System.ComponentModel.Description("获取命令在配置文件中的名称"), System.ComponentModel.DefaultValue("")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		internal protected string ConfigurationName
		{
			get { return string.Concat(Name, "Config"); }
		}

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return ElementName; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return Name; }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return null; } }

		private readonly CommandParameterCollection _Parameters;
		/// <summary>
		/// 初始化 AbstractCommandElement 类实例。
		/// </summary>
		protected AbstractCommandElement(AbstractCustomTypeDescriptor nofity)
			: base(nofity)
		{
			_Parameters = new CommandParameterCollection(this);
			_Parameters.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
		}

		/// <summary>
		/// 当集合更改时发生。
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// 引发 CollectionChanged 事件
		/// </summary>
		/// <param name="sender">引发事件的对象。</param>
		/// <param name="e">有关事件的信息。</param>
		protected internal void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			base.RaisePropertyChanged("ParametersVisibility");
			base.RaisePropertyChanged("Parameters");
			if (CollectionChanged != null)
				CollectionChanged(sender, e);
		}

		/// <summary>
		/// 获取参数占位符
		/// </summary>
		/// <param name="connectionType"></param>
		/// <returns></returns>
		protected internal string ParameterPlaceholder(ConnectionTypeEnum connectionType)
		{
			if (connectionType == ConnectionTypeEnum.ORACLE && _CommandType == CommandType.Text)
				return ":";
			else if (connectionType == ConnectionTypeEnum.ORACLE && _CommandType == CommandType.StoredProcedure)
				return string.Empty;
			//else if (connectionType == ConnectionTypeEnum.SQLSERVER)
			//	return "@";
			//else if (connectionType == ConnectionTypeEnum.MYSQL)
			//	return "@";
			//else if (connectionType == ConnectionTypeEnum.NPGSQL)
			//	return "@";
			//else if (connectionType == ConnectionTypeEnum.DB2)
			//	return "@";
			return "@";
		}

		/// <summary>
		/// 获取参数占位符
		/// </summary>
		/// <param name="connectionType"></param>
		/// <returns></returns>
		protected internal string CreateCommandText(string commandText, ConnectionTypeEnum connectionType)
		{
			commandText = commandText.ToUpperInvariant();
			if (connectionType == ConnectionTypeEnum.ORACLE && _CommandType == CommandType.Text)
			{
				commandText = commandText.Replace("{%GUID%}", "SYSGUID()");
				commandText = commandText.Replace("NEWID()", "SYSGUID()");
				commandText = commandText.Replace("UUID()", "SYSGUID()");
				commandText = commandText.Replace("{%NOW%}", "SYSDATE()");
				commandText = commandText.Replace("GETDATE()", "SYSDATE()");
				commandText = commandText.Replace("NOW()", "SYSDATE()");
				commandText = commandText.Replace("DBO.", "");
				commandText = commandText.Replace("@", ":");
				//commandText = commandText.Replace("CURRENT_TIMESTAMP", "");
				commandText = commandText.Replace("{%", ParameterPlaceholder(connectionType));
				commandText = commandText.Replace("%}", "");
			}
			else if (connectionType == ConnectionTypeEnum.DB2)
			{
				commandText = commandText.Replace("{%GUID%}", "UUID()");
				commandText = commandText.Replace("SYSGUID()", "UUID()");
				commandText = commandText.Replace("NEWID()", "UUID()");

				commandText = commandText.Replace("{%NOW%}", "SYSDATE()");
				commandText = commandText.Replace("GETDATE()", "SYSDATE()");
				commandText = commandText.Replace("NOW()", "SYSDATE()");
				commandText = commandText.Replace("DBO.", "");
				commandText = commandText.Replace(":", "@");
				commandText = commandText.Replace("{%", ParameterPlaceholder(connectionType));
				commandText = commandText.Replace("%}", "");
			}
			else if (connectionType == ConnectionTypeEnum.SQLSERVER)
			{
				commandText = commandText.Replace("{%GUID%}", "NEWID()");
				commandText = commandText.Replace("SYSGUID()", "NEWID()");
				commandText = commandText.Replace("UUID()", "NEWID()");

				commandText = commandText.Replace("{%NOW%}", "GETDATE()");
				commandText = commandText.Replace("SYSDATE()", "GETDATE()");
				commandText = commandText.Replace("NOW()", "GETDATE()");
				commandText = commandText.Replace(":", "@");
				commandText = commandText.Replace("{%", ParameterPlaceholder(connectionType));
				commandText = commandText.Replace("%}", "");
			}
			else if (connectionType == ConnectionTypeEnum.MYSQL)
			{
				commandText = commandText.Replace("{%GUID%}", "UUID()");
				commandText = commandText.Replace("SYSGUID()", "UUID()");
				commandText = commandText.Replace("NEWID()", "UUID()");
				commandText = commandText.Replace("DBO.SYSF_STRINGJOIN(", "GROUP_CONCAT(");
				commandText = commandText.Replace("ISNULL(", "IFNULL(");

				commandText = commandText.Replace("COUNT(1)", "CAST(COUNT(1) AS SIGNED)");
				commandText = commandText.Replace("COUNT(*)", "CAST(COUNT(*) AS SIGNED)");

				commandText = commandText.Replace("{%NOW%}", "CURRENT_TIMESTAMP(3)");
				commandText = commandText.Replace("SYSDATE()", "CURRENT_TIMESTAMP(3)");
				commandText = commandText.Replace("GETDATE()", "CURRENT_TIMESTAMP(3)");
				commandText = commandText.Replace("DBO.", "");
				commandText = commandText.Replace(":", "@");
				commandText = commandText.Replace("{%", ParameterPlaceholder(connectionType));
				commandText = commandText.Replace("%}", "");
			}
			else if (connectionType == ConnectionTypeEnum.NPGSQL)
			{
				commandText = commandText.Replace("{%GUID%}", "UUID()");
				commandText = commandText.Replace("SYSGUID()", "UUID()");
				commandText = commandText.Replace("NEWID()", "UUID()");

				commandText = commandText.Replace("{%NOW%}", "NOW()");
				commandText = commandText.Replace("SYSDATE()", "NOW()");
				commandText = commandText.Replace("GETDATE()", "NOW()");
				commandText = commandText.Replace("DBO.", "");
				commandText = commandText.Replace(":", "@");
				commandText = commandText.Replace("{%", ParameterPlaceholder(connectionType));
				commandText = commandText.Replace("%}", "");
			}
			return commandText;
		}

		/// <summary>
		/// 创建命令参数名称
		/// </summary>
		/// <param name="parameterName">参数名称</param>
		/// <param name="connectionType">创建数据库参数的数据库连接。</param>
		/// <returns>返回促昂件成功的参数名称包含参数符号。</returns>
		protected internal string CreateParameterName(string parameterName,
			ConnectionTypeEnum connectionType = ConnectionTypeEnum.Default)
		{
			if (parameterName == null || parameterName.Length == 0) { return null; }
			if (connectionType == ConnectionTypeEnum.ORACLE && _CommandType == CommandType.Text)
				return string.Concat(":", parameterName);
			else if (connectionType == ConnectionTypeEnum.ORACLE && _CommandType == CommandType.StoredProcedure)
				return parameterName;
			else if (connectionType == ConnectionTypeEnum.SQLSERVER)
				return string.Concat("@", parameterName);
			else if (connectionType == ConnectionTypeEnum.MYSQL)
				return string.Concat("@", parameterName);
			else if (connectionType == ConnectionTypeEnum.DB2)
				return string.Concat("@", parameterName);
			else if (connectionType == ConnectionTypeEnum.NPGSQL)
				return string.Concat("@", parameterName);
			if (connectionType == ConnectionTypeEnum.Default)
			{
				if (!parameterName.StartsWith("{%")) { parameterName = string.Concat("{%", parameterName); }
				if (!parameterName.EndsWith("{%")) { parameterName = string.Concat(parameterName, "%}"); }
				return parameterName;
			}
			return string.Concat("@", parameterName);
		}

		private CommandType _CommandType = CommandType.Text;
		/// <summary>
		/// 获取或设置一个值，该值指示如何解释 CommandText 属性。
		/// </summary>
		/// <value>CommandType 值之一，默认值为 Text。</value>
		[System.ComponentModel.Description("获取或设置一个值，该值指示如何解释 CommandText 属性")]
		[System.ComponentModel.DefaultValue(typeof(CommandType), "Text")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		public CommandType CommandType
		{
			get { return _CommandType; }
			set
			{
				if (_CommandType != value)
				{
					base.OnPropertyChanging("CommandType");
					_CommandType = value;
					base.RaisePropertyChanged("CommandType");
					base.RaisePropertyChanged("Source");
				}
			}
		}

		private int _CommandTimeout = 30;
		/// <summary>
		/// 获取或设置在终止执行命令的尝试并生成错误之前的等待时间。
		/// </summary>
		/// <value>等待命令执行的时间（以秒为单位）,默认为 30 秒。</value>
		[System.ComponentModel.Description("获取或设置在终止执行命令的尝试并生成错误之前的等待时间")]
		[System.ComponentModel.DefaultValue(typeof(int), "30")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		public virtual int CommandTimeout
		{
			get { return _CommandTimeout; }
			set
			{
				if (_CommandTimeout != value)
				{
					base.OnPropertyChanging("CommandTimeout");
					_CommandTimeout = value;
					base.RaisePropertyChanged("CommandTimeout");
				}
			}
		}

		/// <summary>获取命令参数集合</summary>
		public CommandParameterCollection Parameters { get { return _Parameters; } }

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == NameAttribute)
			{
				Name = value.Replace("Config", ""); return true;
			}
			else if (name == CommandTypeAttribute)
			{
				return Enum.TryParse<CommandType>(value, true, out _CommandType);
			}
			else if (name == CommandTimeoutAttribute)
			{
				_CommandTimeout = Convert.ToInt32(value); return true;
			}
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString(NameAttribute, ConfigurationName);
			if (_CommandType != System.Data.CommandType.Text)
				writer.WriteAttributeString(CommandTypeAttribute, CommandType.ToString());
			if (_CommandTimeout != 30)
				writer.WriteAttributeString(CommandTimeoutAttribute, _CommandTimeout.ToString());
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		internal protected override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ParametersElement)
			{
				System.Xml.XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())  //读取所有静态命令节点信息
				{
					if (reader2.NodeType == XmlNodeType.Element && reader2.LocalName == CommandParameter.XmlElementName)
					{
						CommandParameter param = new CommandParameter(this);
						System.Xml.XmlReader paramReader = reader2.ReadSubtree();
						param.ReadXml(paramReader);
						_Parameters.Add(param);
					}
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.LocalName == ParametersElement)
						break;
				}
			}
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			if (_Parameters.Count == 0) { return; }
			writer.WriteStartElement(ParametersElement);
			foreach (CommandParameter param in _Parameters)
			{
				param.WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
			if (_Parameters.Count == 0) { return; }
			writer.WriteStartElement(ParametersElement);
			foreach (CommandParameter param in _Parameters)
			{
				param.GenerateConfiguration(writer, connectionType);
			}
			writer.WriteEndElement();
		}
	}
}
