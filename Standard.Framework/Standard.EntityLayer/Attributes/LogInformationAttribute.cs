using Basic.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Loggers
{
	/// <summary>
	/// 表示一个特性，该特性用于无限制调用方对操作方法的访问。
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class LogInformationAttribute : Attribute
	{
		private readonly string _Information;
		private readonly string[] _FormatProperties;
		/// <summary>
		/// 初始化 Basic.Loggers.LogInformationAttribute 类实例
		/// </summary>
		/// <param name="msg">一个字符串对象，表示自定义日志信息。</param>
		/// <example><![CDATA[LogInformation("新增角色")]]></example>
		public LogInformationAttribute(string msg)
		{
			_Information = msg; _FormatProperties = null;
		}

		/// <summary>
		/// 初始化 Basic.Loggers.LogInformationAttribute 类实例
		/// </summary>
		/// <param name="msg">一个字符串对象，表示自定义日志信息。</param>
		/// <param name="args1">一个字符串对象，表示格式话msg参数对象的其中一个值。</param>
		/// <example><![CDATA[LogInformation("新增角色'{0}'", "RoleName")]]></example>
		public LogInformationAttribute(string msg, string args1)
		{
			if (args1 == null) { throw new ArgumentNullException("args1", "值不能为空！"); }
			_Information = msg;
			_FormatProperties = new string[] { args1 };
		}

		/// <summary>
		/// 初始化 Basic.LogInfo.LogInformationAttribute 类实例
		/// </summary>
		/// <param name="msg">一个字符串对象，表示自定义日志信息。</param>
		/// <param name="args1">一个字符串对象，表示格式话msg参数对象的其中一个值。</param>
		/// <param name="args2">一个字符串对象，表示格式话msg参数对象的其中一个值。</param>
		/// <example><![CDATA[LogInformation("新增角色'{0}','{1}'", "RoleKey", "RoleName")]]></example>
		public LogInformationAttribute(string msg, string args1, string args2)
		{
			if (args1 == null) { throw new ArgumentNullException("args1", "值不能为空！"); }
			if (args2 == null) { throw new ArgumentNullException("args2", "值不能为空！"); }
			_Information = msg;
			_FormatProperties = new string[] { args1, args2 };
		}

		/// <summary>
		/// 初始化 Basic.LogInfo.LogInformationAttribute 类实例
		/// </summary>
		/// <param name="msg">一个字符串对象，表示自定义日志信息。</param>
		/// <param name="args1">一个字符串对象，表示格式话msg参数对象的其中一个值。</param>
		/// <param name="args2">一个字符串对象，表示格式话msg参数对象的其中一个值。</param>
		/// <param name="args3">一个字符串对象，表示格式话msg参数对象的其中一个值。</param>
		/// <example><![CDATA[LogInformation("新增角色'{0}','{1}',{2}", "RoleKey", "RoleName", "EnusName")]]></example>
		public LogInformationAttribute(string msg, string args1, string args2, string args3)
		{
			if (args1 == null) { throw new ArgumentNullException("args1", "值不能为空！"); }
			if (args2 == null) { throw new ArgumentNullException("args2", "值不能为空！"); }
			if (args3 == null) { throw new ArgumentNullException("args3", "值不能为空！"); }
			_Information = msg;
			_FormatProperties = new string[] { args1, args2, args3 };
		}

		/// <summary>
		/// 初始化 Basic.LogInfo.LogInformationAttribute 类实例
		/// </summary>
		/// <param name="msg">一个字符串对象，表示自定义日志信息。</param>
		/// <param name="formatProperties">一个对象数组，其中包含零个或多个要设置格式的参数或参数属性。</param>
		public LogInformationAttribute(string msg, string[] formatProperties)
		{
			_Information = msg;
			_FormatProperties = formatProperties;
		}
		/// <summary>
		/// 获取解析的日志信息
		/// </summary>
		/// <param name="parameters">Action 方法参数名称/值。</param>
		/// <returns>返回格式化的日志信息。</returns>
		public string GetInformation(object[] parameters)
		{
			if (parameters == null || parameters.Length == 0)
				return _Information;
			List<string> objArray = new List<string>(parameters.Length);
			foreach (object param in parameters)
			{
				if (param == null) { continue; }
				else if (param is AbstractEntity)
					objArray.Add((param as AbstractEntity).ToString(false));
				else if (param is AbstractCondition)
					objArray.Add((param as AbstractCondition).ToString(false));
				else if (param is AbstractEntity[])
				{
					foreach (AbstractEntity entity in param as AbstractEntity[])
					{
						objArray.Add(entity.ToString(false));
					}
				}
				else if (param is AbstractCondition[])
				{
					foreach (AbstractCondition condition in param as AbstractCondition[])
					{
						objArray.Add(condition.ToString(false));
					}
				}
				else { objArray.Add(param.ToString()); }
			}
			return string.Format(_Information, string.Join(";", objArray.ToArray()));
		}

		/// <summary>
		/// 获取解析的日志信息
		/// </summary>
		/// <param name="actionParameters">Action 方法参数名称/值。</param>
		/// <returns>返回格式化的日志信息。</returns>
		public string GetInformation(IDictionary<string, object> actionParameters)
		{
			if (_FormatProperties == null || _FormatProperties.Length == 0)
				return _Information;
			object[] objArray = new object[_FormatProperties.Length];
			int index = 0;
			foreach (string parameterName in _FormatProperties)
			{
				objArray[index] = null;
				if (parameterName.IndexOf(".") >= 0)
				{
					#region 解析指定参数和属性的
					string[] strArray = parameterName.Split('.');
					string modelName = strArray[0], propertyName = strArray[1];
					if (actionParameters.ContainsKey(modelName))
					{
						object parameter = actionParameters[modelName];
						if (parameter is AbstractEntity)
						{
							AbstractEntity entity = parameter as AbstractEntity;
							EntityPropertyMeta propertyDescriptor = null;
							if (entity.TryGetProperty(propertyName, out propertyDescriptor))
								objArray[index] = propertyDescriptor.GetValue(parameter);
						}
					}
					#endregion
				}
				else
				{
					if (actionParameters.ContainsKey(parameterName))
					{
						object parameter = actionParameters[parameterName];
						if (parameter is AbstractCondition)
							objArray[index] = (parameter as AbstractCondition).ToString(false);
						else if (parameter is AbstractEntity)
							objArray[index] = (parameter as AbstractEntity).ToString(false);
						else
							objArray[index] = parameter;
					}
					else
					{
						foreach (var keyValue in actionParameters)
						{
							if (keyValue.Value is AbstractEntity)
							{
								AbstractEntity entity = keyValue.Value as AbstractEntity;
								EntityPropertyMeta propertyInfo = null;
								if (entity.TryGetProperty(parameterName, out propertyInfo))
									objArray[index] = propertyInfo.GetValue(entity);
							}
						}
					}
				}
				index++;
			}
			return string.Format(_Information, objArray);
		}

		/// <summary>
		/// 获取解析的日志信息
		/// </summary>
		/// <param name="entity">Action 方法参数名称/值。</param>
		/// <returns>返回格式化的日志信息。</returns>
		public string GetInformation(AbstractEntity entity)
		{
			if (entity is AbstractCondition)
				return string.Format(_Information, entity.ToString(false));
			return string.Format(_Information, entity.ToString(false));
		}

		/// <summary>
		/// 获取解析的日志信息
		/// </summary>
		/// <param name="entities">Action 方法参数名称/值。</param>
		/// <returns>返回格式化的日志信息。</returns>
		public string GetInformation(AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0) { return _Information; }
			List<string> list = new List<string>(entities.Length);
			foreach (AbstractEntity entity in entities)
			{
				if (entity == null) { continue; }
				list.Add(entity.ToString(false));
			}
			return string.Format(_Information, string.Join(";", list.ToArray()));
		}
	}
}
