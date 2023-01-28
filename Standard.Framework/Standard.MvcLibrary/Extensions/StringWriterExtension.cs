using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;

namespace Basic.EasyLibrary
{
	/// <summary>关于 StringWriter Html格式化输出的扩展方法</summary>
	public static class StringWriterExtension
	{
		private static Hashtable _tagKeyLookupTable;
		private static Hashtable _attrKeyLookupTable;
		private static TagInformation[] _tagNameLookupArray;
		private static AttributeInformation[] _attrNameLookupArray;

		static StringWriterExtension()
		{
			_tagKeyLookupTable = new Hashtable(97);
			_tagNameLookupArray = new TagInformation[97];
			RegisterTag(string.Empty, HtmlTextWriterTag.Unknown, TagType.Other);
			RegisterTag("a", HtmlTextWriterTag.A, TagType.Inline);
			RegisterTag("acronym", HtmlTextWriterTag.Acronym, TagType.Inline);
			RegisterTag("address", HtmlTextWriterTag.Address, TagType.Other);
			RegisterTag("area", HtmlTextWriterTag.Area, TagType.NonClosing);
			RegisterTag("b", HtmlTextWriterTag.B, TagType.Inline);
			RegisterTag("base", HtmlTextWriterTag.Base, TagType.NonClosing);
			RegisterTag("basefont", HtmlTextWriterTag.Basefont, TagType.NonClosing);
			RegisterTag("bdo", HtmlTextWriterTag.Bdo, TagType.Inline);
			RegisterTag("bgsound", HtmlTextWriterTag.Bgsound, TagType.NonClosing);
			RegisterTag("big", HtmlTextWriterTag.Big, TagType.Inline);
			RegisterTag("blockquote", HtmlTextWriterTag.Blockquote, TagType.Other);
			RegisterTag("body", HtmlTextWriterTag.Body, TagType.Other);
			RegisterTag("br", HtmlTextWriterTag.Br, TagType.Other);
			RegisterTag("button", HtmlTextWriterTag.Button, TagType.Inline);
			RegisterTag("caption", HtmlTextWriterTag.Caption, TagType.Other);
			RegisterTag("center", HtmlTextWriterTag.Center, TagType.Other);
			RegisterTag("cite", HtmlTextWriterTag.Cite, TagType.Inline);
			RegisterTag("code", HtmlTextWriterTag.Code, TagType.Inline);
			RegisterTag("col", HtmlTextWriterTag.Col, TagType.NonClosing);
			RegisterTag("colgroup", HtmlTextWriterTag.Colgroup, TagType.Other);
			RegisterTag("del", HtmlTextWriterTag.Del, TagType.Inline);
			RegisterTag("dd", HtmlTextWriterTag.Dd, TagType.Inline);
			RegisterTag("dfn", HtmlTextWriterTag.Dfn, TagType.Inline);
			RegisterTag("dir", HtmlTextWriterTag.Dir, TagType.Other);
			RegisterTag("div", HtmlTextWriterTag.Div, TagType.Other);
			RegisterTag("dl", HtmlTextWriterTag.Dl, TagType.Other);
			RegisterTag("dt", HtmlTextWriterTag.Dt, TagType.Inline);
			RegisterTag("em", HtmlTextWriterTag.Em, TagType.Inline);
			RegisterTag("embed", HtmlTextWriterTag.Embed, TagType.NonClosing);
			RegisterTag("fieldset", HtmlTextWriterTag.Fieldset, TagType.Other);
			RegisterTag("font", HtmlTextWriterTag.Font, TagType.Inline);
			RegisterTag("form", HtmlTextWriterTag.Form, TagType.Other);
			RegisterTag("frame", HtmlTextWriterTag.Frame, TagType.NonClosing);
			RegisterTag("frameset", HtmlTextWriterTag.Frameset, TagType.Other);
			RegisterTag("h1", HtmlTextWriterTag.H1, TagType.Other);
			RegisterTag("h2", HtmlTextWriterTag.H2, TagType.Other);
			RegisterTag("h3", HtmlTextWriterTag.H3, TagType.Other);
			RegisterTag("h4", HtmlTextWriterTag.H4, TagType.Other);
			RegisterTag("h5", HtmlTextWriterTag.H5, TagType.Other);
			RegisterTag("h6", HtmlTextWriterTag.H6, TagType.Other);
			RegisterTag("head", HtmlTextWriterTag.Head, TagType.Other);
			RegisterTag("hr", HtmlTextWriterTag.Hr, TagType.NonClosing);
			RegisterTag("html", HtmlTextWriterTag.Html, TagType.Other);
			RegisterTag("i", HtmlTextWriterTag.I, TagType.Inline);
			RegisterTag("iframe", HtmlTextWriterTag.Iframe, TagType.Other);
			RegisterTag("img", HtmlTextWriterTag.Img, TagType.NonClosing);
			RegisterTag("input", HtmlTextWriterTag.Input, TagType.NonClosing);
			RegisterTag("ins", HtmlTextWriterTag.Ins, TagType.Inline);
			RegisterTag("isindex", HtmlTextWriterTag.Isindex, TagType.NonClosing);
			RegisterTag("kbd", HtmlTextWriterTag.Kbd, TagType.Inline);
			RegisterTag("label", HtmlTextWriterTag.Label, TagType.Inline);
			RegisterTag("legend", HtmlTextWriterTag.Legend, TagType.Other);
			RegisterTag("li", HtmlTextWriterTag.Li, TagType.Inline);
			RegisterTag("link", HtmlTextWriterTag.Link, TagType.NonClosing);
			RegisterTag("map", HtmlTextWriterTag.Map, TagType.Other);
			RegisterTag("marquee", HtmlTextWriterTag.Marquee, TagType.Other);
			RegisterTag("menu", HtmlTextWriterTag.Menu, TagType.Other);
			RegisterTag("meta", HtmlTextWriterTag.Meta, TagType.NonClosing);
			RegisterTag("nobr", HtmlTextWriterTag.Nobr, TagType.Inline);
			RegisterTag("noframes", HtmlTextWriterTag.Noframes, TagType.Other);
			RegisterTag("noscript", HtmlTextWriterTag.Noscript, TagType.Other);
			RegisterTag("object", HtmlTextWriterTag.Object, TagType.Other);
			RegisterTag("ol", HtmlTextWriterTag.Ol, TagType.Other);
			RegisterTag("option", HtmlTextWriterTag.Option, TagType.Other);
			RegisterTag("p", HtmlTextWriterTag.P, TagType.Inline);
			RegisterTag("param", HtmlTextWriterTag.Param, TagType.Other);
			RegisterTag("pre", HtmlTextWriterTag.Pre, TagType.Other);
			RegisterTag("ruby", HtmlTextWriterTag.Ruby, TagType.Other);
			RegisterTag("rt", HtmlTextWriterTag.Rt, TagType.Other);
			RegisterTag("q", HtmlTextWriterTag.Q, TagType.Inline);
			RegisterTag("s", HtmlTextWriterTag.S, TagType.Inline);
			RegisterTag("samp", HtmlTextWriterTag.Samp, TagType.Inline);
			RegisterTag("script", HtmlTextWriterTag.Script, TagType.Other);
			RegisterTag("select", HtmlTextWriterTag.Select, TagType.Other);
			RegisterTag("small", HtmlTextWriterTag.Small, TagType.Other);
			RegisterTag("span", HtmlTextWriterTag.Span, TagType.Inline);
			RegisterTag("strike", HtmlTextWriterTag.Strike, TagType.Inline);
			RegisterTag("strong", HtmlTextWriterTag.Strong, TagType.Inline);
			RegisterTag("style", HtmlTextWriterTag.Style, TagType.Other);
			RegisterTag("sub", HtmlTextWriterTag.Sub, TagType.Inline);
			RegisterTag("sup", HtmlTextWriterTag.Sup, TagType.Inline);
			RegisterTag("table", HtmlTextWriterTag.Table, TagType.Other);
			RegisterTag("tbody", HtmlTextWriterTag.Tbody, TagType.Other);
			RegisterTag("td", HtmlTextWriterTag.Td, TagType.Inline);
			RegisterTag("textarea", HtmlTextWriterTag.Textarea, TagType.Inline);
			RegisterTag("tfoot", HtmlTextWriterTag.Tfoot, TagType.Other);
			RegisterTag("th", HtmlTextWriterTag.Th, TagType.Inline);
			RegisterTag("thead", HtmlTextWriterTag.Thead, TagType.Other);
			RegisterTag("title", HtmlTextWriterTag.Title, TagType.Other);
			RegisterTag("tr", HtmlTextWriterTag.Tr, TagType.Other);
			RegisterTag("tt", HtmlTextWriterTag.Tt, TagType.Inline);
			RegisterTag("u", HtmlTextWriterTag.U, TagType.Inline);
			RegisterTag("ul", HtmlTextWriterTag.Ul, TagType.Other);
			RegisterTag("var", HtmlTextWriterTag.Var, TagType.Inline);
			RegisterTag("wbr", HtmlTextWriterTag.Wbr, TagType.NonClosing);
			RegisterTag("xml", HtmlTextWriterTag.Xml, TagType.Other);
			_attrKeyLookupTable = new Hashtable(54);
			_attrNameLookupArray = new AttributeInformation[54];
			RegisterAttribute("abbr", HtmlTextWriterAttribute.Abbr, encode: true);
			RegisterAttribute("accesskey", HtmlTextWriterAttribute.Accesskey, encode: true);
			RegisterAttribute("align", HtmlTextWriterAttribute.Align, encode: false);
			RegisterAttribute("alt", HtmlTextWriterAttribute.Alt, encode: true);
			RegisterAttribute("autocomplete", HtmlTextWriterAttribute.AutoComplete, encode: false);
			RegisterAttribute("axis", HtmlTextWriterAttribute.Axis, encode: true);
			RegisterAttribute("background", HtmlTextWriterAttribute.Background, encode: true, isUrl: true);
			RegisterAttribute("bgcolor", HtmlTextWriterAttribute.Bgcolor, encode: false);
			RegisterAttribute("border", HtmlTextWriterAttribute.Border, encode: false);
			RegisterAttribute("bordercolor", HtmlTextWriterAttribute.Bordercolor, encode: false);
			RegisterAttribute("cellpadding", HtmlTextWriterAttribute.Cellpadding, encode: false);
			RegisterAttribute("cellspacing", HtmlTextWriterAttribute.Cellspacing, encode: false);
			RegisterAttribute("checked", HtmlTextWriterAttribute.Checked, encode: false);
			RegisterAttribute("class", HtmlTextWriterAttribute.Class, encode: true);
			RegisterAttribute("cols", HtmlTextWriterAttribute.Cols, encode: false);
			RegisterAttribute("colspan", HtmlTextWriterAttribute.Colspan, encode: false);
			RegisterAttribute("content", HtmlTextWriterAttribute.Content, encode: true);
			RegisterAttribute("coords", HtmlTextWriterAttribute.Coords, encode: false);
			RegisterAttribute("dir", HtmlTextWriterAttribute.Dir, encode: false);
			RegisterAttribute("disabled", HtmlTextWriterAttribute.Disabled, encode: false);
			RegisterAttribute("for", HtmlTextWriterAttribute.For, encode: false);
			RegisterAttribute("headers", HtmlTextWriterAttribute.Headers, encode: true);
			RegisterAttribute("height", HtmlTextWriterAttribute.Height, encode: false);
			RegisterAttribute("href", HtmlTextWriterAttribute.Href, encode: true, isUrl: true);
			RegisterAttribute("id", HtmlTextWriterAttribute.Id, encode: false);
			RegisterAttribute("longdesc", HtmlTextWriterAttribute.Longdesc, encode: true, isUrl: true);
			RegisterAttribute("maxlength", HtmlTextWriterAttribute.Maxlength, encode: false);
			RegisterAttribute("multiple", HtmlTextWriterAttribute.Multiple, encode: false);
			RegisterAttribute("name", HtmlTextWriterAttribute.Name, encode: false);
			RegisterAttribute("nowrap", HtmlTextWriterAttribute.Nowrap, encode: false);
			RegisterAttribute("onclick", HtmlTextWriterAttribute.Onclick, encode: true);
			RegisterAttribute("onchange", HtmlTextWriterAttribute.Onchange, encode: true);
			RegisterAttribute("readonly", HtmlTextWriterAttribute.ReadOnly, encode: false);
			RegisterAttribute("rel", HtmlTextWriterAttribute.Rel, encode: false);
			RegisterAttribute("rows", HtmlTextWriterAttribute.Rows, encode: false);
			RegisterAttribute("rowspan", HtmlTextWriterAttribute.Rowspan, encode: false);
			RegisterAttribute("rules", HtmlTextWriterAttribute.Rules, encode: false);
			RegisterAttribute("scope", HtmlTextWriterAttribute.Scope, encode: false);
			RegisterAttribute("selected", HtmlTextWriterAttribute.Selected, encode: false);
			RegisterAttribute("shape", HtmlTextWriterAttribute.Shape, encode: false);
			RegisterAttribute("size", HtmlTextWriterAttribute.Size, encode: false);
			RegisterAttribute("src", HtmlTextWriterAttribute.Src, encode: true, isUrl: true);
			RegisterAttribute("style", HtmlTextWriterAttribute.Style, encode: false);
			RegisterAttribute("tabindex", HtmlTextWriterAttribute.Tabindex, encode: false);
			RegisterAttribute("target", HtmlTextWriterAttribute.Target, encode: false);
			RegisterAttribute("title", HtmlTextWriterAttribute.Title, encode: true);
			RegisterAttribute("type", HtmlTextWriterAttribute.Type, encode: false);
			RegisterAttribute("usemap", HtmlTextWriterAttribute.Usemap, encode: false);
			RegisterAttribute("valign", HtmlTextWriterAttribute.Valign, encode: false);
			RegisterAttribute("value", HtmlTextWriterAttribute.Value, encode: true);
			RegisterAttribute("vcard_name", HtmlTextWriterAttribute.VCardName, encode: false);
			RegisterAttribute("width", HtmlTextWriterAttribute.Width, encode: false);
			RegisterAttribute("wrap", HtmlTextWriterAttribute.Wrap, encode: false);
			RegisterAttribute("_designerRegion", HtmlTextWriterAttribute.DesignerRegion, encode: false);
		}

