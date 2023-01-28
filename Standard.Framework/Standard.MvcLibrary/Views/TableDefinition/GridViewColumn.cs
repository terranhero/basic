using System;
using System.IO;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Messages;
using BM = Basic.MvcLibrary;

namespace Basic.MvcLibrary
{
	/// <summary>表示 el-table 列信息定义</summary>
	public abstract class GridViewColumn<T> : Options<GridViewColumn<T>>, IGridViewColumn<T>, IGridViewColumn where T : class
	{
		private readonly GridViewColumnCollection<T> mColumns;
		private readonly IBasicContext mBasic;
		/// <summary>初始化 GridViewColumn 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="props"></param>
		protected GridViewColumn(IBasicContext bh, EntityPropertyCollection props)
		{
			mBasic = bh; HeaderAlignToCenter(); AlignToCenter();
			mColumns = new GridViewColumnCollection<T>(bh, props, this);
		}

		/// <summary></summary>
		public GridViewColumnCollection<T> Columns { get { return mColumns; } }

		/// <summary>设置字段名称</summary>
		public IGridViewColumn Field(string field) { SetAttr("prop", field); return this; }

		/// <summary>设置 type = expand </summary>
		public IGridViewColumn Expand() { SetAttr("type", "expand"); return this; }

		/// <summary>设置 label 属性值</summary>
		/// <param name="title">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn Title(string title)
		{
			string label = mBasic.GetString(title);
			SetAttr("label", label);
			return this;
		}

		/// <summary>设置 label 属性值</summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn Title(string converterName, string name)
		{
			string label = mBasic.GetString(converterName, name);
			SetAttr("label", label);
			return this;
		}

		/// <summary>用指定的长度初始化 System.Web.UI.WebControls.Unit 结构的新实例。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public IGridViewColumn Width(string width) { SetAttr("width", width); return this; }

		/// <summary>用像素为高度的数字初始化 Width 的新值</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public IGridViewColumn Width(int width) { SetAttr("width", string.Concat(width, "px")); return this; }

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		public IGridViewColumn Width(bool condition, int tWidth, int fWidth) { return condition ? Width(tWidth) : Width(fWidth); }

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		public IGridViewColumn Width(bool condition, string tWidth, string fWidth) { return condition ? Width(tWidth) : Width(fWidth); }

		/// <summary>对应列的最小宽度，与 width 的区别是 width 是固定的，min-width 会把剩余宽度按比例分配给设置了 min-width 的列。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public IGridViewColumn MinWidth(string width) { SetAttr("min-width", width); return this; }

		/// <summary>对应列的最小宽度，与 width 的区别是 width 是固定的，min-width 会把剩余宽度按比例分配给设置了 min-width 的列</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public IGridViewColumn MinWidth(int width) { SetAttr("min-width", string.Concat(width, "px")); return this; }

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		public IGridViewColumn MinWidth(bool condition, int tWidth, int fWidth) { return condition ? MinWidth(tWidth) : MinWidth(fWidth); }

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		public IGridViewColumn MinWidth(bool condition, string tWidth, string fWidth) { return condition ? MinWidth(tWidth) : MinWidth(fWidth); }

		/// <summary>设置 header-align 属性值靠左对齐</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn HeaderAlignToLeft() { SetAttr("header-align", "left"); return this; }

		/// <summary>设置 header-align 属性值居中对齐</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn HeaderAlignToCenter() { SetAttr("header-align", "center"); return this; }

		/// <summary>设置 header-align 属性值靠右对齐</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn HeaderAlignToRight() { SetAttr("header-align", "right"); return this; }

		/// <summary>设置 align 属性值靠左对齐</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn AlignToLeft() { SetAttr("align", "left"); return this; }

		/// <summary>设置 align 属性值居中对齐</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn AlignToCenter() { SetAttr("align", "center"); return this; }

		/// <summary>设置 align 属性值靠右对齐</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn AlignToRight() { SetAttr("align", "right"); return this; }

		/// <summary>设置 sortable 属性值</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn AllowSortable() { SetProp("sortable", "true"); return this; }

		/// <summary>设置 sortable 属性值</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn Sortable(bool value) { SetProp("sortable", value); return this; }

		/// <summary>设置 sortable 属性值</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn SortableToRemote() { SetAttr("sortable", "custom"); return this; }

		/// <summary>设置Resizable属性值</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn AllowResizable() { SetProp("resizable", "true"); return this; }

		/// <summary>列固定在左侧。</summary>
		public IGridViewColumn AllowFixed() { SetProp("fixed", "true"); return this; }

		/// <summary>列固定在右侧。</summary>
		public IGridViewColumn FixedToRight() { SetAttr("fixed", "right"); return this; }

		/// <summary>设置列的 formatter 属性值</summary>
		/// <param name="function">单元格formatter(格式化器)函数，带3个参数：value：字段值。row：行记录数据。index: 行索引。 </param>
		/// <example>formatter: function(value,row,index){ if (row){ return row.name; } else { return value; } }</example>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn SetFormatter(string function) { SetProp("formatter", function); return this; }

		/// <summary>对数据进行排序的时候使用的方法，仅当 sortable 设置为 true 的时候有效，需返回一个数字，和 Array.sort 表现一致</summary>
		/// <param name="function">列标题 Label 区域渲染使用的 function(a, b) </param>
		/// <example>sortmethod: function (a, b)</example>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn SetSortMethod(string function) { SetProp("sort-method", function); return this; }

		/// <summary>当内容过长被隐藏时显示 tooltip </summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn ShowOverflowTooltip() { SetProp("show-overflow-tooltip", "true"); return this; }

