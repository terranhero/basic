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
	public sealed class FooterInfo<T>
	{
		private readonly IBasicContext _Context;
		private readonly FooterRowCollection<T> _Rows;
		internal FooterInfo(IBasicContext context)
		{
			_Rows = new FooterRowCollection<T>(this);
			_Context = context;
		}

		/// <summary>
		/// 获取分组信息
		/// </summary>
		/// <returns></returns>
		public FooterRowCollection<T> GetRows() { return _Rows; }

		/// <summary>
		/// <![CDATA[将对象添加到 RowCollection<T> 的结尾处。]]>
		/// </summary>
		/// <returns><![CDATA[返回添加成功的 Row<T> 对象实例。]]></returns>
		public FooterRow<T> AddRow() { return _Rows.AddRow(); }

		/// <summary>Specifies the columns to use. </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public FooterCell<T> ValueFor<TR>(Expression<Func<IEnumerable<T>, TR>> expression)
		{
			FooterCell<T, TR> cell = new FooterCell<T, TR>(_Context, expression.Compile());
			_Rows.Add(cell);
			return cell;
		}

		/// <summary>Specifies the columns to use. </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public FooterCell<T> TitleFor(Expression<Func<IEnumerable<T>, string>> expression)
		{
			FooterTitle<T> column = new FooterTitle<T>(_Context, expression.Compile());
			_Rows.Add(column);
			return column;
		}

		/// <summary>
		/// 设置 RowStyle 属性值
		/// </summary>
		/// <param name="className">样式class名称</param>
		/// <returns>返回当前列对象。</returns>
		public FooterInfo<T> SetRowStyle(string className) { RowStyle = className; return this; }

		/// <summary>
		/// 获取分组行样式名称。
		/// </summary>
		public string RowStyle { get; internal set; }

		/// <summary>
		/// 设置分组行属性
		/// </summary>
		/// <param name="writer"></param>
		public void AddRowAttribute(System.Web.UI.HtmlTextWriter writer)
		{
			if (!string.IsNullOrWhiteSpace(RowStyle))
				writer.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Class, RowStyle);
		}
	}
}
