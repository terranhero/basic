using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.Loggers
{
	/// <summary>日志记录数据库写入类</summary>
	internal sealed class DefaultLoggerWriter : LoggerWriter//, IDbLoggerWriter
	{
		/// <summary>初始化 DbLoggerWriter 类实例</summary>
		internal DefaultLoggerWriter(string key) : base(key) { }

		/// <summary>初始化 DbLoggerWriter 类实例</summary>
		internal DefaultLoggerWriter(IUserContext ctx) : base(ctx) { }
	}
}
