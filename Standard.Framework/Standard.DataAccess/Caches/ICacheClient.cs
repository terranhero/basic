using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示缓存接口
	/// </summary>
	public interface ICacheClient : IDisposable
	{
		/// <summary>获取所有缓存的键</summary>
		/// <returns>如果存在则返回键列表，否则返回 null。</returns>
		List<string> GetKeys();

		/// <summary>根据传入的键移除一条记录</summary>
		/// <param name="key">需要移除记录的键</param>
		/// <returns>移除成功则返回true，否则返回false。</returns>
		bool Remove(string key);

		/// <summary>从缓存中移除指定键的缓存项</summary>
		/// <param name="keys">需要移除记录的键</param>
		/// <returns>移除成功则返回true，否则返回false。</returns>
		void Remove(string[] keys);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="value">该缓存项的数据。</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool Set<T>(string key, T value, DateTime expiresAt);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="value">该缓存项的数据。</param>
		/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool Set<T>(string key, T value, TimeSpan expiresIn);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		T GetValue<T>(string key);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
		/// <returns>与指定的键对应的一组缓存项。</returns>
		IDictionary<string, T> GetValues<T>(params string[] keys);

		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool SetList<T>(string key, List<T> values);

		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool SetList<T>(string key, List<T> values, DateTime expiresAt);

		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool SetList<T>(string key, List<T> values, TimeSpan expiresIn);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		List<T> GetList<T>(string key);

		/// <summary>存储数据到哈希表</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <param name="value">哈希表值</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool HashSet<T>(string hashId, string key, T value);

		/// <summary>存储数据到哈希表</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <param name="value">哈希表值</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool HashSet<T>(string hashId, string key, T value, DateTime expiresAt);

		/// <summary>存储数据到哈希表</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <param name="value">哈希表值</param>
		/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool HashSet<T>(string hashId, string key, T value, TimeSpan expiresIn);

		/// <summary>移除哈希表中的某值</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>移除成功则返回true，否则返回false。</returns>
		bool HashRemove(string hashId, string key);

		/// <summary>确定哈希表中是否存在某个缓存项。</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		bool HashContains(string hashId, string key);

		/// <summary>从哈希表获取数据</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		T HashGet<T>(string hashId, string key);

		/// <summary>获取整个哈希表的数据</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		List<T> HashGetAll<T>(string hashId);
	}
}
