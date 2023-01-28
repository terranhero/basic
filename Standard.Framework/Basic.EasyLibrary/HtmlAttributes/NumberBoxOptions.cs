using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 NumberBox 属性扩展。
	/// </summary>
	public class NumberBoxOptions : EasyOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 NumberBoxOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public NumberBoxOptions() : base() { }

		/// <summary>
		/// 初始化 NumberBoxOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public NumberBoxOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 控件默认值。
		/// </summary>
		public decimal value { set { AddToDataOptions("value", value); } }

		/// <summary>
		/// 允许的最小值，默认值（null）
		/// </summary>
		public decimal min { set { AddToDataOptions("min", value); } }

		/// <summary>
		/// 允许的最大值，默认值（null）
		/// </summary>
		public decimal max { set { AddToDataOptions("max", value); } }

		/// <summary>
		/// 在十进制分隔符之后显示的最大精度（即小数点后的显示精度）。默认值（0）
		/// </summary>
		public int precision { set { AddToDataOptions("precision", value); } }

		/// <summary>
		/// 使用哪一种十进制字符分隔数字的整数和小数部分，默认值（.）
		/// </summary>
		public string decimalSeparator { set { AddToDataOptions("decimalSeparator", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 使用哪一种字符分割整数组，以显示成千上万的数据。(比如：99,999,999.00中的','就是该分隔符设置。)
		/// </summary>
		public string groupSeparator { set { AddToDataOptions("groupSeparator", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 前缀字符。(比如：金额的$或者￥)
		/// </summary>
		public string prefix { set { AddToDataOptions("prefix", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 后缀字符。(比如：后置的欧元符号€)
		/// </summary>
		public string suffix { set { AddToDataOptions("suffix", string.Concat("'", value, "'")); } }
	}
}