		/// <summary>设置列的 render-header 属性值</summary>
		/// <param name="function">列标题 Label 区域渲染使用的 Function(h, { column, $index }) </param>
		/// <example>function renderHeader(h, { column, $index })</example>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn SetRenderHeader(string function) { SetProp("render-header", function); return this; }

		/// <summary>仅对 type=selection 的列有效，类型为 Function，Function 的返回值用来决定这一行的 CheckBox 是否可以勾选</summary>
		/// <param name="function">用来格式化内容	function(row, index) </param>
		/// <example>selectable: function(row, index){}</example>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn SetSelectable(string function) { SetProp("selectable", function); return this; }

		/// <summary>列的 className	string</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn ClassName(string css) { SetAttr("class-name", css); return this; }

		/// <summary>当前列标题的自定义类名(label-class-name)</summary>
		/// <returns>返回当前列对象。</returns>
		public IGridViewColumn LabelClassName(string css) { SetAttr("label-class-name", css); return this; }

		/// <summary>输出列描述</summary>
		/// <param name="writer"></param>
		internal protected virtual void Render(System.IO.TextWriter writer)
		{
			using (TagHtmlWriter builder = new TagHtmlWriter(ViewTags.TableColumn))
			{
				builder.MergeOptions(this);
				builder.RenderBeginTag(writer);
				foreach (GridViewColumn<T> column in Columns)
				{
					column.Render(writer);
				}
				builder.RenderEndTag(writer);
			}
		}
	}

	/// <summary>表示 el-table 列信息定义</summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TR"></typeparam>
	public class GridViewColumn<T, TR> : GridViewColumn<T>, IGridViewColumn<T, TR> where T : class
	{
		/// <summary>
		/// 获取当前列的计算返回值的委托。
		/// </summary>
		internal readonly Func<T, TR> _ValueFunc;
		internal EntityPropertyMeta PropertyMeta { get; }

		/// <summary>初始化 GridViewColumn 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="props"></param>
		/// <param name="field"></param>
		/// <param name="valueFunc">目标值计算公式(从一个表达式，标识包含要呈现的属性的对象)</param>
		/// <param name="metaData">当前列对应的属性元数据</param>
		public GridViewColumn(IBasicContext bh, EntityPropertyCollection props, string field, Func<T, TR> valueFunc, EntityPropertyMeta metaData)
			: base(bh, props)
		{
			Field(field); _ValueFunc = valueFunc; PropertyMeta = metaData;
		}

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public TR GetValue(T model) { return _ValueFunc(model); }
	}

	/// <summary>表示 el-table 列信息定义</summary>
	/// <typeparam name="T"></typeparam>
	public class GridViewButtonColumn<T> : GridViewColumn<T>, IButtonsProvider<T> where T : class
	{
		internal EntityPropertyMeta PropertyMeta { get; }
		private readonly IBasicContext mBasic;
		private readonly ButtonCollection<T> buttons;
		/// <summary>初始化 GridViewColumn 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="props"></param>
		public GridViewButtonColumn(IBasicContext bh, EntityPropertyCollection props)
			: base(bh, props) { mBasic = bh; buttons = new ButtonCollection<T>(bh); }

		/// <summary>表示按钮</summary>
		public Button ElButton()
		{
			return buttons.Add(new Button(mBasic, ViewTags.ElButton));
		}

		/// <summary>表示按钮</summary>
		/// <param name="show">是否显示此按钮</param>
		public Button ElButton(bool show)
		{
			if (show == false) { return BM.Button.Empty(mBasic); }
			return ElButton();
		}

		/// <summary>表示按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button ElButton(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(mBasic, code);
			if (isAuthorization == false) { return BM.Button.Empty(mBasic); }
			return ElButton();
		}
		/// <summary>表示文本按钮</summary>
		public Button Text()
		{
			Button text = new Button(mBasic, ButtonType.Custom);
			return buttons.Add(text.AddClass("el-button--text"));
		}

		/// <summary>表示文本按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Text(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(mBasic, code);
			if (isAuthorization == false) { return BM.Button.Empty(mBasic); }
			return Text();
		}

		/// <summary>表示文本按钮</summary>
		/// <param name="show">是否显示此按钮</param>
		public Button Text(bool show)
		{
			if (show == false) { return BM.Button.Empty(mBasic); }
			return Text();
		}

		/// <summary>表示按钮</summary>
		public Button Button()
		{
			return buttons.Add(new Button(mBasic, ButtonType.Custom));
		}

		/// <summary>表示按钮</summary>
		/// <param name="show">表示权限编码</param>
		public Button Button(bool show)
		{
			if (show == false) { return BM.Button.Empty(mBasic); }
			return Button();
		}

		/// <summary>表示按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Button(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(mBasic, code);
			if (isAuthorization == false) { return BM.Button.Empty(mBasic); }
			return Button();
		}

		/// <summary>输出列描述</summary>
		/// <param name="writer"></param>
		protected internal override void Render(TextWriter writer)
		{
			using (TagHtmlWriter builder = new TagHtmlWriter(ViewTags.TableColumn))
			{
				builder.MergeOptions(this);
				builder.RenderBeginTag(writer);
				using (TagHtmlWriter template = new TagHtmlWriter(ViewTags.Template))
				{
					template.MergeAttribute("slot-scope", "scope");
					template.RenderBeginTag(writer);
					foreach (Button column in buttons)
					{
						column.Render(writer);
					}
					template.RenderEndTag(writer);
				}
				builder.RenderEndTag(writer);
			}
		}
	}
}
