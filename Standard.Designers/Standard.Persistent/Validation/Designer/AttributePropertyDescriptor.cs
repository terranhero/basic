using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Basic.DataEntities;
using Basic.Properties;

namespace Basic.Designer
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class AttributePropertyDescriptor : PropertyDescriptor
	{
		private readonly AbstractAttribute abstractAttribute;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="abstractAttribute"></param>
		public AttributePropertyDescriptor(AbstractAttribute attribute)
			: base(attribute.ElementName, new Attribute[0]) { abstractAttribute = attribute; }

		/// <summary>
		/// 返回重置对象时是否更改其值。
		/// </summary>
		/// <param name="component">要测试重置功能的组件。</param>
		/// <returns>如果重置组件更改其值，则为 true；否则为 false。</returns>
		public override bool CanResetValue(object component) { return false; }

		/// <summary>
		/// 获取该属性绑定到的组件的类型。
		/// </summary>
		public override Type ComponentType { get { return abstractAttribute.GetType(); } }

		/// <summary>
		/// 获取组件上的属性的当前值。
		/// </summary>
		/// <param name="component">具有为其检索值的属性的组件。</param>
		/// <returns>给定组件的属性的值。</returns>
		public override object GetValue(object component) { return abstractAttribute; }

		/// <summary>
		/// 获取该成员所属的类别的名称，如 System.ComponentModel.CategoryAttribute 中所指定的。
		/// </summary>
		/// <value>该成员所属的类别的名称。如果没有 System.ComponentModel.CategoryAttribute，类别名将被设置为默认类别 Misc。</value>
		public override string Category
		{
			get
			{
				if (Attribute.IsDefined(abstractAttribute.GetType(), typeof(PersistentCategoryAttribute)))
				{
					PersistentCategoryAttribute pca = (PersistentCategoryAttribute)Attribute.GetCustomAttribute(abstractAttribute.GetType(),
						typeof(PersistentCategoryAttribute));
					return pca.Category;
				}
				return base.Category;
			}
		}

		/// <summary>
		///   获取成员的说明，如 System.ComponentModel.DescriptionAttribute 中所指定的。
		/// </summary>
		/// <value>成员的说明。如果没有 System.ComponentModel.DescriptionAttribute，属性值被设置为默认值，它是一个空字符串 ("")。</value>
		public override string Description
		{
			get
			{
				if (Attribute.IsDefined(abstractAttribute.GetType(), typeof(PersistentDescriptionAttribute)))
				{
					PersistentDescriptionAttribute pda = (PersistentDescriptionAttribute)Attribute.GetCustomAttribute(abstractAttribute.GetType(),
						typeof(PersistentDescriptionAttribute));
					return pda.Description;
				}
				return base.Description;
			}
		}

		/// <summary>
		/// 获取可以显示在窗口（如“属性”窗口）中的名称。
		/// </summary>
		/// <value>为该成员显示的名称。</value>
		public override string DisplayName
		{
			get
			{
				if (Attribute.IsDefined(abstractAttribute.GetType(), typeof(DisplayNameAttribute)))
				{
					DisplayNameAttribute dna = (DisplayNameAttribute)Attribute.GetCustomAttribute(abstractAttribute.GetType(),
						typeof(DisplayNameAttribute));
					return dna.DisplayName;
				}
				return base.DisplayName;
			}
		}
		/// <summary>
		/// 获取指示该属性是否为只读的值。
		/// </summary>
		public override bool IsReadOnly { get { return true; } }

		/// <summary>
		/// 获取该属性的类型。
		/// </summary>
		public override Type PropertyType { get { return abstractAttribute.GetType(); } }

		/// <summary>
		/// 
		/// </summary>
		public override TypeConverter Converter
		{
			get
			{
				return new ValidationTypeConverter();
			}
		}
		/// <summary>
		/// 将组件的此属性的值重置为默认值。
		/// </summary>
		/// <param name="component">具有要重置为默认值的属性值的组件。</param>
		public override void ResetValue(object component) { }

		/// <summary>
		/// 将组件的值设置为一个不同的值。
		/// </summary>
		/// <param name="component">具有要进行设置的属性值的组件。</param>
		/// <param name="value">新值。</param>
		public override void SetValue(object component, object value) { }

		/// <summary>
		/// 确定一个值，该值指示是否需要永久保存此属性的值。
		/// </summary>
		/// <param name="component">具有要检查其持久性的属性的组件。</param>
		/// <returns>如果属性应该被永久保存，则为 true；否则为 false。</returns>
		public override bool ShouldSerializeValue(object component) { return false; }
	}
}
