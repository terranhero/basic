
namespace Basic.Enums
{
	/// <summary>
	/// Transact-SQL动态参数条件比较
	/// </summary>
	public enum ParameterCompare
	{
		/// <summary>
		/// 相等比较
		/// </summary>
		Equal,
		/// <summary>
		/// 不等于比较。
		/// </summary>
		NotEqual,
		/// <summary>
		/// 大于比较。
		/// </summary>
		GreaterThan,
		/// <summary>
		/// 大于或等于比较。
		/// </summary>
		GreaterThanEqual,
		/// <summary>
		/// 小于比较。
		/// </summary>
		LessThan,
		/// <summary>
		/// 小于或等于比较。
		/// </summary>
		LessThanEqual,
		/// <summary>
		/// LIKE比较
		/// </summary>
		Like,
		/// <summary>
		/// NOT LIKE比较
		/// </summary>
		NotLike,
		/// <summary>
		/// IN比较
		/// </summary>
		In,
		/// <summary>
		/// NOT IN比较
		/// </summary>
		NotIn,
		/// <summary>
		/// 判断字段是否为NULL比较
		/// </summary>
		IsNull,
		/// <summary>
		/// 判断字段是否不为NULL比较
		/// </summary>
		IsNotNull,
		/// <summary>
		/// 按位与后大于零比较
		/// </summary>
		BitAnd
	}


}
