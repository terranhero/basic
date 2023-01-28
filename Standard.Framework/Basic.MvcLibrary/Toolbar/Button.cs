using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web.Routing;
using Basic.Messages;

namespace Basic.MvcLibrary
{
	/// <summary>表示按钮</summary>
	public sealed class Button : Options<Button>
	{
		private readonly string buttonConverter = typeof(WebStrings).Name;
		private readonly string tagName;
		private readonly bool mEmpty = false;
		private readonly IBasicContext mBasic;

		/// <summary>使用按钮类型初始化工具条按钮</summary>
		/// <param name="tb">表示工具条</param>
		/// <param name="type">按钮类型</param>
		internal Button(IToolbar tb, ButtonType type) { mBasic = tb.Basic; Type = type; tagName = ViewTags.Button; }

		/// <summary>使用按钮类型初始化工具条按钮</summary>
		/// <param name="bh">基础开发框架扩展</param>
		/// <param name="type">按钮类型</param>
		internal Button(IBasicContext bh, ButtonType type) { mBasic = bh; Type = type; tagName = ViewTags.Button; }

		/// <summary>使用按钮类型初始化工具条按钮</summary>
		/// <param name="bh">基础开发框架扩展</param>
		/// <param name="tag">按钮标记</param>
		internal Button(IBasicContext bh, string tag) { mBasic = bh; Type = ButtonType.None; tagName = tag; }

		/// <summary>初始化空按钮实例，此按钮不输出。</summary>
		private Button(IBasicContext bh) { mBasic = bh; mEmpty = true; }

		/// <summary>表示空按钮</summary>
		internal static Button Empty(IBasicContext bh) { return new Button(bh); }

		/// <summary>按钮类型</summary>
		public ButtonType Type { get; private set; }

		/// <summary>按钮类型</summary>
		private bool mOutput = false;

		/// <summary>设置按钮提示文本</summary>
		public Button Title(string converter, string source)
		{
			if (string.IsNullOrWhiteSpace(source) == false)
			{
				if (buttonConverter == converter)
				{
					System.Globalization.CultureInfo culture = mBasic.GetCultureInfo();
					string cultureText = string.Concat(source, "_", culture.Name);
					string msgText = MessageContext.GetString(converter, cultureText);
					if (string.IsNullOrWhiteSpace(msgText)) { msgText = MessageContext.GetString(converter, source); }
					else if (cultureText == msgText) { msgText = MessageContext.GetString(converter, source); }
					SetAttr("title", msgText ?? source);
				}
				else
				{
					string msgText = mBasic.GetString(converter, source);
					if (string.IsNullOrWhiteSpace(msgText)) { msgText = mBasic.GetString(converter, source); }
					else if (source == msgText) { msgText = mBasic.GetString(converter, source); }
					SetAttr("title", msgText ?? source);
				}
			}
			return this;
		}

		/// <summary>设置按钮提示文本</summary>
		public Button Title(string key)
		{
			if (string.IsNullOrWhiteSpace(key) == false)
			{
				string sourceName = key, converterName = null;
				if (key.IndexOf(":") >= 0)
				{
					string[] strArray = key.Split(':');
					converterName = strArray[0]; sourceName = strArray[1];
				}
				return Title(converterName, sourceName);
			}
			return this;
		}

		/// <summary>设置开始和结束标签之间的 HTML</summary>
		public Button Text(string converter, string source)
		{
			if (string.IsNullOrWhiteSpace(source) == false)
			{
				if (buttonConverter == converter)
				{
					System.Globalization.CultureInfo culture = mBasic.GetCultureInfo();
					string cultureText = string.Concat(source, "_", culture.Name);
					string msgText = MessageContext.GetString(converter, cultureText);
					if (string.IsNullOrWhiteSpace(msgText)) { msgText = MessageContext.GetString(converter, source); }
					else if (cultureText == msgText) { msgText = MessageContext.GetString(converter, source); }
					mInnerHtml = msgText ?? source;
				}
				else
				{
					string msgText = mBasic.GetString(converter, source);
					if (string.IsNullOrWhiteSpace(msgText)) { msgText = mBasic.GetString(converter, source); }
					else if (source == msgText) { msgText = mBasic.GetString(converter, source); }
					mInnerHtml = msgText ?? source;
				}
			}
			return this;
		}

		/// <summary>设置开始和结束标签之间的 HTML</summary>
		public Button Text(string key)
		{
			if (string.IsNullOrWhiteSpace(key) == false)
			{
				string sourceName = key, converterName = null;
				if (key.IndexOf(":") >= 0)
				{
					string[] strArray = key.Split(':');
					converterName = strArray[0]; sourceName = strArray[1];
				}
				return Text(converterName, sourceName);
			}
			return this;
		}

		private string mInnerHtml;
		/// <summary>设置开始和结束标签之间的 HTML</summary>
		public Button Html(string html) { mInnerHtml = html; return this; }

