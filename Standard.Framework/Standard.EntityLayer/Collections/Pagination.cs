using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Basic.Collections
{
	/// <summary>IPagination接口的默认实现。</summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1005:可简化委托调用。", Justification = "<挂起>")]
	public class Pagination<T> : Collection<T>, Basic.Interfaces.IPagination<T> where T : class
	{
		private bool _NotifyCollectionChanged = true;
		private readonly List<T> mItems;
		/// <summary>
		/// <![CDATA[初始化 Basic.Collections.Pagination<T> 类的新实例。]]>
		/// </summary>
		public Pagination() : this(0) { }

		/// <summary>
		/// <![CDATA[初始化 Basic.Collections.Pagination<T> 类的新实例，该实例为空并且具有指定的初始容量。]]>
		/// </summary>
		/// <param name="capacity">新列表最初可以存储的元素数。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">capacity 小于 0。</exception>
		public Pagination(int capacity) : base() { this.Capacity = capacity; mItems = base.Items as List<T>; }

		/// <summary>
		/// <![CDATA[初始化 Basic.Collections.Pagination<T> 类的新实例。]]>
		/// </summary>
		/// <param name="collection">可用于分页的数据源</param>
		public Pagination(Basic.Interfaces.IPagination<T> collection)
			: base(new List<T>(collection)) { mItems = base.Items as List<T>; this.Capacity = collection.Capacity; }

		/// <summary>
		/// <![CDATA[初始化 Basic.Collections.Pagination<T> 类的新实例。]]>
		/// </summary>
		/// <param name="collection">可用于分页的数据源</param>
		public Pagination(IEnumerable<T> collection)
			: base(new List<T>(collection)) { mItems = base.Items as List<T>; this.Capacity = mItems.Count; }

		/// <summary><![CDATA[初始化 Basic.Collections.Pagination<T> 类的新实例。]]></summary>
		/// <param name="handler">集合变更事件</param>
		public Pagination(NotifyCollectionChangedEventHandler handler) : this(0) { this.CollectionChanged += handler; }

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		/// <summary><![CDATA[将对象添加到 IPagination<T> 的结尾处]]></summary>
		/// <param name="item"><![CDATA[要添加到 IPagination<T> 末尾的对象。 对于引用类型，该值可以为 null。]]></param>
		/// <returns>返回添加到集合末尾的元素。</returns>
		public new T Add(T item) { base.Add(item); return item; }

		/// <summary><![CDATA[将指定集合的元素添加到 Pagination<T> 的末尾。]]></summary>
		/// <param name="collection"><![CDATA[一个集合，其元素应被添加到 Pagination<T> 的末尾。 集合自身不能为 null，但它可以包含为 null 的元素（如果类型 T 为引用类型）。]]></param>
		public void AddRange(IEnumerable<T> collection)
		{
			mItems.AddRange(collection);
		}

		/// <summary>
		/// 当前页面索引
		/// </summary>
		public int PageIndex { get; internal set; }

		/// <summary>
		/// 每页记录数量
		/// </summary>
		public int PageSize { get; internal set; }

		/// <summary>
		/// 当前记录总数.
		/// </summary>
		public int Capacity { get; set; }

		/// <summary>
		/// <![CDATA[从目标数组的指定索引处开始将整个 Pagination<T> 复制到兼容的一维 Array。 ]]>
		/// </summary>
		/// <param name="index">array 中从零开始的索引，从此索引处开始进行复制。</param>
		/// <returns><![CDATA[一个数组，它包含 Pagination<T> 的元素的副本。]]></returns>
		public T[] ToArray(int index)
		{
			if (this.Count == 0) { return new T[0]; }
			T[] array = new T[Count - index];
			this.CopyTo(array, index);
			return array;
		}

		/// <summary>
		/// <![CDATA[将 Pagination<T> 的元素复制到新数组中。 ]]>
		/// </summary>
		/// <returns><![CDATA[一个数组，它包含 Pagination<T> 的元素的副本。]]></returns>
		public T[] ToArray() { return this.ToArray(0); }

		/// <summary><![CDATA[将 IPagination<T>的元素复制到 List<T> 中。]]></summary>
		/// <returns><![CDATA[一个 List<T>实例，它包含 IPagination<T>的元素的副本。]]></returns>
		public List<T> ToList() { return new List<T>(mItems); }

		/// <summary>
		/// <![CDATA[对 Pagination<T> 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 Pagination<T> 的每个元素执行的 Action<T> 委托。]]></param>
		public void ForEach(Action<T> action)
		{
			if (action == null) { throw new ArgumentNullException("action"); }
			if (mItems.Count == 0) { return; }
			mItems.ForEach(action);

		}

		/// <summary>
		/// <![CDATA[对 Pagination<T> 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 Pagination<T> 的每个元素执行的 System.Action<T, int> 委托。]]></param>
		public void ForEach(Action<T, int> action)
		{
			if (action == null) { throw new ArgumentNullException("action"); }
			if (mItems.Count == 0) { return; }
			int size = mItems.Count;
			for (int i = 0; i < size; i++) { action(mItems[i], i); }

		}

		/// <summary>
		/// <![CDATA[对 Pagination<T> 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 Pagination<T> 的每个元素执行的 System.Action<T> 委托。]]></param>
		/// <param name="match"><![CDATA[要对 Pagination<T> 的每个元素执行的 System.Predicate<T> 委托。]]></param>
		public void ForEach(Action<T> action, System.Predicate<T> match)
		{
			if (action == null) { throw new ArgumentNullException("action"); }
			if (match == null) { throw new ArgumentNullException("match"); }
			if (mItems.Count == 0) { return; }
			int size = mItems.Count;
			for (int i = 0; i < size; i++) { T item = mItems[i]; if (match(item)) { action(item); } }
		}

		#region 实现 INotifyCollectionChanged 接口
		/// <summary>启用批量集合初始化，集合的变更将不再引发 CollectionChanged 事件。
		/// 需要调用 EndInitialization() 方法启用 CollectionChanged 事件</summary>
		public void BeginCollectionChanged() { _NotifyCollectionChanged = false; }

		/// <summary>集合更改结束，手动引发 CollectionChanged 事件</summary>
		public void EndCollectionChanged()
		{
			_NotifyCollectionChanged = true;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		/// <summary>
		/// 在添加、移除、更改或移动项或者在刷新整个列表时发生。
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// 在添加、移除、更改或移动项或者在刷新整个列表时发生。
		/// </summary>
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add { CollectionChanged += value; }
			remove { CollectionChanged -= value; }
		}

		/// <summary>引发带有提供的参数的 CollectionChanged 事件。</summary>
		/// <param name="e">集合更改事件参数</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (_NotifyCollectionChanged == false) { return; }
			if (CollectionChanged != null) { CollectionChanged(this, e); }
		}


		/// <summary>
		/// 引发带有提供的参数的 CollectionChanged 事件。
		/// </summary>
		/// <param name="action">引起该事件的操作。 这可以设置为 System.Collections.Specialized.NotifyCollectionChangedAction.Reset、System.Collections.Specialized.NotifyCollectionChangedAction.Add 或 System.Collections.Specialized.NotifyCollectionChangedAction.Remove。</param>
		/// <param name="changedItems">受更改影响的各项。</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:删除未使用的私有成员", Justification = "<挂起>")]
		private void OnCollectionChanged(NotifyCollectionChangedAction action, IList changedItems)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems));
		}

		/// <summary>
		/// 引发带有提供的参数的 CollectionChanged 事件
		/// </summary>
		/// <param name="action">引起该事件的操作。</param>
		/// <param name="item">受更改影响的项。</param>
		/// <param name="index">要替换的项的索引。</param>
		private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
		}

		/// <summary>
		/// 引发带有提供的参数的 CollectionChanged 事件
		/// </summary>
		/// <param name="action">引起该事件的操作。 这可以设置为 System.Collections.Specialized.NotifyCollectionChangedAction.Reset、System.Collections.Specialized.NotifyCollectionChangedAction.Add 或 System.Collections.Specialized.NotifyCollectionChangedAction.Remove。</param>
		private void OnCollectionChanged(NotifyCollectionChangedAction action)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
		}

		/// <summary>
		/// 引发带有提供的参数的 CollectionChanged 事件
		/// </summary>
		/// <param name="action">引起该事件的操作。</param>
		/// <param name="oldItem">要替换的原始项。</param>
		/// <param name="newItem">要替换原始项的新项。</param>
		/// <param name="index">要替换的项的索引。</param>
		private void OnCollectionChanged(NotifyCollectionChangedAction action, T oldItem, T newItem, int index)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, oldItem, newItem, index));
		}

		/// <summary>
		/// <![CDATA[从 Basic.Collections.Pagination<T> 中移除所有元素。]]>
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			this.OnCollectionChanged(NotifyCollectionChangedAction.Reset);
		}

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, T item)
		{
			base.InsertItem(index, item);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}

		/// <summary>
		/// 移除集合中指定索引处的项。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected override void RemoveItem(int index)
		{
			T item = base[index];
			base.RemoveItem(index);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}

		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。</param>
		protected override void SetItem(int index, T item)
		{
			T oldItem = base.Items[index];
			base.SetItem(index, item);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
		}
		#endregion

	}
}
