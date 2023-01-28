using Basic.EntityLayer;
using Basic.Messages;
using BV = Basic.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义多语言模型验证提供者
	/// </summary>
	public sealed class CultureDataAnnotationsModelValidatorProvider : System.Web.Mvc.DataAnnotationsModelValidatorProvider
	{
		/// <summary>
		/// 注册自定义多语言模型验证提供者
		/// </summary>
		public static void RegisterCultureProvider()
		{
			ModelValidatorProviders.Providers.RemoveAt(0);
			ModelValidatorProviders.Providers.Insert(0, new CultureDataAnnotationsModelValidatorProvider());
		}

		/// <summary>
		/// 初始化RegularExpression验证项默认多语言消息
		/// </summary>
		/// <param name="metadata">元数据。</param>
		/// <param name="culInfo">区域信息</param>
		/// <param name="attribute">验证特性</param>
		private void InitializeRegularExpressionErrorMessage(ModelMetadata metadata, CultureInfo culInfo, RegularExpressionAttribute attribute)
		{
			string className = metadata.ContainerType.Name;
			string converterName = null;
			if (Attribute.IsDefined(metadata.ContainerType, typeof(GroupNameAttribute)))
			{
				GroupNameAttribute gna = (GroupNameAttribute)Attribute.GetCustomAttribute(metadata.ContainerType, typeof(GroupNameAttribute));
				className = string.Concat(gna.Name, "_", metadata.PropertyName);
			}
			if (metadata is CultureModelMetadata)
			{
				CultureModelMetadata cultureMetadata = metadata as CultureModelMetadata;
				if (cultureMetadata.WebDisplay != null)
				{
					converterName = cultureMetadata.WebDisplay.ConverterName;
					className = cultureMetadata.WebDisplay.DisplayName;
				}
			}
			string errorMsg = MessageContext.GetString(converterName, string.Concat(className, "_RegularExpression"), culInfo);
			if (!string.IsNullOrWhiteSpace(errorMsg))
				attribute.ErrorMessage = errorMsg;
		}

		/// <summary>
		/// 初始化RequiredAttribute验证项默认多语言消息
		/// </summary>
		/// <param name="metadata">元数据。</param>
		/// <param name="culInfo">区域信息</param>
		/// <param name="attribute">验证特性</param>
		private void InitializeRequiredErrorMessage(ModelMetadata metadata, CultureInfo culInfo, ValidationAttribute attribute)
		{
			string className = metadata.ContainerType.Name;
			string converterName = null;
			if (Attribute.IsDefined(metadata.ContainerType, typeof(GroupNameAttribute)))
			{
				GroupNameAttribute gna = (GroupNameAttribute)Attribute.GetCustomAttribute(metadata.ContainerType, typeof(GroupNameAttribute));
				className = string.Concat(gna.Name, "_", metadata.PropertyName);
			}
			if (metadata is CultureModelMetadata)
			{
				CultureModelMetadata cultureMetadata = metadata as CultureModelMetadata;
				if (cultureMetadata.WebDisplay != null)
				{
					converterName = cultureMetadata.WebDisplay.ConverterName;
					className = cultureMetadata.WebDisplay.DisplayName;
				}
			}
			string errorMsg = MessageContext.GetString(converterName, string.Concat(className, "_Required"), culInfo);
			if (!string.IsNullOrWhiteSpace(errorMsg))
				attribute.ErrorMessage = errorMsg;
		}

		/// <summary>
		/// 初始化StringLengthAttribute验证项默认多语言消息
		/// </summary>
		/// <param name="metadata">元数据。</param>
		/// <param name="culInfo">区域信息</param>
		/// <param name="attribute">验证特性</param>
		private void InitializeStringLengthErrorMessage(ModelMetadata metadata, CultureInfo culInfo, StringLengthAttribute attribute)
		{
			string className = metadata.ContainerType.Name;
			string converterName = null;
			if (Attribute.IsDefined(metadata.ContainerType, typeof(GroupNameAttribute)))
			{
				GroupNameAttribute gna = (GroupNameAttribute)Attribute.GetCustomAttribute(metadata.ContainerType, typeof(GroupNameAttribute));
				className = string.Concat(gna.Name, "_", metadata.PropertyName);
			}
			if (metadata is CultureModelMetadata)
			{
				CultureModelMetadata cultureMetadata = metadata as CultureModelMetadata;
				if (cultureMetadata.WebDisplay != null)
				{
					converterName = cultureMetadata.WebDisplay.ConverterName;
					className = cultureMetadata.WebDisplay.DisplayName;
				}
			}
			string key = null;
			if (attribute.MinimumLength > 0)
				key = string.Concat(className, "_StringLengthIncludingMinimum");
			else
				key = string.Concat(className, "_StringLength");
			string errorMsg = MessageContext.GetString(converterName, key, culInfo, attribute.MaximumLength, attribute.MinimumLength);

			if (!string.IsNullOrWhiteSpace(errorMsg))
				attribute.ErrorMessage = errorMsg;
		}

		/// <summary>
		/// 初始化RangeAttribute验证项默认多语言消息
		/// </summary>
		/// <param name="metadata">元数据。</param>
		/// <param name="culInfo">区域信息</param>
		/// <param name="attribute">验证特性</param>
		private void InitializeRangeMessage(ModelMetadata metadata, CultureInfo culInfo, RangeAttribute attribute)
		{
			string className = metadata.ContainerType.Name;
			string converterName = null; string propertyName = metadata.PropertyName;
			if (Attribute.IsDefined(metadata.ContainerType, typeof(GroupNameAttribute)))
			{
				GroupNameAttribute gna = (GroupNameAttribute)Attribute.GetCustomAttribute(metadata.ContainerType, typeof(GroupNameAttribute));
				className = string.Concat(gna.Name, "_", metadata.PropertyName);
			}
			if (metadata is CultureModelMetadata)
			{
				CultureModelMetadata cultureMetadata = metadata as CultureModelMetadata;
				if (cultureMetadata.WebDisplay != null)
				{
					converterName = cultureMetadata.WebDisplay.ConverterName;
					className = cultureMetadata.WebDisplay.DisplayName;
				}
			}
			string key = string.Concat(className, "_Range");
			string errorMsg = MessageContext.GetString(converterName, key, culInfo, attribute.Minimum, attribute.Maximum);
			if (!string.IsNullOrWhiteSpace(errorMsg))
				attribute.ErrorMessage = errorMsg;
		}

		/// <summary>
		/// 初始化DataTypeAttribute验证项默认多语言消息
		/// </summary>
		/// <param name="metadata">元数据。</param>
		/// <param name="culInfo">区域信息</param>
		/// <param name="attribute">验证特性</param>
		private void InitializeDataTypeMessage(ModelMetadata metadata, CultureInfo culInfo, DataTypeAttribute attribute)
		{
			string className = metadata.ContainerType.Name;
			string converterName = null;
			if (Attribute.IsDefined(metadata.ContainerType, typeof(GroupNameAttribute)))
			{
				GroupNameAttribute gna = (GroupNameAttribute)Attribute.GetCustomAttribute(metadata.ContainerType, typeof(GroupNameAttribute));
				className = string.Concat(gna.Name, "_", metadata.PropertyName);
			}
			if (metadata is CultureModelMetadata)
			{
				CultureModelMetadata cultureMetadata = metadata as CultureModelMetadata;
				if (cultureMetadata.WebDisplay != null)
				{
					converterName = cultureMetadata.WebDisplay.ConverterName;
					className = cultureMetadata.WebDisplay.DisplayName;
				}
			}
			string errorMsg = MessageContext.GetString(converterName, string.Concat(className, "_DataType"), culInfo);
			if (!string.IsNullOrWhiteSpace(errorMsg))
				attribute.ErrorMessage = errorMsg;
		}

		/// <summary>
		/// 初始化WebCompareAttribute验证项默认多语言消息
		/// </summary>
		/// <param name="metadata">元数据。</param>
		/// <param name="culInfo">区域信息</param>
		/// <param name="attribute">验证特性</param>
		private void InitializeCompareMessage(ModelMetadata metadata, CultureInfo culInfo, ValidationAttribute attribute)
		{
			string className = metadata.ContainerType.Name;
			string converterName = null;
			if (Attribute.IsDefined(metadata.ContainerType, typeof(GroupNameAttribute)))
			{
				GroupNameAttribute gna = (GroupNameAttribute)Attribute.GetCustomAttribute(metadata.ContainerType, typeof(GroupNameAttribute));
				className = string.Concat(gna.Name, "_", metadata.PropertyName);
			}
			if (metadata is CultureModelMetadata)
			{
				CultureModelMetadata cultureMetadata = metadata as CultureModelMetadata;
				if (cultureMetadata.WebDisplay != null)
				{
					converterName = cultureMetadata.WebDisplay.ConverterName;
					className = cultureMetadata.WebDisplay.DisplayName;
				}
			}
			string errorMsg = MessageContext.GetString(converterName, string.Concat(className, "_Compare"), culInfo);
			if (!string.IsNullOrWhiteSpace(errorMsg))
				attribute.ErrorMessage = errorMsg;
		}

		/// <summary>
		/// 获取验证程序的列表。
		/// </summary>
		/// <param name="metadata">元数据。</param>
		/// <param name="context">上下文。</param>
		/// <param name="attributes"> 验证特性的列表。</param>
		/// <returns> 验证程序的列表。</returns>
		protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
		{
			CultureInfo culInfo = MessageCulture.GetCultureInfo(context.HttpContext.Request);
			foreach (ValidationAttribute attribute in attributes.OfType<ValidationAttribute>())
			{
				if (attribute is RequiredAttribute || attribute is BV.RequiredAttribute || attribute is BV.BoolRequiredAttribute)
					InitializeRequiredErrorMessage(metadata, culInfo, attribute);
				else if (attribute is BV.RequiredAttribute || attribute is BV.BoolRequiredAttribute)
					InitializeRequiredErrorMessage(metadata, culInfo, attribute);
				else if (attribute is StringLengthAttribute || attribute is BV.StringLengthAttribute)
					InitializeStringLengthErrorMessage(metadata, culInfo, attribute as StringLengthAttribute);
				else if (attribute is DataTypeAttribute || attribute is BV.DataTypeAttribute || attribute is BV.DataTypeAttribute)
					InitializeDataTypeMessage(metadata, culInfo, attribute as DataTypeAttribute);
				else if (attribute is RangeAttribute || attribute is BV.RangeAttribute || attribute is BV.RangeAttribute)
					InitializeRangeMessage(metadata, culInfo, attribute as RangeAttribute);
				else if (attribute is BV.CompareAttribute || attribute is BV.CompareAttribute)
					InitializeCompareMessage(metadata, culInfo, attribute);
				else if (attribute is BV.RegularExpressionAttribute || attribute is RegularExpressionAttribute || attribute is BV.RegularExpressionAttribute)
					InitializeRegularExpressionErrorMessage(metadata, culInfo, attribute as RegularExpressionAttribute);
			}
			IEnumerable<ModelValidator> result = base.GetValidators(metadata, context, attributes);
			return result;
		}
	}
}
