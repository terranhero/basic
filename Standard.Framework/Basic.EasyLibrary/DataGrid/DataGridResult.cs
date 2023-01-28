using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Basic.Interfaces;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 返回树形Grid需要的数据格式。
	/// </summary>
	public sealed class DataGridResult<T> : ActionResult where T : class
	{
		private readonly IPagination<T> source; private readonly string message;

		/// <summary>
		/// 初始化 TreeResult 类实例
		/// </summary>
		/// <param name="dataSource">树形结构数据源</param>
		public DataGridResult(IPagination<T> dataSource) { source = dataSource; }

		/// <summary>
		/// 初始化 TreeResult 类实例
		/// </summary>
		/// <param name="dataSource">树形结构数据源</param>
		/// <param name="msg">显示的错误消息</param>
		public DataGridResult(IPagination<T> dataSource, string msg) { source = dataSource; message = msg; }

		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">用于执行结果的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		public override void ExecuteResult(ControllerContext context)
		{
			HttpResponseBase response = context.HttpContext.Response;
			response.Clear();
			response.ContentType = "application/json";
			if (source == null)
			{
				response.Write("{\"Success\":true,\"total\":0,\"rows\":[]");
				if (!string.IsNullOrWhiteSpace(message)) { response.Write(",\"Message\":\""); response.Write(message); response.Write("\""); }
				response.Write("}");
				return;
			}
			response.Write(string.Concat("{\"Success\":true,\"total\":", source.Capacity));
			if (!string.IsNullOrWhiteSpace(message)) { response.Write(",\"Message\":\""); response.Write(message); response.Write("\""); }
			response.Write(",\"rows\":[");
			if (source.Count > 0)
			{
				int rowIndex = 0;
				foreach (T entity in source)
				{
					if (rowIndex > 0) { response.Write(","); }
					rowIndex++;
					response.Write(JsonSerializer.SerializeObject(entity, true));
				}
			}
			response.Write("]}");
		}
	}
}
