using Basic.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 实体模型属性的验证上下文信息
	/// </summary>
	internal sealed class PropertyValidationContext
	{
		private readonly AbstractEntity fieldEntity;
		private readonly EntityPropertyMeta fieldPropertyDescriptor;
		private readonly ValidationContext fieldValidationContext;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyDescriptor"></param>
		internal PropertyValidationContext(AbstractEntity entity, EntityPropertyMeta propertyDescriptor)
		{
			fieldEntity = entity; fieldPropertyDescriptor = propertyDescriptor;
			fieldValidationContext = new ValidationContext(entity, null, null);
			fieldValidationContext.MemberName = fieldPropertyDescriptor.Name;
			fieldValidationContext.DisplayName = fieldPropertyDescriptor.DisplayName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyDescriptor"></param>
		/// <param name="validationContext"></param>
		internal PropertyValidationContext(AbstractEntity entity, EntityPropertyMeta propertyDescriptor, ValidationContext validationContext)
		{
			fieldEntity = entity; fieldPropertyDescriptor = propertyDescriptor;
			fieldValidationContext = new ValidationContext(entity, validationContext, validationContext.Items);
			fieldValidationContext.MemberName = fieldPropertyDescriptor.Name;
			fieldValidationContext.DisplayName = fieldPropertyDescriptor.DisplayName;
		}

		/// <summary>
		/// 
		/// </summary>
		internal EntityPropertyMeta PropertyDescriptor { get { return fieldPropertyDescriptor; } }

		/// <summary>
		/// 
		/// </summary>
		internal ValidationContext ValidationContext { get { return fieldValidationContext; } }

		/// <summary>
		/// 检查当前属性的值对于当前的验证特性是否有效。
		/// </summary>
		public void ValidationProperty(ValidationEntityResult validationResults)
		{
			object value = fieldPropertyDescriptor.GetValue(fieldEntity);
			validationResults.Remove(fieldPropertyDescriptor.Name);
			foreach (ValidationAttribute validationAttribute in fieldPropertyDescriptor.Validations)
			{
				ValidationResult result = validationAttribute.GetValidationResult(value, fieldValidationContext);
				if (result != ValidationResult.Success)
				{
					ValidationPropertyResult propertyResult = null;
					if (!validationResults.TryGetValue(fieldPropertyDescriptor.Name, out propertyResult))
					{
						propertyResult = new ValidationPropertyResult(fieldEntity, fieldPropertyDescriptor.Name);
						validationResults.Add(propertyResult);
					}
					propertyResult.Add(result.ErrorMessage);
				}
			}
		}

	}
}
