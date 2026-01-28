using Basic.Caches;

namespace Standard.CacheClients;

[Collection("AssemblyInitializeCollection")]
public class RedisClientFactoryTest
{
	private readonly RedisClientFactory factory = new RedisClientFactory("127.0.0.1:6890,password=GoldSoft@6897", 0);
	private readonly ICacheClient cache;
	private readonly ITestOutputHelper _output;
	public RedisClientFactoryTest(ITestOutputHelper output) { _output = output; cache = factory.CreateClient("HRMS"); }

	[Fact()]
	public async Task TestHashGetAllAsync()
	{
		IList<EmployeeAccountInfo> users = await cache.HashGetAllAsync<EmployeeAccountInfo>("HASH_SYS_LOGINUSER_USERKEY");
		foreach (EmployeeAccountInfo user in users)
		{
			Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(user));
		}
		Assert.NotNull(users);
	}

	[Theory(), InlineData("D951B9A9-C300-458E-8047-813F36C91206")]
	public async Task TestHashGetAsync(string userKey)
	{
		EmployeeAccountInfo users = await cache.HashGetAsync<EmployeeAccountInfo>("HASH_SYS_LOGINUSER_USERKEY", userKey);
		Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(users));
		Assert.NotNull(users);

	}

	[Fact()]
	public async Task TestGetKeysAsync()
	{
		var keys = await cache.GetKeyInfosAsync();
		_output.WriteLine(System.Text.Json.JsonSerializer.Serialize(keys));
		Assert.NotNull(keys);

	}
}
