using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using Basic.Enums;
using System.Reflection;
using Basic.Collections;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 抽象实体，提供UI和数据表连接的数据载体
	/// </summary>
	[Serializable, ToolboxItem(false)]
	public abstract class AbstractCondition : AbstractEntity
	{
		/// <summary>
		/// 标识条件类中属性是否赋值。
		/// </summary>
		private readonly BitArray bitArray;
		//private readonly int pSortField;
		//private readonly int pSortOrder;

		/// <summary>
		/// 初始化 AbstractCondition 类实例
		/// </summary>
		protected AbstractCondition() : this(20) { }

		/// <summary>
		/// 初始化 AbstractCondition 类实例
		/// </summary>
		/// <param name="propertyCount">当前条件类中属性的数量。</param>
		protected AbstractCondition(int propertyCount)
			: base()
		{
			bitArray = new BitArray(propertyCount + 2, false);
			//pSortField = propertyCount;
			//pSortOrder = propertyCount + 1;
		}

		/// <summary>
		/// 清空当前条件模型中各属性的值。
		/// </summary>
		/// <returns></returns>
		public bool ClearValues()
		{
			foreach (EntityPropertyMeta propertyInfo in base.GetProperties())
			{
				propertyInfo.ResetValue(this);
			}
			ResetBitValue();
			return true;
		}

		/// <summary>
		/// 将 AbstractCondition 类中所有位置的位设置为False。
		/// </summary>
		public void ResetBitValue() { bitArray.SetAll(false); }

		/// <summary>
		///  反转当前 AbstractCondition 中的所有位值，以便将设置为 true 的元素更改为 false；
		///  将设置为 false 的元素更改为 true。
		/// </summary>
		public void BitNot() { bitArray.Not(); }

		/// <summary>
		/// 将 AbstractCondition 类中特定位置处的位设置为True。
		/// 此方法主要用在子类中更改属性值时更新。
		/// </summary>
		/// <param name="index">要设置的位的从零开始的索引。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">index 小于零。- 或 -index 大于或等于 propertyCount 中初始的元素数。</exception>
		protected void SetBitValue(int index) { bitArray.Set(index, true); }

		/// <summary>
		/// 将 AbstractCondition 中特定位置处的位设置为指定值。
		/// 此方法主要用在子类中更改属性值时更新。
		/// </summary>
		/// <param name="index">要设置的位的从零开始的索引。</param>
		/// <param name="value">要分配给位的布尔值。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">index 小于零。- 或 -index 大于或等于 propertyCount 中初始的元素数。</exception>
		protected void SetBitValue(int index, bool value) { bitArray.Set(index, value); }

		/// <summary>
		/// 获取 AbstractCondition 中特定位置处的位的值。
		/// </summary>
		/// <param name="index">要获取的值的从零开始的索引。</param>
		/// <returns>在 index 位置处的位的值。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">index 小于零。- 或 -index 大于或等于 propertyCount 中初始的元素数。</exception>
		protected bool HasValue(int index) { return bitArray.Get(index); }

		private string m_OrderText = null;
		/// <summary>
		/// 当前自定义排序字段
		/// </summary>
		[global::System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
		[global::Basic.EntityLayer.IgnorePropertyAttribute()]
		public string OrderText
		{
			get { return m_OrderText; }
			set { m_OrderText = value; base.OnPropertyChanged("OrderText"); }
		}

		private int m_TotalCount = 0;
		/// <summary>
		/// 确认当前是否为分页检索，而非新的查询检索。
		/// 如果 当前值为0，则为查询否则为分页。
		/// </summary>
		[global::System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
		[global::Basic.EntityLayer.IgnorePropertyAttribute()]
		public int TotalCount { get { return m_TotalCount; } set { m_TotalCount = value; } }

		/// <summary>
		/// 属性 OrderText 是否已更改。
		/// </summary>
		[global::Basic.EntityLayer.IgnorePropertyAttribute()]
		public bool HasOrderText { get { return string.IsNullOrWhiteSpace(m_OrderText) == false; } }

		private int m_PageIndex = 0;
		/// <summary>
		/// 当前页面索引
		/// </summary>
		[global::System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
		[global::Basic.EntityLayer.ColumnMapping("PAGEINDEX", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.IgnorePropertyAttribute()]
		public int PageIndex
		{
			get { return m_PageIndex; }
			set
			{
				if (m_PageIndex != value)
				{
					m_PageIndex = value;
					base.OnPropertyChanged("PageIndex");
				}
			}
		}

		private int m_PageSize = 0;
		/// <summary>
		/// 每页记录数量
		/// </summary>
		[global::System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
		[global::Basic.EntityLayer.ColumnMapping("PAGESIZE", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.IgnorePropertyAttribute()]
		public int PageSize
		{
			get { return m_PageSize; }
			set
			{
				if (m_PageSize != value)
				{
					m_PageSize = value;
					base.OnPropertyChanged("PageSize");
				}
			}
		}
	}
}
