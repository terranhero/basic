﻿
using System;
using System.Threading;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Loggers;

namespace Basic.Interfaces
{
	/// <summary>
	/// 日志存储接口
	/// </summary>
	public interface ILoggerStorage
	{
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		void WriteLog(System.Guid batchNo, string controllerName, string actionName, string computerName,
			string userName, string message, LogLevel logLevel, LogResult resultType);

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		void WriteLog(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName, System.Exception ex);

		/// <summary>采用异步方式记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		Task WriteAsync(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName,
		  string message, LogLevel logLevel, LogResult resultType);

		/// <summary>采用异步方式记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		Task ErrorAsync(Guid batchNo, string controllerName, string actionName, string computerName, string userName, System.Exception ex);

		/// <summary>根据条件查询日志记录</summary>
		/// <param name="condition">查询条件</param>
		/// <returns>返回日志查询结果</returns>
		Task<IPagination<LoggerEntity>> GetLoggingsAsync(LoggerCondition condition);

		/// <summary>根据条件删除日志记录</summary>
		/// <param name="keys">需要删除的日志主键</param>
		/// <returns>返回日志查询结果</returns>
		Task<Result> DeleteAsync(Guid[] keys);

		/// <summary>根据条件删除日志记录</summary>
		/// <param name="entities">需要删除的日志主键</param>
		/// <returns>返回日志查询结果</returns>
		Task<Result> DeleteAsync(LoggerDelEntity[] entities);

		/// <summary>
		/// 异步清除此流的所有缓冲区，并将任何缓冲数据写入底层设备
		/// </summary>
		/// <returns>表示异步刷新操作的任务</returns>
		Task FlushAsync();

		/// <summary>
		/// 异步清除此流的所有缓冲区，并将任何缓冲数据写入底层设备
		/// </summary>
		/// <param name="cancellationToken">
		/// 用于监视取消请求的 <see cref="System.Threading.CancellationToken">System.Threading.CancellationToken</see>
		/// </param>
		/// <returns>表示异步刷新操作的任务</returns>
		Task FlushAsync(CancellationToken cancellationToken);
	}
}
