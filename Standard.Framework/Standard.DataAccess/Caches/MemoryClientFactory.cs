
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Basic.DataAccess
{
	/// <summary>
	/// 进程内缓存工厂类
	/// </summary>
	public sealed class MemoryClientFactory : CacheClientFactory
	{
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
			/// <summary>
			/// 初始化 MemoryCacheClient 类实例。
			/// </summary>
			/// <param name="name"></param>
			public MemoryCacheClient(string name) : base(name) { }

			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public List<string> GetKeys() { return this.Select(m => m.Key).ToList(); }

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
					HashCacheItem hash = Get(hashId) as HashCacheItem;
					return hash.Remove(key);
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
				if (Contains(hashId))
				{
					HashCacheItem<T> hash = Get(hashId) as HashCacheItem<T>;
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
					HashCacheItem<T> hash = GetCacheItem(hashId) as HashCacheItem<T>;
					return hash.Values;
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
				if (Contains(hashId))
				{
					HashCacheItem<T> hash = Get(hashId) as HashCacheItem<T>;
					if (hash.Contains(key)) { hash.Set(key, value); }
					else { hash.Add(key, value); }
					return true;
				}
				else
				{
					HashCacheItem<T> hash = new HashCacheItem<T>(hashId);
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
					HashCacheItem<T> hash = Get(hashId) as HashCacheItem<T>;
					if (hash.Contains(key)) { hash.Set(key, value); }
					else { hash.Add(key, value); }
					return true;
				}
				else
				{
					HashCacheItem<T> hash = new HashCacheItem<T>(hashId);
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
					HashCacheItem<T> hash = Get(hashId) as HashCacheItem<T>;
					if (hash.Contains(key)) { hash.Set(key, value); }
					else { hash.Add(key, value); }
					return true;
				}
				else
				{
					HashCacheItem<T> hash = new HashCacheItem<T>(hashId);
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
					HashCacheItem hash = Get(hashId) as HashCacheItem;
					return hash.Contains(key);
				}
				return false;
			}
		}


		/// <summary>
		/// 表示 Hash 表缓存项
		/// </summary>
		public abstract class HashCacheItem : CacheItem
		{
			/// <summary>
			/// 使用指定的缓存项键初始化新的 HashCacheItem 实例。
			/// </summary>
			/// <param name="hashId">HashCacheItem 项的唯一标识符。</param>
			/// <param name="value"></param>
			protected HashCacheItem(string hashId, object value) : base(hashId, value) { }

			/// <summary>
			/// <![CDATA[确定 HashCacheItem 是否包含特定键。]]>
			/// </summary>
			/// <param name="key">要添加的元素的键。</param>
			/// <returns>如果 HashCacheItem 包含具有指定键的元素，则为 true；否则为 false。</returns>
			public abstract bool Contains(string key);

			/// <summary>
			/// <![CDATA[从 HashCacheItem 中移除带有指定键的元素。]]>
			/// </summary>
			/// <param name="key">要移除的元素的键。</param>
			/// <returns>如果该元素已成功移除，则为 true；否则为 false。 如果在原始 HashCacheItem 中没有找到 key，此方法也会返回 false。</returns>
			public abstract bool Remove(string key);
		}

		/// <summary>
		/// 表示 Hash 表缓存项
		/// </summary>
		public sealed class HashCacheItem<T> : HashCacheItem
		{
			private readonly SortedList<string, T> mItems;
			/// <summary>
			/// 使用指定的缓存项键初始化新的 HashCacheItem 实例。
			/// </summary>
			/// <param name="hashId">HashCacheItem 项的唯一标识符。</param>
			public HashCacheItem(string hashId) : base(hashId, new SortedList<string, T>(100)) { mItems = (SortedList<string, T>)Value; }

			/// <summary>获得一个包含 T 中的值的集合。</summary>
			public List<T> Values { get { return new List<T>(mItems.Values); } }

			/// <summary>获取一个按排序顺序包含的键的集合。</summary>
			public IList<string> Keys { get { return mItems.Keys; } }

			/// <summary>
			/// <![CDATA[将带有指定键和值的元素添加到 HashCacheItem 中。]]>
			/// </summary>
			/// <param name="key">要添加的元素的键。</param>
			/// <param name="value">要添加的元素的值。 对于引用类型，该值可以为 null。</param>
			public void Add(string key, T value) { mItems.Add(key, value); }

			/// <summary>
			/// <![CDATA[将带有指定键和值的元素添加到 HashCacheItem 中。]]>
			/// </summary>
			/// <param name="key">要添加的元素的键。</param>
			/// <param name="value">要添加的元素的值。 对于引用类型，该值可以为 null。</param>
			public void Set(string key, T value) { mItems[key] = value; }

			/// <summary>
			/// <![CDATA[将带有指定键和值的元素添加到 HashCacheItem 中。]]>
			/// </summary>
			/// <param name="key">要添加的元素的键。</param>
			public T Get(string key) { return mItems[key]; }

			/// <summary>
			/// <![CDATA[从 HashCacheItem 中移除所有元素。]]>
			/// </summary>
			public void Clear() { mItems.Clear(); }

			/// <summary>
			/// <![CDATA[确定 HashCacheItem 是否包含特定键。]]>
			/// </summary>
			/// <param name="key">要添加的元素的键。</param>
			/// <returns>如果 HashCacheItem 包含具有指定键的元素，则为 true；否则为 false。</returns>
			public override bool Contains(string key) { return mItems.ContainsKey(key); }

			/// <summary>
			/// <![CDATA[从 HashCacheItem 中移除带有指定键的元素。]]>
			/// </summary>
			/// <param name="key">要移除的元素的键。</param>
			/// <returns>如果该元素已成功移除，则为 true；否则为 false。 如果在原始 HashCacheItem 中没有找到 key，此方法也会返回 false。</returns>
			public override bool Remove(string key) { return mItems.Remove(key); }
		}
	}
}
