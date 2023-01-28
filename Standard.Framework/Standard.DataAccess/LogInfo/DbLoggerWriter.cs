using Basic.Interfaces;

namespace Basic.LogInfo
{
	/// <summary>日志记录数据库写入类</summary>
	public class DbLoggerWriter : LoggerWriter
	{
		/// <summary>初始化 DbLoggerWriter 类实例</summary>
		internal DbLoggerWriter() : base(new EventLogContext(_EventLogs)) { }

		/// <summary>初始化 DbLoggerWriter 类实例</summary>
		public DbLoggerWriter(string connection) : base(new EventLogContext(connection, _EventLogs)) { }

		/// <summary>初始化 DbLoggerWriter 类实例</summary>
		public DbLoggerWriter(IUserContext ctx) : base(new EventLogContext(ctx, _EventLogs)) { }
	}
}
