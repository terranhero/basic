using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Basic.EntityLayer;
using Basic.Interfaces;

namespace Basic.Collections
{
	/// <summary>
	/// 抽象实体集合抽象基类
	/// </summary>
	/// <typeparam name="T">集合中的元素类型，基于AbstractEntity类的实例。</typeparam>
	public abstract class AbstractEntityCollection<T> : Pagination<T>, IPagination<T>, INotifyPropertyChanged where T : AbstractEntity
	{
		private readonly AbstractEntity _owner;
		/// <summary>初始化 AbstractEntityCollection 类的新实例</summary>
		protected AbstractEntityCollection() : base() { }

		/// <summary><![CDATA[初始化 AbstractEntityCollection<T> 类的新实例，该类包含从指定集合中复制的元素。]]></summary>
		/// <param name="list">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		protected AbstractEntityCollection(IPagination<T> list) : base(list) { }

		/// <summary>初始化AbstractEntityCollection 类的新实例，该类包含从指定列表中复制的元素</summary>
		/// <param name="collection">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		protected AbstractEntityCollection(IEnumerable<T> collection) : base(collection) { }

		/// <summary>初始化AbstractEntityCollection 类的新实例，该类包含从指定列表中复制的元素</summary>
		/// <param name="handler">集合变更事件</param>
		protected AbstractEntityCollection(NotifyCollectionChangedEventHandler handler) : base(handler) { }

		/// <summary><![CDATA[初始化 AbstractEntityCollection 类的新实例。]]></summary>
		/// <param name="dataSource">可用于分页的数据源</param>
		/// <param name="totalItems">本次查询结果集记录总数</param>
		protected AbstractEntityCollection(IEnumerable<T> dataSource, int totalItems) : base(dataSource) { base.Capacity = totalItems; }

		/// <summary>初始化 AbstractEntityCollection 类的新实例</summary>
		/// <param name="owner">当前集合作为子表时，拥有此集合的上级实体模型，上级实体模型会接收集合变化通知</param>
		protected AbstractEntityCollection(AbstractEntity owner) : base() { _owner = owner; }

		/// <summary><![CDATA[初始化 AbstractEntityCollection<T> 类的新实例，该类包含从指定集合中复制的元素。]]></summary>
		/// <param name="owner">当前集合作为子表时，拥有此集合的上级实体模型，上级实体模型会接收集合变化通知</param>
		/// <param name="list">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		protected AbstractEntityCollection(AbstractEntity owner, IPagination<T> list) : base(list) { _owner = owner; }

		/// <summary>初始化AbstractEntityCollection 类的新实例，该类包含从指定列表中复制的元素</summary>
		/// <param name="owner">当前集合作为子表时，拥有此集合的上级实体模型，上级实体模型会接收集合变化通知</param>
		/// <param name="collection">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		protected AbstractEntityCollection(AbstractEntity owner, IEnumerable<T> collection) : base(collection) { _owner = owner; }

		/// <summary>初始化AbstractEntityCollection 类的新实例，该类包含从指定列表中复制的元素</summary>
		/// <param name="owner">当前集合作为子表时，拥有此集合的上级实体模型，上级实体模型会接收集合变化通知</param>
		/// <param name="handler">集合变更事件</param>
		protected AbstractEntityCollection(AbstractEntity owner, NotifyCollectionChangedEventHandler handler) : base(handler) { _owner = owner; }

		/// <summary><![CDATA[初始化 AbstractEntityCollection 类的新实例。]]></summary>
		/// <param name="owner">当前集合作为子表时，拥有此集合的上级实体模型，上级实体模型会接收集合变化通知</param>
		/// <param name="dataSource">可用于分页的数据源</param>
		/// <param name="totalItems">本次查询结果集记录总数</param>
		protected AbstractEntityCollection(AbstractEntity owner, IEnumerable<T> dataSource, int totalItems) : base(dataSource) { base.Capacity = totalItems; _owner = owner; }

		/// <summary>引发带有提供的参数的 CollectionChanged 事件。</summary>
		/// <param name="e">集合更改事件参数</param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			if (_owner != null) { _owner.OnPropertyChanged(typeof(T).Name); }
		}

		#region  接口 INotifyPropertyChanged
		/// <summary>
		/// 更改属性值时发生的事件。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 引发 PropertyChanged 事件
		/// </summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
		}
		/// <summary>判断属性值是否更改，并引发 PropertyChanged 事件</summary>
		/// <param name="propertyName">已更改的属性名。</param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		internal protected TP TryOnPropertyChanged<TP>(string propertyName, out TP oldValue, TP newValue)
		{
			oldValue = newValue;
			OnPropertyChanged("OrderKey");
			return newValue;
		}
		#endregion

	}
}
