using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SWUW = System.Web.UI.WebControls;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示LinkButton 属性扩展。
	/// </summary>
	public sealed class ButtonOptions : EasyOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 ButtonOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public ButtonOptions() : this(ButtonTypeEnum.None) { }

		/// <summary>
		/// 初始化 ButtonOptions 类的新实例。
		/// </summary>
		/// <param name="buttonType">指定使用的样式名称</param>
		public ButtonOptions(ButtonTypeEnum buttonType) : base() { this.className = "easyui-linkbutton"; this.ButtonType = buttonType; }

		/// <summary>
		/// 初始化 ButtonOptions 类的新实例。
		/// </summary>
		/// <param name="styleName">指定使用的样式名称</param>
		public ButtonOptions(string styleName) : base() { this.className = styleName; this.className = "easyui-linkbutton"; }

		/// <summary>
		/// 初始化 ButtonOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public ButtonOptions(EasyOptions dictionary) : base(dictionary) { this.className = "easyui-linkbutton"; }

		/// <summary>
		/// 初始化 ButtonOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public ButtonOptions(IDictionary<string, object> dictionary) : base(dictionary) { this.className = "easyui-linkbutton"; }

		/// <summary>
		/// 初始化 ButtonOptions 类的新实例，该实例为空且具有指定的初始容量，并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="key"> 要添加的元素的键。</param>
		/// <param name="value">要添加的元素的值。对于引用类型，该值可以为 null。</param>
		public ButtonOptions(string key, object value)
			: base()
		{
			this.className = "easyui-linkbutton";
			if (key == "search-for") { this.searchfor = Convert.ToString(value); }
			else { base[key] = value; }
		}

		/// <summary>
		/// 为true时允许用户切换其状态是被选中还是未选择，可实现checkbox复选效果。
		/// </summary>
		public bool toggle { set { AddToDataOptions("toggle", value ? "true" : "false"); } }

		/// <summary>
		/// 定义按钮初始的选择状态，true为被选中，false为未选中。
		/// </summary>
		public bool selected { set { AddToDataOptions("selected", value ? "true" : "false"); } }

		/// <summary>
		/// 指定相同组名称的按钮同属于一个组，可实现radio单选效果。
		/// </summary>
		public string group { set { AddToDataOptions("group", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 为true时显示简洁效果。
		/// </summary>
		internal bool plain { set { AddToDataOptions("plain", value ? "true" : "false"); } }

		/// <summary>
		/// 为true时显示简洁效果。
		/// </summary>
		public string text { set { AddToDataOptions("text", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 显示在按钮文字左侧的图标(16x16)的CSS类ID。
		/// </summary>
		public string iconCls { set { AddToDataOptions("iconCls", string.Concat("'", value, "'")); } }

		private ButtonTypeEnum mButtonType = ButtonTypeEnum.None;
		/// <summary>显示按钮类型。</summary>
		internal ButtonTypeEnum ButtonType { set { AddToDataOptions("buttonType", (int)value); mButtonType = value; } get { return mButtonType; } }

		/// <summary>
		/// 按钮图标位置。
		/// 可用值有：'left','right'（该属性自1.3.2版开始可用）
		/// 可用值有：'top','bottom'（该属性自1.3.6版开始可用）
		/// </summary>
		public string iconAlign { set { AddToDataOptions("iconAlign", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 按钮大小。可用值有：'small','large'。（该属性自1.3.6版开始可用） 
		/// </summary>
		public string size { set { AddToDataOptions("size", value); } }

		/// <summary>
		/// 设置tree-selected-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定删除状态时是否显示)
		/// </summary>
		public PageStatus visibledState { set { AddToDataOptions("visibledState", (int)value); } }

		/// <summary>
		/// 设置tree-selected-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定删除状态时是否显示)
		/// </summary>
		public PageStatus enabledState { set { AddToDataOptions("enabledState", (int)value); } }

		/// <summary>
		/// 设置width属性值
		/// </summary>
		public SWUW.Unit width { set { AddToDataOptions("dWidth", string.Concat("'", value.ToString(), "'")); } }

		/// <summary>
		/// 设置height属性值
		/// </summary>
		public SWUW.Unit height { set { AddToDataOptions("dHeight", string.Concat("'", value.ToString(), "'")); } }

		/// <summary>
		/// 设置url属性值
		/// </summary>
		public string url { set { AddToDataOptions("actionUrl", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 设置searchfor属性值
		/// </summary>
		public string searchfor { set { AddToDataOptions("searchfor", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 设置href属性值
		/// </summary>
		public string href { set { base["href"] = value; } }

		/// <summary>
		/// 设置title属性值
		/// </summary>
		public string title { set { base["title"] = value; } }
	}
}
