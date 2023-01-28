using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using Basic.MvcLibrary;
using Task = System.Threading.Tasks.Task;

namespace Basic.EasyLibrary
{
	/// <summary>生成EasyUI.DataGrid类型表格</summary>
	/// <typeparam name="T">模型实体类型</typeparam>
	public class DataGrid<T> : IDataGrid<T> where T : class
	{
		private readonly DataGridRowCollection<T> mColumns;
		private readonly EntityPropertyCollection mProperties;
		private readonly IBasicContext mBasic;
		private readonly DataGridOptions mOptions;
		private string _Caption;

		/// <summary>
		/// 创建JQuery.EasyUI.DataGrid的MVC对象实例(Version:1.4.4)。
		/// </summary>
		/// <param name="html">当前表格的输出流。</param>
		/// <param name="options">属性</param>
		/// <returns>返回JQuery.EasyUI.DataGrid的MVC对象实例。</returns>
		public DataGrid(IBasicContext html, DataGridOptions options)
		{
			mBasic = html; mColumns = new DataGridRowCollection<T>(this);
			EntityPropertyProvidor.TryGetProperties(typeof(T), out mProperties);
			mOptions = options ?? new DataGridOptions();
			DataGridExtension.ToOptions(mOptions);
		}

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		void IDisposable.Dispose() { }

#if (NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		async ValueTask IAsyncDisposable.DisposeAsync() { await mBasic.FlushAsync(); }
#endif

		#region 属性
		/// <summary>表示当前请求上下文信息</summary>
		public IBasicContext Context { get { return mBasic; } }

		/// <summary>
		/// 属性集合
		/// </summary>
		public DataGridOptions Options { get { return mOptions; } }

		/// <summary>
		/// When loading data from remote site,
		/// </summary>
		public string Caption { get { return _Caption; } }

		/// <summary>
		/// 表示当前表格的数据列
		/// </summary>
		internal DataGridRowCollection<T> Columns { get { return mColumns; } }

		/// <summary>
		/// 使用Lambda表达式指定主键字段。 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public IDataGrid<T> IdFieldFor<TR>(Expression<Func<T, TR>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			mOptions.IdField = memberExpression == null ? null : memberExpression.Member.Name;
			return this;
		}

