﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Basic.Configuration;
using Basic.Designer;
using Basic.Collections;

namespace Basic.Converters
{
    /// <summary>
    /// 表示新值命令集合
    /// </summary>
    internal sealed class ConverterNewCommandCollection : Basic.Collections.AbstractCollection<ConverterNewCommand>
    {
        private readonly ConverterStaticCommand _StaticCommand;
        /// <summary>
        /// 数据库表中所有列配置节名称
        /// </summary>
        public const string XmlElementName = "NewValues";
        /// <summary>
        /// 初始化 ConverterNewCommandCollection 类的新实例。
        /// </summary>
        internal ConverterNewCommandCollection(ConverterStaticCommand element) : base(element) { _StaticCommand = element; }

        /// <summary>
        /// 获取集合的键属性
        /// </summary>
        /// <param name="item">需要获取键的集合子元素</param>
        /// <returns>返回元素的键</returns>
        protected internal override string GetKey(ConverterNewCommand item) { return item.Name; }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal override string ElementName
        {
            get { return NewCommandCollection.XmlElementName; }
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
        protected internal override bool ReadChildContent(System.Xml.XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Whitespace) { continue; }
                else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == NewCommandElement.XmlElementName)
                {
                    ConverterNewCommand element = new ConverterNewCommand(_StaticCommand);
                    element.ReadXml(reader.ReadSubtree());
                    this.Add(element);
                }
                else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == ElementName)
                    break;
            }
            return false;
        }
    }
}
