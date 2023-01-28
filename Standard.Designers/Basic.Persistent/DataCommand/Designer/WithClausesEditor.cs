using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using Basic.Designer;

namespace Basic.Designer
{
	/// <summary>
	/// 表示验证集合编辑器
	/// </summary>
	public class WithClausesEditor : CollectionEditor
	{
		/// <summary>
		/// 使用指定的集合类型初始化 WithClausesEditor 类的新实例
		/// </summary>
		public WithClausesEditor(Type type) : base(type) { }

		/// <summary>
		/// 获取此集合编辑器可包含的数据类型。
		/// </summary>
		/// <returns>此集合可包含的数据类型数组。</returns>
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(WithClause) };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemType"></param>
		/// <returns></returns>
		protected override object CreateInstance(Type itemType)
		{
			//IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			DynamicCommandDescriptor descriptor = base.Context.Instance as DynamicCommandDescriptor;
			return TypeDescriptor.CreateInstance(null, itemType, null, new object[] { descriptor.DefinitionInfo });
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
