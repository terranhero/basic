using System;
using Basic.EntityLayer;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 枚举类型列信息
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TR"></typeparam>
	public class DataGridJsonColumn<T, TR> : DataGridColumn<T, TR> where T : class
	{
		/// <summary>
		/// 初始化 DataGridJsonColumn 类实例
		/// </summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		/// <param name="field">当前列字段名或属性名</param>
		/// <param name="valueFunc">目标值计算公式(从一个表达式，标识包含要呈现的属性的对象)</param>
		/// <param name="metaData">当前列对应的属性元数据</param>
		internal protected DataGridJsonColumn(IBasicContext context, string field, Func<T, TR> valueFunc, EntityPropertyMeta metaData)
			: base(context, field, valueFunc, metaData) { NotExport(); base.JsonData = true; }
	}
}
