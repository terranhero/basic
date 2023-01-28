
using Basic.Properties;
namespace Basic.DataAccess
{
	/// <summary>
	/// 使用指定的描述，解释属性、事件的定义。
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.All)]
	internal class AccessDescriptionAttribute : System.ComponentModel.DescriptionAttribute
	{
		/// <summary>
		/// 初始化 UtilDescriptionAttribute 类的新实例并带有说明。
		/// </summary>
		public AccessDescriptionAttribute(string description)
			: base(description) { }

		/// <summary>
		/// 获取存储在此属性中的说明。
		/// </summary>
		/// <value>存储在此属性中的说明。</value>
		public override string Description
		{
			get
			{
				return Strings.ResourceManager.GetString(base.Description);
			}
		}
	}
}