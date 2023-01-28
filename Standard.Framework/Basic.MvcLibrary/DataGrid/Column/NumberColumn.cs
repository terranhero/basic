using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示行号列
	/// </summary>
	public sealed class NumberColumn<T> : DataGridColumn<T> where T : class
	{
		private int _CurrentNumber = 1;
		/// <summary>
		/// 初始化NumberColumn列信息
		/// </summary>
		/// <param name="context">当前 HTTP 请求的上下文信息。</param>
		/// <param name="startNumber">表示当前行号起始值</param>
		internal NumberColumn(IBasicContext context, int startNumber)
			: base(context) { _CurrentNumber = startNumber; }

		/// <summary>当前行号</summary>
		internal int CurrentNumber { get { return _CurrentNumber; } set { _CurrentNumber = value; } }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="startNumber">表示当前行号起始值</param>
		/// <returns>返回模型的值</returns>
		public void SetRowNumber(int startNumber)
		{
			_CurrentNumber = startNumber;
		}

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override string GetString(T model)
		{
			return Convert.ToString(_CurrentNumber++);
		}


		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override object GetValue(T model)
		{
			return Convert.ToString(_CurrentNumber++);
		}

	}
}
