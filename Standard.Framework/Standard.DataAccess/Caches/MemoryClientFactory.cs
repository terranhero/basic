using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
#elif NETSTANDARD || NET5_0 || NET6_0 || NET7_0
using System.Runtime.Caching;
#endif

namespace Basic.Caches
{
	/// <summary>
	/// 进程内缓存工厂类，用于创建并复用实现 <see cref="ICacheClient"/> 接口的进程内内存缓存客户端实例。
	/// 相同名称的缓存客户端只会创建一次，后续对同名客户端的调用将返回同一个实例。
	/// </summary>
	public sealed partial class MemoryClientFactory : CacheClientFactory
	{
#if NET8_0_OR_GREATER
		/// <summary>进程内缓存的全局配置选项（仅 .NET 8.0 及以上版本可用）；必须在第一次调用 <see cref="CreateClient"/> 之前通过 <see cref="SetOptions"/> 完成配置。</summary>
		private static readonly MemoryCacheOptions options = new MemoryCacheOptions() { };
	
		/// <summary>配置进程内缓存的全局 <see cref="MemoryCacheOptions"/> 选项；必须在第一次调用 <see cref="CreateClient"/> 之前调用。</summary>
		/// <param name="action">用于配置 <see cref="MemoryCacheOptions"/> 选项的委托；为 null 时不执行任何操作。</param>
		public static void SetOptions(Action<MemoryCacheOptions> action)
		{
			if (action != null) { action(options); }
		}
#endif

		/// <summary>已创建的缓存客户端实例集合；键为缓存客户端名称，值为对应的 <see cref="ICacheClient"/> 接口实例。</summary>
		private static ConcurrentDictionary<string, ICacheClient> caches = new ConcurrentDictionary<string, ICacheClient>(-1, 5);
		/// <summary>初始化 MemoryClientFactory 类实例</summary>
		public MemoryClientFactory() { }

		/// <summary>
		/// 创建或获取指定名称的进程内缓存客户端实例；同名客户端在首次创建后将被复用。
		/// </summary>
		/// <param name="name">缓存客户端名称。</param>
		/// <returns>返回实现 ICacheClient 接口的进程内缓存客户端实例。</returns>
		public override ICacheClient CreateClient(string name)
		{
			if (caches.TryGetValue(name, out ICacheClient client) == true) { return client; }
#if NET8_0_OR_GREATER
			client = new MemoryCacheClient(options);
			caches.TryAdd(name, client);
			return client;
#else
			client = new MemoryCacheClient(name);
			caches.TryAdd(name, client);
			return client;
#endif
		}

		/// <summary>定义实现内存中缓存的类型；封装 <see cref="MemoryCache"/> 并实现 <see cref="ICacheClient"/> 接口，提供键值、列表、哈希表、集合及有序集合等缓存操作。</summary>
		private sealed class MemoryCacheClient : MemoryCache, ICacheClient
		{
#if NET8_0_OR_GREATER
			//private readonly MemoryCache memory;
			private readonly ConcurrentDictionary<string, KeyTypes> types = new ConcurrentDictionary<string, KeyTypes>();
			/// <summary>初始化 MemoryCacheClient 类实例。</summary>
			public MemoryCacheClient(IOptions<MemoryCacheOptions> optionsAccessor) : base(optionsAccessor)
			{
				//memory = new MemoryCache(options);
			}
#else
			//private readonly MemoryCache memory;
			private readonly ConcurrentDictionary<string, KeyTypes> types = new ConcurrentDictionary<string, KeyTypes>();
			/// <summary>初始化 MemoryCacheClient 类实例。</summary>
			public MemoryCacheClient(string name) : base(name)
			{
				//memory = new MemoryCache(name);
			}
#endif

			#region 缓存异步方法 - 获取缓存键信息

			/// <summary>获取所有缓存键的相关信息。</summary>
			/// <returns>返回所有缓存键的 <see cref="KeyInfo"/> 信息集合；如果没有缓存项则返回空集合。</returns>
			public IEnumerable<KeyInfo> GetKeyInfos()
			{
#if NET8_0_OR_GREATER
				return this.Keys.Select(m => new KeyInfo((string)m) { });
#else
				return this.Select(m => new KeyInfo(m.Key) { });
#endif
			}

