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
			Assert.IsTrue(true, "ClassInitialize ��ʼ��");
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}

		[TestMethod("��־д�����")]
		public void LoggerWriteTest()
		{
			//System.Threading.
		}
	}
}