		/// <summary>
		/// 向其开始标记中添加指定的标记特性和值。
		/// </summary>
		/// <param name="writer">接收EasyUI.DataGrid内容的 HtmlTextWriter 对象。</param>
		/// <param name="options"></param>
		public static void AddAttribute(this StringWriter writer, IDictionary<string, object> options)
		{
			foreach (var item in options)
			{
				writer.AddAttribute(item.Key, Convert.ToString(item.Value));
			}
		}

		///// <summary>
		///// 向其开始标记中添加指定的标记特性和值。
		///// </summary>
		///// <param name="writer">接收EasyUI.DataGrid内容的 HtmlTextWriter 对象。</param>
		///// <param name="options"></param>
		//public static void AddStyleAttribute(this StringWriter writer, IDictionary<string, string> options)
		//{
		//	foreach (var item in options)
		//	{
		//		writer.AddStyleAttribute(item.Key, item.Value);
		//	}
		//}

		//public static void AddStyleAttribute(string name, string value)
		//{
		//	AddStyleAttribute(name, value, CssTextWriter.GetStyleKey(name));
		//}

		//public static void AddStyleAttribute(HtmlTextWriterStyle key, string value)
		//{
		//	AddStyleAttribute(CssTextWriter.GetStyleName(key), value, key);
		//}

