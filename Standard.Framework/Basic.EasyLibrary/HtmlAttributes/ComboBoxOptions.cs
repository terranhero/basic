using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 ComboBox 属性扩展。
	/// </summary>
	public sealed class ComboBoxOptions : ComboOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 ComboBoxOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public ComboBoxOptions() : base() { }

		/// <summary>
		/// 初始化 ComboBoxOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		internal ComboBoxOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 基础数据值名称绑定到该下拉列表框，默认值（value）
		/// </summary>
		public string valueField { set { AddToDataOptions("valueField", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 基础数据字段名称绑定到该下拉列表框，默认值（text）
		/// </summary>
		public string textField { set { AddToDataOptions("textField", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 指定分组的字段名称（分组的字段由数据源决定）。（该属性自1.3.4版开始可用），默认值（null）
		/// </summary>
		public string groupField { set { AddToDataOptions("groupField", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 定义了当文本改变时如何读取列表数据。设置为'remote'时，下拉列表框将会从服务器加载数据。
		/// 当设置为“remote”模式时，用户输入将被发送到名为'q'的HTTP请求参数到服务器检索新数据。默认值（local）
		/// </summary>
		public string mode { set { AddToDataOptions("mode", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 通过URL加载远程列表数据。默认值（null）
		/// </summary>
		public string url { set { AddToDataOptions("url", string.Concat("'", value, "'")); } }

		/// <summary>
		/// HTTP方法检索数据(POST / GET)。默认值（post）
		/// </summary>
		public string method { set { AddToDataOptions("method", string.Concat("'", value, "'")); } }
	}
}
