using System.Web.Mvc;
using Basic.EntityLayer;
using Basic.Interfaces;
using BM = Basic.MvcLibrary;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 控制器类扩展实例
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
	public static partial class ControllerExtension
	{
		#region 返回客户端Json数据
		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="success">当前Action是否执行成功</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, bool success)
		{
			return new BM.JsonResult(success);
		}

		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="success">当前Action是否执行成功</param>
		/// <param name="msg">操作执行成功，返回客户端的消息</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, bool success, string msg)
		{
			return new BM.JsonResult(success, msg);
		}

		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, AbstractEntity entity) { return new BM.JsonResult(entity); }

		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="contentType">内容的类型</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, AbstractEntity entity, string contentType)
		{
			return new BM.JsonResult(entity) { ContentType = contentType };
		}

		/// <summary>
		/// 返回执行失败的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, ModelStateDictionary modelState)
		{
			return new BM.JsonResult(modelState);
		}


		/// <summary>
		/// 返回执行失败的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <param name="contentType">内容的类型</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, ModelStateDictionary modelState, string contentType)
		{
			//ErrorResult errors = controller.CreateErrorResult(modelState);
			return new BM.JsonResult(modelState) { ContentType = contentType };
		}

		/// <summary>
		/// 返回执行失败的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, Result dbResult)
		{
			return new BM.JsonResult(dbResult);
		}

		/// <summary>
		/// 返回执行失败的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <param name="contentType">内容的类型</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, Result dbResult, string contentType)
		{
			return new BM.JsonResult(dbResult) { ContentType = contentType };
		}

		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, AbstractEntity[] entities)
		{
			return new BM.JsonResult(entities);
		}

		/// <summary>
		/// 返回执行失败的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, AbstractEntity[] entities, Result dbResult)
		{
			return new BM.JsonResult(entities, dbResult);
		}

		/// <summary>
		/// 返回执行失败的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.JsonResult ToJson(this ControllerBase controller, AbstractEntity[] entities, ModelStateDictionary modelState)
		{
			return new BM.JsonResult(entities, modelState);
		}

		/// <summary>
		/// 返回执行失败的JsonResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <returns>返回BM.JsonResult类实例</returns>
		public static BM.ModelResult<T> ToJson<T>(this ControllerBase controller, IPagination<T> entities)
		{
			return new BM.ModelResult<T>(entities);
		}
		#endregion
	}
}
