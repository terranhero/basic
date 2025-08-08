using Basic.EntityLayer;
using Basic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 返回树形Grid需要的数据格式。
	/// </summary>
	public sealed class DataGridResult<T> : IActionResult where T : class
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
		public Task ExecuteResultAsync(ActionContext context)
		{
			HttpResponse response = context.HttpContext.Response;
			//response.Clear();
			if (response.HasStarted)
			{
				throw new System.InvalidOperationException("The response cannot be cleared, it has already started sending.");
			}
			response.StatusCode = 200;
			//response.HttpContext.Features.Get<IHttpResponseFeature>()!.ReasonPhrase = null;
			response.Headers.Clear();
			if (response.Body.CanSeek)
			{
				response.Body.SetLength(0);
			}


			response.ContentType = "application/json";
			if (source == null)
			{
				response.WriteAsync("{\"Success\":true,\"total\":0,\"rows\":[]");
				if (!string.IsNullOrWhiteSpace(message))
				{
					response.WriteAsync(",\"Message\":\"");
					response.WriteAsync(message); response.WriteAsync("\"");
				}
				return response.WriteAsync("}");
			}
			response.WriteAsync(string.Concat("{\"Success\":true,\"total\":", source.Capacity));
			if (!string.IsNullOrWhiteSpace(message))
			{
				response.WriteAsync(",\"Message\":\"");
				response.WriteAsync(message); response.WriteAsync("\"");
			}
			response.WriteAsync(",\"rows\":[");
			if (source.Count > 0)
			{
				int rowIndex = 0;
				foreach (T entity in source)
				{
					if (rowIndex > 0) { response.WriteAsync(","); }
					rowIndex++;
					response.WriteAsync(JsonSerializer.Serialize(entity, true));
				}
			}
			return response.WriteAsync("]}");
		}

	}


}
