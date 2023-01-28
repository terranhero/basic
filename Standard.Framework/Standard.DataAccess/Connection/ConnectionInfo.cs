using System;
using Basic.Enums;

namespace Basic.Configuration
{
	/// <summary>表示数据库连接</summary>
	public sealed class ConnectionInfo : IComparable<ConnectionInfo>, IEquatable<ConnectionInfo>
	{
		/// <summary>初始化 ConnectionInfo 类实例。</summary>
		/// <param name="name">数据库连接名称</param>
		/// <param name="type">数据库连接类型</param>
		/// <param name="version">数据库版本号</param>
		/// <param name="connectionString">数据库连接字符串</param>
		/// <param name="display">当前连接字符串显示值</param>
		public ConnectionInfo(string name, ConnectionType type, int version, string connectionString, string display)
		{
			ConnectionType = type; Name = name; Version = version;
			ConnectionString = connectionString; DisplayName = string.Concat(name, ";", display);
		}

		/// <summary>初始化 ConnectionInfo 类实例。</summary>
		/// <param name="name">数据库连接名称</param>
		/// <param name="type">数据库连接类型</param>
		/// <param name="connectionString">数据库连接字符串</param>
		/// <param name="display">当前连接字符串显示值</param>
		public ConnectionInfo(string name, ConnectionType type, string connectionString, string display)
		{
			ConnectionType = type; Name = name; Version = 10;/*2008*/
			ConnectionString = connectionString; DisplayName = string.Concat(name, ";", display);
		}

		/// <summary>
		/// 数据库连接类型
		/// </summary>
		public ConnectionType ConnectionType { get; private set; }

		/// <summary>数据库版本号</summary>
		public int Version { get; private set; }

		/// <summary>数据库连接名称</summary>
		public string Name { get; private set; }

		/// <summary>当前数据库连接的显示名称</summary>
		public string DisplayName { get; private set; }

		/// <summary>数据库连接字符串</summary>
		public string ConnectionString { get; private set; }

		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns> 一个值，指示要比较的对象的相对顺序。 
		/// 返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 
		/// 零 此对象等于 other。 
		/// 大于零此对象大于 other。</returns>
		int IComparable<ConnectionInfo>.CompareTo(ConnectionInfo other)
		{
			if (Name == null && other.Name == null) { return 0; }
			return Name.CompareTo(other.Name);
		}

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 other 参数，则为 true；否则为 false。</returns>
		bool IEquatable<ConnectionInfo>.Equals(ConnectionInfo other)
		{
			return Name == other.Name;
		}
	}
}
