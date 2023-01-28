using System.Globalization;
using Basic.Localizations;

namespace Basic.Designer
{
	/// <summary>
	///  指定属性或事件的说明。
	/// </summary>
	internal sealed class ResourceDescriptionAttribute : System.ComponentModel.DescriptionAttribute
	{
		/// <summary>
		/// 初始化 PropertyDescription 类的新实例并带有说明。
		/// </summary>
		/// <param name="description">说明文本。</param>
		public ResourceDescriptionAttribute(string description) : base(description) { }

		/// <summary>
		/// 获取存储在此特性中的说明。
		/// </summary>
		/// <value>存储在此特性中的说明。</value>
		public override string Description
		{
			get
			{
				return Strings.ResourceManager.GetString(base.Description, CultureInfo.CurrentUICulture);
			}
		}
	}
}
