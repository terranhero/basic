using System;
using System.CodeDom;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;
using Basic.Designer;
using Basic.EntityLayer;
using System.Xml;
using Basic.Enums;
using Basic.Database;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示实体定义模型中属性定义信息,实体属性对应数据库字段信息
	/// </summary>
	partial class AbstractPropertyElement
	{
		internal const string ColumnElement = "Column";
		internal const string PrimaryKeyAttribute = "PKey";
		internal const string DefaultValueAttribute = "Default";
		internal const string SourceAttribute = "Source";
		internal const string ProfixAttribute = "Profix";
		internal const string DbTypeAttribute = "DbType";
		internal const string SizeAttribute = "Size";
		internal const string PrecisionAttribute = "Precision";
		internal const string ScaleAttribute = "Scale";
		internal const string ComputedAttribute = "Computed";

		private string _Column;
		/// <summary>
		/// 获取或设置数据库表中的列的名称
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_ColumnName")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue("")]
		public string Column
		{
			get { return _Column; }
			set
			{
				if (_Column != value)
				{
					if (_Source == _Column) { Source = value; }
					_Column = value;
					RaisePropertyChanged("Column");
				}
			}
		}

		private string _Source;
		/// <summary>
		/// 获取或设置数据库表中的列的原名称。
		/// </summary>
		[Basic.Designer.PersistentDescription("PropertyDescription_SourceColumn")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue("")]
		public string Source
		{
			get { return _Source; }
			set { if (_Source != value) { _Source = value; RaisePropertyChanged("Source"); } }
		}

		private string _Profix;
		/// <summary>
		/// 字段前缀，主要用于查询
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_ColumnProfix")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue("")]
		public string Profix
		{
			get { return _Profix; }
			set { if (_Profix != value) { _Profix = value; RaisePropertyChanged("Profix"); } }
		}

		private bool _Computed = false;
		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnComputed")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(false), System.ComponentModel.Browsable(false)]
		public bool Computed
		{
			get { return _Computed; }
			set { if (_Computed != value) { _Computed = value; RaisePropertyChanged("Computed"); } }
		}

		/// <summary>
		/// 此列是否已经被选择为条件列。
		/// </summary>
		[System.ComponentModel.DefaultValue(false), System.ComponentModel.Browsable(false)]
		public bool IsWhere { get; set; }

		private DbTypeEnum _DbType = DbTypeEnum.NVarChar;
		/// <summary>
		/// 属性类型名称
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_ColumnDbType")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(typeof(DbTypeEnum), "NVarChar")]
		public DbTypeEnum DbType
		{
			get { return _DbType; }
			set
			{
				if (_DbType != value)
				{
					_DbType = value;
					RaisePropertyChanged("DbType");
					Type = DbBuilderHelper.DbTypeToNetType(_DbType);
				}
			}
		}

		private int _Size = 0;
		/// <summary>
		/// 获取或设置列中数据的最大大小（以字节为单位）。
		/// </summary>
		/// <value>列中数据的最大大小（以字节为单位）。默认值是从参数值推导出的。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_ColumnSize")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(0)]
		public int Size
		{
			get { return _Size; }
			set { if (_Size != value) { _Size = value; RaisePropertyChanged("Size"); } }
		}

		private byte _Precision = 0;
		/// <summary>
		/// 获取或设置用来表示 Value 属性的最大位数。 
		/// </summary>
		/// <value>用于表示 Value 属性的最大位数。 默认值为 0。这指示数据提供程序设置 Value 的精度。 </value>
		[Basic.Designer.PersistentDescription("PersistentDescription_ColumnPrecision")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(typeof(byte), "0")]
		public byte Precision
		{
			get { return _Precision; }
			set { if (_Precision != value) { _Precision = value; RaisePropertyChanged("Precision"); } }
		}

		private byte _Scale = 0;
		/// <summary>
		/// 获取或设置 Value 解析为的小数位数。
		/// </summary>
		/// <value>要将 Value 解析为的小数位数。默认值为 0。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_ColumnScale")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(typeof(byte), "0")]
		public byte Scale
		{
			get { return _Scale; }
			set { if (_Scale != value) { _Scale = value; RaisePropertyChanged("Scale"); } }
		}

		private string _DefaultValue = null;
		/// <summary>
		/// 属性的默认值
		/// </summary>
		[Basic.Designer.PersistentDescription("PropertyDescription_DefaultValue")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.Browsable(false)]
		public string DefaultValue
		{
			get { return _DefaultValue; }
			set
			{
				if (_DefaultValue != value)
				{
					_DefaultValue = value;
					RaisePropertyChanged("DefaultValue");
				}
			}
		}

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		private bool ReadColumn(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			if (reader.HasAttributes)
			{
				for (int index = 0; index < reader.AttributeCount; index++)
				{
					reader.MoveToAttribute(index);
					ReadColumnAttribute(reader.LocalName, reader.GetAttribute(index));
				}
			}
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		private bool ReadColumnAttribute(string name, string value)
		{
			if (name == NameAttribute) { _Column = value; return true; }
			else if (name == SourceAttribute) { _Source = value; return true; }
			else if (name == ProfixAttribute) { _Profix = value; return true; }
			else if (name == DbTypeAttribute) { return Enum.TryParse<DbTypeEnum>(value, out _DbType); }
			else if (name == SizeAttribute) { _Size = Convert.ToInt32(value); return true; }
			else if (name == PrecisionAttribute) { _Precision = Convert.ToByte(value); return true; }
			else if (name == ScaleAttribute) { _Scale = Convert.ToByte(value); return true; }
			else if (name == DefaultValueAttribute) { _DefaultValue = value; return true; }
			else if (name == ComputedAttribute) { _Computed = Convert.ToBoolean(value); return true; }
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		private void WriteColumn(System.Xml.XmlWriter writer)
		{
			if (!string.IsNullOrWhiteSpace(_Column))
			{
				writer.WriteStartElement(ColumnElement);
				if (!string.IsNullOrWhiteSpace(_Profix))
					writer.WriteAttributeString(ProfixAttribute, _Profix);
				writer.WriteAttributeString(NameAttribute, _Column);
				writer.WriteAttributeString(SourceAttribute, _Source);
				writer.WriteAttributeString(DbTypeAttribute, _DbType.ToString());
				if (_Size > 0)
					writer.WriteAttributeString(SizeAttribute, Convert.ToString(_Size));
				if (_Precision > 0)
					writer.WriteAttributeString(PrecisionAttribute, Convert.ToString(_Precision));
				if (_Scale > 0)
					writer.WriteAttributeString(ScaleAttribute, Convert.ToString(_Scale));
				if (!string.IsNullOrWhiteSpace(_DefaultValue))
					writer.WriteAttributeString(DefaultValueAttribute, _DefaultValue);
				if (_Computed)
					writer.WriteAttributeString(ComputedAttribute, "true");
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
		}
		#endregion

		private void WriteColumnCode(CodeMemberProperty property)
		{
			if (!string.IsNullOrWhiteSpace(_Column))
			{
				CodeTypeReference columnTypeReference = new CodeTypeReference(typeof(ColumnMappingAttribute),
					 CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration columnAttribute = new CodeAttributeDeclaration(columnTypeReference);
				columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Profix)));
				columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Column)));
				if (_Source != _Column && !string.IsNullOrWhiteSpace(_Source))
					columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Source)));
				CodeFieldReferenceExpression fieldTypeExpress = new CodeFieldReferenceExpression(
					 new CodeTypeReferenceExpression(typeof(DbTypeEnum).Name), DbType.ToString());
				columnAttribute.Arguments.Add(new CodeAttributeArgument(fieldTypeExpress));
				if (DbType == DbTypeEnum.Binary || DbType == DbTypeEnum.Char || DbType == DbTypeEnum.NChar
					  || DbType == DbTypeEnum.NVarChar || DbType == DbTypeEnum.VarBinary || DbType == DbTypeEnum.VarChar)
				{
					columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Size)));
				}
				else if (DbType == DbTypeEnum.Decimal)
				{
					columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Precision)));
					columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Scale)));
				}
				columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Nullable)));
				property.CustomAttributes.Add(columnAttribute);
			}
		}
	}
}
