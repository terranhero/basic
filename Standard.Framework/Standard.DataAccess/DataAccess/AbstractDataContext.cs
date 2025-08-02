using System;
using System.Linq.Expressions;
using Basic.Collections;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Interfaces;
using Basic.Messages;

namespace Basic.DataAccess
{
	/// <summary>数据上下文管理类</summary>
	public abstract class AbstractDataContext
	{
		private readonly IUserContext _User;
		/// <summary>
		/// 创建 AbstractDataContext 类实例，AbstractDataContext 类为抽象类，所以此方法紧供子类继承调用
		/// </summary>
		protected AbstractDataContext(IUserContext context) { _User = context; }

		/// <summary>
		/// 创建 AbstractDataContext 类实例，AbstractDataContext 类为抽象类，所以此方法紧供子类继承调用
		/// </summary>
		protected AbstractDataContext() { _User = new UserContext(ConnectionContext.DefaultName); }

		/// <summary>
		/// 创建 AbstractDataContext 类实例，AbstractDataContext 类为抽象类，所以此方法紧供子类继承调用
		/// </summary>
		/// <param name="connection">数据库连接名称</param>
		protected AbstractDataContext(string connection) { _User = new UserContext(connection); }

		/// <summary>
		/// 获取当前数据上下文管理类的缓存对象
		/// </summary>
		/// <returns>如果存在系统则返回缓存 ICacheClient 接口的实例，否则返回null。</returns>
		protected ICacheClient GetClient() { return CacheClientFactory.GetClient(_User.Connection); }

		/// <summary>
		/// 获取当前数据上下文管理类的缓存对象
		/// </summary>
		/// <returns>如果存在系统则返回缓存 ICacheClient 接口的实例，否则返回null。</returns>
		protected ICacheClient GetClient(string name) { return CacheClientFactory.GetClient(name); }

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		internal protected string Connection { get { return _User.Connection; } }

		/// <summary>获取用户上下文信息</summary>
		internal protected IUserContext Context { get { return _User; } }

		#region 获取区域字符串资源
		/// <summary>使用 Lambda 表达式获取布尔类型属性值。</summary>
		/// <param name="expression">要获取的资源名。</param>
		/// <param name="item">判断参数</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString<TM>(TM item, Expression<Func<TM, bool>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			bool value = expression.Compile().Invoke(item);
			EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta propertyInfo);

			if (propertyInfo != null && propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converter = wda.ConverterName;
				string valueText = string.Concat(wda.DisplayName, "_", value ? "True" : "False", "Text");

				if (_User.Culture != null) { return MessageContext.GetString(converter, valueText, _User.Culture); }
				return MessageContext.GetString(converter, valueText);
			}

			if (_User.Culture != null) { return MessageContext.GetString(name, _User.Culture); }
			return MessageContext.GetString(name);
		}

		/// <summary>使用 Lambda 表达式获取布尔类型属性值。</summary>
		/// <param name="expression">要获取的资源名。</param>
		/// <param name="item">判断参数</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString<TM>(TM item, Expression<Func<TM, Enum>> expression)
		{
			Enum value = expression.Compile().Invoke(item); Type enumType = value.GetType();
			string converter = null;
			if (Attribute.IsDefined(enumType, typeof(WebDisplayConverterAttribute)))
			{
				WebDisplayConverterAttribute wdca = (WebDisplayConverterAttribute)Attribute.GetCustomAttribute(enumType, typeof(WebDisplayConverterAttribute));
				converter = wdca.ConverterName;
			}
			string enumName = enumType.Name;
			string name = Enum.GetName(enumType, value);
			string itemName = string.Concat(enumName, "_", name);

			if (_User.Culture != null) { return MessageContext.GetString(converter, itemName, _User.Culture); }
			return MessageContext.GetString(converter, itemName);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name)
		{
			if (_User.Culture != null) { return MessageContext.GetString(name, _User.Culture); }
			return MessageContext.GetString(name);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
		public string GetString(string name, params object[] args)
		{
			if (_User.Culture != null) { return MessageContext.GetString(name, _User.Culture, args); }
			return MessageContext.GetString(name, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public string GetString(string converterName, string name)
		{
			if (_User.Culture != null) { return MessageContext.GetString(converterName, name, _User.Culture); }
			return MessageContext.GetString(converterName, name);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public string GetString(string converterName, string name, params object[] args)
		{
			if (_User.Culture != null) { return MessageContext.GetString(converterName, name, _User.Culture, args); }
			return MessageContext.GetString(converterName, name, args);
		}
		#endregion

		#region 返回 Result 实例的错误信息
		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		protected Result Error(string name)
		{
			return new Result(GetString(name));
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
		public Result Error(string name, params object[] args)
		{
			return new Result(GetString(name, args));
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public Result Error(string converterName, string name)
		{
			return new Result(GetString(converterName, name));
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public Result Error(string converterName, string name, params object[] args)
		{
			return new Result(GetString(converterName, name, args));
		}
		#endregion
	}
}
