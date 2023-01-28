using Basic.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Basic.MvcLibrary
{

	/// <summary>
	/// 表示Html属性的键值对集合的默认实现
	/// </summary>
	[Serializable]
	public sealed class HtmlAttrs : Dictionary<string, object>, IHtmlAttrs
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="htmlAttrs"></param>
		/// <returns></returns>
		public static HtmlAttrs Create(object htmlAttrs)
		{
			HtmlAttrs dictionary = new HtmlAttrs();
			if (htmlAttrs != null)
			{
				foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(htmlAttrs))
				{
					dictionary.Add(descriptor.Name.Replace('_', '-'), descriptor.GetValue(htmlAttrs));
				}
			}
			return dictionary;
		}

		/// <summary>
		///  初始化 HtmlAttributes 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public HtmlAttrs() : base() { }

		/// <summary>
		/// 初始化 HtmlAttributes 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public HtmlAttrs(IDictionary<string, object> dictionary) : base(dictionary) { }

		/// <summary>
		/// 初始化 HtmlAttributes 类的新实例，该实例为空且具有指定的初始容量，并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="capacity">HtmlAttributes 可包含的初始元素数。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">capacity 小于 0。</exception>
		public HtmlAttrs(int capacity) : base(capacity) { }

		/// <summary>
		/// 初始化 HtmlAttributes 类的新实例，该实例为空且具有指定的初始容量，并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="key"> 要添加的元素的键。</param>
		/// <param name="value">要添加的元素的值。对于引用类型，该值可以为 null。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">capacity 小于 0。</exception>
		public HtmlAttrs(string key, object value) : base(5) { base[key] = value; }

		/// <summary>
		/// 用序列化数据初始化 HtmlAttributes 类的新实例。
		/// </summary>
		/// <param name="info">一个 System.Runtime.Serialization.SerializationInfo 对象，它包含序列化 HtmlAttributes所需的信息。</param>
		/// <param name="context">System.Runtime.Serialization.StreamingContext 结构，该结构包含与 HtmlAttributes相关联的序列化流的源和目标。</param>
		private HtmlAttrs(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// 设置class属性值
		/// </summary>
		public string className { set { base["class"] = value; } }

		/// <summary>
		/// 设置 accept 属性值
		/// </summary>
		public string accept { set { base["accept"] = value; } }

		/// <summary>
		/// 添加 Css 类
		/// </summary>
		/// <param name="className"></param>
		public void AddCssClass(string className)
		{
			if (!base.ContainsKey("class")) { base["class"] = className; }
			else { base["class"] = string.Concat(base["class"], " ", className); }
		}

		/// <summary>
		/// 设置headerCls属性值
		/// </summary>
		public string headerCls { set { base["headerCls"] = value; } }

		/// <summary>
		/// 设置bodyCls属性值
		/// </summary>
		public string bodyCls { set { base["bodyCls"] = value; } }

		/// <summary>
		/// 设置target属性值
		/// </summary>
		public string target { set { base["target"] = value; } }

		/// <summary>
		/// 设置tools属性值
		/// </summary>
		public string tools { set { base["tools"] = value; } }

		/// <summary>
		/// 设置data-options属性值
		/// </summary>
		public string dataOptions { set { base["data-options"] = value; } }

		/// <summary>
		/// 设置iconCls属性值
		/// </summary>
		public string iconCls { set { base["iconCls"] = value; } }

		/// <summary>
		/// 设置animate属性值
		/// </summary>
		public bool animate { set { base["animate"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置fit属性值
		/// </summary>
		public bool fit { set { base["fit"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置url属性值
		/// </summary>
		public string url { set { base["url"] = value; } }

		/// <summary>
		/// 设置href属性值
		/// </summary>
		public string href { set { base["href"] = value; } }

		/// <summary>
		/// 设置multiple属性值,一般用于Combo及其利用Combo控件的设定是否允许多选。
		/// </summary>
		public bool multiple { set { base["multiple"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置title属性值
		/// </summary>
		public string title { set { base["title"] = value; } }

		/// <summary>
		/// 设置title属性值
		/// </summary>
		public bool border { set { base["border"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置name属性值
		/// </summary>
		public string name { set { base["name"] = value; } }

		/// <summary>
		/// 设置id属性值
		/// </summary>
		public string id { set { base["id"] = value; } }

		/// <summary>
		/// 设置alt属性值
		/// </summary>
		public string alt { set { base["alt"] = value; } }

		/// <summary>
		/// 设置disabled属性值
		/// </summary>
		public bool disabled { set { base["disabled"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置unselected-disabled属性值(此属性主要，在GridPage和TreeEdit的工具条按钮使用)
		/// </summary>
		public bool unselectedDisabled { set { disabled = value; base["unselected-disabled"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置unselected-visibled属性值(此属性主要，在GridPage和TreeEdit的工具条按钮使用)
		/// </summary>
		public bool unselectedVisibled { set { base["unselected-visibled"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置tree-edit-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定新增/修改状态时是否显示)
		/// </summary>
		public bool treeEditState { set { base["tree-edit-state"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置tree-normal-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定普通状态时是否显示)
		/// </summary>
		public bool treeNormalState { set { base["tree-normal-state"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置tree-selected-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定删除状态时是否显示)
		/// </summary>
		public bool treeSelectedState { set { base["tree-selected-state"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置tree-selected-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定删除状态时是否显示)
		/// </summary>
		public PageStatus visibledState { set { base["visibled-state"] = (int)value; } }

		/// <summary>
		/// 设置tree-selected-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定删除状态时是否显示)
		/// </summary>
		public PageStatus enabledState { set { base["enabled-state"] = (int)value; } }

		/// <summary>
		/// 设置enabled属性值
		/// </summary>
		public bool enabled { set { base["enabled"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置height属性值
		/// </summary>
		public int height { set { base["height"] = value; } }

		/// <summary>
		/// 设置size属性值
		/// </summary>
		public int size { set { base["size"] = value; } }

		/// <summary>
		/// 设置height属性值
		/// </summary>
		public int width { set { base["width"] = value; } }

		/// <summary>
		/// 设置readOnly属性值
		/// </summary>
		public bool readOnly { set { base["readOnly"] = value ? "true" : "false"; } }

		/// <summary>
		/// 设置onclick属性值
		/// </summary>
		public string onclick { set { base["onclick"] = value; } }

		/// <summary>
		/// 设置style属性值
		/// </summary>
		public string style { set { base["style"] = value; } }

		/// <summary>
		/// 设置onblur属性值
		/// </summary>
		public string onblur { set { base["onblur"] = value; } }

		/// <summary>
		/// 设置onchange属性值
		/// </summary>
		public string onchange { set { base["onchange"] = value; } }

		/// <summary>
		/// 设置ondblclick属性值
		/// </summary>
		public string ondblclick { set { base["ondblclick"] = value; } }

		/// <summary>
		/// 设置tooltype属性值(工具条按钮属性)
		/// </summary>
		public OperatorModeEnum tooltype { set { base["type"] = value; } }

		/// <summary>
		/// 设置onkeydown 属性值
		/// </summary>
		public string onkeydown { set { base["onkeydown"] = value; } }

		/// <summary>
		/// 设置onfocus属性值
		/// </summary>
		public string onfocus { set { base["onfocus"] = value; } }

		/// <summary>
		/// 设置value属性值
		/// </summary>
		public object value { set { base["value"] = value; } }

		/// <summary>
		/// 设置onpropertychange属性值
		/// </summary>
		public string onpropertychange { set { base["onpropertychange"] = value; } }

	}

}
