using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Basic.Collections;

namespace Basic.Designer
{
	/// <summary>
	/// 
	/// </summary>
	public class NamespacesConverter : TypeConverter
	{
		/// <summary>
		/// 使用指定的上下文和特性返回由 value 参数指定的数组类型的属性的集合。
		/// </summary>
		/// <param name="context"> 一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="value"> 一个 System.Object，指定要为其获取属性的数组类型。</param>
		/// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
		/// <returns>具有为此数据类型公开的属性的 System.ComponentModel.PropertyDescriptorCollection；或者，如果没有属性，则为 null。</returns>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
			if (value is NamespaceCollection)
			{
				NamespaceCollection nslist = value as NamespaceCollection;
				for (int index = 0; index < nslist.Count; index++)
					properties.Add(new NamespaceDescriptor(index));
			}
			return properties;
		}

		/// <summary>
		/// 返回此对象是否支持属性。
		/// </summary>
		/// <param name="context"> 一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <returns>如果应调用 System.ComponentModel.TypeConverter.GetProperties(System.Object) 
		/// 来查找此对象的属性，则为 true；否则为 false。</returns>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		private class NamespaceDescriptor : PropertyDescriptor
		{
			private readonly int keyIndex = -1;
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pKeyName"></param>
			internal NamespaceDescriptor(int index)
				: base(string.Format("Namespace[{0}]", index), null)
			{
				keyIndex = index;
			}

			/// <summary>
			/// 返回重置对象时是否更改其值。
			/// </summary>
			/// <param name="component">要测试重置功能的组件。</param>
			/// <returns>如果重置组件更改其值，则为 true；否则为 false。</returns>
			public override bool CanResetValue(object component)
			{
				return true;
			}

			/// <summary>
			/// 获取该属性绑定到的组件的类型。
			/// </summary>
			public override Type ComponentType
			{
				get { return typeof(string); }
			}

			/// <summary>
			/// 获取组件上的属性的当前值。
			/// </summary>
			/// <param name="component">具有为其检索值的属性的组件。</param>
			/// <returns>给定组件的属性的值。</returns>
			public override object GetValue(object component)
			{
				return (component as NamespaceCollection)[keyIndex];
			}

			/// <summary>
			/// 获取指示该属性是否为只读的值。
			/// </summary>
			public override bool IsReadOnly
			{
				get { return false; }
			}

			/// <summary>
			/// 获取该属性的类型。
			/// </summary>
			public override Type PropertyType
			{
				get { return typeof(object); }
			}

			/// <summary>
			/// 将组件的此属性的值重置为默认值。
			/// </summary>
			/// <param name="component">具有要重置为默认值的属性值的组件。</param>
			public override void ResetValue(object component)
			{
				(component as NamespaceCollection)[keyIndex] = null;
			}

			/// <summary>
			/// 将组件的值设置为一个不同的值。
			/// </summary>
			/// <param name="component">具有要进行设置的属性值的组件。</param>
			/// <param name="value">新值。</param>
			public override void SetValue(object component, object value)
			{
				(component as NamespaceCollection)[keyIndex] = (string)value;
			}

			/// <summary>
			/// 确定一个值，该值指示是否需要永久保存此属性的值。
			/// </summary>
			/// <param name="component">具有要检查其持久性的属性的组件。</param>
			/// <returns>如果属性应该被永久保存，则为 true；否则为 false。</returns>
			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
	}
}
