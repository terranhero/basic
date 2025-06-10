using System;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Loggers;

namespace Basic.Interfaces
{
	/// <summary>日志写入接口</summary>
	public interface ILoggerWriter
	{
		/// <summary>根据条件查询日志记录</summary>
		/// <param name="condition">日志查询条件</param>
		/// <returns>返回日志查询结果</returns>
		Task<IPagination<LoggerEntity>> GetLoggingsAsync(LoggerCondition condition);

		/// <summary>根据条件查询日志记录</summary>
		/// <param name="batchNo">日志批次号</param>
		/// <returns>返回日志查询结果</returns>
		Task<IPagination<LoggerEntity>> GetLoggingsAsync(Guid batchNo);

		/// <summary>根据条件删除日志记录</summary>
		/// <param name="keys">需要删除的日志主键</param>
		/// <returns>返回日志查询结果</returns>
		Task<Result> DeleteAsync(Guid[] keys);

		#region 消息日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(Guid batchNo, string controller, string action, string host, string user, string message);
		#endregion

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

		#region 警告日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task WarningAsync(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task WarningAsync(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task WarningAsync(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task WarningAsync(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task WarningAsync(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task WarningAsync(Guid batchNo, string controller, string action, string host, string user, string message);
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

		#region 调试日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task DebugAsync(string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task DebugAsync(Guid batchNo, string url, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task DebugAsync(string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task DebugAsync(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task DebugAsync(string controller, string action, string host, string user, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task DebugAsync(Guid batchNo, string controller, string action, string host, string user, string message);
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

		#region 错误日志记录 - 异步
		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(string url, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(string url, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(string controller, string action, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(Guid batchNo, string url, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(Guid batchNo, string url, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(Guid batchNo, string controller, string action, string host, string user, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(string url, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(string url, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(string controller, string action, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(Guid batchNo, string url, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(Guid batchNo, string url, string host, string user, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(Guid batchNo, string controller, string action, string host, string user, Exception ex);
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

	/// <summary>将日志写入本地文件中</summary>
	public interface IFileLoggerWriter : ILoggerWriter { }

	/// <summary>将日志写入数据库中</summary>
	public interface IDbLoggerWriter : ILoggerWriter { }

	///// <summary>允许写入文件日志</summary>
	//internal sealed class FileLoggerWriter : LoggerWriter, IFileLoggerWriter
	//{
	//	/// <summary>初始化 LoggerWriter 类实例</summary>
	//	internal FileLoggerWriter() : base(_FileStorage) { }
	//}

}
