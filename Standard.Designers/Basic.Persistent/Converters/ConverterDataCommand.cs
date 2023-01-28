using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Xml.Serialization;
using EnvDTE;

using Basic.DataEntities;
using Basic.Designer;
using Basic.Enums;
using Basic.Interfaces;
using System.Linq.Expressions;
using Basic.EntityLayer;
using Basic.DataAccess;
using Basic.Configuration;

namespace Basic.Converters
{
	/// <summary>
	/// 表示抽象配置命令
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	internal abstract class ConverterDataCommand : AbstractConverterCommand, IXmlSerializable
	{
		private const string CShartFileExtension = "cs";
		private const string VisualVasicFileExtension = "vb";
		protected internal readonly ConverterConfiguration entityElement;
		/// <summary>
		/// 初始化 ConverterDataCommand 类实例
		/// </summary>
		protected ConverterDataCommand(ConverterConfiguration converter)
			: base(converter)
		{
			entityElement = converter;
		}

		private ConfigurationTypeEnum _Kind = ConfigurationTypeEnum.Other;
		/// <summary>
		/// 获取或设置数据命令类型。
		/// </summary>
		public ConfigurationTypeEnum Kind { get { return _Kind; } set { _Kind = value; } }

		private string _Comment = string.Empty;
		/// <summary>
		/// 获取或设置命令描述
		/// </summary>
		public string Comment { get { return _Comment; } set { _Comment = value; } }

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == DataCommandElement.KindAttribute) { return Enum.TryParse<ConfigurationTypeEnum>(value, true, out _Kind); }
			else if (name == DataCommandElement.CommentAttribute) { Comment = value; return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		internal protected override bool ReadContent(System.Xml.XmlReader reader)
		{
			return base.ReadContent(reader);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			if (_Kind != ConfigurationTypeEnum.Other)
				writer.WriteAttributeString(DataCommandElement.KindAttribute, _Kind.ToString());
			if (!string.IsNullOrWhiteSpace(_Comment))
				writer.WriteAttributeString(DataCommandElement.CommentAttribute, _Comment);

		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			base.WriteContent(writer);
		}
	}
}
