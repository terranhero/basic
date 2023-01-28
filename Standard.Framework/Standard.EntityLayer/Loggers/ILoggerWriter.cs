using System;

namespace Basic.Interfaces
{
	/// <summary>日志写入接口</summary>
	public interface ILoggerWriter
	{
		/// <summary>添加 Action映射。</summary>
		/// <param name="url">表示请求的路径。</param>
		/// <param name="controller">表示当前请求所属控制器、窗体名称</param>
		/// <param name="action">表示当前请求名称</param>
		void AddAction(string url, string controller, string action);

		/// <summary>系统是否已经所有可用请求</summary>
		bool HasActions { get; }

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

		#region 日志警告事件
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

		#region 日志调试事件
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
	}
}
