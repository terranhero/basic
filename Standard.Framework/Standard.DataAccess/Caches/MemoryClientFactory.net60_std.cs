using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if NETSTANDARD || NET5_0 || NET6_0 || NET7_0
using System.Runtime.Caching;
#endif

namespace Basic.Caches
{
	/// <summary>
	/// 进程内缓存工厂类
	/// </summary>
	public sealed partial class MemoryClientFactory : CacheClientFactory
	{
#if NETSTANDARD || NET5_0 || NET6_0 || NET7_0
		private static SortedList<string, ICacheClient> caches = new SortedList<string, ICacheClient>(5);
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
			caches.Add(name, client);
			return client;
		}

		/// <summary>定义实现内存中缓存的类型。</summary>
		private sealed class MemoryCacheClient : MemoryCache, ICacheClient
		{
			/// <summary>初始化 MemoryCacheClient 类实例。</summary>
			/// <param name="name"></param>
			public MemoryCacheClient(string name) : base(name) { }

			#region 缓存异步方法 - 获取缓存键信息

			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public IEnumerable<KeyInfo> GetKeyInfosAsync()
			{
				return this.Select(m => new KeyInfo(m.Key) { });
			}
			#endregion

			#region 缓存异步方法 - 移除缓存键及其数据
			/// <summary>根据传入的键移除一条记录</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public Task<bool> RemoveAsync(string key) { return Task.FromResult(true); }

			/// <summary>从缓存中移除指定键的缓存项</summary>
			/// <param name="keys">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public Task RemoveAsync(string[] keys) { return Task.FromResult(true); }
			#endregion

			#region 缓存异步方法 - 判断缓存键是否存在
			/// <summary>使用异步方法判断键是否存在</summary>
			/// <param name="key">需要检查的缓存键.</param>
			/// <returns><see langword="true"/>如果键存在. <see langword="false"/> 如果键不存在.</returns>
			public Task<bool> KeyExistsAsync(string key) { return Task.FromResult(true); }
			#endregion

			#region 缓存异步方法 - 设置缓存键过期
			/// <summary>设置键滑动过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public Task<bool> KeyExpireAsync(string key, TimeSpan expiry) { return Task.FromResult(true); }

