using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 提供将 System.Guid 对象与其他各种表示形式相互转换的类型转换器。
	/// 或提供标准方法生成框架所需内容。
	/// </summary>
	public sealed class GuidConverter : System.ComponentModel.GuidConverter
	{
		/// <summary>
		/// 初始化 Basic.EntityLayer.GuidConverter 类的新实例。
		/// </summary>
		public GuidConverter() : base() { }

		/// <summary>
		/// 获取一个值，该值指示此转换器是否可使用指定上下文将给定源类型的对象转换为 System.Guid。
		/// </summary>
		/// <param name="context">System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
		/// <param name="sourceType">System.Type，表示要从中进行转换的类型。</param>
		/// <returns>如果此对象可以执行转换，则为 true；否则为 false。</returns>
		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// 获取一个值，该值指示此转换器能否使用上下文将对象转换为给定的目标类型。
		/// </summary>
		/// <param name="context">System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
		/// <param name="destinationType">表示要转换到的类型的 System.Type。</param>
		/// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// 将给定对象转换为 System.Guid。
		/// </summary>
		/// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">一个可选的 System.Globalization.CultureInfo。如果未提供区域性设置，则使用当前区域性。</param>
		/// <param name="value">要转换的 System.Object。</param>
		/// <exception cref="System.FormatException">value 不是目标类型的有效值。</exception>
		/// <exception cref="System.NotSupportedException">不能执行转换。</exception>
		/// <returns>表示转换的 value 的 System.Object。</returns>
		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string) { return new Guid((string)value); }
			else if (value is byte[]) { return new Guid((byte[])value); }
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// 将给定对象转换为其他类型。
		/// </summary>
		/// <param name="context">格式化程序上下文.</param>
		/// <param name="culture">一个可选的 System.Globalization.CultureInfo。如果未提供区域性设置，则使用当前区域性。</param>
		/// <param name="value">要转换的 System.Object。</param>
		/// <param name="destinationType">要将值转换成的 System.Type，目标对象类型。</param>
		/// <exception cref="System.NotSupportedException">不能执行转换。</exception>
		/// <returns>表示转换的 value 的 System.Object。</returns>
		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is Guid? && value != null)
			{
				Guid? nullValue = (Guid?)value;
				if (nullValue.HasValue)
					return Convert.ToString((nullValue).Value);
				return string.Empty;
			}
			else if (destinationType == typeof(string) && value is Guid)
			{
				return Convert.ToString(value);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// 生成新Guid字符串值，其值均为零。
		/// </summary>
		public static Guid Empty { get { return Guid.Empty; } }

		private static readonly MD5 md5 = MD5.Create();
		/// <summary>通过原有的 Guid 和新数字，重新Hash 一个新的 Guid</summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static Guid Create(Guid key, int value)
		{
			byte[] bytes = new byte[20];
			Array.Copy(key.ToByteArray(), bytes, 16);
			Array.Copy(BitConverter.GetBytes(value), 0, bytes, 16, 4);
			return new Guid(md5.ComputeHash(bytes));
		}

		/// <summary>
		/// 生成新Guid值
		/// </summary>
		public static Guid NewGuid
		{
			get { return Guid.NewGuid(); }
		}

		/// <summary>
		/// 生成新Guid字符串值, 由连字符分隔的 32 位数字：00000000-0000-0000-0000-000000000000
		/// </summary>
		public static string GuidString
		{
			get { return Guid.NewGuid().ToString("D").ToUpper(); }
		}

		/// <summary>
		/// 生成新Guid字符串值，无字符分隔的 32 位数字：00000000000000000000000000000000
		/// </summary>
		public static string NewString
		{
			get { return Guid.NewGuid().ToString("N").ToUpper(); }
		}
	}
}
