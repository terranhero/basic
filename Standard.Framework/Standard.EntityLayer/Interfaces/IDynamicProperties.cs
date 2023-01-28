using System.Collections;
using System.Collections.Generic;

namespace Basic.Interfaces
{
	/// <summary>
	/// 表示动态属性集合
	/// </summary>
	public interface IDynamicProperties<T> : IEnumerable<T>, IEnumerable where T : IDynamicProperty
	{
	}
}
