using System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 自定义多语言配置信息
	/// </summary>
	public sealed class EasyLibrarySection : ConfigurationSection
	{
		/// <summary>
		/// EasyUI配置信息
		/// </summary>
		public const string ElementName = "basic.easyLibrary";

		/// <summary>
		/// 初始化 EasyLibrarySection 类实例
		/// </summary>
		public EasyLibrarySection() { }

		/// <summary>
		///  指示自上次在派生类中实现此配置元素时保存或加载以来是否对其进行过修改。
		/// </summary>
		/// <returns>如果元素已修改，则为 true；否则为 false。</returns>
		public bool Modified { get { return base.IsModified(); } }

		/// <summary>
		/// 数据库连接配置信息
		/// </summary>
		[ConfigurationProperty("easyGrid", IsRequired = false)]
		public EasyGridElement EasyGrid
		{
			get
			{
				return (EasyGridElement)base["easyGrid"];
			}
		}
	}

}
