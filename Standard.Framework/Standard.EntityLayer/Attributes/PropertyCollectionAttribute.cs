using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示实体属性是所属实体模型的嵌入式属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class PropertyCollectionAttribute : Attribute { }
}
