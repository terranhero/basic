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
			Assert.IsTrue(true, "ClassInitialize ��ʼ��");
			LoggerOptions.Default.Debug.Enabled = true;
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}

		[TestMethod("��־д�����")]
		public async Task LoggerWriteTest()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("HRMS");
			await writer.DebugAsync("LoggerWriterUnitTest", "LoggerWriteTest", null, "Visual Studio 2022", "������־����");
			//System.Threading.
		}
	}
}