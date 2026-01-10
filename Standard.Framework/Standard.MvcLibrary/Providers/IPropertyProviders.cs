using System;
using System.Linq;
using System.Linq.Expressions;
using Basic.Collections;
using Basic.EntityLayer;
using Microsoft.AspNetCore.Html;

namespace Basic.MvcLibrary
{
	/// <summary>表示实体模型属性提供者</summary>
	/// <typeparam name="T"></typeparam>
#if NET6_0_OR_GREATER
	public interface IPropertyProviders<T> : IDisposable, IAsyncDisposable
#else
	public interface IPropertyProviders<T> : IDisposable
#endif
	{
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

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, bool withComma);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="withComma">是否输出逗号。</param>
		IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name, bool withComma);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="format">表示属性值格式化字符串</param>
		/// <param name="withComma">是否输出逗号。</param>
		IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name, string format, bool withComma);
	}

	/// <summary></summary>
	/// <typeparam name="T"></typeparam>
	public class PropertyProviders<T> : IPropertyProviders<T>
	{
		private readonly EntityPropertyCollection mProperties;
		private readonly T _model;
		/// <summary>初始化 ValidationProvider 类实例</summary>
		public PropertyProviders(T model)
		{
			_model = model; EntityPropertyProvidor.TryGetProperties(typeof(T), out mProperties);
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public TR PropertyValue<TR>(Expression<Func<T, TR>> expression)
		{
			return expression.Compile().Invoke(_model);
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, bool withComma)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { return new HtmlString(FormatValue(meta, null, null, withComma)); }
			else { return new HtmlString(string.Concat(property, ":\"\"", withComma ? "," : "")); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="withComma">是否输出逗号。</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name, bool withComma)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { return new HtmlString(FormatValue(meta, name, null, withComma)); }
			else { return new HtmlString(string.Concat(name, ":\"\"", (withComma ? "," : ""))); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="format">表示属性值格式化字符串</param>
		/// <param name="withComma">是否输出逗号。</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name, string format, bool withComma)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta))
			{
				return new HtmlString(FormatValue(meta, name, format, withComma));
			}
			else { return new HtmlString(string.Concat(name, ":\"\"", withComma ? "," : "")); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { return new HtmlString(FormatValue(meta, null, null, false)); }
			else { return new HtmlString(string.Concat(property, ":\"\"")); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { return new HtmlString(FormatValue(meta, name, null, false)); }
			else { return new HtmlString(string.Concat(name, ":\"\"")); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="format">表示属性值格式化字符串</param>
		public IHtmlContent PropertyFor<TR>(Expression<Func<T, TR>> expression, string name, string format)
		{
			string property = LambdaHelper.GetMemberName(expression.Body);
			if (mProperties.TryGetProperty(property, out EntityPropertyMeta meta)) { return new HtmlString(FormatValue(meta, name, format, false)); }
			else { return new HtmlString(string.Concat(name, ":\"\"")); }
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="meta">需要输出的Lambda表达式。</param>
		/// <param name="name">lambda表达式 属性名称</param>
		/// <param name="format">表示属性值格式化字符串</param>
		/// <param name="withComma">是否输出逗号。</param>
		private string FormatValue(EntityPropertyMeta meta, string name, string format, bool withComma)
		{
			if (meta != null)
			{
				if (string.IsNullOrWhiteSpace(name) == true) { name = meta.Name; }
				object obj = null; Type propertyType = meta.PropertyType;
				if (_model != null) { obj = meta.GetValue(_model); }
				string format1 = meta.DisplayFormatString;
				if (string.IsNullOrWhiteSpace(format) == false) { format1 = format; }
				if (obj == null) { return string.Concat(name, ": null", withComma ? "," : ""); }
				else if (meta.PropertyType.IsEnum)
				{
					if (Enum.IsDefined(meta.PropertyType, obj) == false) { return (string.Concat(name, ":null", withComma ? "," : "")); }
					else if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, obj), withComma ? "," : ""); }
					else { return string.Concat(name, ":", Convert.ToInt32(obj), withComma ? "," : ""); }
				}
				else if (meta.PropertyType.IsArray && obj is Array array)
				{
					if (obj == null || array.Length == 0) { return string.Concat(name, ":[]", withComma ? "," : ""); }
					Type elementType = propertyType.GetElementType();
					if (elementType == typeof(string)) { return string.Concat(name, ":['", string.Join("','", array.Cast<string>()), "']", withComma ? "," : ""); }
					else if (elementType == typeof(Guid)) { return string.Concat(name, ":['", string.Join("','", array.Cast<Guid>()), "']", withComma ? "," : ""); }
					else if (elementType == typeof(DateTime)) { return string.Concat(name, ":['", string.Join("','", array.Cast<DateTime>()), "']", withComma ? "," : ""); }
					else if (elementType.IsEnum) { return string.Concat(name, ":[", string.Join(",", array.Cast<int>()), "]", withComma ? "," : ""); }
					else { return string.Concat(name, ":[", string.Join(",", array.OfType<object>()), "]", withComma ? "," : ""); }
				}
				else if (meta.PropertyType == typeof(Guid))
				{
					Guid value = (Guid)obj;
					if (value == Guid.Empty) { return (string.Concat(name, ":\"\"", withComma ? "," : "")); }
					else if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, value), withComma ? "," : ""); }
					else { return string.Concat(name, ":\"", value, "\"", withComma ? "," : ""); }
				}
				else if (meta.PropertyType == typeof(bool) || meta.PropertyType == typeof(bool?))
				{
					string str = System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(obj).ToLower());
					if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, str), withComma ? "," : ""); }
					else { return string.Concat(name, ":\"", str, "\"", withComma ? "," : ""); }
				}
				else if (meta.PropertyType == typeof(int) || meta.PropertyType == typeof(long) || meta.PropertyType == typeof(short))
				{
					if (int.Equals(obj, default(int))) { return string.Concat(name, ":null", withComma ? "," : ""); }
					else if (short.Equals(obj, default(short))) { return string.Concat(name, ":null", withComma ? "," : ""); }
					else if (long.Equals(obj, default(long))) { return string.Concat(name, ":null", withComma ? "," : ""); }
					else if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, obj), withComma ? "," : ""); }
					else { return string.Concat(name, ":", obj, withComma ? "," : ""); }
				}
				else if (meta.PropertyType == typeof(float) || meta.PropertyType == typeof(double) || meta.PropertyType == typeof(decimal))
				{
					if (decimal.Equals(obj, default(decimal))) { return string.Concat(name, ":null", withComma ? "," : ""); }
					else if (float.Equals(obj, default(float))) { return string.Concat(name, ":null", withComma ? "," : ""); }
					else if (double.Equals(obj, default(double))) { return string.Concat(name, ":null", withComma ? "," : ""); }
					else if (string.IsNullOrWhiteSpace(format1) == false) { return string.Concat(name, ":", string.Format(format1, obj), withComma ? "," : ""); }
					else { return string.Concat(name, ":", obj, withComma ? "," : ""); }
				}
				else if (meta.PropertyType == typeof(DateTime))
				{
					DateTime dt = Convert.ToDateTime(obj);
					if (dt == DateTime.MinValue) { return string.Concat(name, ":null", withComma ? "," : ""); }
					else if (string.IsNullOrWhiteSpace(format1) == false)
					{
						string str = string.Format(format1, obj);
						return string.Concat(name, ":\"", str, "\"", withComma ? "," : "");
					}
					else if (dt.Second == 0 && dt.Minute == 0 && dt.Hour == 0)
					{
						return (string.Format("{0}:\"{1:yyyy-MM-dd}\"{2}", name, dt, withComma ? "," : ""));
					}
					else { return string.Concat(name, ":\"", obj, "\"", withComma ? "," : ""); }
				}
				else if (string.IsNullOrWhiteSpace(format1) == false)
				{
					string str = string.Format(format1, obj);
					str = System.Web.HttpUtility.JavaScriptStringEncode(str);
					return string.Concat(name, ":\"", str, "\"", withComma ? "," : "");
				}
				else
				{
					string str = System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(obj));
					return string.Concat(name, ":\"", str, "\"", withComma ? "," : "");
				}
			}
			else { return string.Concat(name, ":\"\"", withComma ? "," : ""); }
		}

		/// <summary></summary>
		void System.IDisposable.Dispose() { }

#if NET6_0_OR_GREATER
		/// <summary></summary>
		System.Threading.Tasks.ValueTask IAsyncDisposable.DisposeAsync() { return System.Threading.Tasks.ValueTask.CompletedTask; }
#endif

	}

}
