using System.Threading.Tasks;
using Basic.Caches;

namespace Standard.CacheClients;

[TestClass()]
public class RedisClientFactoryTest
{
	private readonly RedisClientFactory factory = new RedisClientFactory("127.0.0.1:7016,password=GoldSoft@1220", 0);
	private readonly ICacheClient cache;
	public RedisClientFactoryTest() { cache = factory.CreateClient("HRMS"); }

	[TestMethod()]
	public async Task TestHashGetAllAsync()
	{
		IList<EmployeeAccountInfo> users = await cache.HashGetAllAsync<EmployeeAccountInfo>("HASH_SYS_LOGINUSER_USERKEY");
		foreach (EmployeeAccountInfo user in users)
		{
			Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(user));
		}
		Assert.IsNotNull(users);
	}

	[TestMethod(), DataRow("D951B9A9-C300-458E-8047-813F36C91206")]
	public async Task TestHashGetAsync(string userKey)
	{
		EmployeeAccountInfo users = await cache.HashGetAsync<EmployeeAccountInfo>("HASH_SYS_LOGINUSER_USERKEY", userKey);
		Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(users));
		Assert.IsNotNull(users);

	}
}