			/// <summary>以异步方式获取所有缓存键的相关信息。</summary>
			/// <returns>返回所有缓存键的 <see cref="KeyInfo"/> 信息集合；如果没有缓存项则返回空集合。</returns>
			public Task<IEnumerable<KeyInfo>> GetKeyInfosAsync()
			{
#if NET8_0_OR_GREATER
				return Task.FromResult(this.Keys.Select(m => new KeyInfo((string)m) { }));
#else
				return Task.FromResult(this.Select(m => new KeyInfo(m.Key) { }));
#endif
			}
			#endregion

			#region 缓存同步方法 - 获取所有缓存的键
			/// <summary>获取所有缓存的键。</summary>
			/// <returns>返回所有缓存键的集合；如果没有缓存项则返回空集合。</returns>
			public IEnumerable<string> GetKeys()
			{
#if NET8_0_OR_GREATER
				return this.Keys.Cast<string>();
#else
				return this.Select(m => m.Key);
#endif
			}
			#endregion

			#region 缓存同步方法 - 移除缓存键及其数据
			/// <summary>从缓存中移除指定的一组键对应的缓存项。</summary>
			/// <param name="keys">需要移除记录的键数组；为 null 时直接返回。</param>
			public void KeyDelete(string[] keys)
			{
				if (keys == null) { return; }
				foreach (string key in keys) { this.Remove(key); }
			}

			/// <summary>根据传入的键移除一条缓存记录。</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>此实现总是返回 <see langword="true"/>。</returns>
			public bool KeyDelete(string key)
			{
				this.Remove(key);
				return true;
			}

			#endregion

			#region 缓存异步方法 - 移除缓存键及其数据
			/// <summary>以异步方式根据传入的键移除一条缓存记录。</summary>
			/// <param name="key">需要移除记录的键；为 null 时不执行移除。</param>
			/// <returns>移除成功则返回 <see langword="true"/>；键为 null 时返回 <see langword="false"/>。</returns>
			public Task<bool> KeyDeleteAsync(string key)
			{
				if (key == null) { return Task.FromResult(false); }
				this.Remove(key); return Task.FromResult(true);
			}

			/// <summary>以异步方式从缓存中移除指定的一组键对应的缓存项。</summary>
			/// <param name="keys">需要移除记录的键数组；为 null 时直接返回。</param>
			/// <returns>表示移除操作的任务。</returns>
			public Task KeyDeleteAsync(string[] keys)
			{
				if (keys == null) { return Task.CompletedTask; }
				foreach (string key in keys) { if (key != null) { this.Remove(key); } }
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
				return Task.FromResult(this.TryGetValue(key, out _));
#else
				return Task.FromResult(this.Contains(key));
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
				return this.TryGetValue(key, out _);
#else
				return this.Contains(key);
#endif
			}
			#endregion

			#region 缓存异步方法 - 设置键过期策略
			/// <summary>设置键滑动过期策略；进程内缓存不支持修改已存在缓存项的过期策略，此实现始终返回 <see langword="false"/>。</summary>
			/// <param name="key">需要设置滑动过期时间的缓存键.</param>
			/// <param name="expiry">滑动过期时间。</param>
			/// <returns>本实现不支持该操作，始终返回 <see langword="false"/>。</returns>
			public Task<bool> KeyExpireAsync(string key, TimeSpan expiry) { return Task.FromResult(false); }

			/// <summary>设置键绝对过期策略；进程内缓存不支持修改已存在缓存项的过期策略，此实现始终返回 <see langword="false"/>。</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间。</param>
			/// <returns>本实现不支持该操作，始终返回 <see langword="false"/>。</returns>
			public Task<bool> KeyExpireAsync(string key, DateTime expiry) { return Task.FromResult(false); }
			#endregion

			#region 缓存同步方法 - 设置键过期策略
			/// <summary>设置键绝对过期策略；进程内缓存不支持修改已存在缓存项的过期策略，此实现始终返回 <see langword="false"/>。</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间。</param>
			/// <returns>本实现不支持该操作，始终返回 <see langword="false"/>。</returns>
			public bool KeyExpire(string key, DateTime expiry) { return false; }

