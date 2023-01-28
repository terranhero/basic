using Basic.MvcLibrary;
using System.Collections.Generic;
using System.Linq;
namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 EasyUI 属性抽象定义。
	/// </summary>
	public abstract class EasyOptions : Dictionary<string, object>, IHtmlAttrs
	{
		private readonly Dictionary<string, object> _DataOptions = new Dictionary<string, object>();
		private readonly Dictionary<string, string> _StyleOptions = new Dictionary<string, string>();
		/// <summary>
		///  初始化 EasyOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		protected EasyOptions() : base() { }

		/// <summary>
		/// 初始化 EasyOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		internal protected EasyOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 表示data-options属性集合。
		/// </summary>
		internal protected Dictionary<string, object> DataOptions { get { return _DataOptions; } }

		/// <summary>
		/// 表示 style 属性集合。
		/// </summary>
		internal protected Dictionary<string, string> StyleOptions { get { return _StyleOptions; } }

		/// <summary>
		/// 表示data-options属性集合。
		/// </summary>
		internal protected string GetOptions
		{
			get { return string.Join(",", _DataOptions.Select(m => string.Concat(m.Key, ":", m.Value))); }
		}

		/// <summary>
		/// 表示 style 属性集合。
		/// </summary>
		internal protected string GetStyle
		{
			get { return string.Join(",", _StyleOptions.Select(m => string.Concat(m.Key, "=", m.Value))); }
		}

		/// <summary>
		/// 将属性添加data-options特性中。
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		protected void AddToDataOptions(string name, object value)
		{
			_DataOptions[name] = value;
			base["data-options"] = string.Join(",", _DataOptions.Select(m => string.Concat(m.Key, ":", m.Value)));
			//string newValue;
			//if (name == "data-options") { newValue = ((string)value); }
			//else { newValue = string.Concat(name, ":", value); }
			//if (base.ContainsKey("data-options"))
			//	base["data-options"] = string.Concat(base["data-options"], ",", newValue);
			//else { base["data-options"] = newValue; }
		}

		/// <summary>
		/// 添加 Css 类
		/// </summary>
		/// <param name="className"></param>
		public void AddCssClass(string className)
		{
			if (ContainsKey("class"))
			{
				base["class"] = string.Concat(base["class"], " ", className);
			}
		}

		/// <summary>
		/// 设置id属性值
		/// </summary>
		public string id { set { base["id"] = value; } }

		/// <summary>
		/// 设置name属性值
		/// </summary>
		public string name { set { base["name"] = value; } }

		/// <summary>
		/// 设置class属性值
		/// </summary>
		public string className { set { base["class"] = value; } }

		/// <summary>
		/// 设置disabled属性值
		/// </summary>
		public bool disabled { set { base["disabled"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置水印文本
		/// </summary>
		public string placeHolder { set { base["placeholder"] = value; } }

		/// <summary>
		/// 设置style属性值
		/// </summary>
		public string style { set { base["style"] = value; } }
	}
}
