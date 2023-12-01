using System;
using System.Linq;
using System.Linq.Expressions;
using Basic.Collections;
using Basic.EntityLayer;
using Microsoft.AspNetCore.Html;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public static class PropertyProvider
	{
		/// <summary>初始化 Toolbar 类实例</summary>
		public static IPropertyProvider<T> PropertyInfo<T>(this IBasicContext<T> basic)
		{
			return new PropertyProvider<T>(basic);
		}
	}

	/// <summary>表示实体模型属性提供者</summary>
	/// <typeparam name="T"></typeparam>
	public interface IPropertyProvider<T> : IDisposable
	{
		/// <summary>向当前视图文件输出字符串</summary>
		/// <param name="value">需要输出的字符串</param>
		void WriteString(string value);

		/// <summary>表示上下文信息</summary>
		IBasicContext Basic { get; }

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		TR PropertyValue<TR>(Expression<Func<T, TR>> expression);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="format">表示属性值格式化字符串</param>
		IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name, string format);

		/// <summary>向当前视图文件输出字符串</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		void Property<TR>(Expression<Func<T, TR>> expression);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		void Property<TR>(Expression<Func<T, TR>> expression, bool withComma);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		void Property<TR>(Expression<Func<T, TR>> expression, string name);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="withComma">是否输出逗号。</param>
		void Property<TR>(Expression<Func<T, TR>> expression, string name, bool withComma);

		/// <summary>向当前视图文件输出所有属性</summary>
		void Properties();
	}

	/// <summary></summary>
	/// <typeparam name="T"></typeparam>
	public sealed class PropertyProvider<T> : IPropertyProvider<T>
	{
		private readonly EntityPropertyCollection mProperties;
		private readonly IBasicContext basicContext;
		/// <summary>初始化 ValidationProvider 类实例</summary>
		internal PropertyProvider(IBasicContext bc)
		{
			basicContext = bc; EntityPropertyProvidor.TryGetProperties(typeof(T), out mProperties);
		}

		/// <summary>表示上下文信息</summary>
		public IBasicContext Basic { get { return basicContext; } }

		/// <summary>向当前视图文件输出字符串</summary>
		/// <param name="value">需要输出的字符串</param>
		public void WriteString(string value) { basicContext.Write(value); }

		/// <summary>向当前视图文件输出字符串</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public void Property<TR>(Expression<Func<T, TR>> expression) { Property(expression, false); }

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		public void Property<TR>(Expression<Func<T, TR>> expression, bool withComma)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression != null ? memberExpression.Member.Name : null;
			if (mProperties.TryGetProperty(name, out EntityPropertyMeta meta)) { Property(meta, name, withComma); }
			else { basicContext.WriteLine(string.Concat(name, ":\"\"", withComma == true ? "," : "")); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		public void Property<TR>(Expression<Func<T, TR>> expression, string name)
		{
			Property(expression, name, false);
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="withComma">是否输出逗号。</param>
		public void Property<TR>(Expression<Func<T, TR>> expression, string name, bool withComma)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { Property(meta, name, withComma); }
			else { basicContext.WriteLine(string.Concat(name, ":\"\"", withComma == true ? "," : "")); }
		}

		/// <summary>向当前视图文件输出所有属性</summary>
		public void Properties()
		{
			int count = mProperties.Count; int index = 1;
			foreach (EntityPropertyMeta meta in mProperties)
			{
				if (meta.IsReadOnly) { continue; }
				else if (meta.Ignore) { continue; }
				else if (basicContext.Model != null)
				{
					Property(meta, meta.Name, false);
					if (index != count) { basicContext.WriteLine(","); index++; }
					else { basicContext.WriteLine(); }
				}
				else
				{
					basicContext.Write(string.Concat(meta.Name, ":\"\""));
					if (index != count) { basicContext.WriteLine(","); index++; }
					else { basicContext.WriteLine(); }
				}
			}
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public TR PropertyValue<TR>(Expression<Func<T, TR>> expression)
		{
			return expression.Compile().Invoke((T)basicContext.Model);
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { return new HtmlString(FormatValue(meta, null, null)); }
			else { return new HtmlString(string.Concat(property, ":\"\"")); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { return new HtmlString(FormatValue(meta, name, null)); }
			else { return new HtmlString(string.Concat(name, ":\"\"")); }
		}


		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="format">表示属性值格式化字符串</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name, string format)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { return new HtmlString(FormatValue(meta, name, format)); }
			else { return new HtmlString(string.Concat(name, ":\"\"")); }

		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="meta">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="format">表示属性值格式化字符串</param>
		private string FormatValue(EntityPropertyMeta meta, string name, string format)
		{
			if (meta != null)
			{
				if (string.IsNullOrWhiteSpace(name) == true) { name = meta.Name; }
				object obj = null; Type propertyType = meta.PropertyType;
				if (basicContext.Model != null) { obj = meta.GetValue(basicContext.Model); }
				string format1 = meta.DisplayFormatString;
				if (string.IsNullOrWhiteSpace(format) == false) { format1 = format; }
				if (obj == null) { return string.Concat(name, ":null"); ; }
				else if (meta.PropertyType.IsEnum)
				{
					if (Enum.IsDefined(meta.PropertyType, obj) == false) { return (string.Concat(name, ":null")); }
					else if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, obj)); }
					else { return string.Concat(name, ":", Convert.ToInt32(obj), ""); }
				}
				else if (meta.PropertyType.IsArray && obj is Array array)
				{
					if (obj == null || array.Length == 0) { return string.Concat(name, ":[]"); }
					Type elementType = propertyType.GetElementType();
					if (elementType == typeof(string)) { return string.Concat(name, ":['", string.Join("','", array.Cast<string>()), "']"); }
					else if (elementType == typeof(Guid)) { return string.Concat(name, ":['", string.Join("','", array.Cast<Guid>()), "']"); }
					else if (elementType == typeof(DateTime)) { return string.Concat(name, ":['", string.Join("','", array.Cast<DateTime>()), "']"); }
					else if (elementType.IsEnum) { return string.Concat(name, ":[", string.Join(",", array.Cast<int>()), "]"); }
					else { return string.Concat(name, ":[", string.Join(",", array.OfType<object>()), "]"); }
				}
				else if (meta.PropertyType == typeof(Guid))
				{
					Guid value = (Guid)obj;
					if (value == Guid.Empty) { return (string.Concat(name, ":\"\"")); }
					else if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, value)); }
					else { return string.Concat(name, ":\"", value, "\""); }
				}
				else if (meta.PropertyType == typeof(bool))
				{
					string str = System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(obj).ToLower());
					if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, obj)); }
					else { return string.Concat(name, ":\"", str, "\""); }
				}
				else if (meta.PropertyType == typeof(int) || meta.PropertyType == typeof(long) || meta.PropertyType == typeof(short))
				{
					if (int.Equals(obj, default(int))) { return string.Concat(name, ":null"); }
					else if (short.Equals(obj, default(short))) { return string.Concat(name, ":null"); }
					else if (long.Equals(obj, default(long))) { return string.Concat(name, ":null"); }
					else if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, obj)); }
					else { return string.Concat(name, ":", obj); }
				}
				else if (meta.PropertyType == typeof(float) || meta.PropertyType == typeof(double) || meta.PropertyType == typeof(decimal))
				{
					if (decimal.Equals(obj, default(decimal))) { return string.Concat(name, ":null"); }
					else if (float.Equals(obj, default(float))) { return string.Concat(name, ":null"); }
					else if (double.Equals(obj, default(double))) { return string.Concat(name, ":null"); }
					else if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, obj)); }
					else { return string.Concat(name, ":", obj); }
				}
				else if (meta.PropertyType == typeof(DateTime))
				{
					DateTime dt = Convert.ToDateTime(obj);
					if (dt == DateTime.MinValue) { return string.Concat(name, ":null"); }
					else if (string.IsNullOrWhiteSpace(format1) == false)
					{
						string str = string.Format(format1, obj);
						return string.Concat(name, ":\"", str, "\"");
					}
					else if (dt.Second == 0 && dt.Minute == 0 && dt.Hour == 0)
					{
						return (string.Format("{0}:\"{1:yyyy-MM-dd}\"", name, dt));
					}
					else { return string.Concat(name, ":\"", obj, "\""); }
				}
				else if (string.IsNullOrWhiteSpace(format1) == false)
				{
					string str = string.Format(format1, obj);
					str = System.Web.HttpUtility.JavaScriptStringEncode(str);
					return string.Concat(name, ":\"", str, "\"");
				}
				else
				{
					string str = System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(obj));
					return string.Concat(name, ":\"", str, "\"");
				}
			}
			else { return string.Concat(name, ":\"\""); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="meta">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="withComma">是否输出逗号。</param>
		private void Property(EntityPropertyMeta meta, string name, bool withComma)
		{
			if (meta != null)
			{
				if (string.IsNullOrWhiteSpace(name) == true) { name = meta.Name; }
				object obj = null; Type propertyType = meta.PropertyType;
				if (basicContext.Model != null) { obj = meta.GetValue(basicContext.Model); }

				if (obj == null) { basicContext.WriteLine(string.Concat(name, ":\"\"", withComma == true ? "," : "")); }
				else if (meta.PropertyType.IsEnum)
				{
					if (Enum.IsDefined(meta.PropertyType, obj) == false) { basicContext.WriteLine(string.Concat(name, ":null", withComma == true ? "," : "")); }
					else { basicContext.WriteLine(string.Concat(name, ":", Convert.ToInt32(obj), withComma == true ? "," : "")); }
				}
				else if (meta.PropertyType.IsArray)
				{
					if (obj == null) { basicContext.WriteLine(string.Concat(name, ":[]", withComma == true ? "," : "")); }
					else if (obj is Array array)
					{
						Type elementType = propertyType.GetElementType();
						if (elementType == typeof(string))
						{
							basicContext.WriteLine(string.Concat(name, ":['", string.Join("','", array), "']", withComma == true ? "," : ""));
						}
						if (elementType.IsEnum)
						{
							basicContext.WriteLine(string.Concat(name, ":[", string.Join(",", array.Cast<int>()), "]", withComma == true ? "," : ""));
						}
						else { basicContext.WriteLine(string.Concat(name, ":[", string.Join(",", array), "]", withComma == true ? "," : "")); }
					}
					else { basicContext.WriteLine(string.Concat(name, ":\"", obj, "\"", withComma == true ? "," : "")); }
				}
				else if (meta.PropertyType == typeof(Guid))
				{
					Guid value = (Guid)obj;
					if (value == Guid.Empty) { basicContext.WriteLine(string.Concat(name, ":null", withComma == true ? "," : "")); }
					else { basicContext.WriteLine(string.Concat(name, ":\"", value, "\"", withComma == true ? "," : "")); }
				}
				else if (meta.PropertyType == typeof(bool))
				{
					string str = System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(obj).ToLower());
					basicContext.WriteLine(string.Concat(name, ":\"", str, "\"", withComma == true ? "," : ""));
				}
				else if (meta.PropertyType == typeof(int) || meta.PropertyType == typeof(long) || meta.PropertyType == typeof(short))
				{
					if (int.Equals(obj, default(int))) { basicContext.WriteLine(string.Concat(name, ":null", withComma == true ? "," : "")); }
					else if (short.Equals(obj, default(short))) { basicContext.WriteLine(string.Concat(name, ":null", withComma == true ? "," : "")); }
					else if (long.Equals(obj, default(long))) { basicContext.WriteLine(string.Concat(name, ":null", withComma == true ? "," : "")); }
					else { basicContext.WriteLine(string.Concat(name, ":\"", Convert.ToInt32(obj), "\"", withComma == true ? "," : "")); }
				}
				else if (meta.PropertyType == typeof(DateTime))
				{
					DateTime dt = Convert.ToDateTime(obj);
					if (dt == DateTime.MinValue) { basicContext.WriteLine(string.Concat(name, ":\"\"", withComma == true ? "," : "")); }
					else if (string.IsNullOrWhiteSpace(meta.DisplayFormatString) == false)
					{
						string str = string.Format(meta.DisplayFormatString, obj);
						basicContext.WriteLine(string.Concat(name, ":\"", str, "\"", withComma == true ? "," : ""));
					}
					else if (dt.Second == 0 && dt.Minute == 0 && dt.Hour == 0)
					{
						basicContext.WriteLine(string.Format("{0}:\"{1:yyyy-MM-dd}\"{2}", name, dt, withComma == true ? "," : ""));
					}
					else { basicContext.WriteLine(string.Concat(name, ":\"", obj, "\"", withComma == true ? "," : "")); }
				}
				else if (string.IsNullOrWhiteSpace(meta.DisplayFormatString) == false)
				{
					string str = string.Format(meta.DisplayFormatString, obj);
					str = System.Web.HttpUtility.JavaScriptStringEncode(str);
					basicContext.WriteLine(string.Concat(name, ":\"", str, "\"", withComma == true ? "," : ""));
				}
				else
				{
					string str = System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(obj));
					basicContext.WriteLine(string.Concat(name, ":\"", str, "\"", withComma == true ? "," : ""));
				}
			}
			else { basicContext.WriteLine(string.Concat(name, ":\"\"", withComma == true ? "," : "")); }
		}

		/// <summary></summary>
		void System.IDisposable.Dispose() { }
	}

}
