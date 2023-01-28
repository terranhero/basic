using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 NumberSpinner 属性扩展。
	/// </summary>
	public sealed class NumberSpinnerOptions : NumberBoxOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 NumberSpinnerOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public NumberSpinnerOptions() : base() { }

		/// <summary>
		/// 初始化 NumberSpinnerOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public NumberSpinnerOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 初始化 ComboTreeOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public NumberSpinnerOptions(EasyOptions dictionary) : base(dictionary) { }

		/// <summary>
		/// 组件宽度，默认值（auto）
		/// </summary>
		public int width { set { AddToDataOptions("width", value); } }

		/// <summary>
		/// 组件高度（该属性自1.3.2版开始可用），默认值（22）
		/// </summary>
		public int height { set { AddToDataOptions("height", value); } }

		/// <summary>
		/// 在点击微调按钮的时候的增量值，默认值（1）
		/// </summary>
		public int increment { set { AddToDataOptions("increment", value); } }

		/// <summary>
		/// 定义用户是否可以直接输入值到字段，默认值（true）
		/// </summary>
		public bool editable { set { AddToDataOptions("editable", value ? "true" : "false"); } }

		/// <summary>
		/// 定义控件是否为只读，默认值（false）
		/// </summary>
		public bool readOnly { set { AddToDataOptions("readonly", value ? "true" : "false"); } }
	}
}
