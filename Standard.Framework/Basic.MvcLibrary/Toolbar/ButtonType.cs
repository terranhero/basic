namespace Basic.MvcLibrary
{
	/// <summary>
	/// 按钮操作类型
	/// </summary>
	[System.FlagsAttribute()]
	public enum ButtonType
	{
		/// <summary>
		/// 无任何操作。
		/// </summary>
		None = 0,

		/// <summary>
		/// 执行自定义操作。
		/// </summary>
		Custom = 1,

		/// <summary>
		/// 执行新增操作。
		/// </summary>
		Create = 2,

		/// <summary>
		/// 执行更新操作。
		/// </summary>
		Update = 4,

		/// <summary>
		/// 执行删除操作。
		/// </summary>
		Delete = 8,

		/// <summary>
		/// 执行查询操作。
		/// </summary>
		Search = 16,

		/// <summary>
		/// 执行查询操作。
		/// </summary>
		ComplexSearch = 32,

		/// <summary>
		/// 执行导出操作。
		/// </summary>
		Export = 64,

		/// <summary>
		/// 执行导入操作。
		/// </summary>
		Import = 128,

		/// <summary>
		/// 执行保存操作。
		/// </summary>
		Save = 256,

		/// <summary>
		/// 执行打印操作。
		/// </summary>
		Print = 512,

		/// <summary>
		/// 执行导航首页。
		/// </summary>
		PageFirst = 1024,

		/// <summary>
		/// 执行导航上页。
		/// </summary>
		PagePrev = 2048,

		/// <summary>
		/// 执行导航下页。
		/// </summary>
		PageNext = 4096,

		/// <summary>
		/// 执行导航尾页。
		/// </summary>
		PageLast = 8192
	}
}
