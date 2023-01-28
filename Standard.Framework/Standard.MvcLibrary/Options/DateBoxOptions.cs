using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 DateBox 属性扩展。
	/// </summary>
	public class DateBoxOptions : ComboOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 DateBoxOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public DateBoxOptions() : base() { }

		/// <summary>
		/// 初始化 DateBoxOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public DateBoxOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 初始化 ComboTreeOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public DateBoxOptions(EasyOptions dictionary) : base(dictionary) { }

		/// <summary>
		/// 显示当天按钮，默认值（Today）
		/// </summary>
		public string currentText { set { AddToDataOptions("currentText", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 显示关闭按钮，默认值（Close）
		/// </summary>
		public string closeText { set { AddToDataOptions("closeText", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 显示OK按钮，默认值（Ok）
		/// </summary>
		public string okText { set { AddToDataOptions("okText", string.Concat("'", value, "'")); } }

		/// <summary>在用户选择了一个日期的时候触发</summary>
		public string onSelect { set { AddToDataOptions("onSelect", value); } }

	}
}
