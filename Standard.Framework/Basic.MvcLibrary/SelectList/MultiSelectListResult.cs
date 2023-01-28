using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 获取easyui下拉框数据源结果
	/// </summary>
	public class MultiSelectListResult : ActionResult
	{
		private readonly MultiSelectList source;

		/// <summary>
		/// 初始化 EntityComboResult 类实例
		/// </summary>
		/// <param name="dataSource">树形结构数据源</param>
		public MultiSelectListResult(MultiSelectList dataSource) { source = dataSource; }

		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">用于执行结果的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		public override void ExecuteResult(ControllerContext context)
		{
			HttpResponseBase response = context.HttpContext.Response;
			response.Clear(); response.ContentType = "application/json";
			if (source == null || source == null)
			{
				response.Write("[]");
				return;
			}
			response.Write("["); int rowIndex = 0;
			foreach (SelectListItem item in source)
			{
				if (rowIndex > 0)
					response.Write(",");
				else
					rowIndex++;
				response.Write("{");
				response.Write(string.Format("\"value\":\"{0}\"", item.Value));
				response.Write(string.Format(",\"text\":\"{0}\"", item.Text));
				if (item.Selected)
					response.Write(string.Format(",\"selected\":{0}", item.Selected ? "true" : "false"));
				response.Write("}");
			}
			response.Write("]");
		}
	}
}
