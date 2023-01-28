using System.Web;
using System.Web.Mvc;
using Basic.Interfaces;

namespace Basic.MvcLibrary
{
	/// <summary>客户端执行结果对象</summary>
	public sealed class ModelResult<T> : System.Web.Mvc.JsonResult
	{
		private readonly int _TotalCount = 0;
		private readonly IPagination<T> _Entities = null;

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
			_TotalCount = entities.Capacity; ContentType = "application/json";
			_Entities = entities;
		}

		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">执行结果时所处的上下文。</param>
		public override void ExecuteResult(ControllerContext context)
		{
			HttpResponseBase response = context.HttpContext.Response;
			response.Clear();
			response.ContentType = ContentType;
			response.Write("{");
			response.Write(string.Format("\"Success\":true,\"total\":{0}", _TotalCount));
			if (_Entities != null)
			{
				response.Write(",\"rows\":");
				JsonConverter convert = new JsonConverter(context.HttpContext.Request);
				response.Write(convert.Serialize(_Entities));
			}
			response.Write("}");
		}
	}
}
