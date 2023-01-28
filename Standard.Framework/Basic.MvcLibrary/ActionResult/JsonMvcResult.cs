using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Basic.EntityLayer;
using Basic.Validations;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 返回自定义JsonMvcResult格式
	/// </summary>
	public sealed class JsonMvcResult : System.Web.Mvc.JsonResult
	{
		private readonly bool _Successful = true;
		private readonly bool _SendClientData = true;
		private readonly string _Message;
		private readonly AbstractEntity _Entity;
		private readonly AbstractEntity[] _Entities;
		private readonly ModelStateDictionary _ModelState;
		private readonly Result _Result;

		/// <summary>
		/// 获取一个布尔类型的值，表示当前方法是否执行成功。
		/// </summary>
		public bool Successful { get { return _Successful; } }

		/// <summary>
		/// 当前Action执行时，使用的 AbstractEntity 类型参数。
		/// </summary>
		public AbstractEntity Entity { get { return _Entity; } }

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
		internal JsonMvcResult(bool successful) { _Successful = successful; }

		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="successful">如果执行成功的为true，否则为false</param>
		/// <param name="msg">操作执行成功，返回客户端的消息。</param>
		internal JsonMvcResult(bool successful, string msg) { _Successful = successful; _Message = msg; }

		/// <summary>
		/// 返回执行成功的JsonResult
		/// </summary>
		/// <param name="entity">执行成功的实体类信息</param>
		internal JsonMvcResult(AbstractEntity entity) : this(entity, null, null, null, true) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="sendClientData">当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。</param>
		internal JsonMvcResult(AbstractEntity entity, bool sendClientData)
			: this(entity, null, null, null, sendClientData) { }

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="entity">执行失败的实体类信息</param>
		/// <param name="error">模型状态中的错误信息</param>
		internal JsonMvcResult(AbstractEntity entity, ModelStateDictionary error) : this(entity, null, null, error, true) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="error">当前Action执行的模型错误信息</param>
		/// <param name="sendClientData">当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。</param>
		internal JsonMvcResult(AbstractEntity entity, ModelStateDictionary error, bool sendClientData)
			: this(entity, null, null, error, sendClientData) { }

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="entity">执行失败的实体类信息</param>
		/// <param name="dbResult">模型状态中的错误信息</param>
		internal JsonMvcResult(AbstractEntity entity, Result dbResult) : this(entity, null, dbResult, null, true) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">执行失败的实体类信息</param>
		/// <param name="sendClientData">当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。</param>
		internal JsonMvcResult(AbstractEntity entity, Result dbResult, bool sendClientData)
			: this(entity, null, dbResult, null, sendClientData) { }

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="error">模型状态中的错误信息</param>
		internal JsonMvcResult(ModelStateDictionary error) : this(null, null, null, error, true) { }

		/// <summary>
		/// 返回执行结果的JsonMvcResult
		/// </summary>
		/// <param name="dbResult">执行失败的实体类信息</param>
		internal JsonMvcResult(Result dbResult) : this(null, null, dbResult, null, true) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		internal JsonMvcResult(AbstractEntity[] entities) : this(null, entities, null, null, true) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="sendClientData">当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。</param>
		internal JsonMvcResult(AbstractEntity[] entities, bool sendClientData)
			: this(null, entities, null, null, sendClientData) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">执行失败的实体类信息</param>
		internal JsonMvcResult(AbstractEntity[] entities, Result dbResult) : this(null, entities, dbResult, null, true) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">执行失败的实体类信息</param>
		/// <param name="sendClientData">当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。</param>
		internal JsonMvcResult(AbstractEntity[] entities, Result dbResult, bool sendClientData)
			: this(null, entities, dbResult, null, sendClientData) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="error">当前Action执行的模型错误信息</param>
		internal JsonMvcResult(AbstractEntity[] entities, ModelStateDictionary error) : this(null, entities, null, error, true) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="error">当前Action执行的模型错误信息</param>
		/// <param name="sendClientData">当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。</param>
		internal JsonMvcResult(AbstractEntity[] entities, ModelStateDictionary error, bool sendClientData)
			: this(null, entities, null, error, sendClientData) { }

		/// <summary>
		/// 返回JsonMvcResult类实例
		/// </summary>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">执行失败的实体类信息</param>
		/// <param name="error">当前Action执行的模型错误信息</param>
		/// <param name="sendClientData">当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。</param>
		private JsonMvcResult(AbstractEntity entity, AbstractEntity[] entities, Result dbResult, ModelStateDictionary error, bool sendClientData)
		{
			_SendClientData = sendClientData;
			ContentType = "application/json";
			_Successful = true; _Entities = entities;
			_Entity = entity; _ModelState = error; _Result = dbResult;
			if (_Result == null) { _Result = Result.Empty; }
			if (_Result.Failure) { _Successful = false; }
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
			response.Write(string.Format("\"Success\":{0},\"total\":0,\"rows\":[]", _Successful ? "true" : "false"));

			if (!string.IsNullOrWhiteSpace(_Message))
			{
				response.Write(",\"Message\":\""); response.Write(_Message); response.Write("\"");
			}
			if (_Entities != null && _SendClientData)
			{
				response.Write(",\"Entity\":");
				response.Write(JsonSerializer.SerializeEntity(_Entities, true));
			}
			if (_Entity != null && _SendClientData)
			{
				response.Write(",\"Entity\":");
				response.Write(JsonSerializer.SerializeEntity(_Entity, true));
			}
			CreateResult();
			if (_Result != null && _Result.Failure)
			{
				response.Write(",\"Error\":");
				response.Write(JsonSerializer.SerializeObject(_Result.Errors, false));
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
						int start = ms.Key.IndexOf('[');
						string strIndex = ms.Key.Substring(start + 1, ms.Key.IndexOf(']') - start - 1);
						if (int.TryParse(strIndex, out index) == false) { index = -1; }
					}
					_Result.AddError(index, name, errorMessage);
				}
			}
			if (_Entity != null && _Entity.HasError())
			{
				foreach (ValidationPropertyResult validationResult in _Entity.GetError())
				{
					if (!validationResult.HasError) { continue; }
					if (string.IsNullOrEmpty(validationResult.PropertyName)) { continue; }
					if (AbstractEntity.NullPropertyName == validationResult.PropertyName)
					{
						_Result.AddError("", validationResult.ErrorMessage);
					}
					else
					{
						_Result.AddError(validationResult.PropertyName, validationResult.ErrorMessage);
					}
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
		/// <summary>
		/// 整理错误信息
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, string> CreateErrorResult()
		{
			Dictionary<string, string> errors = new Dictionary<string, string>();
			List<string> summary = new List<string>();
			if (_Result != null && _Result.Failure)
			{
				foreach (ResultError error in _Result.Errors)
				{
					if (string.IsNullOrWhiteSpace(error.Name))
						summary.Add(error.Message);
					else
						errors.Add(error.Name, error.Message);
				}
			}
			if (_ModelState != null)
			{
				foreach (var ms in _ModelState)
				{
					foreach (ModelError error in ms.Value.Errors)
					{
						if (string.IsNullOrEmpty(ms.Key) && string.IsNullOrEmpty(error.ErrorMessage)) { continue; }
						if (string.IsNullOrEmpty(ms.Key)) { summary.Add(error.ErrorMessage); }
						else
						{
							if (errors.ContainsKey(ms.Key))
							{
								if (errors[ms.Key] != error.ErrorMessage)
								{
									errors[ms.Key] = string.Format("{0},{1}", errors[ms.Key], error.ErrorMessage);
								}
							}
							else { errors.Add(ms.Key, error.ErrorMessage); }
						}
					}
				}
			}
			if (_Entity != null && _Entity.HasError())
			{
				foreach (var keyValue in _Entity.GetError())
				{
					if (string.IsNullOrEmpty(keyValue.PropertyName) && string.IsNullOrEmpty(keyValue.ErrorMessage)) { continue; }
					if (AbstractEntity.NullPropertyName == keyValue.PropertyName) { summary.Add(keyValue.ErrorMessage); }
					else
					{
						if (errors.ContainsKey(keyValue.PropertyName))
						{
							if (errors[keyValue.PropertyName] != keyValue.ErrorMessage)
							{
								errors[keyValue.PropertyName] = string.Format("{0},{1}", errors[keyValue.PropertyName], keyValue.ErrorMessage);
							}
						}
						else { errors.Add(keyValue.PropertyName, keyValue.ErrorMessage); }
					}
				}
			}
			if (summary.Count > 0)
				errors[string.Empty] = string.Join("\\r\\n", summary.ToArray());
			return errors;
		}
	}
}
