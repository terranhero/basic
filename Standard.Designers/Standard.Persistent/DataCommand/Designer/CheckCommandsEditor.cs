using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Basic.Configuration;

namespace Basic.Designer
{
	/// <summary>
	/// 表示验证集合编辑器
	/// </summary>
	public class CheckCommandsEditor : CollectionEditor
	{
		/// <summary>
		/// 使用指定的集合类型初始化 ValidationAttributesEditor 类的新实例
		/// </summary>
		public CheckCommandsEditor(Type type) : base(type) { }

		/// <summary>
		/// 获取此集合编辑器可包含的数据类型。
		/// </summary>
		/// <returns>此集合可包含的数据类型数组。</returns>
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(CheckedCommandElement) };
		}

		/// <summary>
		///  创建指定的集合项类型的新实例。
		/// </summary>
		/// <param name="itemType">要创建的项类型。</param>
		/// <returns>指定对象的新实例。</returns>
		protected override object CreateInstance(Type itemType)
		{
			StaticCommandDescriptor entityPropertyDescriptor = base.Context.Instance as StaticCommandDescriptor;
			return TypeDescriptor.CreateInstance(null, itemType, null, new object[] { entityPropertyDescriptor.DefinitionInfo });
		}

		/// <summary>
		/// 指示是否可一次选择多个集合项。
		/// </summary>
		/// <returns></returns>
		protected override bool CanSelectMultipleInstances()
		{
			return false;
		}
	}
}
