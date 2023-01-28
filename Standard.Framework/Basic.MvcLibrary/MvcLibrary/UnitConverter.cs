using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 
	/// </summary>
	public class UnitConverter : System.ComponentModel.TypeConverter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="sourceType"></param>
		/// <returns></returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (!(destinationType == typeof(string)) && !(destinationType == typeof(InstanceDescriptor)))
			{
				return base.CanConvertTo(context, destinationType);
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				return null;
			}
			string str = value as string;
			if (str == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			string s = str.Trim();
			if (s.Length == 0)
			{
				return Unit.Empty;
			}
			if (culture != null)
			{
				return Unit.Parse(s, culture);
			}
			return Unit.Parse(s, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (!(destinationType == typeof(string)))
			{
				if (!(destinationType == typeof(InstanceDescriptor)) || (value == null))
				{
					return base.ConvertTo(context, culture, value, destinationType);
				}
				Unit unit2 = (Unit)value;
				MemberInfo member = null;
				object[] arguments = null;
				if (unit2.IsEmpty)
				{
					member = typeof(Unit).GetField("Empty");
				}
				else
				{
					Type[] types = new Type[] { typeof(double), typeof(UnitType) };
					member = typeof(Unit).GetConstructor(types);
					arguments = new object[] { unit2.Value, unit2.Type };
				}
				if (member != null)
				{
					return new InstanceDescriptor(member, arguments);
				}
				return null;
			}
			if (value != null)
			{
				Unit unit = (Unit)value;
				if (!unit.IsEmpty)
				{
					unit = (Unit)value;
					return unit.ToString(culture);
				}
			}
			return string.Empty;
		}
	}
}
