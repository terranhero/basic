using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Basic.Designer
{
	internal sealed class TableInfoConverter : TypeConverter
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
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);
			return properties.Sort(new string[] { "Owner", "ObjectType", "TableName", "ViewName", "EntityName", "Description" });
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
	}
}
