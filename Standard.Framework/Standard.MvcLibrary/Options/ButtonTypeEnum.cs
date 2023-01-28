namespace Basic.EasyLibrary
{
	/// <summary>
	/// 窗口操作模式枚举
	/// </summary>
	[System.FlagsAttribute()]
	public enum ButtonTypeEnum
	{
		/// <summary>
		/// 无任何操作。
		/// </summary>
		None = 0,

		/// <summary>
		/// 当前窗口执行自定义操作。
		/// </summary>
		Custom = 1,

		/// <summary>
		/// 当前窗口执行新增操作。
		/// </summary>
		Create = 2,

		/// <summary>
		/// 当前窗口执行更新操作。
		/// </summary>
		Update = 4,

		/// <summary>
		/// 当前窗口执行删除操作。
		/// </summary>
		Delete = 8,

		/// <summary>
		/// 当前窗口执行查询操作。
		/// </summary>
		Search = 16,

		/// <summary>
		/// 当前窗口执行查询操作。
		/// </summary>
		ComplexSearch = 32,

		/// <summary>
		/// 当前窗口执行导出操作。
		/// </summary>
		Export = 64,

		/// <summary>
		/// 当前窗口执行导入操作。
		/// </summary>
		Import = 128,

		/// <summary>
		/// 当前窗口执行保存操作。
		/// </summary>
		Save = 256,

		/// <summary>
		/// 当前窗口执行打印操作。
		/// </summary>
		Print = 512,
	}
}
