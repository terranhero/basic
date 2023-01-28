using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Basic.EntityLayer;
using Basic.Interfaces;
using Basic.Validations;

namespace Basic.MvcLibrary
{
	/// <summary>客户端执行结果对象</summary>
	public sealed class JsonResult : System.Web.Mvc.JsonResult
	{
		private readonly bool _Successful = true;
		private readonly string _Message;
		private readonly int _TotalCount = 0;
		private readonly AbstractEntity[] _Entities = null;
		private readonly AbstractEntity _Entity = null;
		private readonly ModelStateDictionary _ModelState = null;
		private readonly Result _Result = null;

		/// <summary>
		/// 获取一个布尔类型的值，表示当前方法是否执行成功。
		/// </summary>
		public bool Successful { get { return _Successful; } }

		/// <summary>
		/// 当前Action执行时，使用的 AbstractEntity[] 类型参数。
		/// </summary>
		public AbstractEntity[] Entities { get { return _Entities; } }

		/// <summary>
		/// 验证结果的键值对模型
		/// </summary>
		public ModelStateDictionary ModelState { get { return _ModelState; } }

		/// <summary>
		/// 数据执行结果
		/// </summary>
		public Result Result { get { return _Result; } }

		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="successful">如果执行成功的为true，否则为false</param>
		internal JsonResult(bool successful) { _Successful = successful; }

		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="successful">如果执行成功的为true，否则为false</param>
		/// <param name="msg">操作执行成功，返回客户端的消息。</param>
		internal JsonResult(bool successful, string msg) { _Successful = successful; _Message = msg; }

		/// <summary>
		/// 返回执行失败的JsonResult
		/// </summary>
		/// <param name="error">模型状态中的错误信息</param>
		internal JsonResult(ModelStateDictionary error) : this(null, null, null, error) { }

		/// <summary>
		/// 返回执行结果的JsonResult
		/// </summary>
		/// <param name="dbResult">执行失败的实体类信息</param>
		internal JsonResult(Result dbResult) : this(null, null, dbResult, null) { }

		/// <summary>返回JsonResult类实例</summary>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		internal JsonResult(AbstractEntity entity) : this(null, entity, null, null) { }

		/// <summary>
		/// 返回JsonResult类实例
		/// </summary>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="error">当前Action执行的模型错误信息</param>
		internal JsonResult(AbstractEntity entity, ModelStateDictionary error) : this(null, entity, null, error) { }

		/// <summary>
		/// 返回JsonResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		internal JsonResult(AbstractEntity[] entities) : this(entities, null, null, null) { }

		/// <summary>
		/// 返回JsonResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">执行失败的实体类信息</param>
		internal JsonResult(AbstractEntity[] entities, Result dbResult)
			: this(entities, null, dbResult, null) { }

		/// <summary>
		/// 返回JsonResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="error">当前Action执行的模型错误信息</param>
		internal JsonResult(AbstractEntity[] entities, ModelStateDictionary error) : this(entities, null, null, error) { }


		/// <summary>
		/// 返回JsonResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		public JsonResult(IPagination<AbstractEntity> entities)
		{
			_TotalCount = entities.Capacity; ContentType = "application/json";
			_Successful = true; _Entities = entities.ToArray();
		}

		/// <summary>
		/// 返回JsonResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">执行失败的实体类信息</param>
		/// <param name="error">当前Action执行的模型错误信息</param>
		private JsonResult(AbstractEntity[] entities, AbstractEntity entity, Result dbResult, ModelStateDictionary error)
		{
			ContentType = "application/json"; _Entity = entity;
			_Successful = true; _Entities = entities;
			_ModelState = error; _Result = dbResult;
			if (_Result == null) { _Result = Result.Empty; }
			if (_Result.Failure) { _Successful = false; }
			if (_Result.Successful && string.IsNullOrWhiteSpace(_Result.Message) == false) { _Message = _Result.Message; }
			if (_ModelState != null && !_ModelState.IsValid) { _Successful = false; }
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
			response.Write(string.Format("\"Success\":{0},\"total\":{1}", _Successful ? "true" : "false", _TotalCount));
			JsonConverter converter = new JsonConverter(context.HttpContext.Request);

			if (!string.IsNullOrWhiteSpace(_Message))
			{
				response.Write(",\"Message\":\""); response.Write(_Message); response.Write("\"");
			}
			if (_Entities != null)
			{
				response.Write(",\"rows\":");
				response.Write(converter.Serialize(_Entities, true));
			}
			if (_Entity != null)
			{
				response.Write(",\"row\":");
				response.Write(converter.Serialize(_Entity, true));
			}
			CreateResult();
			if (_Result != null && _Result.Failure)
			{
				response.Write(",\"errors\":");
				response.Write(converter.Serialize(_Result.Errors, false));
			}
			response.Write("}");
		}

		private void CreateResult()
		{
			if (_ModelState != null)
			{
				foreach (var ms in _ModelState)
				{
					if (ms.Value.Errors.Count == 0) { continue; }
					int index = -1; string name = ms.Key;
					string errorMessage = string.Join(",", ms.Value.Errors.Select(m => m.ErrorMessage).ToArray());
					if (ms.Key.IndexOf('[') >= 0 && ms.Key.IndexOf(']') >= 0 && ms.Key.IndexOf('.') >= 0)
					{
						string[] keyArray = ms.Key.Split('.');
						index = Convert.ToInt32(keyArray[0].Trim('[', ']'));
					}
					_Result.AddError(index, name, errorMessage);
				}
			}
			if (_Entities != null && _Entities.Length > 0)
			{
				for (int index = 0; index < _Entities.Length; index++)
				{
					AbstractEntity entity = _Entities[index];
					if (!entity.HasError()) { continue; }
					foreach (ValidationPropertyResult validationResult in entity.GetError())
					{
						if (!validationResult.HasError) { continue; }
						if (string.IsNullOrEmpty(validationResult.PropertyName)) { continue; }
						if (AbstractEntity.NullPropertyName == validationResult.PropertyName)
						{
							_Result.AddError(index, "", validationResult.ErrorMessage);
						}
						else
						{
							_Result.AddError(index, validationResult.PropertyName, validationResult.ErrorMessage);
						}
					}
				}
			}
		}
	}
}
