using Basic.Configuration;
using Basic.DataAccess;
using Basic.Designer;
using Basic.Enums;
using System;
using System.Drawing.Design;


namespace Basic.DataEntities
{
	/// <summary>
	/// 表示属性生成元素
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(DisplayNameConverter))]
	[PersistentCategory("PersistentCategory_CodeGenerator"), PersistentDescription("PersistentGenerator_Description")]
	public sealed class PersistentGeneratorElement : AbstractCustomTypeDescriptor
	{
		private readonly PersistentConfiguration _PersistentConfiguration;
		private readonly SupportDatabasesConverter _Converter;
		internal const string XmlElementName = "PersistentGenerator";
		internal const string ModifierAttribute = "Modifier";
		internal const string GenerateAttribute = "Generate";
		internal const string NamingAttribute = "Naming";
		internal const string AccessAttribute = "Access";
		internal const string ApplyConnectionAttribute = "ac";
		internal const string ResourceAttribute = "ResxMode";
		internal const string ContextAttribute = "Context";
		internal const string SupportAttribute = "Support";
		internal const string BuildSqlfAttribute = "BuildSqlf";
		internal const string BuildOrafAttribute = "BuildOraf";
		internal readonly string XmlNamespace;
		internal readonly string XmlPrefix;
		/// <summary>
		/// 初始化 PersistentGeneratorElement 类实例。
		/// </summary>
		/// <param name="persistent">包含此类实例的 PersistentConfiguration 类实例。</param>
		/// <param name="prefix">Xml文档元素前缀。</param>
		/// <param name="elementns">Xml文档元素命名空间。</param>
		internal PersistentGeneratorElement(PersistentConfiguration pc, string prefix, string elementns)
			: base(pc)
		{
			_PersistentConfiguration = pc;
			_Converter = new SupportDatabasesConverter();
			//_DatabaseType = ConnectionTypeEnum.SQLSERVER;
			_SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.SQLSERVER };
			XmlNamespace = elementns;
			XmlPrefix = prefix;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _BaseAccess.ToString();
		}

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return XmlNamespace; } }

		/// <summary>
		/// 获取当前节点元素前缀
		/// </summary>
		protected internal override string ElementPrefix { get { return XmlPrefix; } }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return typeof(PersistentGeneratorElement).Name; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return XmlElementName; }

		private NamingRules _NamingRule = NamingRules.DefaultCase;
		/// <summary>获取或设置实体模型命名规则</summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.DefaultValue(typeof(NamingRules), "DefaultCase"), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_NamingRule")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_NamingRule")]
		public NamingRules NamingRule
		{
			get { return _NamingRule; }
			set
			{
				if (_NamingRule != value)
				{
					_NamingRule = value;
					RaisePropertyChanged("NamingRule");
				}
			}
		}

		private GenerateActionEnum _GenerateMode = GenerateActionEnum.Multiple;
		/// <summary>
		/// Gets or sets the entity project folder name.
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.DefaultValue(typeof(GenerateActionEnum), "Multiple"), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_GenerateMode")]
		public GenerateActionEnum GenerateMode
		{
			get { return _GenerateMode; }
			set
			{
				if (_GenerateMode != value)
				{
					_GenerateMode = value;
					RaisePropertyChanged("GenerateMode");
				}
			}
		}

		private ClassModifierEnum _Modifier = ClassModifierEnum.Internal;
		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(typeof(ClassModifierEnum), "Internal"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_Modifier")]
		public ClassModifierEnum Modifier
		{
			get { return _Modifier; }
			set
			{
				if (_Modifier != value)
				{
					_Modifier = value;
					RaisePropertyChanged("Modifier");
				}
			}
		}

		private string _BaseAccess = "AbstractAccess";
		/// <summary>
		/// 获取或设置当前数据持久类基类。
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue("AbstractAccess"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_BaseAccess")]
		[System.ComponentModel.Editor(typeof(BaseAccessSelector), typeof(UITypeEditor))]
		public string BaseAccess
		{
			get { return _BaseAccess; }
			set
			{
				if (_BaseAccess != value)
				{
					_BaseAccess = value;
					RaisePropertyChanged("BaseAccess");
				}
			}
		}

		private bool _ApplyConnection = true;
		/// <summary>
		/// 获取或设置当前数据库上下文类是否指定连接名称。
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_ApplyConnection")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_ApplyConnection")]
		public bool ApplyConnection
		{
			get { return _ApplyConnection; }
			set
			{
				if (_ApplyConnection != value)
				{
					_ApplyConnection = value;
					RaisePropertyChanged("ApplyConnection");
				}
			}
		}

		private bool _GenerateContext = true;
		/// <summary>
		/// Gets or sets the entity project folder name.
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.DefaultValue(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_GenerateContext")]
		public bool GenerateContext
		{
			get { return _GenerateContext; }
			set
			{
				if (_GenerateContext != value)
				{
					_GenerateContext = value;
					RaisePropertyChanged("GenerateContext");
				}
			}
		}


		//private ConnectionTypeEnum _DatabaseType = ConnectionTypeEnum.Default;
		///// <summary>
		///// 获取或设置构建器需要生成支持的数据库类型。
		///// </summary>
		//[System.ComponentModel.DefaultValue(typeof(ConnectionTypeEnum), "Default")]
		//internal ConnectionTypeEnum DatabaseType
		//{
		//	get { return _DatabaseType; }
		//	set { _DatabaseType = value; }
		//}

		private ResxModeEnum _ResxMode = ResxModeEnum.AssemlyResource;
		/// <summary>
		/// 获取或设置数据持久类需要支持的数据库类型。
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentGenerator_ResxMode")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentGenerator_ResxMode_Display")]
		[System.ComponentModel.DefaultValue(typeof(ResxModeEnum), "AssemlyResource")]
		public ResxModeEnum ResxMode
		{
			get { return _ResxMode; }
			set
			{
				if (_ResxMode != value)
				{
					_ResxMode = value;
					base.RaisePropertyChanged("ResxMode");
				}
			}
		}

		private ConnectionTypeEnum[] _SupportDatabases;
		/// <summary>
		/// 获取或设置数据持久类需要支持的数据库类型。
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentGenerator_SupportDatabases")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentGenerator_SupportDatabases_Display")]
		[System.ComponentModel.DefaultValue(typeof(ConnectionTypeEnum), "SQLSERVER")]
		[System.ComponentModel.TypeConverter(typeof(SupportDatabasesConverter))]
		[System.ComponentModel.Editor(typeof(SupportDatabasesEditor), typeof(UITypeEditor))]
		public ConnectionTypeEnum[] SupportDatabases
		{
			get { return _SupportDatabases; }
			set
			{
				if (_SupportDatabases != value)
				{
					_SupportDatabases = value;
					base.RaisePropertyChanged("SupportDatabases");
				}
			}
		}


		private bool _BuildSqlf = true;
		private bool _BuildOraf = false;

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == ModifierAttribute) { return Enum.TryParse(value, out _Modifier); }
			else if (name == GenerateAttribute) { return Enum.TryParse(value, out _GenerateMode); }
			else if (name == NamingAttribute) { return Enum.TryParse(value, out _NamingRule); }
			else if (name == AccessAttribute) { _BaseAccess = value; return true; }
			else if (name == ApplyConnectionAttribute) { _ApplyConnection = Convert.ToBoolean(value); return true; }
			else if (name == ContextAttribute) { _GenerateContext = Convert.ToBoolean(value); return true; }
			else if (name == BuildSqlfAttribute) { _BuildSqlf = Convert.ToBoolean(value); return BuilderSupportDatabases(); }
			else if (name == BuildOrafAttribute) { _BuildOraf = Convert.ToBoolean(value); return BuilderSupportDatabases(); }
			else if (name == SupportAttribute) { _SupportDatabases = (ConnectionTypeEnum[])_Converter.ConvertFromString(value); }
			else if (name == ResourceAttribute) { return Enum.TryParse(value, out _ResxMode); }
			return false;
		}

		private bool BuilderSupportDatabases()
		{
			if (_BuildSqlf && _BuildOraf)
			{
				_SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.SQLSERVER, ConnectionTypeEnum.ORACLE };
			}
			else if (_BuildSqlf)
			{
				_SupportDatabases[0] = ConnectionTypeEnum.SQLSERVER;
			}
			else if (_BuildOraf)
			{
				_SupportDatabases[0] = ConnectionTypeEnum.ORACLE;
			}
			return true;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override bool ReadContent(System.Xml.XmlReader reader) { return true; }

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			if (_Modifier != ClassModifierEnum.Internal)
				writer.WriteAttributeString(ModifierAttribute, _Modifier.ToString());
			if (_NamingRule != NamingRules.PascalCase)
				writer.WriteAttributeString(NamingAttribute, _NamingRule.ToString());
			if (_GenerateMode != GenerateActionEnum.Multiple)
				writer.WriteAttributeString(GenerateAttribute, _GenerateMode.ToString());
			if (_ResxMode != ResxModeEnum.AssemlyResource)
				writer.WriteAttributeString(ResourceAttribute, _ResxMode.ToString());
			if (_BaseAccess != typeof(AbstractAccess).Name)
				writer.WriteAttributeString(AccessAttribute, _BaseAccess.ToString());
			if (_ApplyConnection == false) { writer.WriteAttributeString(ApplyConnectionAttribute, "false"); }
			if (!_GenerateContext) { writer.WriteAttributeString(ContextAttribute, Convert.ToString(_GenerateContext).ToLower()); }
			if (_SupportDatabases.Length > 0)
			{
				writer.WriteAttributeString(SupportAttribute, _Converter.ConvertToString(_SupportDatabases));
			}
			//if (!_BuildSqlf) { writer.WriteAttributeString(BuildSqlfAttribute, Convert.ToString(_BuildSqlf).ToLower()); }
			//if (_BuildOraf) { writer.WriteAttributeString(BuildOrafAttribute, Convert.ToString(_BuildOraf).ToLower()); }
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer) { }

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(System.Xml.XmlWriter writer, ConnectionTypeEnum connectionType) { }
	}
}
