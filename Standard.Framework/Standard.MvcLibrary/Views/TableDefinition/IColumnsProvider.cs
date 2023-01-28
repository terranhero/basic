using System;
using System.Linq.Expressions;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public interface IColumnsProvider<T> where T : class
	{
		/// <summary>表示基础扩展类，包装有Http请求的上下文信息和用户信息</summary>
		IBasicContext Basic { get; }

		/// <summary>生成数组列头</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回生成的列对象</returns>
		GridViewHeaderColumn<T> HeaderFor<TR>(Expression<Func<T, TR>> expression);

		/// <summary>
		/// 使用Lambda表达式设置标题字段。
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回生成的列对象</returns>
		GridViewHeaderColumn<T> HeaderFor(string expression);

		/// <summary>使用 Lambda 表达式设置标题字段。</summary>
		/// <param name="converterName">资源转换器名称</param>
		/// <param name="name">要设置的属性新值</param>
		/// <returns>返回生成的列对象</returns>
		GridViewHeaderColumn<T> HeaderFor(string converterName, string name);

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回生成的列对象</returns>
		GridViewColumn<T, RT> LabelFor<RT>(Expression<Func<T, RT>> expression);

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回生成的列对象</returns>
		GridViewColumn<T, Enum> EnumFor(Expression<Func<T, Enum>> expression);

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回生成的列对象</returns>
		GridViewColumn<T, bool> BooleanFor(Expression<Func<T, bool>> expression);

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回生成的列对象</returns>
		GridViewButtonColumn<T> ButtonsFor(Action<IButtonsProvider<T>> expression);

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="codes">需要在按钮列中验证的权限编码</param>
		/// <returns>返回生成的列对象</returns>
		GridViewButtonColumn<T> ButtonsFor(Action<IButtonsProvider<T>> expression, params int[] codes);

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="authorized">是否授权显示此列</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回生成的列对象</returns>
		GridViewButtonColumn<T> ButtonsFor(bool authorized, Action<IButtonsProvider<T>> expression);

		///// <summary>使用 Lambda 表达式创建列字段。</summary>
		///// <param name="authorizeCode">是否根据授权码确定显示此列</param>
		///// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		///// <returns>返回生成的列对象</returns>
		//GridViewButtonColumn<T> ButtonsFor(int authorizeCode, Action<IButtonsProvider<T>> expression);
	}
}
