using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.DataEntities;
using System.ComponentModel;

namespace Basic.Collections
{
	/// <summary>
	/// 表示验证特性集合
	/// </summary>
	[Editor(typeof(ValidationAttributesListEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public sealed class AbstractValidationCollection : Basic.Collections.BaseCollection<AbstractAttribute>
	{
		private readonly DataEntityPropertyElement ownerProperty;

		internal DataEntityPropertyElement Property { get { return ownerProperty; } }
		/// <summary>
		/// 初始化 AbstractAttributeCollection 类实例。
		/// </summary>
		/// <param name="property">拥有此集合的属性。</param>
		internal AbstractValidationCollection(DataEntityPropertyElement property) : base() { ownerProperty = property; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ownerProperty.OnFileContentChanged(e);
			base.OnCollectionChanged(e);
		}

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, AbstractAttribute item)
		{
			item.FileContentChanged += new System.EventHandler((sender, e) => { ownerProperty.OnFileContentChanged(e); });
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected internal override string GetKey(AbstractAttribute item)
		{
			return item.ElementName;
		}
	}
}
