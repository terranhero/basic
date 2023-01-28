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
	/// 指定数据字段中允许的最小和最大字符长度。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class StringLengthAttribute : System.ComponentModel.DataAnnotations.StringLengthAttribute
	{
		/// <summary>
		/// 使用指定的最大长度初始化 WebStringLengthAttribute  类的新实例。
		/// </summary>
		/// <param name="maximumLength">字符串的最大长度。</param>
		public StringLengthAttribute(int maximumLength) : base(maximumLength) { }

		/// <summary>
		/// 使用指定的最大、最小长度初始化 WebStringLengthAttribute  类的新实例。
		/// </summary>
		/// <param name="maximumLength">字符串的最大长度。</param>
		/// <param name="minimumLength">字符串的最小长度。</param>
		public StringLengthAttribute(int maximumLength, int minimumLength)
			: base(maximumLength)
		{
			base.MinimumLength = minimumLength;
		}
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
				string keyName = string.Concat(gna.Name, "_", validationContext.MemberName, "_StringLength");
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
