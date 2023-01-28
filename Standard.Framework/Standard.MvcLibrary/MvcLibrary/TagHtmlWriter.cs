using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Basic.MvcLibrary
{
	/// <summary>Html标签输出</summary>
	public static class HtmlWriterRender
	{
		/// <summary></summary>
		/// <param name="expression"></param>
		public static void SetPropertyCreator(Expression<Func<string, string>> expression)
		{
			PropertyCreator = expression.Compile();
		}

		internal static Func<string, string> PropertyCreator { get; private set; } = new Func<string, string>((key) =>
		 {
			 return string.Concat(":", key);
		 });

		/// <summary></summary>
		/// <param name="expression"></param>
		public static void SetEventCreator(Expression<Func<string, string>> expression)
		{
			EventCreator = expression.Compile();
		}

		internal static Func<string, string> EventCreator { get; private set; } = new Func<string, string>((key) =>
		{
			return string.Concat("@", key);
		});
	}

	/// <summary>HTML标签输出类</summary>
	internal class TagHtmlWriter : IDisposable
	{
		// Fields
		private string _idAttributeDotReplacement;
		/// <summary>
		/// 使用tagName初始化 TagHtmlWriter 类实例。
		/// </summary>
		/// <param name="tagName"></param>
		internal TagHtmlWriter(string tagName)
		{
			SetTagName(tagName);
			Attributes = new SortedDictionary<string, string>(StringComparer.Ordinal);
			Styles = new SortedDictionary<string, string>(StringComparer.Ordinal);
		}

		/// <summary></summary>
		void IDisposable.Dispose() { }

		/// <summary></summary>
		public void AddCssClass(string value)
		{
			if (Attributes.TryGetValue("class", out string str))
			{
				Attributes["class"] = value + " " + str;
			}
			else
			{
				Attributes["class"] = value;
			}
		}

		private void AppendStyles(StringBuilder sb)
		{
			if (Styles.Count == 0) { return; }
			sb.Append(" style=\"");
			foreach (KeyValuePair<string, string> pair in Styles)
			{
				string key = pair.Key;
				string str2 = HttpUtility.HtmlAttributeEncode(pair.Value);
				sb.Append(key).Append(":").Append(str2).Append(";");
			}
			sb.Append("\"");
		}

		private void AppendAttributes(StringBuilder sb)
		{
			foreach (KeyValuePair<string, string> pair in Attributes)
			{
				string key = pair.Key;
				if (string.IsNullOrWhiteSpace(key) == true) { continue; }

				if (string.Equals(key, "id", StringComparison.Ordinal) == true && string.IsNullOrEmpty(pair.Value) == false)
				{
					string str2 = HttpUtility.HtmlAttributeEncode(pair.Value);
					sb.Append(' ').Append(key).Append("=\"").Append(str2).Append("\"");
				}
				else if (string.IsNullOrEmpty(pair.Value) == false)
				{
					string str2 = HttpUtility.HtmlAttributeEncode(pair.Value);
					sb.Append(' ').Append(key).Append("=\"").Append(str2).Append("\"");
				}
				else { sb.Append(' ').Append(key); }
			}
		}

		/// <summary></summary>
		public static string CreateSanitizedId(string originalId)
		{
			return CreateSanitizedId(originalId, "_");
		}

		/// <summary></summary>
		public static string CreateSanitizedId(string originalId, string invalidCharReplacement)
		{
			if (string.IsNullOrEmpty(originalId))
			{
				return null;
			}
			if (invalidCharReplacement == null)
			{
				throw new ArgumentNullException("invalidCharReplacement");
			}
			char c = originalId[0];
			if (!Html401IdUtil.IsLetter(c))
			{
				return null;
			}
			StringBuilder builder = new StringBuilder(originalId.Length);
			builder.Append(c);
			for (int i = 1; i < originalId.Length; i++)
			{
				char ch2 = originalId[i];
				if (Html401IdUtil.IsValidIdCharacter(ch2))
				{
					builder.Append(ch2);
				}
				else
				{
					builder.Append(invalidCharReplacement);
				}
			}
			return builder.ToString();
		}

		/// <summary></summary>
		public void GenerateId(string name)
		{
			if (!Attributes.ContainsKey("id"))
			{
				string str = CreateSanitizedId(name, IdAttributeDotReplacement);
				if (!string.IsNullOrEmpty(str))
				{
					Attributes["id"] = Attributes["ref"] = str;
				}
			}
		}

		/// <summary></summary>
		public void MergeOptions(IOptions opts)
		{
			if (opts.HasCssClass) { AddCssClass(opts.CssClass); }
			MergeAttributes(opts.Attributes);
			MergeProperties(opts.Properties);
			MergeEvents(opts.Events);
			MergeStyles(opts.Styles);
		}

		/// <summary>添加新样式属性或替换标记中的现有样式属性</summary>
		public void AddStyleAttribute(string key, string value)
		{
			if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
			if (Styles.ContainsKey(key)) { Styles[key] = value; }
			else { Styles.Add(key, value); }
		}

		/// <summary>添加新样式属性或替换标记中的现有样式属性</summary>
		public void MergeStyle(string key, string value)
		{
			if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
			if (Styles.ContainsKey(key)) { Styles[key] = value; }
			else { Styles.Add(key, value); }
		}

		/// <summary>添加新样式属性或替换标记中的现有样式属性</summary>
		public void MergeStyles<TKey, TValue>(Dictionary<TKey, TValue> attributes)
		{
			if (attributes != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in attributes)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeStyle(key, str2);
				}
			}
		}

		/// <summary>添加新样式属性或替换标记中的现有样式属性</summary>
		public void MergeStyles<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> attributes)
		{
			if (attributes != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in attributes)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeStyle(key, str2);
				}
			}
		}

		/// <summary>添加新样式属性或替换标记中的现有样式属性</summary>
		public void MergeStyles<TKey, TValue>(IDictionary<TKey, TValue> attributes)
		{
			if (attributes != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in attributes)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeStyle(key, str2);
				}
			}
		}

		/// <summary>添加新组件属性或替换标记中的现有组件属性</summary>
		public void MergeEvent(string key, string value)
		{
			if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
			key = HtmlWriterRender.EventCreator.Invoke(key);
			if (Attributes.ContainsKey(key)) { Attributes[key] = value; }
			else { Attributes.Add(key, value); }
		}

		/// <summary>添加新组件属性或替换标记中的现有组件属性</summary>
		public void MergeEvents<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> props)
		{
			if (props != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in props)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeEvent(key, str2);
				}
			}
		}

		/// <summary>添加新组件属性或替换标记中的现有组件属性</summary>
		public void MergeEvents<TKey, TValue>(IDictionary<TKey, TValue> props)
		{
			if (props != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in props)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeEvent(key, str2);
				}
			}
		}

		/// <summary>添加新组件属性或替换标记中的现有组件属性</summary>
		public void MergeProperty(string key, string value)
		{
			if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
			key = HtmlWriterRender.PropertyCreator.Invoke(key);
			if (Attributes.ContainsKey(key)) { Attributes[key] = value; }
			else { Attributes.Add(key, value); }
		}

		/// <summary>添加新组件属性或替换标记中的现有组件属性</summary>
		public void MergeProperties<TKey, TValue>(Dictionary<TKey, TValue> props)
		{
			if (props != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in props)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeProperty(key, str2);
				}
			}
		}

		/// <summary>添加新组件属性或替换标记中的现有组件属性</summary>
		public void MergeProperties<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> props)
		{
			if (props != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in props)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeProperty(key, str2);
				}
			}
		}

		/// <summary>添加新组件属性或替换标记中的现有组件属性</summary>
		public void MergeProperties<TKey, TValue>(IDictionary<TKey, TValue> props)
		{
			if (props != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in props)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeProperty(key, str2);
				}
			}
		}

		/// <summary>添加新属性或替换标记中的现有属性</summary>
		public void MergeAttribute(string key, object value)
		{
			if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
			if (Attributes.ContainsKey(key)) { Attributes[key] = Convert.ToString(value); }
			else { Attributes.Add(key, Convert.ToString(value)); }
		}

		/// <summary>添加新属性或替换标记中的现有属性</summary>
		public void AddAttribute(string key, string value)
		{
			if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
			if (Attributes.ContainsKey(key)) { Attributes[key] = value; }
			else { Attributes.Add(key, value); }
		}

		/// <summary>添加新属性或替换标记中的现有属性</summary>
		public void MergeAttribute(string key, string value)
		{
			if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
			if (Attributes.ContainsKey(key)) { Attributes[key] = value; }
			else { Attributes.Add(key, value); }
		}

		/// <summary>添加新属性或替换标记中的现有属性</summary>
		public void MergeAttributes<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> attributes)
		{
			if (attributes != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in attributes)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeAttribute(key, str2);
				}
			}
		}

		/// <summary>添加新属性或替换标记中的现有属性</summary>
		public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes)
		{
			if (attributes != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in attributes)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeAttribute(key, str2);
				}
			}
		}

		/// <summary>添加新组件属性或替换标记中的现有组件属性</summary>
		public void MergeAttributes<TKey, TValue>(Dictionary<TKey, TValue> attributes)
		{
			if (attributes != null)
			{
				foreach (KeyValuePair<TKey, TValue> pair in attributes)
				{
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					MergeAttribute(key, str2);
				}
			}
		}

		/// <summary></summary>
		public TagHtmlWriter Write(object value)
		{
			InnerHtml = HttpUtility.HtmlEncode(value); return this;
		}

		/// <summary></summary>
		public TagHtmlWriter Write(string innerText)
		{
			InnerHtml = HttpUtility.HtmlEncode(innerText); return this;
		}

		/// <summary></summary>
		public TagHtmlWriter SetInnerText(object value)
		{
			InnerHtml = HttpUtility.HtmlEncode(value); return this;
		}

		/// <summary></summary>
		public TagHtmlWriter SetInnerText(string innerText)
		{
			InnerHtml = HttpUtility.HtmlEncode(innerText); return this;
		}

		/// <summary>输出开始标记，包含属性</summary>
		public void RenderBeginTag(TextWriter writer)
		{
			writer.WriteLine(ToString(TagRenderMode.StartTag));
		}

		/// <summary>输出标签结束标记</summary>
		public void RenderEndTag(TextWriter writer)
		{
			writer.WriteLine(ToString(TagRenderMode.EndTag));
		}

		/// <summary></summary>
		public void Render(TextWriter writer)
		{
			writer.WriteLine(ToString());
		}

		/// <summary></summary>
		public void RenderContent(TextWriter writer)
		{
			writer.WriteLine(InnerHtml);
		}

		/// <summary></summary>
		public void Render(TextWriter writer, TagRenderMode mode)
		{
			writer.WriteLine(ToString(mode));
		}

		/// <summary></summary>
		public override string ToString()
		{
			return ToString(TagRenderMode.Normal);
		}

		internal string ToString(TagRenderMode renderMode)
		{
			StringBuilder sb = new StringBuilder();
			switch (renderMode)
			{
				case TagRenderMode.StartTag:
					sb.Append('<').Append(TagName);
					AppendAttributes(sb);
					AppendStyles(sb);
					sb.Append('>');
					break;

				case TagRenderMode.EndTag:
					sb.Append("</").Append(TagName).Append('>');
					break;

				case TagRenderMode.SelfClosing:
					sb.Append('<').Append(TagName);
					AppendAttributes(sb);
					AppendStyles(sb);
					sb.Append(" />");
					break;

				default:
					sb.Append('<').Append(TagName);
					AppendAttributes(sb);
					AppendStyles(sb);
					sb.Append('>').Append(InnerHtml).Append("</").Append(TagName).Append('>');
					break;
			}
			return sb.ToString();
		}

		/// <summary></summary>
		public IDictionary<string, string> Styles { get; private set; }

		/// <summary></summary>
		public IDictionary<string, string> Attributes { get; private set; }

		/// <summary></summary>
		public string IdAttributeDotReplacement
		{
			get
			{
				if (string.IsNullOrEmpty(_idAttributeDotReplacement))
				{
					_idAttributeDotReplacement = "_";
				}
				return _idAttributeDotReplacement;
			}
			set
			{
				_idAttributeDotReplacement = value;
			}
		}

		/// <summary></summary>
		public string InnerHtml { get; set; }

		/// <summary></summary>
		public string TagName { get; private set; }

		/// <summary>设置 Html 标签名称</summary>
		/// <param name="tagName">当前 TagHtmlWriter 实例输出的标签名称</param>
		internal void SetTagName(string tagName)
		{
			if (string.IsNullOrEmpty(tagName))
			{
				throw new ArgumentNullException(nameof(tagName));
			}
			TagName = tagName;
		}

		// Nested Types
		private static class Html401IdUtil
		{
			// Methods
			private static bool IsAllowableSpecialCharacter(char c)
			{
				if (((c != '-') && (c != ':')) && (c != '_'))
				{
					return false;
				}
				return true;
			}

			/// <summary></summary>
			private static bool IsDigit(char c)
			{
				return (('0' <= c) && (c <= '9'));
			}

			/// <summary></summary>
			public static bool IsLetter(char c)
			{
				return ((('A' <= c) && (c <= 'Z')) || (('a' <= c) && (c <= 'z')));
			}

			/// <summary></summary>
			public static bool IsValidIdCharacter(char c)
			{
				if (!IsLetter(c) && !IsDigit(c))
				{
					return IsAllowableSpecialCharacter(c);
				}
				return true;
			}
		}
	}

	//
	// 摘要:
	//     .
	/// <summary>Enumerates the modes that are available for rendering HTML tags</summary>
	internal enum TagRenderMode
	{
		/// <summary>
		/// Represents the mode for rendering normal text
		/// </summary>
		Normal = 0,
		/// <summary>
		/// <![CDATA[Represents the mode for rendering an opening tag (for example, <tag>).]]>
		/// </summary>
		StartTag,

		/// <summary>
		/// <![CDATA[Represents the mode for rendering a closing tag (for example, </tag>).]]>
		/// </summary>
		EndTag,

		/// <summary>
		/// Represents the mode for rendering a self-closing tag (for example, <tag />).
		/// </summary>
		SelfClosing
	}
}
