using Basic.Configuration;
using Basic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Designer
{
    /// <summary>
    /// 表示Visual Studio项目信息(2010/2012)
    /// </summary>
    public sealed class ProjectInfo : AbstractCustomTypeDescriptor, IEquatable<Guid>
    {
        private readonly PersistentConfiguration persistentConfiguration;
        private Guid _ProjectGuid = Guid.Empty;
        private string _ProjectName = null;
        private string _UniqueName = null;
        /// <summary>
        /// Visual Studio项目项目唯一名称
        /// </summary>
        public string UniqueName { get { return _UniqueName; } set { _UniqueName = value; RaisePropertyChanged("UniqueName"); } }

        /// <summary>
        /// Visual Studio项目Guid
        /// </summary>
        public Guid ProjectGuid { get { return _ProjectGuid; } set { _ProjectGuid = value; RaisePropertyChanged("ProjectGuid"); } }

        /// <summary>
        /// Visual Studio项目显示名称
        /// </summary>
        public string ProjectName { get { return _ProjectName; } set { _ProjectName = value; RaisePropertyChanged("ProjectName"); } }

        /// <summary>
        /// 采用项目信息初始化 ProjectInfo 类实例。
        /// </summary>
        /// <param name="persistent">数据持久类文件</param>
        public ProjectInfo(PersistentConfiguration persistent) : this(persistent, Guid.Empty, null, null) { }

        /// <summary>
        /// 采用项目信息初始化 ProjectInfo 类实例。
        /// </summary>
        /// <param name="persistent">数据持久类文件</param>
        /// <param name="guid"></param>
        /// <param name="name"></param>
        /// <param name="uniqueName"></param>
        public ProjectInfo(PersistentConfiguration persistent, Guid guid, string name, string uniqueName)
            : base(persistent)
        {
            persistentConfiguration = persistent;
            _ProjectGuid = guid;
            _ProjectName = name;
            _UniqueName = uniqueName;
        }

        /// <summary>
        /// 判断当前项目信息是否为空。
        /// </summary>
        public bool IsEmpty { get { return _ProjectGuid == Guid.Empty; } }

        /// <summary>
        /// 判断当前项目信息是否为空。
        /// </summary>
        public bool NotEmpty { get { return _ProjectGuid != Guid.Empty; } }

        /// <summary>
        /// 指示当前对象是否等于同一类型的另一个对象。
        /// </summary>
        /// <param name="other">与此对象进行比较的对象。</param>
        /// <returns>如果当前对象等于 other 参数，则为 true；否则为 false。</returns>
        public bool Equals(Guid other) { return _ProjectGuid == other; }
        /// <summary>
        /// 表示 ProjectInfo 的字符串表示形式。
        /// </summary>
        /// <returns>返回 ProjectInfo 的字符串表示形式</returns>
        public override string ToString() { return _ProjectName; }
        internal const string XmlElementName = "EntityProject";
        internal const string NameAttribute = "Name";
        internal const string UniqueAttribute = "Unique";
        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public override string GetComponentName() { return XmlElementName; }

        /// <summary>
        /// 获取当前节点元素命名空间
        /// </summary>
        protected internal override string ElementNamespace { get { return persistentConfiguration.ElementNamespace; } }

        /// <summary>
        /// 获取当前节点元素前缀
        /// </summary>
        protected internal override string ElementPrefix { get { return persistentConfiguration.ElementPrefix; } }

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public override string GetClassName() { return typeof(ProjectInfo).Name; }

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
            if (name == NameAttribute) { _ProjectName = value; return true; }
            else if (name == UniqueAttribute) { _UniqueName = value; return true; }
            return false;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        protected internal override bool ReadContent(System.Xml.XmlReader reader)
        {
            _ProjectGuid = new Guid(reader.ReadString());
            return true;
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
        {
            if (!IsEmpty)
            {
                writer.WriteAttributeString(NameAttribute, _ProjectName);
                writer.WriteAttributeString(UniqueAttribute, _UniqueName);
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal override void WriteContent(System.Xml.XmlWriter writer)
        {
            writer.WriteValue(_ProjectGuid.ToString());
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!IsEmpty)
            {
                writer.WriteStartElement(PersistentConfiguration.XmlElementPrefix, ElementName, PersistentConfiguration.XmlConfigNamespace);
                WriteAttribute(writer);
                WriteContent(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal override void GenerateConfiguration(System.Xml.XmlWriter writer, ConnectionTypeEnum connectionType) { }
    }

    /// <summary>
    /// VS2010项目信息
    /// </summary>
    public class ProjectInfo1 : IEquatable<Guid>
    {
        private readonly Guid _ProjectGuid;
        private readonly EnvDTE.Project _Project;
        public ProjectInfo1(Guid projectGuid, EnvDTE.Project project)
        {
            _ProjectGuid = projectGuid; _Project = project;
        }

        public Guid ProjectGuid { get { return _ProjectGuid; } }

        public EnvDTE.Project Project { get { return _Project; } }

        public string Name { get { return _Project.Name; } }

        public string UniqueName { get { return _Project.UniqueName; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Guid other)
        {
            return ProjectGuid == other;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_Project != null) { return _Project.Name; }
            return string.Empty;
        }
    }
}
