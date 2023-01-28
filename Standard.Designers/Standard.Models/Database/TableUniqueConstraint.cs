using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Designer;
using Basic.Collections;
using Basic.Configuration;
using System.Xml;
using Basic.Enums;

namespace Basic.Database
{
    /// <summary>
    /// 唯一性约束
    /// </summary>
    public class UniqueConstraint : AbstractCustomTypeDescriptor
    {
        internal const string XmlElementName = "UniqueConstraint";
        internal const string PrimaryKeyElementName = "PrimaryKeyConstraint";
        internal const string NameAttribute = "Name";
        internal const string ColumnsAttribute = "Columns";
        private readonly bool _PrimaryKeyConstraint = false;
        private readonly DesignTableInfo tableInfo;
        private readonly DesignColumnCollection tableColumns;
        private readonly DesignColumnCollection uniqueColumns;
        /// <summary>
        /// 使用带参构造函数，初始化UniqueConstraintInfo类实例
        /// </summary>
        /// 《票》
        /// <param name="persistent"></param>
        /// <param name="uniqueName">Unique 约束名称</param>
        /// <param name="pkConstraint">是否主键约束</param>
        /// <param name="columns"></param>
        public UniqueConstraint(DesignTableInfo persistent, string uniqueName, DesignColumnInfo[] columns, bool pkConstraint)
            : base(persistent)
        {
            _PrimaryKeyConstraint = pkConstraint;
            tableInfo = persistent;
            Name = uniqueName;
            uniqueColumns = new DesignColumnCollection(persistent);
            foreach (DesignColumnInfo column in columns)
                uniqueColumns.Add(column);
        }

        /// <summary>
        /// 使用带参构造函数，初始化UniqueConstraintInfo类实例
        /// </summary>
        /// <param name="pkConstraint">是否主键约束</param>
        /// <param name="columnCollection">Unique 约束列</param>
        /// <param name="persistent"></param>
        public UniqueConstraint(DesignTableInfo persistent, DesignColumnCollection columnCollection, bool pkConstraint)
            : base(persistent)
        {
            _PrimaryKeyConstraint = pkConstraint;
            tableInfo = persistent;
            tableColumns = columnCollection;
            uniqueColumns = new DesignColumnCollection(persistent);
        }

        private string _Name = string.Empty;
        /// <summary>
        /// 获取或设置数据库表中的约束的名称。
        /// </summary>
        [Basic.Designer.PackageDescription("PersistentDescription_ColumnName")]
        [Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
        [System.ComponentModel.DefaultValue("")]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// 数据库表主键信息
        /// </summary>
        public DesignColumnCollection Columns { get { return uniqueColumns; } }

        #region 接口 IXmlSerializable 默认实现
        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override bool ReadAttribute(string name, string value)
        {
            if (name == NameAttribute) { _Name = value; return true; }
            else if (name == ColumnsAttribute)
            {
                if (value != null && value.Contains(','))
                {
                    string[] cnArray = value.Split(',');
                    for (int index = 0; index < cnArray.Length; index++)
                    {
                        string columnName = cnArray[index];
                        if (tableColumns.ContainsKey(columnName))
                            uniqueColumns.Add(tableColumns[columnName]);
                    }
                }
                else if (value != null && tableColumns.ContainsKey(value))
                {
                    uniqueColumns.Add(tableColumns[value]);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>如果当前节点已经读到结尾则返回 true，否则返回 false。</returns>
        protected internal override bool ReadContent(System.Xml.XmlReader reader)
        {
            if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == XmlElementName)
            {
                return true;
            }
            else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == PrimaryKeyElementName)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString(NameAttribute, _Name);
            if (uniqueColumns != null && uniqueColumns.Count > 0)
            {
                List<string> columnNameList = new List<string>(uniqueColumns.Count + 1);
                foreach (DesignColumnInfo column in uniqueColumns)
                {
                    columnNameList.Add(column.Name);
                }
                writer.WriteAttributeString(ColumnsAttribute, string.Join<string>(",", columnNameList));
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteContent(System.Xml.XmlWriter writer) { }

        /// <summary>
        /// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
        {
        }

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public override string GetClassName() { return XmlElementName; }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public override string GetComponentName() { return Name; }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal override string ElementName { get { return _PrimaryKeyConstraint ? PrimaryKeyElementName : XmlElementName; } }

        /// <summary>
        /// 获取当前节点元素命名空间
        /// </summary>
        protected internal override string ElementNamespace { get { return null; } }

        #endregion
    }
}
