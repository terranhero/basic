using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.Configuration;
using Basic.DataAccess;

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
			ConnectionContext.InitializeConfiguration(@"D:\BASIC\PD_04_Gitee Code\database.Test.config");
			SqlServerRegister.RegisterSqlServer2012Access();
			Assert.IsTrue(true, "AssemblyInitialize 初始化");
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}
	}
}
