using System;
using Basic.Properties;

namespace Basic.Designer
{
	/// <summary>
	/// 指定当属性或事件显示在一个设置为“按分类顺序”模式的 System.Windows.Forms.PropertyGrid 控件中时，
	/// 用于给属性或事件分组的类别的名称。
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class PersistentCategoryAttribute : System.ComponentModel.CategoryAttribute
	{
		public const string CategoryCodeGenerator = "PersistentCategory_CodeGenerator";
		public const string CategoryContent = "PersistentCategory_Content";
		public const string CategoryDataCommand = "PersistentCategory_DataCommand";
		public const string CategoryDynamicCommand = "PersistentCategory_DynamicCommand";
		public const string CategoryCollections = "PersistentCategory_Collections";
		public const string CategoryNewCommand = "PersistentCategory_NewCommands";
		public const string CategoryCheckCommand = "PersistentCategory_CheckCommands";
		public const string CategoryColumn = "PersistentCategory_Column";
		public const string CategoryProperty = "PersistentCategory_Property";
		public const string CategoryParameter = "PersistentCategory_Parameter";
		public const string CategoryCondition = "PersistentCategory_DataCondition";

		/// <summary>
		/// 使用指定的类别名称初始化 System.ComponentModel.CategoryAttribute 类的新实例。
		/// </summary>
		/// <param name="category">类别名称。</param>
		public PersistentCategoryAttribute(string category) : base(category) { }

		/// <summary>
		/// 查阅指定类别的本地化名称。
		/// </summary>
		/// <param name="value">要查阅的类别的标识符。</param>
		/// <returns>类别的本地化名称；如果本地化名称不存在，则为 null。</returns>
		protected override string GetLocalizedString(string value)
		{
			return StringUtils.GetString(value);
		}

		private static volatile PersistentCategoryAttribute content;
		private static volatile PersistentCategoryAttribute dataCommand;


		// Properties
		/// <summary>
		/// 获取表示“Content”类别的 Basic.Designer.PersistentCategoryAttribute。
		/// </summary>
		public static PersistentCategoryAttribute Content
		{
			get
			{
				if (content == null)
					content = new PersistentCategoryAttribute(CategoryContent);
				return content;
			}
		}
		/// <summary>
		/// 获取表示“DataCommand”类别的 Basic.Designer.PersistentCategoryAttribute。
		/// </summary>
		public static PersistentCategoryAttribute DataCommand
		{
			get
			{
				if (dataCommand == null)
					dataCommand = new PersistentCategoryAttribute(CategoryDataCommand);
				return dataCommand;
			}
		}

	}
}
