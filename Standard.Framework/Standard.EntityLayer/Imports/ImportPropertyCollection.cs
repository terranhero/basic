using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Basic.Interfaces;
using Basic.EntityLayer;
using System.Collections.Specialized;

namespace Basic.Imports
{
	/// <summary>
	/// 表示 ImportItem 类集合
	/// </summary>
	internal sealed class ImportPropertyCollection<TE> : ObservableCollection<IImportProperty<TE>>, IImportPropertyCollection<TE>
		where TE : AbstractEntity, new()
	{
		/// <summary>
		/// 初始化 ImportPropertyCollection 类的新实例。
		/// </summary>
		public ImportPropertyCollection() : base() { }

		/// <summary>
		/// 初始化 ImportPropertyCollection 类的新实例，该类包含从指定集合中复制的元素。
		/// </summary>
		/// <param name="collection">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		public ImportPropertyCollection(IEnumerable<IImportProperty<TE>> collection) : base(collection) { }

		/// <summary>
		/// 初始化 ImportPropertyCollection 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="list">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		public ImportPropertyCollection(List<IImportProperty<TE>> list) : base(list) { }

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, IImportProperty<TE> item)
		{
			for (int newIndex = 0; newIndex < base.Count; newIndex++)
			{
				if (base[newIndex].Index > item.Index) { base.InsertItem(newIndex, item); return; }
			}
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 将指定索引处的项移至集合中的新位置。
		/// </summary>
		/// <param name="oldIndex">从零开始的索引，用于指定要移动的项的位置。</param>
		/// <param name="newIndex">从零开始的索引，用于指定项的新位置。</param>
		protected override void MoveItem(int oldIndex, int newIndex) { }

		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。</param>
		protected override void SetItem(int index, IImportProperty<TE> item)
		{
			base.RemoveItem(index);
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 获取指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取的元素的从零开始的索引。</param>
		/// <returns>指定索引处的元素。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">index 小于零。- 或 -index 等于或大于 Count。</exception>
		IImportProperty<TE> IImportPropertyCollection<TE>.this[int index]
		{
			get { return base[index]; }
		}

		/// <summary>
		/// 确定某元素是否在 IImportPropertyCollection 中。
		/// </summary>
		/// <param name="value">要在 IImportPropertyCollection 中定位的对象。对于引用类型，该值可以为 null。</param>
		/// <returns>如果在 IImportPropertyCollection 中找到 value，则为 true；否则为 false。</returns>
		bool IImportPropertyCollection<TE>.Contains(IImportProperty<TE> value)
		{
			return base.Contains(value);
		}
	}
}
