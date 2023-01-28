using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Basic.EntityLayer;
using Basic.MvcLibrary;
using Unit = Basic.MvcLibrary.Unit;
using UnitType = Basic.MvcLibrary.UnitType;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 DataGrid列信息定义
	/// </summary>
	public abstract class DataGridColumn<T> : IDataGridColumn
	{
		internal readonly IBasicContext _Context;
		private readonly List<string> cssClasses = new List<string>();
		/// <summary>
		/// 初始化 DataGridColumn 类实例
		/// </summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		protected DataGridColumn(IBasicContext context)
		{
			_Context = context;
			AllowExport = true; AllowToHtml = true; Fixed = false; Frozen = false;
			HeaderAlignToCenter(); AlignToCenter();
			Sortable = false; JsonData = false;
		}

		/// <summary>设置字段名称</summary>
		public IDataGridColumn SetField(string field) { Field = field; return this; }

		/// <summary>
		/// 设置Title属性值
		/// </summary>
		/// <param name="title">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn SetTitle(string title) { return this.SetTitle(title, false); }

		/// <summary>
		/// 设置Title属性值
		/// </summary>
		/// <param name="name">要设置的属性新值</param>
		/// <param name="localization">是否使用本地化资源</param>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn SetTitle(string name, bool localization)
		{
			Title = name;
			if (localization)
			{
				string title = _Context.GetString(name);
				if (!string.IsNullOrEmpty(title)) { Title = title; }
			}
			return this;
		}

		/// <summary>
		/// 设置Title属性值
		/// </summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn SetTitle(string converterName, string name)
		{
			Title = name;
			string title = _Context.GetString(converterName, name);
			if (!string.IsNullOrEmpty(title)) { Title = title; }
			return this;
		}
		/// <summary>获取当前列宽度。</summary>
		public Unit Width { get; internal set; }

		/// <summary>用厘米为高度的数字初始化 Height 的新值。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public IDataGridColumn SetWidth(float width) { Width = new Unit(width, UnitType.Cm); return this; }

		/// <summary>用指定的长度初始化 System.Web.UI.WebControls.Unit 结构的新实例。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public IDataGridColumn SetWidth(string width) { Width = new Unit(width); return this; }

		/// <summary>用像素为高度的数字初始化 Width 的新值</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public IDataGridColumn SetWidth(int width) { Width = new Unit(width); return this; }

		/// <summary>
		/// 设置Rowspan属性值
		/// </summary>
		/// <param name="rowspan">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn SetRowspan(int rowspan) { Rowspan = rowspan; return this; }

		/// <summary>
		/// 设置Colspan属性值
		/// </summary>
		/// <param name="colspan">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn SetColspan(int colspan) { Colspan = colspan; return this; }

		/// <summary>
		/// 设置 halign 属性值靠左对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn HeaderAlignToLeft() { HeaderAlign = "left"; return this; }

		/// <summary>
		/// 设置 halign 属性值居中对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn HeaderAlignToCenter() { HeaderAlign = "center"; return this; }

		/// <summary>
		/// 设置 halign 属性值靠右对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn HeaderAlignToRight() { HeaderAlign = "right"; return this; }

		/// <summary>
		/// 设置 align 属性值靠左对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn AlignToLeft() { Align = "left"; return this; }

		/// <summary>
		/// 设置 align 属性值居中对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn AlignToCenter() { Align = "center"; return this; }

		/// <summary>
		/// 设置 align 属性值靠右对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn AlignToRight() { Align = "right"; return this; }

		/// <summary>
		/// 设置Sortable属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn AllowSort() { Sortable = true; return this; }

		/// <summary>
		/// 设置Resizable属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn AllowResize() { Resizable = true; return this; }

		/// <summary>
		/// 设置Hidden属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn Hide() { Hidden = true; return this; }

		/// <summary>设置当前列是否固定列宽度。</summary>
		public IDataGridColumn AllowFixed() { Fixed = true; return this; }

		/// <summary>获取或设置当前列是否需要冻结在表格左边。</summary>
		public IDataGridColumn AllowFrozen() { Frozen = true; return this; }

		/// <summary>
		/// 设置Checkbox属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn AllowCheck() { Checkbox = true; return NotExport(); }

		/// <summary>设置Format属性值</summary>
		/// <param name="format">格式化字符串，如{0:yyyy-MM-dd}</param>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn SetFormat(string format) { Format = format; return this; }

		/// <summary>设置列的 formatter 属性值</summary>
		/// <param name="formatter">单元格formatter(格式化器)函数，带3个参数：value：字段值。row：行记录数据。index: 行索引。 </param>
		/// <example>formatter: function(value,row,index){ if (row){ return row.name; } else { return value; } }</example>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn SetFormatter(string formatter) { Formatter = formatter; return this; }

		/// <summary>
		/// 设置数据单元格样式名称 属性值
		/// </summary>
		/// <param name="calssName">设置当前单元格样式</param>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn SetCssClass(string calssName)
		{
			if (cssClasses.Contains(calssName) == false) { cssClasses.Add(calssName); }
			return this;
		}

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public abstract string GetString(T model);

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public abstract object GetValue(T model);


		#region 输出属性定义

		/// <summary>
		/// 设置 AllowToHtml 属性值为 false。
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn NotToHtml() { AllowToHtml = false; return this; }

		/// <summary>
		/// 是否允许导出.
		/// </summary>
		internal protected bool AllowToHtml { get; internal set; }

		/// <summary>
		/// 设置 AllowExport 属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public IDataGridColumn NotExport() { AllowExport = false; return this; }

		#endregion

		#region 列属性

		/// <summary>是否允许导出.</summary>
		public bool AllowExport { get; internal set; }

		/// <summary>获取当前列标题文本。</summary>
		public string Title { get; internal set; }

		/// <summary>获取当前列数据属性名称.</summary>
		internal protected string Field { get; internal set; }

		/// <summary>判断当前列是否有字段名属性，即 Field 属性不为空。</summary>
		public bool HasField { get { return string.IsNullOrWhiteSpace(Field) == false; } }

		/// <summary>获取或设置当前列是否固定列。</summary>
		internal protected bool Fixed { get; internal set; }

		/// <summary>获取或设置当前列是否需要冻结在表格左边。</summary>
		internal protected bool Frozen { get; internal set; }


		/// <summary>
		/// 获取当前列是否允许输出模型属性值。
		/// </summary>
		public bool JsonData { get; internal set; }

		/// <summary>
		/// Indicate how many rows a cell should take up. 
		/// </summary>
		/// <value>default value is "0"</value>
		public int Rowspan { get; internal set; }

		/// <summary>
		/// Indicate how many columns a cell should take up.
		/// </summary>
		/// <value>default value is "1"</value>
		public int Colspan { get; internal set; }

		/// <summary>
		/// 当前列的索引.
		/// </summary>
		/// <value>default value is "1"</value>
		public int ColumnIndex { get; internal set; }

		/// <summary>
		/// Indicate how to align the column data. 'left','right','center' can be used.
		/// </summary>
		/// <value>default value is "TextAlign.Left"</value>
		public string Align { get; internal set; }

		/// <summary>
		/// Indicate how to align the column data. 'left','right','center' can be used.
		/// </summary>
		/// <value>default value is "TextAlign.Left"</value>
		public string HeaderAlign { get; internal set; }

		/// <summary>
		/// True to hide the column.
		/// </summary>
		/// <value>default value is "false"</value>
		public bool Hidden { get; internal set; }

		/// <summary>
		/// True to allow the column can be sorted.
		/// </summary>
		/// <value>default value is "false"</value>
		internal protected bool Sortable { get; internal set; }

		/// <summary>
		/// True to allow the column can be resized.
		/// </summary>
		/// <value>default value is "false"</value>
		internal protected bool Resizable { get; internal set; }

		/// <summary>
		/// True to show a checkbox. The checkbox column has fixed width.
		/// </summary>
		/// <value>default value is "false"</value>
		public bool Checkbox { get; internal set; }

		/// <summary>formatter: function(value,row,index){ if (row){ return row.name; } else { return value; } }</summary>
		/// <value>返回当前列对象。</value>
		public string Formatter { get; internal set; }

		/// <summary>
		/// 格式化字符串，如{0:yyyy-MM-dd}
		/// </summary>
		/// <value>返回当前列对象。</value>
		public string Format { get; internal set; }

		/// <summary>获取数据单元格样式名称</summary>
		public string CssClass { get { return string.Join(" ", cssClasses.ToArray()); } }

		/// <summary>获取当前数据单元格样式名称是否为空。</summary>
		public bool HasCssClass { get { return cssClasses.Count > 0; } }

		/// <summary>获取当前数据单元格样式名称是否为空。</summary>
		public bool HasClass(string className)
		{
			return cssClasses.Contains(className);
		}

		#endregion
	}

	/// <summary>
	/// 表示 DataGrid列信息定义
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TR"></typeparam>
	public class DataGridColumn<T, TR> : DataGridColumn<T>, IDataGridColumn<T, TR> where T : class
	{
		/// <summary>
		/// 获取当前列的计算返回值的委托。
		/// </summary>
		internal readonly Func<T, TR> _ValueFunc;
		internal readonly EntityPropertyMeta _PropertyMeta;
		internal Func<IEnumerable<T>, TR> _AggFunc;
		/// <summary>
		/// 初始化 DataGridColumn 类实例
		/// </summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		/// <param name="field">当前列字段名或属性名</param>
		/// <param name="valueFunc">目标值计算公式(从一个表达式，标识包含要呈现的属性的对象)</param>
		/// <param name="metaData">当前列对应的属性元数据</param>
		public DataGridColumn(IBasicContext context, string field, Func<T, TR> valueFunc, EntityPropertyMeta metaData)
			: base(context) { Field = field; _ValueFunc = valueFunc; _PropertyMeta = metaData; }

		/// <summary>
		/// <![CDATA[计算序列IEnumerable<T>的聚合。]]>
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public DataGridColumn<T, TR> Grouping(Func<IEnumerable<T>, TR> aggFunc)
		{
			_AggFunc = aggFunc; return this;
		}

		private Func<TR, string> _GetValue;
		/// <summary>获取当前列实体类的值</summary>
		/// <param name="expression">获取对象属性的属性值</param>
		/// <returns>返回模型的值</returns>
		public DataGridColumn<T, TR> SetPropertyValueFor(Expression<Func<TR, string>> expression)
		{
			_GetValue = expression.Compile(); return this;
		}

		/// <summary>获取当前列实体类的值</summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public string GetPropertyValue(T model)
		{
			if (_GetValue == null) { return null; }
			TR value = _ValueFunc(model);
			if (value == null) { return null; }
			return _GetValue(value);
		}

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public TR GetModelValue(T model) { return _ValueFunc(model); }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override object GetValue(T model) { return _ValueFunc(model); }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override string GetString(T model)
		{
			TR result = _ValueFunc((T)model);
			if (!string.IsNullOrWhiteSpace(this.Format))
			{
				return string.Format(System.Globalization.CultureInfo.CurrentCulture, Format, result);
			}
			else if (result is DateTime) { return GetDateTimeString(Convert.ToDateTime(result)); }
			else if (result is Nullable<DateTime>)
			{
				if (result == null) { return string.Empty; }
				return GetDateTimeString(Convert.ToDateTime(result));
			}
			else if (result is TimeSpan) { return GetTimeString((TimeSpan)Convert.ChangeType(result, typeof(TimeSpan))); }
			else if (result is Nullable<TimeSpan>)
			{
				if (result == null) { return string.Empty; }
				return GetTimeString((TimeSpan)Convert.ChangeType(result, typeof(TimeSpan)));
			}
			else if (result != null && result is string)
			{
				return Convert.ToString(result, System.Globalization.CultureInfo.CurrentCulture);
			}
			else if (result != null && result.GetType().IsClass)
			{
				return JsonSerializer.SerializeObject(result, true);
			}
			return Convert.ToString(result, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// 获取日期时间格式化返回值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		internal string GetDateTimeString(DateTime value)
		{
			if (value == DateTime.MinValue || value == DateTimeConverter.MinValue)
				return string.Empty;
			else if (!string.IsNullOrWhiteSpace(_PropertyMeta.DisplayFormatString))
				return string.Format(_PropertyMeta.DisplayFormatString, value);
			else if (value.Hour > 0 || value.Minute > 0 || value.Second > 0)
				return string.Format("{0:yyyy-MM-dd HH:mm:ss}", value);
			else
				return string.Format("{0:yyyy-MM-dd}", value);
		}

		/// <summary>
		/// 获取日期时间格式化返回值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		internal string GetTimeString(TimeSpan value)
		{
			if (!string.IsNullOrWhiteSpace(_PropertyMeta.DisplayFormatString))
				return string.Format(_PropertyMeta.DisplayFormatString, value);
			else if (value.Seconds > 0)
				return string.Format("{0:HH:mm:ss}", value);
			else
				return string.Format("{0:HH:mm}", value);
		}
	}
}
