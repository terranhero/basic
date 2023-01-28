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
using Basic.Configuration;
using Basic.EntityLayer;

namespace Basic.Converters
{
    /// <summary>
    /// 表示抽象配置命令
    /// </summary>
    internal sealed class ConverterCheckCommand : AbstractConverterCommand, IXmlSerializable
    {
        private readonly ConverterStaticCommand _StaticCommand;
        /// <summary>
        /// 初始化 ConverterCheckCommand 类实例。
        /// </summary>
        /// <param name="staticCommand">拥有此检测命令的静态命令结构</param>
        internal ConverterCheckCommand(ConverterStaticCommand staticCommand)
            : base(staticCommand) { _StaticCommand = staticCommand; Name = Basic.EntityLayer.GuidConverter.GuidString; }

        /// <summary>
        /// 当前命令是否需要写入Name属性。
        /// </summary>
        protected override bool WriteName { get { return false; } }

        private string _ErrorCode = null;
        /// <summary>
        /// 获取或设置检查命令执行失败时的错误编码
        /// </summary>
        /// <value>需要返回的错误编码，默认值为null</value>
        public string ErrorCode { get { return _ErrorCode; } set { _ErrorCode = value; } }

        private string _Parameter = null;
        /// <summary>
        /// 获取或设置当前检测命令如果测试失败，则需要将失败信息对应为实体类中哪个属性
        /// </summary>
        public string Parameter { get { return _Parameter; } set { _Parameter = value; } }

        private string _SourceColumn = null;
        /// <summary>
        /// 获取或设置当前检测命令如果测试失败，则需要将失败信息对应为实体类中哪个属性
        /// </summary>
        public string SourceColumn { get { return _SourceColumn; } set { _SourceColumn = value; } }

        private bool _CheckExist = true;
        /// <summary>
        /// 获取或设置要对当前检测命令是检测数据存在还是检测数据不存在。
        /// </summary>
        public bool CheckExist { get { return _CheckExist; } set { _CheckExist = value; } }

        private string _CommandText = null;
        /// <summary>
        /// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
        /// </summary>
        /// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
        public string CommandText { get { return _CommandText; } set { _CommandText = value; } }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal override string ElementName { get { return CheckedCommandElement.XmlElementName; } }

        /// <summary>
        /// 
        /// </summary>
        internal void ResetParameters()
        {
            if (!string.IsNullOrEmpty(_Parameter))
            {
                string[] strArray = _Parameter.Split(',');
                //if (_Parameter.IndexOf(',') >= 0)
                //    strArray = _Parameter.Split(',');
                foreach (string parameterName in strArray)
                {
                    if (_StaticCommand.Parameters.ContainsKey(parameterName))
                    {
                        Parameters.Add(_StaticCommand.Parameters[parameterName]);
                    }
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
            if (name == CheckedCommandElement.ParameterAttribute) { _Parameter = value; return true; }
            else if (name == CheckedCommandElement.CheckExistAttribute) { _CheckExist = Convert.ToBoolean(value); return true; }
            else if (name == CheckedCommandElement.PropertyNameAttribute) { _SourceColumn = value; return true; }
            else if (name == CheckedCommandElement.ErrorCodeAttribute) { _ErrorCode = value; return true; }
            else if (name == "ReturnValue") { _ErrorCode = value; return true; }//兼容 3.0 版本。
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
                writer.WriteAttributeString(CheckedCommandElement.CheckExistAttribute, Convert.ToString(_CheckExist));
            if (!string.IsNullOrWhiteSpace(_SourceColumn))
                writer.WriteAttributeString(CheckedCommandElement.PropertyNameAttribute, _SourceColumn);
            writer.WriteAttributeString(CheckedCommandElement.ErrorCodeAttribute, _ErrorCode);
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
            else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CheckedCommandElement.CommandTextElement)//兼容5.0新版结构信息
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
            writer.WriteElementString(CheckedCommandElement.CommandTextElement, CommandText);
            base.WriteContent(writer);
        }
    }
}
