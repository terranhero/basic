using Basic.LogInfo;

namespace Standard.DataAccess.Test
{
	[TestClass]
	public class LoggerWriterUnitTest
	{
		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			LoggerWriterFactory.InitializeOptions(opts =>
			{
				opts.Interval = TimeSpan.FromSeconds(30);
			});
			Assert.IsTrue(true, "ClassInitialize 初始化");
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}

		[TestMethod("日志写入测试")]
		public void LoggerWriteTest()
		{
			//System.Threading.
		}
	}
}