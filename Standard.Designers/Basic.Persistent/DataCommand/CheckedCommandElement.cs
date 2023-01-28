using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Basic.Enums;
using Basic.Designer;
using System.Drawing.Design;
using Basic.Properties;
using System.ComponentModel;

namespace Basic.Configuration
{
	/// <summary>
	/// 表示抽象配置命令
	/// </summary>
	[System.ComponentModel.DisplayName(XmlElementName)]
	[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCheckCommand)]
	[Basic.Designer.PersistentDescription("PersistentDescription_CheckCommand")]
	public sealed class CheckedCommandElement : AbstractCommandElement, IXmlSerializable
	{
		/// <summary>
		/// 获取检测Transact-SQL 命令需要使用的参数名称。
		/// 必须和IDataCommand的DbParameters中存在的参数名对应。
		/// 如果有多个请使用英文状态下的逗号(,)分割。
		/// </summary>
		/// <value>返回参数名</value>
		private string Parameter { get; set; }

		#region Xml 节点名称常量
		/// <summary>
		/// 表示 Converter 属性
		/// </summary>
		internal const string ConverterAttribute = "Converter";
		/// <summary>
		/// 表示 ErrorCode 属性
		/// </summary>
		internal const string ErrorCodeAttribute = "ErrorCode";

		/// <summary>
		/// 表示 CheckExist 属性
		/// </summary>
		internal const string CheckExistAttribute = "CheckExist";

		/// <summary>
		/// 表示 SourceColumn 属性
		/// </summary>
		internal const string SourceColumnAttribute = "SourceColumn";

		/// <summary>
		/// 表示 PropertyName 属性
		/// </summary>
		internal const string PropertyNameAttribute = "PropertyName";

		/// <summary>
		/// 表示 CommandText 元素。
		/// </summary>
		internal const string CommandTextElement = "CommandText";

		/// <summary>
		/// 表示 Parameter 属性。
		/// </summary>
		internal const string ParameterAttribute = "Parameter";
		#endregion

		private readonly StaticCommandElement _StaticCommand;
		private readonly PersistentConfiguration _Persistent;
		/// <summary>
		/// 初始化 CheckCommandElement 类实例。
		/// </summary>
		/// <param name="staticCommand">拥有此检测命令的静态命令结构</param>
		internal CheckedCommandElement(StaticCommandElement staticCommand)
			: base(staticCommand) { _StaticCommand = staticCommand; _Persistent = staticCommand.Persistent; }

		//private string _Converter = null;
		///// <summary>
		///// 获取或设置检查命令执行失败时的错误编码
		///// </summary>
		///// <value>需要返回的错误编码，默认值为null</value>
		//[System.ComponentModel.DefaultValue(""), TypeConverter(typeof(MessageTypeConverter))]
		//[System.ComponentModel.Description("获取或设置检查命令执行失败时的错误编码转换器")]
		//[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		//public string Converter
		//{
		//	get { return _Converter; }
		//	set
		//	{
		//		if (_Converter != value)
		//		{
		//			_Converter = value;
		//			base.RaisePropertyChanged("Converter");
		//		}
		//	}
		//}

		private string _ErrorCode = null;
		/// <summary>
		/// 获取或设置检查命令执行失败时的错误编码
		/// </summary>
		/// <value>需要返回的错误编码，默认值为null</value>
		[System.ComponentModel.DefaultValue(""), TypeConverter(typeof(ErrorCodeConveter))]
		[System.ComponentModel.Description("获取或设置检查命令执行失败时的错误编码")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		public string ErrorCode
		{
			get { return _ErrorCode; }
			set
			{
				if (_ErrorCode != value)
				{
					base.OnPropertyChanging("ErrorCode");
					_ErrorCode = value;
					base.RaisePropertyChanged("ErrorCode");
				}
			}
		}

		/// <summary>
		/// 返回表示当前 Basic.Configuration.NewCommandElement 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 Basic.Configuration.NewCommandElement。</returns>
		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(Name))
				return typeof(CheckedCommandElement).Name;
			return Name;
		}

		private string _PropertyName = null;
		/// <summary>
		/// 获取或设置当前检测命令如果测试失败，则需要将失败信息对应为实体类中哪个属性
		/// </summary>
		[System.ComponentModel.DefaultValue(""), TypeConverter(typeof(PropertyNameConverter))]
		[System.ComponentModel.Description("获取或设置当前检测命令如果测试失败，则需要将失败信息对应为实体类中哪个属性")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		public string PropertyName
		{
			get { return _PropertyName; }
			set
			{
				if (_PropertyName != value)
				{
					base.OnPropertyChanging("PropertyName");
					_PropertyName = value;
					base.RaisePropertyChanged("PropertyName");
				}
			}
		}

		private bool _CheckExist = true;
		/// <summary>
		/// 获取或设置要对当前检测命令是检测数据存在还是检测数据不存在。
		/// </summary>
		[System.ComponentModel.DefaultValue(true)]
		[System.ComponentModel.Description("获取或设置要对当前检测命令是检测数据存在还是检测数据不存在")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		public bool CheckExist
		{
			get { return _CheckExist; }
			set
			{
				if (_CheckExist != value)
				{
					base.OnPropertyChanging("CheckExist");
					_CheckExist = value;
					base.RaisePropertyChanged("CheckExist");
				}
			}
		}

		private string _CommandText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程")]
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Editor(typeof(CommandTextEditor), typeof(UITypeEditor))]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		public string CommandText
		{
			get { return _CommandText; }
			set
			{
				if (_CommandText != value)
				{
					base.OnPropertyChanging("CommandText");
					_CommandText = value;
					base.RaisePropertyChanged("CommandText");
				}
			}
		}
		internal const string XmlElementName = "CheckCommand";
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
			if (name == ParameterAttribute) { Parameter = value; return true; }
			else if (name == CheckExistAttribute) { _CheckExist = Convert.ToBoolean(value); return true; }
			else if (name == PropertyNameAttribute) { _PropertyName = value; return true; }
			else if (name == SourceColumnAttribute) { _PropertyName = value; return true; }
			//else if (name == ConverterAttribute) { _Converter = value; return true; }
			else if (name == ErrorCodeAttribute) { _ErrorCode = value; return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			if (!_CheckExist)
				writer.WriteAttributeString(CheckExistAttribute, Convert.ToString(_CheckExist));
			if (!string.IsNullOrWhiteSpace(_PropertyName))
				writer.WriteAttributeString(PropertyNameAttribute, _PropertyName);
			//if (!string.IsNullOrEmpty(_Converter))
			//	writer.WriteAttributeString(ConverterAttribute, _Converter);
			if (!string.IsNullOrEmpty(_ErrorCode))
				writer.WriteAttributeString(ErrorCodeAttribute, _ErrorCode);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Text && reader.LocalName == string.Empty)//兼容5.0旧版结构信息
			{
				_CommandText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CommandTextElement)//兼容5.0新版结构信息
			{
				_CommandText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == ElementName)
			{
				return true;
			}
			return base.ReadContent(reader);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(CommandTextElement);
			writer.WriteCData(CommandText);//写CData
			writer.WriteEndElement();
			base.WriteContent(writer);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
			writer.WriteStartElement(XmlElementName);
			if (CommandType != System.Data.CommandType.Text)
				writer.WriteAttributeString(CommandTypeAttribute, CommandType.ToString());
			if (CommandTimeout != 30)
				writer.WriteAttributeString(CommandTimeoutAttribute, CommandTimeout.ToString());
			if (!_CheckExist)
				writer.WriteAttributeString(CheckExistAttribute, Convert.ToString(_CheckExist));
			if (!string.IsNullOrWhiteSpace(_PropertyName))
				writer.WriteAttributeString(PropertyNameAttribute, _PropertyName);
			if (!string.IsNullOrEmpty(_Persistent.MessageConverter.ConverterName))
				writer.WriteAttributeString(ConverterAttribute, _Persistent.MessageConverter.ConverterName);
			if (!string.IsNullOrEmpty(_ErrorCode))
				writer.WriteAttributeString(ErrorCodeAttribute, _ErrorCode);

			writer.WriteStartElement(CommandTextElement);
			writer.WriteCData(CreateCommandText(_CommandText, connectionType));//写CData
			writer.WriteEndElement();

			base.GenerateConfiguration(writer, connectionType);
			writer.WriteEndElement();
		}
	}
}
