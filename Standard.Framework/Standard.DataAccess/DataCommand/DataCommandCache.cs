using System.Collections.Concurrent;
using System.Collections.Generic;
using Basic.Collections;

namespace Basic.DataAccess
{
	/// <summary>
	/// 数据库表命令结构缓存
	/// </summary>
	internal class DataCommandCache : ConcurrentDictionary<string, TableConfiguration>
	{
		/// <summary>
		/// 初始化 DataCommandCache 类的新实例，该实例为空且具有指定的初始容量。
		/// </summary>
		private DataCommandCache() : base() { }

		/// <summary>
		/// 表示数据库命令缓存
		/// </summary>
		private readonly static DataCommandCache _CommandCaches = new DataCommandCache();

		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取的值的键。</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。</param>
		/// <returns>如果 Basic.DataCache.SqlStructTableCache 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public static bool GetValue(string key, out TableConfiguration value)
		{
			return _CommandCaches.TryGetValue(key, out value);
		}

		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取的值的键。</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。</param>
		/// <returns>如果 Basic.DataCache.SqlStructTableCache 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public static TableConfiguration SetValue(string key, TableConfiguration value)
		{
			lock (_CommandCaches)
			{
				_CommandCaches[key] = value;
				return value;
			}
		}
	}
}
