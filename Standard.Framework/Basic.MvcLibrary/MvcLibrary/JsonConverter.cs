﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text;

using System.Collections;
using System.Web;
using Basic.Properties;
using System.Security;
using System.Security.Permissions;
using Basic.EntityLayer;
using System.Collections.Generic;
using Basic.Messages;
using System.Runtime.Serialization;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义Json序列化帮助类
	/// </summary>
	public sealed class JsonConverter
	{
		private readonly HttpRequestBase mRequest;
		/// <summary>初始化 JsonConverter 类实例</summary>
		public JsonConverter(HttpRequestBase request) { mRequest = request; }

		/// <summary>将对象转换为 JSON 字符串。</summary>
		/// <param name="value">需要序列化的DataEntity子类实例</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public string Serialize<T>(T value)
		{
			return Serialize<T>(value, true);
		}

		/// <summary>将对象转换为 JSON 字符串。</summary>
		/// <param name="value">需要序列化的DataEntity子类实例</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public string Serialize<T>(IEnumerable<T> value)
		{
			return Serialize<T>(value, true);
		}

		/// <summary>将对象转换为 JSON 字符串。</summary>
		/// <param name="value">需要序列化的DataEntity子类实例</param>
		/// <param name="includeBrace">是否需要包含Json对象的大括号</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public string Serialize<T>(T value, bool includeBrace)
		{
			StringBuilder resultBuilder = new StringBuilder(500);
			SerializeValuePrivate(resultBuilder, value, includeBrace, 0, null);
			//if (!includeBrace)
			//{
			//	resultBuilder.Replace("{", "", 0, 1);
			//	return resultBuilder.Replace("}", "", resultBuilder.Length - 1, 1).ToString();
			//}
			return resultBuilder.ToString();
		}

		/// <summary>将对象转换为 JSON 字符串。</summary>
		/// <param name="value">需要序列化的DataEntity子类实例</param>
		/// <param name="includeBrace">是否需要包含Json对象的大括号</param>
		/// <returns>DataEntity子类实例序列化的结果</returns>
		public string Serialize<T>(IEnumerable<T> value, bool includeBrace)
		{
			StringBuilder resultBuilder = new StringBuilder(500);
			SerializeValuePrivate(resultBuilder, value, includeBrace, 0, null);
			//if (!includeBrace)
			//{
			//	resultBuilder.Replace("{", "", 0, 1);
			//	return resultBuilder.Replace("}", "", resultBuilder.Length - 1, 1).ToString();
			//}
			return resultBuilder.ToString();
		}

		private void SerializeValuePrivate(StringBuilder sb, object value, bool includeBrace, int depth, string format)
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
								throw new InvalidOperationException(MvcStrings.JSON_InvalidEnumType);
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
								if (value is IEnumerable enumerable)
								{
									SerializeEnumerable(sb, enumerable, depth);
								}
								else
								{
									SerializeCustomObject(sb, value, depth, includeBrace);
								}
							}
						}
					}
				}
			}
		}
		private void SerializeCustomObject(StringBuilder sb, object value, int depth, bool includeBrace)
		{
			Type type = value.GetType();
			Type mdType = null;
			MetadataTypeAttribute mta = Attribute.GetCustomAttribute(type, typeof(MetadataTypeAttribute)) as MetadataTypeAttribute;
			if (mta != null) { mdType = mta.MetadataClassType; }
			bool flag = true;
			if (includeBrace == true) { sb.Append('{'); }
			foreach (FieldInfo info in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				if (!flag) { sb.Append(','); }
				DataMemberAttribute dma = info.GetCustomAttribute<DataMemberAttribute>();
				if (dma != null && dma.Name != null) { SerializeString(sb, dma.Name, null); }
				else { SerializeString(sb, info.Name, null); }
				sb.Append(':');
				SerializeValuePrivate(sb, FieldInfoGetValue(info, value), true, depth, null);
				flag = false;
			}
			foreach (PropertyInfo info2 in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
			{
				if (info2.Name == "EntityKey" || info2.Name == "EntityState") { continue; }
				DisplayFormatAttribute dfa = info2.GetCustomAttribute<DisplayFormatAttribute>();
				if (dfa == null && mdType != null)
				{
					PropertyInfo mdInfo = mdType.GetProperty(info2.Name, BindingFlags.Public | BindingFlags.Instance);
					if (mdInfo != null) { dfa = mdInfo.GetCustomAttribute<DisplayFormatAttribute>(); }
				}
				WebDisplayAttribute wdAttr = info2.GetCustomAttribute<WebDisplayAttribute>();

				MethodInfo getMethod = info2.GetGetMethod();
				if ((getMethod != null) && (getMethod.GetParameters().Length <= 0))
				{
					if (!flag) { sb.Append(','); }
					DataMemberAttribute dma = info2.GetCustomAttribute<DataMemberAttribute>();
					if (dma != null && dma.Name != null) { SerializeString(sb, dma.Name, null); }
					else { SerializeString(sb, info2.Name, null); }
					sb.Append(':');
					object propValue = MethodInfoInvoke(getMethod, value, null);
					if (dfa != null) { SerializeValuePrivate(sb, propValue, true, depth, dfa.DataFormatString); }
					else { SerializeValuePrivate(sb, propValue, true, depth, null); }

					if (info2.PropertyType == typeof(bool) && wdAttr != null)
					{
						sb.Append(',');
						SerializeString(sb, info2.Name + "Text", null);
						sb.Append(':');
						SerializeBoolean(sb, wdAttr, (bool)propValue);
					}
					else if (info2.PropertyType == typeof(Nullable<bool>) && propValue != null && wdAttr != null)
					{
						sb.Append(',');
						SerializeString(sb, info2.Name + "Text", null);
						sb.Append(':');
						SerializeBoolean(sb, wdAttr, (bool)propValue);
					}
					else if (info2.PropertyType.IsEnum && wdAttr != null)
					{
						sb.Append(',');
						SerializeString(sb, info2.Name + "Text", null);
						sb.Append(':');
						string eValue = Enum.Format(info2.PropertyType, propValue, "F");
						SerializeEnumValue(sb, info2, wdAttr, eValue);
					}
					else if (info2.PropertyType == typeof(Nullable<>) && info2.PropertyType.DeclaringType.IsEnum && propValue != null && wdAttr != null)
					{
						sb.Append(',');
						SerializeString(sb, info2.Name + "Text", null);
						sb.Append(':');
						string eValue = Enum.Format(info2.PropertyType, propValue, "F");
						SerializeEnumValue(sb, info2, wdAttr, eValue);
					}
					flag = false;
				}
			}
			if (includeBrace == true) { sb.Append('}'); }
		}
		private void SerializeDictionary(StringBuilder sb, IDictionary value, int depth)
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
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, MvcStrings.JSON_DictionaryTypeNotSupported, new object[] { value.GetType().FullName }));
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
		private void SerializeDictionaryKeyValue(StringBuilder sb, string key, object value, int depth)
		{
			SerializeString(sb, key, null);
			sb.Append(':');
			SerializeValuePrivate(sb, value, true, depth, null);
		}
		private void SerializeEnumerable(StringBuilder sb, IEnumerable enumerable, int depth)
		{
			sb.Append('[');
			bool flag = false;
			foreach (object obj2 in enumerable)
			{
				if (flag) { sb.Append(','); }
				SerializeValuePrivate(sb, obj2, true, depth, null);
				flag = true;
			}
			sb.Append(']');

		}
		private void SerializeChar(StringBuilder sb, char value, string format)
		{
			if (value == '\0')
				sb.Append("null");
			else
				sb.Append('"').Append(HttpUtility.JavaScriptStringEncode(value.ToString())).Append('"');
		}
		private void SerializeString(StringBuilder sb, string value, string format)
		{
			sb.Append('"');
			sb.Append(HttpUtility.JavaScriptStringEncode(value));
			sb.Append('"');
		}
		private void SerializeDateTime(StringBuilder sb, DateTime value, string format)
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
		private void SerializeTimeSpan(StringBuilder sb, System.TimeSpan value, string format)
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
		private void SerializeEnumValue(StringBuilder sb, PropertyInfo info2, WebDisplayAttribute wdAttr, string value)
		{
			Type eType = info2.PropertyType; string converterName = null;
			WebDisplayConverterAttribute wdca = eType.GetCustomAttribute<WebDisplayConverterAttribute>();
			if (wdca != null) { converterName = wdca.ConverterName; }

			string enumName = info2.PropertyType.Name;
			if (value.IndexOf(",") >= 0)
			{
				List<string> names = new List<string>(10);
				foreach (string item in value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					string itemName = string.Concat(enumName, "_", item.Trim());
					names.Add(mRequest.GetString(converterName, itemName));
				}
				SerializeString(sb, string.Join(", ", names.ToArray()), null);
			}
			else
			{
				string itemName = string.Concat(enumName, "_", value);
				SerializeString(sb, mRequest.GetString(converterName, itemName), null);
			}
		}
		private void SerializeBoolean(StringBuilder sb, WebDisplayAttribute wdAttr, bool value)
		{
			string source = string.Concat(wdAttr.DisplayName, value ? "_TrueText" : "_FalseText");
			SerializeString(sb, mRequest.GetString(wdAttr.ConverterName, source), null);
		}
		private void SerializeBoolean(StringBuilder sb, bool value)
		{
			if (value) { sb.Append("true"); } else { sb.Append("false"); }
		}
		private void SerializeGuid(StringBuilder sb, Guid value)
		{
			sb.Append("\"").Append(value.ToString()).Append("\"");
		}
		private void SerializeUri(StringBuilder sb, Uri uri)
		{
			sb.Append("\"").Append(uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped)).Append("\"");
		}

		#region 类型安全检测
		private object MethodInfoInvoke(MethodInfo method, object target, object[] args)
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
		private bool GenericArgumentsAreVisible(MethodInfo method)
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
		private object FieldInfoGetValue(FieldInfo field, object target)
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
		private void DemandGrantSet(Assembly assembly)
		{
			PermissionSet permissionSet = assembly.PermissionSet;
			permissionSet.AddPermission(RestrictedMemberAccessPermission);
			permissionSet.Demand();
		}
		private ReflectionPermission restrictedMemberAccessPermission;
		private ReflectionPermission RestrictedMemberAccessPermission
		{
			get
			{
				if (restrictedMemberAccessPermission == null)
				{
					restrictedMemberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
				}
				return restrictedMemberAccessPermission;
			}
		}
		private void DemandReflectionAccess(Type type)
		{
			try
			{
				MemberAccessPermission.Demand();
			}
			catch (SecurityException)
			{
				DemandGrantSet(type.Assembly);
			}
		}
		private ReflectionPermission memberAccessPermission;
		private ReflectionPermission MemberAccessPermission
		{
			get
			{
				if (memberAccessPermission == null)
				{
					memberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
				}
				return memberAccessPermission;
			}
		}
		#endregion
	}
}
