using System;
using Basic.EntityLayer;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataGridBooleanColumn<T> : DataGridColumn<T, bool> where T : class
	{
		private readonly string TrueText;
		private readonly string FalseText;

		/// <summary>
		/// 初始化 DataGridBooleanColumn 列信息
		/// </summary>
		/// <param name="context">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="field">数据库字段名称</param>
		/// <param name="valueFunc">以表达式树的形式将强类型 lambda 表达式表示为数据结构。</param>
		/// <param name="metaData">包含当前字段的模型字典数据</param>
		/// <param name="trueText">如果Lambda表达式结果为True的显示文本</param>
		/// <param name="falseText">如果Lambda表达式结果为False的显示文本</param>
		internal protected DataGridBooleanColumn(IBasicContext context, string field, Func<T, bool> valueFunc, EntityPropertyMeta metaData, string trueText, string falseText)
			: base(context, field, valueFunc, metaData) { TrueText = trueText; FalseText = falseText; }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override string GetString(T model)
		{
			bool result = _ValueFunc((T)model);
			return result ? TrueText : FalseText;
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataGridNullableBooleanColumn<T> : DataGridColumn<T, bool?> where T : class
	{
		private readonly string TrueText;
		private readonly string FalseText;

		/// <summary>
		/// 初始化 DataGridNullableBooleanColumn 列信息
		/// </summary>
		/// <param name="context">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="field">数据库字段名称</param>
		/// <param name="valueFunc">以表达式树的形式将强类型 lambda 表达式表示为数据结构。</param>
		/// <param name="metaData">包含当前字段的模型字典数据</param>
		/// <param name="trueText">如果Lambda表达式结果为True的显示文本</param>
		/// <param name="falseText">如果Lambda表达式结果为False的显示文本</param>
		internal protected DataGridNullableBooleanColumn(IBasicContext context, string field, Func<T, bool?> valueFunc, EntityPropertyMeta metaData, string trueText, string falseText)
			: base(context, field, valueFunc, metaData) { TrueText = trueText; FalseText = falseText; }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override string GetString(T model)
		{
			bool? result = _ValueFunc((T)model);
			if (result == null) { return null; }
			return result.Value ? TrueText : FalseText;
		}
	}
}
