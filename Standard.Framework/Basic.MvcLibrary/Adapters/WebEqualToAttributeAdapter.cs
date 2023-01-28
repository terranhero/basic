using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Basic.Validations.Adapters
{
	/// <summary>
	/// 提供 WebEqualToAttribute 特性的适配器。
	/// </summary>
	public class WebEqualToAttributeAdapter : DataAnnotationsModelValidator<Basic.Validations.EqualToAttribute>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="metadata"></param>
		/// <param name="context"></param>
		/// <param name="attribute"></param>
		public WebEqualToAttributeAdapter(ModelMetadata metadata, ControllerContext context, Basic.Validations.EqualToAttribute attribute)
			: base(metadata, context, attribute)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static string FormatPropertyForClientValidation(string property)
		{
			if (property == null)
			{
				throw new ArgumentException("Value cannot be null or empty.", "property");
			}
			return ("*." + property);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
		{
			base.Attribute.OtherPropertyDisplayName = this.GetOtherPropertyDisplayName();
			string other = FormatPropertyForClientValidation(base.Attribute.OtherProperty);
			return new ModelClientValidationEqualToRule[] { new ModelClientValidationEqualToRule(base.ErrorMessage, other) };
		}

		private string GetOtherPropertyDisplayName()
		{
			Func<object> modelAccessor = null;
			if ((base.Metadata.ContainerType == null) || string.IsNullOrEmpty(base.Attribute.OtherProperty))
			{
				return base.Attribute.OtherProperty;
			}
			if (modelAccessor == null)
			{
				modelAccessor = () => base.Metadata.Model;
			}
			return ModelMetadataProviders.Current.GetMetadataForProperty(modelAccessor, base.Metadata.ContainerType, base.Attribute.OtherProperty).GetDisplayName();
		}

	}
}
