using System;
using System.Xml.Linq;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;
using Microsoft.VisualStudio.Settings.Internal;
using Newtonsoft.Json.Linq;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示属性生成元素
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(DisplayNameConverter))]
	[PersistentCategory("PersistentCategory_Attributes"), PersistentDescription("DisplayName_Description")]
	public sealed class DesignerPropertyGenerator : AbstractCustomTypeDescriptor
	{
		internal const string XmlElementName = "Generator";
		internal const string ModifierAttribute = "Modifier";
		internal const string IgnoreAttribute = "Ignore";
		internal const string IgnoreSerializeAttribute = "Serialize";
		internal const string MemberAttribute = "Member";
		internal const string InheritAttribute = "Inherit";
		internal const string OverrideAttribute = "Override";
		internal const string VirtualAttribute = "Virtual";
		private readonly AbstractDesignerProperty propertyElement;
		/// <summary>
		/// 初始化 DesignerPropertyGenerator 类实例。
		/// </summary>
		/// <param name="property"></param>
		internal DesignerPropertyGenerator(AbstractDesignerProperty property)
			: base(property) { propertyElement = property; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return XmlElementName; }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return null; } }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return typeof(DesignerPropertyGenerator).Name; }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		private PropertyModifierEnum _Modifier = PropertyModifierEnum.Public;
		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(typeof(PropertyModifierEnum), "Public"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		public PropertyModifierEnum Modifier
		{
			get { return _Modifier; }
			set
			{
				if (_Modifier != value)
				{
					_Modifier = value;
					RaisePropertyChanged("Modifier");
				}
			}
		}

		private bool _Ignore = false;
		/// <summary>是否为属性添加 IgnorePropertyAttribute 特性标记</summary>
		/// <value>是否 IgnoreProperty 标记属性，true 表示添加特性标记。默认值为 false。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyIgnore")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Ignore
		{
			get { return _Ignore; }
			set
			{
				if (_Ignore != value)
				{
					_Ignore = value;
					base.RaisePropertyChanged("Ignore");
				}
			}
		}

		private IgnoreSerializes _IgnoreSerialize = IgnoreSerializes.Serialized;
		/// <summary>获取或设置序列化时忽略当前属性的条件。</summary>
		/// <value>一个 <see cref="IgnoreConditions"/> 枚举值，指定在何种条件下忽略属性的序列化。默认值为 <c>Always</c>。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_IgnoreSerialize")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(typeof(IgnoreSerializes), "Serialized")]
		public IgnoreSerializes IgnoreSerialize
		{
			get { return _IgnoreSerialize; }
			set
			{
				if (_IgnoreSerialize != value)
				{
					_IgnoreSerialize = value;
					base.RaisePropertyChanged("IgnoreSerialize");
				}
			}
		}

		private bool _DataMember = false;
		/// <summary>
		/// 是否为继承属性，如果是继承属性，则在当前类实例中不写入属性信息。
		/// </summary>
		/// <value>是否为继承属性，true 表示是继承属性。默认值为 false。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyDataMember")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool DataMember
		{
			get { return _DataMember; }
			set
			{
				if (_DataMember != value)
				{
					_DataMember = value;
					base.RaisePropertyChanged("DataMember");
				}
			}
		}

		private bool _Inheritance = false;
		/// <summary>
		/// 是否为继承属性，如果是继承属性，则在当前类实例中不写入属性信息。
		/// </summary>
		/// <value>是否为继承属性，true 表示是继承属性。默认值为 false。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyInheritance")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Inheritance
		{
			get { return _Inheritance; }
			set
			{
				if (_Inheritance != value)
				{
					_Inheritance = value;
					base.RaisePropertyChanged("Inheritance");
				}
			}
		}

		private bool _Override = false;
		/// <summary>
		/// 获取或设置属性的本地显示名称的转换器名称。
		/// </summary>
		[PersistentDescription("PersistentDescription_PropertyOverride")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Override
		{
			get { return _Override; }
			set
			{
				if (_Override != value)
				{
					_Override = value;
					if (_Override) { Virtual = false; }
					base.RaisePropertyChanged("Override");
				}
			}
		}

		private bool _Virtual = false;
		/// <summary>
		/// 获取或设置属性的本地显示名称的转换器名称。
		/// </summary>
		[PersistentDescription("PersistentDescription_PropertyVirtual")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Virtual
		{
			get { return _Virtual; }
			set
			{
				if (_Virtual != value)
				{
					_Virtual = value;
					if (_Virtual) { Override = false; }
					base.RaisePropertyChanged("Virtual");
				}
			}
		}

		/// <summary>
		/// 是否是空
		/// </summary>
		internal bool IsEmpty
		{
			get
			{
				if (_Modifier != PropertyModifierEnum.Public) { return false; }
				else if (_DataMember || _Ignore || _Inheritance || _Override || _Virtual) { return false; }
				else if (_IgnoreSerialize !=  IgnoreSerializes.Serialized) { return false; }
				return true;
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
			if (name == ModifierAttribute) { return Enum.TryParse(value, out _Modifier); }
			else if (name == MemberAttribute) { _DataMember = Convert.ToBoolean(value); return true; }
			else if (name == IgnoreAttribute) { _Ignore = Convert.ToBoolean(value); return true; }
			else if (name == IgnoreSerializeAttribute) { return Enum.TryParse(value, out _IgnoreSerialize); }
			else if (name == InheritAttribute) { _Inheritance = Convert.ToBoolean(value); return true; }
			else if (name == OverrideAttribute) { _Override = Convert.ToBoolean(value); return true; }
			else if (name == VirtualAttribute) { _Virtual = Convert.ToBoolean(value); return true; }
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override bool ReadContent(System.Xml.XmlReader reader) { return true; }

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			if (_Modifier != PropertyModifierEnum.Public)
				writer.WriteAttributeString(ModifierAttribute, _Modifier.ToString());
			if (_Ignore) { writer.WriteAttributeString(IgnoreAttribute, Convert.ToString(_Ignore).ToLower()); }
			if (_IgnoreSerialize != IgnoreSerializes.Serialized) { writer.WriteAttributeString(IgnoreSerializeAttribute, _IgnoreSerialize.ToString()); }
			if (_DataMember)
				writer.WriteAttributeString(MemberAttribute, Convert.ToString(_DataMember).ToLower());
			if (_Inheritance)
				writer.WriteAttributeString(InheritAttribute, Convert.ToString(_Inheritance).ToLower());
			if (_Override)
				writer.WriteAttributeString(OverrideAttribute, Convert.ToString(_Override).ToLower());
			if (_Virtual)
				writer.WriteAttributeString(VirtualAttribute, Convert.ToString(_Virtual).ToLower());
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer) { }

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(System.Xml.XmlWriter writer, ConnectionTypes connectionType) { }
	}

	/// <summary>
	/// 定义属性在序列化过程中被忽略的条件。
	/// </summary>
	/// <remarks>
	/// 该枚举用于 <see cref="IgnorePropertyAttribute"/> 中，提供灵活的忽略策略，
	/// 以便在不同场景下控制属性的序列化行为。
	/// </remarks>
	public enum IgnoreSerializes
	{
		/// <summary>
		/// 属性始终被序列化。
		/// </summary>
		Serialized = 0,

		/// <summary>
		/// 属性始终被忽略，无论其值是什么。
		/// </summary>
		/// <remarks>
		/// 使用此选项时，该属性不会出现在 ToString(bool) 方法生成的字符串表示中。
		/// 适用于不希望暴露的敏感信息或内部使用的属性。
		/// </remarks>
		Always = 1,

		/// <summary>
		/// 仅当属性的值为 null 时才忽略该属性。
		/// 此条件仅适用于引用类型（类、接口、委托、字符串、数组等）的属性或字段。
		/// </summary>
		/// <remarks>
		/// 使用此选项可以精简输出字符串，避免序列化大量无意义的 null 值属性。
		/// 如果属性有非 null 值（包括空字符串或空集合），则仍会正常序列化。
		/// 对于值类型（如 int、bool、DateTime 等），由于不能为 null，此条件等同于不忽略。
		/// </remarks>
		/// <example>
		/// <code>
		/// public class Product
		/// {
		///     public string Name { get; set; }
		///     
		///     [IgnoreProperty(IgnoreConditions.WhenIsNull)]
		///     public string Description { get; set; }  // 仅在为 null 时忽略
		///     
		///     [IgnoreProperty(IgnoreConditions.WhenIsNull)]
		///     public List&lt;string&gt; Tags { get; set; }  // 仅在为 null 时忽略，空列表不会忽略
		/// }
		/// </code>
		/// </example>
		WhenIsNull = 3,
	}
}
