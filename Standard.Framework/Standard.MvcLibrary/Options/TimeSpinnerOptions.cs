using Basic.MvcLibrary;
using System.Collections.Generic;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 TimeSpinner 属性抽象定义。
	/// </summary>
	public class TimeSpinnerOptions : SpinnerOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 TimeSpinnerOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public TimeSpinnerOptions() : base() { }

		/// <summary>
		/// 初始化 TimeSpinnerOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public TimeSpinnerOptions(IDictionary<string, object> dictionary) : base(dictionary) { }
		/// <summary>
		/// 初始化 ComboTreeOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public TimeSpinnerOptions(EasyOptions dictionary) : base(dictionary) { }

		/// <summary>
		/// 定义在小时、分钟和秒之间的分隔符，默认值（:）
		/// </summary>
		public string separator { set { AddToDataOptions("separator", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 定义是否显示秒钟信息，默认值（false）
		/// </summary>
		public bool showSeconds { set { AddToDataOptions("showSeconds", value ? "true" : "false"); } }

		/// <summary>
		/// 初始选中的字段 0=小时,1=分钟，默认值（0）
		/// </summary>
		public int highlight { set { AddToDataOptions("highlight", value); } }
	}

	/// <summary>
	/// 表示 DateTimeSpinner 属性抽象定义。
	/// </summary>
	public class DtSpinnerOptions : TimeSpinnerOptions
	{
		/// <summary>
		///  初始化 DtSpinnerOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public DtSpinnerOptions() : base() { }

		/// <summary>
		/// 初始化 TimeSpinnerOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public DtSpinnerOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 选择高亮部分的值（该值必须设置正确，否则微调的时候会出问题）。
		/// </summary>
		public string[] selections
		{
			set
			{
				AddToDataOptions("separator", string.Concat("'[[", string.Join("],[", value), "]]'"));
			}
		}
	}
}