		//private virtual void AddStyleAttribute(string name, string value, HtmlTextWriterStyle key)
		//{
		//	if (_styleList == null)
		//	{
		//		_styleList = new RenderStyle[20];
		//	}
		//	else if (_styleCount > _styleList.Length)
		//	{
		//		RenderStyle[] array = new RenderStyle[_styleList.Length * 2];
		//		Array.Copy(_styleList, array, _styleList.Length);
		//		_styleList = array;
		//	}

		//	RenderStyle renderStyle = default(RenderStyle);
		//	renderStyle.name = name;
		//	renderStyle.key = key;
		//	string value2 = value;
		//	if (CssTextWriter.IsStyleEncoded(key))
		//	{
		//		value2 = HttpUtility.HtmlAttributeEncode(value);
		//	}

		//	renderStyle.value = value2;
		//	_styleList[_styleCount] = renderStyle;
		//	_styleCount++;
		//}

		/// <summary></summary>
		/// <param name="writer"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void AddAttribute(this StringWriter writer, string name, string value)
		{
			HtmlTextWriterAttribute attributeKey = GetAttributeKey(name);
			value = EncodeAttributeValue(attributeKey, value);
			writer.Write("{0}=\"{1}\"", name, value);
		}

		private static HtmlTextWriterAttribute GetAttributeKey(string attrName)
		{
			if (!string.IsNullOrEmpty(attrName))
			{
				object obj = _attrKeyLookupTable[attrName.ToLower(CultureInfo.InvariantCulture)];
				if (obj != null)
				{
					return (HtmlTextWriterAttribute)obj;
				}
			}

			return (HtmlTextWriterAttribute)(-1);
		}

