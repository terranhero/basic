
namespace Basic.Interfaces
{
	/// <summary>
	/// 表示动态属性接口定义。
	/// </summary>
	public interface IDynamicProperty
	{
		/// <summary>
		/// 表示属性字段名称
		/// </summary>
		string Key { get; }

		/// <summary>
		/// 字段值
		/// </summary>
		object Value { get; }
	}
}
