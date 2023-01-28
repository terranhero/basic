
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// <![CDATA[表示 Cell<T> 的集合，一个集合表示分组中的一行]]>
	/// </summary>
	/// <typeparam name="T">表示当前 DataGrid 实体模型</typeparam>
	public sealed class GroupRowCollection<T> : IEnumerable<GroupRow<T>>
	{
		private readonly GroupInfo<T> _GroupInfo;
		private readonly List<GroupRow<T>> _Rows = new List<GroupRow<T>>(5);
		private GroupRow<T> _CurrentRow;
		/// <summary>
		///  初始化 GroupRowCollection 类实例。
		/// </summary>
		/// <param name="gi"><![CDATA[拥有此实例的 GroupInfo<T> 对象。]]></param>
		public GroupRowCollection(GroupInfo<T> gi)
		{
			_GroupInfo = gi; _Rows = new List<GroupRow<T>>(5);
			_CurrentRow = new GroupRow<T>(gi);
			_Rows.Add(_CurrentRow);
		}

		/// <summary>
		/// <![CDATA[将对象添加到 RowCollection<T> 的结尾处。]]>
		/// </summary>
		/// <returns><![CDATA[返回添加成功的 Row<T> 对象实例。]]></returns>
		public GroupRow<T> AddRow()
		{
			_CurrentRow = new GroupRow<T>(_GroupInfo);
			_Rows.Add(_CurrentRow);
			return _CurrentRow;
		}

		/// <summary>
		/// <![CDATA[将对象添加到 Row<T> 的结尾处。]]>
		/// </summary>
		/// <param name="item"><![CDATA[要添加到 Row<T> 的末尾处的对象。]]></param>
		/// <returns><![CDATA[返回添加成功的 Cell<T> 对象实例。]]></returns>
		public GroupCell<T> Add(GroupCell<T> item)
		{
			if (item == null) { throw new System.ArgumentNullException("item"); }
			return _CurrentRow.Add(item);
		}

		/// <summary>
		/// <![CDATA[返回循环访问 RowCollection<Row<T>> 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 RowCollection<T> 的 RowCollection<T>.Enumerator。]]></returns>
		public IEnumerator<GroupRow<T>> GetEnumerator() { return _Rows.GetEnumerator(); }

		/// <summary>
		/// <![CDATA[返回循环访问 RowCollection 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 RowCollection 的 RowCollection.Enumerator。]]></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _Rows.GetEnumerator(); }
	}

	/// <summary>
	/// <![CDATA[表示 Cell<T> 的集合，一个集合表示分组中的一行]]>
	/// </summary>
	/// <typeparam name="T">表示当前 DataGrid 实体模型</typeparam>
	public sealed class GroupRow<T> : IEnumerable<GroupCell<T>>
	{
		private readonly GroupInfo<T> _GroupInfo;
		private readonly List<GroupCell<T>> _Cells = new List<GroupCell<T>>(20);
		/// <summary>
		///  初始化 Row 类实例。
		/// </summary>
		/// <param name="gi"><![CDATA[拥有此实例的 GroupInfo<T> 对象。]]></param>
		public GroupRow(GroupInfo<T> gi) { _GroupInfo = gi; }

		/// <summary>
		/// <![CDATA[将对象添加到 Row<T> 的结尾处。]]>
		/// </summary>
		/// <param name="item"><![CDATA[要添加到 Row<T> 的末尾处的对象。]]></param>
		/// <returns><![CDATA[返回添加成功的 Cell<T> 对象实例。]]></returns>
		internal GroupCell<T> Add(GroupCell<T> item) { _Cells.Add(item); return item; }

		/// <summary>
		/// <![CDATA[返回循环访问 RowCollection<Row<T>> 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 RowCollection<T> 的 RowCollection<T>.Enumerator。]]></returns>
		public IEnumerator<GroupCell<T>> GetEnumerator()
		{
			return _Cells.GetEnumerator();
		}

		/// <summary>
		/// <![CDATA[返回循环访问 RowCollection 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 RowCollection 的 RowCollection.Enumerator。]]></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _Cells.GetEnumerator();
		}
	}

	/// <summary>
	/// 分组项输出时表示表格的Cell
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class GroupCell<T>
	{
		private readonly IBasicContext _Context;

		/// <summary>初始化 GroupItem 类实例</summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		protected GroupCell(IBasicContext context)
		{
			_Context = context;
			Rowspan = 0; Colspan = 0; AlignToCenter();
		}

		/// <summary>获取分组汇总值</summary>
		/// <param name="source">汇总数据源</param>
		/// <returns>返回分组汇总值。</returns>
		public abstract object GetValue(IGrouping<dynamic, T> source);

		/// <summary>
		/// 设置 align 属性值靠左对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public GroupCell<T> AlignToLeft() { Align = "left"; return this; }

		/// <summary>
		/// 设置 align 属性值居中对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public GroupCell<T> AlignToCenter() { Align = "center"; return this; }

		/// <summary>
		/// 设置 align 属性值靠右对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public GroupCell<T> AlignToRight() { Align = "right"; return this; }

		/// <summary>
		/// 获取或设置单元格如何显示列数据。“left”、“right”、“center”可以使用。
		/// </summary>
		public string Align { get; internal set; }
		/// <summary>
		/// True to hide the column.
		/// </summary>
		/// <value>default value is "false"</value>
		public bool Hidden { get; internal set; }
		/// <summary>
		/// 设置Hidden属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public GroupCell<T> Hide() { Hidden = true; return this; }

		/// <summary>
		/// 设置Colspan属性值
		/// </summary>
		/// <param name="colspan">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public GroupCell<T> SetColspan(int colspan) { Colspan = colspan; return this; }

		/// <summary>
		/// 表示组头列单元格应该占用多少列。
		/// </summary>
		public int Colspan { get; internal set; }

		/// <summary>
		/// 设置Rowspan属性值
		/// </summary>
		/// <param name="rowspan">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public GroupCell<T> SetRowspan(int rowspan) { Rowspan = rowspan; return this; }

		/// <summary>
		/// Indicate how many rows a cell should take up. 
		/// </summary>
		/// <value>default value is "0"</value>
		public int Rowspan { get; internal set; }

		/// <summary>
		/// 获取样式名称。
		/// </summary>
		public string CssClass { get { return string.Join(" ", cssClasses.ToArray()); } }

		private readonly List<string> cssClasses = new List<string>(10);
		/// <summary>
		/// 设置数据单元格样式名称 属性值
		/// </summary>
		/// <param name="calssName">设置当前单元格样式</param>
		/// <returns>返回当前列对象。</returns>
		public GroupCell<T> SetCssClass(string calssName)
		{
			if (cssClasses.Contains(calssName) == false) { cssClasses.Add(calssName); }
			return this;
		}

		/// <summary>获取当前数据单元格样式名称是否为空。</summary>
		public bool HasCssClass { get { return cssClasses.Count > 0; } }

		/// <summary>判断当前行是否包含指定的样式类。</summary>
		public bool HasClass(string className)
		{
			return cssClasses.Contains(className);
		}

	}

	/// <summary>
	/// 分组项输出时表示表格的Cell
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TR"></typeparam>
	public sealed class GroupCell<T, TR> : GroupCell<T>
	{
		private readonly Func<IGrouping<dynamic, T>, TR> _AggFunc;
		private readonly Basic.MvcLibrary.IBasicContext _Context;

		/// <summary>
		/// 初始化 GroupItem 类实例
		/// </summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		/// <param name="aggFunc"><![CDATA[计算序列IEnumerable<T>的聚合函数。]]></param>
		public GroupCell(IBasicContext context, Func<IGrouping<dynamic, T>, TR> aggFunc)
			: base(context) { _Context = context; _AggFunc = aggFunc; }

		/// <summary>获取分组汇总值</summary>
		/// <param name="source">汇总数据源</param>
		/// <returns>返回分组汇总值。</returns>
		public override object GetValue(IGrouping<dynamic, T> source)
		{
			return _AggFunc.Invoke(source);
		}
	}

	/// <summary>
	/// 分组项输出时表示表格的Cell
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class GroupTitle<T> : GroupCell<T>
	{
		private readonly Func<IGrouping<dynamic, T>, string> _AggFunc;
		private readonly IBasicContext _Context;

		/// <summary>
		/// 初始化 GroupTitle 类实例
		/// </summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		/// <param name="aggFunc"><![CDATA[计算序列IEnumerable<T>的聚合函数。]]></param>
		public GroupTitle(IBasicContext context, Func<IGrouping<dynamic, T>, string> aggFunc)
			: base(context) { _Context = context; _AggFunc = aggFunc; }

		/// <summary>获取分组汇总值</summary>
		/// <param name="source">汇总数据源</param>
		/// <returns>返回分组汇总值。</returns>
		public override object GetValue(IGrouping<dynamic, T> source)
		{
			return _AggFunc.Invoke(source);
		}
	}
}
