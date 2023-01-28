using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Basic.Enums;
using SCC = System.Collections.Concurrent;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示数据表配置文件信息包含数据表字段和当前表所需执行的命令
	/// </summary>
	public sealed class TableConfiguration : SCC.ConcurrentDictionary<string, DataCommand>, IXmlSerializable
	{
		#region 实体定义字段
		private readonly ColumnInfoCollection _Columns;
		internal const string XmlNamespace = "http://dev.goldsoft.com/2013/BasicDataPersistentSchema-5.0.xsd";
		internal const string XmlPrefix = "dpdl";
		internal const string XmlElementName = "TableInfo";
		internal const string TableNameAttribute = "TableName";
		internal const string ViewNameAttribute = "ViewName";
		internal const string DescriptionAttribute = "Description";

		/// <summary>
		/// 结构配置节名称
		/// </summary>
		private const string DataCommandsElement = "DataCommands";

		/// <summary>
		/// 静态命令结构配置节名称
		/// </summary>
		private const string StaticCommandElement = "StaticCommand";

		/// <summary>
		/// 动态命令结构配置节名称
		/// </summary>
		private const string DynamicCommandElement = "DynamicCommand";

		#endregion

		/// <summary>
		/// 表示数据库工厂类
		/// </summary>
		private readonly ConnectionFactory _ConnectionFactory;
		/// <summary>
		/// 初始化 TableInfo 类实例。
		/// </summary>
		/// <param name="factory"></param>
		internal TableConfiguration(ConnectionFactory factory)
		{
			_Columns = new ColumnInfoCollection(this);
			_ConnectionFactory = factory;
		}

		private string _TableName = null;
		/// <summary>
		/// 获取或设置数据库表名称。
		/// </summary>
		/// <value>数据库表名称。</value>
		public string TableName
		{
			get { return _TableName; }
		}

		private string _ViewName = null;
		/// <summary>
		/// 获取或设置数据库视图名称。
		/// </summary>
		/// <value>数据库视图名称。</value>
		public string ViewName
		{
			get { return _ViewName; }
		}

		private string _Description = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		public string Description
		{
			get { return _Description; }
		}

		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取的值的键。</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。</param>
		/// <returns>如果 Basic.DataCache.SqlStructTableCache 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public void SetValue(string key, DataCommand value)
		{
			base[key] = value;
		}

		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。</param>
		/// <returns>如果 Basic.DataCache.SqlStructTableCache 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public void SetValue(DataCommand value)
		{
			base[value.CommandName] = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public ColumnInfoCollection Columns { get { return _Columns; } }

		#region 接口 IXmlSerializable 默认实现
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
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == XmlElementName)
				{
					if (reader.HasAttributes)
					{
						for (int index = 0; index < reader.AttributeCount; index++)
						{
							reader.MoveToAttribute(index);
							if (reader.LocalName == TableNameAttribute) { _TableName = reader.GetAttribute(index); }
							else if (reader.LocalName == ViewNameAttribute) { _ViewName = reader.GetAttribute(index); }
							else if (reader.LocalName == DescriptionAttribute) { _Description = reader.GetAttribute(index); }
						}
					}
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ColumnInfo.XmlElementName)
				{
					ColumnInfo column = _Columns.CreateColumn();
					column.ReadXml(reader.ReadSubtree());
					_Columns.Add(column);
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataCommandsElement)
				{
					while (reader.Read())   //版本5.0使用
					{
						if (reader.NodeType == XmlNodeType.Element && reader.LocalName == StaticCommandElement)
						{
							StaticCommand dataCommand = _ConnectionFactory.CreateStaticCommand();
							dataCommand.ReadXml(reader.ReadSubtree());
							if (dataCommand.ConfigurationType == ConfigurationTypeEnum.AddNew)
								dataCommand.CommandName = AbstractDbAccess.CreateConfig;
							else if (dataCommand.ConfigurationType == ConfigurationTypeEnum.Modify)
								dataCommand.CommandName = AbstractDbAccess.ModifyConfig;
							else if (dataCommand.ConfigurationType == ConfigurationTypeEnum.Remove)
								dataCommand.CommandName = AbstractDbAccess.RemoveConfig;
							else if (dataCommand.ConfigurationType == ConfigurationTypeEnum.SearchTable)
								dataCommand.CommandName = AbstractDbAccess.SearchTableConfig;
							else if (dataCommand.ConfigurationType == ConfigurationTypeEnum.SelectByKey)
								dataCommand.CommandName = AbstractDbAccess.SelectByKeyConfig;
							this.SetValue(dataCommand.CommandName, dataCommand);
						}
						else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DynamicCommandElement)
						{
							DynamicCommand dataCommand = _ConnectionFactory.CreateDynamicCommand();
							dataCommand.ReadXml(reader.ReadSubtree());
							if (dataCommand.ConfigurationType == ConfigurationTypeEnum.SearchTable)
								dataCommand.CommandName = AbstractDbAccess.SearchTableConfig;
							else if (dataCommand.ConfigurationType == ConfigurationTypeEnum.SelectByKey)
								dataCommand.CommandName = AbstractDbAccess.SelectByKeyConfig;
							this.SetValue(dataCommand.CommandName, dataCommand);
						}
						else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == DataCommandsElement)
							break;
					}
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 && reader.LocalName == DataCommand.StaticCommandConfig)
				{
					while (reader.Read())
					{
						if (reader.NodeType == XmlNodeType.Element && reader.Depth == 2)
						{
							StaticCommand staticCommand = _ConnectionFactory.CreateStaticCommand();
							if (reader.LocalName == AbstractDbAccess.SelectAllConfig)
								staticCommand.CommandName = AbstractDbAccess.SearchTableConfig;
							else
								staticCommand.CommandName = reader.LocalName;
							staticCommand.ReadXml(reader.ReadSubtree());
							this.SetValue(staticCommand);
						}
						else if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == 1 && reader.LocalName == DataCommand.StaticCommandConfig)
							break;
					}
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 && reader.LocalName == DataCommand.DynamicCommandConfig)
				{
					while (reader.Read())
					{
						if (reader.NodeType == XmlNodeType.Element && reader.Depth == 2)
						{
							DynamicCommand dynamicCommand = _ConnectionFactory.CreateDynamicCommand();
							if (reader.LocalName == AbstractDbAccess.SelectAllConfig)
								dynamicCommand.CommandName = AbstractDbAccess.SearchTableConfig;
							else
								dynamicCommand.CommandName = reader.LocalName;
							dynamicCommand.ReadXml(reader.ReadSubtree());
							this.SetValue(dynamicCommand);
						}
						else if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == 1 && reader.LocalName == DataCommand.DynamicCommandConfig)
							break;
					}
				}
				#region 关闭支持
				//else if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 && reader.LocalName == DataCommand.CommandConfig)
				//{	
				//	while (reader.Read())//版本4.0使用,目前埃塞克斯铜软件使用测配置信息
				//	{
				//		if (reader.NodeType == XmlNodeType.Element && reader.Depth == 2)
				//		{
				//			StaticCommand staticCommand = CreateDbStaticCommand();
				//			staticCommand.ReadXml(reader.ReadSubtree());
				//			this.AddCommand(configFileName, staticCommand);
				//		}
				//		else if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == 1 && reader.LocalName == DataCommand.CommandConfig)
				//			break;
				//	}
				//}
				#endregion
			}
		}

		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
		/// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { }
		#endregion

	}
}
