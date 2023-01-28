using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Designer
{
	/// <summary>
	/// 设计文档剪贴板格式。
	/// </summary>
	internal static class ClipboardFormat
	{
		public const int ClipboardFormatLength = 6;
		/// <summary>
		/// 本地化资源剪贴板格式
		/// </summary>
		public const string LocalizationFormat = "0X0101";

		/// <summary>
		/// 数据持久化动态命令剪贴板格式
		/// </summary>
		public const string PersistentDataEntityFormat = "0X0200";

		/// <summary>
		/// 数据持久化属性剪贴板格式
		/// </summary>
		public const string PersistentEntityPropertyFormat = "0X0201";

		/// <summary>
		/// 数据持久化属性剪贴板格式
		/// </summary>
		public const string PersistentConditionPropertyFormat = "0X0202";

		/// <summary>
		/// 数据持久化动态命令剪贴板格式
		/// </summary>
		public const string PersistentStaticCommandFormat = "0X0203";

		/// <summary>
		/// 数据持久化静态命令剪贴板格式
		/// </summary>
		public const string PersistentDynamicCommandFormat = "0X0204";
	}
}
