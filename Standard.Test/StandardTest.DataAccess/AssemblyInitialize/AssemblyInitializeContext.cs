using Basic.Configuration;
using Basic.DataAccess;
using Microsoft.Extensions.Configuration;

namespace StandardTest.DataAccess
{
	// 定义测试集合，关联全局 Fixture
	[CollectionDefinition("AssemblyInitializeCollection")]
	public class AssemblyInitializeCollection : ICollectionFixture<AssemblyInitializeContext>
	{
		// 这个类不需要实现任何逻辑，仅作为标记类，用于关联集合和 Fixture
	}
	// 全局 Fixture 类：承载所有测试的全局初始化/清理逻辑
	public class AssemblyInitializeContext : IDisposable
	{
		// 全局初始化：构造函数（所有测试执行前，只执行一次）
		public AssemblyInitializeContext()
		{
			// 在这里编写你的全局初始化逻辑，例如：
			// 1. 启动数据库服务、Redis 服务
			// 2. 加载配置文件、初始化全局常量
			// 3. 创建测试用的全局资源（如临时文件夹、全局 HttpClient 实例）
			string path = @"D:\BASIC\PD_04_Gitee Code\Standard.Test\StandardTest.DataAccess";
			IConfigurationBuilder configBuilder = new ConfigurationBuilder().SetBasePath(path);
			IConfigurationRoot root = configBuilder.AddJsonFile("databases.json", true, true).Build();
			IConfigurationSection connections = root.GetSection("Connections");
			ConnectionContext.InitializeConnections(connections);
			SqlServerRegister.RegisterSqlServerAccess();
			SqlServerRegister.RegisterSqlServer2012Access();
		}

		// 全局清理：IDisposable 的 Dispose 方法（所有测试执行完成后，只执行一次）
		public void Dispose()
		{
			// 在这里编写你的全局清理逻辑，例如：
			// 1. 关闭数据库连接、停止服务
			// 2. 删除临时文件、释放全局资源
			// 3. 清理测试产生的脏数据
		}
	}
}
