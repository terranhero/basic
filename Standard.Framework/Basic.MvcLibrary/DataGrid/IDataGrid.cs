using System;
using System.Linq.Expressions;
using System.Web;

using Basic.Interfaces;
using Basic.MvcLibrary;
using Basic.EntityLayer;
using Basic.Enums;
using System.Collections.Generic;

namespace Basic.EasyLibrary
{
	/// <summary>表示DataGrid接口</summary>
	public interface IDataGrid : IDisposable
	{
		/// <summary>表示当前请求上下文信息</summary>
		IBasicContext Context { get; }
	}

	/// <summary>
	/// 表示生成EasyUI.DataGrid类型表格的接口
	/// </summary>
	/// <typeparam name="T">模型实体类型</typeparam>
	public interface IDataGrid<T> : IDataGrid where T : class
	{
		/// <summary>
		/// Defines which column can be sorted.
		/// Defines the column sort order, can only be 'asc' or 'desc'. asc 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="sortDirection">排序方向</param>
		/// <returns></returns>
		IDataGrid<T> Sort(Expression<Func<T, object>> expression, SortDirection sortDirection);

		/// <summary>
		/// 使用Lambda表达式设置标题字段。
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		IDataGrid<T> CaptionFor(string expression);

		/// <summary>
		/// 使用Lambda表达式设置标题字段。
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="localization">是否使用本地化资源</param>
		/// <returns></returns>
		IDataGrid<T> CaptionFor(string expression, bool localization);

		/// <summary>使用 Lambda 表达式设置标题字段。</summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		IDataGrid<T> CaptionFor(string converterName, string name);

		/// <summary>
		/// 使用Lambda表达式指定主键字段。 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		IDataGrid<T> IdFieldFor<TR>(Expression<Func<T, TR>> expression);

		/// <summary>
		/// 使用Lambda表达式指定时间戳字段。 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		IDataGrid<T> TsFieldFor<TR>(Expression<Func<T, TR>> expression);

		/// <summary>生成数组列头</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridHeaderColumn<T> HeaderFor<TR>(Expression<Func<T, IEnumerable<PropertyMeta<TR>>>> expression)
			 where TR : struct, IComparable<TR>, IEquatable<TR>;

		/// <summary>生成数组列头</summary>
		/// <param name="fields">包含列头集合。</param>
		/// <returns></returns>
		IDictionary<string, DataGridHeaderColumn<T>> HeaderFor(IDictionary<string, IDictionary<string, string>> fields);

		/// <summary>
		/// 使用Lambda表达式设置标题字段。
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridHeaderColumn<T> HeaderFor(string expression);

		/// <summary>
		/// 使用Lambda表达式设置标题字段。
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="localization">是否使用本地化资源</param>
		/// <returns></returns>
		DataGridHeaderColumn<T> HeaderFor(string expression, bool localization);

		/// <summary>使用 Lambda 表达式设置标题字段。</summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		DataGridHeaderColumn<T> HeaderFor(string converterName, string name);

		/// <summary>设置 CssClass 属性值</summary>
		/// <param name="cssName">样式class名称</param>
		/// <returns>返回当前列对象。</returns>
		DataGridRow<T> SetCurrentRowClass(string cssName);

		/// <summary>
		/// <![CDATA[将对象添加到 DataGridRow<T> 的结尾处。]]>
		/// </summary>
		/// <returns><![CDATA[返回添加成功的 DataGridRow<T> 对象实例。]]></returns>
		DataGridRow<T> CreateRow();

		/// <summary>
		/// <![CDATA[获取一个 DataGridRow<T>对象,该对象表示当前行。]]>
		/// </summary>
		/// <returns><![CDATA[一个 DataGridRow<T>对象,该对象表示当前行。]]></returns>
		DataGridRow<T> GetCurrentRow();

		/// <summary>设置行号列</summary>
		/// <returns>返回行号列对象。</returns>
		NumberColumn<T> RowNumberFor();

		/// <summary>设置行号列</summary>
		/// <param name="startNumber">表示行号起始值</param>
		/// <returns>返回行号列对象。</returns>
		NumberColumn<T> RowNumberFor(int startNumber);

		/// <summary>设置行号列</summary>
		/// <returns>返回行号列对象。</returns>
		ButtonColumn<T> ButtonsFor(Func<T, string> expression);

