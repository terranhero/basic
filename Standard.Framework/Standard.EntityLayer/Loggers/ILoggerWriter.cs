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

		/// <summary>添加 Action映射。</summary>
		/// <param name="url">表示请求的路径。</param>
		/// <param name="controller">表示当前请求所属控制器、窗体名称</param>
		/// <param name="action">表示当前请求名称</param>
		void AddAction(string url, string controller, string action);

		/// <summary>系统是否已经所有可用请求</summary>
		bool HasActions { get; }

		#region 日志信息事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(string url, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(Guid batchNo, string url, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(string url, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(Guid batchNo, string url, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(string controller, string action, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task InformationAsync(Guid batchNo, string controller, string action, string host, string userName, string message);
		#endregion

		#region 日志信息事件 - 同步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(string url, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(Guid batchNo, string url, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(string url, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(Guid batchNo, string url, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(string controller, string action, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Information(Guid batchNo, string controller, string action, string host, string userName, string message);
		#endregion

		#region 日志警告事件 - 同步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(string url, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(Guid batchNo, string url, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(string url, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(Guid batchNo, string url, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(string controller, string action, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Warning(Guid batchNo, string controller, string action, string host, string userName, string message);
		#endregion

		#region 日志调试事件 - 同步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(string url, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(Guid batchNo, string url, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(string url, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(Guid batchNo, string url, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(string controller, string action, string host, string userName, string message);

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Debug(Guid batchNo, string controller, string action, string host, string userName, string message);
		#endregion

		#region 错误日志记录 - 异步
		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(string url, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(string url, string host, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(string controller, string action, string host, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(Guid batchNo, string url, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(Guid batchNo, string url, string host, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ErrorAsync(Guid batchNo, string controller, string action, string host, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(string url, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(string url, string host, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(string controller, string action, string host, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(Guid batchNo, string url, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(Guid batchNo, string url, string host, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ErrorAsync(Guid batchNo, string controller, string action, string host, string userName, Exception ex);
		#endregion

		#region 错误日志记录 - 同步
		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(string url, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(Guid batchNo, string url, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(string url, string host, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(Guid batchNo, string url, string host, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(string controller, string action, string host, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		void Error(Guid batchNo, string controller, string action, string host, string userName, string message);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(string url, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(Guid batchNo, string url, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(string url, string host, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(Guid batchNo, string url, string host, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(string controller, string action, string host, string userName, Exception ex);

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		void Error(Guid batchNo, string controller, string action, string host, string userName, Exception ex);
		#endregion
	}
}
