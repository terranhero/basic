using Basic.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Designer
{
    /// <summary>
    /// 表示数据库支持
    /// </summary>
    public sealed class SupportDatabasesConverter : TypeConverter
    {
        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context">一个 System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
        /// <param name="sourceType">一个 System.Type，表示要转换的类型。</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型。
        /// </summary>
        /// <param name="context">一个 System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
        /// <param name="destinationType">一个 System.Type，表示要转换到的类型。</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context">一个 System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
        /// <param name="culture">用作当前区域性的 System.Globalization.CultureInfo。</param>
        /// <param name="value">要转换的 System.Object。</param>
        /// <returns>表示转换的 value 的 System.Object。</returns>
        /// <exception cref="System.NotSupportedException">不能执行转换。</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] strArray = (value as string).Split(',');
                ConnectionTypeEnum result = ConnectionTypeEnum.Default;
                List<ConnectionTypeEnum> list = new List<ConnectionTypeEnum>(strArray.Length + 1);
                for (int index = 0; index < strArray.Length; index++)
                {
                    if (Enum.TryParse<ConnectionTypeEnum>(strArray[index], out result))
                    {
                        list.Add(result);
                    }
                }
                return list.ToArray();
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型。
        /// </summary>
        /// <param name="context">一个 System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
        /// <param name="culture">System.Globalization.CultureInfo。 如果传递 null，则采用当前区域性。</param>
        /// <param name="value">要转换的 System.Object。</param>
        /// <param name="destinationType">value 参数要转换成的 System.Type。</param>
        /// <returns>表示转换的 value 的 System.Object。</returns>
        /// <exception cref="System.ArgumentNullException">destinationType 参数为 null。</exception>
        /// <exception cref="System.NotSupportedException">不能执行转换。</exception>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is ConnectionTypeEnum[])
            {
                List<string> list = new List<string>();
                foreach (ConnectionTypeEnum element in (ConnectionTypeEnum[])value)
                {
                    list.Add(Enum.GetName(typeof(ConnectionTypeEnum), element));
                }
                return string.Join(",", list);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
