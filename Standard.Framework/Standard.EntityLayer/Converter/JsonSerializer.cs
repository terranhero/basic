using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Web;
using Basic.Properties;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 自定义Json序列化帮助类
	/// </summary>
	public static class JsonSerializer
	{
		/// <summary>
		/// 序列化DataEntity子类实例
		/// </summary>
		/// <param name="entity">需要序列化的DataEntity子类实例</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public static string SerializeEntity(AbstractEntity entity)
		{
			return SerializeObject(entity, false);
		}

		/// <summary>
		/// 序列化DataEntity子类实例
		/// </summary>
		/// <param name="entity">需要序列化的DataEntity子类实例</param>
		/// <param name="includeBrace">是否需要包含Json对象的大括号</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public static string SerializeEntity(AbstractEntity entity, bool includeBrace)
		{
			return SerializeObject(entity, includeBrace);
		}

		/// <summary>
		/// 序列化DataEntity子类实例
		/// </summary>
		/// <param name="entities">需要序列化的DataEntity子类实例</param>
		/// <param name="includeBrace">是否需要包含Json对象的大括号</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public static string SerializeEntity(AbstractEntity[] entities, bool includeBrace)
		{
			return SerializeObject(entities, includeBrace);
		}

		/// <summary>
		/// 将对象转换为 JSON 字符串。
		/// </summary>
		/// <param name="value">需要序列化的DataEntity子类实例</param>
		/// <param name="includeBrace">是否需要包含Json对象的大括号</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public static string Serialize<T>(T value, bool includeBrace)
		{
			StringBuilder resultBuilder = new StringBuilder(500);
			SerializeValuePrivate(resultBuilder, value, 0, null);
			if (!includeBrace)
			{
				resultBuilder.Replace("{", "", 0, 1);
				return resultBuilder.Replace("}", "", resultBuilder.Length - 1, 1).ToString();
			}
			return resultBuilder.ToString();
		}

		/// <summary>
		/// 将对象转换为 JSON 字符串。
		/// </summary>
		/// <param name="value">需要序列化的DataEntity子类实例</param>
		/// <param name="includeBrace">是否需要包含Json对象的大括号</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public static string Serialize<T>(IEnumerable<T> value, bool includeBrace)
		{
			StringBuilder resultBuilder = new StringBuilder(500);
			SerializeValuePrivate(resultBuilder, value, 0, null);
			if (!includeBrace)
			{
				resultBuilder.Replace("{", "", 0, 1);
				return resultBuilder.Replace("}", "", resultBuilder.Length - 1, 1).ToString();
			}
			return resultBuilder.ToString();
		}

		/// <summary>
		/// 将对象转换为 JSON 字符串。
		/// </summary>
		/// <param name="value">需要序列化的DataEntity子类实例</param>
		/// <param name="includeBrace">是否需要包含Json对象的大括号</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public static string SerializeObject(object value, bool includeBrace)
		{
			StringBuilder resultBuilder = new StringBuilder(500);
			SerializeValuePrivate(resultBuilder, value, 0, null);
			if (!includeBrace)
			{
				resultBuilder.Replace("{", "", 0, 1);
				return resultBuilder.Replace("}", "", resultBuilder.Length - 1, 1).ToString();
			}
			return resultBuilder.ToString();
		}

		/// <summary>
		/// 将对象转换为 JSON 字符串。
		/// </summary>
		/// <param name="value">需要序列化的DataEntity子类实例</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public static string SerializeObject(object value)
		{
			return SerializeObject(value, false);
		}

		private static void SerializeValuePrivate(StringBuilder sb, object value, int depth, string format)
		{
			if (value == null || DBNull.Value == value)
			{
				sb.Append("null");
			}
			else
			{
				if (value is string)
				{
					SerializeString(sb, (string)value, format);
				}
				else if (value is char)
				{
					SerializeChar(sb, (char)value, format);
				}
				else if (value is bool)
				{
					SerializeBoolean(sb, (bool)value);
				}
				else if (value is DateTime)
				{
					SerializeDateTime(sb, (DateTime)value, format);
				}
				else if (value is System.TimeSpan)
				{
					SerializeTimeSpan(sb, (System.TimeSpan)value, format);
				}
				else if (value is DateTimeOffset)
				{
					DateTimeOffset offset = (DateTimeOffset)value;
					SerializeDateTime(sb, offset.UtcDateTime, format);
				}
				else if (value is Guid)
				{
					SerializeGuid(sb, (Guid)value);
				}
				else
				{
					if (value is Uri)
					{
						SerializeUri(sb, value as Uri);
					}
					else if (value is double)
					{
						sb.Append(((double)value).ToString("r", CultureInfo.InvariantCulture));
					}
					else if (value is float)
					{
						sb.Append(((float)value).ToString("r", CultureInfo.InvariantCulture));
					}
					else if (value.GetType().IsPrimitive || (value is decimal))
					{
						if (value is IConvertible)
							sb.Append((value as IConvertible).ToString(CultureInfo.InvariantCulture));
						else
							sb.Append(value.ToString());
					}
					else
					{
						Type enumType = value.GetType();
						if (enumType.IsEnum)
						{
							Type underlyingType = Enum.GetUnderlyingType(enumType);
							if ((underlyingType == typeof(long)) || (underlyingType == typeof(ulong)))
								throw new InvalidOperationException(Strings.JSON_InvalidEnumType);
							sb.Append(((Enum)value).ToString("D"));
						}
						else
						{
							if (value is IDictionary)
							{
								SerializeDictionary(sb, value as IDictionary, depth);
							}
							else
							{
								IEnumerable enumerable = value as IEnumerable;
								if (enumerable != null)
								{
									SerializeEnumerable(sb, enumerable, depth);
								}
								else
								{
									SerializeCustomObject(sb, value, depth);
								}
							}
						}
					}
				}
			}
		}
		private static void SerializeCustomObject(StringBuilder sb, object value, int depth)
		{
			Type type = value.GetType();
			bool flag = true;
			sb.Append('{');
			foreach (FieldInfo info in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				if (!flag) { sb.Append(','); }
				DataMemberAttribute dma = info.GetCustomAttribute<DataMemberAttribute>();
				if (dma != null && dma.Name != null) { SerializeString(sb, dma.Name, null); }
				else { SerializeString(sb, info.Name, null); }

				sb.Append(':');
				SerializeValuePrivate(sb, FieldInfoGetValue(info, value), depth, null);
				flag = false;
			}
			foreach (PropertyInfo info2 in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
			{
				if (info2.Name == "EntityKey" || info2.Name == "EntityState") { continue; }

				MethodInfo getMethod = info2.GetGetMethod();
				if ((getMethod != null) && (getMethod.GetParameters().Length <= 0))
				{
					if (flag == false) { sb.Append(','); }
					DataMemberAttribute dma = info2.GetCustomAttribute<DataMemberAttribute>();
					if (dma != null && dma.Name != null) { SerializeString(sb, dma.Name, null); }
					else { SerializeString(sb, info2.Name, null); }
					sb.Append(':');
					DisplayFormatAttribute dfa = info2.GetCustomAttribute<DisplayFormatAttribute>();
					object propValue = MethodInfoInvoke(getMethod, value, null);
					if (dfa != null) { SerializeValuePrivate(sb, propValue, depth, dfa.DataFormatString); }
					else { SerializeValuePrivate(sb, propValue, depth, null); }

					flag = false;
				}
			}
			sb.Append('}');
		}
		private static void SerializeDictionary(StringBuilder sb, IDictionary value, int depth)
		{
			sb.Append('{');
			bool flag = true;
			bool flag2 = false;
			if (value.Contains("__type"))
			{
				flag = false;
				flag2 = true;
				SerializeDictionaryKeyValue(sb, "__type", value["__type"], depth);
			}
			foreach (DictionaryEntry entry in value)
			{
				string key = entry.Key as string;
				if (key == null)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Strings.JSON_DictionaryTypeNotSupported, new object[] { value.GetType().FullName }));
				}
				if (flag2 && string.Equals(key, "__type", StringComparison.Ordinal))
				{
					flag2 = false;
				}
				else
				{
					if (!flag) { sb.Append(','); }
					SerializeDictionaryKeyValue(sb, key, entry.Value, depth);
					flag = false;
				}
			}
			sb.Append('}');

		}
		private static void SerializeDictionaryKeyValue(StringBuilder sb, string key, object value, int depth)
		{
			SerializeString(sb, key, null);
			sb.Append(':');
			SerializeValuePrivate(sb, value, depth, null);
		}
		private static void SerializeEnumerable(StringBuilder sb, IEnumerable enumerable, int depth)
		{
			sb.Append('[');
			bool flag = false;
			foreach (object obj2 in enumerable)
			{
				if (flag) { sb.Append(','); }
				SerializeValuePrivate(sb, obj2, depth, null);
				flag = true;
			}
			sb.Append(']');

		}
		private static void SerializeChar(StringBuilder sb, char value, string format)
		{
			if (value == '\0')
				sb.Append("null");
			else
				sb.Append('"').Append(HttpUtility.JavaScriptStringEncode(value.ToString())).Append('"');
		}
		private static void SerializeString(StringBuilder sb, string value, string format)
		{
			sb.Append('"');
			sb.Append(HttpUtility.JavaScriptStringEncode(value));
			sb.Append('"');
		}
		private static void SerializeDateTime(StringBuilder sb, DateTime value, string format)
		{
			if (!string.IsNullOrWhiteSpace(format))
				sb.Append('"').AppendFormat(format, value).Append('"');
			else if (value.Millisecond > 0)
				sb.AppendFormat("\"{0:yyyy-MM-dd HH:mm:ss.fff}\"", value);
			else if (value.Second > 0)
				sb.AppendFormat("\"{0:yyyy-MM-dd HH:mm:ss}\"", value);
			else if (value.Hour > 0 || value.Minute > 0)
				sb.AppendFormat("\"{0:yyyy-MM-dd HH:mm}\"", value);
			else
				sb.AppendFormat("\"{0:yyyy-MM-dd}\"", value);
		}
		private static void SerializeTimeSpan(StringBuilder sb, System.TimeSpan value, string format)
		{
			if (!string.IsNullOrWhiteSpace(format))
				sb.Append('"').AppendFormat(format, value).Append('"');
			else if (value.Milliseconds > 0)
				sb.AppendFormat("\"{0:HH:mm:ss.fff}\"", value);
			else if (value.Seconds > 0)
				sb.AppendFormat("\"{0:HH:mm:ss}\"", value);
			else
				sb.AppendFormat("\"{0:HH:mm}\"", value);
		}
		private static void SerializeBoolean(StringBuilder sb, bool value)
		{
			if (value) { sb.Append("true"); } else { sb.Append("false"); }
		}
		private static void SerializeGuid(StringBuilder sb, Guid value)
		{
			sb.Append("\"").Append(value.ToString()).Append("\"");
		}
		private static void SerializeUri(StringBuilder sb, Uri uri)
		{
			sb.Append("\"").Append(uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped)).Append("\"");
		}

		#region 类型安全检测
		private static object MethodInfoInvoke(MethodInfo method, object target, object[] args)
		{
			Type declaringType = method.DeclaringType;
			if (declaringType == null)
			{
				if (!method.IsPublic || !GenericArgumentsAreVisible(method))
				{
					DemandGrantSet(method.Module.Assembly);
				}
			}
			else if ((!declaringType.IsVisible || !method.IsPublic) || !GenericArgumentsAreVisible(method))
			{
				DemandReflectionAccess(declaringType);
			}
			return method.Invoke(target, args);
		}
		private static bool GenericArgumentsAreVisible(MethodInfo method)
		{
			if (method.IsGenericMethod)
			{
				foreach (Type type in method.GetGenericArguments())
				{
					if (!type.IsVisible)
					{
						return false;
					}
				}
			}
			return true;
		}
		private static object FieldInfoGetValue(FieldInfo field, object target)
		{
			Type declaringType = field.DeclaringType;
			if (declaringType == null)
			{
				if (!field.IsPublic)
				{
					DemandGrantSet(field.Module.Assembly);
				}
			}
			else if (((declaringType == null) || !declaringType.IsVisible) || !field.IsPublic)
			{
				DemandReflectionAccess(declaringType);
			}
			return field.GetValue(target);
		}
		[SecuritySafeCritical]
		private static void DemandGrantSet(Assembly assembly)
		{
			//PermissionSet permissionSet = assembly.PermissionSet;
			//permissionSet.AddPermission(RestrictedMemberAccessPermission);
			//permissionSet.Demand();
		}

		private static void DemandReflectionAccess(Type type)
		{
		}

		#endregion
	}
}
