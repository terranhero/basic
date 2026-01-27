using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.Configuration;
using Basic.DataAccess;
using Microsoft.Extensions.Configuration;

namespace Standard.EntityLayer.Test
{
	[TestClass]
	public class AssemblyInitializeContextCtor
	{
		private readonly TestContext _testContext;

		public AssemblyInitializeContextCtor(TestContext testContext)
		{
			_testContext = testContext;
		}

		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			string path = @"D:\BASIC\PD_04_Gitee Code\Standard.Test\StandardTest.DataAccess";
			IConfigurationBuilder configBuilder = new ConfigurationBuilder().SetBasePath(path);
			IConfigurationRoot root = configBuilder.AddJsonFile("databases.json", true, true).Build();
			IConfigurationSection connections = root.GetSection("Connections");
			ConnectionContext.InitializeConnections(connections);
			SqlServerRegister.RegisterSqlServerAccess();
			SqlServerRegister.RegisterSqlServer2012Access();
			Assert.IsTrue(true, "AssemblyInitialize 初始化");
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}
	}
}
