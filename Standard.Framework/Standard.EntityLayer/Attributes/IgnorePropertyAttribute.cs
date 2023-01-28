using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示实体类在调用 ToString(bool) 方法时确认属性是否需要序列化
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IgnorePropertyAttribute : Attribute { }
}
