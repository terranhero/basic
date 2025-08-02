using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Transactions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Basic.Collections;
using Basic.DataAccess;
using Basic.Database;
using Basic.DataEntities;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.Configuration
{
	/// <summary>
	/// 表示抽象配置文件
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
	[XmlRoot(ElementName = XmlElementName, Namespace = XmlConfigNamespace)]
	public sealed class PersistentConfiguration : AbstractCustomTypeDescriptor, IDisposable, ICloneable, INotifyCollectionChanged
	{
		#region 节点元素名称
		internal const string XmlElementName = "PersistentConfiguration";
		internal const string XmlElementPrefix = "dpdl";
		internal const string XmlConfigNamespace = "http://dev.goldsoft.com/2013/BasicPersistentSchema-5.0.xsd";
		internal const string XmlDataNamespace = "http://dev.goldsoft.com/2013/BasicDataPersistentSchema-5.0.xsd";
		internal const string VersionAttribute = "Version";

		internal const string EntityFolderElement = "EntityFolder";
		internal const string MessageConverterElement = "MessageConverter";
		internal const string TableNameElement = "TableName";
		internal const string ViewNameElement = "ViewName";
		internal const string NamespaceElement = "Namespace";
		internal const string NamespacesElement = "Namespaces";
		#endregion

		private readonly NamespaceCollection _ImportNamespaces;
		private readonly DataCommandCollection _DataCommands;
		private readonly DesignTableInfo _TableInfo;
		private readonly DataEntityElementCollection _DataEntityElements;
		private readonly ProjectInfo _ProjectInfo;
		private readonly MessageInfo _MessageInfo;
		private readonly PersistentGeneratorElement _Generator;
		private Version _Version;
		/// <summary>
		/// 初始化 PersistentConfiguration 类实例
		/// </summary>
		internal PersistentConfiguration()
		{
			_Version = new System.Version(5, 0, 0, 0);
			_ImportNamespaces = new NamespaceCollection();
			_ImportNamespaces.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
			_ProjectInfo = new ProjectInfo(this);
			_MessageInfo = new MessageInfo(this, null, null);
			_TableInfo = new DesignTableInfo(this, XmlElementPrefix, XmlConfigNamespace);
			_TableInfo.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "EntityName" && _MessageInfo.GroupName == _TableInfo.OldEntityName)
				{
					_MessageInfo.GroupName = _TableInfo.EntityName;
				}
			};
			_Generator = new PersistentGeneratorElement(this, XmlElementPrefix, XmlConfigNamespace);
			_DataCommands = new DataCommandCollection(this);
			_DataEntityElements = new DataEntityElementCollection(this, XmlElementPrefix, XmlConfigNamespace);
			_DataEntityElements.CollectionChanged += new NotifyCollectionChangedEventHandler(DataEntityElements_CollectionChanged);
		}

		private void DataEntityElements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.OnCollectionChanged(this, e);
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (DataEntityElement entity in e.NewItems)
				{
					foreach (DataCommandElement dataCommand in entity.DataCommands)
					{
						if (dataCommand.Kind == ConfigurationTypeEnum.AddNew) { _NewEntity = entity; continue; }
						else if (dataCommand.Kind == ConfigurationTypeEnum.Modify) { _EditEntity = entity; continue; }
						else if (dataCommand.Kind == ConfigurationTypeEnum.Remove) { _DeleteEntity = entity; continue; }
						else if (dataCommand.Kind == ConfigurationTypeEnum.SearchTable) { _SearchEntity = entity; continue; }
					}
				}
			}
		}

		/// <summary></summary>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		internal DataCommandCollection DataCommands { get { return _DataCommands; } }

		#region 控件显示属性
		private System.Windows.Visibility _EnterEntity = System.Windows.Visibility.Collapsed;

		/// <summary>Gets or sets the entity project folder name.</summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public System.Windows.Visibility EnterEntity
		{
			get { return _EnterEntity; }
			set
			{
				if (_EnterEntity != value)
				{
					_EnterEntity = value;
					OnPropertyChanged("EnterEntity");
				}
			}
		}

		private System.Windows.Visibility _EnterProperty = System.Windows.Visibility.Collapsed;
		/// <summary>
		/// Gets or sets the entity project folder name.
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public System.Windows.Visibility EnterProperty
		{
			get { return _EnterProperty; }
			set
			{
				if (_EnterProperty != value)
				{
					_EnterProperty = value;
					OnPropertyChanged("EnterProperty");
				}
			}
		}

		private System.Windows.Visibility _EnterCommand = System.Windows.Visibility.Collapsed;
		/// <summary>
		/// Gets or sets the entity project folder name.
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public System.Windows.Visibility EnterCommand
		{
			get { return _EnterCommand; }
			set
			{
				if (_EnterCommand != value)
				{
					_EnterCommand = value;
					OnPropertyChanged("EnterCommand");
				}
			}
		}

		/// <summary>
		/// Gets or sets the entity project folder name.
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public System.Windows.Visibility RefreshTableVisibility
		{
			get
			{
				if (string.IsNullOrWhiteSpace(TableName))
					return System.Windows.Visibility.Collapsed;
				return System.Windows.Visibility.Visible;
			}
		}

		/// <summary>
		/// Gets or sets the entity project folder name.
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public System.Windows.Visibility InitializeTableVisibility
		{
			get
			{
				if (string.IsNullOrWhiteSpace(TableName))
					return System.Windows.Visibility.Visible;
				return System.Windows.Visibility.Collapsed;
			}
		}
		#endregion

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return GetType().Name; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return AccessName; }

		/// <summary>
		/// 当前配置文件对应的实体类型名称
		/// </summary>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public string EntityName { get { return string.Concat(_TableInfo.EntityName, "Entity"); } }

		/// <summary>
		/// 当前表的 Access 类名称。
		/// </summary>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public string AccessName { get { return string.Concat(_TableInfo.EntityName, "Access"); } }

		/// <summary>
		/// 当前表的 Context 类名称。
		/// </summary>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public string ContextName { get { return string.Concat(_TableInfo.EntityName, "Context"); } }

		/// <summary>
		/// 表示配置文件中实体类集合
		/// </summary>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public DataEntityElementCollection DataEntities { get { return _DataEntityElements; } }

		/// <summary>
		/// 更新当前持久类中属性与数据库字段映射关系。
		/// </summary>
		/// <param name="mappingProperties"></param>
		public void UpdatePropertyMapping(IDictionary<string, string> mappingProperties)
		{
			foreach (DataEntityElement entity in _DataEntityElements)
			{
				foreach (DataEntityPropertyElement property in entity.Properties)
				{
					if (string.IsNullOrWhiteSpace(property.Column)) { continue; }
					else if (!mappingProperties.ContainsKey(property.Column)) { mappingProperties.Add(property.Column, property.Name); }
				}
			}
		}

		#region 代码生成
		/// <summary>获取或设置实体模型命名规则</summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.DefaultValue(typeof(NamingRules), "DefaultCase"), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_NamingRule")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_NamingRule")]
		public NamingRules NamingRule
		{
			get { return _Generator.NamingRule; }
			set { _Generator.NamingRule = value; }
		}

		/// <summary>获取或设置一个布尔类型值，该值表示是否生成新增实体模型.</summary>
		[System.ComponentModel.DefaultValue(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_GenerateNewEntity")]
		public bool GenerateNewEntity
		{
			get { return _Generator.GenerateNewEntity; }
			set { _Generator.GenerateNewEntity = value; }
		}

		/// <summary>获取或设置一个布尔类型值，该值表示是否生成修改实体模型.</summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.DefaultValue(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_GenerateEditEntity")]
		public bool GenerateEditEntity
		{
			get { return _Generator.GenerateEditEntity; }
			set { _Generator.GenerateEditEntity = value; }
		}

		/// <summary>获取或设置一个布尔类型值，该值表示是否生成删除实体模型.</summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.DefaultValue(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_GenerateDelEntity")]
		public bool GenerateDelEntity
		{
			get { return _Generator.GenerateDelEntity; }
			set { _Generator.GenerateDelEntity = value; }
		}

		/// <summary>
		/// Gets or sets the entity project folder name.
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.DefaultValue(typeof(GenerateActionEnum), "Multiple"), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_GenerateMode")]
		public GenerateActionEnum GenerateMode
		{
			get { return _Generator.GenerateMode; }
			set { _Generator.GenerateMode = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(typeof(ClassModifierEnum), "Internal"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_Modifier")]
		public ClassModifierEnum Modifier
		{
			get { return _Generator.Modifier; }
			set { _Generator.Modifier = value; }
		}

		/// <summary>
		/// 获取或设置数据持久类需要支持的数据库类型。
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentGenerator_ResxMode")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDisplay("PersistentGenerator_ResxMode_Display")]
		[System.ComponentModel.DefaultValue(typeof(ResxModeEnum), "AssemlyResource")]
		public ResxModeEnum ResxMode
		{
			get { return _Generator.ResxMode; }
			set { _Generator.ResxMode = value; }
		}

		/// <summary>获取或设置当前数据持久类基类。</summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue("Basic.DataAccess.AbstractAccess"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_BaseAccess")]
		[System.ComponentModel.Editor(typeof(BaseAccessSelector), typeof(UITypeEditor))]
		public string BaseAccess
		{
			get { return _Generator.BaseAccess; }
			set { _Generator.BaseAccess = value; }
		}

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
			get { return _Generator.ApplyConnection; }
			set { _Generator.ApplyConnection = value; }
		}

		/// <summary>
		/// Gets or sets the entity project folder name.
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[System.ComponentModel.DefaultValue(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentGenerator_GenerateContext")]
		public bool GenerateContext
		{
			get { return _Generator.GenerateContext; }
			set { _Generator.GenerateContext = value; }
		}

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
			get { return _Generator.SupportDatabases; }
			set { _Generator.SupportDatabases = value; }
		}

		///// <summary>
		///// 获取或设置实体类项目文件信息。
		///// </summary>
		///// <value>The string value assigned to the Name property</value>
		//[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		//public PersistentGeneratorElement Generator
		//{
		//	get { return _Generator; }
		//}
		#endregion

		/// <summary>
		/// 获取或设置实体类项目文件信息。
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDescription("PersistentDescription_EntityProject")]
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityProject")]
		[System.ComponentModel.Editor(typeof(ProjectSelectorEditor), typeof(UITypeEditor))]
		public ProjectInfo Project { get { return _ProjectInfo; } set {/*必须有，否则属性编辑器不能显示编辑*/ } }

		/// <summary>
		/// 获取实体类项目 名称 信息.
		/// </summary>
		/// <value>一个 System.Guid 值，该值表示实体项目的 Guid 信息。</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDescription("PersistentDescription_EntityProject")]
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityProject")]
		[System.ComponentModel.Editor(typeof(ProjectSelectorEditor), typeof(UITypeEditor))]
		public string ProjectName
		{
			get { return _ProjectInfo.ProjectName; }
			set { _ProjectInfo.ProjectName = value; }
		}

		/// <summary>
		/// 获取或设置实体类项目 GUID 信息.
		/// </summary>
		/// <value>一个 System.Guid 值，该值表示实体项目的 Guid 信息。</value>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public System.Guid ProjectGuid
		{
			get { return _ProjectInfo.ProjectGuid; }
		}

		/// <summary>
		/// 获取实体类Visual Studio项目项目唯一名称。
		/// </summary>
		/// <value>一个 System.Guid 值，该值表示实体项目的 Guid 信息。</value>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		public string UniqueName
		{
			get { return _ProjectInfo.UniqueName; }
		}

		/// <summary>
		/// 获取或设置 实体类所在项目的文件夹名称，包含项目下的全路径(但不包含项目的文件夹路径)。
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_Resource")]
		[Basic.Designer.PersistentDescription("PersistentDescription_MessageConverter")]
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_MessageConverter")]
		[System.ComponentModel.Editor(typeof(MessageFileEditor), typeof(UITypeEditor))]
		public MessageInfo MessageConverter { get { return _MessageInfo; } set { } }

		/// <summary>获取或设置当前数据持久类的私有资源组名称</summary>
		[Basic.Designer.PersistentCategory("PersistentCategory_Resource")]
		[Basic.Designer.PersistentDescription("PersistentDescription_GroupName")]
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_GroupName")]
		[System.ComponentModel.TypeConverter(typeof(GroupNameConverter))]
		public string GroupName { get { return _MessageInfo.GroupName; } set { _MessageInfo.GroupName = value; } }

		/// <summary>获取或设置当前数据持久类的私有资源组名称</summary>
		[Basic.Designer.PersistentCategory("PersistentCategory_Resource")]
		[Basic.Designer.PersistentDescription("PersistentDescription_GroupName")]
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_PublicGroupName")]
		[System.ComponentModel.TypeConverter(typeof(GroupNameConverter))]
		public string PublicGroupName { get { return _MessageInfo.PublicGroupName; } set { _MessageInfo.PublicGroupName = value; } }

		private string _EntityFolder = null;
		/// <summary>
		/// 获取或设置 实体类所在项目的文件夹名称，包含项目下的全路径(但不包含项目的文件夹路径)。
		/// </summary>
		/// <value>The string value assigned to the entity project folder name</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDescription("PersistentDescription_EntityFolder")]
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(true)]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityFolder")]
		[System.ComponentModel.Editor(typeof(ProjectFolderEditor), typeof(UITypeEditor))]
		public string EntityFolder
		{
			get { return _EntityFolder; }
			set
			{
				if (_EntityFolder != value)
				{
					_EntityFolder = value;
					RaisePropertyChanged("EntityFolder");
				}
			}
		}

		/// <summary>
		/// 返回当前实例的可选择对象。
		/// </summary>
		/// <value>当前实例的 System.Collections.ICollection 类可选择对象。</value>
		protected internal override ICollection GetSelectedObjects
		{
			get { return new object[] { new PersistentDescriptor(this) }; }
		}

		/// <summary>
		/// 设置当前实例添加入 System.Collections.IList 的可选择对象中。
		/// </summary>
		/// <param name="selectionList">选择器可选择对象集合</param>
		protected internal override IList SetSelectedObjects(IList selectionList)
		{
			selectionList.Add(new PersistentDescriptor(this));
			foreach (DataEntityElement entity in _DataEntityElements)
			{
				selectionList.Add(new ObjectDescriptor<DataEntityElement>(entity));
				foreach (DataCommandElement element in entity.DataCommands)
				{
					selectionList.Add(new ObjectDescriptor<DataCommandElement>(element));
				}
			}
			return selectionList;
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
		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			base.OnFileContentChanged(EventArgs.Empty);
			if (CollectionChanged != null) { CollectionChanged(sender, e); }
		}

		#region

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_TableName")]
		[Basic.Designer.PersistentDescription("PersistentDescription_TableName")]
		public string TableName
		{
			get { return _TableInfo.TableName; }
			internal set { _TableInfo.TableName = value; }
		}

		/// <summary>
		/// 当前配置文件关联的数据库视图名称
		/// </summary>
		/// <value>数据库视图名称。</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_ViewName")]
		[Basic.Designer.PersistentDescription("PersistentDescription_ViewName")]
		public string ViewName
		{
			get { return _TableInfo.ViewName; }
			set { _TableInfo.ViewName = value; }
		}

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityName")]
		[Basic.Designer.PersistentDescription("PersistentDescription_EntityName")]
		public string PersistentName
		{
			get { return _TableInfo.EntityName; }
			set { _TableInfo.EntityName = value; }
		}

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Description")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Description")]
		public string Description
		{
			get { return _TableInfo.Description; }
			set { _TableInfo.Description = value; }
		}

		/// <summary>
		/// 数据库列表列集合.
		/// </summary>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Columns")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Columns")]
		public DesignColumnCollection Columns { get { return _TableInfo.Columns; } }

		/// <summary>
		/// 数据库表信息
		/// </summary>
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDescription("PersistentDescription_TableInfo")]
		[System.ComponentModel.Browsable(false)]
		public DesignTableInfo TableInfo { get { return _TableInfo; } }
		#endregion

		private DataEntityElement _NewEntity = null;
		/// <summary>
		/// 表示新增实体类类型
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		internal DataEntityElement NewEntity { get { return _NewEntity; } }

		private DataEntityElement _EditEntity = null;
		/// <summary>
		/// 表示修改实体类类型
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		internal DataEntityElement EditEntity { get { return _EditEntity; } }

		private DataEntityElement _DeleteEntity = null;
		/// <summary>
		/// 表示删除实体类类型
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		internal DataEntityElement DeleteEntity { get { return _DeleteEntity; } }

		private DataEntityElement _SearchEntity = null;
		/// <summary>
		/// 表示查询实体类类型
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		internal DataEntityElement SearchEntity { get { return _SearchEntity; } }

		private string _Namespace = null;
		/// <summary>
		/// 获取或设置当前配置文件生成代码时使用的命名空间。
		/// </summary>
		/// <value>当前配置文件生成代码时使用的命名空间，默认值为空字符串。</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PropertyDescription_NameSpace")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_Namespace")]
		public string Namespace
		{
			get { return _Namespace; }
			set
			{
				if (_Namespace != value)
				{
					_Namespace = value;
					base.RaisePropertyChanged("Namespace");
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
		[System.ComponentModel.Browsable(false)]
		public bool NamespaceChanged { get { return _OldNamespace != _Namespace; } }

		/// <summary>
		/// 
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public string EntityOldNamespace { get { return _DataEntityElements.OldNamespace; } }

		/// <summary>
		/// 判断命名空间自上次保存后是否已更改。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool EntityNamespaceChanged { get { return _DataEntityElements.NamespaceChanged; } }

		/// <summary>
		/// 获取或设置当前配置文件生成代码时使用的命名空间。
		/// </summary>
		/// <value>当前配置文件生成代码时使用的命名空间，默认值为空字符串。</value>
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PropertyDescription_EntityNamespace")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityNamespace")]
		public string EntityNamespace
		{
			get { return _DataEntityElements.Namespace; }
			set
			{
				if (_DataEntityElements.Namespace != value)
				{
					_DataEntityElements.Namespace = value;
					base.RaisePropertyChanged("EntityNamespace");
				}
			}
		}

		/// <summary>
		/// 获取或设置当前配置文件生成代码时使用的命名空间
		/// </summary>
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PropertyDescription_EntityNamespace")]
		[System.ComponentModel.DisplayName("Namespaces")]
		public NamespaceCollection ImportNameSpaces { get { return _ImportNamespaces; } }

		/// <summary>
		/// 当前配置文件关联的数据库表名称
		/// </summary>
		[System.ComponentModel.Description("表示当前数据持久类编辑器版本号。")]
		[System.ComponentModel.Browsable(false)]
		public Version Version { get { return _Version; } }

		/// <summary>
		/// 创建作为当前实例副本的新对象。
		/// </summary>
		/// <returns>作为此实例副本的新对象。</returns>
		protected override AbstractCustomTypeDescriptor Clone() { return this; }

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		protected override void Dispose() { }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return XmlConfigNamespace; } }

		/// <summary>
		/// 获取当前节点元素前缀
		/// </summary>
		protected internal override string ElementPrefix { get { return XmlElementPrefix; } }

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
			if (name == VersionAttribute) { _Version = Version.Parse(value); return true; }
			else if (name == PersistentGeneratorElement.ModifierAttribute)
			{
				ClassModifierEnum _Modifier = _Generator.Modifier;
				if (Enum.TryParse<ClassModifierEnum>(value, out _Modifier))
					_Generator.Modifier = _Modifier;
				return true;
			}
			else if (name == PersistentGeneratorElement.GenerateAttribute)
			{
				GenerateActionEnum _Generate = _Generator.GenerateMode;
				if (Enum.TryParse<GenerateActionEnum>(value, out _Generate))
					_Generator.GenerateMode = _Generate;
				return true;
			}
			else if (name == PersistentGeneratorElement.AccessAttribute)
			{
				_Generator.BaseAccess = value;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString(VersionAttribute, Version.ToString());
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == TableNameElement)
			{
				_TableInfo.TableName = reader.ReadString(); return false;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ViewNameElement)
			{
				_TableInfo.ViewName = reader.ReadString(); return false;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ProjectInfo.XmlElementName)
			{
				_ProjectInfo.ReadXml(reader.ReadSubtree()); return false;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == EntityFolderElement)
			{
				_EntityFolder = reader.ReadString(); return false;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == MessageInfo.XmlElementName)
			{
				_MessageInfo.ReadXml(reader.ReadSubtree());
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == PersistentGeneratorElement.XmlElementName)
			{
				_Generator.ReadXml(reader.ReadSubtree()); return false;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DesignTableInfo.XmlElementName)
			{
				_TableInfo.ReadXml(reader.ReadSubtree()); return false;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataEntityElementCollection.XmlElementName)
			{
				_DataEntityElements.ReadXml(reader.ReadSubtree()); return false;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == NamespaceElement)
			{
				_Namespace = reader.ReadString();
				_OldNamespace = _Namespace;
				if (string.IsNullOrWhiteSpace(_DataEntityElements.Namespace))
					_DataEntityElements.Namespace = _Namespace;
				return false;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == NamespacesElement)
			{
				System.Xml.XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())  //读取所有静态命令节点信息
				{
					if (reader.NodeType == XmlNodeType.Element && reader.LocalName == NamespaceElement)
						this._ImportNamespaces.Add(reader2.ReadString());
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.LocalName == NamespacesElement)
						return false;
				}
			}

			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlElementName)
			{
				if (string.IsNullOrEmpty(_MessageInfo.GroupName))
					_MessageInfo.GroupName = _TableInfo.EntityName;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			if (_ProjectInfo.NotEmpty) { _ProjectInfo.WriteXml(writer); }
			if (!string.IsNullOrWhiteSpace(_EntityFolder))
				writer.WriteElementString(XmlElementPrefix, EntityFolderElement, XmlConfigNamespace, _EntityFolder);
			_MessageInfo.WriteXml(writer);
			_Generator.WriteXml(writer);
			_TableInfo.WriteXml(writer);
			writer.WriteElementString(XmlElementPrefix, NamespaceElement, XmlConfigNamespace, _Namespace);
			writer.WriteStartElement(XmlElementPrefix, NamespacesElement, XmlConfigNamespace);
			if (_ImportNamespaces != null && _ImportNamespaces.Count > 0)
			{
				foreach (string ns in _ImportNamespaces)
				{
					writer.WriteElementString(NamespaceElement, ns);
				}
			}
			writer.WriteEndElement();
			_DataEntityElements.WriteXml(writer);
		}

		/// <summary>
		/// 当读取器遇到验证错误时发生。
		/// </summary>
		public event XmlSchemaEventHandler XmlSchemaError;

		private void OnXmlSchemaError(object sender, XmlSchemaEventArgs ex)
		{
			if (XmlSchemaError != null)
				XmlSchemaError(sender, new XmlSchemaEventArgs(ex.Exception, ex.Severity));
		}

		private void OnValidationEvent(object sender, ValidationEventArgs ex)
		{
			OnXmlSchemaError(sender, new XmlSchemaEventArgs(ex.Exception, ex.Severity));
		}

		private XmlReaderSettings CreateXmlReaderSettings()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.XmlResolver = null;
			//settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(OnValidationEvent);
			//Type emdlType = GetType();
			//string xsdSource = string.Format("{0}.BasicPersistentSchema-5.0.xsd", emdlType.Namespace);
			//Stream schema = emdlType.Assembly.GetManifestResourceStream(xsdSource);
			//using (XmlTextReader schemaReader = new XmlTextReader(schema))
			//{
			//	try
			//	{
			//		settings.Schemas.Add("http://dev.goldsoft.com/2013/BasicPersistentSchema-5.0.xsd", schemaReader);
			//	}
			//	catch (XmlException xex)
			//	{
			//		XmlSchemaException xse = new XmlSchemaException(xex.Message, xex, xex.LineNumber, xex.LinePosition);
			//		OnXmlSchemaError(this, new XmlSchemaEventArgs(xse, XmlSeverityType.Error));
			//	}
			//	catch (XmlSchemaException xsex)
			//	{
			//		OnXmlSchemaError(this, new XmlSchemaEventArgs(xsex, XmlSeverityType.Error));
			//	}
			//}
			return settings;
		}

		/// <summary>
		/// 将一序列化的文件转换成实体对象
		/// </summary>
		/// <param name="stream"></param>
		public void VerifyEntityModelSchema(TextReader reader, string baseUri)
		{
			XmlReaderSettings settings = CreateXmlReaderSettings();
			using (XmlReader xmlReader = XmlReader.Create(reader, settings, baseUri))
			{
				this.ReadXml(xmlReader);
			}
		}

		/// <summary>
		/// 清理配置文件内容
		/// </summary>
		public void ClearContent()
		{
			_ImportNamespaces.Clear();
			_DataEntityElements.Clear();
		}

		/// <summary>
		/// 将一序列化的文件转换成实体对象
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(TextReader reader)
		{
			XmlReaderSettings settings = CreateXmlReaderSettings();
			using (XmlReader xmlReader = XmlReader.Create(reader))
			{
				ClearContent();
				this.ReadXml(xmlReader);
			}
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeComplieUnit">表示需要写入代码的命名空间</param>
		/// <param name="provider">代码生成器和代码编译器的实例(一般为CSharpCodeProvider或VBCodeProvider)。</param>
		internal void WriteContextCode(System.CodeDom.CodeCompileUnit codeComplieUnit, CodeDomProvider provider)
		{
			// Just for VB.NET:
			// Option Strict On (controls whether implicit type conversions are allowed)
			codeComplieUnit.UserData.Add("AllowLateBound", false);
			// Option Explicit On (controls whether variable declarations are required)
			codeComplieUnit.UserData.Add("RequireVariableDeclaration", true);
			CodeNamespace codeNamespace = new CodeNamespace(Namespace);
			//codeNamespace.Comments.Add(new CodeCommentStatement("<summary>", true));
			//codeNamespace.Comments.Add(new CodeCommentStatement(string.Format("{0}", entityNs), true));
			//codeNamespace.Comments.Add(new CodeCommentStatement("</summary>", true));
			codeComplieUnit.Namespaces.Add(codeNamespace);
			if (!_ImportNamespaces.Contains("System.Transactions"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Transactions"));
			if (!_ImportNamespaces.Contains("System.Threading.Tasks"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Threading.Tasks"));
			if (!_ImportNamespaces.Contains("Basic.Expressions"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Expressions"));
			if (!_ImportNamespaces.Contains("Basic.DataAccess"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.DataAccess"));
			foreach (string ns in _ImportNamespaces)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(ns));
			}
			CodeTypeDeclaration persistentCode = new CodeTypeDeclaration(ContextName);//?????????????????????
			persistentCode.Comments.Add(new CodeCommentStatement("<summary>", true));
			persistentCode.Comments.Add(new CodeCommentStatement(string.Format("{0}", _TableInfo.Description), true));
			persistentCode.Comments.Add(new CodeCommentStatement("</summary>", true));
			persistentCode.IsPartial = true;
			persistentCode.IsClass = true;
			persistentCode.TypeAttributes = persistentCode.TypeAttributes | TypeAttributes.Public;
			persistentCode.Attributes = persistentCode.Attributes | MemberAttributes.Public;

			CodeTypeReference codeType = new CodeTypeReference(typeof(ToolboxItemAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration codeAttribute = new CodeAttributeDeclaration(codeType);
			codeAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(false)));
			persistentCode.CustomAttributes.Add(codeAttribute);

			codeNamespace.Types.Add(persistentCode);
		}

		/// <summary>
		/// 实现设计时代码(designer.cs)
		/// </summary>
		/// <param name="codeComplieUnit">表示需要写入代码的命名空间</param>
		/// <param name="provider">代码生成器和代码编译器的实例(一般为CSharpCodeProvider或VBCodeProvider)。</param>
		internal void WriteContextDesignerCode(System.CodeDom.CodeCompileUnit codeComplieUnit, CodeDomProvider provider)
		{
			// Just for VB.NET:
			// Option Strict On (controls whether implicit type conversions are allowed)
			codeComplieUnit.UserData.Add("AllowLateBound", false);
			// Option Explicit On (controls whether variable declarations are required)
			codeComplieUnit.UserData.Add("RequireVariableDeclaration", true);
			CodeNamespace codeNamespace = new CodeNamespace(Namespace);
			codeComplieUnit.Namespaces.Add(codeNamespace);
			if (EntityNamespace != Namespace)
				codeNamespace.Imports.Add(new CodeNamespaceImport(EntityNamespace));
			if (!_ImportNamespaces.Contains("System.Transactions"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Transactions"));
			if (!_ImportNamespaces.Contains("System.Threading.Tasks"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Threading.Tasks"));
			if (!_ImportNamespaces.Contains("Basic.Expressions"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Expressions"));
			if (!_ImportNamespaces.Contains("Basic.DataAccess"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.DataAccess"));
			foreach (string ns in _ImportNamespaces)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(ns));
			}
			CodeTypeDeclaration contextCode = new CodeTypeDeclaration(ContextName);
			contextCode.Comments.Add(new CodeCommentStatement("<summary>", true));
			contextCode.Comments.Add(new CodeCommentStatement(string.Format("{0}", _TableInfo.Description), true));
			contextCode.Comments.Add(new CodeCommentStatement("</summary>", true));
			if (_Generator.BaseAccess.IndexOf("DbAccess") >= 0)
			{
				contextCode.BaseTypes.Add(new CodeTypeReference(typeof(AbstractDataContext), CodeTypeReferenceOptions.GlobalReference));
			}
			else if (_Generator.ApplyConnection == true)
				contextCode.BaseTypes.Add(new CodeTypeReference(typeof(AbstractContext), CodeTypeReferenceOptions.GlobalReference));
			else if (_Generator.ApplyConnection == false)
				contextCode.BaseTypes.Add(new CodeTypeReference(typeof(AbstractContext), CodeTypeReferenceOptions.GlobalReference));

			contextCode.IsPartial = true;
			contextCode.IsClass = true;
			contextCode.TypeAttributes = TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Public;
			contextCode.Attributes = contextCode.Attributes | MemberAttributes.Public;

			CodeConstructor constructor0 = new CodeConstructor();
			constructor0.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor0.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", ContextName), true));
			constructor0.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor0.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeMethodReferenceExpression baseConstructor0 = new CodeMethodReferenceExpression();
			constructor0.BaseConstructorArgs.Add(baseConstructor0);
			contextCode.Members.Add(constructor0);

			CodeConstructor constructor1 = new CodeConstructor();
			constructor1.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor1.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", ContextName), true));
			constructor1.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor1.Comments.Add(new CodeCommentStatement("<param name=\"connection\">数据库连接名称</param>", true));
			constructor1.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor1.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
			constructor1.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "connection" });
			contextCode.Members.Add(constructor1);

			CodeConstructor constructor2 = new CodeConstructor();
			constructor2.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor2.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", ContextName), true));
			constructor2.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor2.Comments.Add(new CodeCommentStatement("<param name=\"context\">用户上下文信息(包含语言、登录信息等)</param>", true));
			constructor2.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IUserContext), "context"));
			constructor2.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "context" });
			contextCode.Members.Add(constructor2);

			if (_Generator.BaseAccess.IndexOf("DbAccess") >= 0)
			{
				foreach (DataEntityElement entity in _DataEntityElements)
				{
					foreach (DataCommandElement command in entity.DataCommands)
					{
						if (command.Kind == ConfigurationTypeEnum.AddNew && command.AutoGenerated)
							command.WriteContextDesignerCode(contextCode.Members, this, provider);
						else if (command.Kind == ConfigurationTypeEnum.Modify && command.AutoGenerated)
							command.WriteContextDesignerCode(contextCode.Members, this, provider);
						else if (command.Kind == ConfigurationTypeEnum.Remove && command.AutoGenerated)
							command.WriteContextDesignerCode(contextCode.Members, this, provider);
						else if (command.Kind == ConfigurationTypeEnum.SelectByKey && command.AutoGenerated)
							command.WriteContextDesignerCode(contextCode.Members, this, provider);
					}
				}
			}
			else
			{
				#region 上下文数据持久类创建方法
				CodeTypeReference accessTypereference = new CodeTypeReference(AccessName);
				//CodeMemberMethod memberMethod0 = new CodeMemberMethod();
				//memberMethod0.Comments.Add(new CodeCommentStatement("<summary>", true));
				//memberMethod0.Comments.Add(new CodeCommentStatement(string.Format("创建 {0} 类的实例。", AccessName), true));
				//memberMethod0.Comments.Add(new CodeCommentStatement("</summary>", true));
				//memberMethod0.Comments.Add(new CodeCommentStatement(string.Format("<returns>返回 {0} 类的实例。</returns>", AccessName), true));
				//memberMethod0.Attributes = MemberAttributes.Private | MemberAttributes.Final;
				//memberMethod0.Name = string.Format("Create{0}", AccessName);
				//memberMethod0.ReturnType = accessTypereference;
				//CodeMethodReturnStatement returnMethod0 = new CodeMethodReturnStatement();
				//returnMethod0.Expression = new CodeObjectCreateExpression(accessTypereference);
				//memberMethod0.Statements.Add(returnMethod0);
				//contextCode.Members.Add(memberMethod0);

				//CodeMemberMethod memberMethod1 = new CodeMemberMethod();
				//memberMethod1.Comments.Add(new CodeCommentStatement("<summary>", true));
				//memberMethod1.Comments.Add(new CodeCommentStatement(string.Format("创建 {0} 类的实例。", AccessName), true));
				//memberMethod1.Comments.Add(new CodeCommentStatement("</summary>", true));
				//memberMethod1.Comments.Add(new CodeCommentStatement("<param name=\"startTransaction\">是否启用事务</param>", true));
				//memberMethod1.Comments.Add(new CodeCommentStatement(string.Format("<returns>返回 {0} 类的实例。</returns>", AccessName), true));
				//memberMethod1.Attributes = MemberAttributes.Private | MemberAttributes.Final;
				//memberMethod1.Name = string.Format("Create{0}", AccessName);
				//memberMethod1.ReturnType = accessTypereference;
				//memberMethod1.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "startTransaction"));
				//CodeMethodReturnStatement returnMethod1 = new CodeMethodReturnStatement();
				//CodeObjectCreateExpression objectCreate1 = new CodeObjectCreateExpression(accessTypereference);
				//objectCreate1.Parameters.Add(new CodeVariableReferenceExpression("startTransaction"));
				//returnMethod1.Expression = objectCreate1;
				//memberMethod1.Statements.Add(returnMethod1);
				//contextCode.Members.Add(memberMethod1);

				//CodeMemberMethod memberMethod3 = new CodeMemberMethod();
				//memberMethod3.Comments.Add(new CodeCommentStatement("<summary>", true));
				//memberMethod3.Comments.Add(new CodeCommentStatement(string.Format("创建 {0} 类的实例。", AccessName), true));
				//memberMethod3.Comments.Add(new CodeCommentStatement("</summary>", true));
				//memberMethod3.Comments.Add(new CodeCommentStatement("<param name=\"transaction\">对用于登记的现有 Transaction 的引用。</param>", true));
				//memberMethod3.Comments.Add(new CodeCommentStatement(string.Format("<returns>返回 {0} 类的实例。</returns>", AccessName), true));
				//memberMethod3.Attributes = MemberAttributes.Private | MemberAttributes.Final;
				//memberMethod3.Name = string.Format("Create{0}", AccessName);
				//memberMethod3.ReturnType = accessTypereference;
				//CodeTypeReference ctReference = new CodeTypeReference(typeof(CommittableTransaction), CodeTypeReferenceOptions.GlobalReference);
				//memberMethod3.Parameters.Add(new CodeParameterDeclarationExpression(ctReference, "transaction"));
				//CodeMethodReturnStatement returnMethod3 = new CodeMethodReturnStatement();
				//CodeObjectCreateExpression objectCreate3 = new CodeObjectCreateExpression(accessTypereference);
				//objectCreate3.Parameters.Add(new CodeVariableReferenceExpression("transaction"));
				//returnMethod3.Expression = objectCreate3;
				//memberMethod3.Statements.Add(returnMethod3);
				//contextCode.Members.Add(memberMethod3);

				//CodeMemberMethod memberMethod4 = new CodeMemberMethod();
				//memberMethod4.Comments.Add(new CodeCommentStatement("<summary>", true));
				//memberMethod4.Comments.Add(new CodeCommentStatement(string.Format("创建 {0} 类的实例。", AccessName), true));
				//memberMethod4.Comments.Add(new CodeCommentStatement("</summary>", true));
				//memberMethod4.Comments.Add(new CodeCommentStatement("<param name=\"access\">数据持久类实例。</param>", true));
				//memberMethod4.Comments.Add(new CodeCommentStatement(string.Format("<returns>返回 {0} 类的实例。</returns>", AccessName), true));
				//memberMethod4.Attributes = MemberAttributes.Private | MemberAttributes.Final;
				//memberMethod4.Name = string.Format("Create{0}", AccessName);
				//memberMethod4.ReturnType = accessTypereference;
				//memberMethod4.Parameters.Add(new CodeParameterDeclarationExpression(typeof(AbstractDbAccess).Name, "access"));
				//CodeMethodReturnStatement returnMethod4 = new CodeMethodReturnStatement();
				//CodeObjectCreateExpression objectCreate4 = new CodeObjectCreateExpression(accessTypereference);
				//objectCreate4.Parameters.Add(new CodeVariableReferenceExpression("access"));
				//returnMethod4.Expression = objectCreate4;
				//memberMethod4.Statements.Add(returnMethod4);
				//contextCode.Members.Add(memberMethod4);

				CodeMemberMethod memberMethod5 = new CodeMemberMethod();
				memberMethod5.Comments.Add(new CodeCommentStatement("<summary>", true));
				memberMethod5.Comments.Add(new CodeCommentStatement(string.Format("创建 {0} 类的实例。", AccessName), true));
				memberMethod5.Comments.Add(new CodeCommentStatement("</summary>", true));
				memberMethod5.Comments.Add(new CodeCommentStatement("<param name=\"connection\">基础框架配置的数据库连接名称</param>", true));
				memberMethod5.Comments.Add(new CodeCommentStatement(string.Format("<returns>返回 {0} 类的实例。</returns>", AccessName), true));
				memberMethod5.Attributes = MemberAttributes.Family | MemberAttributes.Override;
				memberMethod5.Name = "CreateAccess";
				memberMethod5.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
				memberMethod5.ReturnType = new CodeTypeReference("AbstractAccess");

				CodeObjectCreateExpression objectCreate5 = new CodeObjectCreateExpression(accessTypereference);
				objectCreate5.Parameters.Add(new CodeVariableReferenceExpression("connection"));
				CodeMethodReturnStatement returnMethod5 = new CodeMethodReturnStatement();
				returnMethod5.Expression = objectCreate5;
				memberMethod5.Statements.Add(returnMethod5);
				contextCode.Members.Add(memberMethod5);

				CodeMemberMethod memberMethod6 = new CodeMemberMethod();
				memberMethod6.Comments.Add(new CodeCommentStatement("<summary>", true));
				memberMethod6.Comments.Add(new CodeCommentStatement(string.Format("创建 {0} 类的实例。", AccessName), true));
				memberMethod6.Comments.Add(new CodeCommentStatement("</summary>", true));
				memberMethod6.Comments.Add(new CodeCommentStatement("<param name=\"connection\">基础框架配置的数据库连接名称</param>", true));
				memberMethod6.Comments.Add(new CodeCommentStatement("<param name=\"startTransaction\">是否启用事务</param>", true));
				memberMethod6.Comments.Add(new CodeCommentStatement(string.Format("<returns>返回 {0} 类的实例。</returns>", AccessName), true));
				memberMethod6.Attributes = MemberAttributes.Family | MemberAttributes.Override;
				memberMethod6.Name = "CreateAccess";
				memberMethod6.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
				memberMethod6.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "startTransaction"));
				memberMethod6.ReturnType = new CodeTypeReference("AbstractAccess");

				CodeObjectCreateExpression objectCreate6 = new CodeObjectCreateExpression(accessTypereference);
				objectCreate6.Parameters.Add(new CodeVariableReferenceExpression("connection"));
				objectCreate6.Parameters.Add(new CodeVariableReferenceExpression("startTransaction"));
				CodeMethodReturnStatement returnMethod6 = new CodeMethodReturnStatement();
				returnMethod6.Expression = objectCreate6;
				memberMethod6.Statements.Add(returnMethod6);
				contextCode.Members.Add(memberMethod6);

				CodeMemberMethod memberMethod7 = new CodeMemberMethod();
				memberMethod7.Comments.Add(new CodeCommentStatement("<summary>", true));
				memberMethod7.Comments.Add(new CodeCommentStatement(string.Format("使用指定的事物隔离级别，创建 {0} 类的实例。", AccessName), true));
				memberMethod7.Comments.Add(new CodeCommentStatement("</summary>", true));
				memberMethod7.Comments.Add(new CodeCommentStatement("<param name=\"connection\">基础框架配置的数据库连接名称</param>", true));
				memberMethod7.Comments.Add(new CodeCommentStatement("<param name=\"isolationLevel\">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>", true));
				memberMethod7.Comments.Add(new CodeCommentStatement(string.Format("<returns>返回 {0} 类的实例。</returns>", AccessName), true));
				memberMethod7.Attributes = MemberAttributes.Family | MemberAttributes.Override;
				memberMethod7.Name = "CreateAccess";
				memberMethod7.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
				memberMethod7.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IsolationLevel), "isolationLevel"));
				memberMethod7.ReturnType = new CodeTypeReference("AbstractAccess");

				CodeTypeReferenceExpression timeSpanReference = new CodeTypeReferenceExpression(typeof(TimeSpan));

				CodeObjectCreateExpression objectCreate7 = new CodeObjectCreateExpression(accessTypereference);
				objectCreate7.Parameters.Add(new CodeVariableReferenceExpression("connection"));
				objectCreate7.Parameters.Add(new CodeVariableReferenceExpression("isolationLevel"));
				objectCreate7.Parameters.Add(new CodeMethodInvokeExpression(timeSpanReference, "FromSeconds", new CodePrimitiveExpression(30)));
				memberMethod7.Statements.Add(new CodeMethodReturnStatement() { Expression = objectCreate7 });
				contextCode.Members.Add(memberMethod7);

				CodeMemberMethod memberMethod8 = new CodeMemberMethod();
				memberMethod8.Comments.Add(new CodeCommentStatement("<summary>", true));
				memberMethod8.Comments.Add(new CodeCommentStatement(string.Format("使用指定的事物隔离级别和事务超时时间，创建 {0} 类的实例。", AccessName), true));
				memberMethod8.Comments.Add(new CodeCommentStatement("</summary>", true));
				memberMethod8.Comments.Add(new CodeCommentStatement("<param name=\"connection\">基础框架配置的数据库连接名称</param>", true));
				memberMethod8.Comments.Add(new CodeCommentStatement("<param name=\"isolationLevel\">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>", true));
				memberMethod8.Comments.Add(new CodeCommentStatement("<param name=\"second\">一个 int 类型的值，该值表示事务 <see cref=\"System.Transactions.CommittableTransaction\"/> 的超时时间限制，单位秒。</param>", true));
				memberMethod8.Comments.Add(new CodeCommentStatement(string.Format("<returns>返回 {0} 类的实例。</returns>", AccessName), true));
				memberMethod8.Attributes = MemberAttributes.Family | MemberAttributes.Override;
				memberMethod8.Name = "CreateAccess";
				memberMethod8.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
				memberMethod8.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IsolationLevel), "isolationLevel"));
				memberMethod8.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "second"));
				memberMethod8.ReturnType = new CodeTypeReference("AbstractAccess");

				CodeObjectCreateExpression objectCreate8 = new CodeObjectCreateExpression(accessTypereference);
				objectCreate8.Parameters.Add(new CodeVariableReferenceExpression("connection"));
				objectCreate8.Parameters.Add(new CodeVariableReferenceExpression("isolationLevel"));
				objectCreate8.Parameters.Add(new CodeMethodInvokeExpression(timeSpanReference, "FromSeconds", new CodeVariableReferenceExpression("second")));
				memberMethod8.Statements.Add(new CodeMethodReturnStatement() { Expression = objectCreate8 });
				contextCode.Members.Add(memberMethod8);
				#endregion
			}
			codeNamespace.Types.Add(contextCode);
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeComplieUnit">表示需要写入代码的命名空间</param>
		internal void WriteAccessCode(System.CodeDom.CodeCompileUnit codeComplieUnit)
		{
			// Just for VB.NET:
			// Option Strict On (controls whether implicit type conversions are allowed)
			codeComplieUnit.UserData.Add("AllowLateBound", false);
			// Option Explicit On (controls whether variable declarations are required)
			codeComplieUnit.UserData.Add("RequireVariableDeclaration", true);
			CodeNamespace codeNamespace = new CodeNamespace(Namespace);
			//codeNamespace.Comments.Add(new CodeCommentStatement("<summary>", true));
			//codeNamespace.Comments.Add(new CodeCommentStatement(string.Format("{0}", entityNs), true));
			//codeNamespace.Comments.Add(new CodeCommentStatement("</summary>", true));
			codeComplieUnit.Namespaces.Add(codeNamespace);
			if (!_ImportNamespaces.Contains("System.Transactions"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Transactions"));
			if (!_ImportNamespaces.Contains("System.Threading.Tasks"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Threading.Tasks"));
			foreach (string ns in _ImportNamespaces)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(ns));
			}
			if (!_ImportNamespaces.Contains("Basic.Expressions"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Expressions"));
			if (!_ImportNamespaces.Contains("Basic.DataAccess"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.DataAccess"));
			CodeTypeDeclaration persistentCode = new CodeTypeDeclaration(AccessName);//?????????????????????
			persistentCode.Comments.Add(new CodeCommentStatement("<summary>", true));
			persistentCode.Comments.Add(new CodeCommentStatement(string.Format("{0}", _TableInfo.Description), true));
			persistentCode.Comments.Add(new CodeCommentStatement("</summary>", true));
			persistentCode.IsPartial = true;
			persistentCode.IsClass = true;
			if (_Generator.Modifier == ClassModifierEnum.Internal)
			{
				persistentCode.TypeAttributes = persistentCode.TypeAttributes | TypeAttributes.NestedAssembly;
				persistentCode.Attributes = persistentCode.Attributes | MemberAttributes.Assembly;
			}
			else if (_Generator.Modifier == ClassModifierEnum.Public)
			{
				persistentCode.TypeAttributes = persistentCode.TypeAttributes | TypeAttributes.Public;
				persistentCode.Attributes = persistentCode.Attributes | MemberAttributes.Public;
			}
			codeNamespace.Types.Add(persistentCode);
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeComplieUnit">表示需要写入代码的命名空间</param>
		internal void WriteAccessDesignerCode(System.CodeDom.CodeCompileUnit codeComplieUnit, string modelName, int targetFramework)
		{
			// Just for VB.NET:
			// Option Strict On (controls whether implicit type conversions are allowed)
			codeComplieUnit.UserData.Add("AllowLateBound", false);
			// Option Explicit On (controls whether variable declarations are required)
			codeComplieUnit.UserData.Add("RequireVariableDeclaration", true);
			CodeNamespace codeNamespace = new CodeNamespace(Namespace);
			if (!_ImportNamespaces.Contains("System.Transactions"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Transactions"));
			if (!_ImportNamespaces.Contains("System.Threading.Tasks"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Threading.Tasks"));
			codeComplieUnit.Namespaces.Add(codeNamespace);
			if (EntityNamespace != Namespace)
				codeNamespace.Imports.Add(new CodeNamespaceImport(EntityNamespace));
			foreach (string ns in _ImportNamespaces)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(ns));
			}
			if (!_ImportNamespaces.Contains("Basic.Expressions"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Expressions"));
			if (!_ImportNamespaces.Contains("Basic.DataAccess"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.DataAccess"));
			CodeTypeDeclaration accessCode = new CodeTypeDeclaration(AccessName);//?????????????????????
			accessCode.Comments.Add(new CodeCommentStatement("<summary>", true));
			accessCode.Comments.Add(new CodeCommentStatement(string.Format("{0}", _TableInfo.Description), true));
			accessCode.Comments.Add(new CodeCommentStatement("</summary>", true));
			if (_Generator.BaseAccess == typeof(AbstractDbAccess).Name)
				accessCode.BaseTypes.Add(new CodeTypeReference(typeof(AbstractDbAccess), CodeTypeReferenceOptions.GlobalReference));
			else if (_Generator.BaseAccess == typeof(AbstractAccess).Name)
				accessCode.BaseTypes.Add(new CodeTypeReference(typeof(AbstractAccess), CodeTypeReferenceOptions.GlobalReference));
			else
				accessCode.BaseTypes.Add(new CodeTypeReference(_Generator.BaseAccess, CodeTypeReferenceOptions.GlobalReference));
			accessCode.IsPartial = true;
			accessCode.IsClass = true;
			accessCode.TypeAttributes = TypeAttributes.Class | TypeAttributes.Sealed;
			if (_Generator.Modifier == ClassModifierEnum.Internal)
			{
				accessCode.TypeAttributes = accessCode.TypeAttributes | TypeAttributes.NestedAssembly;
				accessCode.Attributes = accessCode.Attributes | MemberAttributes.Assembly;
			}
			else if (_Generator.Modifier == ClassModifierEnum.Public)
			{
				accessCode.TypeAttributes = accessCode.TypeAttributes | TypeAttributes.Public;
				accessCode.Attributes = accessCode.Attributes | MemberAttributes.Public;
			}
			CodeTypeReference codeType = new CodeTypeReference(typeof(ConfigurationAttribute).Name);
			CodeAttributeDeclaration codeAttribute = new CodeAttributeDeclaration(codeType);
			if (!string.IsNullOrWhiteSpace(modelName))
				codeAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(modelName)));
			codeAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(TableName)));
			if (ResxMode == ResxModeEnum.Resource)
			{
				CodeFieldReferenceExpression fieldTypeExpress = new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression(typeof(ConfigFileType).Name), "Resource");
				codeAttribute.Arguments.Add(new CodeAttributeArgument(fieldTypeExpress));
			}
			else if (ResxMode == ResxModeEnum.AssemlyResource)
			{
				CodeFieldReferenceExpression fieldTypeExpress = new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression(typeof(ConfigFileType).Name), "AssemlyResource");
				codeAttribute.Arguments.Add(new CodeAttributeArgument(fieldTypeExpress));
			}
			else
			{
				CodeFieldReferenceExpression fieldTypeExpress = new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression(typeof(ConfigFileType).Name), "NotSet");
				codeAttribute.Arguments.Add(new CodeAttributeArgument(fieldTypeExpress));
			}
			accessCode.CustomAttributes.Add(codeAttribute);

			#region 构造函数
			CodeTypeReference ctrUserContext = new CodeTypeReference(typeof(IUserContext), CodeTypeReferenceOptions.GlobalReference);
			CodeTypeReference ctrCommittable = new CodeTypeReference(typeof(CommittableTransaction), CodeTypeReferenceOptions.GlobalReference);

			CodeConstructor constructor0 = new CodeConstructor();
			constructor0.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor0.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor0.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor0.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeMethodReferenceExpression baseConstructor0 = new CodeMethodReferenceExpression();
			constructor0.BaseConstructorArgs.Add(baseConstructor0);
			accessCode.Members.Add(constructor0);

			CodeConstructor constructor1 = new CodeConstructor();
			constructor1.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor1.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor1.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor1.Comments.Add(new CodeCommentStatement("<param name=\"startTransaction\">是否启用事务</param>", true));
			constructor1.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor1.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "startTransaction"));
			constructor1.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "startTransaction" });
			accessCode.Members.Add(constructor1);

			CodeConstructor constructor2 = new CodeConstructor();
			constructor2.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor2.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor2.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor2.Comments.Add(new CodeCommentStatement("<param name=\"access\">抽象类 AbstractAccess 子类实例</param>", true));
			constructor2.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeTypeReference dbAccessReference = new CodeTypeReference(typeof(AbstractDbAccess), CodeTypeReferenceOptions.GlobalReference);
			constructor2.Parameters.Add(new CodeParameterDeclarationExpression(dbAccessReference, "access"));
			constructor2.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "access" });
			accessCode.Members.Add(constructor2);

			CodeConstructor constructor3 = new CodeConstructor();
			constructor3.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor3.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor3.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor3.Comments.Add(new CodeCommentStatement("<param name=\"transaction\">对用于登记的现有 Transaction 的引用。</param>", true));
			constructor3.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor3.Parameters.Add(new CodeParameterDeclarationExpression(ctrCommittable, "transaction"));
			constructor3.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "transaction" });
			accessCode.Members.Add(constructor3);

			CodeConstructor constructor4 = new CodeConstructor();
			constructor4.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor4.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor4.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor4.Comments.Add(new CodeCommentStatement("<param name=\"timeout\">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>", true));
			constructor4.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeTypeReference timespanReference4 = new CodeTypeReference(typeof(TimeSpan), CodeTypeReferenceOptions.GlobalReference);
			constructor4.Parameters.Add(new CodeParameterDeclarationExpression(timespanReference4, "timeout"));
			constructor4.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "timeout" });
			accessCode.Members.Add(constructor4);

			CodeConstructor constructor5 = new CodeConstructor();
			constructor5.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor5.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor5.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor5.Comments.Add(new CodeCommentStatement("<param name=\"isolationLevel\">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>", true));
			constructor5.Comments.Add(new CodeCommentStatement("<param name=\"timeout\">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>", true));
			constructor5.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeTypeReference isolationLevelReference5 = new CodeTypeReference(typeof(IsolationLevel), CodeTypeReferenceOptions.GlobalReference);
			CodeTypeReference timespanReference5 = new CodeTypeReference(typeof(TimeSpan), CodeTypeReferenceOptions.GlobalReference);
			constructor5.Parameters.Add(new CodeParameterDeclarationExpression(isolationLevelReference5, "isolationLevel"));
			constructor5.Parameters.Add(new CodeParameterDeclarationExpression(timespanReference5, "timeout"));
			constructor5.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "isolationLevel" });
			constructor5.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "timeout" });
			accessCode.Members.Add(constructor5);

			CodeConstructor constructor11 = new CodeConstructor();
			constructor11.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor11.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor11.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor11.Comments.Add(new CodeCommentStatement("<param name=\"connection\">数据库连接名称</param>", true));
			constructor11.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor11.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
			constructor11.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "connection" });
			accessCode.Members.Add(constructor11);

			CodeConstructor constructor12 = new CodeConstructor();
			constructor12.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor12.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor12.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor12.Comments.Add(new CodeCommentStatement("<param name=\"connection\">数据库连接名称</param>", true));
			constructor12.Comments.Add(new CodeCommentStatement("<param name=\"startTransaction\">是否启用事务</param>", true));
			constructor12.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor12.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
			constructor12.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "startTransaction"));
			constructor12.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "connection" });
			constructor12.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "startTransaction" });
			accessCode.Members.Add(constructor12);

			CodeConstructor constructor13 = new CodeConstructor();
			constructor13.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor13.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor13.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor13.Comments.Add(new CodeCommentStatement("<param name=\"connection\">数据库连接名称</param>", true));
			constructor13.Comments.Add(new CodeCommentStatement("<param name=\"transaction\">对用于登记的现有 Transaction 的引用。</param>", true));
			constructor13.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor13.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
			constructor13.Parameters.Add(new CodeParameterDeclarationExpression(ctrCommittable, "transaction"));
			constructor13.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "connection" });
			constructor13.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "transaction" });
			accessCode.Members.Add(constructor13);

			CodeConstructor constructor14 = new CodeConstructor();
			constructor14.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor14.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor14.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor14.Comments.Add(new CodeCommentStatement("<param name=\"connection\">数据库连接名称</param>", true));
			constructor14.Comments.Add(new CodeCommentStatement("<param name=\"timeout\">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>", true));
			constructor14.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor14.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
			constructor14.Parameters.Add(new CodeParameterDeclarationExpression(timespanReference4, "timeout"));
			constructor14.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "connection" });
			constructor14.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "timeout" });
			accessCode.Members.Add(constructor14);

			CodeConstructor constructor15 = new CodeConstructor();
			constructor15.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor15.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor15.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor15.Comments.Add(new CodeCommentStatement("<param name=\"connection\">数据库连接名称</param>", true));
			constructor15.Comments.Add(new CodeCommentStatement("<param name=\"isolationLevel\">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>", true));
			constructor15.Comments.Add(new CodeCommentStatement("<param name=\"timeout\">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>", true));
			constructor15.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor15.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connection"));
			constructor15.Parameters.Add(new CodeParameterDeclarationExpression(isolationLevelReference5, "isolationLevel"));
			constructor15.Parameters.Add(new CodeParameterDeclarationExpression(timespanReference5, "timeout"));
			constructor15.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "connection" });
			constructor15.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "isolationLevel" });
			constructor15.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "timeout" });
			accessCode.Members.Add(constructor15);

			CodeConstructor constructor21 = new CodeConstructor();
			constructor21.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor21.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor21.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor21.Comments.Add(new CodeCommentStatement("<param name=\"user\">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>", true));
			constructor21.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor21.Parameters.Add(new CodeParameterDeclarationExpression(ctrUserContext, "user"));
			constructor21.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "user" });
			accessCode.Members.Add(constructor21);

			CodeConstructor constructor22 = new CodeConstructor();
			constructor22.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor22.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor22.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor22.Comments.Add(new CodeCommentStatement("<param name=\"user\">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>", true));
			constructor22.Comments.Add(new CodeCommentStatement("<param name=\"startTransaction\">是否启用事务</param>", true));
			constructor22.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor22.Parameters.Add(new CodeParameterDeclarationExpression(ctrUserContext, "user"));
			constructor22.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "startTransaction"));
			constructor22.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "user" });
			constructor22.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "startTransaction" });
			accessCode.Members.Add(constructor22);

			CodeConstructor constructor23 = new CodeConstructor();
			constructor23.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor23.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor23.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor23.Comments.Add(new CodeCommentStatement("<param name=\"user\">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>", true));
			constructor23.Comments.Add(new CodeCommentStatement("<param name=\"timeout\">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>", true));
			constructor23.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor23.Parameters.Add(new CodeParameterDeclarationExpression(ctrUserContext, "user"));
			constructor23.Parameters.Add(new CodeParameterDeclarationExpression(timespanReference5, "timeout"));
			constructor23.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "user" });
			constructor23.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "timeout" });
			accessCode.Members.Add(constructor23);

			CodeConstructor constructor24 = new CodeConstructor();
			constructor24.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor24.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor24.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor24.Comments.Add(new CodeCommentStatement("<param name=\"user\">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>", true));
			constructor24.Comments.Add(new CodeCommentStatement("<param name=\"transaction\">对用于登记的现有 Transaction 的引用。</param>", true));
			constructor24.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor24.Parameters.Add(new CodeParameterDeclarationExpression(ctrUserContext, "user"));
			constructor24.Parameters.Add(new CodeParameterDeclarationExpression(ctrCommittable, "transaction"));
			constructor24.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "user" });
			constructor24.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "transaction" });
			accessCode.Members.Add(constructor24);

			CodeConstructor constructor25 = new CodeConstructor();
			constructor25.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor25.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", AccessName), true));
			constructor25.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor25.Comments.Add(new CodeCommentStatement("<param name=\"user\">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>", true));
			constructor25.Comments.Add(new CodeCommentStatement("<param name=\"isolationLevel\">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>", true));
			constructor25.Comments.Add(new CodeCommentStatement("<param name=\"timeout\">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>", true));
			constructor25.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor25.Parameters.Add(new CodeParameterDeclarationExpression(ctrUserContext, "user"));
			constructor25.Parameters.Add(new CodeParameterDeclarationExpression(isolationLevelReference5, "isolationLevel"));
			constructor25.Parameters.Add(new CodeParameterDeclarationExpression(timespanReference5, "timeout"));
			constructor25.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "user" });
			constructor25.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "isolationLevel" });
			constructor25.BaseConstructorArgs.Add(new CodeFieldReferenceExpression() { FieldName = "timeout" });
			accessCode.Members.Add(constructor25);
			#endregion

			foreach (DataEntityElement entity in _DataEntityElements)
			{
				foreach (DataCommandElement command in entity.DataCommands)
				{
					if (_Generator.BaseAccess.IndexOf("DbAccess") >= 0)
					{
						command.WriteDesignerCode(accessCode, targetFramework);
					}
					else
					{
						if (command.Kind == ConfigurationTypeEnum.Other)
							command.WriteDesignerCode(accessCode, targetFramework);
						else if (command.Kind == ConfigurationTypeEnum.Insert)
							command.WriteDesignerCode(accessCode, targetFramework);
						else if (command.Kind == ConfigurationTypeEnum.Update)
							command.WriteDesignerCode(accessCode, targetFramework);
						else if (command.Kind == ConfigurationTypeEnum.Delete)
							command.WriteDesignerCode(accessCode, targetFramework);
					}
					//else if (_Generator.BaseAccess == typeof(AbstractDbAccess).Name)
					//	command.WriteDesignerCode(accessCode, targetFramework);
					//else
					//	command.WriteDesignerCode(accessCode, targetFramework);
				}
			}
			codeNamespace.Types.Add(accessCode);
		}

		/// <summary>实现设计时代码</summary>
		/// <param name="codeComplieUnit">表示需要写入代码的命名空间</param>
		internal void WriteEntityDesignerCode(System.CodeDom.CodeCompileUnit codeComplieUnit)
		{
			// Just for VB.NET:
			// Option Strict On (controls whether implicit type conversions are allowed)
			codeComplieUnit.UserData.Add("AllowLateBound", false);
			// Option Explicit On (controls whether variable declarations are required)
			codeComplieUnit.UserData.Add("RequireVariableDeclaration", true);
			CodeNamespace codeNamespace = new CodeNamespace(EntityNamespace);
			codeComplieUnit.Namespaces.Add(codeNamespace);
			foreach (string ns in _ImportNamespaces)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(ns));
			}
			if (!_ImportNamespaces.Contains("Basic.EntityLayer"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.EntityLayer"));
			if (!_ImportNamespaces.Contains("Basic.Enums"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Enums"));
			if (!_ImportNamespaces.Contains("Basic.Interfaces"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Interfaces"));
			foreach (DataEntityElement entity in _DataEntityElements)
			{
				if (entity.Condition.Arguments.Count > 0 || entity.BaseCondition != typeof(AbstractCondition).FullName)
					entity.Condition.WriteEntityDesignerCode(codeNamespace);
				if (entity.GeneratorMode == GenerateModeEnum.DataEntity)
				{
					entity.WriteEntityDesignerCode(codeNamespace);
				}
				else if (entity.GeneratorMode == GenerateModeEnum.DataTable)
				{
					entity.WriteTableDesignerCode(codeNamespace);
				}
				else
				{
					entity.WriteEntityDesignerCode(codeNamespace);
					entity.WriteTableDesignerCode(codeNamespace);
				}
			}
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeComplieUnit">表示需要写入代码的命名空间</param>
		internal void WriteEntityCode(System.CodeDom.CodeCompileUnit codeComplieUnit)
		{
			// Just for VB.NET:
			// Option Strict On (controls whether implicit type conversions are allowed)
			codeComplieUnit.UserData.Add("AllowLateBound", false);
			// Option Explicit On (controls whether variable declarations are required)
			codeComplieUnit.UserData.Add("RequireVariableDeclaration", true);
			CodeNamespace codeNamespace = new CodeNamespace(EntityNamespace);
			//codeNamespace.Comments.Add(new CodeCommentStatement("<summary>", true));
			//codeNamespace.Comments.Add(new CodeCommentStatement(string.Format("{0}", entityNs), true));
			//codeNamespace.Comments.Add(new CodeCommentStatement("</summary>", true));
			codeComplieUnit.Namespaces.Add(codeNamespace);
			foreach (string ns in _ImportNamespaces)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(ns));
			}
			if (!_ImportNamespaces.Contains("Basic.EntityLayer"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.EntityLayer"));
			if (!_ImportNamespaces.Contains("Basic.Enums"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Enums"));
			if (!_ImportNamespaces.Contains("Basic.Interfaces"))
				codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Interfaces"));
			foreach (DataEntityElement entity in _DataEntityElements)
			{
				if (entity.GeneratorMode == GenerateModeEnum.DataEntity)
					entity.WriteEntityCode(codeNamespace);
				else if (entity.GeneratorMode == GenerateModeEnum.DataTable)
					entity.WriteTableCode(codeNamespace);
				else
				{
					entity.WriteEntityCode(codeNamespace);
					entity.WriteTableCode(codeNamespace);
				}
				if (entity.Condition.Arguments.Count > 0)
					entity.Condition.WriteEntityCode(codeNamespace);
			}
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
			writer.WriteStartElement(XmlElementPrefix, XmlElementName, XmlDataNamespace);
			_TableInfo.GenerateConfiguration(writer, connectionType);
			writer.WriteStartElement(XmlElementPrefix, DataCommandCollection.XmlElementName, XmlDataNamespace);
			foreach (DataEntityElement dataCommand in _DataEntityElements)
			{
				dataCommand.GenerateConfiguration(writer, connectionType);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
}