		/// <summary>使用 Lambda 表达式设置行选择字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, RT> SelectFor<RT>(Expression<Func<T, RT>> expression);

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, RT> LabelFor<RT>(Expression<Func<T, RT>> expression);

		/// <summary>是否启用本地化表达式</summary>
		/// <param name="localization">是否使用本地化表达式</param>
		/// <param name="expression1">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="expression2">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, RT> LabelFor<RT>(bool localization, Expression<Func<T, RT>> expression1, Expression<Func<T, RT>> expression2);

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, RT> LabelFor<RT>(DataGridRow<T> row, Expression<Func<T, RT>> expression);

		/// <summary>生成后台Json对象属性，使用此方法，系统不生成表格列。 </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, TR> JsonFor<TR>(Expression<Func<T, TR>> expression);

		/// <summary>使用 Lambda 表达式创建枚举类型列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridEnumColumn<T> EnumFor(Expression<Func<T, Enum>> expression);

		/// <summary>使用 Lambda 表达式创建枚举类型列字段。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridEnumColumn<T> EnumFor(DataGridRow<T> row, Expression<Func<T, Enum>> expression);

		/// <summary>使用 Lambda 表达式创建日期类型列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, DateTime> DateFor(Expression<Func<T, DateTime>> expression);

		/// <summary>使用 Lambda 表达式创建日期类型列字段。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, DateTime> DateFor(DataGridRow<T> row, Expression<Func<T, DateTime>> expression);

		/// <summary>使用 Lambda 表达式创建日期类型列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, DateTime?> DateFor(Expression<Func<T, DateTime?>> expression);

		/// <summary>使用 Lambda 表达式创建日期类型列字段。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, DateTime?> DateFor(DataGridRow<T> row, Expression<Func<T, DateTime?>> expression);

		/// <summary>使用 Lambda 表达式创建布尔类型列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, bool> BooleanFor(Expression<Func<T, bool>> expression);

		/// <summary>使用 Lambda 表达式创建布尔类型列字段。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		DataGridColumn<T, bool> BooleanFor(DataGridRow<T> row, Expression<Func<T, bool>> expression);

		/// <summary>生成数组列</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="fields"></param>
		/// <returns></returns>
		DataGridArrayColumn<T, TR> ArrayFor<TR>(Expression<Func<T, IEnumerable<PropertyMeta<TR>>>> expression,
		  IDictionary<string, string> fields) where TR : struct, IComparable<TR>, IEquatable<TR>;

		/// <summary>生成数组列</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="fields">表</param>
		/// <returns></returns>
		DataGridArraysColumn<T, TR> ArraysFor<TR>(Expression<Func<T, IEnumerable<PropertyMeta<TR>>>> expression,
		  IDictionary<string, IDictionary<string, string>> fields) where TR : struct, IComparable<TR>, IEquatable<TR>;

		/// <summary>创建选择列。 </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		GroupInfo<T> GroupBy(Expression<Func<T, object>> expression);

		/// <summary>创建页脚。 </summary>
		/// <returns></returns>
		FooterInfo<T> CreateFooter();

		/// <summary>
		/// 获取DataGrid 分组信息
		/// </summary>
		/// <returns></returns>
		GroupInfo<T> GetGroupInfo();

		/// <summary>
		/// 获取DataGrid 页脚信息
		/// </summary>
		/// <returns></returns>
		FooterInfo<T> GetFooterInfo();

		/// <summary>
		/// 自定义行输出对象（此方法仅限于输出HTML格式）。
		/// </summary>
		void SetRowTemplate(Func<T, string> expression);

		/// <summary>
		/// 将网格呈现到创建时指定的HtmlTextWriter
		/// </summary>
		void Render(IPagination<T> dataSource);

		/// <summary>
		/// 将网格呈现到创建时指定的HtmlTextWriter
		/// </summary>
		void RenderTable(IPagination<T> dataSource);

		/// <summary>
		/// 将 DataGrid 以OpenXml.Spreadsheet对象的形式输出到客户端。
		/// </summary>
		void RenderExcel(IPagination<T> dataSource);

		/// <summary>
		/// 将 DataGrid 以Json对象的形式输出到客户端。
		/// </summary>
		void RenderJson(IPagination<T> dataSource);
	}
}
