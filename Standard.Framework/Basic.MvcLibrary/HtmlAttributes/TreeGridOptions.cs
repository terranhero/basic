using Basic.Enums;
using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 EasyGrid 属性定义
	/// </summary>
	public class TreeGridOptions : DataGridOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 EasyGridOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public TreeGridOptions() : base("easyui-treepage ") { }

		/// <summary>
		///  使用样式名称初始化 EasyGridOptions 类实例。
		/// </summary>
		/// <param name="classname">表示当前EasyGrid的样式名称。</param>
		public TreeGridOptions(string classname) : base() { className = classname; }
		/// <summary>
		/// 初始化 ButtonOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public TreeGridOptions(EasyOptions dictionary) : base(dictionary) { }

		/// <summary>
		/// 初始化 EasyGridOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public TreeGridOptions(IDictionary<string, object> dictionary) : base(dictionary) { className = "easyui-treepage"; }

	}
}
