using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Basic.EntityLayer
{
    /// <summary>
    /// 提供将 System.Boolean 对象与其他各种表示形式相互转换的类型转换器。
    /// </summary>
    public class BooleanConverter : System.ComponentModel.BooleanConverter
    {
        /// <summary>
        /// 初始化 Basic.EntityLayer.BooleanConverter 类的实例。
        /// </summary>
        public BooleanConverter() : base() { }

        /// <summary>
        /// 获取一个值，该值指示此转换器是否可使用指定上下文将给定源类型的对象转换为 System.DateTime。
        /// </summary>
        /// <param name="context">System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
        /// <param name="sourceType">System.Type，表示要从中进行转换的类型。</param>
        /// <returns>如果此对象可以执行转换，则为 true；否则为 false。</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// 获取一个值，该值指示此转换器能否使用上下文将对象转换为给定的目标类型。
        /// </summary>
        /// <param name="context">System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
        /// <param name="destinationType">表示要转换到的类型的 System.Type。</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// 将给定的值对象转换为 System.DateTime。
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">一个可选的 System.Globalization.CultureInfo。如果未提供区域性设置，则使用当前区域性。</param>
        /// <param name="value">要转换的 System.Object。</param>
        /// <exception cref="System.FormatException">value 不是目标类型的有效值。</exception>
        /// <exception cref="System.NotSupportedException">不能执行转换。</exception>
        /// <returns>表示转换的 value 的 System.Object。</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string && !string.IsNullOrEmpty((string)value))
            {
                if (((string)value) == "Y" || ((string)value) == "N" || ((string)value) == "Yes" || ((string)value) == "No")
                {
                    return ((string)value) == "Y" || ((string)value) == "Yes";
                }
            }
            else if (value is long || value is int || value is short)
            {
                return Convert.ToInt64(value) > 0;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to a System.DateTime using the arguments.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">一个可选的 System.Globalization.CultureInfo。如果未提供区域性设置，则使用当前区域性。</param>
        /// <param name="value">要转换的 System.Object。</param>
        /// <param name="destinationType">要将值转换成的 System.Type。</param>
        /// <exception cref="System.NotSupportedException">不能执行转换。</exception>
        /// <returns>表示转换的 value 的 System.Object。</returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is bool)
            {
                return Convert.ToString(((bool)value));
            }
            else if ((destinationType == typeof(long)) && value is bool)
            {
                return ((bool)value) ? 1L : 0L;
            }
            else if ((destinationType == typeof(int)) && value is bool)
            {
                return ((bool)value) ? 1 : 0;
            }
            else if ((destinationType == typeof(short)) && value is bool)
            {
                return ((bool)value) ? (short)1 : (short)0;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
