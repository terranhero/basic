using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardTest.EntityLayer
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
			Assert.IsTrue(true, "AssemblyInitialize 初始化");
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			Assert.IsTrue(true, "ClassInitialize 初始化");
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}
	}
}
