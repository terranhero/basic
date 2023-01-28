namespace Basic.Enums
{
	/// <summary>
	/// 配置文件类型枚举
	/// </summary>
	public enum ConfigFileType : byte
	{
		/// <summary>
		/// 配置信息没有设置
		/// </summary>
		NotSet = 0,
		/// <summary>
		/// 配置文件信息已资源形式嵌入在程序集中
		/// </summary>
		AssemlyResource = 1,
		/// <summary>
		/// 配置文件信息已资源形式嵌入在资源文件中。
		/// </summary>
		Resource = 2,
		/// <summary>
		/// 配置文件以本地文件形式保存在硬盘中
		/// </summary>
		LocalFile = 4,
		/// <summary>
		/// 配置文件信息保存于数据库中
		/// </summary>
		DataBase = 8,
		///// <summary>
		///// 配置文件信息保存于数据库中(且使用服务器进程保存缓存数据)
		///// </summary>
		//DataBaseCache = 4
	}
}