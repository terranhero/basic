using System.Collections.Generic;
using Basic.Enums;

namespace Basic.Interfaces
{
	/// <summary>表示数据库连接</summary>
	public interface IConnectionInfo : IReadOnlyDictionary<string, string>
	{
		/// <summary>数据库连接类型</summary>
		ConnectionType ConnectionType { get; set; }

		/// <summary>数据库版本号</summary>
		int Version { get; set; }

		/// <summary>数据库连接名称</summary>
		string Name { get; set; }

		/// <summary>是否有效</summary>
		bool Enabled { get; set; }
	}
}