		/// <summary></summary>
		/// <param name="writer"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="fEndode"></param>
		public static void AddAttribute(this StringWriter writer, string name, string value, bool fEndode)
		{
			value = EncodeAttributeValue(value, fEndode);
			writer.AddAttribute(name, value);
		}

		private static string EncodeAttributeValue(string value, bool fEncode)
		{
			if (value == null) { return null; }

			if (!fEncode) { return value; }

			return HttpUtility.HtmlAttributeEncode(value);
		}

		private static string EncodeAttributeValue(HtmlTextWriterAttribute attrKey, string value)
		{
			bool fEncode = true;
			if (HtmlTextWriterAttribute.Accesskey <= attrKey && (int)attrKey < _attrNameLookupArray.Length)
			{
				fEncode = _attrNameLookupArray[(int)attrKey].encode;
			}

			return EncodeAttributeValue(value, fEncode);
		}

		private enum HtmlTextWriterAttribute
		{
			Accesskey,
			Align,
			Alt,
			Background,
			Bgcolor,
			Border,
			Bordercolor,
			Cellpadding,
			Cellspacing,
			Checked,
			Class,
			Cols,
			Colspan,
			Disabled,
			For,
			Height,
			Href,
			Id,
			Maxlength,
			Multiple,
			Name,
			Nowrap,
			Onchange,
			Onclick,
			ReadOnly,
			Rows,
			Rowspan,
			Rules,
			Selected,
			Size,
			Src,
			Style,
			Tabindex,
			Target,
			Title,
			Type,
			Valign,
			Value,
			Width,
			Wrap,
			Abbr,
			AutoComplete,
			Axis,
			Content,
			Coords,
			DesignerRegion,
			Dir,
			Headers,
			Longdesc,
			Rel,
			Scope,
			Shape,
			Usemap,
			VCardName
		}

