using System;
using System.Drawing;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.Loggers
{
	/// <summary>日志记录数据库写入类</summary>
	internal sealed class DefaultLoggerWriter : LoggerWriter, IDbLoggerWriter
	{
		/// <summary>初始化 DbLoggerWriter 类实例</summary>
		internal DefaultLoggerWriter(string connection) : base(connection) { }

		/// <summary>初始化 DbLoggerWriter 类实例</summary>
		internal DefaultLoggerWriter(IUserContext ctx) : base(ctx) { }

		/// <summary>根据条件查询日志记录</summary>
		/// <param name="con">日志查询条件</param>
		/// <returns>返回日志查询结果</returns>
		public override async Task<IPagination<LoggerEntity>> GetLoggingsAsync(LoggerCondition con)
		{
			return await _storage.GetLoggingsAsync(con);
		}

		/// <summary>根据条件查询日志记录</summary>
		/// <param name="batchNo">日志批次号</param>
		/// <returns>返回日志查询结果</returns>
		public override async Task<IPagination<LoggerEntity>> GetLoggingsAsync(Guid batchNo)
		{
			return await _storage.GetLoggingsAsync(new LoggerCondition() { BatchNo = batchNo });
		}

		/// <summary>根据条件删除日志记录</summary>
		/// <param name="keys">需要删除的日志主键</param>
		/// <returns>返回日志查询结果</returns>
		public override async Task<Result> DeleteAsync(Guid[] keys)
		{
			return await _storage.DeleteAsync(keys);
		}
	}
}
