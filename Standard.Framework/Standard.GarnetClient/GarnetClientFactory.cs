using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Text.Json;
using Garnet.client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Basic.Caches
{
	/// <summary>_connection缓存工厂类</summary>
	public sealed class GarnetClientFactory : CacheClientFactory
	{
		private static ConcurrentDictionary<string, ICacheClient> caches = new ConcurrentDictionary<string, ICacheClient>(-1, 5);
		private readonly string _configuration = "127.0.0.1:6890,password=GoldSoft@2015";//,abortConnect=false
		private readonly int database = 0;//,abortConnect=false
		/// <summary>初始化 RedisClientFactory 类实例</summary>
		public GarnetClientFactory(string configuration, int db)
		{
			_configuration = configuration ?? "127.0.0.1:6890";
			database = db;
		}

		///// <summary>初始化 RedisClientFactory 类实例</summary>
		//public GarnetClientFactory(RuntimeOptions opts)
		//{
		//	_configuration = opts.RedisConnection ?? "127.0.0.1:6897";
		//	database = opts.RedisDatabase;
		//}

		///// <summary>初始化 RedisClientFactory 类实例</summary>
		//public RedisClientFactory() { }

		/// <summary>返回实现 ICacheClient 类的提供程序的类的一个新实例。</summary>
		/// <param name="name">数据库连接名称</param>
		/// <returns>返回缓存 ICacheClient 接口的实例。</returns>
		public override ICacheClient CreateClient(string name)
		{
			if (caches.TryGetValue(name, out ICacheClient client) == true) { return client; }

			client = new GarnetCacheClient(_configuration, database);
			caches.TryAdd(name, client);
			return client;
		}

		/// <summary>定义实现内存中缓存的类型。</summary>
		private class GarnetCacheClient : ICacheClient
		{
			private readonly SslClientAuthenticationOptions options;
			private readonly System.Net.EndPoint mEndPoint;
			private readonly GarnetClient _database;
			public GarnetCacheClient(string configuration, int db)
			{
				IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 6890);
				SslClientAuthenticationOptions options = new SslClientAuthenticationOptions();
				//options.Password = "GoldSoft@6897";
				//mEndPoint = options.EndPoints.First();
				_database = new GarnetClient(IPEndPoint.Parse(""), options);
				//_database = _connection..GetDatabase(db);
			}

			/// <summary>释放由 System.Runtime.Caching.MemoryCache 类的当前实例占用的所有资源。</summary>
			public void Dispose() { }

			#region 缓存序列化方法 -  序列化和反序列化操作
			//private static RedisValue SerializeValue<T>(T value)
			//{
			//	using (MemoryStream stream = new MemoryStream())
			//	{
			//		JsonSerializer.Serialize<T>(stream, value);
			//		return RedisValue.CreateFrom(stream);
			//	}
			//}

			//private static T DeserializeValue<T>(RedisValue value)
			//{
			//	return JsonSerializer.Deserialize<T>(value);
			//}

			private static string Serialize<T>(T value)
			{
				return JsonSerializer.Serialize<T>(value);
			}

			private static T Deserialize<T>(string value)
			{
				return JsonSerializer.Deserialize<T>(value);
			}
			#endregion

			#region 缓存异步方法 - 获取缓存键信息

			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public IEnumerable<KeyInfo> GetKeyInfos()
			{
				IServer server = _connection.GetServer(mEndPoint);
				return server.Keys(_database.Database).Select(m => new KeyInfo(m)).ToList();
			}

			/// <summary>获取所有缓存的键</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public IEnumerable<KeyInfo> GetKeyInfosAsync()
			{
				IServer server = _connection.GetServer(mEndPoint);
				return server.Keys(_database.Database).Select(m => new KeyInfo(m)).ToList();
			}
			#endregion

			#region 缓存同步方法 - 获取所有缓存的键

			/// <summary>获取所有缓存的键。</summary>
			/// <returns>如果存在则返回键列表，否则返回 null。</returns>
			public IEnumerable<string> GetKeys()
			{
				IServer server = _connection.GetServer(mEndPoint);
				return server.Keys(_database.Database).Select(m => m.ToString()).ToList();
			}
			#endregion

			#region 缓存同步方法 - 移除缓存键及其数据

			/// <summary>根据传入的键移除一条记录</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public bool KeyDelete(string key)
			{
				_database.KeyDelete(key, null);
				return true;
			}

			/// <summary>从缓存中移除指定键的缓存项</summary>
			/// <param name="keys">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public void KeyDelete(string[] keys)
			{
				if (keys == null || keys.Length == 0) { return; }
				_database.KeyDelete(keys, null);
			}
			#endregion

			#region 缓存异步方法 - 移除缓存键及其数据
			/// <summary>根据传入的键移除一条记录</summary>
			/// <param name="key">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public async Task<bool> KeyDeleteAsync(string key)
			{
				if (key == null) { return false; }
				return await _database.KeyDeleteAsync(key);
			}

			/// <summary>从缓存中移除指定键的缓存项</summary>
			/// <param name="keys">需要移除记录的键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public async Task KeyDeleteAsync(string[] keys)
			{
				if (keys == null) { return; }

				await _database.KeyDeleteAsync(keys);
			}
			#endregion

			#region 缓存异步方法 - 判断缓存键是否存在
			/// <summary>使用异步方法判断键是否存在</summary>
			/// <param name="key">需要检查的缓存键.</param>
			/// <returns><see langword="true"/>如果键存在. <see langword="false"/> 如果键不存在.</returns>
			public async Task<bool> KeyExistsAsync(string key)
			{
				if (key == null) { return false; }

				return await _database.KeyExistsAsync(key);
			}
			#endregion

			#region 缓存同步方法 - 判断键是否存在
			/// <summary>判断键是否存在</summary>
			/// <param name="key">需要检查的缓存键.</param>
			/// <returns><see langword="true"/> 如果键存在. <see langword="false"/> 如果键不存在.</returns>
			public bool KeyExists(string key)
			{
				if (key == null) { return false; }

				return _database.KeyExists(key);
			}
			#endregion

			#region 缓存异步方法 - 设置键过期策略
			/// <summary>设置键滑动过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public async Task<bool> KeyExpireAsync(string key, TimeSpan expiry)
			{
				if (key == null) { return false; }
				return await _database.KeyExpireAsync(key, expiry);
			}

			/// <summary>设置键绝对过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public async Task<bool> KeyExpireAsync(string key, DateTime expiry)
			{
				if (key == null) { return false; }
				return await _database.KeyExpireAsync(key, expiry);
			}
			#endregion

			#region 缓存同步方法 - 设置键过期策略
			/// <summary>设置键绝对过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public bool KeyExpire(string key, DateTime expiry)
			{
				if (key == null) { return false; }
				return _database.KeyExpire(key, expiry);
			}

			/// <summary>设置键滑动过期策略</summary>
			/// <param name="key">需要设置绝对过期时间的缓存键.</param>
			/// <param name="expiry">绝对过期时间</param>
			/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
			public bool KeyExpire(string key, TimeSpan expiry)
			{
				if (key == null) { return false; }
				return _database.KeyExpire(key, expiry);
			}
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
				if (_database.KeyExists(key) == false) { return default(T); }
				string value = _database.StringGet(key);
				if (value == null) { return default(T); }
				return Deserialize<T>(value);
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
				Dictionary<string, T> values = new Dictionary<string, T>(keys.Length);
				foreach (string key in keys)
				{
					string value = _database.StringGet(key);
					if (value == null) { continue; }
					values[key] = Deserialize<T>(key);
				}
				return values;
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="value">该缓存项的数据。</param>
			/// <param name="expiresAt">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool Set<T>(string key, T value, DateTime expiresAt)
			{
				string value1 = Serialize<T>(value);
				return _database.StringSet(key, value1, expiresAt - DateTime.Now);
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="value">该缓存项的数据。</param>
			/// <param name="expiresIn">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool Set<T>(string key, T value, TimeSpan expiresIn)
			{
				string value1 = Serialize<T>(value);
				return _database.StringSet(key, value1, expiresIn);
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
			public async Task<bool> SetAsync<T>(string key, T value, DateTime expiresAt)
			{
				string value1 = Serialize<T>(value);
				return await _database.StringSetAsync(key, value1, expiresAt - DateTime.Now);
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public async Task<T> GetAsync<T>(string key)
			{
				if (_database.KeyExists(key) == false) { return default(T); }
				string value = await _database.StringGetAsync(key);
				if (value == null) { return default(T); }
				return Deserialize<T>(value);
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
			/// <returns>与指定的键对应的一组缓存项。</returns>
			public async Task<IDictionary<string, T>> GetAsync<T>(string[] keys)
			{
				if (keys == null || keys.Length == 0) { return null; }
				Dictionary<string, T> values = new Dictionary<string, T>(keys.Length);
				foreach (string key in keys)
				{
					string value = await _database.StringGetAsync(key);
					if (value == null) { continue; }
					values[key] = Deserialize<T>(key);
				}
				return values;
			}
			#endregion

			#region 缓存异步方法 - 列表操作，设置，插入
			/// <summary>通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="item">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public async Task<bool> ListPushAsync<T>(string key, T item)
			{
				if (item == null) { return false; }

				long length = await _database.ListRightPushAsync(key, new RedisValue(Serialize(item)));
				return length > 0;
			}

			/// <summary>通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public async Task<bool> ListAsync<T>(string key, IList<T> values)
			{
				if (values == null || values.Count == 0) { return false; }
				RedisValue[] list = values.Select(m => SerializeValue(m)).ToArray();
				long length = await _database.ListRightPushAsync(key, list);
				return length > 0;
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public async Task<bool> ListAsync<T>(string key, IList<T> values, DateTime expiresAt)
			{
				if (values == null || values.Count == 0) { return false; }
				RedisValue[] list = values.Select(m => SerializeValue(m)).ToArray();
				long length = await _database.ListRightPushAsync(key, list);

				if (_database.KeyExists(key) == true) { return await _database.KeyExpireAsync(key, expiresAt); }
				return length > 0;
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public async Task<bool> ListAsync<T>(string key, IList<T> values, TimeSpan expiresIn)
			{
				if (values == null || values.Count == 0) { return false; }

				long length = await _database.ListRightPushAsync(key, values.Select(m => SerializeValue(m)).ToArray());

				if (_database.KeyExists(key) == true) { return await _database.KeyExpireAsync(key, expiresIn); }
				return length > 0;
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			async Task<long> ICacheClient.ListLengthAsync<T>(string key)
			{
				return await _database.ListLengthAsync(key);
			}

			/// <summary>
			///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public async Task<IList<T>> ListAsync<T>(string key)
			{
				if (await _database.KeyExistsAsync(key) == false) { return null; }
				return await _database.ListRangeAsync(key).ContinueWith(results =>
				{
					return results.Result.Select(m => Deserialize<T>(m)).ToList();
				});
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
				if (_database.KeyExists(key) == false) { return null; }
				return _database.ListRange(key).Select(m => Deserialize<T>(m)).ToList();
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			long ICacheClient.ListLength<T>(string key)
			{
				return _database.ListLength(key);
			}

			/// <summary>
			/// 通过使用键、列标值和逐出设置，将某个缓存项插入缓存中
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">要插入的缓存项的唯一标识符</param>
			/// <param name="values">该缓存项的数据列表</param>
			/// <returns>创建成功则为true，否则为false</returns>
			/// <exception cref="NotImplementedException"></exception>
			public bool List<T>(string key, IList<T> values)
			{
				if (values == null || values.Count == 0) { return false; }
				foreach (T item in values) { _database.ListRightPush(key, Serialize(item)); }
				return true;
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresAt">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool List<T>(string key, IList<T> values, DateTime expiresAt)
			{
				if (values == null || values.Count == 0) { return false; }
				foreach (T item in values)
				{
					_database.ListRightPush(key, Serialize(item));
				}
				if (_database.KeyExists(key) == true) { return _database.KeyExpire(key, expiresAt); }
				return false;
			}

			/// <summary>
			///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
			/// </summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
			/// <param name="values">该缓存项的数据列表。</param>
			/// <param name="expiresIn">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool List<T>(string key, IList<T> values, TimeSpan expiresIn)
			{
				if (values == null || values.Count == 0) { return false; }
				foreach (T item in values)
				{
					string value = Serialize(item);
					_database.ListRightPush(key, value);
				}
				if (_database.KeyExists(key) == true) { return _database.KeyExpire(key, expiresIn); }
				return false;
			}
			#endregion

			#region 缓存异步方法 - 哈希表操作，设置，插入
			/// <summary>确定哈希表中是否存在某个缓存项。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			async Task<bool> ICacheClient.HashDeleteAsync(string hashId, string key)
			{
				return await _database.HashDeleteAsync(hashId, key);
			}

			/// <summary>返回存储在key处的哈希中包含的字段数</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>哈希中的字段数，当键不存在时为 0</returns>
			async Task<long> ICacheClient.HashLengthAsync<T>(string hashId)
			{
				return await _database.HashLengthAsync(hashId);
			}

			/// <summary>确定哈希表中是否存在某个缓存项。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public async Task<bool> HashExistsAsync(string hashId, string key)
			{
				return await _database.HashExistsAsync(hashId, key);
			}

			/// <summary>从哈希表获取数据。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public async Task<T> HashGetAsync<T>(string hashId, string key)
			{
				RedisValue value = await _database.HashGetAsync(hashId, key);
				if (value.HasValue == false) { return default; }
				return Deserialize<T>(value);
			}

			/// <summary>获取整个哈希表的数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public async Task<IList<T>> HashGetAllAsync<T>(string hashId)
			{
				return await _database.HashGetAllAsync(hashId).ContinueWith(res =>
				{
					return res.Result.Where(m => m.Value.HasValue).Select(m => Deserialize<T>(m.Value)).ToList();
				});
			}

			/// <summary>从哈希表获取数据。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public async Task<bool> HashSetAsync<T>(string hashId, string key, T value)
			{
				string item = Serialize(value);
				return await _database.HashSetAsync(hashId, key, item);
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public async Task<bool> HashSetAsync<T>(string hashId, string key, T value, DateTime expiresAt)
			{
				string item = Serialize(value);
				bool result = await _database.HashSetAsync(hashId, key, item);
				if (result == false) { return result; }
				var results = await _database.HashFieldExpireAsync(hashId, new RedisValue[] { key }, expiresAt);
				return results.Any(m => m == ExpireResult.Success);
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public async Task<bool> HashSetAsync<T>(string hashId, string key, T value, TimeSpan expiresIn)
			{
				string item = Serialize(value);
				bool result = await _database.HashSetAsync(hashId, key, item);
				if (result == false) { return result; }
				var results = await _database.HashFieldExpireAsync(hashId, new RedisValue[] { key }, expiresIn);
				return results.Any(m => m == ExpireResult.Success);

				//return await _database.KeyExpireAsync(hashId, expiresIn);
			}
			#endregion

			#region 缓存同步方法 - 哈希表操作，设置，插入
			/// <summary>确定哈希表中是否存在某个缓存项。</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			public bool HashExists(string hashId, string key)
			{
				return _database.HashExists(hashId, key);
			}

			/// <summary>从哈希表获取数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public T HashGet<T>(string hashId, string key)
			{
				RedisValue value = _database.HashGet(hashId, key);
				if (value.HasValue == false) { return default; }
				return Deserialize<T>(value);
			}

			/// <summary>返回存储在key处的哈希中包含的字段数</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>哈希中的字段数，当键不存在时为 0</returns>
			long ICacheClient.HashLength<T>(string hashId)
			{
				return _database.HashLength(hashId);
			}

			/// <summary>获取整个哈希表的数据</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
			public List<T> HashGetAll<T>(string hashId)
			{
				return _database.HashGetAll(hashId).Where(m => m.Value.HasValue).Select(m => Deserialize<T>(m.Value)).ToList();
			}

			/// <summary>移除哈希表中的某值</summary>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <returns>移除成功则返回true，否则返回false。</returns>
			public bool HashDelete(string hashId, string key)
			{
				return _database.HashDelete(hashId, key);
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			public bool HashSet<T>(string hashId, string key, T value)
			{
				string item = Serialize(value);
				return _database.HashSet(hashId, key, item);
			}

			/// <summary>存储数据到哈希表</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="hashId">哈希表缓存键</param>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			private bool HashSet(string hashId, string key, string value, DateTime expiresAt)
			{
				bool result = _database.HashSet(hashId, key, value);
				if (result) { _database.HashFieldExpire(hashId, new RedisValue[] { key }, expiresAt); }
				return result;
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
				string item = Serialize(value);
				bool result = _database.HashSet(hashId, key, item);
				if (result) { _database.KeyExpire(hashId, expiresAt); }
				return result;
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
				string item = Serialize(value);
				bool result = _database.HashSet(hashId, key, item);
				if (result) { _database.KeyExpire(hashId, expiresIn); }
				return result;
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
				return _database.SetAdd(key, SerializeValue(value));
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			bool ICacheClient.SetAdd<T>(string key, T value, DateTime expiresAt)
			{
				bool result = _database.SetAdd(key, SerializeValue(value));

				if (result) { _database.KeyExpire(key, expiresAt); }
				return result;
			}

			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="items">需要添加到集合的项目列表</param>
			/// <returns>返回添加成功的集合项数量。</returns>
			long ICacheClient.SetAdd<T>(string key, IEnumerable<T> items)
			{
				if (items == null || items.Any() == false) { return 0; }
				return _database.SetAdd(key, items.Select(m => SerializeValue(m)).ToArray());
			}

			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="items">需要添加到集合的项目列表</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>返回添加成功的集合项数量。</returns>
			long ICacheClient.SetAdd<T>(string key, IEnumerable<T> items, DateTime expiresAt)
			{
				if (items == null || items.Any() == false) { return 0; }
				long count = _database.SetAdd(key, items.Select(m => SerializeValue(m)).ToArray());
				if (count > 0) { _database.KeyExpire(key, expiresAt); }
				return count;
			}

			/// <summary>返回存储在key处的集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			long ICacheClient.SetLength<T>(string key)
			{
				return _database.SetLength(key);
			}

			/// <summary>获取集合中所有成员</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			ICollection<T> ICacheClient.SetMembers<T>(string key)
			{
				RedisValue[] values = _database.SetMembers(key);
				if (values == null) { return null; }
				return values.Select(m => Deserialize<T>(m)).ToList();
			}

			/// <summary>存储数据到有序集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			bool ICacheClient.ZSetAdd<T>(string key, T value, double score)
			{
				return _database.SortedSetAdd(key, SerializeValue(value), score);
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
				bool result = _database.SortedSetAdd(key, SerializeValue(value), score);

				if (result) { _database.KeyExpire(key, expiresAt); }
				return result;
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
				bool result = _database.SortedSetAdd(key, SerializeValue(value), score);

				if (result) { _database.KeyExpire(key, expiresIn); }
				return result;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			long ICacheClient.ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc)
			{
				if (values == null || values.Any() == false) { return 0L; }
				return _database.SortedSetAdd(key, values.Select(m => new SortedSetEntry(SerializeValue(m), scoreFunc(m))).ToArray());
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
				if (values == null || values.Any() == false) { return 0L; }
				long result = _database.SortedSetAdd(key, values.Select(m => new SortedSetEntry(SerializeValue(m), scoreFunc(m))).ToArray());
				_database.KeyExpire(key, expiresAt);
				return result;
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
				if (values == null || values.Any() == false) { return 0L; }
				long result = _database.SortedSetAdd(key, values.Select(m => new SortedSetEntry(SerializeValue(m), scoreFunc(m))).ToArray());
				_database.KeyExpire(key, expiresIn);
				return result;
			}

			/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			long ICacheClient.ZSetLength<T>(string key)
			{
				return _database.SortedSetLength(key);
			}

			/// <summary>从有序集合中读取所有数据。</summary>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			ICollection<T> ICacheClient.ZSetMembers<T>(string key)
			{
				RedisValue[] values = _database.SortedSetRangeByRank(key);
				if (values == null) { return null; }
				return values.Select(m => Deserialize<T>(m)).ToList();
			}
			#endregion

			#region 缓存异步方法 - 集合和有序集合操作
			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			async Task<bool> ICacheClient.SetAddAsync<T>(string key, T value)
			{
				return await _database.SetAddAsync(key, SerializeValue(value));
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">哈希表键</param>
			/// <param name="value">哈希表值</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			async Task<bool> ICacheClient.SetAddAsync<T>(string key, T value, DateTime expiresAt)
			{
				bool result = await _database.SetAddAsync(key, SerializeValue(value));

				if (result) { _database.KeyExpire(key, expiresAt); }
				return await Task.FromResult(result);
			}

			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="items">需要添加到集合的项目列表</param>
			/// <returns>返回添加成功的集合项数量。</returns>
			async Task<long> ICacheClient.SetAddAsync<T>(string key, IEnumerable<T> items)
			{
				if (items == null || items.Any() == false) { return 0; }
				return await _database.SetAddAsync(key, items.Select(m => SerializeValue(m)).ToArray());
			}

			/// <summary>存储数据到集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="items">需要添加到集合的项目列表</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>返回添加成功的集合项数量。</returns>
			async Task<long> ICacheClient.SetAddAsync<T>(string key, IEnumerable<T> items, DateTime expiresAt)
			{
				if (items == null || items.Any() == false) { return 0; }
				long count = await _database.SetAddAsync(key, items.Select(m => SerializeValue(m)).ToArray());
				if (count > 0) { await _database.KeyExpireAsync(key, expiresAt); }
				return count;
			}

			/// <summary>返回存储在key处的集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			async Task<long> ICacheClient.SetLengthAsync<T>(string key)
			{
				return await _database.SetLengthAsync(key);
			}

			/// <summary>获取集合中所有成员</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			async Task<ICollection<T>> ICacheClient.SetMembersAsync<T>(string key)
			{
				RedisValue[] values = await _database.SetMembersAsync(key);
				return values.Select(m => DeserializeValue<T>(m)).ToList();
			}

			/// <summary>存储数据到有序集合。</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			async Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score)
			{
				return await _database.SortedSetAddAsync(key, SerializeValue(value), score);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresAt">指定键过期的时间点。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			async Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score, DateTime expiresAt)
			{
				bool result = await _database.SortedSetAddAsync(key, SerializeValue(value), score);

				if (result) { await _database.KeyExpireAsync(key, expiresAt); }
				return result;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="value">哈希表值</param>
			/// <param name="score">与元素关联的分数，用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			async Task<bool> ICacheClient.ZSetAddAsync<T>(string key, T value, double score, TimeSpan expiresIn)
			{
				bool result = await _database.SortedSetAddAsync(key, SerializeValue(value), score);

				if (result) { await _database.KeyExpireAsync(key, expiresIn); }
				return result;
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			async Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc)
			{
				if (values == null || values.Any() == false) { return 0L; }
				SortedSetEntry[] items = values.Select(m => new SortedSetEntry(SerializeValue(m), scoreFunc(m))).ToArray();
				return await _database.SortedSetAddAsync(key, items);
			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <param name="expiresAt">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			async Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, DateTime expiresAt)
			{
				if (values == null || values.Any() == false) { return 0L; }
				SortedSetEntry[] items = values.Select(m => new SortedSetEntry(SerializeValue(m), scoreFunc(m))).ToArray();
				long result = await _database.SortedSetAddAsync(key, items);

				await _database.KeyExpireAsync(key, expiresAt);
				return result;

			}

			/// <summary>存储数据到有序集合</summary>
			/// <typeparam name="T">缓存值类型</typeparam>
			/// <param name="key">集合键名</param>
			/// <param name="values">哈希表值</param>
			/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
			/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
			/// <returns>创建成功则为true，否则为false。</returns>
			async Task<long> ICacheClient.ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, TimeSpan expiresIn)
			{
				if (values == null || values.Any() == false) { return 0L; }
				SortedSetEntry[] items = values.Select(m => new SortedSetEntry(SerializeValue(m), scoreFunc(m))).ToArray();
				long result = await _database.SortedSetAddAsync(key, items);

				await _database.KeyExpireAsync(key, expiresIn);
				return result;
			}

			/// <summary>返回存储在key处的集合的集合基数（元素数）</summary>
			/// <param name="key">集合的键</param>
			/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
			async Task<long> ICacheClient.ZSetLengthAsync<T>(string key)
			{
				return await _database.SortedSetLengthAsync(key);
			}

			/// <summary>从有序集合中读取所有数据。</summary>
			/// <param name="key">哈希表键</param>
			/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
			async Task<ICollection<T>> ICacheClient.ZSetMembersAsync<T>(string key)
			{
				RedisValue[] values = await _database.SetMembersAsync(key);
				return values.Select(m => DeserializeValue<T>(m)).ToList();
			}
			#endregion
		}
	}



}
