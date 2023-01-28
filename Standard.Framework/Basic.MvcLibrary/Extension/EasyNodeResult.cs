using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 根据树节点返回
	/// </summary>
	public sealed class EasyNodeResult : ActionResult
	{
		private readonly IEnumerable<EasyTreeNode> source;
		/// <summary>
		/// 初始化 TreeResult 类实例
		/// </summary>
		/// <param name="dataSource">树形结构数据源</param>
		public EasyNodeResult(IEnumerable<EasyTreeNode> dataSource) { source = dataSource; }

		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">用于执行结果的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		public override void ExecuteResult(ControllerContext context)
		{
			HttpResponseBase response = context.HttpContext.Response;
			response.Clear();
			response.ContentType = "application/json";
			if (source == null || (source != null && source.Count() == 0))
			{
				response.Write("[]"); return;
			}
			response.Write("["); int rowIndex = 0;
			foreach (EasyTreeNode node in source)
			{
				if (rowIndex > 0)
					response.Write(",");
				else
					rowIndex++;
				response.Write("{");
				node.WriteNodeJson(response);
				response.Write("}");
			}
			response.Write("]");
		}
	}
}
