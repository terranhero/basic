using System;
using System.Reflection;
using Basic.Exceptions;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示当前数据表和实体类与之对应的Access类实例
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class AccessConfigurationAttribute : Attribute
	{
		/// <summary>
		/// 获取此特性与之关联的AbstractAccess子类实例
		/// </summary>
		public Type AccessType { get; private set; }
		/// <summary>
		/// 初始化AccessConfigurationAttribute类实例
		/// </summary>
		/// <param name="accessType">此特性与之关联的AbstractAccess子类实例</param>
		public AccessConfigurationAttribute(Type accessType)
		{
			AccessType = accessType;
		}

		/// <summary>
		/// 获取AbstractAccess子类实例
		/// </summary>
		/// <returns></returns>
		internal AbstractAccess CreateAccess()
		{
			if (!AccessType.IsSubclassOf(typeof(AbstractAccess)))
				throw new ParameterException("AbstractAccess_ConfigurationParameterType");
			ConstructorInfo info = AccessType.GetConstructor(new Type[0]);
			if (info == null)
				throw new ParameterException("AbstractAccess_NoParameterConstructor", AccessType);
			return info.Invoke(null) as AbstractAccess;
		}
	}
}
