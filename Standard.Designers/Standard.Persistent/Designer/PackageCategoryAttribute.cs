using System;
using Basic.Properties;

namespace Basic.Designer
{
	/// <summary>
	/// 指定当属性或事件显示在一个设置为“按分类顺序”模式的 System.Windows.Forms.PropertyGrid 控件中时，
	/// 用于给属性或事件分组的类别的名称。
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class PackageCategoryAttribute : System.ComponentModel.CategoryAttribute
	{
		internal const string CategoryCodeGenerator = "PersistentCategory_CodeGenerator";
		internal const string CategoryContent = "PersistentCategory_Content";
		internal const string CategoryDataCommand = "PersistentCategory_DataCommand";
		internal const string CategoryDynamicCommand = "PersistentCategory_DynamicCommand";
		internal const string CategoryCollections = "PersistentCategory_Collections";
		internal const string CategoryNewCommand = "PersistentCategory_NewCommands";
		internal const string CategoryCheckCommand = "PersistentCategory_CheckCommands";
		internal const string CategoryColumn = "PersistentCategory_Column";
		internal const string CategoryProperty = "PersistentCategory_Property";
		internal const string CategoryParameter = "PersistentCategory_Parameter";
		internal const string CategoryCondition = "PersistentCategory_DataCondition";

		/// <summary>
		/// 使用指定的类别名称初始化 System.ComponentModel.CategoryAttribute 类的新实例。
		/// </summary>
		/// <param name="category">类别名称。</param>
		public PackageCategoryAttribute(string category) : base(category) { }

		/// <summary>
		/// 查阅指定类别的本地化名称。
		/// </summary>
		/// <param name="value">要查阅的类别的标识符。</param>
		/// <returns>类别的本地化名称；如果本地化名称不存在，则为 null。</returns>
		protected override string GetLocalizedString(string value)
		{
			return DesignerStrings.ResourceManager.GetString(value);
		}
	}
}
