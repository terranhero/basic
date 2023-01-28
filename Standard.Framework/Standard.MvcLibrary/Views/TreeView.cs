using System;
using System.Linq.Expressions;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public interface ITreeView : IDisposable
	{
		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		IToolbarTemplate Toolbar();

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		/// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		IToolbarTemplate Toolbar(Expression<Action<IOptions>> expression);

		/// <summary><![CDATA[初始化 NodeTemplate 类实例。
		/// 输出 <template v-slot:node="{node, data}"></template>标签。
		/// 可以自定义输出树节点样式。]]></summary>
		/// <example><![CDATA[带图标的样式 <span><i :class="['el-icon', data.iconCls]" />{{data.text}}</span>]]></example>
		ITemplateView NodeTemplate();

		/// <summary><![CDATA[初始化 NodeTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		/// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		ITemplateView NodeTemplate(Expression<Action<IOptions>> expression);

	}

	/// <summary>
	/// 表示属性视图框架页，此为左右结构
	/// 左边为树，右边上部为工具条按钮，
	/// 右边下部为内容页，可以是树节点相关联的内容也可以是自定义内容
	/// </summary>
	public sealed class TreeView<TM> : InputProvider<TM>, ITreeView where TM : class
	{
		private readonly IBasicContext basicContext;
		private readonly TagHtmlWriter hWriter;

		/// <summary>初始化树形视图</summary>
		/// <param name="bh">基础开发框架扩展</param>
		internal TreeView(IBasicContext bh) : base(bh)
		{
			basicContext = bh; hWriter = new TagHtmlWriter(ViewTags.TreeView);
		}

		/// <summary>开始输出树视图</summary>
		public TreeView<TM> Begin()
		{
			hWriter.MergeOptions(this);
			hWriter.RenderBeginTag(basicContext.Writer);
			return this;
		}

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		public IToolbarTemplate Toolbar() { return new ToolbarTemplate<TM>(basicContext); }

		/// <summary><![CDATA[初始化 ToolbarTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		/// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		public IToolbarTemplate Toolbar(Expression<Action<IOptions>> expression)
		{
			Options opts = new Options();
			if (expression != null) { expression.Compile().Invoke(opts); }
			return new ToolbarTemplate<TM>(basicContext, opts);
		}

		/// <summary><![CDATA[初始化 NodeTemplate 类实例。
		/// 输出 <template v-slot:node="{node, data}"></template>标签。
		/// 可以自定义输出树节点样式。]]></summary>
		/// <example><![CDATA[带图标的样式 <span><i :class="['el-icon', data.iconCls]" />{{data.text}}</span>]]></example>
		public ITemplateView NodeTemplate() { return new TreeNodeTemplate<TM>(basicContext); }

		/// <summary><![CDATA[初始化 NodeTemplate 类实例。
		/// 输出 <template v-slot:toolbar></template>标签]]></summary>
		/// <param name="expression">生成属性/样式/组件属性的参数的表达式</param>
		public ITemplateView NodeTemplate(Expression<Action<IOptions>> expression)
		{
			Options opts = new Options();
			if (expression != null) { expression.Compile().Invoke(opts); }
			return new TreeNodeTemplate<TM>(basicContext, opts);
		}

		/// <summary>释放非托管资源</summary>
		protected override void Dispose() { hWriter.RenderEndTag(basicContext.Writer); }

		/// <summary>释放非托管资源</summary>
		void IDisposable.Dispose() { this.Dispose(); }

		/// <summary>树型控件的数据。</summary>
		/// <param name="value">数组类型的变量</param>
		/// <returns>返回当前对象。</returns>
		public new TreeView<TM> Model(string value) { SetProp("data", value); return this; }

		/// <summary>设置表格的查询对象。</summary>
		/// <param name="obj">要设置的对象</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> QueryParams(string obj) { SetProp("query-params", obj); return this; }

		/// <summary>标题控件高度。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> Height(string height) { SetAttr("height", height); return this; }

		/// <summary>标题控件高度。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> Height(int height) { SetAttr("height", string.Concat(height, "px")); return this; }

		/// <summary>左侧树型控件宽度，默认 230px。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> Width(string value) { SetAttr("width", value); return this; }

		/// <summary>左侧树型控件宽度，默认 230px。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> Width(int value) { SetAttr("width", string.Concat(value, "px")); return this; }

		/// <summary>图标类名</summary>
		/// <param name="css">图标类名</param>
		/// <returns>返回当前按钮实例</returns>
		public TreeView<TM> Icon(string css) { SetProp("icon", css); return this; }

		/// <summary>设置页面标题</summary>
		public TreeView<TM> Header(string key) { SetProp("title", key); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public TreeView<TM> Action(string action) { Attr("url", basicContext.Action(action, null)); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public TreeView<TM> Action(string action, string controller) { Attr("url", basicContext.Action(action, controller)); return this; }

		/// <summary>设置执行请求。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public TreeView<TM> Action(string action, string controller, object routeValues)
		{
			Attr("url", basicContext.Action(action, controller, routeValues)); return this;
		}

		private TreeView<TM> SetCreate(string url) { SetAttr("new-url", url); return this; }

		/// <summary>树型视图新增请求地址</summary>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Create(string action) { return SetCreate(basicContext.Action(action, null)); }

		/// <summary>树型视图新增请求地址</summary>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Create(string action, string controller) { return SetCreate(basicContext.Action(action, controller)); }

		/// <summary>树型视图新增请求地址。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Create(string action, string controller, object routeValues)
		{
			return SetCreate(basicContext.Action(action, controller, routeValues));
		}

		private TreeView<TM> SetEdit(string url) { SetAttr("edit-url", url); return this; }

		/// <summary>树型视图修改请求地址</summary>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Edit(string action) { return SetEdit(basicContext.Action(action, null)); }

		/// <summary>树型视图修改请求地址</summary>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Edit(string action, string controller) { return SetEdit(basicContext.Action(action, controller)); }

		/// <summary>树型视图修改请求地址。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Edit(string action, string controller, object routeValues)
		{
			return SetEdit(basicContext.Action(action, controller, routeValues));
		}

		private TreeView<TM> SetDelete(string url) { SetAttr("del-url", url); return this; }

		/// <summary>树型视图删除请求地址</summary>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Delete(string action) { return SetDelete(basicContext.Action(action, null)); }

		/// <summary>树型视图删除请求地址</summary>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Delete(string action, string controller) { return SetDelete(basicContext.Action(action, controller)); }

		/// <summary>树型视图删除请求地址。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Delete(string action, string controller, object routeValues)
		{
			return SetDelete(basicContext.Action(action, controller, routeValues));
		}

		/// <summary>内容为空的时候展示的文本。</summary>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> EmptyText(string text) { SetAttr("empty-text", text); return this; }

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="key">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> NodeKey(string key) { SetAttr("node-key", key); return this; }

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="expression">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> NodeKey<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			return NodeKey(name);
		}
		/// <summary>是否在点击节点的时候展开或者收缩节点， 默认值为 true，如果为 false，则只有点箭头图标的时候才会展开或者收缩节点。</summary>
		/// <param name="value">默认值为 true，如果为 false，则只有点箭头图标的时候才会展开或者收缩节点。</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> ExpandOnClickNode(bool value = true)
		{
			return Prop("expand-on-click-node", value) as TreeView<TM>;
		}

		/// <summary>是否默认展开所有节点</summary>
		/// <param name="value"></param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> DefaultExpandedAll(bool value = false)
		{
			return Prop("default-expand-all", value) as TreeView<TM>;
		}

		/// <summary>展开子节点的时候是否自动展开父节点</summary>
		/// <param name="value">展开子节点的时候是否自动展开父节点</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> AutoExpandParent(bool value = true)
		{
			return Prop("auto-expand-parent", value) as TreeView<TM>;
		}

		/// <summary>在显示复选框的情况下，是否严格的遵循父子不互相关联的做法，默认为 false</summary>
		/// <param name="value">在显示复选框的情况下，是否严格的遵循父子不互相关联的做法，默认为 false</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> CheckStrictly(bool value = false)
		{
			return Prop("check-strictly", value) as TreeView<TM>;
		}

		/// <summary>当前选中的节点。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> CurrentNodeKey(string value)
		{
			return Prop("current-node-key", value) as TreeView<TM>;
		}

		/// <summary>当前选中的节点。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> CurrentNodeKey(int value)
		{
			return Prop("current-node-key", value) as TreeView<TM>;
		}

		/// <summary>自定义树节点的图标</summary>
		/// <param name="value">树节点的图标</param>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> IconCladd(string value) { SetProp("icon-class", value); return this; }

		/// <summary>相邻级节点间的水平缩进，单位为像素</summary>
		/// <param name="value">相邻级节点间的水平缩进，单位为像素</param>
		/// <returns>返回当前实例</returns>
		public TreeView<TM> Indent(int value = 16) { SetProp("indent", value); return this; }

		/// <summary>默认展开的节点的 key 的数组</summary>
		/// <param name="value">默认展开的节点的 key 的数组</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> DefaultExpandedKeys(string value)
		{
			return Prop("default-expanded-keys", value) as TreeView<TM>;
		}
		/// <summary>是否每次只打开一个同级树节点展开</summary>
		/// <param name="value">默认值 false</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> Accordion(bool value = false)
		{
			return Prop("accordion", value) as TreeView<TM>;
		}

		/// <summary>节点是否可被选择</summary>
		/// <param name="value">默认值 false</param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> ShowCheckbox(bool value = false)
		{
			return SetProp("show-checkbox", value) as TreeView<TM>;
		}

		/// <summary>节点被点击时的回调。</summary>
		/// <param name="callback">接收事件的方法 function(data,node,target){ }
		/// 共三个参数，依次为：传递给 data 属性的数组中该节点所对应的对象、节点对应的 Node、节点组件本身。
		/// </param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> OnNodeClick(string callback)
		{
			Event("node-click", callback); return this;
		}

		/// <summary>节点选中状态发生变化时的回调。</summary>
		/// <param name="callback">接收事件的方法 function(data,checked,target){}
		/// 共三个参数，依次为：
		/// 传递给 data 属性的数组中该节点所对应的对象、
		/// 节点本身是否被选中、
		/// 节点的子树中是否有被选中的节点
		/// </param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> OnCheckChange(string callback)
		{
			Event("check-change", callback); return this;
		}

		/// <summary>当前选中节点变化时触发的事件。</summary>
		/// <param name="callback">接收事件的方法 function(data,node){}
		/// 共两个参数，依次为：当前节点的数据，当前节点的 Node 对象
		/// </param>
		/// <returns>返回当前对象。</returns>
		public TreeView<TM> OnCurrentChange(string callback)
		{
			Event("current-change", callback); return this;
		}

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		public new TreeView<TM> Event(string evt, string callback) { base.Event(evt, callback); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TreeView<TM> Style(string key, string value) { base.Style(key, value); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TreeView<TM> Attr<TP>(string key, TP value) { base.Attr(key, value); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new TreeView<TM> Prop<TP>(string key, TP value) { base.Prop(key, value); return this; }
	}

	/// <summary></summary>
	public static class TreeViewExtension
	{
		/// <summary>初始化 IGridView 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="id">表示当前 GridView ID</param>
		public static TreeView<TM> TreeView<TM>(this IBasicContext bh, string id) where TM : class
		{
			TreeView<TM> view = new TreeView<TM>(bh);
			view.Id(id).Ref(id);
			return view;
		}

		/// <summary>初始化 Toolbar 类实例</summary>
		/// <param name="bh"></param>
		public static TreeView<T> TreeView<T>(this IBasicContext<T> bh) where T : class
		{
			return new TreeView<T>(bh);
		}
	}
}
