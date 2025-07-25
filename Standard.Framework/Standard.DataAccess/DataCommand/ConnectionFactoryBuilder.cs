﻿using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Configuration;
using Basic.Enums;
using Basic.Exceptions;
using Basic.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Basic.DataAccess
{
	/// <summary>
	/// 数据持久类工厂构造器，
	/// 基于不同的数据库构建不同的持久类工厂
	/// </summary>
	public static class ConnectionFactoryBuilder
	{
		private static DefaultConnectionFactory _defaultFactory = new DefaultConnectionFactory();
		///// <summary>当前系统框架支持的所有 ConnectionFactory 类实例集合。</summary>
		//private static readonly ConnectionFactoryCollection _ConnectionFactorys;
		/// <summary>当前系统框架支持的所有 ConnectionFactory 类实例集合。</summary>
		private static readonly SortedList<ConnectionType, ConnectionFactory> _ConnectionFactorys;
		static ConnectionFactoryBuilder()
		{
			_ConnectionFactorys = new SortedList<ConnectionType, ConnectionFactory>(8);
			_ConnectionFactorys.Add(ConnectionType.SqlConnection, new DefaultConnectionFactory());
		}

		/// <summary>根据数据库连接信息，构建 ConnectionInfo 对象。</summary>
		/// <param name="info">数据库连接配置信息</param>
		/// <returns>返回构建完成的 ConnectionInfo 对象。</returns>
		public static ConnectionInfo CreateConnectionInfo(IConnectionInfo info)
		{
			if (_ConnectionFactorys.TryGetValue(info.ConnectionType, out ConnectionFactory factory))
			{
				return factory.CreateConnectionInfo(info);
			}
			return _defaultFactory.CreateConnectionInfo(info);
		}

		/// <summary>根据数据库连接信息，构建 ConnectionInfo 对象。</summary>
		/// <param name="info">数据库连接配置信息</param>
		/// <returns>返回构建完成的 ConnectionInfo 对象。</returns>
		public static ConnectionInfo CreateConnectionInfo(ConnectionElement info)
		{
			if (_ConnectionFactorys.TryGetValue(info.ConnectionType, out ConnectionFactory factory))
			{
				return factory.CreateConnectionInfo(info);
			}
			return _defaultFactory.CreateConnectionInfo(info);
		}

		/// <summary>获取系统中已经注册的数据库类型</summary>
		public static ConnectionType[] GetRegisterConnections()
		{
			return _ConnectionFactorys.Keys.ToArray();
		}

		/// <summary>
		/// 注册登记一个表示特定数据库的 ConnectionFactory 类实例。
		/// </summary>
		/// <param name="connectionType">数据库连接类型</param>
		/// <param name="factory">表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例</param>
		public static void RegisterConnectionFactory(ConnectionType connectionType, ConnectionFactory factory)
		{
			if (factory == null) { throw new ArgumentNullException("factory"); }
			if (_ConnectionFactorys.ContainsKey(connectionType))
				_ConnectionFactorys[connectionType] = factory;
			else
				_ConnectionFactorys.Add(connectionType, factory);
		}

		/// <summary>
		/// 注册登记一个表示特定数据库的 ConnectionFactory 类实例。
		/// </summary>
		/// <param name="info">数据库连接类型</param>
		/// <param name="factory">表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例</param>
		private static void RegisterConnectionFactory(ConnectionInfo info, ConnectionFactory factory)
		{
			//if (factory == null) { throw new ArgumentNullException("factory"); }
			//if (_ConnectionFactorys.ContainsKey(info.ConnectionType, info.Version))
			//	_ConnectionFactorys[info.ConnectionType, info.Version] = factory;
			//else
			//	_ConnectionFactorys.Add(info.ConnectionType, info.Version, factory);
		}

		/// <summary>
		/// 注册登记一个表示特定数据库的 ConnectionFactory 类实例。
		/// </summary>
		/// <param name="connectionType">数据库连接类型</param>
		/// <param name="ver"></param>
		/// <param name="factory">表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例</param>
		private static void RegisterConnectionFactory(ConnectionType connectionType, int ver, ConnectionFactory factory)
		{
			//if (factory == null) { throw new ArgumentNullException("factory"); }
			//if (_ConnectionFactorys.ContainsKey(connectionType, ver))
			//	_ConnectionFactorys[connectionType, ver] = factory;
			//else
			//	_ConnectionFactorys.Add(connectionType, ver, factory);
		}

		/// <summary>
		/// 获取主服务器操作类实例
		/// </summary>
		internal static ConnectionFactory CreateConnectionFactory()
		{
			return CreateConnectionFactory(ConnectionContext.DefaultConnection);
		}

		/// <summary>
		/// 获取主服务器操作类实例
		/// </summary>
		/// <param name="connectionName">当前数据库连接名称，对应系统配置中的数据库连接。</param>
		internal static ConnectionFactory CreateConnectionFactory(string connectionName)
		{
			if (string.IsNullOrEmpty(connectionName)) { return CreateConnectionFactory(ConnectionContext.DefaultConnection); }
			if (ConnectionContext.TryGetConnection(connectionName, out ConnectionInfo info)) { return CreateConnectionFactory(info); }
			return CreateConnectionFactory(ConnectionContext.DefaultConnection);
		}

		/// <summary>
		/// 创建一个表示特定数据库的 ConnectionFactory 类实例。
		/// </summary>
		/// <returns>返回特定数据库的 ConnectionFactory 类实例。</returns>
		internal static ConnectionFactory CreateConnectionFactory(ConnectionInfo info)
		{
			if (_ConnectionFactorys.TryGetValue(info.ConnectionType, out ConnectionFactory factory)) { return factory; }

			//if (info.ConnectionType == ConnectionType.SqlConnection && info.Version <= 10)
			//	factory = new SqlServer.SqlConnectionFactory();
			//else if (info.ConnectionType == ConnectionType.SqlConnection && info.Version > 10)
			//	factory = new SqlServer2012.SqlConnectionFactory();
			//_ConnectionFactorys[info.ConnectionType] = factory;
			throw new NotSupportedException(string.Format("不支持此类型{0}的数据库操作", info.ConnectionType));
		}

		///// <summary>
		///// 创建一个表示特定数据库的 ConnectionFactory 类实例。
		///// </summary>
		///// <returns>返回特定数据库的 ConnectionFactory 类实例。</returns>
		//internal static ConnectionFactory CreateConnectionFactory(ConnectionType connectionType)
		//{
		//	ConnectionFactory factory = null;
		//	if (!_ConnectionFactorys.TryGetValue(connectionType, out factory))
		//	{
		//		if (connectionType == ConnectionType.SqlConnection)
		//			factory = new SqlServer.SqlConnectionFactory();
		//		_ConnectionFactorys[connectionType] = factory;
		//	}
		//	return factory;
		//}

		/// <summary>
		/// 获取主服务器操作类实例
		/// </summary>
		internal static ConnectionFactory CreateConnectionFactory(ConfigurationInfo configInfo, Type type)
		{
			if (string.IsNullOrEmpty(configInfo.ConnectionName))
				return CreateConnectionFactory(ConnectionContext.DefaultConnection);
			//类{0}中配置信息错误,名称为{1}的连接配置信息不存在，请检查应用程序配置文件(*.config)。
			if (!ConnectionContext.Contains(configInfo.ConnectionName))
				throw new ConfigurationException("Access_NotExistsConnection", type, configInfo.ConnectionName);
			return CreateConnectionFactory(ConnectionContext.GetConnection(configInfo.ConnectionName));
		}
	}

	/// <summary></summary>
	public struct DataConnection : IEquatable<DataConnection>, IComparer<DataConnection>
	{
		/// <summary>初始化 DataConnection 实例</summary>
		/// <param name="ct"></param>
		public DataConnection(ConnectionType ct) { Type = ct; MinVersion = 0; MaxVersion = int.MaxValue; }

		/// <summary>初始化 DataConnection 实例</summary>
		/// <param name="ct"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public DataConnection(ConnectionType ct, int min, int max) { Type = ct; MinVersion = min; MaxVersion = max; }

		/// <summary></summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(DataConnection other)
		{
			return false;
		}

		/// <summary></summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(DataConnection x, DataConnection y)
		{
			throw new NotImplementedException();
		}

		/// <summary>数据库连接类型</summary>
		public ConnectionType Type { get; private set; }

		/// <summary>支持数据库的最小版本</summary>
		public int MinVersion { get; private set; }

		/// <summary>支持数据库的最大版本</summary>
		public int MaxVersion { get; private set; }
	}

}
