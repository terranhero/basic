using System.Collections.Generic;

namespace Basic.Interfaces
{
	/// <summary>
	/// IParameter参数集合
	/// </summary>
	public interface IParameterCollection<T> : IList<T>, ICollection<T>, IEnumerable<T> where T : IParameter
	{
		/// <summary>
		/// 获取或设置指定索引处的参数。
		/// </summary>
		/// <param name="parameterName">要检索的参数的名称。</param>
		/// <value>指定索引处的 IParameter子类实例。</value>
		T this[string parameterName] { get; set; }

		/// <summary>
		/// 获取一个值，该值指示集合中是否存在具有指定名称的参数。
		/// </summary>
		/// <param name="parameterName">参数名。</param>
		/// <returns>如果集合包含该参数，则为 true；否则为 false。</returns>
		bool Contains(string parameterName);

		/// <summary>
		/// 获取 System.Data.IDataParameter 在集合中的位置。
		/// </summary>
		/// <param name="parameterName">参数名。</param>
		/// <returns>IParameter 在集合中从零开始的位置。</returns>
		int IndexOf(string parameterName);

		/// <summary>
		/// 从集合中移除 IParameter。
		/// </summary>
		/// <param name="parameterName">参数名。</param>
		void RemoveAt(string parameterName);
	}
}
