using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Basic.EntityLayer
{
	/// <summary>
	/// �ṩ�� System.Guid �������������ֱ�ʾ��ʽ�໥ת��������ת������
	/// ���ṩ��׼�������ɿ���������ݡ�
	/// </summary>
	public sealed class GuidConverter : System.ComponentModel.GuidConverter
	{
		/// <summary>
		/// ��ʼ�� Basic.EntityLayer.GuidConverter �����ʵ����
		/// </summary>
		public GuidConverter() : base() { }

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ת�����Ƿ��ʹ��ָ�������Ľ�����Դ���͵Ķ���ת��Ϊ System.Guid��
		/// </summary>
		/// <param name="context">System.ComponentModel.ITypeDescriptorContext���ṩ��ʽ�����ġ�</param>
		/// <param name="sourceType">System.Type����ʾҪ���н���ת�������͡�</param>
		/// <returns>����˶������ִ��ת������Ϊ true������Ϊ false��</returns>
		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ת�����ܷ�ʹ�������Ľ�����ת��Ϊ������Ŀ�����͡�
		/// </summary>
		/// <param name="context">System.ComponentModel.ITypeDescriptorContext���ṩ��ʽ�����ġ�</param>
		/// <param name="destinationType">��ʾҪת���������͵� System.Type��</param>
		/// <returns>�����ת�����ܹ�ִ��ת������Ϊ true������Ϊ false��</returns>
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// ����������ת��Ϊ System.Guid��
		/// </summary>
		/// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">һ����ѡ�� System.Globalization.CultureInfo�����δ�ṩ���������ã���ʹ�õ�ǰ�����ԡ�</param>
		/// <param name="value">Ҫת���� System.Object��</param>
		/// <exception cref="System.FormatException">value ����Ŀ�����͵���Чֵ��</exception>
		/// <exception cref="System.NotSupportedException">����ִ��ת����</exception>
		/// <returns>��ʾת���� value �� System.Object��</returns>
		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string) { return new Guid((string)value); }
			else if (value is byte[]) { return new Guid((byte[])value); }
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// ����������ת��Ϊ�������͡�
		/// </summary>
		/// <param name="context">��ʽ������������.</param>
		/// <param name="culture">һ����ѡ�� System.Globalization.CultureInfo�����δ�ṩ���������ã���ʹ�õ�ǰ�����ԡ�</param>
		/// <param name="value">Ҫת���� System.Object��</param>
		/// <param name="destinationType">Ҫ��ֵת���ɵ� System.Type��Ŀ��������͡�</param>
		/// <exception cref="System.NotSupportedException">����ִ��ת����</exception>
		/// <returns>��ʾת���� value �� System.Object��</returns>
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
		/// ������Guid�ַ���ֵ����ֵ��Ϊ�㡣
		/// </summary>
		public static Guid Empty { get { return Guid.Empty; } }

		private static readonly MD5 md5 = MD5.Create();
		/// <summary>ͨ��ԭ�е� Guid �������֣�����Hash һ���µ� Guid</summary>
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
		/// ������Guidֵ
		/// </summary>
		public static Guid NewGuid
		{
			get { return Guid.NewGuid(); }
		}

		/// <summary>
		/// ������Guid�ַ���ֵ, �����ַ��ָ��� 32 λ���֣�00000000-0000-0000-0000-000000000000
		/// </summary>
		public static string GuidString
		{
			get { return Guid.NewGuid().ToString("D").ToUpper(); }
		}

		/// <summary>
		/// ������Guid�ַ���ֵ�����ַ��ָ��� 32 λ���֣�00000000000000000000000000000000
		/// </summary>
		public static string NewString
		{
			get { return Guid.NewGuid().ToString("N").ToUpper(); }
		}
	}
}
