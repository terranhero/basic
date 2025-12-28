using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
#if NET8_0_OR_GREATER
using Microsoft.Extensions.Caching.Memory;
#elif NETSTANDARD || NET5_0 || NET6_0 || NET7_0
using System.Runtime.Caching;
#endif

namespace Basic.Caches
{
	/// <summary>
	/// 进程内缓存工厂类
	/// </summary>
	public sealed partial class MemoryClientFactory : CacheClientFactory
	{
		private static ConcurrentDictionary<string, ICacheClient> caches = new ConcurrentDictionary<string, ICacheClient>(-1, 5);
		/// <summary>初始化 MemoryClientFactory 类实例</summary>
		public MemoryClientFactory() { }

		/// <summary>
		/// 返回实现 DbConnection 类的提供程序的类的一个新实例。
		/// </summary>
		/// <param name="name">数据库连接名称</param>
		/// <returns>返回缓存 ICacheClient 接口的实例。</returns>
		public override ICacheClient CreateClient(string name)
		{
			if (caches.TryGetValue(name, out ICacheClient client) == true) { return client; }

			client = new MemoryCacheClient(name);
			caches.TryAdd(name, client);
			return client;
		}

		/// <summary>定义实现内存中缓存的类型。</summary>
		private sealed class MemoryCacheClient : ICacheClient
		{
#if NET8_0_OR_GREATER
			private readonly MemoryCacheOptions options = new MemoryCacheOptions() { };
			private readonly MemoryCache memory;
#else
			private readonly MemoryCache memory;

#endif
			/// <summary>初始化 MemoryCacheClient 类实例。</summary>
			/// <param name="name"></param>
			public MemoryCacheClient(string name)
			{
#if NET8_0_OR_GREATER
				memory = new MemoryCache(options);
#else
				memory = new MemoryCache(name);
#endif
			}

			/// <summary>Disposes the cache and clears all entries</summary>
			public void Dispose() { }

			#region 缓存异步方法 - 获取缓存键信息

			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public IEnumerable<KeyInfo> GetKeyInfos()
			{
#if NET8_0_OR_GREATER
				return memory.Keys.Select(m => new KeyInfo((string)m) { });
#else
				return memory.Select(m => new KeyInfo(m.Key) { });
#endif
			}

			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public IEnumerable<KeyInfo> GetKeyInfosAsync()
			{
#if NET8_0_OR_GREATER
				return memory.Keys.Select(m => new KeyInfo((string)m) { });
#else
				return memory.Select(m => new KeyInfo(m.Key) { });
#endif
			}
			#endregion

			#region 缓存同步方法 - 获取所有缓存的键
			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public IEnumerable<string> GetKeys()
			{
#if NET8_0_OR_GREATER
				return memory.Keys.Cast<string>();
#else
				return memory.Select(m => m.Key);
#endif
			}
			#endregion

			#region 缓存同步方法 - 移除缓存键及其数据
			/// <summary>从缓存中移除指定键的缓存项</summary>
			/// <param name="keys">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public void KeyDelete(string[] keys)
			{
				if (keys == null) { return; }
				foreach (string key in keys) { memory.Remove(key); }
			}

			/// <summary>根据传入的键移除一条记录</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public bool KeyDelete(string key)
			{
				memory.Remove(key);
				return true;
			}

			#endregion

			#region 缓存异步方法 - 移除缓存键及其数据
			/// <summary>根据传入的键移除一条记录</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public Task<bool> KeyDeleteAsync(string key)
			{
				if (key == null) { return Task.FromResult(false); }
				memory.Remove(key); return Task.FromResult(true);
			}

			/// <summary>从缓存中移除指定键的缓存项</summary>
			/// <param name="keys">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public Task KeyDeleteAsync(string[] keys)
			{
				if (keys == null) { return Task.CompletedTask; }
				foreach (string key in keys) { if (key != null) { memory.Remove(key); } }
				return Task.CompletedTask;
			}
			#endregion

			#region 缓存异步方法 - 判断缓存键是否存在
			/// <summary>使用异步方法判断键是否存在</summary>
			/// <param name="key">需要检查的缓存键.</param>
			/// <returns><see langword="true"/>如果键存在. <see langword="false"/> 如果键不存在.</returns>
			public Task<bool> KeyExistsAsync(string key)
			{
#if NET8_0_OR_GREATER
				return Task.FromResult(memory.TryGetValue(key, out _));
#else
				return Task.FromResult(memory.Contains(key));
#endif
			}
			#endregion

