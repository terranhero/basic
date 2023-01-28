using System.Collections.Generic;
using System.Collections.Specialized;
using SC = System.Collections;

namespace Basic.Interfaces
{
	/// <summary>
	/// 一个提供用于实现分页控件的实体类集合
	/// </summary>
	public interface IPagination : SC.IEnumerable, INotifyCollectionChanged
	{
		/// <summary>
		/// 当前集合记录数.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// 当前记录总数.
		/// </summary>
		int Capacity { get; set; }

		/// <summary>
		/// 当前页面索引
		/// </summary>
		int PageIndex { get; }

		/// <summary>
		/// 每页记录数量
		/// </summary>
		int PageSize { get; }
	}

	/// <summary>
	/// 提供泛型的分页接口
	/// </summary>
	/// <typeparam name="T">Type of object being paged</typeparam>
	public interface IPagination<T> : IPagination, IEnumerable<T>
	{
		/// <summary><![CDATA[将指定集合的元素添加到 Pagination<T> 的末尾。]]></summary>
		/// <param name="collection"><![CDATA[一个集合，其元素应被添加到 IPagination<T> 的末尾。 集合自身不能为 null，但它可以包含为 null 的元素（如果类型 T 为引用类型）。]]></param>
		void AddRange(IEnumerable<T> collection);

		/// <summary><![CDATA[将对象添加到 IPagination<T> 的结尾处]]></summary>
		/// <param name="item"><![CDATA[要添加到 IPagination<T> 末尾的对象。 对于引用类型，该值可以为 null。]]></param>
		/// <returns>返回添加到集合末尾的元素。</returns>
		T Add(T item);

		/// <summary><![CDATA[确定某元素是否在 IPagination<T> 中]]></summary>
		/// <param name="item"><![CDATA[要在 IPagination<T>中定位的对象。 对于引用类型，该值可以为 null。]]></param>
		bool Contains(T item);

		/// <summary>
		/// 从目标数组的指定索引处开始将整个 IPagination&lt;T&gt;复制到兼容的一维 Array。 
		/// </summary>
		/// <param name="index">array 中从零开始的索引，从此索引处开始进行复制。</param>
		/// <returns>一个数组，它包含 IPagination&lt;T&gt;的元素的副本。</returns>
		T[] ToArray(int index);

		/// <summary>
		/// 将 IPagination&lt;T&gt;的元素复制到新数组中。
		/// </summary>
		/// <returns>一个数组，它包含 IPagination&lt;T&gt;的元素的副本。</returns>
		T[] ToArray();

		/// <summary><![CDATA[将 IPagination<T>的元素复制到新 List<T> 中。]]></summary>
		/// <returns><![CDATA[一个 List<T>实例，它包含 IPagination<T>的元素的副本。]]></returns>
		List<T> ToList();

		/// <summary>
		/// <![CDATA[对 IPagination<T> 的每个元素执行指定操作。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 IPagination<T> 的每个元素执行的 Action<T> 委托。]]></param>
		void ForEach(System.Action<T> action);

		/// <summary>
		/// <![CDATA[对 Pagination<T> 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 Pagination<T> 的每个元素执行的 System.Action<T, int> 委托。]]></param>
		void ForEach(System.Action<T, int> action);


		/// <summary>
		/// <![CDATA[对 Pagination<T> 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 Pagination<T> 的每个元素执行的 System.Action<T> 委托。]]></param>
		/// <param name="match"><![CDATA[要对 Pagination<T> 的每个元素执行的 System.Predicate<T> 委托。]]></param>
		void ForEach(System.Action<T> action, System.Predicate<T> match);
	}
}
