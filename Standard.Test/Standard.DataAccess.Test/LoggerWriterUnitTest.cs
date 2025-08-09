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

		[TestMethod("TestInformationAsync")]
		public async Task TestInformationAsync()
		{
			//ManualResetEvent resetEvent = new ManualResetEvent(false);

			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");

			await writer.InformationAsync("LoggerWriter", "InformationAsync", null, "Visual Studio 2022", "������־����");
			//// �ȴ� BackgroundWorker ��ɣ���ʱ��
			//bool completed = resetEvent.WaitOne(1000 * 10); // ��ָ����ʱʱ�䣬�� 5000ms
		}

		[TestMethod("TestInformationChineseAsync")]
		public async Task TestInformationChineseAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			await writer.InformationAsync("���Կ�����", "���Է������ȶ���", null, "Visual Studio 2022", "������־����");
		}

		[TestMethod("TestErrorAsync")]
		public async Task TestErrorAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			await writer.ErrorAsync("LoggerWriter", "ErrorAsync", null, "Visual Studio 2022", "������־����");
		}

		[TestMethod("TestDebugAsync")]
		public async Task TestDebugAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			await writer.DebugAsync("LoggerWriter", "DebugAsync", null, "Visual Studio 2022", "������־����");
		}

		[TestMethod("TestWarnAsync")]
		public async Task TestWarnAsync()
		{
			ILoggerWriter writer = LoggerWriter.GetWriter("LOCAL");
			for (int index = 0; index <= 2000; index++)
			{
				await writer.WarningAsync("������", "WarningAsync", null, "Visual Studio 2022", "������־����");
				//await writer.WarningAsync("������", "WarningAsync", null, "Visual Studio 2022", "������־����");
			}
			await Task.Delay(3000);
		}
	}
}