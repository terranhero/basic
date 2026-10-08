using Basic.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Standard.CacheClients
{
	/// <summary>
	/// <see cref="Basic.Caches.MemoryClientFactory"/> 及其内部实现 <c>MemoryCacheClient</c>（进程内内存缓存）的单元测试。
	/// 由于内部类为私有，全部测试均通过 <see cref="MemoryClientFactory.CreateClient"/> 返回的 <see cref="ICacheClient"/> 实例进行。
	/// </summary>
	[Collection("AssemblyInitializeCollection")]
	public class MemoryClientFactoryTest
	{
		private readonly MemoryClientFactory factory = new MemoryClientFactory();
		private readonly ITestOutputHelper _output;
		public MemoryClientFactoryTest(ITestOutputHelper output) { _output = output; }

		/// <summary>为每个测试创建独立的缓存客户端，避免 <see cref="MemoryClientFactory"/> 中静态缓存字典互相干扰。</summary>
		private ICacheClient NewClient([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
		{
			return factory.CreateClient($"{memberName}_{Guid.NewGuid():N}");
		}

		#region CreateClient - 实例复用
		[Fact(DisplayName = "CreateClient: 相同名称复用同一实例")]
		public void CreateClient_SameName_ReturnsSameInstance()
		{
			string name = $"Same_{Guid.NewGuid():N}";
			ICacheClient first = factory.CreateClient(name);
			ICacheClient second = factory.CreateClient(name);
			Assert.Same(first, second);
		}

		[Fact(DisplayName = "CreateClient: 不同名称返回不同实例")]
		public void CreateClient_DifferentName_ReturnsDifferentInstances()
		{
			ICacheClient first = factory.CreateClient($"DiffA_{Guid.NewGuid():N}");
			ICacheClient second = factory.CreateClient($"DiffB_{Guid.NewGuid():N}");
			Assert.NotSame(first, second);
		}
		#endregion

		#region GetKeyInfos / GetKeyInfosAsync / GetKeys
		[Fact(DisplayName = "GetKeys/GetKeyInfos/GetKeyInfosAsync: 返回所有缓存键")]
		public async Task GetKeys_And_KeyInfos()
		{
			ICacheClient cache = NewClient();
			cache.Set("key_1", "v1", TimeSpan.FromMinutes(5));
			cache.Set("key_2", 2, TimeSpan.FromMinutes(5));

			IEnumerable<string> keys = cache.GetKeys();
			Assert.Contains("key_1", keys);
			Assert.Contains("key_2", keys);

			IEnumerable<KeyInfo> infos = cache.GetKeyInfos();
			Assert.Contains(infos, m => m.KeyName == "key_1");
			Assert.Contains(infos, m => m.KeyName == "key_2");

			IEnumerable<KeyInfo> infosAsync = await cache.GetKeyInfosAsync();
			Assert.NotNull(infosAsync);
			Assert.Contains(infosAsync, m => m.KeyName == "key_1");
			_output.WriteLine($"GetKeyInfos 返回 {infos.Count()} 个键。");
		}
		#endregion

		#region KeyDelete / KeyDeleteAsync
		[Fact(DisplayName = "KeyDelete: 单键、数组及 null/空数组安全")]
		public void KeyDelete_Sync_AllOverloads()
		{
			ICacheClient cache = NewClient();
			cache.Set("a", 1, TimeSpan.FromMinutes(5));
			cache.Set("b", 2, TimeSpan.FromMinutes(5));

			Assert.True(cache.KeyDelete("a"));
			Assert.False(cache.KeyExists("a"));

			cache.KeyDelete(new string[] { "b", "not_exist" });
			Assert.False(cache.KeyExists("b"));

			cache.KeyDelete((string[])null);
			cache.KeyDelete(Array.Empty<string>());
		}

		[Fact(DisplayName = "KeyDeleteAsync: 单键、null 键、数组及 null/空数组")]
		public async Task KeyDelete_Async_AllOverloads()
		{
			ICacheClient cache = NewClient();
			cache.Set("a", 1, TimeSpan.FromMinutes(5));
			cache.Set("b", 2, TimeSpan.FromMinutes(5));

			Assert.False(await cache.KeyDeleteAsync((string)null));
			Assert.True(await cache.KeyDeleteAsync("a"));
			Assert.False(cache.KeyExists("a"));

			await cache.KeyDeleteAsync(new string[] { "b", "not_exist" });
			Assert.False(cache.KeyExists("b"));

			await cache.KeyDeleteAsync((string[])null);
			await cache.KeyDeleteAsync(Array.Empty<string>());
		}
		#endregion

		#region KeyExists / KeyExistsAsync
		[Fact(DisplayName = "KeyExists/KeyExistsAsync: 键存在性判断")]
		public async Task KeyExists_Sync_And_Async()
		{
			ICacheClient cache = NewClient();

			Assert.False(cache.KeyExists("missing"));
			Assert.False(await cache.KeyExistsAsync("missing"));

			cache.Set("exists", "v", TimeSpan.FromMinutes(5));
			Assert.True(cache.KeyExists("exists"));
			Assert.True(await cache.KeyExistsAsync("exists"));
		}
		#endregion

		#region KeyExpire / KeyExpireAsync - 进程内缓存不支持
		[Fact(DisplayName = "KeyExpire/KeyExpireAsync: 进程内缓存不支持,始终返回 false")]
		public async Task KeyExpire_AllOverloads_ReturnFalse()
		{
			ICacheClient cache = NewClient();
			cache.Set("k", "v", TimeSpan.FromMinutes(5));

			Assert.False(cache.KeyExpire("k", DateTime.Now.AddMinutes(1)));
			Assert.False(cache.KeyExpire("k", TimeSpan.FromMinutes(1)));
			Assert.False(await cache.KeyExpireAsync("k", DateTime.Now.AddMinutes(1)));
			Assert.False(await cache.KeyExpireAsync("k", TimeSpan.FromMinutes(1)));
		}
		#endregion

		#region Get<T> / Set<T>
		[Fact(DisplayName = "Get<T>/Set<T>: 单键同步读写(TimeSpan/DateTime)")]
		public void Get_Set_SingleKey()
		{
			ICacheClient cache = NewClient();

			Assert.Equal(0, cache.Get<int>("age"));
			Assert.Null(cache.Get<string>("name"));

			Assert.True(cache.Set("age", 18, TimeSpan.FromMinutes(5)));
			Assert.Equal(18, cache.Get<int>("age"));

			Assert.True(cache.Set("age", 20, DateTime.Now.AddMinutes(5)));
			Assert.Equal(20, cache.Get<int>("age"));
		}

		[Fact(DisplayName = "Get<T>(params keys): 多键读取与 null/空数组")]
		public void Get_MultipleKeys()
		{
			ICacheClient cache = NewClient();
			cache.Set("k1", "v1", TimeSpan.FromMinutes(5));
			cache.Set("k2", "v2", TimeSpan.FromMinutes(5));

			IDictionary<string, string> values = cache.Get<string>("k1", "k2", "not_exist");
			Assert.NotNull(values);
			Assert.Equal(2, values.Count);
			Assert.Equal("v1", values["k1"]);
			Assert.Equal("v2", values["k2"]);

			Assert.Null(cache.Get<string>((string[])null));
			Assert.Null(cache.Get<string>(Array.Empty<string>()));
		}
		#endregion

		#region SetAsync<T> / GetAsync<T>
		[Fact(DisplayName = "SetAsync/GetAsync<T>: 单键异步读写")]
		public async Task SetAsync_GetAsync_SingleKey()
		{
			ICacheClient cache = NewClient();

			Assert.True(await cache.SetAsync("k", "hello", DateTime.Now.AddMinutes(5)));
			Assert.Equal("hello", await cache.GetAsync<string>("k"));
			Assert.Null(await cache.GetAsync<string>("missing"));
		}

		[Fact(DisplayName = "GetAsync<T>(keys): 异步多键读取与 null/空数组")]
		public async Task GetAsync_MultipleKeys()
		{
			ICacheClient cache = NewClient();
			cache.Set("k1", 100, TimeSpan.FromMinutes(5));
			cache.Set("k2", 200, TimeSpan.FromMinutes(5));

			IDictionary<string, int> values = await cache.GetAsync<int>(new[] { "k1", "k2", "not_exist" });
			Assert.NotNull(values);
			Assert.Equal(2, values.Count);
			Assert.Equal(100, values["k1"]);
			Assert.Equal(200, values["k2"]);

			Assert.Null(await cache.GetAsync<string>((string[])null));
			Assert.Null(await cache.GetAsync<string>(Array.Empty<string>()));
		}
		#endregion

		#region List<T> / ListLength<T> - 同步
		[Fact(DisplayName = "List<T>/ListLength<T>: 同步列表全部重载")]
		public void List_Sync_AllOverloads()
		{
			ICacheClient cache = NewClient();

			Assert.True(cache.List<string>("fruits", new List<string> { "apple", "banana" }));
			IList<string> read = cache.List<string>("fruits");
			Assert.NotNull(read);
			Assert.Equal(2, read.Count);
			Assert.Contains("apple", read);
			Assert.Contains("banana", read);
			Assert.Equal(2L, cache.ListLength<string>("fruits"));

			Assert.True(cache.List<string>("fruits", new List<string> { "cherry" }, TimeSpan.FromMinutes(5)));
			Assert.True(cache.List<string>("fruits", new List<string> { "date" }, DateTime.Now.AddMinutes(5)));
			_output.WriteLine($"List<string> 元素数: {cache.List<string>("fruits").Count}");

			Assert.Null(cache.List<string>("not_exist"));
		}
		#endregion

		#region ListAsync<T> / ListPushAsync<T> / ListLengthAsync<T> - 异步
		[Fact(DisplayName = "ListAsync/ListPushAsync/ListLengthAsync: 异步列表全部重载")]
		public async Task List_Async_AllOverloads()
		{
			ICacheClient cache = NewClient();

			Assert.True(await cache.ListAsync<string>("list", new List<string> { "a", "b" }));
			IList<string> read = await cache.ListAsync<string>("list");
			Assert.NotNull(read);
			Assert.Equal(2, read.Count);
			Assert.Equal(2L, await cache.ListLengthAsync<string>("list"));

			Assert.True(await cache.ListPushAsync("list", "c"));
			Assert.Equal(3, (await cache.ListAsync<string>("list")).Count);

			Assert.True(await cache.ListAsync<string>("list", new List<string> { "d" }, TimeSpan.FromMinutes(5)));
			Assert.True(await cache.ListAsync<string>("list", new List<string> { "e" }, DateTime.Now.AddMinutes(5)));

			Assert.Null(await cache.ListAsync<string>("not_exist"));
		}
		#endregion

		#region Hash 同步
		[Fact(DisplayName = "Hash 同步: HashSet/HashGet/HashExists/HashDelete/HashLength/HashGetAll")]
		public void Hash_Sync_AllOverloads()
		{
			ICacheClient cache = NewClient();
			const string hashId = "user_hash";

			Assert.False(cache.HashExists(hashId, "f1"));

			Assert.True(cache.HashSet(hashId, "f1", "v1"));
			Assert.True(cache.HashSet(hashId, "f2", "v2"));
			Assert.True(cache.HashSet(hashId, "f3", "v3", TimeSpan.FromMinutes(5)));
			Assert.True(cache.HashSet(hashId, "f4", "v4", DateTime.Now.AddMinutes(5)));

			Assert.Equal("v1", cache.HashGet<string>(hashId, "f1"));
			Assert.Null(cache.HashGet<string>(hashId, "missing"));
			Assert.True(cache.HashExists(hashId, "f1"));
			Assert.False(cache.HashExists(hashId, "missing"));
			Assert.Equal(4L, cache.HashLength<string>(hashId));

			List<string> all = cache.HashGetAll<string>(hashId);
			Assert.NotNull(all);
			Assert.Equal(4, all.Count);

			Assert.True(cache.HashDelete(hashId, "f1"));
			Assert.False(cache.HashExists(hashId, "f1"));
			Assert.Equal(3L, cache.HashLength<string>(hashId));
		}
		#endregion

		#region Hash 异步
		[Fact(DisplayName = "Hash 异步: HashSetAsync/HashGetAsync/HashExistsAsync/HashDeleteAsync/HashLengthAsync/HashGetAllAsync")]
		public async Task Hash_Async_AllOverloads()
		{
			ICacheClient cache = NewClient();
			const string hashId = "user_hash_async";

			Assert.False(await cache.HashExistsAsync(hashId, "f1"));

			Assert.True(await cache.HashSetAsync(hashId, "f1", "v1"));
			Assert.True(await cache.HashSetAsync(hashId, "f2", "v2"));
			Assert.True(await cache.HashSetAsync(hashId, "f3", "v3", TimeSpan.FromMinutes(5)));
			Assert.True(await cache.HashSetAsync(hashId, "f4", "v4", DateTime.Now.AddMinutes(5)));

			Assert.Equal("v1", await cache.HashGetAsync<string>(hashId, "f1"));
			Assert.Null(await cache.HashGetAsync<string>(hashId, "missing"));
			Assert.True(await cache.HashExistsAsync(hashId, "f1"));
			Assert.Equal(4L, await cache.HashLengthAsync<string>(hashId));

			IList<string> all = await cache.HashGetAllAsync<string>(hashId);
			Assert.NotNull(all);
			Assert.Equal(4, all.Count);

			Assert.True(await cache.HashDeleteAsync(hashId, "f1"));
			Assert.False(await cache.HashExistsAsync(hashId, "f1"));
		}

		[Fact(DisplayName = "HashSetAsync(IDictionary): 批量存储键值对")]
		public async Task HashSetAsync_Dictionary()
		{
			ICacheClient cache = NewClient();
			const string hashId = "batch_hash";

			IDictionary<string, string> values = new Dictionary<string, string> { { "a", "1" }, { "b", "2" } };
			Assert.True(await cache.HashSetAsync(hashId, values));

			Assert.Equal(2L, await cache.HashLengthAsync<string>(hashId));
			Assert.Equal("1", cache.HashGet<string>(hashId, "a"));

			IDictionary<string, string> overwrite = new Dictionary<string, string> { { "a", "9" }, { "c", "3" } };
			Assert.True(await cache.HashSetAsync(hashId, overwrite));
			Assert.Equal("9", cache.HashGet<string>(hashId, "a"));
			Assert.Equal(3L, await cache.HashLengthAsync<string>(hashId));
		}
		#endregion

		#region Set 集合 - 同步
		[Fact(DisplayName = "SetAdd/SetLength/SetMembers: 集合同步全部重载")]
		public void Set_Sync_AllOverloads()
		{
			ICacheClient cache = NewClient();
			const string key = "tag_set";

			Assert.True(cache.SetAdd(key, "red"));
			Assert.True(cache.SetAdd(key, "green", DateTime.Now.AddMinutes(5)));
			Assert.Equal(2L, cache.SetLength<string>(key));

			ICollection<string> members = cache.SetMembers<string>(key);
			Assert.NotNull(members);
			Assert.Equal(2, members.Count);

			Assert.Equal(3L, cache.SetAdd<string>(key, new[] { "blue", "yellow", "red" }));
			Assert.Equal(2L, cache.SetAdd<string>(key, new[] { "purple", "orange" }, DateTime.Now.AddMinutes(5)));
			Assert.Equal(6L, cache.SetLength<string>(key));
			Assert.Contains("blue", cache.SetMembers<string>(key));

			Assert.Null(cache.SetMembers<string>("not_exist"));
		}
		#endregion

		#region Set 集合 - 异步
		[Fact(DisplayName = "SetAddAsync/SetLengthAsync/SetMembersAsync: 集合异步全部重载")]
		public async Task Set_Async_AllOverloads()
		{
			ICacheClient cache = NewClient();
			const string key = "tag_set_async";

			Assert.True(await cache.SetAddAsync(key, "red"));
			Assert.True(await cache.SetAddAsync(key, "green", DateTime.Now.AddMinutes(5)));
			Assert.Equal(2L, await cache.SetLengthAsync<string>(key));

			ICollection<string> members = await cache.SetMembersAsync<string>(key);
			Assert.NotNull(members);
			Assert.Equal(2, members.Count);

			Assert.Equal(3L, await cache.SetAddAsync<string>(key, new[] { "blue", "yellow", "red" }));
			Assert.Equal(2L, await cache.SetAddAsync<string>(key, new[] { "purple", "orange" }, DateTime.Now.AddMinutes(5)));

			Assert.Null(await cache.SetMembersAsync<string>("not_exist"));
		}
		#endregion

		#region ZSet 有序集合 - 同步
		[Fact(DisplayName = "ZSetAdd/ZSetLength/ZSetMembers: 有序集合同步全部重载")]
		public void ZSet_Sync_AllOverloads()
		{
			ICacheClient cache = NewClient();
			const string key = "rank_zset";

			Assert.True(cache.ZSetAdd(key, "alice", 10.0));
			Assert.True(cache.ZSetAdd(key, "bob", 20.0, TimeSpan.FromMinutes(5)));
			Assert.True(cache.ZSetAdd(key, "carol", 30.0, DateTime.Now.AddMinutes(5)));
			Assert.Equal(3L, cache.ZSetLength<string>(key));

			Assert.Equal(2L, cache.ZSetAdd<string>(key, new[] { "dave", "eve" }, m => (double)m.Length));
			Assert.Equal(2L, cache.ZSetAdd<string>(key, new[] { "f1", "g1" }, m => (double)m.Length, TimeSpan.FromMinutes(5)));
			Assert.Equal(2L, cache.ZSetAdd<string>(key, new[] { "h1", "i1" }, m => (double)m.Length, DateTime.Now.AddMinutes(5)));

			ICollection<string> members = cache.ZSetMembers<string>(key);
			Assert.NotNull(members);
			Assert.Equal(9, members.Count);

			Assert.Null(cache.ZSetMembers<string>("not_exist"));
		}
		#endregion

		#region ZSet 有序集合 - 异步
		[Fact(DisplayName = "ZSetAddAsync/ZSetLengthAsync/ZSetMembersAsync: 有序集合异步全部重载")]
		public async Task ZSet_Async_AllOverloads()
		{
			ICacheClient cache = NewClient();
			const string key = "rank_zset_async";

			Assert.True(await cache.ZSetAddAsync(key, "alice", 10.0));
			Assert.True(await cache.ZSetAddAsync(key, "bob", 20.0, TimeSpan.FromMinutes(5)));
			Assert.True(await cache.ZSetAddAsync(key, "carol", 30.0, DateTime.Now.AddMinutes(5)));
			Assert.Equal(3L, await cache.ZSetLengthAsync<string>(key));

			Assert.Equal(2L, await cache.ZSetAddAsync<string>(key, new[] { "dave", "eve" }, m => (double)m.Length));
			Assert.Equal(2L, await cache.ZSetAddAsync<string>(key, new[] { "f1", "g1" }, m => (double)m.Length, TimeSpan.FromMinutes(5)));
			Assert.Equal(2L, await cache.ZSetAddAsync<string>(key, new[] { "h1", "i1" }, m => (double)m.Length, DateTime.Now.AddMinutes(5)));

			ICollection<string> members = await cache.ZSetMembersAsync<string>(key);
			Assert.NotNull(members);
			Assert.Equal(9, members.Count);

			Assert.Null(await cache.ZSetMembersAsync<string>("not_exist"));
		}
		#endregion

		#region 空集合 / null 参数保护
		[Fact(DisplayName = "Set/ZSet: 空集合或 null 成员参数保护")]
		public async Task Empty_And_Null_Collection_Safety()
		{
			ICacheClient cache = NewClient();

			Assert.Equal(0L, cache.SetAdd<string>("empty_set", Array.Empty<string>()));
			Assert.Equal(0L, cache.SetAdd<string>("empty_set", (string[])null));

			Assert.Equal(0L, await cache.SetAddAsync<string>("empty_set_async", Array.Empty<string>()));
			Assert.Equal(0L, await cache.SetAddAsync<string>("empty_set_async", (string[])null));

			Assert.Equal(0L, cache.ZSetAdd<string>("empty_zset", Array.Empty<string>(), m => 1.0));
			Assert.Equal(0L, cache.ZSetAdd<string>("empty_zset", (string[])null, m => 1.0));

			Assert.Equal(0L, await cache.ZSetAddAsync<string>("empty_zset_async", Array.Empty<string>(), m => 1.0));
			Assert.Equal(0L, await cache.ZSetAddAsync<string>("empty_zset_async", (string[])null, m => 1.0));
		}
		#endregion

		#region 不存在键的缺省行为
		[Fact(DisplayName = "不存在的键: 各方法返回默认值/空")]
		public async Task MissingKey_DefaultBehaviors()
		{
			ICacheClient cache = NewClient();

			Assert.Equal(0L, cache.HashLength<string>("missing_hash"));
			Assert.Null(cache.HashGetAll<string>("missing_hash"));
			Assert.Null(await cache.HashGetAllAsync<string>("missing_hash"));
			Assert.Null(cache.HashGet<string>("missing_hash", "field"));
			Assert.False(cache.HashExists("missing_hash", "field"));
			Assert.False(cache.HashDelete("missing_hash", "field"));
			Assert.False(await cache.HashExistsAsync("missing_hash", "field"));
			Assert.False(await cache.HashDeleteAsync("missing_hash", "field"));

			Assert.Null(cache.Get<string>("missing_key"));
			Assert.Null(await cache.GetAsync<string>("missing_key"));
			Assert.False(cache.KeyExists("missing_key"));
			Assert.False(await cache.KeyExistsAsync("missing_key"));
		}
		#endregion

		#region 复杂对象缓存往返
		[Fact(DisplayName = "复杂对象 EmployeeAccountInfo 的缓存往返读写")]
		public void ComplexObject_RoundTrip()
		{
			ICacheClient cache = NewClient();

			EmployeeAccountInfo employee = new EmployeeAccountInfo
			{
				CorpKey = 10,
				OrgKey = 20,
				LoginName = "tester",
				Password = "secret",
				MobilePhone = "13800000000",
				EmailAddress = "tester@example.com",
				UserKind = UserKinds.SystemUsers,
				Enabled = true
			};

			Assert.True(cache.Set("employee", employee, TimeSpan.FromMinutes(5)));
			EmployeeAccountInfo restored = cache.Get<EmployeeAccountInfo>("employee");
			Assert.NotNull(restored);
			Assert.Equal(10, restored.CorpKey);
			Assert.Equal(20, restored.OrgKey);
			Assert.Equal("tester", restored.LoginName);
			Assert.Equal(UserKinds.SystemUsers, restored.UserKind);
			Assert.Equal("13800000000", restored.MobilePhone);
		}
		#endregion
	}
}
