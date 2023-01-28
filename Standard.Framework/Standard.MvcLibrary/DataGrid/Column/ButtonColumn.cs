using Basic.MvcLibrary;
using System;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示行号列
	/// </summary>
	public sealed class ButtonColumn<T> : DataGridColumn<T> where T : class
	{
		/// <summary>获取当前列的计算返回值的委托。</summary>
		internal readonly Func<T, string> _ValueFunc;

		/// <summary>初始化 ButtonColumn 列信息</summary>
		/// <param name="context">当前 HTTP 请求的上下文信息。</param>
		/// <param name="func">目标值计算公式(从一个表达式，标识包含要呈现的属性的对象)</param>
		internal ButtonColumn(IBasicContext context, Func<T, string> func) : base(context)
		{
			_ValueFunc = func; NotExport();
			SetField("Buttons"); SetTitle(typeof(EasyStrings).Name, "Button_Options");
		}

		/// <summary>获取当前列实体类的值</summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override object GetValue(T model) { return _ValueFunc(model); }

		/// <summary>获取当前列实体类的值</summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override string GetString(T model)
		{
			return _ValueFunc(model);
		}
	}
}
