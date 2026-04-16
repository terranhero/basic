using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示实体类在调用 ToString(bool) 方法时确认属性是否需要序列化
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IgnorePropertyAttribute : Attribute { }

	/// <summary>
	/// 表示实体类在调用 <see cref="JsonConverter.Serialize{T}(T)"/>方法时确认属性是否需要序列化。
	/// 应用此特性的属性会在生成字符串表示时被有条件地忽略。
	/// </summary>
	/// <remarks>
	/// 使用此特性可以精细控制实体类在转换为字符串时的输出内容，
	/// 例如隐藏敏感信息、排除冗余数据或根据值为 null 的条件动态忽略属性。
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IgnoreSerializeAttribute : Attribute
	{
		/// <summary>
		/// 初始化 <see cref="IgnoreSerializeAttribute"/> 类的新实例。
		/// 默认忽略条件为 <see cref="IgnoreConditions.Always"/>，即始终忽略标记的属性。
		/// </summary>
		/// <remarks>
		/// 该无参构造方法会调用有参构造方法，并传入 <c>IgnoreConditions.Always</c> 作为默认条件。
		/// </remarks>
		public IgnoreSerializeAttribute() : this(IgnoreConditions.Always) { }

		/// <summary>
		/// 使用指定的忽略条件初始化 <see cref="IgnoreSerializeAttribute"/> 类的新实例。
		/// </summary>
		/// <param name="condition">
		/// 指定的忽略条件，决定该属性在何种情况下被忽略。
		/// 可选值包括 <see cref="IgnoreConditions.Always"/> 和 <see cref="IgnoreConditions.WhenIsNull"/>。
		/// </param>
		/// <example>
		/// 以下示例展示了如何在属性上应用此特性：
		/// <code>
		/// [IgnoreSerialize(IgnoreConditions.WhenIsNull)]
		/// public string Description { get; set; }
		/// </code>
		/// </example>
		public IgnoreSerializeAttribute(IgnoreConditions condition) { Condition = condition; }

		/// <summary>
		/// 获取属性被忽略的条件。
		/// </summary>
		/// <value>
		/// <see cref="IgnoreConditions"/> 枚举值，表示当前属性的忽略策略。
		/// </value>
		public IgnoreConditions Condition { get; private set; }
	}

	/// <summary>
	/// 定义属性在序列化过程中被忽略的条件。
	/// </summary>
	/// <remarks>
	/// 该枚举用于 <see cref="IgnorePropertyAttribute"/> 中，提供灵活的忽略策略，
	/// 以便在不同场景下控制属性的序列化行为。
	/// </remarks>
	public enum IgnoreConditions
	{
		/// <summary>
		/// 属性始终被忽略，无论其值是什么。
		/// </summary>
		/// <remarks>
		/// 使用此选项时，该属性不会出现在 ToString(bool) 方法生成的字符串表示中。
		/// 适用于不希望暴露的敏感信息或内部使用的属性。
		/// </remarks>
		Always = 1,

		/// <summary>
		/// 仅当属性的值为 null 时才忽略该属性。
		/// 此条件仅适用于引用类型（类、接口、委托、字符串、数组等）的属性或字段。
		/// </summary>
		/// <remarks>
		/// 使用此选项可以精简输出字符串，避免序列化大量无意义的 null 值属性。
		/// 如果属性有非 null 值（包括空字符串或空集合），则仍会正常序列化。
		/// 对于值类型（如 int、bool、DateTime 等），由于不能为 null，此条件等同于不忽略。
		/// </remarks>
		/// <example>
		/// <code>
		/// public class Product
		/// {
		///     public string Name { get; set; }
		///     
		///     [IgnoreProperty(IgnoreConditions.WhenIsNull)]
		///     public string Description { get; set; }  // 仅在为 null 时忽略
		///     
		///     [IgnoreProperty(IgnoreConditions.WhenIsNull)]
		///     public List&lt;string&gt; Tags { get; set; }  // 仅在为 null 时忽略，空列表不会忽略
		/// }
		/// </code>
		/// </example>
		WhenIsNull = 3,
	}
}