			/// <summary>设置键绝对过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public Task<bool> KeyExpireAsync(string key, DateTime expiry) { return Task.FromResult(true); }
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
			public Task<bool> SetAsync<T>(string key, T value, DateTime expiresAt) { return Task.FromResult(true); }

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public Task<T> GetAsync<T>(string key)
			{
				return Task.FromResult((T)base.Get(key));
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
			/// <returns>与指定的键对应的一组缓存项。</returns>
			public Task<IDictionary<string, T>> GetAsync<T>(string[] keys)
			{
				return Task.FromResult((IDictionary<string, T>)base.GetValues(keys));
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
				IList<T> list = (IList<T>)base.Get(key);
				if (list == null) { list = new List<T>(); }
				list.Add(item); base.Set(key, list, DateTimeOffset.MaxValue);
				return Task.FromResult(true);
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> SetListAsync<T>(string key, IList<T> values) { return Task.FromResult(true); }

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> SetListAsync<T>(string key, IList<T> values, DateTime expiresAt) { return Task.FromResult(true); }

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public Task<bool> SetListAsync<T>(string key, IList<T> values, TimeSpan expiresIn) { return Task.FromResult(true); }

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public Task<IList<T>> GetListAsync<T>(string key)
			{
				return Task.FromResult<IList<T>>((IList<T>)base.Get(key));
			}
			#endregion

			/// <summary>判断键是否存在</summary>
			/// <param name="key">需要检查的缓存键.</param>
			/// <returns><see langword="true"/> 如果键存在. <see langword="false"/> 如果键不存在.</returns>
			public bool KeyExists(string key) { return base.Contains(key); }

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

			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public List<string> GetKeys() { return this.Select(m => m.Key).ToList(); }

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public T GetValue<T>(string key)
			{
				return (T)base.Get(key);
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
			/// <returns>与指定的键对应的一组缓存项。</returns>
			public IDictionary<string, T> GetValues<T>(params string[] keys)
			{
				return (IDictionary<string, T>)base.GetValues(keys);
			}

			/// <summary>从缓存中移除指定键的缓存项</summary>
			/// <param name="keys">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public void Remove(string[] keys)
			{
				if (keys == null) { return; }
				foreach (string key in keys) { base.Remove(key); }
			}
			/// <summary>根据传入的键移除一条记录</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public bool Remove(string key)
			{
				base.Remove(key);
				return true;
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
				base.Set(key, value, DateTimeOffset.Now.Add(expiresIn));
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
				base.Set(key, value, new DateTimeOffset(expiresAt));
				return true;
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public List<T> GetList<T>(string key)
			{
				return (List<T>)base.Get(key);
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
				base.Set(key, values, DateTimeOffset.MaxValue);
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
				base.Set(key, values, DateTimeOffset.Now.Add(expiresIn));
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
				base.Set(key, values, new DateTimeOffset(expiresAt));
				return true;
			}

			/// <summary>释放由 System.Runtime.Caching.MemoryCache 类的当前实例占用的所有资源。</summary>
			public new void Dispose() { }

			/// <summary>移除哈希表中的某值</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public bool HashRemove(string hashId, string key)
			{
				if (Contains(hashId))
				{
					IHashTableItem hash = Get(hashId) as IHashTableItem;
					return hash.Remove(key);
				}
				return false;
			}

			/// <summary>从哈希表获取数据。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public Task<T> HashGetAsync<T>(string hashId, string key)
			{
				if (Contains(hashId))
				{
					HashTableItem<T> hash = Get(hashId) as HashTableItem<T>;
					if (hash.Contains(key)) { return Task.FromResult(hash.Get(key)); }
					return Task.FromResult(default(T));
				}
				return Task.FromResult(default(T));
			}

			/// <summary>从哈希表获取数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public T HashGet<T>(string hashId, string key)
			{
				if (Contains(hashId))
				{
					HashTableItem<T> hash = Get(hashId) as HashTableItem<T>;
					if (hash.Contains(key)) { return hash.Get(key); }
					return default(T);
				}
				return default(T);
			}

			/// <summary>获取整个哈希表的数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public List<T> HashGetAll<T>(string hashId)
			{
				if (Contains(hashId))
				{
					HashTableItem<T> hash = GetCacheItem(hashId) as HashTableItem<T>;
					return hash.Values;
				}
				return null;
			}


			/// <summary>确定哈希表中是否存在某个缓存项。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public Task<bool> HashExistsAsync(string hashId, string key)
			{
				if (Contains(hashId))
				{
					IHashTableItem hash = Get(hashId) as IHashTableItem;
					return Task.FromResult(hash.Contains(key));
				}
				return Task.FromResult(false);
			}

			/// <summary>获取整个哈希表的数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public Task<IList<T>> HashGetAllAsync<T>(string hashId)
			{
				if (Contains(hashId))
				{
					HashTableItem<T> hash = GetCacheItem(hashId) as HashTableItem<T>;
					return Task.FromResult<IList<T>>(hash.Values);
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
				if (Contains(hashId))
				{
					HashTableItem<T> hash = Get(hashId) as HashTableItem<T>;
					if (hash.Contains(key)) { hash.Set(key, value); }
					else { hash.Add(key, value); }
					return Task.FromResult(true);
				}
				else
				{
					HashTableItem<T> hash = new HashTableItem<T>(hashId);
					hash.Add(key, value);
					base.Set(key, value, DateTimeOffset.MaxValue);
					return Task.FromResult(true);
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
				if (Contains(hashId))
				{
					HashTableItem<T> hash = Get(hashId) as HashTableItem<T>;
					if (hash.Contains(key)) { hash.Set(key, value); }
					else { hash.Add(key, value); }
					return Task.FromResult(true);
				}
				else
				{
					HashTableItem<T> hash = new HashTableItem<T>(hashId);
					hash.Add(key, value);
					return SetAsync(hashId, hash, expiresAt);
				}
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool HashSet<T>(string hashId, string key, T value)
			{
				if (Contains(hashId))
				{
					HashTableItem<T> hash = Get(hashId) as HashTableItem<T>;
					if (hash.Contains(key)) { hash.Set(key, value); }
					else { hash.Add(key, value); }
					return true;
				}
				else
				{
					HashTableItem<T> hash = new HashTableItem<T>(hashId);
					hash.Add(key, value);
					base.Set(key, value, DateTimeOffset.MaxValue);
					return true;
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
				if (Contains(hashId))
				{
					HashTableItem<T> hash = Get(hashId) as HashTableItem<T>;
					if (hash.Contains(key)) { hash.Set(key, value); }
					else { hash.Add(key, value); }
					return true;
				}
				else
				{
					HashTableItem<T> hash = new HashTableItem<T>(hashId);
					hash.Add(key, value);
					return Set(hashId, hash, expiresAt);
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
				if (Contains(hashId))
				{
					HashTableItem<T> hash = Get(hashId) as HashTableItem<T>;
					if (hash.Contains(key)) { hash.Set(key, value); }
					else { hash.Add(key, value); }
					return true;
				}
				else
				{
					HashTableItem<T> hash = new HashTableItem<T>(hashId);
					hash.Add(key, value);
					return Set(hashId, hash, expiresIn);
				}
			}

			/// <summary>确定哈希表中是否存在某个缓存项。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public bool HashContains(string hashId, string key)
			{
				if (Contains(hashId))
				{
					IHashTableItem hash = Get(hashId) as IHashTableItem;
					return hash.Contains(key);
				}
				return false;
			}

			#region 实现哈希表
			/// <summary>
			/// 表示 Hash 表缓存项
			/// </summary>
			private interface IHashTableItem
			{
				/// <summary>
				/// <![CDATA[确定 HashTableItem 是否包含特定键。]]>
				/// </summary>
				/// <param name="key">要添加的元素的键。</param>
				/// <returns>如果 HashTableItem 包含具有指定键的元素，则为 true；否则为 false。</returns>
				bool Contains(string key);

				/// <summary>
				/// <![CDATA[从 HashTableItem 中移除带有指定键的元素。]]>
				/// </summary>
				/// <param name="key">要移除的元素的键。</param>
				/// <returns>如果该元素已成功移除，则为 true；否则为 false。 如果在原始 HashTableItem 中没有找到 key，此方法也会返回 false。</returns>
				bool Remove(string key);
			}

			/// <summary>
			/// 表示 Hash 表缓存项
			/// </summary>
			private sealed class HashTableItem<T> : CacheItem, IHashTableItem
			{
				private readonly System.Collections.Concurrent.ConcurrentDictionary<string, T> _hash;
				/// <summary>
				/// 使用指定的缓存项键初始化新的 HashTableItem 实例。
				/// </summary>
				/// <param name="hashId">HashTableItem 项的唯一标识符。</param>
				public HashTableItem(string hashId) : base(hashId, new ConcurrentDictionary<string, T>(-1, 10)) { _hash = (ConcurrentDictionary<string, T>)Value; }

				/// <summary>获得一个包含 T 中的值的集合。</summary>
				public List<T> Values { get { return new List<T>(_hash.Values); } }

				/// <summary>获取一个按排序顺序包含的键的集合。</summary>
				public IList<string> Keys { get { return new List<string>(_hash.Keys); } }

				/// <summary>
				/// <![CDATA[将带有指定键和值的元素添加到 HashTableItem 中。]]>
				/// </summary>
				/// <param name="key">要添加的元素的键。</param>
				/// <param name="value">要添加的元素的值。 对于引用类型，该值可以为 null。</param>
				public bool Add(string key, T value) { return _hash.TryAdd(key, value); }

				/// <summary>
				/// <![CDATA[将带有指定键和值的元素添加到 HashTableItem 中。]]>
				/// </summary>
				/// <param name="key">要添加的元素的键。</param>
				/// <param name="value">要添加的元素的值。 对于引用类型，该值可以为 null。</param>
				public void Set(string key, T value) { _hash[key] = value; }

				/// <summary>
				/// <![CDATA[将带有指定键和值的元素添加到 HashTableItem 中。]]>
				/// </summary>
				/// <param name="key">要添加的元素的键。</param>
				public T Get(string key) { return _hash[key]; }

				/// <summary>
				/// <![CDATA[从 HashTableItem 中移除所有元素。]]>
				/// </summary>
				public void Clear() { _hash.Clear(); }

				/// <summary>
				/// <![CDATA[确定 HashTableItem 是否包含特定键。]]>
				/// </summary>
				/// <param name="key">要添加的元素的键。</param>
				/// <returns>如果 HashTableItem 包含具有指定键的元素，则为 true；否则为 false。</returns>
				public bool Contains(string key) { return _hash.ContainsKey(key); }

				/// <summary>
				/// <![CDATA[从 HashTableItem 中移除带有指定键的元素。]]>
				/// </summary>
				/// <param name="key">要移除的元素的键。</param>
				/// <returns>如果该元素已成功移除，则为 true；否则为 false。 如果在原始 HashTableItem 中没有找到 key，此方法也会返回 false。</returns>
				public bool Remove(string key) { return _hash.TryRemove(key, out T _); }
			}

			#endregion
		}
#endif
	}
}
