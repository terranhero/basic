using System.Threading.Tasks;
using Basic.Caches;

namespace Standard.CacheClients;

[TestClass()]
public class CacheClientTest
{
	[TestMethod()]
	public async Task TestMethod1()
	{
		ICacheClient cache = CacheClientFactory.GetClient("HRMS");
		EmployeeAccountInfo user = await cache.HashGetAsync<EmployeeAccountInfo>("HASH_SYS_LOGINUSER_USERKEY", "");
	}
}
