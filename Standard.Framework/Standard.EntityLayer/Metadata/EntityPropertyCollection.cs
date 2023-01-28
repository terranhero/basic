using Basic.EntityLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Basic.Collections
{
	/// <summary>
	/// 表示 EntityPropertyDescriptor 类行集合。
	/// </summary>
	public sealed class EntityPropertyCollection : Collection<EntityPropertyMeta>
	{
		private readonly SortedDictionary<string, EntityPropertyMeta> propertyDectionary;
		private readonly SortedDictionary<string, EntityPropertyMeta> columnDectionary;
		/// <summary>
		/// 初始化为空的 Basic.Collections.EntityPropertyCollection 类的新实例。
		/// </summary>
		internal EntityPropertyCollection()
			: base()
		{
			propertyDectionary = new SortedDictionary<string, EntityPropertyMeta>(StringComparer.OrdinalIgnoreCase);
			columnDectionary = new SortedDictionary<string, EntityPropertyMeta>(StringComparer.OrdinalIgnoreCase);
		}
		/// <summary>
		/// 初始化为空的 Basic.Collections.EntityPropertyCollection 类的新实例。
		/// </summary>
		internal EntityPropertyCollection(IList<EntityPropertyMeta> list)
			: base(list)
		{
			if (list == null)
			{
				propertyDectionary = new SortedDictionary<string, EntityPropertyMeta>(StringComparer.OrdinalIgnoreCase);
				columnDectionary = new SortedDictionary<string, EntityPropertyMeta>(StringComparer.OrdinalIgnoreCase);
			}
			else
			{
				propertyDectionary = new SortedDictionary<string, EntityPropertyMeta>(list.ToDictionary(m => m.Name),
					StringComparer.OrdinalIgnoreCase);
				columnDectionary = new SortedDictionary<string, EntityPropertyMeta>(list.ToDictionary(m => m.Mapping != null ?
				m.Mapping.ColumnName : m.Name), StringComparer.OrdinalIgnoreCase);
			}
		}
		/// <summary>
		/// 从集合中移除所有项。
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			propertyDectionary.Clear();
			columnDectionary.Clear();
		}

		/// <summary>
		/// <![CDATA[对 EntityPropertyCollection 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 EntityPropertyCollection 的每个元素执行的 Action<EntityPropertyDescriptor> 委托。]]></param>
		public void ForEach(Action<EntityPropertyMeta> action)
		{
			if (action == null) { throw new ArgumentNullException("action"); }
			for (int index = 0; index < Items.Count; index++) { action(Items[index]); }
		}

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, EntityPropertyMeta item)
		{
			if (item == null) { return; }
			propertyDectionary[item.Name] = item;
			if (item.Mapping != null)
				columnDectionary[item.Mapping.ColumnName] = item;
			else
				columnDectionary[item.Name] = item;
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 移除集合中指定索引处的项。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected override void RemoveItem(int index)
		{
			EntityPropertyMeta item = base[index];
			propertyDectionary.Remove(item.Name);
			if (item.Mapping != null)
				columnDectionary.Remove(item.Mapping.ColumnName);
			else
				columnDectionary.Remove(item.Name);
			base.RemoveItem(index);
		}

		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。</param>
		protected override void SetItem(int index, EntityPropertyMeta item)
		{
			if (item == null) { return; }
			EntityPropertyMeta oldItem = base[index];
			propertyDectionary.Remove(oldItem.Name);
			if (oldItem.Mapping != null)
				columnDectionary.Remove(oldItem.Mapping.ColumnName);
			else
				columnDectionary.Remove(oldItem.Name);

			base.SetItem(index, item);
			propertyDectionary[item.Name] = item;
			if (item.Mapping != null)
				columnDectionary.Remove(item.Mapping.ColumnName);
			else
				columnDectionary.Remove(item.Name);
		}

		/// <summary>
		/// 将键值对集合中值集合转换成值数组。
		/// </summary>
		public EntityPropertyMeta[] ToArray()
		{
			EntityPropertyMeta[] array = new EntityPropertyMeta[base.Count];
			CopyTo(array, 0);
			return array;
		}

		/// <summary>
		/// 获取属性的定义信息。
		/// </summary>
		/// <param name="propertyName">属性名称</param>
		/// <param name="propertyInfo">需要返回的EntityPropertyDescriptor 类实例，属性定义信息。</param>
		/// <returns>如果 EntityPropertyCollection 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public bool TryGetProperty(string propertyName, out EntityPropertyMeta propertyInfo)
		{
			return propertyDectionary.TryGetValue(propertyName, out propertyInfo);
		}

		/// <summary>
		/// 判断当前实体是否存在数据库字段信息
		/// </summary>
		/// <param name="name">字段名称</param>
		/// <returns>如果实体包含具有指定键的元素，则为 true；否则为 false。</returns>
		public bool ContainColumn(string name)
		{
			return columnDectionary.ContainsKey(name);
		}

		/// <summary>
		/// 获取属性的定义信息。
		/// </summary>
		/// <param name="name">字段名称</param>
		/// <param name="propertyInfo">需要返回的EntityPropertyDescriptor 类实例，属性定义信息。</param>
		/// <returns>如果 EntityPropertyCollection 包含具有指定键的元素，则为 true；否则为 false。</returns>
		public bool TryGetDbProperty(string name, out EntityPropertyMeta propertyInfo)
		{
			return columnDectionary.TryGetValue(name, out propertyInfo);
		}
	}
}
