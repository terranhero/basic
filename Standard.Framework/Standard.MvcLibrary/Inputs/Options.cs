using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 表示参数设置
	/// </summary>
	public interface IOptions : System.IDisposable
	{
		/// <summary>表示 style 属性集合。</summary>
		IReadOnlyDictionary<string, string> Styles { get; }

		/// <summary>表示 attrs 属性集合。</summary>
		IReadOnlyDictionary<string, string> Attributes { get; }

		/// <summary>表示 style 属性集合。</summary>
		IReadOnlyDictionary<string, string> Properties { get; }

		/// <summary>表示组件事件集合。</summary>
		IReadOnlyDictionary<string, string> Events { get; }

		/// <summary>添加 Css 类</summary>
		/// <param name="className"></param>
		IOptions AddClass(string className);

		/// <summary>设置当前元素的 id 属性。</summary>
		IOptions Id(string id);

		/// <summary>设置当前元素的 ref 属性。</summary>
		IOptions Ref(string name);

		/// <summary>获取数据单元格样式名称</summary>
		string CssClass { get; }

		/// <summary>获取当前数据单元格样式名称是否为空。</summary>
		bool HasCssClass { get; }

		/// <summary>获取当前数据单元格样式名称是否为空。</summary>
		bool HasClass(string className);

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		IOptions Style(string key, string value);

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		IOptions Attr<T>(string key, T value);

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		IOptions Prop<T>(string key, T value);

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		IOptions Event(string evt, string callback);
	}

	/// <summary>
	/// 表示属性抽象定义。
	/// </summary>
	public class Options : IOptions
	{
		private readonly Dictionary<string, string> attrs;
		private readonly List<string> cssClasses = new List<string>(5);
		private readonly Dictionary<string, string> style = new Dictionary<string, string>(20);
		private readonly Dictionary<string, string> props = new Dictionary<string, string>(20);
		private readonly Dictionary<string, string> events = new Dictionary<string, string>(20);
		/// <summary>
		///  初始化 Options 类的新实例，
		///  该实例为空且具有默认的初始容量，
		///  并使用键类型的默认相等比较器。
		/// </summary>
		internal protected Options() { attrs = new Dictionary<string, string>(20); }

		/// <summary>释放非托管资源</summary>
		protected virtual void Dispose() { }
		/// <summary>释放非托管资源</summary>
		void System.IDisposable.Dispose() { this.Dispose(); }

		/// <summary>
		/// 初始化 HtmlAttributes 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		internal protected Options(IDictionary<string, string> dictionary) { attrs = new Dictionary<string, string>(dictionary); }

		/// <summary>表示 Html style 属性集合。</summary>
		public IReadOnlyDictionary<string, string> Styles { get { return new ReadOnlyDictionary<string, string>(style); } }

		/// <summary>表示 Html Attributes 属性集合。</summary>
		public IReadOnlyDictionary<string, string> Attributes { get { return new ReadOnlyDictionary<string, string>(attrs); } }

		/// <summary>表示组件属性集合。</summary>
		public IReadOnlyDictionary<string, string> Properties { get { return new ReadOnlyDictionary<string, string>(props); } }

		/// <summary>表示组件事件集合。</summary>
		public IReadOnlyDictionary<string, string> Events { get { return new ReadOnlyDictionary<string, string>(events); } }

		/// <summary>移除指定键的属性</summary>
		/// <param name="key">需要移除的键</param>
		protected bool RemoveAttribute(string key) { return attrs.Remove(key); }

		/// <summary>移除指定键的属性</summary>
		/// <param name="key">需要移除的键</param>
		protected bool RemoveProperty(string key) { return props.Remove(key); }

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		internal protected string Key { get; set; }

		/// <summary>设置当前元素的 id 属性。</summary>
		public IOptions Id(string id) { Key = id; SetAttr("id", id); return this; }

		/// <summary>设置当前元素的 ref 属性。</summary>
		public IOptions Ref(string name) { Key = name; SetAttr("ref", name); return this; }

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		public IOptions Event(string evt, string callback)
		{
			if (events.ContainsKey(evt)) { events[evt] = callback; }
			else { events.Add(evt, callback); }
			return this;
		}

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		public IOptions SetEvent(string evt, string callback)
		{
			if (events.ContainsKey(evt)) { events[evt] = callback; }
			else { events.Add(evt, callback); }
			return this;
		}

		/// <summary>添加 Css 类</summary>
		/// <param name="className"></param>
		public IOptions AddClass(string className)
		{
			if (cssClasses.Contains(className) == false)
			{
				cssClasses.Add(className);
			}
			return this;
		}

		/// <summary>获取数据单元格样式名称</summary>
		public string CssClass { get { return string.Join(" ", cssClasses.ToArray()); } }

		/// <summary>获取当前样式名称是否为空。</summary>
		public bool HasCssClass { get { return cssClasses.Count > 0; } }

		/// <summary>获取当前样式名称是否为空。</summary>
		public bool HasClass(string className)
		{
			return cssClasses.Contains(className);
		}

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		protected IOptions SetStyle(string key, string value)
		{
			if (style.ContainsKey(key)) { style[key] = value; }
			else { style.Add(key, value); }
			return this;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public IOptions Style(string key, string value)
		{
			if (style.ContainsKey(key)) { style[key] = value; }
			else { style.Add(key, value); }
			return this;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		protected IOptions Attr(string key, string value)
		{
			if (attrs.ContainsKey(key)) { attrs[key] = value; }
			else { attrs.Add(key, value); }
			return this;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public IOptions Attr<T>(string key, T value)
		{
			string val = System.Convert.ToString(value);
			if (value is bool) { val = System.Convert.ToString(value).ToLower(); }
			if (attrs.ContainsKey(key)) { attrs[key] = val; }
			else { attrs.Add(key, val); }
			return this;
		}
		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		protected IOptions SetAttr(string key, string value)
		{
			if (attrs.ContainsKey(key)) { attrs[key] = value; }
			else { attrs.Add(key, value); }
			return this;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		protected IOptions SetAttr<T>(string key, T value)
		{
			string val = System.Convert.ToString(value);
			if (value is bool) { val = System.Convert.ToString(value).ToLower(); }
			if (attrs.ContainsKey(key)) { attrs[key] = val; }
			else { attrs.Add(key, val); }
			return this;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		protected IOptions SetProp(string key, string value)
		{
			if (props.ContainsKey(key)) { props[key] = value; }
			else { props.Add(key, value); }
			return this;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		protected IOptions SetProp<T>(string key, T value)
		{
			string val = System.Convert.ToString(value);
			if (value is bool) { val = System.Convert.ToString(value).ToLower(); }
			if (props.ContainsKey(key)) { props[key] = val; }
			else { props.Add(key, val); }
			return this;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		protected IOptions Prop(string key, string value)
		{
			if (props.ContainsKey(key)) { props[key] = value; }
			else { props.Add(key, value); }
			return this;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public IOptions Prop<T>(string key, T value)
		{
			string val = System.Convert.ToString(value);
			if (value is bool) { val = System.Convert.ToString(value).ToLower(); }
			if (props.ContainsKey(key)) { props[key] = val; }
			else { props.Add(key, val); }
			return this;
		}
	}

	/// <summary>
	/// 表示参数设置
	/// </summary>
	public interface IOptions<TR> : IOptions
	{
		/// <summary>添加 Css 类</summary>
		/// <param name="className"></param>
		new TR AddClass(string className);

		/// <summary>设置当前元素的 id 属性。</summary>
		new TR Id(string id);

		/// <summary>设置当前元素的 ref 属性。</summary>
		new TR Ref(string name);

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		new TR Style(string key, string value);

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		new TR Attr<T>(string key, T value);

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		new TR Prop<T>(string key, T value);

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		new TR Event(string evt, string callback);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TR"></typeparam>
	public abstract class Options<TR> : Options, IOptions<TR> where TR : Options
	{
		/// <summary>
		///  初始化 Options 类的新实例，
		///  该实例为空且具有默认的初始容量，
		///  并使用键类型的默认相等比较器。
		/// </summary>
		internal protected Options() : base() { }

		/// <summary>
		/// 初始化 HtmlAttributes 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		internal protected Options(IDictionary<string, string> dictionary) : base(dictionary) { }

		/// <summary>设置当前元素的 id 属性。</summary>
		public new TR Id(string id) { return base.Id(id) as TR; }

		/// <summary>设置当前元素的 ref 属性。</summary>
		public new TR Ref(string name) { return base.Ref(name) as TR; }

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR Event(string evt, string callback)
		{
			return base.Event(evt, callback) as TR;
		}

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR SetEvent(string evt, string callback)
		{
			return base.SetEvent(evt, callback) as TR;
		}

		/// <summary>添加 Css 类</summary>
		/// <param name="className"></param>
		public new TR AddClass(string className)
		{
			return base.AddClass(className) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR SetStyle(string key, string value)
		{
			return base.SetStyle(key, value) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR Style(string key, string value)
		{
			return base.Style(key, value) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR Attr(string key, string value)
		{
			return base.Attr(key, value) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR Attr<T>(string key, T value)
		{
			return base.Attr(key, value) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR SetAttr(string key, string value)
		{
			return base.SetAttr(key, value) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR SetAttr<T>(string key, T value)
		{
			return base.SetAttr(key, value) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR SetProp(string key, string value)
		{
			return base.SetProp(key, value) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR SetProp<T>(string key, T value)
		{
			return base.SetProp<T>(key, value) as TR;
		}


		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR Prop(string key, string value)
		{
			return base.Prop(key, value) as TR;
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TR Prop<T>(string key, T value)
		{
			return base.Prop<T>(key, value) as TR;
		}
	}
}
