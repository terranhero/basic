
namespace Basic.MvcLibrary
{
	/// <summary>表示 el-table 列信息定义</summary>
	public interface IGridViewColumn : IOptions
	{
		/// <summary>设置字段名称</summary>
		IGridViewColumn Field(string field);

		/// <summary>设置 label 属性值</summary>
		/// <param name="label">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn Title(string label);

		/// <summary>设置Title属性值</summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn Title(string converterName, string name);

		/// <summary>用指定的长度初始化 System.Web.UI.WebControls.Unit 结构的新实例。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		IGridViewColumn Width(string width);

		/// <summary>用像素为高度的数字初始化 Width 的新值</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		IGridViewColumn Width(int width);

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		IGridViewColumn Width(bool condition, int tWidth, int fWidth);

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		IGridViewColumn Width(bool condition, string tWidth, string fWidth);

		/// <summary>对应列的最小宽度，与 width 的区别是 width 是固定的，min-width 会把剩余宽度按比例分配给设置了 min-width 的列。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		IGridViewColumn MinWidth(string width);

		/// <summary>对应列的最小宽度，与 width 的区别是 width 是固定的，min-width 会把剩余宽度按比例分配给设置了 min-width 的列</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		IGridViewColumn MinWidth(int width);

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		IGridViewColumn MinWidth(bool condition, int tWidth, int fWidth);

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		IGridViewColumn MinWidth(bool condition, string tWidth, string fWidth);

		/// <summary>设置 header-align 属性值靠左对齐</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn HeaderAlignToLeft();

		/// <summary>设置 header-align 属性值居中对齐</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn HeaderAlignToCenter();

		/// <summary>设置 header-align 属性值靠右对齐</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn HeaderAlignToRight();

		/// <summary>设置 align 属性值靠左对齐</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn AlignToLeft();

		/// <summary>设置 align 属性值居中对齐</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn AlignToCenter();

		/// <summary>设置 align 属性值靠右对齐</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn AlignToRight();

		/// <summary>设置 sortable 属性值</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn AllowSortable();

		/// <summary>对数据进行排序的时候使用的方法，仅当 sortable 设置为 true 的时候有效，需返回一个数字，和 Array.sort 表现一致</summary>
		/// <param name="function">列标题 Label 区域渲染使用的 function(a, b) </param>
		/// <example>function sortmethod(a, b)</example>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn SetSortMethod(string function);

		/// <summary>设置 resizable 属性值 </summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn AllowResizable();

		/// <summary>列固定在左侧。</summary>
		IGridViewColumn AllowFixed();

		/// <summary>列固定在右侧。</summary>
		IGridViewColumn FixedToRight();

		/// <summary>当内容过长被隐藏时显示 tooltip </summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn ShowOverflowTooltip();

		/// <summary>设置列的 render-header 属性值</summary>
		/// <param name="function">列标题 Label 区域渲染使用的 Function(h, { column, $index }) </param>
		/// <example>function renderHeader(h, { column, $index })</example>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn SetRenderHeader(string function);

		/// <summary>用来格式化内容</summary>
		/// <param name="function">用来格式化内容	function(row, column, cellValue, index) </param>
		/// <example>formatter: function(row, column, cellValue, index){ if (row){ return row.name; } else { return value; } }</example>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn SetFormatter(string function);

		/// <summary>仅对 type=selection 的列有效，类型为 Function，Function 的返回值用来决定这一行的 CheckBox 是否可以勾选</summary>
		/// <param name="function">用来格式化内容	function(row, index) </param>
		/// <example>selectable: function(row, index){}</example>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn SetSelectable(string function);

		/// <summary>列的 className	string</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn ClassName(string css);

		/// <summary>当前列标题的自定义类名(label-class-name)</summary>
		/// <returns>返回当前列对象。</returns>
		IGridViewColumn LabelClassName(string css);
	}

	/// <summary>表示 el-table 列信息定义</summary>
	/// <typeparam name="T">表示模型类型</typeparam>
	public interface IGridViewColumn<T> : IGridViewColumn
	{
	}

	/// <summary>表示 el-table 列信息定义</summary>
	/// <typeparam name="T">表示模型类型</typeparam>
	/// <typeparam name="TR">当前属性返回值类型</typeparam>
	public interface IGridViewColumn<T, TR> : IGridViewColumn<T>, IGridViewColumn
	{
		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		TR GetValue(T model);
	}
}
