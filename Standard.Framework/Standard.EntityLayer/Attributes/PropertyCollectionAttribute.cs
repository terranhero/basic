using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 指示实体属性表示所属实体模型中的一个嵌入式属性集合。
	/// 常用于文档数据库、复杂类型映射或动态属性扩展场景。
	/// </summary>
	/// <remarks>
	/// 该特性标记的属性会被视为嵌套的子集合，在序列化、反序列化或 ORM 处理时，
	/// 可能需要将集合元素扁平化为特定前缀的键值对或单独存储。
	/// <para>
	/// 如果使用 <see cref="PropertyCollectionAttribute()"/> 无参构造函数，则采用默认前缀（可能为空或类名）；
	/// 如果使用 <see cref="PropertyCollectionAttribute(string)"/> 并指定 prefix，则所有子项键名将使用该前缀以避免命名冲突。
	/// </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class PropertyCollectionAttribute : Attribute
	{
		private readonly string _prefix;

		/// <summary>
		/// 初始化 <see cref="PropertyCollectionAttribute"/> 类的新实例，不指定前缀（使用默认行为）。
		/// </summary>
		public PropertyCollectionAttribute() { }

		/// <summary>
		/// 初始化 <see cref="PropertyCollectionAttribute"/> 类的新实例，并指定嵌入式集合的键名前缀。
		/// </summary>
		/// <param name="prefix">键名前缀字符串，用于区分不同集合或避免命名冲突。</param>
		public PropertyCollectionAttribute(string prefix) { _prefix = prefix; }

		/// <summary>获取嵌入式集合的键名前缀。</summary>
		/// <value>在构造函数中指定的前缀字符串</value>
		public string Prefix { get { return _prefix; } }
	}
}
