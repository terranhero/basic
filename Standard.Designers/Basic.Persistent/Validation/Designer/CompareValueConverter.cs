using System;
using System.ComponentModel;
using System.Globalization;

namespace Basic.DataEntities
{
	/// <summary>
	/// 提供将 object 对象与其他各种表示形式相互转换的类型转换器。
	/// </summary>
	public sealed class CompareValueConverter : TypeConverter
	{
		/// <summary>
		/// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
		/// </summary>
		/// <param name="context"> 一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="sourceType">一个 System.Type，表示要转换的类型。</param>
		/// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			ICompareProperty property = context.Instance as ICompareProperty;
			if (property != null)
				return !string.IsNullOrWhiteSpace(property.OtherProperty);
			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// 使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型。
		/// </summary>
		/// <param name="propertyType"></param>
		/// <param name="context"> 一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="culture">用作当前区域性的 System.Globalization.CultureInfo。</param>
		/// <param name="value">要转换的 System.Object。</param>
		/// <exception cref="System.NotSupportedException">不能执行转换。</exception>
		/// <returns>表示转换的 value 的 System.Object。</returns>
		private object ConvertToValue(Type propertyType, ITypeDescriptorContext context, CultureInfo culture, object value)
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
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// 使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型。
		/// </summary>
		/// <param name="context"> 一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="culture">用作当前区域性的 System.Globalization.CultureInfo。</param>
		/// <param name="value">要转换的 System.Object。</param>
		/// <exception cref="System.NotSupportedException">不能执行转换。</exception>
		/// <returns>表示转换的 value 的 System.Object。</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			AbstractAttribute validation = context.Instance as AbstractAttribute;
			ICompareProperty property = context.Instance as ICompareProperty;
			if (validation != null && property != null)
			{
				DataEntityPropertyElement otherProperty = null;
				DataEntityElement entity = validation.Property.Owner as DataEntityElement;
				if (entity.Properties.TryGetValue(property.OtherProperty, out otherProperty))
					return ConvertToValue(otherProperty.Type, context, culture, value);
				return base.ConvertFrom(context, culture, value);
			}
			return base.ConvertFrom(context, culture, value);
		}

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
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <param name="destinationType">一个 System.Type，表示要转换到的类型。</param>
		/// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			ICompareProperty property = context.Instance as ICompareProperty;
			if (property != null)
				return !string.IsNullOrWhiteSpace(property.OtherProperty);
			return base.CanConvertTo(context, destinationType);
		}
	}
}
