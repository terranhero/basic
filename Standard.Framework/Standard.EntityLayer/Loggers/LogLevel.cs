
namespace Basic.Enums
{
	/// <summary>
	/// 日志级别
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		///  信息事件。它指示重要、成功的操作。 
		/// </summary>
		Information = 0x01,

		/// <summary>
		///  警告事件。它指示并不立即具有重要性的问题，但此问题可能表示将来会导致问题的条件。 
		/// </summary>
		Warning = 0x02,

		/// <summary>
		/// 错误事件。它指示用户应该知道的严重问题（通常是功能或数据的丢失）。 
		/// </summary>
		Error = 0x04,

		/// <summary>
		/// 调试事件。
		/// </summary>
		Debug = 0x08
	}
}
