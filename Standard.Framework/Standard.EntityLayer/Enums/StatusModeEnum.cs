namespace Basic.Enums
{
	/// <summary>
	/// 页面状态枚举
	/// </summary>
	[System.FlagsAttribute()]
	public enum StatusModeEnum
	{
		/// <summary>
		/// 当前窗口正在执行新增操作，一般为开始新增后标记。
		/// </summary>
		None ,
		/// <summary>
		/// 当前窗口正在执行新增操作，一般为开始新增后标记。
		/// </summary>
		AddNewing ,
		/// <summary>
		/// 当前窗口已经执行完新增操作。
		/// </summary>
		AddNewed,

		/// <summary>
		/// 当前窗口正在执行更新操作，一般为开始更新后标记。
		/// </summary>
		Updating,
		/// <summary>
		/// 当前窗口已经执行完更新操作。
		/// </summary>
		Updated,

		/// <summary>
		/// 当前窗口正在执行删除操作，一般为开始删除后标记。
		/// </summary>
		Deleting,
		/// <summary>
		/// 当前窗口已经执行完删除操作。
		/// </summary>
		Deleted,

		/// <summary>
		/// 当前窗口正在执行查询操作，一般为开始查询后标记。
		/// </summary>
		Searching,
		/// <summary>
		/// 当前窗口已经执行完查询操作。
		/// </summary>
		Searched,

		/// <summary>
		/// 当前窗口正在执行导入操作，一般为开始导入后标记。
		/// </summary>
		Importing,
		/// <summary>
		/// 当前窗口已经执行完导入操作。
		/// </summary>
		Imported,

		/// <summary>
		/// 当前窗口正在执行导出操作，一般为开始导出增后标记。
		/// </summary>
		Exporting,
		/// <summary>
		/// 当前窗口已经执行完导出操作。
		/// </summary>
		Exported,

		/// <summary>
		/// 当前窗口正在执行确认操作，一般为开始确认后标记。
		/// </summary>
		Confirming,
		/// <summary>
		/// 当前窗口已经执行完确认操作。
		/// </summary>
		Confirmed,

		/// <summary>
		/// 当前窗口正在执行保存操作，一般为开始保存后标记。
		/// </summary>
		Saving,
		/// <summary>
		/// 当前窗口已经执行保存增操作。
		/// </summary>
		Saved,

		/// <summary>
		/// 当前窗口正在执行取消操作，一般为开始取消后标记。
		/// </summary>
		Canceling,
		/// <summary>
		/// 当前窗口已经执行完取消操作。
		/// </summary>
		Canceled,

		/// <summary>
		/// 当前窗口正在执行未知操作，一般为开始未知操作后标记。
		/// </summary>
		UnkwonOperating,

		/// <summary>
		/// 当前窗口已经执行完未知操作。
		/// </summary>
		UnkwonOperated,
	}
}