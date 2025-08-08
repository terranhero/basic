using System.Threading.Tasks;
using Basic.Interfaces;
using Basic.Loggers;

namespace Standard.DataAccess.Test
{
	[TestClass]
	public class LoggerWriterUnitTest
	{
		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			Assert.IsTrue(true, "ClassInitialize 初始化");
			LoggerOptions.Default.Debug.Enabled = true;
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}

		[TestMethod("日志写入测试")]
		public async Task LoggerWriteTest()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("HRMS");
			await writer.DebugAsync("LoggerWriterUnitTest", "LoggerWriteTest", null, "Visual Studio 2022", "这是日志测试");
			//System.Threading.
		}
	}
}