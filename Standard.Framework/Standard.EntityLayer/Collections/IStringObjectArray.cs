
namespace Basic.Collections
{
	/// <summary>
	/// 表示键/值对的集合，提供为对象提供动态自定义类型信息的接口。
	/// </summary>
	public interface IStringObjectArray //: ICustomTypeDescriptor//, IDictionary<string, object>
	{
		/// <summary>
		/// 确定IStringObjectArray是否包含具有指定键的元素。
		/// </summary>
		/// <param name="key">要在IStringObjectArray中定位的键。</param>
		/// <returns>如果IStringObjectArray包含带有该键的元素，则为 true；否则，为 false。</returns>
		bool ContainsKey(string key);

		/// <summary>
		/// 获取或设置具有指定键的元素。
		/// </summary>
		/// <param name="key">要获取或设置的元素的键。</param>
		/// <returns>带有指定键的元素。</returns>
		object this[string key] { get; set; }

		/// <summary>
		/// 获取集合的元素个数
		/// </summary>
		int Count { get; }

		/// <summary>
		/// 设置指定键的值
		/// </summary>
		/// <param name="key">字典中键的名称</param>
		/// <param name="value">需要设置的值</param>
		void SetObject(string key, object value);

		/// <summary>
		/// 获取指定键的Object值。
		/// </summary>
		/// <param name="key">字典中键的名称</param>
		/// <returns>指定键的Object值。</returns>
		object GetObject(string key);
	}
}
