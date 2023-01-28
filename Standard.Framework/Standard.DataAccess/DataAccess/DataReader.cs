using System;
using System.ComponentModel;
using Basic.Exceptions;
using Basic.Properties;

namespace Basic.DataAccess
{
	/// <summary>
	/// 读取类属性值
	/// </summary>
	public static class DataReader
	{
		/// <summary>
		/// 获取一个值，该值指示指定的列是否包含 null 值。
		/// </summary>
		/// <param name="value">需要判断的值</param>
		/// <returns>如果列包含 null 值，则为 true；否则为 false。</returns>
		internal static bool IsNull(object value)
		{
			if ((value != null) && !Convert.IsDBNull(value))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 检索指定对象的指定属性的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值。</returns>
		public static object GetPropertyValue(object container, string propName)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			if (string.IsNullOrEmpty(propName))
			{
				throw new ArgumentNullException("propName");
			}
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(container).Find(propName, true);
			if (descriptor == null)
			{
				//DataReader_Prop_Not_Found=DataReader: 类中'{0}'中不存在属性'{0}'。
				throw new RuntimeException(Errors.DataReader_Prop_Not_Found, container.GetType().FullName, propName);
			}
			return descriptor.GetValue(container);
		}

		/// <summary>
		/// 检索指定对象的指定属性的值并格式化结果。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <param name="format">指定结果显示格式的字符串。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static string GetPropertyValue(object container, string propName, string format)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(format))
			{
				return propertyValue.ToString();
			}
			return string.Format(format, propertyValue);
		}

		/// <summary>
		/// 检验
		/// </summary>
		/// <param name="objectiveType">目标类型</param>
		/// <param name="originalType">原始类型</param>
		private static void CheckTypeConverter(Type objectiveType, Type originalType)
		{
			if (objectiveType != originalType)
			{

			}
		}

		/// <summary>
		/// 检索指定对象的指定属性的Int16类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static short? FindInt16Value(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (short)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的Int32类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static int? FindInt32Value(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (int)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的Int64类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static long? FindInt64Value(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (long)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的Byte类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static byte? FindByteValue(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (byte)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的String类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static string FindStringValue(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return Convert.ToString(propertyValue);
		}

		/// <summary>
		/// 检索指定对象的指定属性的Guid类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static Guid? FindGuidValue(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (Guid)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的Boolean类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static bool? FindBooleanValue(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (bool)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的Guid类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static DateTime? FindDateTimeValue(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (DateTime)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的Guid类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static Decimal? FindDecimalValue(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (Decimal)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的Guid类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static Double? FindDoubleValue(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (Double)propertyValue;
		}

		/// <summary>
		/// 检索指定对象的指定属性的Guid类型的值。
		/// </summary>
		/// <param name="container">包含该属性的对象。</param>
		/// <param name="propName">包含要检索的值的属性名称。</param>
		/// <returns>指定的属性的值，格式由 format 指定。</returns>
		public static byte[] FindByteArrayValue(object container, string propName)
		{
			object propertyValue = GetPropertyValue(container, propName);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return null;
			}
			return (byte[])propertyValue;
		}
	}
}
