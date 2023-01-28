using System;
using System.Linq.Expressions;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public interface IListView<TM> : IOptions, IDisposable where TM : class
	{
		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		ToolbarTemplate<TT> Toolbar<TT>() where TT : class;

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		ToolbarTemplate<TM> Toolbar();

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		/// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		ToolbarTemplate<TM> Toolbar(Expression<Action<IOptions>> expression);

		/// <summary><![CDATA[初始化 ListItemTemplate 类实例。
		/// 输出 <template v-slot:item="{row, index, rows}"></template>标签]]></summary>
		ListItemTemplate<TM> ListItem();

		/// <summary><![CDATA[初始化 ListItemTemplate 类实例。
		/// 输出 <template v-slot:item="{row, index, rows}"></template>标签]]></summary>
		ListItemTemplate<TT> ListItem<TT>() where TT : class;
	}

	/// <summary>
	/// 表示属性视图框架页，此为左右结构
	/// 左边为树，右边上部为工具条按钮，
	/// 右边下部为内容页，可以是树节点相关联的内容也可以是自定义内容
	/// </summary>
	public sealed class ListView<TM> : Options<ListView<TM>>, IListView<TM>, IOptions, IDisposable where TM : class
	{
		private readonly IBasicContext mBasic;
		private readonly TagHtmlWriter builder;

		/// <summary>初始化树形视图</summary>
		/// <param name="bh">基础开发框架扩展</param>
		internal ListView(IBasicContext bh)
		{
			mBasic = bh; builder = new TagHtmlWriter(ViewTags.ListView);
		}

		/// <summary></summary>
		public ListView<TM> Begin()
		{
			builder.MergeOptions(this);
			builder.RenderBeginTag(mBasic.Writer);
			return this;
		}

		/// <summary><![CDATA[初始化 ListItemTemplate 类实例。
		/// 输出 <template v-slot:item="{row, index, rows}"></template>标签]]></summary>
		public ListItemTemplate<TM> ListItem()
		{
			return new ListItemTemplate<TM>(mBasic);
		}

		/// <summary><![CDATA[初始化 ListItemTemplate 类实例。
		/// 输出 <template v-slot:item="{row, index, rows}"></template>标签]]></summary>
		public ListItemTemplate<TT> ListItem<TT>() where TT : class
		{
			return new ListItemTemplate<TT>(mBasic);
		}

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		public ToolbarTemplate<TT> Toolbar<TT>() where TT : class
		{
			return new ToolbarTemplate<TT>(mBasic);
		}

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		public ToolbarTemplate<TM> Toolbar() { return new ToolbarTemplate<TM>(mBasic); }

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		/// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		public ToolbarTemplate<TM> Toolbar(Expression<Action<IOptions>> expression)
		{
			Options opts = new Options();
			if (expression != null) { expression.Compile().Invoke(opts); }
			return new ToolbarTemplate<TM>(mBasic, opts);
		}

		/// <summary>释放非托管资源</summary>
		protected override void Dispose() { builder.RenderEndTag(mBasic.Writer); }

		/// <summary>释放非托管资源</summary>
		void IDisposable.Dispose() { this.Dispose(); }

		/// <summary>设置当前元素的 id 属性。</summary>
		public new ListView<TM> Id(string id) { Attr("id", id); return this; }

		/// <summary>设置当前元素的 ref 属性。</summary>
		public new ListView<TM> Ref(string name) { Attr("ref", name); return this; }

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="key">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> RowKey(string key) { Attr("row-key", key); return this; }

		/// <summary>表格数据源主键列。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> ShowExport(bool value = true)
		{
			return Prop("show-export", value);
		}

		/// <summary>表格数据源主键列。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> ShowRefresh(bool value = true)
		{
			return Prop("show-refresh", value);
		}

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="expression">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> RowKey<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			return RowKey(name);
		}

		/// <summary>表格数据源主键列。</summary>
		/// <param name="keys">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> RowKeys(params string[] keys)
		{
			if (keys != null && keys.Length > 0) { Prop("row-keys", string.Concat("['", string.Join("','", keys), "']")); }
			return this;
		}

		/// <summary>是否要高亮当前行，默认为 false，调用此方法后将设置为 true。</summary>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> HighlightCurrentRow() { Prop("highlight-current-row", "true"); return this; }

		/// <summary>显示为斑马纹 table。</summary>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> Stripe() { Prop("stripe", true); return this; }

		/// <summary>设置表格的查询对象。</summary>
		/// <param name="obj">要设置的对象</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> QueryParams(string obj) { Prop("query-params", obj); return this; }

		/// <summary>表格数据源时间戳列名。</summary>
		/// <param name="ts">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> Timestamp(string ts) { Attr("timestamp", ts); return this; }

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="expression">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> Timestamp<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			return Timestamp(name);
		}

		/// <summary>Table 的高度，默认为自动高度。如果 height 为 string 类型，则这个高度会设置为 Table 的 style.height 的值，Table 的高度会受控于外部样式。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> Height(string height) { Attr("height", height); return this; }

		/// <summary>Table 的高度，默认为自动高度。如果 height 为 number 类型，单位 px；Table 的高度会受控于外部样式。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> Height(int height) { Attr("height", string.Concat(height, "px")); return this; }

		/// <summary>Table 的最大高度。合法的值为数字或者单位为 px 的高度。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> MaxHeight(string height) { Attr("max-height", height); return this; }

		/// <summary>Table 的最大高度。合法的值为数字或者单位为 px 的高度。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> MaxHeight(int height) { Attr("max-height", string.Concat(height, "px")); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public ListView<TM> Action(string action) { Attr("url", mBasic.Action(action, null)); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public ListView<TM> Action(string action, string controller) { Attr("url", mBasic.Action(action, controller)); return this; }

		/// <summary>设置执行请求。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public ListView<TM> Action(string action, string controller, object routeValues)
		{
			Attr("url", mBasic.Action(action, controller, routeValues)); return this;
		}

		/// <summary>显示为斑马纹 table。</summary>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> ShowStripe() { Prop("stripe", true); return this; }

		/// <summary>显示带有纵向边框。</summary>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> ShowBorder() { Prop("border", true); return this; }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> SizeToMedium() { Attr("size", "medium"); return this; }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> SizeToSmall() { Attr("size", "small"); return this; }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public ListView<TM> SizeToMini() { Attr("size", "mini"); return this; }
	}

	/// <summary></summary>
	public static class ListViewExtension
	{
		/// <summary>初始化 IGridView 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="id">表示当前 GridView ID</param>
		public static ListView<TM> ListView<TM>(this IBasicContext bh, string id) where TM : class
		{
			ListView<TM> view = new ListView<TM>(bh);
			view.Id(id).Ref(id);
			return view;
		}
		/// <summary>初始化 IListView 类实例</summary>
		/// <param name="bh"></param>
		public static ListView<T> ListView<T>(this IBasicContext bh) where T : class
		{
			return new ListView<T>(bh);
		}
	}
}
