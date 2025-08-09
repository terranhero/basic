using System;
using System.Threading;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Loggers;

namespace Basic.Interfaces
{
	/// <summary>日志写入接口</summary>
	public interface ILoggerWriter
	{
		/// <summary>返回当前日志写入器对应的数据库存储库</summary>
		IDataBaseStorage Storage { get; }

#if NET6_0_OR_GREATER
		#region 消息日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask InformationAsync(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask InformationAsync(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask InformationAsync(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask InformationAsync(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask InformationAsync(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask InformationAsync(Guid batchNo, string controller, string action, string host, string user, string message);
		#endregion

		#region 警告日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask WarningAsync(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask WarningAsync(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask WarningAsync(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask WarningAsync(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask WarningAsync(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask WarningAsync(Guid batchNo, string controller, string action, string host, string user, string message);
		#endregion

		#region 调试日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask DebugAsync(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask DebugAsync(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask DebugAsync(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask DebugAsync(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask DebugAsync(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask DebugAsync(Guid batchNo, string controller, string action, string host, string user, string message);
		#endregion

		#region 错误日志记录 - 异步
		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ErrorAsync(string url, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ErrorAsync(string url, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ErrorAsync(string controller, string action, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ErrorAsync(Guid batchNo, string url, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ErrorAsync(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ErrorAsync(Guid batchNo, string controller, string action, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ErrorAsync(string url, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ErrorAsync(string url, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ErrorAsync(string controller, string action, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ErrorAsync(Guid batchNo, string url, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ErrorAsync(Guid batchNo, string url, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ErrorAsync(Guid batchNo, string controller, string action, string host, string user, Exception ex);
		#endregion
#endif

		#region 消息日志事件 - 同步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(Guid batchNo, string controller, string action, string host, string user, string message);
		#endregion

		#region 警告日志事件 - 同步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(Guid batchNo, string controller, string action, string host, string user, string message);
		#endregion

		#region 调试日志事件 - 同步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(Guid batchNo, string controller, string action, string host, string user, string message);
		#endregion

		#region 错误日志记录 - 同步
		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(string url, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(Guid batchNo, string url, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(string url, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(string controller, string action, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(Guid batchNo, string controller, string action, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(string url, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(Guid batchNo, string url, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(string url, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(Guid batchNo, string url, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(string controller, string action, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(Guid batchNo, string controller, string action, string host, string user, Exception ex);
		#endregion
	}

	///// <summary>将日志写入本地文件中</summary>
	//public interface IFileLoggerWriter : ILoggerWriter { }

	///// <summary>将日志写入数据库中</summary>
	//public interface IDbLoggerWriter : ILoggerWriter { }

	///// <summary>允许写入文件日志</summary>
	//internal sealed class FileLoggerWriter : LoggerWriter, IFileLoggerWriter
	//{
	//	/// <summary>初始化 LoggerWriter 类实例</summary>
	//	internal FileLoggerWriter() : base(_FileStorage) { }
	//}

}
