using Basic.EntityLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Basic.Collections
{
	/// <summary>
	/// 提供实体模型属性管理
	/// </summary>
	public static class EntityPropertyProvidor
	{
		private static readonly ConcurrentDictionary<string, EntityPropertyCollection> _properties = new ConcurrentDictionary<string, EntityPropertyCollection>();
		private static readonly ConcurrentDictionary<string, EntityPropertyCollection> _primaryKeys = new ConcurrentDictionary<string, EntityPropertyCollection>();

		/// <summary>
		/// 尝试从集合中获取实体属性信息，如果获取成功额为true，否则为false。
		/// </summary>
		/// <param name="type">继承于 AbstractEntity 的实体模型类型</param>
		/// <param name="properties">返回当前实体模型属性集合</param>
		/// <param name="pkProperties">返回当前实体模型关键字属性集合</param>
		/// <returns>如果包含具有指定键的元素，则为 true；否则为 false。</returns>
		public static bool TryGetProperties(Type type, out EntityPropertyCollection properties, out EntityPropertyCollection pkProperties)
		{
			string fullName = type.FullName;
			if (_properties.TryGetValue(fullName, out properties))
			{
				return _primaryKeys.TryGetValue(fullName, out pkProperties);
			}
			else
			{
				PropertyInfo[] pis = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				properties = new EntityPropertyCollection();
				pkProperties = new EntityPropertyCollection();
				if (pis == null || pis.Length == 0) { return false; }
				foreach (PropertyInfo propertyInfo in pis)
				{
					EntityPropertyMeta descriptor = new EntityPropertyMeta(propertyInfo);
					properties.Add(descriptor);
					if (descriptor.PrimaryKey) { pkProperties.Add(descriptor); }
				}
				_primaryKeys.TryAdd(type.FullName, pkProperties);
				_properties.TryAdd(type.FullName, properties);
				return true;
			}
		}

		/// <summary>
		/// 尝试从集合中获取实体属性信息，如果获取成功额为true，否则为false。
		/// </summary>
		/// <param name="type"> AbstractEntity 类型的实例</param>
		/// <param name="properties">返回当前实体模型属性集合</param>
		/// <returns>如果包含具有指定键的元素，则为 true；否则为 false。</returns>
		public static bool TryGetProperties(Type type, out EntityPropertyCollection properties)
		{
			if (_properties.TryGetValue(type.FullName, out properties))
			{
				return true;
			}
			PropertyInfo[] pis = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			properties = new EntityPropertyCollection();
			if (pis == null || pis.Length == 0) { return false; }
			EntityPropertyCollection pkProperties = new EntityPropertyCollection();
			foreach (PropertyInfo propertyInfo in pis)
			{
				EntityPropertyMeta descriptor = new EntityPropertyMeta(propertyInfo);
				properties.Add(descriptor);
				if (descriptor.PrimaryKey) { pkProperties.Add(descriptor); }
			}
			_primaryKeys.TryAdd(type.FullName, pkProperties);
			_properties.TryAdd(type.FullName, properties);
			return true;
			//lock (_properties)
			//{
			//	if (!_properties.TryGetValue(type.FullName, out properties))
			//	{
			//		PropertyInfo[] pis = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			//		properties = new EntityPropertyCollection();
			//		if (pis == null || pis.Length == 0) { return false; }
			//		EntityPropertyCollection pkProperties = new EntityPropertyCollection();
			//		foreach (PropertyInfo propertyInfo in pis)
			//		{
			//			EntityPropertyMeta descriptor = new EntityPropertyMeta(propertyInfo);
			//			properties.Add(descriptor);
			//			if (descriptor.PrimaryKey) { pkProperties.Add(descriptor); }
			//		}
			//		_properties[type.FullName] = properties;
			//		if (_primaryKeys.ContainsKey(type.FullName)) { _primaryKeys.Remove(type.FullName); }
			//		_primaryKeys[type.FullName] = pkProperties;
			//	}
			//	return true;
			//}
		}

		/// <summary>返回当前实体模型属性集合。</summary>
		/// <param name="type"> AbstractEntity 类型的实例</param>
		/// <returns>返回当前实体模型属性集合。</returns>
		public static EntityPropertyCollection GetProperties(Type type)
		{
			if (_properties.TryGetValue(type.FullName, out EntityPropertyCollection infos))
			{
				return infos;
			}
			PropertyInfo[] pis = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			EntityPropertyCollection properties = new EntityPropertyCollection();
			if (pis == null || pis.Length == 0) { return properties; }
			EntityPropertyCollection pkProperties = new EntityPropertyCollection();
			foreach (PropertyInfo propertyInfo in pis)
			{
				EntityPropertyMeta descriptor = new EntityPropertyMeta(propertyInfo);
				properties.Add(descriptor);
				if (descriptor.PrimaryKey) { pkProperties.Add(descriptor); }
			}
			_primaryKeys.TryAdd(type.FullName, pkProperties);
			_properties.TryAdd(type.FullName, properties);
			//_properties[type.FullName] = properties;
			//if (_primaryKeys.ContainsKey(type.FullName)) {
			//	_primaryKeys.TryUpdate(type.FullName, pkProperties);
			//	_primaryKeys.TryRemove(type.FullName); }
			//_primaryKeys[type.FullName] = pkProperties;
			return properties;
		}

		/// <summary>返回当前实体模型属性集合。</summary>
		public static IReadOnlyCollection<EntityPropertyMeta> GetProperties<T>() where T : AbstractEntity
		{
			return GetProperties(typeof(T));
		}

		/// <summary>
		/// 尝试从集合中获取实体属性信息，如果获取成功额为true，否则为false。
		/// </summary>
		/// <param name="type">实体模型类型信息</param>
		/// <param name="propertyName">属性名称</param>
		/// <param name="propertyInfo">需要返回的EntityPropertyDescriptor 类实例，属性定义信息。</param>
		/// <returns>如果包含具有指定键的元素，则为 true；否则为 false。</returns>
		public static bool TryGetProperty(Type type, string propertyName, out EntityPropertyMeta propertyInfo)
		{
			if (TryGetProperties(type, out EntityPropertyCollection properties))
			{
				return properties.TryGetProperty(propertyName, out propertyInfo);
			}
			propertyInfo = null;
			return false;
		}

		/// <summary>
		/// 尝试从集合中获取实体属性信息，如果获取成功额为true，否则为false。
		/// </summary>
		/// <typeparam name="TE">表示实体模型类型</typeparam>
		/// <param name="properties">返回当前实体模型属性集合</param>
		/// <returns>如果包含具有指定键的元素，则为 true；否则为 false。</returns>
		public static bool TryGetProperties<TE>(out EntityPropertyCollection properties)
		{
			return TryGetProperties(typeof(TE), out properties);
		}

		/// <summary>
		/// 尝试从集合中获取实体属性信息，如果获取成功额为true，否则为false。
		/// </summary>
		/// <typeparam name="TE">表示实体模型类型</typeparam>
		/// <param name="propertyName">属性名称</param>
		/// <param name="propertyInfo">需要返回的EntityPropertyDescriptor 类实例，属性定义信息。</param>
		/// <returns>如果包含具有指定键的元素，则为 true；否则为 false。</returns>
		public static bool TryGetProperty<TE>(string propertyName, out EntityPropertyMeta propertyInfo)
		{
			return TryGetProperty(typeof(TE), propertyName, out propertyInfo);
		}
	}
}
