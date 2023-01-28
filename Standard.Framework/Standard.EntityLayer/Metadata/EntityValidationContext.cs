using Basic.EntityLayer;
using Basic.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Collections
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class EntityValidationContext : BaseCollection<PropertyValidationContext>
	{
		private readonly AbstractEntity fieldEntity;
		/// <summary>
		/// 初始化 EntityValidationContext&gt;T&lt; 类的新实例。
		/// </summary>
		internal EntityValidationContext(AbstractEntity entity) : base() { fieldEntity = entity; }

		/// <summary>
		/// 检查当前属性的值对于当前的验证特性是否有效。
		/// </summary>
		/// <returns></returns>
		public bool Validation(ValidationEntityResult validationResults)
		{
			foreach (PropertyValidationContext item in base.Items)
			{
				item.ValidationProperty(validationResults);
			}
			return fieldEntity.HasError();
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(PropertyValidationContext item)
		{
			return item.PropertyDescriptor.Name;
		}
	}
}
