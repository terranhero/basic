using System;
using System.CodeDom;
using System.Globalization;
using System.Xml.Serialization;
using Basic.Enums;
using Basic.Designer;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示抽象的属性类
	/// </summary>
	public abstract class AbstractAttribute : AbstractCustomTypeDescriptor
	{
		private readonly DataEntityPropertyElement property;

		/// <summary>
		/// 初始化 AbstractAttribute 类实例。
		/// </summary>
		/// <param name="nofity">需要通知 DataEntityPropertyElement 类实例当前类的属性已更改。</param>
		protected AbstractAttribute(DataEntityPropertyElement nofity) : base(nofity) { property = nofity; }

		/// <summary>
		/// 当前特性应用的属性。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DataEntityPropertyElement Property { get { return property; } }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName()
		{
			return GetType().Name;
		}

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName()
		{
			return GetType().Name;
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property"></param>
		protected internal abstract void WriteDesignerCodeAttribute(CodeMemberProperty property);

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName
		{
			get
			{
				if (Attribute.IsDefined(GetType(), typeof(XmlRootAttribute)))
				{
					XmlRootAttribute xea = (XmlRootAttribute)Attribute.GetCustomAttribute(GetType(), typeof(XmlRootAttribute));
					return xea.ElementName;
				}
				return GetType().Name;
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value) { return false; }

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer) { }

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader) { return true; }

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
		protected internal override void GenerateConfiguration(System.Xml.XmlWriter writer, ConnectionTypeEnum connectionType) { }
	}

	/// <summary>
	/// 表示抽象的验证类型
	/// </summary>
	public abstract class AbstractValidationAttribute : AbstractAttribute
	{
		internal const string ConverterAttribute = "Converter";
		internal const string MessageAttribute = "Msg";

		/// <summary>
		/// 初始化 AbstractAttribute 类实例。
		/// </summary>
		/// <param name="nofity">需要通知 DataEntityPropertyElement 类实例当前类的属性已更改。</param>
		protected AbstractValidationAttribute(DataEntityPropertyElement nofity) : base(nofity) { }

		/// <summary>
		/// 异常关键字。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public abstract string ErrorKey { get; }

		private string _ErrorMessage = null;
		/// <summary>获取或设置一条错误消息，以便在验证失败时与验证控件关联</summary>
		[PersistentDescription("DisplayName_ErrorMessage")]
		public virtual string ErrorMessage
		{
			get { return _ErrorMessage; }
			set
			{
				if (_ErrorMessage != value)
				{
					_ErrorMessage = value;
					base.RaisePropertyChanged("ErrorMessage");
				}
			}
		}

		private string _Converter = null;
		/// <summary>
		/// 获取或设置属性的本地显示名称的转换器名称。
		/// </summary>
		[PersistentDescription("DisplayName_ConverterName"), System.ComponentModel.Browsable(false)]
		public string Converter
		{
			get { return _Converter; }
			set
			{
				if (_Converter != value)
				{
					_Converter = value;
					base.RaisePropertyChanged("Converter");
				}
			}
		}

		/// <summary>使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型。</summary>
		/// <param name="propertyType"></param>
		/// <param name="context"> 一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="culture">用作当前区域性的 System.Globalization.CultureInfo。</param>
		/// <param name="value">要转换的 System.Object。</param>
		/// <exception cref="System.NotSupportedException">不能执行转换。</exception>
		/// <returns>表示转换的 value 的 System.Object。</returns>
		protected internal object ConvertToValue(Type propertyType, CultureInfo culture, string value)
		{
			if (propertyType == typeof(System.String))
				return value;
			string strValue = Convert.ToString(value);
			if (propertyType == typeof(System.Boolean))
			{
				bool result = false;
				if (bool.TryParse(strValue, out result))
					return result;
			}
			else if (propertyType == typeof(System.Byte))
			{
				byte result = 0;
				if (byte.TryParse(strValue, NumberStyles.Integer, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.Char))
			{
				char result = char.MinValue;
				if (char.TryParse(strValue, out result))
					return result;
			}
			else if (propertyType == typeof(System.DateTime))
			{
				DateTime result = DateTime.MinValue;
				if (DateTime.TryParse(strValue, culture, DateTimeStyles.None, out result))
					return result;
			}
			else if (propertyType == typeof(System.DateTimeOffset))
			{
				DateTimeOffset result = DateTimeOffset.MinValue;
				if (DateTimeOffset.TryParse(strValue, culture, DateTimeStyles.None, out result))
					return result;
			}
			else if (propertyType == typeof(System.Decimal))
			{
				Decimal result = Decimal.Zero;
				if (Decimal.TryParse(strValue, NumberStyles.Number, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.Double))
			{
				Double result = Double.NaN;
				if (Double.TryParse(strValue, NumberStyles.Float, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.Guid))
			{
				Guid result = Guid.Empty;
				if (Guid.TryParse(strValue, out result))
					return result;
			}
			else if (propertyType == typeof(System.Int16))
			{
				Int16 result = 0;
				if (Int16.TryParse(strValue, NumberStyles.Integer, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.Int32))
			{
				Int32 result = 0;
				if (Int32.TryParse(strValue, NumberStyles.Integer, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.Int64))
			{
				Int64 result = 0;
				if (Int64.TryParse(strValue, NumberStyles.Integer, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.Single))
			{
				Single result = 0;
				if (Single.TryParse(strValue, NumberStyles.Float, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.SByte))
			{
				SByte result = 0;
				if (SByte.TryParse(strValue, NumberStyles.Integer, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.TimeSpan))
			{
				TimeSpan result = TimeSpan.Zero;
				if (TimeSpan.TryParse(strValue, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.UInt32))
			{
				UInt32 result = 0;
				if (UInt32.TryParse(strValue, NumberStyles.Integer, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.UInt64))
			{
				UInt64 result = 0;
				if (UInt64.TryParse(strValue, NumberStyles.Integer, culture, out result))
					return result;
			}
			else if (propertyType == typeof(System.UInt16))
			{
				UInt16 result = 0;
				if (UInt16.TryParse(strValue, NumberStyles.Integer, culture, out result))
					return result;
			}
			return value;
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == ConverterAttribute) { _Converter = value; return true; }
			else if (name == MessageAttribute) { _ErrorMessage = value; return true; }
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			if (!string.IsNullOrWhiteSpace(_Converter)) { writer.WriteAttributeString(ConverterAttribute, _Converter); }
			if (!string.IsNullOrWhiteSpace(_ErrorMessage)) { writer.WriteAttributeString(MessageAttribute, _ErrorMessage); }
		}
	}
}