			/// <summary>设置键滑动过期策略；进程内缓存不支持修改已存在缓存项的过期策略，此实现始终返回 <see langword="false"/>。</summary>
			/// <param name="key">需要设置滑动过期时间的缓存键.</param>
			/// <param name="expiry">滑动过期时间。</param>
			/// <returns>本实现不支持该操作，始终返回 <see langword="false"/>。</returns>
			public bool KeyExpire(string key, TimeSpan expiry) { return false; }
			#endregion

			#region 缓存同步方法 - 获取或设置缓存数据
			/// <summary>
			///  获取与指定缓存键关联的缓存项。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为默认值。</returns>
			public T Get<T>(string key)
			{
#if NET8_0_OR_GREATER
				return this.Get<T>(key);
#else
				return (T)this.Get(key);
#endif

			}
			/// <summary>
			///  获取与一组指定缓存键关联的缓存项。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
			/// <returns>与指定的键对应的一组缓存项；keys 为 null 或空数组时返回 null。</returns>
			public IDictionary<string, T> Get<T>(params string[] keys)
			{
				if (keys == null || keys.Length == 0) { return null; }
#if NET8_0_OR_GREATER
				IDictionary<string, T> values = new Dictionary<string, T>();
				foreach (string key in keys)
				{
					if (this.TryGetValue<T>(key, out T value)) { values[key] = value; }
				}
				return values;
#else
				return (IDictionary<string, T>)this.GetValues(keys);
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
#if NET8_0_OR_GREATER
				this.Set(key, value, DateTimeOffset.Now.Add(expiresIn));
#else
				this.Set(key, value, DateTimeOffset.Now.Add(expiresIn));
#endif
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
				this.Set(key, value, new DateTimeOffset(expiresAt));
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
				this.Set(key, value, new DateTimeOffset(expiresAt));
				return Task.FromResult(true);
			}

			/// <summary>
			///  以异步方式获取与指定缓存键关联的缓存项。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为默认值。</returns>
			public Task<T> GetAsync<T>(string key)
			{
#if NET8_0_OR_GREATER
				return Task.FromResult(this.Get<T>(key));
#else
				return Task.FromResult((T)this.Get(key));
#endif
			}

			/// <summary>
			///  以异步方式获取与一组指定缓存键关联的缓存项。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
			/// <returns>与指定的键对应的一组缓存项；keys 为 null 或空数组时返回 null。</returns>
			public Task<IDictionary<string, T>> GetAsync<T>(string[] keys)
			{
				if (keys == null || keys.Length == 0) { return Task.FromResult<IDictionary<string, T>>(null); }
#if NET8_0_OR_GREATER
				IDictionary<string, T> values = new Dictionary<string, T>();
				foreach (string key in keys)
				{
					if (this.TryGetValue<T>(key, out T value)) { values[key] = value; }
				}
				return Task.FromResult(values);
#else
				return Task.FromResult((IDictionary<string, T>)this.GetValues(keys));
#endif

			}
			#endregion

			#region 缓存异步方法 - 列表操作，设置，插入
			/// <summary>将一个元素追加到列表缓存的末尾；如果列表不存在则先创建列表。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="item">要追加到列表末尾的元素。</param>
			/// <returns>追加成功则为true，否则为false。</returns>
			public Task<bool> ListPushAsync<T>(string key, T item)
			{
				IList<T> list = this.Get<IList<T>>(key);
				if (list == null) { list = new List<T>(); }
				lock (list) { list.Add(item); }
				this.Set<IList<T>>(key, list);
				return Task.FromResult(true);
			}

			/// <summary>通过使用键、列表值和逐出设置，将某个缓存项插入缓存中。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> ListAsync<T>(string key, IList<T> values)
			{
				this.Set(key, values);
				return Task.FromResult(true);
			}

			/// <summary>
			///  通过使用键、列表值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> ListAsync<T>(string key, IList<T> values, DateTime expiresAt)
			{
				this.Set(key, values, expiresAt);
				return Task.FromResult(true);
			}

