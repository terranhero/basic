using System.Collections.Generic;
using Basic.Enums;

namespace Basic.Interfaces
{
	/// <summary>表示数据库连接</summary>
	public interface IConnectionInfo : IDictionary<string, string>
	{
		/// <summary>数据库连接类型</summary>
		ConnectionType ConnectionType { get; set; }

		/// <summary>数据库版本号</summary>
		int Version { get; set; }

		/// <summary>数据库连接名称</summary>
		string Name { get; set; }
	}

	/// <summary>
	/// 默认 IConnectionInfo 接口的实现，用于绑定数据
	/// </summary>
	internal sealed class JsonConnectionInfo : Dictionary<string, string>, IConnectionInfo
	{
		/// <summary>数据库连接类型</summary>
		public ConnectionType ConnectionType { get; set; }

		/// <summary>数据库版本号</summary>
		public int Version { get; set; }

		/// <summary>数据库连接名称</summary>
		public string Name { get; set; }
	}
}
