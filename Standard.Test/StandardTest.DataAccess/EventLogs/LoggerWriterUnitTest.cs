using Basic.Configuration;
using Basic.Interfaces;
using Basic.Loggers;
using Xunit.v3.Priority;

namespace StandardTest.DataAccess
{
	[Collection("AssemblyInitializeCollection")]
	[TestCaseOrderer(typeof(PriorityOrderer))]
	public class LoggerWriterUnitTest
	{
		private readonly ITestOutputHelper _output;
		public LoggerWriterUnitTest(ITestOutputHelper output)
		{
			_output = output;
			Assert.True(true, "ClassInitialize 初始化");
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

		[Fact(DisplayName = "GetLoggers"), Priority(0)]
		public void GetLoggers()
		{
			string ddd = ConfigurationAlgorithm.Encryption("sqyBUYx1j1BSkdGPK7RYdNgFAB7Ph2bzevoc4thohpo=");
			_output.WriteLine(ddd);

			//ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			//IPagination<LoggerEntity> logs = await writer.Storage.GetLoggingsAsync(new Guid("EE7F553B-3541-4354-A451-D2E74D9C64CC"));
			//string text = System.Text.Json.JsonSerializer.Serialize(logs);
			//_output.WriteLine(text);
		}

		[Fact(DisplayName = "TestInformationAsync"), Priority(1)]
		public async Task TestInformationAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 1; index <= 5000; index++)
			{
				await writer.InformationAsync("LoggerWriter", "InformationAsync", null, "Visual Studio 2022", "这是日志测试");
			}
			//return Task.Delay(1000);
		}

		[Fact(DisplayName = "TestErrorAsync"), Priority(2)]
		public async Task TestErrorAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 1; index <= 5000; index++)
			{
				await writer.ErrorAsync("LoggerWriter", "ErrorAsync", null, "Visual Studio 2022", "这是日志测试");
			}
			//return Task.Delay(1000);
		}

		[Fact(DisplayName = "TestDebugAsync"), Priority(3)]
		public async Task TestDebugAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 1; index <= 5000; index++)
			{
				await writer.DebugAsync("LoggerWriter", "DebugAsync", null, "Visual Studio 2022", "这是日志测试");
			}
			//await Task.Delay(1000);
		}

		[Fact(DisplayName = "TestWarnAsync"), Priority(4)]
		public async Task TestWarnAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 1; index <= 5000; index++)
			{
				await writer.WarningAsync("LoggerWriter", "WarningAsync", null, "Visual Studio 2022", $"这是日志测试{index}");
			}
		}
		[Fact(DisplayName = "ZWarnAsync,等待日志队列"), Priority(20)]
		public async Task Waiting() { await Task.Delay(10000, TestContext.Current.CancellationToken); }
	}
}