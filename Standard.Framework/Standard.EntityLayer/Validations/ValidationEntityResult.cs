using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Collections;
using Basic.EntityLayer;

namespace Basic.Validations
{
	/// <summary>
	/// AbstractEntity 子类实体验证结果。
	/// </summary>
	public sealed class ValidationEntityResult : BaseCollection<ValidationPropertyResult>
	{
		private readonly AbstractEntity abstractEntity;
		internal ValidationEntityResult(AbstractEntity entity) { abstractEntity = entity; }

		/// <summary>
		/// 获取一个指示该实体是否有验证错误的值。
		/// </summary>
		/// <value>如果该实体当前有验证错误，则为 true；否则为 false。</value>
		public bool HasError { get { return this.Count > 0; } }

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="propertyName">要获取或设置的值的键</param>
		/// <returns>返回元素的键</returns>
		public bool Remove(string propertyName)
		{
			if (base.ContainsKey(propertyName))
			{
				ValidationPropertyResult item = base[propertyName];
				item.Clear(); if (item.Count == 0) { return base.Remove(item); }
			}
			return false;
		}


		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(ValidationPropertyResult item)
		{
			return item.PropertyName;
		}
	}
}
