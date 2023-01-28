using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 ComboGrid 属性扩展。
	/// </summary>
	public sealed class ComboGridOptions : ComboOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 ComboGridOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public ComboGridOptions() : base() { }

		/// <summary>
		/// 初始化 ComboGridOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public ComboGridOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// ID字段名称，默认值（null）
		/// </summary>
		public string idField { set { AddToDataOptions("idField", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 要显示在文本框中的文本字段，默认值（null）
		/// </summary>
		public string textField { set { AddToDataOptions("textField", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 在数据表格加载远程数据的时候显示消息，默认值（null）
		/// </summary>
		public string loadMsg { set { AddToDataOptions("loadMsg", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 定义在文本改变的时候如何读取数据网格数据。设置为'remote'，数据表格将从远程服务器加载数据。
		/// 当设置为'remote'模式的时候，用户输入将会发送到名为'q'的http请求参数，向服务器检索新的数据。默认值（local）
		/// </summary>
		public string mode { set { AddToDataOptions("mode", string.Concat("'", value, "'")); } }
	}
}