		private enum HtmlTextWriterTag
		{
			Unknown,
			A,
			Acronym,
			Address,
			Area,
			B,
			Base,
			Basefont,
			Bdo,
			Bgsound,
			Big,
			Blockquote,
			Body,
			Br,
			Button,
			Caption,
			Center,
			Cite,
			Code,
			Col,
			Colgroup,
			Dd,
			Del,
			Dfn,
			Dir,
			Div,
			Dl,
			Dt,
			Em,
			Embed,
			Fieldset,
			Font,
			Form,
			Frame,
			Frameset,
			H1,
			H2,
			H3,
			H4,
			H5,
			H6,
			Head,
			Hr,
			Html,
			I,
			Iframe,
			Img,
			Input,
			Ins,
			Isindex,
			Kbd,
			Label,
			Legend,
			Li,
			Link,
			Map,
			Marquee,
			Menu,
			Meta,
			Nobr,
			Noframes,
			Noscript,
			Object,
			Ol,
			Option,
			P,
			Param,
			Pre,
			Q,
			Rt,
			Ruby,
			S,
			Samp,
			Script,
			Select,
			Small,
			Span,
			Strike,
			Strong,
			Style,
			Sub,
			Sup,
			Table,
			Tbody,
			Td,
			Textarea,
			Tfoot,
			Th,
			Thead,
			Title,
			Tr,
			Tt,
			U,
			Ul,
			Var,
			Wbr,
			Xml
		}