			/// <summary>
			///  通过使用键、列表值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> ListAsync<T>(string key, IList<T> values, TimeSpan expiresIn)
			{
				this.Set(key, values, expiresIn);
				return Task.FromResult(true);
			}

			/// <summary>返回存储在指定键处的列表的元素数量。</summary>
			/// <param name="key">列表的键</param>
			/// <returns>列表的元素数量，如果键不存在，则为0。</returns>
			Task<long> ICacheClient.ListLengthAsync<T>(string key)
			{
				IList<T> list = this.Get<IList<T>>(key);
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
				return Task.FromResult<IList<T>>(this.Get<IList<T>>(key));
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
				return this.Get<IList<T>>(key);
			}

			/// <summary>返回存储在指定键处的列表的元素数量。</summary>
			/// <param name="key">列表的键</param>
			/// <returns>列表的元素数量，如果键不存在，则为0。</returns>
			long ICacheClient.ListLength<T>(string key)
			{
				IList<T> list = this.Get<IList<T>>(key);
				return list.Count;
			}

			/// <summary>
			///  通过使用键、列表值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool List<T>(string key, IList<T> values)
			{
				this.Set(key, values, DateTimeOffset.MaxValue);
				return true;
			}

			/// <summary>
			///  通过使用键、列表值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool List<T>(string key, IList<T> values, System.TimeSpan expiresIn)
			{
				this.Set(key, values, DateTimeOffset.Now.Add(expiresIn));
				return true;
			}

			/// <summary>
			///  通过使用键、列表值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool List<T>(string key, IList<T> values, System.DateTime expiresAt)
			{
				this.Set(key, values, new DateTimeOffset(expiresAt));
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
				if (this.TryGetValue<IDictionary>(hashId, out IDictionary hash))
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
				if (this.TryGetValue(hashId, out IDictionary hash))
				{
					return Task.FromResult(hash.Contains(key));
				}
				return Task.FromResult(false);
			}

			/// <summary>以异步方式从哈希表获取数据。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为默认值。</returns>
			public Task<T> HashGetAsync<T>(string hashId, string key)
			{
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
				if (this.TryGetValue(hashId, out IDictionary<string, T> hash))
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
				if (this.TryGetValue(hashId, out IDictionary<string, T> hash))
				{
					return Task.FromResult<IList<T>>(hash.Values.ToList());
				}
				return Task.FromResult<IList<T>>(null);
			}


			/// <summary>存储多个键值对到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="values">要存储的键值对字典，其中键为哈希表键，值为哈希表值</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> HashSetAsync<T>(string hashId, IDictionary<string, T> values)
			{
				if (this.TryGetValue(hashId, out IDictionary<string, T> hash))
				{
					foreach (KeyValuePair<string, T> kvp in values)
					{
#if NET8_0_OR_GREATER
						if (hash.ContainsKey(kvp.Key)) { hash[kvp.Key] = kvp.Value; }
						else { hash.TryAdd(kvp.Key, kvp.Value); }
#else
						if (hash.ContainsKey(kvp.Key)) { hash[kvp.Key] = kvp.Value; }
						else { hash.Add(kvp.Key, kvp.Value); }
#endif
					}
					return Task.FromResult(true);
				}
				else
				{
					hash = new ConcurrentDictionary<string, T>(-1, values, null) { };
					this.Set(hashId, hash);
					return Task.FromResult(true);
				}
			}

			/// <summary>以异步方式存储数据到哈希表；如果哈希表不存在则先创建哈希表。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>存储成功则返回true，否则返回false。</returns>
			public Task<bool> HashSetAsync<T>(string hashId, string key, T value)
			{
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
					this.Set(hashId, hash);
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
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
					this.Set(hashId, hash, new DateTimeOffset(expiresAt));
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
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> HashSetAsync<T>(string hashId, string key, T value, TimeSpan expiresIn)
			{
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
					this.Set(hashId, hash, DateTimeOffset.Now.Add(expiresIn));
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
				if (this.TryGetValue<IDictionary>(hashId, out IDictionary hash))
				{
					hash.Remove(key); return true;
				}
				return false;
			}

			/// <summary>从哈希表获取数据。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为默认值。</returns>
			public T HashGet<T>(string hashId, string key)
			{
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
				if (this.TryGetValue(hashId, out IDictionary<string, T> hash))
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
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
					this.Set(hashId, hash);
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
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
					this.Set(hashId, hash, expiresAt);
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
				if (this.TryGetValue<IDictionary<string, T>>(hashId, out IDictionary<string, T> hash))
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
					this.Set(hashId, hash, expiresIn);
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
				if (this.TryGetValue<IDictionary>(hashId, out IDictionary hash))
				{
					return hash.Contains(key);
				}
				return false;
			}

			#endregion

			#region 缓存同步方法 - 集合和有序集合操作
			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">要添加到集合的成员。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			bool ICacheClient.SetAdd<T>(string key, T value)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list);
				return true;
			}

