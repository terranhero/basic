
namespace Basic.Enums
{
	/// <summary>数据库配置文件类型</summary>
	public enum ResxModeEnum
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
		Resource = 2
	}
}
