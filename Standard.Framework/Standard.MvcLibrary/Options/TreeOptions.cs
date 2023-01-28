using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 Tree 属性扩展。
	/// </summary>
	public sealed class TreeOptions : EasyOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 TreeOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public TreeOptions() : base() { }

		/// <summary>
		/// 初始化 TreeOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public TreeOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 初始化 ComboTreeOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public TreeOptions(EasyOptions dictionary) : base(dictionary) { }

		/// <summary>
		/// 检索远程数据的URL地址，默认值（null）
		/// </summary>
		public string url { set { AddToDataOptions("url", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 检索数据的HTTP方法，默认值（post）
		/// </summary>
		public string method { set { AddToDataOptions("method", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 定义节点在展开或折叠的时候是否显示动画效果，默认值（false）
		/// </summary>
		public bool animate { set { AddToDataOptions("animate", value ? "true" : "false"); } }

		/// <summary>
		/// 定义是否在每一个借点之前都显示复选框，默认值（false）
		/// </summary>
		public bool checkbox { set { AddToDataOptions("checkbox", value ? "true" : "false"); } }

		/// <summary>
		/// 定义是否层叠选中状态，默认值（true）
		/// </summary>
		public bool cascadeCheck { set { AddToDataOptions("cascadeCheck", value ? "true" : "false"); } }

		/// <summary>
		/// 定义是否只在末级节点之前显示复选框，默认值（true）
		/// </summary>
		public bool onlyLeafCheck { set { AddToDataOptions("onlyLeafCheck", value ? "true" : "false"); } }

		/// <summary>
		/// 定义是否只在末级节点之前显示复选框，默认值（true）
		/// </summary>
		public bool lines { set { AddToDataOptions("lines", value ? "true" : "false"); } }

		/// <summary>
		/// 定义是否启用拖拽功能，默认值（true）
		/// </summary>
		public bool dnd { set { AddToDataOptions("dnd", value ? "true" : "false"); } }
	}
}
