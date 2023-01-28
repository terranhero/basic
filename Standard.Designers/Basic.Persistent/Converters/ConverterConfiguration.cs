using Basic.DataAccess;
using Basic.Collections;
using Basic.Configuration;
using Basic.Database;
using Basic.Designer;
using Basic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Basic.Converters
{
    internal class ConverterConfiguration : AbstractCustomTypeDescriptor
    {
        private readonly ConverterDataCommandCollection dataCommands;

		/// <summary>
		/// 数据库表名称配置节名称
		/// </summary>
		internal protected const string EntityConfig = "EntityConfig";
        private readonly DesignTableInfo tableInfo;

        /// <summary>
        /// 初始化 ConverterConfiguration 类实例
        /// </summary>
        internal ConverterConfiguration(string fileName)
        {
            _Version = new System.Version(4, 0, 0, 0);
            tableInfo = new DesignTableInfo(this, PersistentConfiguration.XmlElementPrefix, PersistentConfiguration.XmlDataNamespace);
            dataCommands = new ConverterDataCommandCollection(this, PersistentConfiguration.XmlElementPrefix, PersistentConfiguration.XmlDataNamespace);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            using (XmlReader reader = XmlReader.Create(fileName, settings))
            {
                ReadXml(reader);
            }
        }

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public override string GetClassName() { return GetType().Name; }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public override string GetComponentName() { return GetType().Name; }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal override string ElementName { get { return PersistentConfiguration.XmlElementName; } }

        /// <summary>
        /// 获取当前节点元素命名空间
        /// </summary>
        protected internal override string ElementNamespace { get { return PersistentConfiguration.XmlDataNamespace; } }

        /// <summary>
        /// 获取当前节点元素前缀
        /// </summary>
        protected internal override string ElementPrefix { get { return PersistentConfiguration.XmlElementPrefix; } }

        private Version _Version;
        /// <summary>
        /// 当前配置文件关联的数据库表名称
        /// </summary>
        public Version Version { get { return _Version; } set { _Version = value; } }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override bool ReadAttribute(string name, string value)
        {
            if (name == PersistentConfiguration.VersionAttribute) { _Version = Version.Parse(value); return true; }
            return false;
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString(PersistentConfiguration.VersionAttribute, _Version.ToString());
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        protected internal override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.LocalName == EntityConfig && reader.Prefix == "eadl" && reader.NodeType == XmlNodeType.Element)
            {
                base.ReadXml(reader); return;
            }
            else if (reader.LocalName == EntityConfig && reader.NodeType == XmlNodeType.Element)
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
                    else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == PersistentConfiguration.TableNameElement)
                    {
                        tableInfo.TableName = reader.ReadString(); continue;
                    }
                    else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == PersistentConfiguration.ViewNameElement)
                    {
                        tableInfo.ViewName = reader.ReadString(); continue;
                    }
                    #region 3.0版本数据持久类, 当前使用项目EssexCu
                    else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataCommand.CommandConfig)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 2)
                            {
                                ConverterStaticCommand staticCommand = new ConverterStaticCommand(this);
                                if (reader.LocalName == AbstractDbAccess.SelectAllConfig)
                                    staticCommand.Name = AbstractDbAccess.SearchTableConfig;
                                else
                                    staticCommand.Name = reader.LocalName;
                                staticCommand.ReadXml(reader.ReadSubtree());
                                dataCommands.Add(staticCommand);
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == 1 && reader.LocalName == DataCommand.CommandConfig)
                                break;
                        }
                    }
                    #endregion

                    #region 3.5版本数据持久类, 当前使用项目EssexCost
                    else if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 && reader.LocalName == DataCommand.StaticCommandConfig)
                    {
                        XmlReader reader1 = reader.ReadSubtree();
                        while (reader1.Read())
                        {
                            if (reader1.NodeType == XmlNodeType.Whitespace) { continue; }
                            else if (reader1.NodeType == XmlNodeType.Element && reader1.Depth == 1)
                            {
                                ConverterStaticCommand staticCommand = new ConverterStaticCommand(this);
                                if (reader.LocalName == AbstractDbAccess.SelectAllConfig)
                                    staticCommand.Name = AbstractDbAccess.SearchTableConfig;
                                else
                                    staticCommand.Name = reader.LocalName;
                                staticCommand.ReadXml(reader1.ReadSubtree());
                                dataCommands.Add(staticCommand);
                            }
                            else if (reader1.NodeType == XmlNodeType.EndElement && reader1.Depth == 0 && reader1.LocalName == DataCommand.StaticCommandConfig)
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 && reader.LocalName == DataCommand.DynamicCommandConfig)
                    {
                        XmlReader reader2 = reader.ReadSubtree();
                        while (reader2.Read())
                        {
                            if (reader2.NodeType == XmlNodeType.Whitespace) { continue; }
                            else if (reader2.NodeType == XmlNodeType.Element && reader2.Depth == 1)
                            {
                                ConverterDynamicCommand dynamicCommand = new ConverterDynamicCommand(this);
                                if (reader2.LocalName == AbstractDbAccess.SelectAllConfig)
                                    dynamicCommand.Name = AbstractDbAccess.SearchTableConfig;
                                else
                                    dynamicCommand.Name = reader2.LocalName;
                                dynamicCommand.ReadXml(reader2.ReadSubtree());
                                dataCommands.Add(dynamicCommand);
                            }
                            else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Depth == 1 && reader2.LocalName == DataCommand.DynamicCommandConfig)
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == EntityConfig)
                    {
                        if (dataCommands.ContainsKey(AbstractDbAccess.NewKeyConfig))
                        {
                            ConverterStaticCommand newCommand = dataCommands[AbstractDbAccess.NewKeyConfig] as ConverterStaticCommand;
                            if (dataCommands.ContainsKey(AbstractDbAccess.CreateConfig) && !string.IsNullOrWhiteSpace(newCommand.CommandText))
                            {
                                ConverterStaticCommand staticCommand = dataCommands[AbstractDbAccess.CreateConfig] as ConverterStaticCommand;
                                staticCommand.NewCommands.Add(newCommand.CreateNewCommand(staticCommand));
                                dataCommands.Remove(newCommand);
                            }
                        }
                        return;
                    }
                    #endregion
                }
            }
            base.ReadXml(reader);
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
        protected internal override bool ReadContent(System.Xml.XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == PersistentConfiguration.TableNameElement)
            {
                tableInfo.TableName = reader.ReadString(); return false;
            }
            else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == PersistentConfiguration.ViewNameElement)
            {
                tableInfo.ViewName = reader.ReadString(); return false;
            }
            else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DesignTableInfo.XmlElementName)
            {
                tableInfo.ReadXml(reader.ReadSubtree()); return false;
            }
            else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == dataCommands.ElementName)
            {
                dataCommands.ReadXml(reader.ReadSubtree()); return false;
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == PersistentConfiguration.XmlElementName)
            {
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
            tableInfo.GenerateConfiguration(writer, ConnectionTypeEnum.Default);
            dataCommands.WriteXml(writer);
        }

    }
}