			#region 缓存同步方法 - 判断缓存键是否存在
			/// <summary>判断键是否存在</summary>
			/// <param name="key">需要检查的缓存键.</param>
			/// <returns><see langword="true"/> 如果键存在. <see langword="false"/> 如果键不存在.</returns>
			public bool KeyExists(string key)
			{
#if NET8_0_OR_GREATER
				return memory.TryGetValue(key, out _);
#else
				return memory.Contains(key);
#endif
			}
			#endregion

			#region 缓存异步方法 - 设置键过期策略
			/// <summary>设置键滑动过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public Task<bool> KeyExpireAsync(string key, TimeSpan expiry) { return Task.FromResult(false); }

			/// <summary>设置键绝对过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public Task<bool> KeyExpireAsync(string key, DateTime expiry) { return Task.FromResult(false); }
			#endregion

			#region 缓存同步方法 - 设置键过期策略
			/// <summary>设置键绝对过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public bool KeyExpire(string key, DateTime expiry) { return false; }

			/// <summary>设置键滑动过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public bool KeyExpire(string key, TimeSpan expiry) { return false; }
			#endregion

			#region 缓存同步方法 - 获取或设置缓存数据
			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public T Get<T>(string key)
			{
#if NET8_0_OR_GREATER
				return memory.Get<T>(key);
#else
				return (T)memory.Get(key);
#endif

			}
			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
			/// <returns>与指定的键对应的一组缓存项。</returns>
			public IDictionary<string, T> Get<T>(params string[] keys)
			{
				if (keys == null || keys.Length == 0) { return null; }
#if NET8_0_OR_GREATER
				IDictionary<string, T> values = new Dictionary<string, T>();
				foreach (string key in keys)
				{
					if (memory.TryGetValue<T>(key, out T value)) { values[key] = value; }
				}
				return values;
#else
				return (IDictionary<string, T>)memory.GetValues(keys);
#endif
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="value">该缓存项的数据。</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool Set<T>(string key, T value, System.TimeSpan expiresIn)
			{
				memory.Set(key, value, DateTimeOffset.Now.Add(expiresIn));
				return true;
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="value">该缓存项的数据。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool Set<T>(string key, T value, System.DateTime expiresAt)
			{
				memory.Set(key, value, new DateTimeOffset(expiresAt));
				return true;
			}
			#endregion

			#region 缓存异步方法 - 获取或设置缓存数据
			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="value">该缓存项的数据。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> SetAsync<T>(string key, T value, DateTime expiresAt)
			{
				memory.Set(key, value, new DateTimeOffset(expiresAt));
				return Task.FromResult(true);
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public Task<T> GetAsync<T>(string key)
			{
#if NET8_0_OR_GREATER
				return Task.FromResult(memory.Get<T>(key));
#else
				return Task.FromResult((T)memory.Get(key));
#endif
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
			/// <returns>与指定的键对应的一组缓存项。</returns>
			public Task<IDictionary<string, T>> GetAsync<T>(string[] keys)
			{
				if (keys == null || keys.Length == 0) { return Task.FromResult<IDictionary<string, T>>(null); }
#if NET8_0_OR_GREATER
				IDictionary<string, T> values = new Dictionary<string, T>();
				foreach (string key in keys)
				{
					if (memory.TryGetValue<T>(key, out T value)) { values[key] = value; }
				}
				return Task.FromResult(values);
#else
				return Task.FromResult((IDictionary<string, T>)memory.GetValues(keys));
#endif

			}
			#endregion

			#region 缓存异步方法 - 列表操作，设置，插入
			/// <summary>通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="item">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> ListPushAsync<T>(string key, T item)
			{
				IList<T> list = memory.Get<IList<T>>(key);
				if (list == null) { list = new List<T>(); }
				lock (list) { list.Add(item); }
				memory.Set<IList<T>>(key, list);
				return Task.FromResult(true);
			}

			/// <summary>通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> ListAsync<T>(string key, IList<T> values)
			{
				memory.Set(key, values);
				return Task.FromResult(true);
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> ListAsync<T>(string key, IList<T> values, DateTime expiresAt)
			{
				memory.Set(key, values, expiresAt);
				return Task.FromResult(true);
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> ListAsync<T>(string key, IList<T> values, TimeSpan expiresIn)
			{
				memory.Set(key, values, expiresIn);
				return Task.FromResult(true);
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			Task<long> ICacheClient.ListLengthAsync<T>(string key)
			{
				IList<T> list = memory.Get<IList<T>>(key);
				return Task.FromResult<long>(list.Count);
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public Task<IList<T>> ListAsync<T>(string key)
			{
				return Task.FromResult<IList<T>>(memory.Get<IList<T>>(key));
			}
			#endregion

			#region 缓存同步方法 - 列表操作，设置，插入
			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public IList<T> List<T>(string key)
			{
				return memory.Get<IList<T>>(key);
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			long ICacheClient.ListLength<T>(string key)
			{
				IList<T> list = memory.Get<IList<T>>(key);
				return list.Count;
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool List<T>(string key, IList<T> values)
			{
				memory.Set(key, values, DateTimeOffset.MaxValue);
				return true;
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool List<T>(string key, IList<T> values, System.TimeSpan expiresIn)
			{
				memory.Set(key, values, DateTimeOffset.Now.Add(expiresIn));
				return true;
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool List<T>(string key, IList<T> values, System.DateTime expiresAt)
			{
				memory.Set(key, values, new DateTimeOffset(expiresAt));
				return true;
			}
			#endregion

			#region 缓存异步方法 - 哈希表操作，设置，插入
			/// <summary>移除哈希表中的某值</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public Task<bool> HashDeleteAsync(string hashId, string key)
			{
				if (memory.TryGetValue<IDictionary>(hashId, out IDictionary hash))
				{
					hash.Remove(key); return Task.FromResult(true);
				}
				return Task.FromResult(false);
			}

			/// <summary>确定哈希表中是否存在某个缓存项。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public Task<bool> HashExistsAsync(string hashId, string key)
			{
				if (memory.TryGetValue(hashId, out IDictionary hash))
				{
					return Task.FromResult(hash.Contains(key));
				}
				return Task.FromResult(false);
			}

			/// <summary>从哈希表获取数据。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public Task<T> HashGetAsync<T>(string hashId, string key)
			{
				if (memory.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
				{
					if (hash.TryGetValue(key, out T value)) { return Task.FromResult(value); }
				}
				return Task.FromResult(default(T));
			}

			/// <summary>返回存储在key处的哈希中包含的字段数</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>哈希中的字段数，当键不存在时为 0</returns>
			Task<long> ICacheClient.HashLengthAsync<T>(string hashId)
			{
				if (memory.TryGetValue(hashId, out IDictionary<string, T> hash))
				{
					return Task.FromResult<long>(hash.Count);
				}
				return Task.FromResult<long>(0);
			}


			/// <summary>获取整个哈希表的数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public Task<IList<T>> HashGetAllAsync<T>(string hashId)
			{
				if (memory.TryGetValue(hashId, out IDictionary<string, T> hash))
				{
					return Task.FromResult<IList<T>>(hash.Values.ToList());
				}
				return Task.FromResult<IList<T>>(null);
			}

			/// <summary>从哈希表获取数据。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public Task<bool> HashSetAsync<T>(string hashId, string key, T value)
			{
				if (memory.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
				{
#if NET8_0_OR_GREATER
					if (hash.ContainsKey(key)) { hash[key] = value; return Task.FromResult(true); }
					else { return Task.FromResult(hash.TryAdd(key, value)); }
#else
					if (hash.ContainsKey(key)) { hash[key] = value; }
					else { hash.Add(key, value); }
					return Task.FromResult(true);
#endif
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash);
#if NET8_0_OR_GREATER
					return Task.FromResult(hash.TryAdd(key, value));
#else
					hash.Add(key, value);
					return Task.FromResult(true);
#endif
				}
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> HashSetAsync<T>(string hashId, string key, T value, DateTime expiresAt)
			{
				if (memory.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
				{
#if NET8_0_OR_GREATER
					if (hash.ContainsKey(key)) { hash[key] = value; return Task.FromResult(true); }
					else { return Task.FromResult(hash.TryAdd(key, value)); }
#else
					if (hash.ContainsKey(key)) { hash[key] = value; }
					else { hash.Add(key, value); }
					return Task.FromResult(true);
#endif
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash, new DateTimeOffset(expiresAt));
#if NET8_0_OR_GREATER
					return Task.FromResult(hash.TryAdd(key, value));
#else
					hash.Add(key, value);
					return Task.FromResult(true);
#endif
				}
			}
			#endregion

			#region 缓存同步方法 - 哈希表操作，设置，插入
			/// <summary>移除哈希表中的某值</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public bool HashDelete(string hashId, string key)
			{
				if (memory.TryGetValue<IDictionary>(hashId, out IDictionary hash))
				{
					hash.Remove(key); return true;
				}
				return false;
			}

			/// <summary>从哈希表获取数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public T HashGet<T>(string hashId, string key)
			{
				if (memory.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
				{
					if (hash.TryGetValue(key, out T value)) { return value; }
				}
				return default(T);
			}

			/// <summary>返回存储在key处的哈希中包含的字段数</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>哈希中的字段数，当键不存在时为 0</returns>
			long ICacheClient.HashLength<T>(string hashId)
			{
				if (memory.TryGetValue(hashId, out IDictionary<string, T> hash))
				{
					return (hash.Count);
				}
				return 0L;
			}

			/// <summary>获取整个哈希表的数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public List<T> HashGetAll<T>(string hashId)
			{
				if (memory.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
				{
					return hash.Values.ToList();
				}
				return null;
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool HashSet<T>(string hashId, string key, T value)
			{
				if (memory.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
				{
#if NET8_0_OR_GREATER
					if (hash.ContainsKey(key)) { hash[key] = value; return true; }
					else { return hash.TryAdd(key, value); }
#else
					if (hash.ContainsKey(key)) { hash[key] = value; }
					else { hash.Add(key, value); }
					return true;
#endif
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash);
#if NET8_0_OR_GREATER
					return hash.TryAdd(key, value);
#else
					hash.Add(key, value);
					return true;
#endif
				}
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool HashSet<T>(string hashId, string key, T value, DateTime expiresAt)
			{
				if (memory.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
				{
#if NET8_0_OR_GREATER
					if (hash.ContainsKey(key)) { hash[key] = value; return true; }
					else { return hash.TryAdd(key, value); }
#else
					if (hash.ContainsKey(key)) { hash[key] = value; }
					else { hash.Add(key, value); }
					return true;
#endif
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash, expiresAt);
#if NET8_0_OR_GREATER
					return hash.TryAdd(key, value);
#else
					hash.Add(key, value);
					return true;
#endif
				}
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool HashSet<T>(string hashId, string key, T value, TimeSpan expiresIn)
			{
				if (memory.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
				{
#if NET8_0_OR_GREATER
					if (hash.ContainsKey(key)) { hash[key] = value; return true; }
					else { return hash.TryAdd(key, value); }
#else
					if (hash.ContainsKey(key)) { hash[key] = value; }
					else { hash.Add(key, value); }
					return true;
#endif
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash, expiresIn);
#if NET8_0_OR_GREATER
					return hash.TryAdd(key, value);
#else
					hash.Add(key, value);
					return true;
#endif
				}
			}

			/// <summary>确定哈希表中是否存在某个缓存项。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public bool HashExists(string hashId, string key)
			{
				if (memory.TryGetValue<IDictionary>(hashId, out IDictionary hash))
				{
					return hash.Contains(key);
				}
				return false;
			}

			#endregion

			#region 缓存同步方法 - 集合和有序集合操作
			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			bool ICacheClient.SetAdd<T>(string key, T value)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list);
				return true;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			bool ICacheClient.SetAdd<T>(string key, T value, DateTime expiresAt)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list, expiresAt);
				return true;
			}

			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="items">需要添加到集合的项目列表</param>
			/// <returns>返回添加成功的集合项数量。</returns>
			long ICacheClient.SetAdd<T>(string key, IEnumerable<T> items)
			{
				if (items == null || items.Any() == false) { return (0L); }
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { foreach (T item in items) { list.Add(item); } }
				memory.Set<ISet<T>>(key, list);
				return (items.LongCount());
			}

			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="items">需要添加到集合的项目列表</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>返回添加成功的集合项数量。</returns>
			long ICacheClient.SetAdd<T>(string key, IEnumerable<T> items, DateTime expiresAt)
			{
				if (items == null || items.Any() == false) { return (0L); }
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { foreach (T item in items) { list.Add(item); } }
				memory.Set<ISet<T>>(key, list, expiresAt);
				return (items.LongCount());
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			long ICacheClient.SetLength<T>(string key)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				return list.Count;
			}

			/// <summary>获取集合中所有成员</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			ICollection<T> ICacheClient.SetMembers<T>(string key)
			{
				if (memory.TryGetValue(key, out ISet<T> value))
				{
					return value;
				}
				return null;
			}

			/// <summary>存储数据到有序集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			bool ICacheClient.ZSetAdd<T>(string key, T value, double score)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list);
				return true;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			bool ICacheClient.ZSetAdd<T>(string key, T value, double score, DateTime expiresAt)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list, expiresAt);
				return true;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			bool ICacheClient.ZSetAdd<T>(string key, T value, double score, TimeSpan expiresIn)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list, expiresIn);
				return true;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			long ICacheClient.ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc)
			{
				if (values == null || values.Any() == false) { return (0L); }

				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				memory.Set<ISet<T>>(key, list);
				return values.LongCount();
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <param name="expiresAt">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			long ICacheClient.ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, DateTime expiresAt)
			{
				if (values == null || values.Any() == false) { return (0L); }
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				memory.Set<ISet<T>>(key, list, expiresAt);
				return values.LongCount();
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			long ICacheClient.ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, TimeSpan expiresIn)
			{
				if (values == null || values.Any() == false) { return (0L); }
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				memory.Set<ISet<T>>(key, list, expiresIn);
				return values.LongCount();
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			long ICacheClient.ZSetLength<T>(string key)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				return list.Count;
			}

			/// <summary>从有序集合中读取所有数据。</summary>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			ICollection<T> ICacheClient.ZSetMembers<T>(string key)
			{
				if (memory.TryGetValue(key, out ISet<T> value))
				{
					return (value);
				}
				return null;
			}
			#endregion

			#region 缓存异步方法 - 集合和有序集合操作
			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			Task<bool> ICacheClient.SetAddAsync<T>(string key, T value)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			Task<bool> ICacheClient.SetAddAsync<T>(string key, T value, DateTime expiresAt)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list, expiresAt);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="items">需要添加到集合的项目列表</param>
			/// <returns>返回添加成功的集合项数量。</returns>
			Task<long> ICacheClient.SetAddAsync<T>(string key, IEnumerable<T> items)
			{
				if (items == null || items.Any() == false) { return Task.FromResult(0L); }
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { foreach (T item in items) { list.Add(item); } }
				memory.Set<ISet<T>>(key, list);
				return Task.FromResult(items.LongCount());
			}

			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="items">需要添加到集合的项目列表</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>返回添加成功的集合项数量。</returns>
			Task<long> ICacheClient.SetAddAsync<T>(string key, IEnumerable<T> items, DateTime expiresAt)
			{
				if (items == null || items.Any() == false) { return Task.FromResult(0L); }
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { foreach (T item in items) { list.Add(item); } }
				memory.Set<ISet<T>>(key, list, expiresAt);
				return Task.FromResult(items.LongCount());
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			Task<long> ICacheClient.SetLengthAsync<T>(string key)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				return Task.FromResult<long>(list.Count);
			}

			/// <summary>获取集合中所有成员</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			Task<ICollection<T>> ICacheClient.SetMembersAsync<T>(string key)
			{
				if (memory.TryGetValue(key, out ISet<T> value))
				{
					return Task.FromResult<ICollection<T>>(value);
				}
				return Task.FromResult<ICollection<T>>(null);
			}

			/// <summary>存储数据到有序集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score, DateTime expiresAt)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list, expiresAt);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score, TimeSpan expiresIn)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				memory.Set<ISet<T>>(key, list, expiresIn);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc)
			{
				if (values == null || values.Any() == false) { return Task.FromResult(0L); }

				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				memory.Set<ISet<T>>(key, list);
				return Task.FromResult(values.LongCount());
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <param name="expiresAt">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, DateTime expiresAt)
			{
				if (values == null || values.Any() == false) { return Task.FromResult(0L); }

				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				memory.Set<ISet<T>>(key, list, expiresAt);
				return Task.FromResult(values.LongCount());
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, TimeSpan expiresIn)
			{
				if (values == null || values.Any() == false) { return Task.FromResult(0L); }

				ISet<T> list = memory.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				memory.Set<ISet<T>>(key, list, expiresIn);
				return Task.FromResult(values.LongCount());
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			Task<long> ICacheClient.ZSetLengthAsync<T>(string key)
			{
				ISet<T> list = memory.Get<ISet<T>>(key);
				return Task.FromResult<long>(list.Count);
			}

			/// <summary>从有序集合中读取所有数据。</summary>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			Task<ICollection<T>> ICacheClient.ZSetMembersAsync<T>(string key)
			{
				if (memory.TryGetValue(key, out ISet<T> value))
				{
					return Task.FromResult<ICollection<T>>(value);
				}
				return Task.FromResult<ICollection<T>>(null);
			}
			#endregion
		}
	}

#if NETSTANDARD || NET6_0
	internal static class MemoryCacheExtension
	{
		/// <summary>
		/// Tries to get the value associated with the given key.
		/// </summary>
		/// <typeparam name="TItem">The type of the object to get.</typeparam>
		/// <param name="cache">The <see cref="MemoryCache"/> instance this method extends.</param>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">The value associated with the given key.</param>
		/// <returns><c>true</c> if the key was found; <c>false</c> otherwise.</returns>
		public static bool TryGetValue<TItem>(this MemoryCache cache, string key, out TItem value)
		{
			value = (TItem)cache.Get(key);
			return value != null;
		}

		/// <summary>
		/// Gets the value associated with this key if present.
		/// </summary>
		/// <typeparam name="TItem">The type of the object to get.</typeparam>
		/// <param name="cache">The <see cref="MemoryCache"/> instance this method extends.</param>
		/// <param name="key">The key of the value to get.</param>
		/// <returns>The value associated with this key, or <c>default(TItem)</c> if the key is not present.</returns>
		public static TItem Get<TItem>(this MemoryCache cache, string key)
		{
			return (TItem)(cache.Get(key) ?? default(TItem));
		}

		/// <summary>
		/// Associate a value with a key in the <see cref="MemoryCache"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of the object to set.</typeparam>
		/// <param name="cache">The <see cref="MemoryCache"/> instance this method extends.</param>
		/// <param name="key">The key of the entry to set.</param>
		/// <param name="value">The value to associate with the key.</param>
		/// <returns>The value that was set.</returns>
		public static void Set<TItem>(this MemoryCache cache, string key, TItem value)
		{
			cache.Set(key, value, DateTimeOffset.MaxValue);
		}

		/// <summary>
		/// Associate a value with a key in the <see cref="MemoryCache"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of the object to set.</typeparam>
		/// <param name="cache">The <see cref="MemoryCache"/> instance this method extends.</param>
		/// <param name="key">The key of the entry to set.</param>
		/// <param name="value">The value to associate with the key.</param>
		/// <param name="absoluteExpiration">The value to associate with the key.</param>
		/// <returns>The value that was set.</returns>
		public static void Set<TItem>(this MemoryCache cache, string key, TItem value, DateTimeOffset absoluteExpiration)
		{
			cache.Set(key, value, absoluteExpiration);
		}

		/// <summary>
		/// Associate a value with a key in the <see cref="MemoryCache"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of the object to set.</typeparam>
		/// <param name="cache">The <see cref="MemoryCache"/> instance this method extends.</param>
		/// <param name="key">The key of the entry to set.</param>
		/// <param name="value">The value to associate with the key.</param>
		/// <param name="expiresAt">The value to associate with the key.</param>
		/// <returns>The value that was set.</returns>
		public static void Set<TItem>(this MemoryCache cache, string key, TItem value, DateTime expiresAt)
		{
			cache.Set(key, value, new DateTimeOffset(expiresAt));
		}

		/// <summary>
		/// Associate a value with a key in the <see cref="MemoryCache"/>.
		/// </summary>
		/// <typeparam name="TItem">The type of the object to set.</typeparam>
		/// <param name="cache">The <see cref="MemoryCache"/> instance this method extends.</param>
		/// <param name="key">The key of the entry to set.</param>
		/// <param name="value">The value to associate with the key.</param>
		/// <param name="expiresIn">The value to associate with the key.</param>
		/// <returns>The value that was set.</returns>
		public static void Set<TItem>(this MemoryCache cache, string key, TItem value, TimeSpan expiresIn)
		{
			cache.Set(key, value, DateTimeOffset.Now.Add(expiresIn));
		}
	}
#endif
}