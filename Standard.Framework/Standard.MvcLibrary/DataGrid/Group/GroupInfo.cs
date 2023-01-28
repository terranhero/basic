using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Basic.EntityLayer;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示
	/// </summary>
	public sealed class GroupInfo<T>
	{
		private readonly Func<T, object> _KeySelector;
		private readonly IBasicContext _Context;
		private readonly GroupRowCollection<T> _Rows;
		internal GroupInfo(IBasicContext context, Func<T, object> expression)
		{
			_Rows = new GroupRowCollection<T>(this);
			_KeySelector = expression; _Context = context;
		}

		/// <summary>
		/// 获取分组信息
		/// </summary>
		/// <returns></returns>
		public GroupRowCollection<T> GetRows() { return _Rows; }

		/// <summary>
		/// <![CDATA[将对象添加到 RowCollection<T> 的结尾处。]]>
		/// </summary>
		/// <returns><![CDATA[返回添加成功的 Row<T> 对象实例。]]></returns>
		public GroupRow<T> AddRow() { return _Rows.AddRow(); }

		/// <summary>Specifies the columns to use. </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public GroupCell<T> ValueFor<TR>(Expression<Func<IGrouping<object, T>, TR>> expression)
		{
			GroupCell<T, TR> cell = new GroupCell<T, TR>(_Context, expression.Compile());
			_Rows.Add(cell);
			return cell;
		}

		/// <summary>Specifies the columns to use. </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public GroupCell<T> TitleFor(Expression<Func<IGrouping<object, T>, string>> expression)
		{
			GroupTitle<T> column = new GroupTitle<T>(_Context, expression.Compile());
			_Rows.Add(column);
			return column;
		}

		/// <summary>
		/// 获取分组后的数据源
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		internal IEnumerable<IGrouping<object, T>> GroupBy(IEnumerable<T> source)
		{
			return source.GroupBy(_KeySelector);
		}

		/// <summary>
		/// 设置 RowStyle 属性值
		/// </summary>
		/// <param name="className">样式class名称</param>
		/// <returns>返回当前列对象。</returns>
		public GroupInfo<T> SetRowStyle(string className) { RowStyle = className; return this; }

		/// <summary>
		/// 获取分组行样式名称。
		/// </summary>
		public string RowStyle { get; internal set; }

		///// <summary>
		///// 设置分组行属性
		///// </summary>
		///// <param name="writer"></param>
		//public void AddRowAttribute(System.Web.UI.HtmlTextWriter writer)
		//{
		//	if (!string.IsNullOrWhiteSpace(RowStyle))
		//		writer.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Class, RowStyle);
		//}
	}
}
