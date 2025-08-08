using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
using Microsoft.Extensions.Caching.Memory;
#endif

namespace Basic.Caches
{
	/// <summary>
	/// 进程内缓存工厂类
	/// </summary>
	public sealed partial class MemoryClientFactory : CacheClientFactory
	{
#if NET8_0_OR_GREATER
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
			private readonly MemoryCacheOptions options = new MemoryCacheOptions() { };
			private readonly MemoryCache memory;

			/// <summary>初始化 MemoryCacheClient 类实例。</summary>
			/// <param name="name"></param>
			public MemoryCacheClient(string name)
			{
				memory = new MemoryCache(options);
			}

			#region 缓存异步方法 - 获取缓存键信息

			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public IEnumerable<KeyInfo> GetKeyInfosAsync()
			{
				return memory.Keys.Select(m => new KeyInfo((string)m) { });
			}
			#endregion

			#region 缓存同步方法 - 获取所有缓存的键
			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public List<string> GetKeys() { return memory.Keys.Cast<string>().ToList(); }
			#endregion

			#region 缓存同步方法 - 移除缓存键及其数据
			/// <summary>从缓存中移除指定键的缓存项</summary>
			/// <param name="keys">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public void Remove(string[] keys)
			{
				if (keys == null) { return; }
				foreach (string key in keys) { memory.Remove(key); }
			}

			/// <summary>根据传入的键移除一条记录</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public bool Remove(string key)
			{
				memory.Remove(key);
				return true;
			}

			#endregion

			#region 缓存异步方法 - 移除缓存键及其数据
			/// <summary>根据传入的键移除一条记录</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public Task<bool> RemoveAsync(string key)
			{
				if (key == null) { return Task.FromResult(false); }
				memory.Remove(key); return Task.FromResult(true);
			}

			/// <summary>从缓存中移除指定键的缓存项</summary>
			/// <param name="keys">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public Task RemoveAsync(string[] keys)
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
			public Task<bool> KeyExistsAsync(string key) { return Task.FromResult(memory.TryGetValue(key, out _)); }
			#endregion

			#region 缓存同步方法 - 判断键是否存在
			/// <summary>判断键是否存在</summary>
			/// <param name="key">需要检查的缓存键.</param>
			/// <returns><see langword="true"/> 如果键存在. <see langword="false"/> 如果键不存在.</returns>
			public bool KeyExists(string key) { return memory.TryGetValue(key, out _); }
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
			public T GetValue<T>(string key)
			{
				return memory.Get<T>(key);
			}
			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
			/// <returns>与指定的键对应的一组缓存项。</returns>
			public IDictionary<string, T> GetValues<T>(params string[] keys)
			{
				if (keys == null || keys.Length == 0) { return null; }
				IDictionary<string, T> values = new Dictionary<string, T>();
				foreach (string key in keys)
				{
					if (memory.TryGetValue<T>(key, out T value)) { values[key] = value; }
				}
				return values;
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
				return Task.FromResult(memory.Get<T>(key));
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
				IDictionary<string, T> values = new Dictionary<string, T>();
				foreach (string key in keys)
				{
					if (memory.TryGetValue<T>(key, out T value)) { values[key] = value; }
				}
				return Task.FromResult(values);
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
			public Task<bool> SetListAsync<T>(string key, IList<T> values)
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
			public Task<bool> SetListAsync<T>(string key, IList<T> values, DateTime expiresAt)
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
			public Task<bool> SetListAsync<T>(string key, IList<T> values, TimeSpan expiresIn)
			{
				memory.Set(key, values, expiresIn);
				return Task.FromResult(true);
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public Task<IList<T>> GetListAsync<T>(string key)
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
			public List<T> GetList<T>(string key)
			{
				return memory.Get<List<T>>(key);
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool SetList<T>(string key, List<T> values)
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
			public bool SetList<T>(string key, List<T> values, System.TimeSpan expiresIn)
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
			public bool SetList<T>(string key, List<T> values, System.DateTime expiresAt)
			{
				memory.Set(key, values, new DateTimeOffset(expiresAt));
				return true;
			}
			#endregion

			#region 缓存异步方法 - 哈希表操作，设置，插入

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
					if (hash.ContainsKey(key)) { return Task.FromResult(hash.TryAdd(key, value)); }
					else { return Task.FromResult(hash.TryAdd(key, value)); }
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash);
					return Task.FromResult(hash.TryAdd(key, value));
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
					if (hash.ContainsKey(key)) { return Task.FromResult(hash.TryAdd(key, value)); }
					else { return Task.FromResult(hash.TryAdd(key, value)); }
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash, new DateTimeOffset(expiresAt));
					return Task.FromResult(hash.TryAdd(key, value));
				}
			}
			#endregion

			#region 缓存同步方法 - 哈希表操作，设置，插入
			/// <summary>移除哈希表中的某值</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public bool HashRemove(string hashId, string key)
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
					if (hash.ContainsKey(key)) { return hash.TryAdd(key, value); }
					else { return hash.TryAdd(key, value); }
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash);
					return hash.TryAdd(key, value);
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
					if (hash.ContainsKey(key)) { return hash.TryAdd(key, value); }
					else { return hash.TryAdd(key, value); }
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash, expiresAt);
					return hash.TryAdd(key, value);
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
					if (hash.ContainsKey(key)) { return hash.TryAdd(key, value); }
					else { return hash.TryAdd(key, value); }
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, 5);
					memory.Set(hashId, hash, expiresIn);
					return hash.TryAdd(key, value);
				}
			}

			/// <summary>确定哈希表中是否存在某个缓存项。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public bool HashContains(string hashId, string key)
			{
				if (memory.TryGetValue<IDictionary>(hashId, out IDictionary hash))
				{
					return hash.Contains(key);
				}
				return false;
			}

			/// <summary>
			/// 
			/// </summary>
			public void Dispose() { }
			#endregion
		}
#endif
	}
}