		/// <summary>
		/// 使用Lambda表达式指定时间戳字段。 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public IDataGrid<T> TsFieldFor<TR>(Expression<Func<T, TR>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			mOptions.tsField = memberExpression == null ? null : memberExpression.Member.Name;
			return this;
		}

		/// <summary>
		/// Defines which column can be sorted.
		/// Defines the column sort order, can only be 'asc' or 'desc'. asc 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="sortDirection">排序方向</param>
		/// <returns></returns>
		public IDataGrid<T> Sort(Expression<Func<T, object>> expression, SortDirection sortDirection) { return this; }
		#endregion

		#region 输出列头信息
		/// <summary>
		/// 使用Lambda表达式设置标题字段。
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public IDataGrid<T> CaptionFor(string expression) { return this.CaptionFor(expression, false); }

		/// <summary>
		/// 使用Lambda表达式设置标题字段。
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="localization">是否使用本地化资源</param>
		/// <returns></returns>
		public IDataGrid<T> CaptionFor(string expression, bool localization)
		{
			_Caption = expression;
			if (localization)
			{
				string title = mBasic.GetString(expression);
				if (string.IsNullOrEmpty(title) == false) { _Caption = title; }
			}
			return this;
		}

		/// <summary>使用 Lambda 表达式设置标题字段。</summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public IDataGrid<T> CaptionFor(string converterName, string name)
		{
			string title = mBasic.GetString(converterName, name);
			if (string.IsNullOrEmpty(title) == false) { _Caption = title; }
			return this;
		}

		/// <summary>设置 CssClass 属性值</summary>
		/// <param name="cssName">样式class名称</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridRow<T> SetCurrentRowClass(string cssName) { return mColumns.SetCssClass(cssName); }

		/// <summary>生成数组列</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridHeaderColumn<T> HeaderFor<TR>(Expression<Func<T, IEnumerable<PropertyMeta<TR>>>> expression)
			 where TR : struct, IComparable<TR>, IEquatable<TR>
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			DataGridHeaderColumn<T> header = new DataGridHeaderColumn<T>(mBasic);
			mColumns.Append(header);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				header.SetTitle(propertyInfo.Name, false);
			else
				header.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					header.SetTitle(wda.DisplayName, true);
				else
					header.SetTitle(converterName, wda.DisplayName);
			}
			return header;
		}

		/// <summary>
		/// 生成数组列
		/// </summary>
		/// <param name="fields">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public IDictionary<string, DataGridHeaderColumn<T>> HeaderFor(IDictionary<string, IDictionary<string, string>> fields)
		{
			Dictionary<string, DataGridHeaderColumn<T>> dic = new Dictionary<string, DataGridHeaderColumn<T>>();
			foreach (var item in fields)
			{
				DataGridHeaderColumn<T> header = new DataGridHeaderColumn<T>(mBasic);
				header.Colspan = item.Value.Count;
				header.SetTitle(item.Key, false);
				mColumns.Append(header);
				dic.Add(item.Key, header);
			}
			return dic;
		}

		/// <summary>
		/// Specifies the columns to use. 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridHeaderColumn<T> HeaderFor(string expression)
		{
			DataGridHeaderColumn<T> column = new DataGridHeaderColumn<T>(mBasic);
			column.SetTitle(expression, false);
			mColumns.Append(column);
			return column;
		}

		/// <summary>
		/// Specifies the columns to use. 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="localization">是否使用本地化资源</param>
		/// <returns></returns>
		public DataGridHeaderColumn<T> HeaderFor(string expression, bool localization)
		{
			DataGridHeaderColumn<T> column = new DataGridHeaderColumn<T>(mBasic);
			column.SetTitle(expression, localization);
			mColumns.Append(column);
			return column;
		}

		/// <summary>
		/// 设置Title属性值
		/// </summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridHeaderColumn<T> HeaderFor(string converterName, string name)
		{
			DataGridHeaderColumn<T> column = new DataGridHeaderColumn<T>(mBasic);
			column.SetTitle(converterName, name);
			mColumns.Append(column);
			return column;
		}

		/// <summary>
		/// <![CDATA[将对象添加到 DataGridRow<T> 的结尾处。]]>
		/// </summary>
		/// <returns><![CDATA[返回添加成功的 DataGridRow<T> 对象实例。]]></returns>
		public DataGridRow<T> CreateRow() { return mColumns.CreateRow(); }
		#endregion

		/// <summary>
		/// <![CDATA[获取一个 DataGridRow<T>对象,该对象表示当前行。]]>
		/// </summary>
		/// <returns><![CDATA[一个 DataGridRow<T>对象,该对象表示当前行。]]></returns>
		public DataGridRow<T> GetCurrentRow() { return mColumns.GetCurrentRow(); }

		#region 
		/// <summary>添加按钮列</summary>
		/// <returns>返回按钮列。</returns>
		public ButtonColumn<T> ButtonsFor(Func<T, string> expression)
		{
			ButtonColumn<T> column = new ButtonColumn<T>(mBasic, expression);
			mColumns.Append(column);
			return column;
		}
		#endregion

		#region 输出行号
		/// <summary>
		/// 设置行号列
		/// </summary>
		/// <param name="startNumber">表示行号起始值</param>
		/// <returns>返回当前列对象。</returns>
		public NumberColumn<T> RowNumberFor(int startNumber)
		{
			NumberColumn<T> column = new NumberColumn<T>(mBasic, startNumber);
			mColumns.Append(column);
			return column;
		}

		/// <summary>
		/// 设置行号列
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public NumberColumn<T> RowNumberFor()
		{
			NumberColumn<T> column = new NumberColumn<T>(mBasic, 1);
			mColumns.Append(column);
			return column;
		}
		#endregion

		#region 输出选择列信息
		/// <summary>
		/// 创建选择列. 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, RT> SelectFor<RT>(Expression<Func<T, RT>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;
			EntityPropertyMeta propertyInfo = null; mProperties.TryGetProperty(name, out propertyInfo);
			DataGridColumn<T, RT> column = new DataGridColumn<T, RT>(mBasic, name, expression.Compile(), propertyInfo);
			column.AllowCheck(); mColumns.Insert(0, column);
			return column;
		}
		#endregion

		#region 输出列信息
		/// <summary>是否启用本地化表达式</summary>
		/// <param name="localization">是否使用本地化表达式</param>
		/// <param name="expression1">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="expression2">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, RT> LabelFor<RT>(bool localization, Expression<Func<T, RT>> expression1, Expression<Func<T, RT>> expression2)
		{
			Expression<Func<T, RT>> expression = localization ? expression1 : expression2;
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			DataGridColumn<T, RT> column = new DataGridColumn<T, RT>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(column);
			return column;
		}

		/// <summary>
		/// Specifies the columns to use. 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, RT> LabelFor<RT>(Expression<Func<T, RT>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			DataGridColumn<T, RT> column = new DataGridColumn<T, RT>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(column);
			return column;
		}

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, RT> LabelFor<RT>(DataGridRow<T> row, Expression<Func<T, RT>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			DataGridColumn<T, RT> column = new DataGridColumn<T, RT>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(row, column);
			return column;
		}

		/// <summary>
		/// Specifies the columns to use. 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, object> LabelFor(Expression<Func<T, object>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			DataGridColumn<T, object> column = new DataGridColumn<T, object>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(column);
			return column;
		}

		/// <summary>
		/// 生成后台Json对象属性，使用此方法，系统不生成表格列。 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, RT> JsonFor<RT>(Expression<Func<T, RT>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);
			DataGridColumn<T, RT> column = new DataGridJsonColumn<T, RT>(mBasic, name, expression.Compile(), propertyInfo);
			mColumns.Append(column);
			return column;
		}

		/// <summary>
		/// Specifies the columns to use. 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridEnumColumn<T> EnumFor(Expression<Func<T, Enum>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			DataGridEnumColumn<T> column = new DataGridEnumColumn<T>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(column);
			return column;
		}

		/// <summary>生成后台Json对象属性，使用此方法，系统不生成表格列。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridEnumColumn<T> EnumFor(DataGridRow<T> row, Expression<Func<T, Enum>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			DataGridEnumColumn<T> column = new DataGridEnumColumn<T>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(row, column);
			return column;
		}

		/// <summary>
		/// 日期类型列
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回列信息</returns>
		public DataGridColumn<T, DateTime> DateFor(Expression<Func<T, DateTime>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			EntityPropertyMeta propertyInfo = null; mProperties.TryGetProperty(name, out propertyInfo);

			DataGridColumn<T, DateTime> column = new DataGridColumn<T, DateTime>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(column);
			return column;
		}

		/// <summary>生成后台Json对象属性，使用此方法，系统不生成表格列。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, DateTime> DateFor(DataGridRow<T> row, Expression<Func<T, DateTime>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			EntityPropertyMeta propertyInfo = null; mProperties.TryGetProperty(name, out propertyInfo);

			DataGridColumn<T, DateTime> column = new DataGridColumn<T, DateTime>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(row, column);
			return column;
		}

		/// <summary>
		/// 日期类型列 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, DateTime?> DateFor(Expression<Func<T, DateTime?>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			EntityPropertyMeta propertyInfo = null; mProperties.TryGetProperty(name, out propertyInfo);

			DataGridColumn<T, DateTime?> column = new DataGridColumn<T, DateTime?>(mBasic, name, expression.Compile(), propertyInfo);

			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(column);
			return column;
		}

		/// <summary>生成后台Json对象属性，使用此方法，系统不生成表格列。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, DateTime?> DateFor(DataGridRow<T> row, Expression<Func<T, DateTime?>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			EntityPropertyMeta propertyInfo = null; mProperties.TryGetProperty(name, out propertyInfo);

			DataGridColumn<T, DateTime?> column = new DataGridColumn<T, DateTime?>(mBasic, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.SetTitle(propertyInfo.Name, false);
			else
				column.SetTitle(propertyInfo.DisplayName, false);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(row, column);
			return column;
		}
		#endregion

		#region 输出数组列信息
		/// <summary>生成数组列</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="fields"></param>
		/// <returns></returns>
		public DataGridArrayColumn<T, TR> ArrayFor<TR>(Expression<Func<T, IEnumerable<PropertyMeta<TR>>>> expression,
			IDictionary<string, string> fields) where TR : struct, IComparable<TR>, IEquatable<TR>
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);
			DataGridArrayColumn<T, TR> column = new DataGridArrayColumn<T, TR>(mBasic, expression.Compile(), propertyInfo, fields);
			if (fields == null || fields.Count == 0) { return column; }
			mColumns.Append(column);
			return column;
		}

		/// <summary>
		/// 生成数组列
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="fields">表</param>
		/// <returns></returns>
		public DataGridArraysColumn<T, TR> ArraysFor<TR>(Expression<Func<T, IEnumerable<PropertyMeta<TR>>>> expression,
			IDictionary<string, IDictionary<string, string>> fields) where TR : struct, IComparable<TR>, IEquatable<TR>
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);
			DataGridArraysColumn<T, TR> column = new DataGridArraysColumn<T, TR>(mBasic, expression.Compile(), propertyInfo, fields);
			if (fields == null || fields.Count == 0) { return column; }
			mColumns.Append(column);
			return column;
		}

		#endregion

		#region 输出布尔类型列数据
		/// <summary>
		/// Specifies the columns to use. 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, bool> BooleanFor(Expression<Func<T, bool>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			string trueText = "True", falseText = "False";
			if (propertyInfo != null && propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					trueText = mBasic.GetString(string.Concat(wda.DisplayName, "_TrueText"));
				else
					trueText = mBasic.GetString(converterName, string.Concat(wda.DisplayName, "_TrueText"));

				if (string.IsNullOrWhiteSpace(converterName))
					falseText = mBasic.GetString(string.Concat(wda.DisplayName, "_FalseText"));
				else
					falseText = mBasic.GetString(converterName, string.Concat(wda.DisplayName, "_FalseText"));
			}

			DataGridBoolColumn<T> column = new DataGridBoolColumn<T>(mBasic, name, expression.Compile(), propertyInfo, trueText, falseText);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(column);
			return column;
		}

		/// <summary>使用 Lambda 表达式创建布尔类型列字段。</summary>
		/// <param name="row">表示创建的列追加的行</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public DataGridColumn<T, bool> BooleanFor(DataGridRow<T> row, Expression<Func<T, bool>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			string trueText = "True", falseText = "False";
			if (propertyInfo != null && propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					trueText = mBasic.GetString(string.Concat(wda.DisplayName, "_TrueText"));
				else
					trueText = mBasic.GetString(converterName, string.Concat(wda.DisplayName, "_TrueText"));

				if (string.IsNullOrWhiteSpace(converterName))
					falseText = mBasic.GetString(string.Concat(wda.DisplayName, "_FalseText"));
				else
					falseText = mBasic.GetString(converterName, string.Concat(wda.DisplayName, "_FalseText"));
			}

			DataGridBoolColumn<T> column = new DataGridBoolColumn<T>(mBasic, name, expression.Compile(), propertyInfo, trueText, falseText);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.SetTitle(wda.DisplayName, true);
				else
					column.SetTitle(converterName, wda.DisplayName);
			}
			mColumns.Append(row, column);
			return column;
		}
		#endregion

		#region 输出表格
		/// <summary>
		/// 自定义行输出对象（此方法仅限于输出HTML格式）。
		/// </summary>
		/// <param name="expression">自定义输出方法</param>
		public void SetRowTemplate(Func<T, string> expression)
		{
			_RenderRowTemplate = expression ?? throw new ArgumentNullException("expression");
		}
		private Func<T, string> _RenderRowTemplate;

		/// <summary>自定义行输出方法</summary>
		internal Func<T, string> RenderRowTemplate { get { return _RenderRowTemplate; } }

		private RenderMode GetRenderMode()
		{
			string httpMethod = mBasic.HttpMethod.ToUpper();
			if (httpMethod == "GET")
				return RenderMode.Template;
			else if (httpMethod == "POST")
			{
				string renderModeString = mBasic.QueryString["ExportMode"];
				if (string.IsNullOrWhiteSpace(renderModeString))
					renderModeString = mBasic.Form["ExportMode"];
				if (renderModeString != null)
				{
					RenderMode mode = RenderMode.ExcelXlsx;
					if (Regex.IsMatch(renderModeString, @"^\d+$"))
					{
						byte byteMode = Convert.ToByte(renderModeString);
						if (Enum.IsDefined(typeof(RenderMode), byteMode))
							return (RenderMode)byteMode;
						return RenderMode.Json;
					}
					else if (Enum.TryParse<RenderMode>(mBasic.Form["ExportMode"], false, out mode))
						return mode;
				}
				return RenderMode.Json;
			}
			return RenderMode.Template;
		}

		/// <summary>
		/// 根据输出模式输出表格
		/// </summary>
		/// <returns></returns>
		private IRender<T> GridRender(RenderMode mode)
		{
			if (mode == RenderMode.Json) { return new DataRender<T>(this); }
			else if (mode == RenderMode.ExcelXlsx) { return new XlsxRender<T>(this); }
			else if (mode == RenderMode.Table) { return new ContentRender<T>(this); }
			return new HtmlRender<T>(this);
		}

		/// <summary>
		/// 输出表格到客户端请求中
		/// </summary>
		public void RenderReport(IPagination<T> dataSource)
		{
			RenderMode render = GetRenderMode();
			if (render == RenderMode.ExcelXlsx)
			{
				IRender<T> rd = new XlsxRender<T>(this)
				{
					FileName = DownloadFileAttribute.GetFileName(mBasic.Headers)
				};
				rd.Render(mBasic.Writer, mBasic.Response, dataSource);
			}
			else { new ReportRender<T>(this).Render(mBasic.Writer, mBasic.Response, dataSource); }
		}

		/// <summary>输出表格到客户端请求中</summary>
		public async Task RenderAsync(IPagination<T> dataSource)
		{
			RenderMode render = GetRenderMode();
			IRender<T> rd = GridRender(render);
			rd.FileName = DownloadFileAttribute.GetFileName(mBasic.Headers);
			await rd.RenderAsync(mBasic.Writer, mBasic.Response, dataSource);
		}

		/// <summary>
		/// 输出表格到客户端请求中
		/// </summary>
		public void Render(IPagination<T> dataSource)
		{
			RenderMode render = GetRenderMode();
			IRender<T> rd = GridRender(render);
			rd.FileName = DownloadFileAttribute.GetFileName(mBasic.Headers);
			rd.Render(mBasic.Writer, mBasic.Response, dataSource);
		}

		/// <summary>
		/// 输出表格到客户端请求中
		/// </summary>
		public void RenderTable(IPagination<T> dataSource)
		{
			new ContentRender<T>(this).Render(mBasic.Writer, mBasic.Response, dataSource);
		}

		/// <summary>
		/// 输出表格到客户端请求中
		/// </summary>
		public void RenderExcel(IPagination<T> dataSource)
		{
			new XlsxRender<T>(this).Render(mBasic.Writer, mBasic.Response, dataSource);
		}
		/// <summary>
		/// Renders the grid to the HtmlTextWriter specified at creation
		/// </summary>
		public void RenderJson(IPagination<T> dataSource)
		{
			new DataRender<T>(this).Render(mBasic.Writer, mBasic.Response, dataSource);
		}

		#endregion

		#region 数据源分组
		private GroupInfo<T> _GroupInfo;
		/// <summary>创建选择列。 </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public GroupInfo<T> GroupBy(Expression<Func<T, object>> expression)
		{
			_GroupInfo = new GroupInfo<T>(mBasic, expression.Compile());
			return _GroupInfo as GroupInfo<T>;
		}

		/// <summary>
		/// 获取DataGrid 分组信息
		/// </summary>
		/// <returns></returns>
		public GroupInfo<T> GetGroupInfo() { return _GroupInfo; }
		#endregion

		#region 输出页脚
		private FooterInfo<T> _FooterInfo;
		/// <summary>创建页脚。 </summary>
		/// <returns></returns>
		public FooterInfo<T> CreateFooter()
		{
			_FooterInfo = new FooterInfo<T>(mBasic);
			return _FooterInfo;
		}

		/// <summary>
		/// 获取DataGrid 页脚信息
		/// </summary>
		/// <returns></returns>
		public FooterInfo<T> GetFooterInfo() { return _FooterInfo; }

		#endregion
	}
}