		/// <summary>图标类名</summary>
		/// <param name="iconCls">图标类名</param>
		/// <returns>返回当前按钮实例</returns>
		public Button Icon(string iconCls) { SetAttr("icon", iconCls); return this; }

		/// <summary>图标类名</summary>
		/// <param name="callback">图标类名</param>
		/// <returns>返回当前按钮实例</returns>
		public Button Click(string callback) { SetEvent("click", callback); return this; }

		/// <summary>按钮类型</summary>
		/// <param name="size">medium / small / mini</param>
		/// <returns>返回当前按钮实例</returns>
		public Button Size(string size) { SetAttr("size", size); return this; }

		/// <summary>设置为朴素按钮</summary>
		/// <returns>返回当前按钮实例</returns>
		public Button Plain() { SetProp("plain", "true"); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public Button Action(string action) { SetAttr("url", mBasic.Action(action, null)); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public Button Action(string action, string controller) { SetAttr("url", mBasic.Action(action, controller)); return this; }

		/// <summary>设置执行请求。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public Button Action(string action, string controller, RouteValueDictionary routeValues)
		{
			SetAttr("url", mBasic.Action(action, controller, routeValues)); return this;
		}

		/// <summary>设置执行请求。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public Button Action(string action, string controller, object routeValues)
		{
			SetAttr("url", mBasic.Action(action, controller, routeValues)); return this;
		}

		/// <summary>按钮类型</summary>
		/// <param name="type">primary / success / warning / danger / info / text</param>
		/// <returns>返回当前按钮实例</returns>
		public Button SetType(string type) { SetAttr("type", type); return this; }

		/// <summary>设置按钮可用状态</summary>
		/// <param name="status">表示页面状态</param>
		/// <returns>返回当前按钮实例</returns>
		public Button Status(PageStatus status) { SetProp("status", (int)status); return this; }

		/// <summary>设置按钮可见状态</summary>
		/// <param name="status">表示页面状态</param>
		/// <returns>返回当前按钮实例</returns>
		public Button Visible(PageStatus status) { SetProp("visible", (int)status); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="writer">包含要添加的属性的名称的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		internal void Render(TextWriter writer)
		{
			if (mEmpty == true) { return; }
			if (mOutput == true) { return; }
			using (TagHtmlWriter builder = new TagHtmlWriter(tagName))
			{
				if (HasCssClass) { builder.AddCssClass(CssClass); }
				SetProp("btn-type", System.Convert.ToString((int)Type));
				builder.MergeOptions(this);
				builder.SetInnerText(mInnerHtml);
				builder.Render(writer);
			}
		}

		/// <summary>显示按钮的字符串表示形式(HTML)</summary>
		/// <returns>返回按钮的 Html 字符串</returns>
		public string ToString(bool isOutput)
		{
			if (mEmpty == true) { return string.Empty; }
			using (TagHtmlWriter builder = new TagHtmlWriter(tagName))
			{
				mOutput = isOutput;
				if (HasCssClass) { builder.AddCssClass(CssClass); }
				SetProp("btn-type", System.Convert.ToString((int)Type));
				builder.MergeOptions(this);
				builder.SetInnerText(mInnerHtml);
				return builder.ToString();
			}
		}

		/// <summary>显示按钮的字符串表示形式(HTML)</summary>
		/// <returns>返回按钮的 Html 字符串</returns>
		public override string ToString()
		{
			return ToString(false);
		}
	}

	/// <summary>表示工具条集合按钮</summary>
	[SuppressMessage("CodeQuality", "IDE0052:删除未读的私有成员", Justification = "<挂起>")]
	public sealed class ButtonCollection<T> : IEnumerable<Button> where T : class
	{
		private readonly List<Button> buttons = new List<Button>(20);
		private readonly Toolbar<T> mToolBar;
		private readonly IBasicContext mBasic;
		internal ButtonCollection(Toolbar<T> toolbar) { mToolBar = toolbar; }
		internal ButtonCollection(IBasicContext bh) { mBasic = bh; }

		/// <summary></summary>
		/// <param name="button"></param>
		/// <returns></returns>
		internal Button Add(Button button)
		{
			buttons.Add(button);
			return button;
		}

		/// <summary>返回循环访问 ButtonCollection 的枚举数</summary>
		/// <returns><![CDATA[一个可以循环访问集合的 System.Collections.IEnumerator<IButton> 实例 ]]></returns>
		IEnumerator<Button> IEnumerable<Button>.GetEnumerator() { return buttons.GetEnumerator(); }

		/// <summary>返回循环访问 ButtonCollection 的枚举数</summary>
		/// <returns>一个可以循环访问 集合的 System.Collections.IEnumerator 实例</returns>
		IEnumerator IEnumerable.GetEnumerator() { return buttons.GetEnumerator(); }
	}

}
