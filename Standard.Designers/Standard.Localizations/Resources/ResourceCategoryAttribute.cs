using System;
using Basic.Properties;
using System.Globalization;

namespace Basic.Designer
{
	/// <summary>
	/// 指定当属性或事件显示在一个设置为“按分类顺序”模式的 System.Windows.Forms.PropertyGrid 控件中时，
	/// 用于给属性或事件分组的类别的名称。
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ResourceCategoryAttribute : System.ComponentModel.CategoryAttribute
	{
		/// <summary>
		/// 使用指定的类别名称初始化 System.ComponentModel.CategoryAttribute 类的新实例。
		/// </summary>
		/// <param name="category">类别名称。</param>
		public ResourceCategoryAttribute(string category) : base(category) { }

		/// <summary>
		/// 查阅指定类别的本地化名称。
		/// </summary>
		/// <param name="value">要查阅的类别的标识符。</param>
		/// <returns>类别的本地化名称；如果本地化名称不存在，则为 null。</returns>
		protected override string GetLocalizedString(string value)
		{
			return DesignerStrings.GetString(value, CultureInfo.CurrentUICulture);
		}
	}
}
