using System;
using System.Linq.Expressions;
using Basic.Collections;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public interface IGridView<TM> : IOptions, IDisposable
	{
		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		IToolbarTemplate<TM> Toolbar();

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		/// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		IToolbarTemplate<TM> Toolbar(Expression<Action<IOptions>> expression);

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		IToolbarTemplate<T> Toolbar<T>() where T : class;
	}

	/// <summary>
	/// 表示属性视图框架页，此为左右结构
	/// 左边为树，右边上部为工具条按钮，
	/// 右边下部为内容页，可以是树节点相关联的内容也可以是自定义内容
	/// </summary>
	public sealed class GridView<TM> : Options<GridView<TM>>, IGridView<TM> where TM : class
	{
		private readonly GridViewRootColumn<TM> root;
		private readonly EntityPropertyCollection mProperties;
		private readonly IBasicContext basicContext;
		private readonly TagHtmlWriter builder;

		/// <summary>初始化树形视图</summary>
		/// <param name="bh">基础开发框架扩展</param>
		internal GridView(IBasicContext bh)
		{
			basicContext = bh; builder = new TagHtmlWriter(ViewTags.GridView);
			EntityPropertyProvidor.TryGetProperties(typeof(TM), out mProperties);
			root = new GridViewRootColumn<TM>(this, mProperties);
		}

		internal IBasicContext Basic { get { return basicContext; } }

		/// <summary></summary>
		public GridView<TM> Begin()
		{
			builder.MergeOptions(this);
			builder.RenderBeginTag(basicContext.Writer);
			return this;
		}

		/// <summary>设置当前元素的 id 属性。</summary>
		public new GridView<TM> Id(string id) { SetAttr("id", id); return this; }

		/// <summary>设置当前元素的 ref 属性。</summary>
		public new GridView<TM> Ref(string name) { SetAttr("ref", name); return this; }

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="expression">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> RowKey<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			return RowKey(name);
		}

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="key">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> RowKey(string key) { SetAttr("row-key", key); return this; }

		/// <summary>表格数据源主键列。</summary>
		/// <param name="keys">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> RowKey(params string[] keys)
		{
			if (keys != null && keys.Length > 0) { SetProp("row-key", string.Concat("['", string.Join("','", keys), "']")); }
			return this;
		}

		/// <summary>表格数据源主键列。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> ShowExport(bool value = true)
		{
			return Prop("show-export", value);
		}

		/// <summary>表格数据源主键列。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> ShowRefresh(bool value = true)
		{
			return Prop("show-refresh", value);
		}

		/// <summary>设置页面标题</summary>
		public GridView<TM> Header(string css, string title) { SetProp("icon", css); SetProp("title", title); return this; }

		/// <summary>设置页面标题</summary>
		public GridView<TM> Header(string title) { SetProp("title", title); return this; }

		/// <summary>设置表格的查询对象。</summary>
		/// <param name="obj">要设置的对象</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> QueryParams(string obj) { SetProp("query-params", obj); return this; }

		/// <summary>表格数据源时间戳列名。</summary>
		/// <param name="ts">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> Timestamp(string ts) { SetAttr("timestamp", ts); return this; }

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="expression">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> Timestamp<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			return Timestamp(name);
		}

		/// <summary>Table 的高度，默认为自动高度。如果 height 为 string 类型，则这个高度会设置为 Table 的 style.height 的值，Table 的高度会受控于外部样式。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> SetHeight(string height) { SetAttr("height", height); return this; }

		/// <summary>Table 的高度，默认为自动高度。如果 height 为 number 类型，单位 px；Table 的高度会受控于外部样式。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> SetHeight(int height) { SetAttr("height", string.Concat(height, "px")); return this; }

		/// <summary>Table 的最大高度。合法的值为数字或者单位为 px 的高度。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> SetMaxHeight(string height) { SetAttr("max-height", height); return this; }

		/// <summary>Table 的最大高度。合法的值为数字或者单位为 px 的高度。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> SetMaxHeight(int height) { SetAttr("max-height", string.Concat(height, "px")); return this; }

		/// <summary>显示显示选择列。</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> HideCheckbox() { SetProp("show-checkbox", false); return this; }

		/// <summary>显示行号。</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> HideRowNumber() { SetProp("show-row-number", false); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public GridView<TM> Action(string action) { SetAttr("url", basicContext.Action(action, null)); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public GridView<TM> Action(string action, string controller) { SetAttr("url", basicContext.Action(action, controller)); return this; }

		/// <summary>设置执行请求。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public GridView<TM> Action(string action, string controller, object routeValues)
		{
			SetAttr("url", basicContext.Action(action, controller, routeValues)); return this;
		}

		/// <summary>显示为斑马纹 table。</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> ShowStripe() { SetProp("stripe", true); return this; }

		/// <summary>显示带有纵向边框。</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> ShowBorder() { SetProp("border", true); return this; }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> SizeToMedium() { SetAttr("size", "medium"); return this; }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> SizeToSmall() { SetAttr("size", "small"); return this; }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> SizeToMini() { SetAttr("size", "mini"); return this; }

		/// <summary>列的宽度禁止自撑开，默认为true，调用此方法后将设置为false。</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> FitToFalse() { SetProp("fit", "false"); return this; }

		/// <summary>是否显示表头，默认为true，调用此方法后将设置为false。</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> ShowHeaderToFalse() { SetProp("show-header", "false"); return this; }

		/// <summary>是否要高亮当前行，默认为 false，调用此方法后将设置为 true。</summary>
		/// <returns>返回当前对象。</returns>
		public GridView<TM> HighlightCurrentRow() { SetProp("highlight-current-row", "true"); return this; }

		/// <summary>创建列</summary>
		/// <param name="expression"></param>
		public void ColumnsFor(Action<IColumnsProvider<TM>> expression)
		{
			if (expression == null) { return; }
			expression.Invoke(root.Columns);
		}

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		public IToolbarTemplate<TM> Toolbar() => new ToolbarTemplate<TM>(basicContext);

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		public IToolbarTemplate<T> Toolbar<T>() where T : class { return new ToolbarTemplate<T>(basicContext); }

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		/// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		public IToolbarTemplate<TM> Toolbar(Expression<Action<IOptions>> expression)
		{
			Options opts = new Options();
			if (expression != null) { expression.Compile().Invoke(opts); }
			return new ToolbarTemplate<TM>(basicContext, opts);
		}

		/// <summary>释放非托管资源</summary>
		protected override void Dispose()
		{
			root.Render(basicContext.Writer);
			builder.RenderEndTag(basicContext.Writer);
		}

		/// <summary>释放非托管资源</summary>
		void IDisposable.Dispose() { this.Dispose(); }
	}

	/// <summary></summary>
	public static class GridViewExtension
	{
		/// <summary>初始化 IGridView 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="id">表示当前 GridView ID</param>
		public static GridView<TM> GridView<TM>(this IBasicContext bh, string id) where TM : class
		{
			GridView<TM> view = new GridView<TM>(bh);
			view.Id(id).Ref(id);
			return view;
		}

		/// <summary>初始化 IGridView 类实例</summary>
		/// <param name="bh"></param>
		public static GridView<TM> GridView<TM>(this IBasicContext bh) where TM : class
		{
			return new GridView<TM>(bh);
		}

		///// <summary>初始化 IGridView 类实例</summary>
		///// <param name="bh">基础开发框架扩展</param>
		///// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		//public static GridView<TM> GridView<TM>(this IBasicContext bh, Expression<Action<GridViewOptions>> expression) where T : class
		//{
		//	GridView<TM> view = new GridView<TM>(bh);
		//	if (expression != null) { expression.Compile().Invoke(view); }
		//	return view;
		//}
	}
}
