using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Basic.Collections;

using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>
	/// 创建命令类接口
	/// </summary>
	public abstract class CommandBuilder : IDisposable
	{
		///// <summary>
		///// 根据命令名称创建数据库命令
		///// </summary>
		///// <param name="tableCache">当前配置文件的缓存</param>
		//public abstract void CreateDataCommand(DatabaseConfiguration tableCache);

		///// <summary>
		///// 根据命令名称创建数据库命令
		///// </summary>
		///// <param name="dbCommands">当前配置文件的缓存</param>
		///// <param name="configFileName">配置文件名称</param>
		//public abstract void CreateDataCommand(DatabaseCommands dbCommands, string configFileName);

		/// <summary>
		/// 根据命令名称创建数据库命令
		/// </summary>
		/// <param name="tableInfo">当前配置文件的缓存</param>
		internal abstract void CreateDataCommand(TableConfiguration tableInfo);

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		public virtual void Dispose() { }
	}
}
