using System;
using System.ComponentModel;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Enums;

namespace Basic.Designer
{
	/// <summary>
	/// 提供将 AbstractAttribute 及其子类对象与其他各种表示形式相互转换的类型转换器。
	/// </summary>
	public sealed class CommandParameterConverter : TypeConverter
	{
		/// <summary>
		/// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="destinationType">一个 System.Type，表示要转换到的类型。</param>
		/// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}
		private PropertyDescriptorCollection propertyDescriptors = null;
		/// <summary>
		/// 使用指定的上下文和特性返回由 value 参数指定的数组类型的属性的集合。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="value">一个 System.Object，指定要为其获取属性的数组类型。</param>
		/// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
		/// <returns>具有为此数据类型公开的属性的 System.ComponentModel.PropertyDescriptorCollection；或者，如果没有属性，则为null。</returns>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
			if (propertyDescriptors == null)
				propertyDescriptors = TypeDescriptor.GetProperties(value, attributes, true).Sort(new string[] { "Name", "SourceColumn", 
					"ParameterType", "Direction", "Size","Precision", "Scale","Nullable" });
			CommandParameter parameter = value as CommandParameter;
			foreach (PropertyDescriptor property in propertyDescriptors)
			{
				if (string.IsNullOrWhiteSpace(parameter.Name) && property.Name != "Name")
				{
					continue;
				}
				else if (parameter.ParameterType == DbTypeEnum.Binary || parameter.ParameterType == DbTypeEnum.VarChar ||
					parameter.ParameterType == DbTypeEnum.Char || parameter.ParameterType == DbTypeEnum.NChar ||
					parameter.ParameterType == DbTypeEnum.NVarChar || parameter.ParameterType == DbTypeEnum.VarBinary)
				{
					if (property.Name == "Precision" || property.Name == "Scale")
						continue;
				}
				else if (parameter.ParameterType == DbTypeEnum.Decimal)
				{
					if (property.Name == "Size")
						continue;
				}
				else if (property.Name == "Size" || property.Name == "Precision" || property.Name == "Scale")
				{
					continue;
				}
				if (property.IsBrowsable)
					properties.Add(property);
			}
			return properties.Sort(new string[] { "Name", "SourceColumn", 
					"ParameterType", "Direction", "Size","Precision", "Scale","Nullable" });
		}

		/// <summary>
		/// 使用指定的上下文返回该对象是否支持属性。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <returns>如果应调用 System.ComponentModel.TypeConverter.GetProperties(System.Object) 来查找此对象的属性，则为true；否则为 false。</returns>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

	}
}
