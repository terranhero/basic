using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.EntityLayer;
using Basic.Designer;
using Basic.Configuration;
using System.Xml;
using Basic.Enums;
using System.Drawing.Design;
using Newtonsoft.Json.Linq;

namespace Basic.Database
{
	/// <summary>
	/// 数据库表列信息
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(TableColumnConverter))]
	public sealed class DesignColumnInfo : AbstractCustomTypeDescriptor
	{
		#region 实体定义字段
		private readonly DesignTableInfo tableInfo;

		internal const string XmlElementName = "TableColumn";
		internal const string NameAttribute = "Name";
		internal const string TypeNameAttribute = "Type";
		internal const string DbTypeAttribute = "DbType";
		internal const string NullableAttribute = "Nullable";
		internal const string SizeAttribute = "Size";
		internal const string PrecisionAttribute = "Precision";
		internal const string ScaleAttribute = "Scale";
		internal const string PrimaryKeyAttribute = "PKey";
		internal const string PropertyAttribute = "Property";
		internal const string DefaultValueAttribute = "Default";
		internal const string ComputedAttribute = "Computed";
		#endregion

		/// <summary>
		/// 初始化 DesignColumnInfo 类实例
		/// </summary>
		/// <param name="table">包含此类实例的 PersistentConfiguration 类实例。</param>
		internal DesignColumnInfo(DesignTableInfo table) : this(table, null, DbTypeEnum.NVarChar, 50) { }

		/// <summary>
		/// 初始化 DesignColumnInfo 类实例
		/// </summary>
		/// <param name="table">包含此类实例的 PersistentConfiguration 类实例。</param>
		/// <param name="name">数据库表中的列的名称。</param>
		internal DesignColumnInfo(DesignTableInfo table, string name) : this(table, name, DbTypeEnum.NVarChar, 50) { }

		/// <summary>
		/// 初始化 TableColumnElement 类实例
		/// </summary>
		/// <param name="table">包含此类实例的 PersistentConfiguration 类实例。</param>
		/// <param name="name">数据库表中的列的名称。</param>
		/// <param name="dbType">数据库表中的列的类型。</param>
		internal DesignColumnInfo(DesignTableInfo table, string name, DbTypeEnum dbType) : this(table, name, dbType, 0) { }

		/// <summary>
		/// 初始化 TableColumnElement 类实例
		/// </summary>
		/// <param name="table">包含此类实例的 PersistentConfiguration 类实例。</param>
		/// <param name="name">数据库表中的列的名称。</param>
		/// <param name="dbType">数据库表中的列的类型。</param>
		/// <param name="size">列中数据的最大大小（以字节为单位）。</param>
		internal DesignColumnInfo(DesignTableInfo table, string name, DbTypeEnum dbType, int size)
		{
			tableInfo = table;
			this._Name = name;
			this._PropertyName = name;
			this._DbType = dbType;
			this._Size = size;
		}

		/// <summary>
		/// 返回表示当前 DesignerBoolReqiured 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DesignerBoolReqiured。</returns>
		public override string ToString()
		{
			return _Name;
		}

		/// <summary>
		/// 当前列是否是CreatedTime列。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		internal bool IsCreatedTimeColumn
		{
			get
			{
				return (string.Compare(_Name, DesignTableInfo.CreatedTimeColumn, true) == 0 || string.Compare(_Name, DesignTableInfo.CreateTimeColumn, true) == 0) &&
					_DbType == DbTypeEnum.DateTime;
			}
		}

		/// <summary>
		/// 当前列是否是CreatedTime列。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		internal bool IsModifiedTimeColumn
		{
			get
			{
				return ((string.Compare(_Name, DesignTableInfo.ModifiedTimeColumn, true) == 0 || string.Compare(_Name, DesignTableInfo.ModifyTimeColumn, true) == 0)
					&& (_DbType == DbTypeEnum.DateTime || _DbType == DbTypeEnum.DateTime2)) || _DbType == DbTypeEnum.Timestamp;
			}
		}

		/// <summary>
		/// 引发 FileContentChanged 事件
		/// </summary>
		/// <param name="e">引发事件的 EventArgs 类实例参数</param>
		protected internal override void OnFileContentChanged(EventArgs e)
		{
			base.OnFileContentChanged(e);
			if (tableInfo != null) { tableInfo.OnFileContentChanged(e); }
		}

		private bool _PrimaryKey = false;
		/// <summary>
		/// 当前列是否为主键
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnPrimaryKey")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(false)]
		public bool PrimaryKey
		{
			get { return _PrimaryKey; }
			set { if (_PrimaryKey != value) { _PrimaryKey = value; RaisePropertyChanged("PrimaryKey"); } }
		}

		private bool _Computed = false;
		/// <summary>
		/// 当前列是否为计算列
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnComputed")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Computed
		{
			get { return _Computed; }
			set { if (_Computed != value) { _Computed = value; RaisePropertyChanged("Computed"); } }
		}

		private string _Name = string.Empty;
		/// <summary>
		/// 获取或设置数据库表中的列的名称。
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnName")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
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

		private string _PropertyName = string.Empty;
		/// <summary>
		/// 获取或设置数据库表中的列的名称。
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_PropertyName")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue("")]
		public string PropertyName
		{
			get { return _PropertyName; }
			set
			{
				if (_PropertyName != value)
				{
					_PropertyName = value;
					RaisePropertyChanged("PropertyName");
				}
			}
		}

		private string _Comment;
		/// <summary>
		/// 属性对应数据库字段描述
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_PropertyComment")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue("")]
		public string Comment
		{
			get { return _Comment; }
			set
			{
				if (_Comment != value)
				{
					_Comment = value;
					RaisePropertyChanged("Comment");
				}
			}
		}

		private string _TypeName = null;
		/// <summary>属性类型名称</summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyType")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.Editor(typeof(ReflectedTypeEditor), typeof(UITypeEditor))]
		public string TypeName
		{
			get
			{
				return _TypeName;
			}
			set
			{
				_TypeName = value;
				RaisePropertyChanged("TypeName");
			}
		}

		private DbTypeEnum _DbType = DbTypeEnum.NVarChar;
		/// <summary>
		/// 属性类型名称
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnDbType")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
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
				}
			}
		}

		private int _Size = 0;
		/// <summary>
		/// 获取或设置列中数据的最大大小（以字节为单位）。
		/// </summary>
		/// <value>列中数据的最大大小（以字节为单位）。默认值是从参数值推导出的。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnSize")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(0)]
		public int Size
		{
			get { return _Size; }
			set { if (_Size != value) { _Size = value; RaisePropertyChanged("Size"); } }
		}

		/// <summary>
		/// 需要使用Size属性的字段类型
		/// </summary>
		private bool UsedSize
		{
			get
			{
				switch (_DbType)
				{
					case DbTypeEnum.Binary:
					case DbTypeEnum.VarBinary:
					case DbTypeEnum.Char:
					case DbTypeEnum.VarChar:
					case DbTypeEnum.NChar:
					case DbTypeEnum.NVarChar:
						return true;
					default:
						return false;
				}
			}
		}

		private byte _Precision = 0;
		/// <summary>
		/// 获取或设置用来表示 Value 属性的最大位数。 
		/// </summary>
		/// <value>用于表示 Value 属性的最大位数。 默认值为 0。这指示数据提供程序设置 Value 的精度。 </value>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnPrecision")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
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
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnScale")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(typeof(byte), "0")]
		public byte Scale
		{
			get { return _Scale; }
			set { if (_Scale != value) { _Scale = value; RaisePropertyChanged("Scale"); } }
		}

		private bool _Nullable = false;
		/// <summary>
		/// 属性是否允许为空
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_PropertyNullable")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
		[System.ComponentModel.DefaultValue(typeof(bool), "false")]
		public bool Nullable
		{
			get { return _Nullable; }
			set
			{
				if (_Nullable != value)
				{
					_Nullable = value;
					RaisePropertyChanged("Nullable");
				}
			}
		}

		private string _DefaultValue = null;
		/// <summary>
		/// 属性的默认值
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_PropertyDefaultValue")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryColumn)]
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
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == NameAttribute) { _Name = value; return true; }
			else if (name == DbTypeAttribute) { return Enum.TryParse<DbTypeEnum>(value, out _DbType); }
			else if (name == TypeNameAttribute) { _TypeName = value; return true; }
			else if (name == NullableAttribute) { _Nullable = Convert.ToBoolean(value); return true; }
			else if (name == SizeAttribute) { _Size = Convert.ToInt32(value); return true; }
			else if (name == PrecisionAttribute) { _Precision = Convert.ToByte(value); return true; }
			else if (name == ScaleAttribute) { _Scale = Convert.ToByte(value); return true; }
			else if (name == PrimaryKeyAttribute) { _PrimaryKey = Convert.ToBoolean(value); return true; }
			else if (name == DefaultValueAttribute) { _DefaultValue = value; return true; }
			else if (name == ComputedAttribute) { _Computed = Convert.ToBoolean(value); return true; }
			else if (name == PropertyAttribute) { _PropertyName = value; return true; }
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>如果当前节点已经读到结尾则返回 true，否则返回 false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == System.Xml.XmlNodeType.Text && reader.LocalName == string.Empty)
			{
				_Comment = reader.ReadString(); return false;
			}
			else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == XmlElementName)
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
			writer.WriteAttributeString(DbTypeAttribute, _DbType.ToString());
			if (_Nullable)
				writer.WriteAttributeString(NullableAttribute, "true");
			if (_Size > 0 && UsedSize)
				writer.WriteAttributeString(SizeAttribute, Convert.ToString(_Size));
			if (_Precision > 0 && _DbType == DbTypeEnum.Decimal)
				writer.WriteAttributeString(PrecisionAttribute, Convert.ToString(_Precision));
			if (_Scale > 0 && _DbType == DbTypeEnum.Decimal)
				writer.WriteAttributeString(ScaleAttribute, Convert.ToString(_Scale));
			if (_PrimaryKey)
				writer.WriteAttributeString(PrimaryKeyAttribute, "true");
			if (!string.IsNullOrWhiteSpace(_TypeName))
				writer.WriteAttributeString(TypeNameAttribute, _TypeName);
			if (!string.IsNullOrWhiteSpace(_PropertyName))
				writer.WriteAttributeString(PropertyAttribute, _PropertyName);
			if (!string.IsNullOrWhiteSpace(_DefaultValue))
				writer.WriteAttributeString(DefaultValueAttribute, _DefaultValue);

			if (_Computed)
				writer.WriteAttributeString(ComputedAttribute, "true");
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer) { writer.WriteString(_Comment); }

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
			writer.WriteStartElement(XmlElementName);
			writer.WriteAttributeString(NameAttribute, _Name);
			writer.WriteAttributeString(DbTypeAttribute, _DbType.ToString());
			if (_Nullable)
				writer.WriteAttributeString(NullableAttribute, "true");

			if (_Size > 0 && UsedSize)
				writer.WriteAttributeString(SizeAttribute, Convert.ToString(_Size));
			if (_Precision > 0 && _DbType == DbTypeEnum.Decimal)
				writer.WriteAttributeString(PrecisionAttribute, Convert.ToString(_Precision));
			if (_Scale > 0 && _DbType == DbTypeEnum.Decimal)
				writer.WriteAttributeString(ScaleAttribute, Convert.ToString(_Scale));
			if (_PrimaryKey) { writer.WriteAttributeString(PrimaryKeyAttribute, "true"); }
			if (!string.IsNullOrWhiteSpace(_TypeName))
				writer.WriteAttributeString(TypeNameAttribute, _TypeName);
			if (!string.IsNullOrWhiteSpace(_DefaultValue))
				writer.WriteAttributeString(DefaultValueAttribute, _DefaultValue);
			if (_Computed) { writer.WriteAttributeString(ComputedAttribute, "true"); }
			writer.WriteString(_Comment);
			writer.WriteEndElement();
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
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return null; } }

		#endregion
	}
}
