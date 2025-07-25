namespace Basic.Enums
{
	/// <summary>
	/// 日志管理类型 
	/// </summary>
	public enum LogSaveType
	{
		/// <summary>
		/// 忽略日志信息
		/// </summary>
		None = 0,

		/// <summary>
		/// EventLog
		/// </summary>
		Windows = 1,

		/// <summary>
		/// 本地文件 
		/// </summary>
		LocalFile = 2,

		/// <summary>
		/// 数据库
		/// </summary>
		DataBase = 4,

		/// <summary>
		/// 控制台
		/// </summary>
		Console = 8,
	}
}