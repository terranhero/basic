using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using Basic.Collections;
using Basic.Configuration;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using Basic.Tables;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示实体类定义信息
	/// </summary>
	public sealed class DataEntityElement : AbstractEntityElement
	{
		internal const string XmlElementName = "DataEntityElement";
		internal const string GenerateModeAttribute = "GenerateMode";
		internal const string GenerateCollectionAttribute = "gc";

		internal const string EnabledValidationAttribute = "EnabledValidation";
		private readonly DataCommandCollection dataCommands;
		private readonly DataCommandCollection fileDataCommands;
		private readonly DataEntityPropertyCollection propertyCollection;
		private readonly PersistentConfiguration _Persistent;
		private readonly DataEntityElementCollection dataEntityElements;
		private readonly DesignerInfoElement designerInfo;
		private readonly DataContractElement _DataContract;
		/// <summary>初始化 EntityDefinition 类的新实例。 </summary>
		public DataEntityElement(PersistentConfiguration persistent)
			: base(persistent, typeof(Basic.EntityLayer.AbstractEntity).FullName)
		{
			_Persistent = persistent;
			_DataContract = new DataContractElement(this);
			designerInfo = new DesignerInfoElement(persistent);
			dataCondition = new DataConditionElement(this);
			dataEntityElements = persistent.DataEntities;
			dataCommands = new DataCommandCollection(persistent);
			dataCommands.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCommandsCollectionChanged);
			fileDataCommands = persistent.DataCommands;
			propertyCollection = new DataEntityPropertyCollection(this);
			_EntityGenerator = new DataEntityGenerator(this);
			_ConditionGenerator = new DataConditionGenerator(this);
		}

		private void OnCommandsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (DataCommandElement element in e.NewItems)
				{
					fileDataCommands.Add(element);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (DataCommandElement element in e.OldItems)
				{
					fileDataCommands.Remove(element);
				}
			}
			base.OnPropertyChanged("Visibility");
		}

		#region 数据契约定义
		///// <summary>
		///// 表示当前对象关于数据契约的定义.
		///// </summary>
		///// <value>The string value assigned to the Name property</value>
		//[System.ComponentModel.Bindable(true)]
		//[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		//[Basic.Designer.PersistentDescription("PersistentDescription_DataContract")]
		//public DataContractElement DataContract
		//{
		//	get { return _DataContract; }
		//}

		/// <summary>
		/// 获取或设置类型的数据协定的名称。
		/// </summary>
		/// <value>数据协定的本地名称。 默认值是应用了该属性的类的名称。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_DataContract_Name")]
		[Basic.Designer.PersistentCategory("PersistentCategory_DataContract")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_DataContract_Name")]
		public string DataContractName
		{
			get { return _DataContract.Name; }
			set { _DataContract.Name = value; }
		}

		/// <summary>
		/// 获取或设置类型的数据协定的命名空间。
		/// </summary>
		/// <value>协定的命名空间。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_DataContract_Namespace")]
		[Basic.Designer.PersistentCategory("PersistentCategory_DataContract")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_DataContract_Namespace")]
		public string DataContractNamespace
		{
			get { return _DataContract.Namespace; }
			set { _DataContract.Namespace = value; }
		}

		/// <summary>
		/// 获取或设置类型是否使用数据协定。
		/// </summary>
		/// <value>协定的命名空间。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_DataContract_Generate")]
		[Basic.Designer.PersistentCategory("PersistentCategory_DataContract")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_DataContract_Generate")]
		[System.ComponentModel.DefaultValue(false)]
		public bool DataContractGenerate
		{
			get { return _DataContract.Generate; }
			set { _DataContract.Generate = value; }
		}

		/// <summary>
		/// 获取或设置类型是否使用数据协定。
		/// </summary>
		/// <value>协定的命名空间。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_DataContract_IsReference")]
		[Basic.Designer.PersistentCategory("PersistentCategory_DataContract")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_DataContract_IsReference")]
		[System.ComponentModel.DefaultValue(false)]
		public bool DataContractIsReference
		{
			get { return _DataContract.IsReference; }
			set { _DataContract.IsReference = value; }
		}

		#endregion

		internal PersistentConfiguration Persistent { get { return _Persistent; } }
		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DataCommandCollection DataCommands { get { return dataCommands; } }

		private readonly DataEntityGenerator _EntityGenerator;
		///// <summary>实体类设计信息</summary>
		//public DataEntityGenerator EntityGenerator { get { return _EntityGenerator; } }

		#region 实体模型生成属性
		/// <summary>
		/// 获取或设置一个值，表示实体类型Guid。
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Condition_Guid")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_Guid")]
		[System.ComponentModel.Editor(typeof(GuidTypeEditor), typeof(UITypeEditor))]
		public System.Guid EntityGuid
		{
			get { return _EntityGenerator.Guid; }
			set { _EntityGenerator.Guid = value; }
		}

		/// <summary>获取或设置当前实体类的基类类型.</summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Condition_BaseClass")]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_BaseClass")]
		[System.ComponentModel.DefaultValue("Basic.EntityLayer.AbstractCondition")]
		[System.ComponentModel.Editor(typeof(BaseClassSelector), typeof(UITypeEditor))]
		public string EntityBaseClass
		{
			get { return _EntityGenerator.BaseClass; }
			set { _EntityGenerator.BaseClass = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Condition_IsAbstract")]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_IsAbstract")]
		[System.ComponentModel.DefaultValue(false)]
		public bool EntityIsAbstract
		{
			get { return _EntityGenerator.IsAbstract; }
			set { _EntityGenerator.IsAbstract = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[DefaultValue(typeof(ClassModifierEnum), "Public"), Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Condition_Modifier")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_Modifier")]
		public ClassModifierEnum EntityModifier
		{
			get { return _EntityGenerator.Modifier; }
			set { _EntityGenerator.Modifier = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_ClassName")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Entity_ClassName")]
		public string EntityClassName { get { return _EntityGenerator.ClassName; } }


		private bool _GenerateCollection = false;
		/// <summary>是否生成模型集合</summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_GenerateCollection")]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Entity_GenerateCollection")]
		[System.ComponentModel.DefaultValue(false)]
		public bool GenerateCollection
		{
			get { return _GenerateCollection; }
			set
			{
				if (_GenerateCollection != value)
				{
					_GenerateCollection = value;
					RaisePropertyChanged("GenerateCollection");
				}
			}
		}
		private System.Guid _CollectionGuid = Guid.NewGuid();
		/// <summary>
		/// 获取或设置一个值，表示实体类型Guid。
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_CollectionGuid")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_CollectionGuid")]
		[System.ComponentModel.Editor(typeof(GuidTypeEditor), typeof(UITypeEditor))]
		public System.Guid CollectionGuid
		{
			get { return _CollectionGuid; }
			set { _CollectionGuid = value; RaisePropertyChanged("GenerateCollection"); }
		}
		#endregion

		#region 条件模型生成属性
		private readonly DataConditionGenerator _ConditionGenerator;
		///// <summary>实体类设计信息</summary>
		//public DataConditionGenerator ConditionGenerator { get { return _ConditionGenerator; } }
		/// <summary>
		/// 获取或设置一个值，表示实体类型Guid。
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Condition_Guid")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_Guid")]
		[System.ComponentModel.Editor(typeof(GuidTypeEditor), typeof(UITypeEditor))]
		public System.Guid ConditionGuid
		{
			get { return _ConditionGenerator.Guid; }
			set { _ConditionGenerator.Guid = value; }
		}

		/// <summary>获取或设置当前实体类的基类类型.</summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Condition_BaseClass")]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_BaseClass")]
		[System.ComponentModel.DefaultValue("Basic.EntityLayer.AbstractCondition")]
		[System.ComponentModel.Editor(typeof(BaseConditionSelector), typeof(UITypeEditor))]
		public string ConditionBaseClass
		{
			get { return _ConditionGenerator.BaseClass; }
			set { _ConditionGenerator.BaseClass = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Condition_IsAbstract")]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_IsAbstract")]
		[System.ComponentModel.DefaultValue(false)]
		public bool ConditionIsAbstract
		{
			get { return _ConditionGenerator.IsAbstract; }
			set { _ConditionGenerator.IsAbstract = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(typeof(ClassModifierEnum), "Public"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Condition_Modifier")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_Modifier")]
		public ClassModifierEnum ConditionModifier
		{
			get { return _ConditionGenerator.Modifier; }
			set { _ConditionGenerator.Modifier = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_ClassName")]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Condition_ClassName")]
		public string ConditionClassName { get { return _ConditionGenerator.ClassName; } }
		#endregion

		/// <summary>
		/// 当前条命令元素是否显示
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public System.Windows.Visibility Visibility
		{
			get
			{
				if (dataCommands.Count > 0)
					return System.Windows.Visibility.Visible;
				return System.Windows.Visibility.Collapsed;
			}
		}

		/// <summary>
		/// 实体类设计信息
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DesignerInfoElement DesignerInfo { get { return designerInfo; } }

		private readonly DataConditionElement dataCondition;
		/// <summary>
		/// 实体类设计信息
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DataConditionElement Condition { get { return dataCondition; } }

		/// <summary>
		/// 结果类实例
		/// </summary>
		[System.ComponentModel.Browsable(false), System.ComponentModel.Bindable(true)]
		public DataEntityPropertyCollection Properties { get { return propertyCollection; } }

		private bool? m_EnabledValidation = null;
		/// <summary>
		/// 获取或设置一个值，表示实体类名称
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Browsable(false)]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDescription("PropertyDescription_EnabledValidation")]
		public bool? EnabledValidation
		{
			get { return m_EnabledValidation; }
			set
			{
				if (m_EnabledValidation != value)
				{
					m_EnabledValidation = value;
					base.RaisePropertyChanged("EnabledValidation");
				}
			}
		}

		/// <summary>
		/// 确定 DataCommandCollection 是否包含指定的键。
		/// </summary>
		/// <param name="key">要在 DataCommandCollection 中定位的键。</param>
		/// <returns>如果 DataCommandCollection 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public bool ContainsKey(string key)
		{
			return fileDataCommands.ContainsKey(key);
		}

		/// <summary>
		/// 获取或设置一个值，表示实体类名称
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDescription("PropertyDescription_Name")]
		public override string Name
		{
			get { return base.Name; }
			set
			{
				if (base.Name != value)
				{
					base.Name = value;
					base.RaisePropertyChanged("Name");
					base.RaisePropertyChanged("ClassName");
					base.RaisePropertyChanged("EntityName");
					dataCondition.RaisePropertyChanged("Name");
					dataCondition.RaisePropertyChanged("ClassName");
					dataCondition.RaisePropertyChanged("EntityName");
				}
			}
		}

		/// <summary>
		/// 获取或设置当前实体类的基类类型.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_BaseClass")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.DefaultValue("Basic.EntityLayer.AbstractEntity")]
		[System.ComponentModel.Editor(typeof(BaseClassSelector), typeof(UITypeEditor))]
		[System.ComponentModel.Browsable(false)]
		public override string BaseClass
		{
			get { return base.BaseClass; }
			set { base.BaseClass = value; }
		}

		/// <summary>
		/// 获取或设置当前实体类的基类类型.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_BaseClass")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.DefaultValue("Basic.EntityLayer.AbstractCondition")]
		[System.ComponentModel.Editor(typeof(BaseConditionSelector), typeof(UITypeEditor))]
		[System.ComponentModel.Browsable(false)]
		public string BaseCondition
		{
			get { return dataCondition.BaseClass; }
			set { dataCondition.BaseClass = value; }
		}

		/// <summary>Gets or sets the EntityDefinition ClassName.</summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_EntityCollectionName")]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCollectionName")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityCollectionName")]
		[System.ComponentModel.Browsable(false)]
		public string EntityCollectionName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Name) == false)
				{
					if (Name.EndsWith("Info", StringComparison.OrdinalIgnoreCase))
					{
						return string.Concat(Name.Replace("Info", ""), "Collection");
					}
					return string.Concat(Name, "Collection");
				}
				return string.Concat(TableName, "Collection");
			}
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_EntityName")]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityName")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityName")]
		[System.ComponentModel.Browsable(false)]
		public override string EntityName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Name) == false)
				{
					if (Name.EndsWith("Info", StringComparison.OrdinalIgnoreCase))
					{
						return Name;
					}
					else if (Name.EndsWith("Keys", StringComparison.OrdinalIgnoreCase))
					{
						return Name;
					}
					return string.Concat(Name, "Entity");
				}
				return BaseClassName;
			}
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_ClassName")]
		[Basic.Designer.PersistentCategory("PersistentCategory_ClassName")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_ClassName")]
		[System.ComponentModel.Browsable(false)]
		public override string ClassName
		{
			get
			{
				if (GeneratorMode == GenerateModeEnum.DataTable && string.IsNullOrWhiteSpace(Name))
					return typeof(DataTable).Name;
				else if (GeneratorMode == GenerateModeEnum.DataTable && !string.IsNullOrWhiteSpace(Name))
					return DataTableName;
				else if (string.IsNullOrWhiteSpace(Name))
					return BaseClassName;
				return EntityName;
			}
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.Browsable(false)]
		internal string OldDataTableName { get { return string.Concat(OldName, "Table"); } }

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.Browsable(false)]
		internal string OldDataRowName { get { return string.Concat(OldName, "Row"); } }

		public DataEntityPropertyElement CreateProperty()
		{
			return new DataEntityPropertyElement(this);
		}

		public DataEntityPropertyElement CreateProperty(string name)
		{
			return new DataEntityPropertyElement(this, name);
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		internal string DataTableName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(base.Name))
					return string.Concat(Name, "Table");
				return base.BaseClassName;
			}
		}

		internal string DataRowName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(base.Name))
					return string.Concat(Name, "Row");
				return base.BaseClassName;
			}
		}

		internal string FullDataRowName
		{
			get
			{
				return string.Concat(DataTableName, ".", DataRowName);
			}
		}

		/// <summary>
		/// 返回表示当前 Basic.DataEntities.DataEntityElement 的 System.String 表示形式。
		/// </summary>
		/// <returns>System.String，表示当前的 Basic.DataEntities.DataEntityElement。</returns>
		public override string ToString()
		{
			//if(string.IsNullOrWhiteSpace(Name))
			//   return typeof(AbstractAttribute
			if (GeneratorMode == GenerateModeEnum.DataTable && string.IsNullOrWhiteSpace(Name))
				return typeof(DataTable).Name;
			else if (GeneratorMode == GenerateModeEnum.DataTable && !string.IsNullOrWhiteSpace(Name))
				return DataTableName;
			else if (string.IsNullOrWhiteSpace(Name))
				return BaseClassName;
			return EntityName;
		}
		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return XmlElementName.Replace("Element", ""); }

		private GenerateModeEnum _GeneratorMode = GenerateModeEnum.DataEntity;
		/// <summary>
		/// 获取或设置当前实体是否需要实体类 。
		/// </summary>
		/// <value>如果需要则为true，否则为false。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_GeneratorMode")]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_GeneratorMode")]
		[System.ComponentModel.DefaultValue(typeof(GenerateModeEnum), "DataEntity")]
		public GenerateModeEnum GeneratorMode
		{
			get { return _GeneratorMode; }
			set
			{
				if (_GeneratorMode != value)
				{
					_GeneratorMode = value;
					RaisePropertyChanged("GeneratorMode");
				}
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == GenerateModeAttribute) { return Enum.TryParse<GenerateModeEnum>(value, out _GeneratorMode); }
			else if (name == EnabledValidationAttribute) { m_EnabledValidation = Convert.ToBoolean(value); return true; }
			else if (name == GenerateCollectionAttribute) { _GenerateCollection = Convert.ToBoolean(value); return true; }
			else if (name == DesignerInfoElement.WidthAttribute) { designerInfo.Width = Convert.ToDouble(value); return true; }
			else if (name == DesignerInfoElement.HeightAttribute) { designerInfo.Height = Convert.ToDouble(value); return true; }
			else if (name == DesignerInfoElement.LeftAttribute) { designerInfo.Left = Convert.ToDouble(value); return true; }
			else if (name == DesignerInfoElement.TopAttribute) { designerInfo.Top = Convert.ToDouble(value); return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);
			if (base.Guid == Guid.Empty) { base.Guid = Guid.NewGuid(); }
			if (dataCondition.Visibility == System.Windows.Visibility.Visible && dataCondition.Guid == Guid.Empty)
			{
				dataCondition.Guid = Guid.NewGuid();
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override bool ReadContent(XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataEntityPropertyCollection.XmlElementName)
			{
				propertyCollection.ReadXml(reader.ReadSubtree());
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataConditionPropertyCollection.XmlElementName)
			{
				dataCondition.Arguments.ReadXml(reader.ReadSubtree());
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DesignerInfoElement.XmlElementName)
			{
				designerInfo.ReadXml(reader.ReadSubtree());
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataContractElement.XmlElementName)
			{
				_DataContract.ReadXml(reader.ReadSubtree());
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataCommandCollection.XmlElementName)
			{
				System.Xml.XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())  //读取所有命令节点信息(动态/静态)
				{
					if (reader2.NodeType == XmlNodeType.Element && reader2.LocalName == StaticCommandElement.XmlElementName)
					{
						StaticCommandElement dataCommand = new StaticCommandElement(this);
						dataCommand.ReadXml(reader2);
						dataCommands.Add(dataCommand);
					}
					else if (reader2.NodeType == XmlNodeType.Element && reader2.LocalName == DynamicCommandElement.XmlElementName)
					{
						DynamicCommandElement dataCommand = new DynamicCommandElement(this);
						dataCommand.ReadXml(reader2);
						dataCommands.Add(dataCommand);
					}
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.LocalName == DataCommandCollection.XmlElementName)
						return false;
				}
			}
			return base.ReadContent(reader);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			if (_GenerateCollection == true) { writer.WriteAttributeString(GenerateCollectionAttribute, _GenerateCollection.ToString().ToLower()); }
			if (_GeneratorMode != GenerateModeEnum.DataEntity)
				writer.WriteAttributeString(GenerateModeAttribute, _GeneratorMode.ToString());
			if (m_EnabledValidation.HasValue)
				writer.WriteAttributeString(EnabledValidationAttribute, Convert.ToString(m_EnabledValidation.Value).ToLower());
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			base.WriteContent(writer);
			writer.WriteStartElement(DataEntityPropertyCollection.XmlElementName);
			foreach (DataEntityPropertyElement property in propertyCollection)
				property.WriteXml(writer);
			writer.WriteEndElement();
			writer.WriteStartElement(DataConditionPropertyCollection.XmlElementName);
			writer.WriteAttributeString(DataConditionElement.BaseClassAttribute, dataCondition.BaseClass);
			if (dataCondition.Guid != Guid.Empty)
				writer.WriteAttributeString(GuidAttribute, dataCondition.Guid.ToString("D"));
			if (Expanded) { writer.WriteAttributeString(ExpandedAttribute, dataCondition.Expanded.ToString().ToLower()); }

			foreach (DataConditionPropertyElement property in dataCondition.Arguments)
				property.WriteXml(writer);
			writer.WriteEndElement();
			_DataContract.WriteXml(writer);
			designerInfo.WriteXml(writer);
			writer.WriteStartElement(DataCommandCollection.XmlElementName);
			if (dataCommands != null && dataCommands.Count > 0)
			{
				foreach (DataCommandElement dataCommand in dataCommands)
				{
					dataCommand.WriteXml(writer);
				}
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
			foreach (DataCommandElement dataCommand in dataCommands)
			{
				dataCommand.GenerateConfiguration(writer, connectionType);
			}
		}

		/// <summary>生成实体模型可修改部分代码</summary>
		/// <param name="codeNamespace"></param>
		/// <returns></returns>
		protected internal override CodeTypeDeclaration WriteEntityCode(CodeNamespace codeNamespace)
		{
			return base.WriteEntityCode(codeNamespace);
		}

		/// <summary>生成实体模型集合类</summary>
		/// <param name="codeNamespace"></param>
		/// <returns></returns>
		private CodeTypeDeclaration GenerateEntityCollectionDesignerCode(CodeNamespace codeNamespace)
		{
			CodeTypeDeclaration entityCollectionClass = new CodeTypeDeclaration(EntityCollectionName);
			CodeTypeReference aeBase = new CodeTypeReference(typeof(AbstractEntityCollection<>).FullName, CodeTypeReferenceOptions.GlobalReference);
			aeBase.TypeArguments.Add(EntityName);

			codeNamespace.Types.Add(entityCollectionClass);
			entityCollectionClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			if (string.IsNullOrWhiteSpace(Comment))
				entityCollectionClass.Comments.Add(new CodeCommentStatement(EntityCollectionName, true));
			else
				entityCollectionClass.Comments.Add(new CodeCommentStatement(Comment, true));
			entityCollectionClass.Comments.Add(new CodeCommentStatement("</summary>", true));

			entityCollectionClass.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Declaration", EntityCollectionName)));
			entityCollectionClass.BaseTypes.Add(aeBase);
			entityCollectionClass.IsPartial = true;
			entityCollectionClass.IsClass = true;
			entityCollectionClass.TypeAttributes = TypeAttributes.Class;
			if (Modifier == ClassModifierEnum.Internal)
			{
				entityCollectionClass.TypeAttributes = entityCollectionClass.TypeAttributes | TypeAttributes.NestedAssembly;
				entityCollectionClass.Attributes = entityCollectionClass.Attributes | MemberAttributes.Assembly;
			}
			else if (Modifier == ClassModifierEnum.Public)
			{
				entityCollectionClass.TypeAttributes = entityCollectionClass.TypeAttributes | TypeAttributes.Public;
				entityCollectionClass.Attributes = entityCollectionClass.Attributes | MemberAttributes.Public;
			}

			CodeTypeReference serializableTypeReference = new CodeTypeReference(typeof(SerializableAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration serializableAttribute = new CodeAttributeDeclaration(serializableTypeReference);
			entityCollectionClass.CustomAttributes.Add(serializableAttribute);
			CodeTypeReference toolboxItemTypeReference = new CodeTypeReference(typeof(ToolboxItemAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration toolboxItemAttribute = new CodeAttributeDeclaration(toolboxItemTypeReference);
			toolboxItemAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(false)));
			entityCollectionClass.CustomAttributes.Add(toolboxItemAttribute);

			CodeTypeReference guidAttributeReference = new CodeTypeReference(typeof(GuidAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration guidAttribute = new CodeAttributeDeclaration(guidAttributeReference);
			guidAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(CollectionGuid.ToString("D").ToUpper())));
			entityCollectionClass.CustomAttributes.Add(guidAttribute);

			if (!string.IsNullOrWhiteSpace(TableName))
			{
				CodeTypeReference tableTypeReference = new CodeTypeReference(typeof(TableMappingAttribute), CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration tableAttribute = new CodeAttributeDeclaration(tableTypeReference);
				tableAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(TableName)));
				entityCollectionClass.CustomAttributes.Add(tableAttribute);
			}


			CodeConstructor emptyConstructor = new CodeConstructor();
			emptyConstructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			emptyConstructor.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", EntityCollectionName), true));
			emptyConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			emptyConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeMethodReferenceExpression baseConstructor = new CodeMethodReferenceExpression();
			emptyConstructor.BaseConstructorArgs.Add(baseConstructor);
			entityCollectionClass.Members.Add(emptyConstructor);

			CodeConstructor listConstructor = new CodeConstructor();
			listConstructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			listConstructor.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", EntityCollectionName), true));
			listConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			listConstructor.Comments.Add(new CodeCommentStatement("<param name=\"list\">从中复制元素的集合</param>", true));
			listConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeTypeReference ipaginationTypeReference = new CodeTypeReference(typeof(IPagination<>).FullName);
			ipaginationTypeReference.TypeArguments.Add(EntityName);
			listConstructor.Parameters.Add(new CodeParameterDeclarationExpression(ipaginationTypeReference, "list"));
			listConstructor.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "list" });
			entityCollectionClass.Members.Add(listConstructor);

			CodeConstructor enumerableConstructor = new CodeConstructor();
			enumerableConstructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			enumerableConstructor.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", EntityCollectionName), true));
			enumerableConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			enumerableConstructor.Comments.Add(new CodeCommentStatement("<param name=\"collection\">从中复制元素的集合</param>", true));
			enumerableConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeTypeReference enumerableTypeReference = new CodeTypeReference(typeof(IEnumerable<>).FullName);
			enumerableTypeReference.TypeArguments.Add(EntityName);
			enumerableConstructor.Parameters.Add(new CodeParameterDeclarationExpression(enumerableTypeReference, "collection"));
			enumerableConstructor.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "collection" });
			entityCollectionClass.Members.Add(enumerableConstructor);

			entityCollectionClass.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
			return entityCollectionClass;
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeNamespace">表示需要写入代码的命名空间</param>
		protected internal override CodeTypeDeclaration WriteEntityDesignerCode(CodeNamespace codeNamespace)
		{
			CodeTypeDeclaration entityClass = base.WriteEntityDesignerCode(codeNamespace);

			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			CodeMethodInvokeExpression initializationClassInvoke = new CodeMethodInvokeExpression(thisReference, "InitializationClass");

			CodeTypeReference groupNameTypeReference = new CodeTypeReference(typeof(GroupNameAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration groupNameAttribute = new CodeAttributeDeclaration(groupNameTypeReference);
			//groupNameAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Persistent.TableInfo.EntityName)));
			/*允许资源组名称自定义，这样可以减少资源文件重复项。*/
			groupNameAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Persistent.GroupName)));
			if (!string.IsNullOrWhiteSpace(_Persistent.MessageConverter.ConverterName))
			{
				groupNameAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Persistent.MessageConverter.ConverterName)));
			}
			entityClass.CustomAttributes.Add(groupNameAttribute);

			//IEnumerable<DataEntityPropertyElement> list = propertyCollection.Where(m => m.DataMember == true);
			//if (list.Count() > 0)
			//{
			//    CodeTypeReference dataContractTypeReference = new CodeTypeReference(typeof(DataContractAttribute), CodeTypeReferenceOptions.GlobalReference);
			//    CodeAttributeDeclaration dataContractAttribute = new CodeAttributeDeclaration(dataContractTypeReference);
			//    entityClass.CustomAttributes.Add(dataContractAttribute);
			//}

			//if (_DataContract.Generate)
			//{
			//    CodeTypeReference dataContractTypeReference = new CodeTypeReference(typeof(DataContractAttribute), CodeTypeReferenceOptions.GlobalReference);
			//    CodeAttributeDeclaration dataContractAttribute = new CodeAttributeDeclaration(dataContractTypeReference);
			//    if (_DataContract.IsReference)
			//        dataContractAttribute.Arguments.Add(new CodeAttributeArgument("IsReference", new CodePrimitiveExpression(true)));
			//    if (!string.IsNullOrWhiteSpace(_DataContract.Name))
			//        dataContractAttribute.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(_DataContract.Name)));
			//    if (!string.IsNullOrWhiteSpace(_DataContract.Namespace))
			//        dataContractAttribute.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(_DataContract.Namespace)));
			//    entityClass.CustomAttributes.Add(dataContractAttribute);
			//}

			CodeConstructor constructor = new CodeConstructor();
			constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", EntityName), true));
			constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeMethodReferenceExpression baseConstructor = new CodeMethodReferenceExpression();
			if (m_EnabledValidation.HasValue)
				constructor.BaseConstructorArgs.Add(new CodePrimitiveExpression(m_EnabledValidation.Value));
			else
				constructor.BaseConstructorArgs.Add(baseConstructor);
			constructor.Statements.Add(initializationClassInvoke);
			entityClass.Members.Add(constructor);

			CodeConstructor pkConstructor = new CodeConstructor();
			pkConstructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			pkConstructor.Comments.Add(new CodeCommentStatement(string.Format("使用关键字初始化 {0} 类的实例。", EntityName), true));
			pkConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			pkConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			if (m_EnabledValidation.HasValue)
				pkConstructor.BaseConstructorArgs.Add(new CodePrimitiveExpression(m_EnabledValidation.Value));
			else
				pkConstructor.BaseConstructorArgs.Add(baseConstructor);

			#region 实体属性
			foreach (DataEntityPropertyElement property in propertyCollection)
			{
				property.WriteDesignerCode(entityClass, pkConstructor);
				if (property.PrimaryKey)
				{
					string varName = string.Concat("p", property.Name);
					pkConstructor.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">{1}</param>", varName, property.Comment), true));
					if (property.Nullable && property.Type != null && !property.Type.IsClass)
						pkConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Nullable<>).MakeGenericType(property.Type), varName));
					else if (property.Nullable && property.Type != null && property.Type.IsClass)
						pkConstructor.Parameters.Add(new CodeParameterDeclarationExpression(property.Type, varName));
					else if (property.Nullable && property.Type == null)
						pkConstructor.Parameters.Add(new CodeParameterDeclarationExpression(string.Concat(property.TypeName, "?"), varName));
					else if (!property.Nullable && property.Type != null)
						pkConstructor.Parameters.Add(new CodeParameterDeclarationExpression(property.Type, varName));
					else
						pkConstructor.Parameters.Add(new CodeParameterDeclarationExpression(property.TypeName, varName));
					CodeAssignStatement assign = new CodeAssignStatement();
					assign.Left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), property.FieldName);
					assign.Right = new CodeVariableReferenceExpression(varName);
					pkConstructor.Statements.Add(assign);
				}
			}

			#endregion

			if (pkConstructor.Parameters.Count > 0)
			{
				pkConstructor.Statements.Add(initializationClassInvoke);
				entityClass.Members.Add(pkConstructor);
			}

			CodeSnippetTypeMember initializationClass = new CodeSnippetTypeMember("\t\tpartial void InitializationClass();\r\n");
			initializationClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			initializationClass.Comments.Add(new CodeCommentStatement("初始化类实例。", true));
			initializationClass.Comments.Add(new CodeCommentStatement("</summary>", true));
			entityClass.Members.Add(initializationClass);

			entityClass.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));

			if (GenerateCollection) { GenerateEntityCollectionDesignerCode(codeNamespace); }
			return entityClass;
		}

		/// <summary>
		/// 写入 DataTable 代码（partial部分）
		/// </summary>
		/// <param name="codeNamespace"></param>
		/// <param name="requireRegion">是否需要生成#region片段</param>
		internal CodeTypeDeclaration WriteTableCode(CodeNamespace codeNamespace)
		{
			string tableClassName = DataTableName;
			CodeTypeDeclaration tableCode = new CodeTypeDeclaration(tableClassName);
			tableCode.IsPartial = true;
			tableCode.IsClass = true;
			tableCode.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Declaration", DataTableName)));

			CodeMemberMethod customInitClass = new CodeMemberMethod();
			customInitClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			customInitClass.Comments.Add(new CodeCommentStatement("初始化类实例。", true));
			customInitClass.Comments.Add(new CodeCommentStatement("</summary>", true));
			customInitClass.Name = "CustomInitClass";
			customInitClass.ReturnType = new CodeTypeReference("partial void");
			customInitClass.Attributes = MemberAttributes.Private | MemberAttributes.AccessMask;
			tableCode.Members.Add(customInitClass);

			CodeMemberMethod customInitColumns = new CodeMemberMethod();
			customInitColumns.Comments.Add(new CodeCommentStatement("<summary>", true));
			customInitColumns.Comments.Add(new CodeCommentStatement("初始化列信息。", true));
			customInitColumns.Comments.Add(new CodeCommentStatement("</summary>", true));
			customInitColumns.Name = "CustomInitColumns";
			customInitColumns.ReturnType = new CodeTypeReference("partial void");
			customInitColumns.Attributes = MemberAttributes.Private | MemberAttributes.AccessMask;
			tableCode.Members.Add(customInitColumns);

			WriteTableRowCode(tableCode);
			tableCode.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
			codeNamespace.Types.Add(tableCode);
			return tableCode;
		}

		/// <summary>
		/// 写入 DataTable 代码
		/// </summary>
		/// <param name="codeNamespace">强类型 DataTable 所在命名空间。</param>
		internal CodeTypeDeclaration WriteTableDesignerCode(CodeNamespace codeNamespace)
		{
			if (_GeneratorMode == GenerateModeEnum.DataEntity) { return null; }
			CodeTypeDeclaration tableCode = new CodeTypeDeclaration(DataTableName);
			tableCode.Comments.Add(new CodeCommentStatement("<summary>", true));
			if (string.IsNullOrWhiteSpace(Comment))
				tableCode.Comments.Add(new CodeCommentStatement(DataTableName, true));
			else
				tableCode.Comments.Add(new CodeCommentStatement(Comment, true));
			tableCode.Comments.Add(new CodeCommentStatement("</summary>", true));
			tableCode.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Declaration", DataTableName)));
			CodeTypeReference baseType = new CodeTypeReference(typeof(Basic.Tables.BaseTableType<>), CodeTypeReferenceOptions.GlobalReference);
			baseType.TypeArguments.Add(string.Concat(DataTableName, ".", DataRowName));
			tableCode.BaseTypes.Add(baseType);
			tableCode.Attributes = MemberAttributes.Public;
			tableCode.IsPartial = true;
			tableCode.IsClass = true;
			tableCode.TypeAttributes = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;
			CodeTypeReference serializableTypeReference = new CodeTypeReference(typeof(SerializableAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration serializableAttribute = new CodeAttributeDeclaration(serializableTypeReference);
			tableCode.CustomAttributes.Add(serializableAttribute);
			CodeTypeReference toolboxItemTypeReference = new CodeTypeReference(typeof(ToolboxItemAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration toolboxItemAttribute = new CodeAttributeDeclaration(toolboxItemTypeReference);
			toolboxItemAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(false)));
			tableCode.CustomAttributes.Add(toolboxItemAttribute);

			CodeConstructor constructor = new CodeConstructor();
			constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", DataRowName), true));
			constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor.BaseConstructorArgs.Add(new CodePrimitiveExpression(TableName));
			tableCode.Members.Add(constructor);

			CodeSnippetTypeMember customInitClass = new CodeSnippetTypeMember("\t\tpartial void CustomInitClass();\r\n");
			customInitClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			customInitClass.Comments.Add(new CodeCommentStatement("初始化类实例。", true));
			customInitClass.Comments.Add(new CodeCommentStatement("</summary>", true));
			tableCode.Members.Add(customInitClass);

			CodeSnippetTypeMember customInitColumns = new CodeSnippetTypeMember("\t\tpartial void CustomInitColumns();");
			customInitColumns.Comments.Add(new CodeCommentStatement("<summary>", true));
			customInitColumns.Comments.Add(new CodeCommentStatement("初始化列信息。", true));
			customInitColumns.Comments.Add(new CodeCommentStatement("</summary>", true));
			tableCode.Members.Add(customInitColumns);
			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			CodeMethodInvokeExpression customInitClassInvoke = new CodeMethodInvokeExpression(thisReference, "CustomInitClass");
			CodeMethodInvokeExpression customInitColumnsInvoke = new CodeMethodInvokeExpression(thisReference, "CustomInitColumns");

			CodeMemberMethod initClass = new CodeMemberMethod();
			initClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			initClass.Comments.Add(new CodeCommentStatement("初始化类实例。", true));
			initClass.Comments.Add(new CodeCommentStatement("</summary>", true));
			initClass.Name = "InitClass";
			initClass.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			tableCode.Members.Add(initClass);

			CodeMemberMethod initColumns = new CodeMemberMethod();
			initColumns.Comments.Add(new CodeCommentStatement("<summary>", true));
			initColumns.Comments.Add(new CodeCommentStatement("初始化列信息。", true));
			initColumns.Comments.Add(new CodeCommentStatement("</summary>", true));
			initColumns.Name = "InitColumns";
			initColumns.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			tableCode.Members.Add(initColumns);

			#region 重载clone方法
			//重载clone方法
			CodeMemberMethod cloneMethod = new CodeMemberMethod();
			cloneMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			cloneMethod.Comments.Add(new CodeCommentStatement(string.Format("克隆 {0} 的结构，包括所有 {0} 架构和约束。", DataTableName), true));
			cloneMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			cloneMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>新的 {0}，与当前的 {0} 具有相同的架构。</returns>", DataTableName), true));
			cloneMethod.Name = "Clone";
			cloneMethod.ReturnType = new CodeTypeReference(typeof(DataTable), CodeTypeReferenceOptions.GlobalReference);
			cloneMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			CodeVariableDeclarationStatement cloneTableDeclaration = new CodeVariableDeclarationStatement(DataTableName, "cloneTable");
			CodeMethodInvokeExpression baseCloneMethod = new CodeMethodInvokeExpression();
			baseCloneMethod.Method.TargetObject = new CodeBaseReferenceExpression();
			baseCloneMethod.Method.MethodName = "Clone";
			cloneTableDeclaration.InitExpression = new CodeCastExpression(DataTableName, baseCloneMethod);
			cloneMethod.Statements.Add(cloneTableDeclaration);
			CodeVariableReferenceExpression cloneTableVariable = new CodeVariableReferenceExpression("cloneTable");
			CodeMethodInvokeExpression thisInitColumns = new CodeMethodInvokeExpression();
			thisInitColumns.Method.TargetObject = cloneTableVariable;
			thisInitColumns.Method.MethodName = "InitColumns";
			cloneMethod.Statements.Add(thisInitColumns);
			cloneMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("cloneTable")));
			tableCode.Members.Add(cloneMethod);
			#endregion

			#region 重载 CreateInstance 方法
			CodeMemberMethod createInstanceMethod = new CodeMemberMethod();
			createInstanceMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			createInstanceMethod.Comments.Add(new CodeCommentStatement(string.Format("创建 {0} 类实例。", DataTableName), true));
			createInstanceMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			createInstanceMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>新的 {0} 与当前的 {0} 具有相同的架构。</returns>", DataTableName), true));
			createInstanceMethod.Name = "CreateInstance";
			createInstanceMethod.ReturnType = new CodeTypeReference(typeof(DataTable), CodeTypeReferenceOptions.GlobalReference);
			createInstanceMethod.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			CodeMethodReturnStatement createInstanceReturn = new CodeMethodReturnStatement();
			createInstanceReturn.Expression = new CodeObjectCreateExpression(DataTableName);
			createInstanceMethod.Statements.Add(createInstanceReturn);
			tableCode.Members.Add(createInstanceMethod);
			#endregion

			#region 重载 NewRowFromBuilder 方法
			CodeMemberMethod nrfbMethod = new CodeMemberMethod();
			nrfbMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			nrfbMethod.Comments.Add(new CodeCommentStatement("从现有的行创建新行。", true));
			nrfbMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			nrfbMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>新的 {0} 与当前的 {0} 具有相同的架构。</returns>", DataTableName), true));
			nrfbMethod.Name = "NewRowFromBuilder";
			nrfbMethod.ReturnType = new CodeTypeReference(typeof(DataRow), CodeTypeReferenceOptions.GlobalReference);
			nrfbMethod.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			CodeTypeReference dataRowBuilder = new CodeTypeReference(typeof(DataRowBuilder), CodeTypeReferenceOptions.GlobalReference);
			nrfbMethod.Parameters.Add(new CodeParameterDeclarationExpression(dataRowBuilder, "builder"));
			CodeMethodReturnStatement nrfbReturn = new CodeMethodReturnStatement();
			nrfbReturn.Expression = new CodeObjectCreateExpression(DataRowName, new CodeVariableReferenceExpression("builder"));
			nrfbMethod.Statements.Add(nrfbReturn);
			tableCode.Members.Add(nrfbMethod);
			#endregion

			#region 定义 FindByKey 方法
			CodeMemberMethod fbkMethod = new CodeMemberMethod();
			fbkMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			fbkMethod.Comments.Add(new CodeCommentStatement("获取包含指定的主键值的行", true));
			fbkMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			fbkMethod.Name = "FindByKey";
			fbkMethod.ReturnType = new CodeTypeReference(DataRowName, CodeTypeReferenceOptions.GenericTypeParameter);
			fbkMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeMethodInvokeExpression baseFindByKey = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "FindByKey");
			CodeMethodReturnStatement fbkReturn = new CodeMethodReturnStatement(baseFindByKey);
			fbkMethod.Statements.Add(fbkReturn);
			#endregion

			#region 创建关键字列信息
			CodeArrayCreateExpression newColumnArray = new CodeArrayCreateExpression();
			newColumnArray.CreateType = new CodeTypeReference(typeof(System.Data.DataColumn), CodeTypeReferenceOptions.GlobalReference);
			#endregion

			foreach (DataEntityPropertyElement entityProperty in this.Properties)
			{
				entityProperty.WriteIniColumnCode(initColumns);
				entityProperty.WriteIniClassCode(initClass);
				entityProperty.WriteTableCode(tableCode);
				entityProperty.WritePrimaryKeyCode(fbkMethod, baseFindByKey);
				entityProperty.WritePrimaryKeyCode(newColumnArray);
			}
			if (newColumnArray.Initializers.Count > 0)
			{
				CodeBaseReferenceExpression baseReference = new CodeBaseReferenceExpression();
				CodePropertyReferenceExpression constraintsProperty = new CodePropertyReferenceExpression(baseReference, "Constraints");
				CodeMethodInvokeExpression constraintsAddMethod = new CodeMethodInvokeExpression(constraintsProperty, "Add");
				//创建UniqueConstraint 的实例引用
				CodeObjectCreateExpression ucCreate = new CodeObjectCreateExpression();
				ucCreate.CreateType = new CodeTypeReference(typeof(System.Data.UniqueConstraint), CodeTypeReferenceOptions.GlobalReference);
				ucCreate.Parameters.Add(new CodePrimitiveExpression(string.Concat("PK_", TableName)));
				ucCreate.Parameters.Add(newColumnArray);
				ucCreate.Parameters.Add(new CodePrimitiveExpression(true));
				constraintsAddMethod.Parameters.Add(ucCreate);
				initClass.Statements.Add(constraintsAddMethod);
			}
			initClass.Statements.Add(customInitClassInvoke);
			initColumns.Statements.Add(customInitColumnsInvoke);

			fbkMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>包含指定的主键值的 {0} 对象的数组；否则为空值（如果 {1} 中不存在主键值）。</returns>", DataRowName, DataTableName), true));
			if (fbkMethod.Parameters.Count > 0)
				tableCode.Members.Add(fbkMethod);

			WriteTableRowDesignerCode(tableCode);
			tableCode.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
			codeNamespace.Types.Add(tableCode);
			return tableCode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableCode"></param>
		private void WriteTableRowCode(CodeTypeDeclaration tableCode)
		{
			string entityName = DataRowName;
			CodeTypeDeclaration rowCode = new CodeTypeDeclaration(entityName)
			{
				IsPartial = true,
				IsClass = true
			};
			tableCode.Members.Add(rowCode);
		}

		private void WriteTableRowDesignerCode(CodeTypeDeclaration tableCode)
		{
			CodeTypeDeclaration rowCode = new CodeTypeDeclaration(DataRowName);
			rowCode.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Declaration", DataRowName)));
			rowCode.Comments.Add(new CodeCommentStatement("<summary>", true));
			if (string.IsNullOrWhiteSpace(Comment))
				rowCode.Comments.Add(new CodeCommentStatement(DataRowName, true));
			else
				rowCode.Comments.Add(new CodeCommentStatement(Comment, true));
			rowCode.Comments.Add(new CodeCommentStatement("</summary>", true));
			CodeTypeReference baseType = new CodeTypeReference(typeof(BaseTableRowType), CodeTypeReferenceOptions.GlobalReference);
			rowCode.BaseTypes.Add(baseType);
			rowCode.Attributes = MemberAttributes.Public;
			rowCode.IsPartial = true;
			rowCode.IsClass = true;
			rowCode.TypeAttributes = TypeAttributes.Public | TypeAttributes.Class;

			string tableMemberName = string.Concat("table", Name);
			CodeMemberField tableMemberField = new CodeMemberField(DataTableName, tableMemberName);
			CodeComment tableMemberFieldComment = new CodeComment(string.Format("具有此行架构的 {0} 类实例。", DataTableName), true);
			tableMemberField.Comments.Add(new CodeCommentStatement("<summary>", true));
			tableMemberField.Comments.Add(new CodeCommentStatement(tableMemberFieldComment));
			tableMemberField.Comments.Add(new CodeCommentStatement("</summary>", true));
			rowCode.Members.Add(tableMemberField);

			CodeConstructor constructor = new CodeConstructor();
			constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", DataRowName), true));
			constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor.Attributes = MemberAttributes.Assembly;
			CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression(typeof(DataRowBuilder), "rb");
			constructor.Parameters.Add(parameter);
			CodeAssignStatement assigMember = new CodeAssignStatement();
			assigMember.Left = new CodeVariableReferenceExpression(tableMemberName);
			CodePropertyReferenceExpression tableProperty = new CodePropertyReferenceExpression();
			tableProperty.PropertyName = "Table";
			tableProperty.TargetObject = new CodeBaseReferenceExpression();
			assigMember.Right = new CodeCastExpression(DataTableName, tableProperty);
			constructor.Statements.Add(assigMember);
			constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("rb"));

			rowCode.Members.Add(constructor);
			#region 实体属性
			foreach (DataEntityPropertyElement property in this.Properties)
			{
				property.WriteRowCode(rowCode, Name);
			}
			#endregion

			rowCode.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
			tableCode.Members.Add(rowCode);
		}
	}
}
