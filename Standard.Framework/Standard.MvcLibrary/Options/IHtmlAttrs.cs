using System.Collections.Generic;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 表示Html属性的键值对集合接口
	/// </summary>
	public interface IHtmlAttrs : IDictionary<string, object>
	{
		/// <summary>
		/// 设置class属性值
		/// </summary>
		string className { set; }

		/// <summary>
		/// 添加 Css 类
		/// </summary>
		/// <param name="className"></param>
		void AddCssClass(string className);
		///// <summary>
		///// 设置target属性值
		///// </summary>
		//string target { set; }

		///// <summary>
		///// 设置title属性值
		///// </summary>
		//string title { set; }

		///// <summary>
		///// 设置url属性值
		///// </summary>
		//string url { set; }

		///// <summary>
		///// 设置href属性值
		///// </summary>
		//string href { set; }

		///// <summary>
		///// 设置multiple属性值,一般用于Combo及其利用Combo控件的设定是否允许多选。
		///// </summary>
		//bool multiple { set; }

		///// <summary>
		///// 设置value属性值
		///// </summary>
		//object value { set; }

		///// <summary>
		///// 设置data-options属性值
		///// </summary>
		//string dataOptions { set; }

		///// <summary>
		///// 设置name属性值
		///// </summary>
		//string name { set; }

		/// <summary>
		/// 设置id属性值
		/// </summary>
		string id { set; }

		///// <summary>
		///// 设置alt属性值
		///// </summary>
		//string alt { set; }

		///// <summary>
		///// 设置disabled属性值
		///// </summary>
		//bool disabled { set; }

		///// <summary>
		///// 设置unselected-disabled属性值(此属性主要，在GridPage和TreeEdit的工具条按钮使用)
		///// </summary>
		//bool unselectedDisabled { set; }

		///// <summary>
		///// 设置unselected-visibled属性值(此属性主要，在GridPage和TreeEdit的工具条按钮使用)
		///// </summary>
		//bool unselectedVisibled { set; }

		///// <summary>
		///// 设置tree-edit-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定新增/修改状态时是否显示)
		///// </summary>
		//bool treeEditState { set; }

		///// <summary>
		///// 设置tree-normal-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定普通状态时是否显示)
		///// </summary>
		//bool treeNormalState { set; }

		///// <summary>
		///// 设置tree-selected-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定删除状态时是否显示)
		///// </summary>
		//bool treeSelectedState { set; }

		///// <summary>
		///// 设置tree-selected-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定删除状态时是否显示)
		///// </summary>
		//PageStatus visibledState { set; }

		///// <summary>
		///// 设置tree-selected-state属性值(此属性主要，在TreeEdit的工具条按钮使用,确定删除状态时是否显示)
		///// </summary>
		//PageStatus enabledState { set; }

		///// <summary>
		///// 设置enabled属性值
		///// </summary>
		//bool enabled { set; }

		///// <summary>
		///// 设置height属性值
		///// </summary>
		//int height { set; }

		///// <summary>
		///// 设置height属性值
		///// </summary>
		//int width { set; }

		///// <summary>
		///// 设置readOnly属性值
		///// </summary>
		//bool readOnly { set; }

		///// <summary>
		///// 设置onclick属性值
		///// </summary>
		//string onclick { set; }

		///// <summary>
		///// 设置style属性值
		///// </summary>
		//string style { set; }

		///// <summary>
		///// 设置onblur属性值
		///// </summary>
		//string onblur { set; }

		///// <summary>
		///// 设置onchange属性值
		///// </summary>
		//string onchange { set; }

		///// <summary>
		///// 设置ondblclick属性值
		///// </summary>
		//string ondblclick { set; }

		///// <summary>
		///// 设置tooltype属性值(工具条按钮属性)
		///// </summary>
		//OperatorModeEnum tooltype { set; }

		///// <summary>
		///// 设置onkeydown 属性值
		///// </summary>
		//string onkeydown { set; }

		///// <summary>
		///// 设置onfocus属性值
		///// </summary>
		//string onfocus { set; }

		///// <summary>
		///// 设置onpropertychange属性值
		///// </summary>
		//string onpropertychange { set; }

		///// <summary>
		///// 设置size属性值
		///// </summary>
		//int size { set; }
	}
}
