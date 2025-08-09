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

		[TestMethod("GetLoggers")]
		public async Task GetLoggers()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			IPagination<LoggerEntity> logs = await writer.Storage.GetLoggingsAsync(new Guid("EE7F553B-3541-4354-A451-D2E74D9C64CC"));
			string text = System.Text.Json.JsonSerializer.Serialize(logs);
			Console.WriteLine(text);
		}

		[TestMethod("TestInformationAsync")]
		public async Task TestInformationAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 1; index <= 5000; index++)
			{
				await writer.InformationAsync("LoggerWriter", "InformationAsync", null, "Visual Studio 2022", "������־����");
			}
			//return Task.Delay(1000);
		}

		[TestMethod("TestErrorAsync")]
		public async Task TestErrorAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 1; index <= 5000; index++)
			{
				await writer.ErrorAsync("LoggerWriter", "ErrorAsync", null, "Visual Studio 2022", "������־����");
			}
			//return Task.Delay(1000);
		}

		[TestMethod("TestDebugAsync")]
		public async Task TestDebugAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 1; index <= 5000; index++)
			{
				await writer.DebugAsync("LoggerWriter", "DebugAsync", null, "Visual Studio 2022", "������־����");
			}
			//await Task.Delay(1000);
		}

		[TestMethod("TestWarnAsync")]
		public async Task TestWarnAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 1; index <= 5000; index++)
			{
				await writer.WarningAsync("LoggerWriter", "WarningAsync", null, "Visual Studio 2022", $"������־����{index}");
			}
		}
		[TestMethod("ZWarnAsync,�ȴ���־����")]
		public async Task Waiting() { await Task.Delay(10000); }
	}
}