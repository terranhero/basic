using System;
using System.Globalization;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Basic.MvcLibrary
{
	/// <summary>客户端执行结果对象</summary>
	public sealed class ModelResult<T> : Microsoft.AspNetCore.Mvc.IActionResult
	{
		private readonly int _TotalCount = 0;
		private readonly IPagination<T> _Entities = null;
		private static CultureInfo _CultureInfo = new CultureInfo(2052);

		/// <summary>
		/// 当前Action执行时，使用的 AbstractEntity[] 类型参数。
		/// </summary>
		public IPagination<T> Entities { get { return _Entities; } }

		/// <summary>
		/// 返回JsonResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		public ModelResult(IPagination<T> entities)
		{
			_TotalCount = entities.Capacity;
			_Entities = entities;
		}

		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">执行结果时所处的上下文。</param>
		/// <returns></returns>
		public async Task ExecuteResultAsync(ActionContext context)
		{
			HttpResponse response = context.HttpContext.Response;
			response.Clear();
			response.ContentType = "application/json";
			await response.WriteAsync("{");
			await response.WriteAsync(string.Format("\"Success\":true,\"total\":{0}", _TotalCount));
			if (_Entities != null)
			{
				await response.WriteAsync(",\"rows\":");
				IServiceProvider provider = context.HttpContext.RequestServices;
				JsonConverter converter = provider.GetRequiredService<JsonConverter>();
				if (converter == null) { converter = new JsonConverter(_CultureInfo); }
				await response.WriteAsync(converter.Serialize(_Entities));
			}
			await response.WriteAsync("}");
		}
	}
}
