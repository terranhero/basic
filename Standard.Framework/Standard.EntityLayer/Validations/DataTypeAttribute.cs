using Basic.EntityLayer;
using Basic.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Basic.Validations
{
	/// <summary>
	/// 指定要与数据字段关联的附加类型的名称。
	/// </summary>
	public class DataTypeAttribute : System.ComponentModel.DataAnnotations.DataTypeAttribute
	{
		/// <summary>
		///  使用指定的类型名称初始化 WebDataTypeAttribute  类的新实例。
		/// </summary>
		/// <param name="dataType">要与数据字段关联的类型的名称。</param>
		public DataTypeAttribute(DataType dataType) : base(dataType) { }

		/// <summary>
		/// 使用指定的字段模板名称初始化 WebDataTypeAttribute 类的新实例。
		/// </summary>
		/// <param name="customDataType">要与数据字段关联的自定义字段模板的名称。</param>
		/// <exception cref="System.ArgumentException">customDataType 为 null 或空字符串 ("")。</exception>
		public DataTypeAttribute(string customDataType) : base(customDataType) { }

		/// <summary>
		/// 基于发生错误的数据字段对错误消息应用格式设置。
		/// </summary>
		/// <param name="validationContext"> 有关验证操作的上下文信息。</param>
		/// <returns>带有格式的错误消息的实例。</returns>
		private string FormatErrorMessage(ValidationContext validationContext)
		{
			GroupNameAttribute gna = (GroupNameAttribute)Attribute.GetCustomAttribute(validationContext.ObjectType, typeof(GroupNameAttribute));
			if (gna != null)
			{
				string keyName = string.Concat(gna.Name, "_", validationContext.MemberName, "_DataType");
				string value = MessageContext.GetString(gna.ConverterName, keyName, CultureInfo.CurrentCulture);
				if (value != keyName) { return value; }
			}
			return base.FormatErrorMessage(validationContext.DisplayName);
		}

		/// <summary>
		/// 根据当前的验证特性来验证指定的值。
		/// </summary>
		/// <param name="value">要验证的值。</param>
		/// <param name="validationContext">有关验证操作的上下文信息。</param>
		/// <returns> System.ComponentModel.DataAnnotations.ValidationResult 类的实例。</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			ValidationResult result = base.IsValid(value, validationContext);
			if (result != ValidationResult.Success)
			{
				string[] memberNames = new string[] { validationContext.MemberName };
				return new ValidationResult(this.FormatErrorMessage(validationContext), memberNames);
			}
			return result;
		}
	}
}
