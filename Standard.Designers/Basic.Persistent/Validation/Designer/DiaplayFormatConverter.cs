using System.ComponentModel;
using System;

namespace Basic.DataEntities
{
	/// <summary>
	/// 提供将 DiaplayFormat 对象与其他各种表示形式相互转换的类型转换器。
	/// </summary>
	public sealed class DiaplayFormatConverter : TypeConverter
	{
		/// <summary>
		/// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="culture">System.Globalization.CultureInfo。如果传递 null，则采用当前区域性。</param>
		/// <param name="value">要转换的 System.Object。</param>
		/// <param name="destinationType">value 参数要转换到的 System.Type。</param>
		/// <exception cref="System.ArgumentNullException">destinationType 参数为 null。</exception>
		/// <exception cref="System.NotSupportedException">不能执行转换。</exception>
		/// <returns>表示转换的 value 的 System.Object。</returns>
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			DisplayFormat format = value as DisplayFormat;
			if (format != null && !string.IsNullOrWhiteSpace(format.DataFormatString))
			{
				return format.DataFormatString;
			}
			return value.GetType().Name;
		}

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

		/// <summary>
		/// 使用指定的上下文和特性返回由 value 参数指定的数组类型的属性的集合。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="value">一个 System.Object，指定要为其获取属性的数组类型。</param>
		/// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
		/// <returns>具有为此数据类型公开的属性的 System.ComponentModel.PropertyDescriptorCollection；或者，如果没有属性，则为null。</returns>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(value.GetType(), attributes);
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
