
namespace Basic.Enums
{
	/// <summary>
	/// 表示 Access 基类类型枚举
	/// </summary>
	public enum BaseAccessEnum : byte
	{
		/// <summary>
		/// 表示继承基于数据库操作的 Access 类。
		/// </summary>
		AbstractDbAccess,

		/// <summary>
		/// 表示继承基于数据库表操作的 Access 类(包含标准的CURD操作)。
		/// </summary>
		AbstractAccess
	}
}