		private struct TagInformation
		{
			public string name;

			public TagType tagType;

			public string closingTag;

			public TagInformation(string name, TagType tagType, string closingTag)
			{
				this.name = name;
				this.tagType = tagType;
				this.closingTag = closingTag;
			}
		}

		private enum HorizontalAlign
		{
			NotSet,
			Left,
			Center,
			Right,
			Justify
		}

		private class Layout
		{
			private bool _wrap;

			private HorizontalAlign _align;

			public bool Wrap
			{
				get
				{
					return _wrap;
				}
				set
				{
					_wrap = value;
				}
			}

			public HorizontalAlign Align
			{
				get
				{
					return _align;
				}
				set
				{
					_align = value;
				}
			}

			public Layout(HorizontalAlign alignment, bool wrapping)
			{
				Align = alignment;
				Wrap = wrapping;
			}
		}

		private struct AttributeInformation
		{
			public string name;

			public bool isUrl;

			public bool encode;

			public AttributeInformation(string name, bool encode, bool isUrl)
			{
				this.name = name;
				this.encode = encode;
				this.isUrl = isUrl;
			}
		}

		private enum TagType
		{
			Inline,
			NonClosing,
			Other
		}

		private static void RegisterTag(string name, HtmlTextWriterTag key)
		{
			RegisterTag(name, key, TagType.Other);
		}

		private static void RegisterTag(string name, HtmlTextWriterTag key, TagType type)
		{
			string text = name.ToLower(CultureInfo.InvariantCulture);
			_tagKeyLookupTable.Add(text, key);
			string closingTag = null;
			if (type != TagType.NonClosing && key != 0)
			{
				closingTag = "</" + text + '>'.ToString(CultureInfo.InvariantCulture);
			}

			if ((int)key < _tagNameLookupArray.Length)
			{
				_tagNameLookupArray[(int)key] = new TagInformation(name, type, closingTag);
			}
		}

		private static void RegisterAttribute(string name, HtmlTextWriterAttribute key)
		{
			RegisterAttribute(name, key, encode: false);
		}

		private static void RegisterAttribute(string name, HtmlTextWriterAttribute key, bool encode)
		{
			RegisterAttribute(name, key, encode, isUrl: false);
		}

		private static void RegisterAttribute(string name, HtmlTextWriterAttribute key, bool encode, bool isUrl)
		{
			string key2 = name.ToLower(CultureInfo.InvariantCulture);
			_attrKeyLookupTable.Add(key2, key);
			if ((int)key < _attrNameLookupArray.Length)
			{
				_attrNameLookupArray[(int)key] = new AttributeInformation(name, encode, isUrl);
			}
		}



	}
}