			/// <summary>存储数据到集合；如果集合不存在则先创建集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">要添加到集合的成员。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			bool ICacheClient.SetAdd<T>(string key, T value, DateTime expiresAt)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list, expiresAt);
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
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { foreach (T item in items) { list.Add(item); } }
				this.Set<ISet<T>>(key, list);
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
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { foreach (T item in items) { list.Add(item); } }
				this.Set<ISet<T>>(key, list, expiresAt);
				return (items.LongCount());
			}

			/// <summary>返回存储在指定键处的集合的基数（元素数）。</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			long ICacheClient.SetLength<T>(string key)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				return list.Count;
			}

			/// <summary>获取集合中所有成员。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <returns>返回集合中的所有成员；如果键不存在则返回 null。</returns>
			ICollection<T> ICacheClient.SetMembers<T>(string key)
			{
				if (this.TryGetValue(key, out ISet<T> value))
				{
					return value;
				}
				return null;
			}

			/// <summary>存储数据到有序集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="value">要添加到有序集合的成员。</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			bool ICacheClient.ZSetAdd<T>(string key, T value, double score)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list);
				return true;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="value">要添加到有序集合的成员。</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			bool ICacheClient.ZSetAdd<T>(string key, T value, double score, DateTime expiresAt)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list, expiresAt);
				return true;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="value">要添加到有序集合的成员。</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			bool ICacheClient.ZSetAdd<T>(string key, T value, double score, TimeSpan expiresIn)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list, expiresIn);
				return true;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="values">要添加到有序集合的成员列表。</param>
			/// <param name="scoreFunc">用于计算成员分数的函数，分数用于排序。</param>
			/// <returns>返回成功添加到有序集合的成员数量。</returns>
			long ICacheClient.ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc)
			{
				if (values == null || values.Any() == false) { return (0L); }

				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				this.Set<ISet<T>>(key, list);
				return values.LongCount();
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="values">要添加到有序集合的成员列表。</param>
			/// <param name="scoreFunc">用于计算成员分数的函数，分数用于排序。</param>
			/// <param name="expiresAt">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>返回成功添加到有序集合的成员数量。</returns>
			long ICacheClient.ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, DateTime expiresAt)
			{
				if (values == null || values.Any() == false) { return (0L); }
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				this.Set<ISet<T>>(key, list, expiresAt);
				return values.LongCount();
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="values">要添加到有序集合的成员列表。</param>
			/// <param name="scoreFunc">用于计算成员分数的函数，分数用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>返回成功添加到有序集合的成员数量。</returns>
			long ICacheClient.ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, TimeSpan expiresIn)
			{
				if (values == null || values.Any() == false) { return (0L); }
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				this.Set<ISet<T>>(key, list, expiresIn);
				return values.LongCount();
			}

			/// <summary>返回存储在指定键处的有序集合的基数（元素数）。</summary>
			/// <param name="key">有序集合的键</param>
			/// <returns>有序集合的基数（元素数），如果键不存在，则为0。</returns>
			long ICacheClient.ZSetLength<T>(string key)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				return list.Count;
			}

			/// <summary>从有序集合中读取所有数据。</summary>
			/// <param name="key">有序集合键名</param>
			/// <returns>返回有序集合中的所有成员；如果键不存在则返回 null。</returns>
			ICollection<T> ICacheClient.ZSetMembers<T>(string key)
			{
				if (this.TryGetValue(key, out ISet<T> value))
				{
					return (value);
				}
				return null;
			}
			#endregion

			#region 缓存异步方法 - 集合和有序集合操作
			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">要添加到集合的成员。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			Task<bool> ICacheClient.SetAddAsync<T>(string key, T value)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到集合；如果集合不存在则先创建集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">要添加到集合的成员。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			Task<bool> ICacheClient.SetAddAsync<T>(string key, T value, DateTime expiresAt)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list, expiresAt);
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
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { foreach (T item in items) { list.Add(item); } }
				this.Set<ISet<T>>(key, list);
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
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new HashSet<T>(); }
				lock (list) { foreach (T item in items) { list.Add(item); } }
				this.Set<ISet<T>>(key, list, expiresAt);
				return Task.FromResult(items.LongCount());
			}

			/// <summary>返回存储在指定键处的集合的基数（元素数）。</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			Task<long> ICacheClient.SetLengthAsync<T>(string key)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				return Task.FromResult<long>(list.Count);
			}

			/// <summary>获取集合中所有成员。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <returns>返回集合中的所有成员；如果键不存在则返回 null。</returns>
			Task<ICollection<T>> ICacheClient.SetMembersAsync<T>(string key)
			{
				if (this.TryGetValue(key, out ISet<T> value))
				{
					return Task.FromResult<ICollection<T>>(value);
				}
				return Task.FromResult<ICollection<T>>(null);
			}

			/// <summary>存储数据到有序集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="value">要添加到有序集合的成员。</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="value">要添加到有序集合的成员。</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score, DateTime expiresAt)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list, expiresAt);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="value">要添加到有序集合的成员。</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>添加成功则为 true，否则为 false。</returns>
			Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score, TimeSpan expiresIn)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.Add(value); }
				this.Set<ISet<T>>(key, list, expiresIn);
				return Task.FromResult(true);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="values">要添加到有序集合的成员列表。</param>
			/// <param name="scoreFunc">用于计算成员分数的函数，分数用于排序。</param>
			/// <returns>返回成功添加到有序集合的成员数量。</returns>
			Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc)
			{
				if (values == null || values.Any() == false) { return Task.FromResult(0L); }

				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				this.Set<ISet<T>>(key, list);
				return Task.FromResult(values.LongCount());
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="values">要添加到有序集合的成员列表。</param>
			/// <param name="scoreFunc">用于计算成员分数的函数，分数用于排序。</param>
			/// <param name="expiresAt">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>返回成功添加到有序集合的成员数量。</returns>
			Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, DateTime expiresAt)
			{
				if (values == null || values.Any() == false) { return Task.FromResult(0L); }

				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				this.Set<ISet<T>>(key, list, expiresAt);
				return Task.FromResult(values.LongCount());
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">有序集合键名</param>
			/// <param name="values">要添加到有序集合的成员列表。</param>
			/// <param name="scoreFunc">用于计算成员分数的函数，分数用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>返回成功添加到有序集合的成员数量。</returns>
			Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, TimeSpan expiresIn)
			{
				if (values == null || values.Any() == false) { return Task.FromResult(0L); }

				ISet<T> list = this.Get<ISet<T>>(key);
				if (list == null) { list = new SortedSet<T>(); }
				lock (list) { list.UnionWith(values); }
				this.Set<ISet<T>>(key, list, expiresIn);
				return Task.FromResult(values.LongCount());
			}

			/// <summary>返回存储在指定键处的有序集合的基数（元素数）。</summary>
			/// <param name="key">有序集合的键</param>
			/// <returns>有序集合的基数（元素数），如果键不存在，则为0。</returns>
			Task<long> ICacheClient.ZSetLengthAsync<T>(string key)
			{
				ISet<T> list = this.Get<ISet<T>>(key);
				return Task.FromResult<long>(list.Count);
			}

			/// <summary>从有序集合中读取所有数据。</summary>
			/// <param name="key">有序集合键名</param>
			/// <returns>返回有序集合中的所有成员；如果键不存在则返回 null。</returns>
			Task<ICollection<T>> ICacheClient.ZSetMembersAsync<T>(string key)
			{
				if (this.TryGetValue(key, out ISet<T> value))
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