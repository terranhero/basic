
namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 DataGrid 列信息定义
	/// </summary>
	public interface IDataGridColumn
	{
		/// <summary>设置字段名称</summary>
		IDataGridColumn SetField(string field);

		/// <summary>
		/// 设置Title属性值
		/// </summary>
		/// <param name="title">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetTitle(string title);

		/// <summary>
		/// 设置Title属性值
		/// </summary>
		/// <param name="name">要设置的属性新值</param>
		/// <param name="localization">是否使用本地化资源</param>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetTitle(string name, bool localization);

		/// <summary>
		/// 设置Title属性值
		/// </summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetTitle(string converterName, string name);

		/// <summary>
		/// 设置Width属性值
		/// </summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetWidth(float width);

		/// <summary>用指定的长度初始化 System.Web.UI.WebControls.Unit 结构的新实例。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		IDataGridColumn SetWidth(string width);

		/// <summary>用像素为高度的数字初始化 Width 的新值</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		IDataGridColumn SetWidth(int width);

		/// <summary>
		/// 设置Rowspan属性值
		/// </summary>
		/// <param name="rowspan">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetRowspan(int rowspan);

		/// <summary>
		/// 设置Colspan属性值
		/// </summary>
		/// <param name="colspan">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetColspan(int colspan);

		/// <summary>
		/// 设置 halign 属性值靠左对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn HeaderAlignToLeft();

		/// <summary>
		/// 设置 halign 属性值居中对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn HeaderAlignToCenter();

		/// <summary>
		/// 设置 halign 属性值靠右对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn HeaderAlignToRight();

		/// <summary>
		/// 设置 align 属性值靠左对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn AlignToLeft();

		/// <summary>
		/// 设置 align 属性值居中对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn AlignToCenter();

		/// <summary>
		/// 设置 align 属性值靠右对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn AlignToRight();

		/// <summary>
		/// 设置Sortable属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn AllowSort();

		/// <summary>
		/// 设置Resizable属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn AllowResize();

		/// <summary>
		/// 设置Hidden属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn Hide();

		/// <summary>获取或设置当前列是否需要冻结在表格左边。</summary>
		IDataGridColumn AllowFrozen();

		/// <summary>设置当前列是否固定列宽度。</summary>
		IDataGridColumn AllowFixed();

		/// <summary>
		/// 设置Checkbox属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn AllowCheck();

		/// <summary>设置Format属性值</summary>
		/// <param name="format">格式化字符串，如{0:yyyy-MM-dd}</param>
		/// <example>
		/// $('#dg').datagrid({columns:[[{field:'userId',title:'User', width:80,
		/// formatter: function(value,row,index){if (row.user){return row.user.name;} else {return value;}}}]]});
		/// </example>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetFormat(string format);

		/// <summary>设置列的 formatter 属性值</summary>
		/// <param name="formatter">单元格formatter(格式化器)函数，带3个参数：value：字段值。row：行记录数据。index: 行索引。 </param>
		/// <example>formatter: function(value,row,index){ if (row){ return row.name; } else { return value; } }</example>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetFormatter(string formatter);

		/// <summary>
		/// 设置数据单元格样式名称 属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn SetCssClass(string styler);

		/// <summary>
		/// 设置 AllowToHtml 属性值为 false。
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn NotToHtml();

		/// <summary>
		/// 设置 AllowExport 属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		IDataGridColumn NotExport();

	}

	/// <summary>
	/// 表示 DataGrid 列信息定义
	/// </summary>
	/// <typeparam name="T">表示模型类型</typeparam>
	/// <typeparam name="TR">当前属性返回值类型</typeparam>
	public interface IDataGridColumn<T, TR> : IDataGridColumn
	{
		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		TR GetModelValue(T model);
	}
}
