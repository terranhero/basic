using Basic.MvcLibrary;
using System.Collections.Generic;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 Combo 属性扩展。
	/// </summary>
	public class ComboOptions : EasyOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 ComboOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public ComboOptions() : base() { }

		/// <summary>
		/// 初始化 ComboOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public ComboOptions(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 设置width属性值
		/// </summary>
		public int width { set { AddToDataOptions("width", value); } }

		/// <summary>
		/// 设置height属性值
		/// </summary>
		public int height { set { AddToDataOptions("height", value); } }

		/// <summary>
		/// 设置panelWidth属性值
		/// </summary>
		public int panelWidth { set { AddToDataOptions("panelWidth", value); } }

		/// <summary>
		/// 设置panelHeight属性值
		/// </summary>
		public int panelHeight { set { AddToDataOptions("panelHeight", value); } }

		/// <summary>
		/// 设置panelMinWidth属性值
		/// </summary>
		public int panelMinWidth { set { AddToDataOptions("panelMinWidth", value); } }

		/// <summary>
		/// 设置panelMaxWidth属性值
		/// </summary>
		public int panelMaxWidth { set { AddToDataOptions("panelMaxWidth", value); } }

		/// <summary>
		/// 设置panelMinHeight属性值
		/// </summary>
		public int panelMinHeight { set { AddToDataOptions("panelMinHeight", value); } }

		/// <summary>
		/// 设置panelMaxHeight属性值
		/// </summary>
		public int panelMaxHeight { set { AddToDataOptions("panelMaxHeight", value); } }

		/// <summary>
		/// 定义是否支持多选。
		/// </summary>
		public bool multiple { set { AddToDataOptions("multiple", value ? "true" : "false"); } }

		/// <summary>
		/// 定义是否允许使用键盘导航来选择项目。（该属性自1.3.3版开始可用）
		/// </summary>
		public bool selectOnNavigation { set { AddToDataOptions("selectOnNavigation", value ? "true" : "false"); } }

		/// <summary>
		/// 在多选的时候使用何种分隔符进行分割
		/// </summary>
		public string separator { set { AddToDataOptions("separator", string.Concat("'", value, "'")); } }

		/// <summary>
		/// 定义用户是否可以直接输入文本到字段中
		/// </summary>
		public bool editable { set { AddToDataOptions("editable", value ? "true" : "false"); } }

		/// <summary>
		/// 设置readonly属性值
		/// </summary>
		public bool readOnly { set { AddToDataOptions("readonly", value ? "true" : "false"); } }

		/// <summary>
		/// 定义是否显示向下箭头按钮
		/// </summary>
		public bool hasDownArrow { set { AddToDataOptions("hasDownArrow", value ? "true" : "false"); } }

		/// <summary>当字段值改变的时候触发</summary>
		public string onChange { set { AddToDataOptions("onChange", value); } }
	}
}
