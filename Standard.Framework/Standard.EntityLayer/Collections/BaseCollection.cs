using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System;
using System.Globalization;
using System.ComponentModel;

namespace Basic.Collections
{
	/// <summary>
	/// 提供可排序集合管理
	/// </summary>
	/// <typeparam name="T">字段值类型</typeparam>
	public abstract class BaseCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>,
		IList<T>, ICollection<T>, IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : class
	{
		/// <summary>
		/// 在更改属性值时发生。
		/// </summary>
		public new event PropertyChangedEventHandler PropertyChanged;
		private readonly SortedDictionary<string, T> dictonary;
		private readonly StringComparer comparer = StringComparer.Create(CultureInfo.CurrentCulture, true);

		#region 构造函数
		/// <summary>
		/// 初始化 Basic.Collections.BaseCollection&lt;T&gt; 类的新实例。
		/// </summary>
		public BaseCollection() : base() { dictonary = new SortedDictionary<string, T>(comparer); }

		/// <summary>
		/// 初始化 Basic.Collections.BaseCollection&lt;T&gt; 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="collection">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		public BaseCollection(IEnumerable<T> collection) : base(collection) { dictonary = new SortedDictionary<string, T>(comparer); }

		/// <summary>
		/// 初始化 Basic.Collections.BaseCollection&lt;T&gt; 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="list">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		public BaseCollection(List<T> list) : base(list) { dictonary = new SortedDictionary<string, T>(comparer); }
		#endregion

		/// <summary>
		/// 引发 PropertyChanged 事件
		/// </summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal abstract string GetKey(T item);

		/// <summary>
		/// 从集合中移除所有项。
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			dictonary.Clear();
		}

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, T item)
		{
			string key = GetKey(item);
			if (string.IsNullOrEmpty(key)) { throw new KeyNotFoundException("插入集合的项，键值不能为空！"); }
			dictonary[key] = item;
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 移除集合中指定索引处的项。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected override void RemoveItem(int index)
		{
			T oldItem = base[index];
			string key = GetKey(oldItem);
			dictonary.Remove(key);
			base.RemoveItem(index);
		}

		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。</param>
		protected override void SetItem(int index, T item)
		{
			T oldItem = base[index];
			string key = GetKey(oldItem);
			dictonary.Remove(key);
			base.SetItem(index, item);
			key = GetKey(item);
			dictonary.Add(key, item);
		}

		/// <summary>
		/// 确定 Basic.Collections.BaseCollection&lt;string,T&gt; 是否包含具有指定键的元素。
		/// </summary>
		/// <param name="key">要在 Basic.Collections.BaseCollection&lt;string,T&gt; 中定位的键。</param>
		/// <returns>如果 Basic.Collections.BaseCollection&lt;string,T&gt; 包含具有指定键的元素，则为 true；否则为 false。</returns>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		public bool ContainsKey(string key)
		{
			return dictonary.ContainsKey(key);
		}

		/// <summary>
		/// 获取或设置与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取或设置的值的键。</param>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		/// <returns>与指定的键相关联的值。</returns>
		public T this[string key]
		{
			get
			{
				if (dictonary.ContainsKey(key))
				{
					return dictonary[key];
				}
				return null;
			}
		}

		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取的值的键。</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。</param>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		/// <returns>如果 System.Collections.Generic.SortedDictionary&lt;string,T&gt; 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public bool TryGetValue(string key, out T value)
		{
			return dictonary.TryGetValue(key, out value);
		}
	}
}
