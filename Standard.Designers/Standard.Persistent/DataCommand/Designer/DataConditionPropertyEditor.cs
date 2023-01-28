using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using Basic.DataEntities;
using System.ComponentModel;

namespace Basic.Designer
{
	/// <summary>
	/// 表示条件属性编辑器
	/// </summary>
	public sealed class DataConditionPropertyEditor : CollectionEditor
	{
		/// <summary>
		/// 使用指定的集合类型初始化 DataConditionPropertyEditor 类的新实例
		/// </summary>
		public DataConditionPropertyEditor(Type type) : base(type) { }

		/// <summary>
		/// 获取此集合编辑器可包含的数据类型。
		/// </summary>
		/// <returns>此集合可包含的数据类型数组。</returns>
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(DataConditionPropertyElement) };
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
