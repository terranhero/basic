using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Basic.Collections
{
	/// <summary>
	/// 提供可排序集合管理
	/// </summary>
	/// <typeparam name="T">字段值类型</typeparam>
	public abstract class BaseDictionary<T> : SortedList<string, T>, ICollection<T>, IEnumerable<T>, INotifyCollectionChanged
	{
		#region 构造函数
		/// <summary>
		/// 初始化 BaseDictionary&lt;T&gt; 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		public BaseDictionary() : base() { }

		/// <summary>
		/// 初始化 BaseDictionary&lt;T&gt; 类的新实例，该实例为空且具有指定的初始容量，并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="capacity">System.Collections.Generic.Dictionary&lt;T&gt; 可包含的初始元素数。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">capacity 小于 0。</exception>
		public BaseDictionary(int capacity) : base(capacity) { }

		/// <summary>
		/// 初始化 BaseDictionary&lt;T&gt; 类的新实例，该实例包含从指定的 IDictionary&lt;string, T&gt;中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">IDictionary&lt;T&gt;，它的元素被复制到新的 BaseDictionary&lt;T&gt;中。</param>
		public BaseDictionary(IDictionary<string, T> dictionary) : base(dictionary) { }

		/// <summary>
		/// 初始化 BaseDictionary&lt;T&gt; 类的新实例，该实例为空且具有默认的初始容量，
		/// 并使用指定的 System.Collections.Generic.IEqualityComparer&lt;T&gt;。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 System.Collections.Generic.IEqualityComparer&lt;string&gt; 
		/// 实现，或者为 null，以便为键类型使用默认的 System.Collections.Generic.EqualityComparer&lt;string&gt;。</param>
		public BaseDictionary(IComparer<string> comparer) : base(comparer) { }


		/// <summary>
		/// 初始化 BaseDictionary&lt;T&gt; 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;T&gt; 中
		/// 复制的元素并使用指定的 System.Collections.Generic.IEqualityComparer&lt;T&gt;。
		/// </summary>
		/// <param name="dictionary">
		/// System.Collections.Generic.IDictionary&lt;T&gt;，
		/// 它的元素被复制到新的 System.Collections.Generic.Dictionary&lt;T&gt; 中。
		/// </param>
		/// <param name="comparer">比较键时要使用的 System.Collections.Generic.IEqualityComparer&lt;T&gt; 实现，
		/// 或者为 null，以便为键类型使用默认的 System.Collections.Generic.EqualityComparer&lt;T&gt;。</param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public BaseDictionary(IDictionary<string, T> dictionary, IComparer<string> comparer) : base(dictionary, comparer) { }

		/// <summary>
		/// 初始化 BaseDictionary&lt;T&gt; 类的新实例，该实例为空且具有指定的初始容量，
		/// 并使用指定的 System.Collections.Generic.IEqualityComparer&lt;T&gt;。
		/// </summary>
		/// <param name="capacity">System.Collections.Generic.Dictionary&lt;T&gt; 可包含的初始元素数。</param>
		/// <param name="comparer">
		/// 比较键时要使用的 System.Collections.Generic.IEqualityComparer&lt;T&gt; 实现，
		/// 或者为 null，以便为键类型使用默认的System.Collections.Generic.EqualityComparer&lt;T&gt;。
		/// </param>
		/// <exception cref="System.ArgumentOutOfRangeException">capacity 小于 0。</exception>
		public BaseDictionary(int capacity, IComparer<string> comparer) : base(capacity, comparer) { }
		#endregion

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		/// <param name="item">要添加的对象。</param>
		public abstract void Add(T item);

		/// <summary>
		/// 从当前字典值集合中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从当前字典值集合 中移除的对象。</param>
		/// <exception cref="System.NotSupportedException">当前字典集合是只读的。</exception>
		/// <returns>如果已从当前字典值集合中成功移除 item，则为 true；否则为 false。
		/// 如果在原始当前字典值集合中没有找到 item，该方法也会返回 false。</returns>
		public abstract bool Remove(T item);

		#region ICollection&lt;T&gt; 接口成员
		/// <summary>
		/// 确定当前字典值集合中是否包含特定项。
		/// </summary>
		/// <param name="item">需要当前字典值集合中定位的对象。</param>
		/// <returns>如果在 当前字典值集合中找到 item，则为 true；否则为 false。</returns>
		public bool Contains(T item)
		{
			return base.ContainsValue(item);
		}

		/// <summary>
		/// 从特定的 System.Array 索引处开始，将当前字典值集合的元素复制到一个System.Array 中。
		/// </summary>
		/// <param name="array">作为从当前字典值集合复制的元素的目标位置的一维 System.Array。System.Array必须具有从零开始的索引。</param>
		/// <param name="arrayIndex">array 中从零开始的索引，将在此处开始复制。</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			base.Values.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// 将键值对集合中值集合转换成值数组。
		/// </summary>
		public virtual T[] ToArray()
		{
			return base.Values.ToArray();
		}

		/// <summary>
		/// 获取一个值，该值指示 当前字典值集合 是否为只读。
		/// </summary>
		/// <value>当前字典值集合始终为只读，因此始终返回 true。</value>
		public bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// 返回循环访问 当前字典值集合 的枚举数。
		/// </summary>
		/// <returns>当前字典集合中 ValueCollection 的 ValueCollection.Enumerator 结构</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return base.Values.GetEnumerator();
		}

		/// <summary>
		/// 返回循环访问 当前字典值集合 的枚举数。
		/// </summary>
		/// <returns>当前字典集合中 ValueCollection 的 ValueCollection.Enumerator 结构</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return base.Values.GetEnumerator();
		}
		#endregion

		#region 重载方法 用以更新集合更改通知
		/// <summary>
		/// 获取或设置与指定的键相关联的值。
		/// </summary>
		/// <param name="key"> 要获取或设置其值的键。</param>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">已检索该属性，并且集合中不存在 key。</exception>
		/// <returns>与指定的键相关联的值。如果找不到指定的键，则 get 操作会引发 System.Collections.Generic.KeyNotFoundException，而set 操作会创建一个使用指定键的新元素。</returns>
		public new T this[string key]
		{
			get { return base[key]; }
			set
			{
				base[key] = value;
				NotifyCollectionChangedEventArgs notifyEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value);
				OnCollectionChanged(notifyEventArgs);
			}
		}

		/// <summary>
		/// 将带有指定键和值的元素添加到当前字典集合中。
		/// </summary>
		/// <param name="key">要添加的元素的键。</param>
		/// <param name="value">要添加的元素的值。对于引用类型，该值可以为 null。</param>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		/// <exception cref="System.ArgumentException">当前字典中已存在具有相同键的元素。</exception>
		public void AddItem(string key, T value)
		{
			base[key] = value;
			NotifyCollectionChangedEventArgs notifyEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value);
			OnCollectionChanged(notifyEventArgs);
		}

		/// <summary>
		/// 从 BaseDictionary&lt;T&gt; 中移除带有指定键的元素。
		/// </summary>
		/// <param name="key">要移除的元素的键。</param>
		/// <returns>如果该元素已成功移除，则为 true；否则为 false。如果在 BaseDictionary&lt;T&gt; 中没有找到 key，此方法也会返回 false。</returns>
		public new bool Remove(string key)
		{
			T value = base[key];
			bool result = base.Remove(key);
			if (result)
			{
				NotifyCollectionChangedEventArgs notifyEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
			}
			return result;
		}

		/// <summary>
		/// 在添加、移除、更改或移动项或者在刷新整个列表时发生。
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// 引发带有提供的参数的 BaseDictionary&lt;T&gt;.CollectionChanged 事件。
		/// </summary>
		/// <param name="e">要引发的事件的参数。</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null) { CollectionChanged(this, e); }
		}
		#endregion
	}
}
