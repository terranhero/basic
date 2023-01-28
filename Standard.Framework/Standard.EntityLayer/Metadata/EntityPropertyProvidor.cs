using Basic.EntityLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Basic.Collections
{
	/// <summary>
	/// 提供实体模型属性管理
	/// </summary>
	public static class EntityPropertyProvidor
	{
		private static SortedDictionary<string, EntityPropertyCollection> _PropertyCollection = new SortedDictionary<string, EntityPropertyCollection>();
		private static SortedDictionary<string, EntityPropertyCollection> _PrimaryKeyCollection = new SortedDictionary<string, EntityPropertyCollection>();

		/// <summary>
		/// 尝试从集合中获取实体属性信息，如果获取成功额为true，否则为false。
		/// </summary>
		/// <param name="type">继承于 AbstractEntity 的实体模型类型</param>
		/// <param name="properties">返回当前实体模型属性集合</param>
		/// <param name="pkProperties">返回当前实体模型关键字属性集合</param>
		/// <returns>如果包含具有指定键的元素，则为 true；否则为 false。</returns>
		public static bool TryGetProperties(Type type, out EntityPropertyCollection properties, out EntityPropertyCollection pkProperties)
		{
			lock (_PropertyCollection)
			{
				string fullName = type.FullName;
				if (_PropertyCollection.TryGetValue(fullName, out properties))
				{
					return _PrimaryKeyCollection.TryGetValue(fullName, out pkProperties);
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
					_PropertyCollection[fullName] = properties;
					if (_PrimaryKeyCollection.ContainsKey(fullName)) { _PrimaryKeyCollection.Remove(fullName); }
					_PrimaryKeyCollection[fullName] = pkProperties;
					return true;
				}
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
			lock (_PropertyCollection)
			{
				if (!_PropertyCollection.TryGetValue(type.FullName, out properties))
				{
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
					_PropertyCollection[type.FullName] = properties;
					if (_PrimaryKeyCollection.ContainsKey(type.FullName)) { _PrimaryKeyCollection.Remove(type.FullName); }
					_PrimaryKeyCollection[type.FullName] = pkProperties;
				}
				return true;
			}
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
			TryGetProperties(type, out EntityPropertyCollection properties);
			return properties.TryGetProperty(propertyName, out propertyInfo);
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
			return TryGetProperty(typeof(TE), propertyName, out  propertyInfo);
		}
	}
}
