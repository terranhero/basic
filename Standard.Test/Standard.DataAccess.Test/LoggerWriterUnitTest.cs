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
			LoggerWriter.ConfigureOptions(opts =>
			{
				opts.Information.Enabled = true;
				opts.Information.SaveType = Basic.Enums.LogSaveType.DataBase;
				opts.Warning.Enabled = true;
				opts.Warning.SaveType = Basic.Enums.LogSaveType.DataBase;
				opts.Error.Enabled = true;
				opts.Error.SaveType = Basic.Enums.LogSaveType.DataBase;
				opts.Debug.Enabled = true;
				opts.Debug.SaveType = Basic.Enums.LogSaveType.DataBase;
			});
			// Access TestContext properties and methods here. The properties related to the test run are not available.
		}

		[TestMethod("TestInformationAsync")]
		public async Task TestInformationAsync()
		{
			//ManualResetEvent resetEvent = new ManualResetEvent(false);

			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");

			await writer.InformationAsync("LoggerWriter", "InformationAsync", null, "Visual Studio 2022", "这是日志测试");
			//// 等待 BackgroundWorker 完成（或超时）
			//bool completed = resetEvent.WaitOne(1000 * 10); // 或指定超时时间，如 5000ms
		}

		[TestMethod("TestInformationChineseAsync")]
		public async Task TestInformationChineseAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			await writer.InformationAsync("测试控制器", "测试方法长度而已", null, "Visual Studio 2022", "这是日志测试");
		}

		[TestMethod("TestErrorAsync")]
		public async Task TestErrorAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			await writer.ErrorAsync("LoggerWriter", "ErrorAsync", null, "Visual Studio 2022", "这是日志测试");
		}

		[TestMethod("TestDebugAsync")]
		public async Task TestDebugAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			await writer.DebugAsync("LoggerWriter", "DebugAsync", null, "Visual Studio 2022", "这是日志测试");
		}

		[TestMethod("TestWarnAsync")]
		public async Task TestWarnAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 0; index <= 2000; index++)
			{
				await writer.WarningAsync("控制器", "WarningAsync", null, "Visual Studio 2022", "这是日志测试");
				//await writer.WarningAsync("控制器", "WarningAsync", null, "Visual Studio 2022", "这是日志测试");
			}
			await Task.Delay(3000);
		}
	}
}