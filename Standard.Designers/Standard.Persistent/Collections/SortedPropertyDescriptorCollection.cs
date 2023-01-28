using Basic.Designer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basic.Collections
{
    /// <summary>
    /// 已经排序的 PropertyDescriptor 类型集合
    /// </summary>
    public sealed class SortedPropertyDescriptorCollection : ObservableCollection<PropertyDescriptor>
    {
        /// <summary>
        /// 
        /// </summary>
        public SortedPropertyDescriptorCollection() : base() { }

        /// <summary>
        /// 将一项插入集合中指定索引处。
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 item。</param>
        /// <param name="item">要插入的对象。</param>
        protected override void InsertItem(int index, PropertyDescriptor item)
        {
            for (int newIndex = 0; newIndex < base.Count; newIndex++)
            {
                PropertyDescriptor property = base[newIndex];
                PropertyOrderAttribute xOrder = property.Attributes[typeof(PropertyOrderAttribute)] as PropertyOrderAttribute;
                PropertyOrderAttribute yOrder = item.Attributes[typeof(PropertyOrderAttribute)] as PropertyOrderAttribute;
                int xOrderIndex = 0, yOrderIndex = 0;
                if (xOrder != null) { xOrderIndex = xOrder.Index; }
                if (yOrder != null) { yOrderIndex = yOrder.Index; }
                if (xOrderIndex >= yOrderIndex) { base.InsertItem(newIndex, item); return; }
            }
            base.InsertItem(index, item);
        }

        /// <summary>
        /// 返回已经排序的 PropertyDescriptorCollection 类实例
        /// </summary>
        /// <returns>已经排序的 PropertyDescriptorCollection 类实例。</returns>
        public PropertyDescriptorCollection ToSortedCollection()
        {
            PropertyDescriptor[] propertyArray = new PropertyDescriptor[base.Count];
            base.CopyTo(propertyArray, 0);
            return new PropertyDescriptorCollection(propertyArray, true);
        }

        internal class PropertyDescriptorComparer : Comparer<PropertyDescriptor>
        {
            public override int Compare(PropertyDescriptor x, PropertyDescriptor y)
            {
                PropertyOrderAttribute xOrder = x.Attributes[typeof(PropertyOrderAttribute)] as PropertyOrderAttribute;
                PropertyOrderAttribute yOrder = y.Attributes[typeof(PropertyOrderAttribute)] as PropertyOrderAttribute;
                int xOrderIndex = 0, yOrderIndex = 0;
                if (xOrder != null) { xOrderIndex = xOrder.Index; }
                if (yOrder != null) { yOrderIndex = yOrder.Index; }
                if (xOrderIndex > yOrderIndex) { return 1; }
                else if (xOrderIndex < yOrderIndex) { return -1; }
                return 0;
            }
        }
    }
}
