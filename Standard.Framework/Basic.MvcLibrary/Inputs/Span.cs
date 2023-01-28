using System;
using System.Reflection;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示标签</summary>
	public class Span : Options, System.IDisposable
	{
		private readonly IBasicContext _basic;
		private readonly EntityPropertyMeta _meta;
		/// <summary></summary>
		protected readonly TagHtmlWriter tagBuilder;
		/// <summary>初始化 Label 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal protected Span(IBasicContext basic, EntityPropertyMeta meta)
		{
			_meta = meta; _basic = basic; tagBuilder = new TagHtmlWriter("span");
		}

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public Span Value(string text) { tagBuilder.SetInnerText(text); return this; }

		private string formatString = null;
		/// <summary>格式化内容字符串</summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public Span Format(string format) { formatString = format; return this; }

		/// <summary></summary>
		/// <param name="value"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		private Span FormatValue(object value, string format)
		{
			if (string.IsNullOrWhiteSpace(format)) { return Value(Convert.ToString(value)); }
			else { return Value(string.Format(format, value)); }
		}

		/// <summary>输入框关联的label文字</summary>
		/// <returns>返回当前对象。</returns>
		public Span Value()
		{
			object obj = null; System.Type propertyType = _meta.PropertyType;
			if (_basic.Model != null) { obj = _meta.GetValue(_basic.Model); }
			if (obj == null) { return this; }
			else if (_meta.PropertyType.IsEnum)
			{
				if (Enum.IsDefined(_meta.PropertyType, obj) == false) { return this; }
				string converterName = null;
				if (Attribute.IsDefined(_meta.PropertyType, typeof(WebDisplayConverterAttribute)))
				{
					WebDisplayConverterAttribute wdca = _meta.PropertyType.GetCustomAttribute<WebDisplayConverterAttribute>();
					converterName = wdca.ConverterName;
				}
				string enumName = _meta.PropertyType.Name;
				string name = Enum.GetName(_meta.PropertyType, obj);
				string itemName = string.Concat(enumName, "_", name);
				string text = _basic.GetString(converterName, itemName);
				if (text == itemName) { return FormatValue(name, formatString); }
				return FormatValue(text, formatString);
			}
			else if ((_meta.PropertyType == typeof(bool) || _meta.PropertyType == typeof(bool?)) && _meta.Display != null)
			{
				bool check = (bool)obj; string text;
				WebDisplayAttribute wda = _meta.Display;
				string converterName = wda.ConverterName;

				if (check)
				{
					if (string.IsNullOrWhiteSpace(converterName))
						text = _basic.GetString(string.Concat(wda.DisplayName, "_TrueText"));
					else
						text = _basic.GetString(converterName, string.Concat(wda.DisplayName, "_TrueText"));
				}
				else
				{
					if (string.IsNullOrWhiteSpace(converterName))
						text = _basic.GetString(string.Concat(wda.DisplayName, "_FalseText"));
					else
						text = _basic.GetString(converterName, string.Concat(wda.DisplayName, "_FalseText"));
				}
				return FormatValue(text, formatString);
			}
			else if (_meta.PropertyType.IsArray)
			{
				if (obj is System.Collections.IEnumerable array)
				{
					Type elementType = propertyType.GetElementType();
					if (elementType == typeof(string))
					{
						return FormatValue(string.Join("','", array), formatString);
					}
					if (elementType.IsEnum)
					{
						return FormatValue(string.Join("','", array), formatString);
					}
					else { return FormatValue(string.Join("','", array), formatString); }
				}
				else { return FormatValue(obj, formatString); }
			}
			else if (_meta.PropertyType == typeof(Guid))
			{
				return FormatValue(obj, formatString);
			}
			else if (_meta.PropertyType == typeof(int) || _meta.PropertyType == typeof(long) || _meta.PropertyType == typeof(short))
			{
				return FormatValue(obj, formatString);
			}
			else if (_meta.PropertyType == typeof(DateTime))
			{
				DateTime dt = Convert.ToDateTime(obj);
				if (dt == DateTime.MinValue) { return this; }
				else if (string.IsNullOrWhiteSpace(_meta.DisplayFormatString) == false)
				{
					return Value(string.Format(_meta.DisplayFormatString, obj));
				}
				else if (dt.Second == 0 && dt.Minute == 0 && dt.Hour == 0)
				{
					return Value(string.Format("{0:yyyy-MM-dd}", obj));
				}
				else { return FormatValue(obj, formatString); }
			}
			else if (string.IsNullOrWhiteSpace(formatString) == false)
			{
				return FormatValue(obj, formatString);
			}
			else if (string.IsNullOrWhiteSpace(_meta.DisplayFormatString) == false)
			{
				return FormatValue(obj, _meta.DisplayFormatString);
			}
			else
			{
				return FormatValue(obj, formatString);
			}
		}

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public Span Html(string html) { tagBuilder.InnerHtml = html; return this; }

		/// <summary>释放非托管资源</summary>
		protected override void Dispose()
		{
			tagBuilder.RenderEndTag(_basic.Writer);
		}

		/// <summary>显示输入标签的字符串表示形式(HTML)</summary>
		/// <returns>返回按钮的 Html 字符串</returns>
		public override string ToString()
		{
			tagBuilder.MergeOptions(this);
			return tagBuilder.ToString();
		}
	}
}
