using Basic.EntityLayer;
using Basic.Messages;
using Basic.Properties;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Basic.Validations
{
	/// <summary>
	/// 表示数组或字符串允许的最大长度的特性。
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class MaxLengthAttribute : System.ComponentModel.DataAnnotations.MaxLengthAttribute
	{
		/// <summary>初始化 MaxLengthAttribute 类实例</summary>
		/// <param name="length">数组或字符串允许的最大长度，值必须大于零。 </param>
		public MaxLengthAttribute(int length) : base(length) { }

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
				string keyName = string.Concat(gna.Name, "_", validationContext.MemberName, "_MaxLength");
				string value = MessageContext.GetString(gna.ConverterName, keyName, CultureInfo.CurrentUICulture);
				if (value != keyName) { return value; }
			}
			return string.Format(Strings.MaxAttribute_Invalid, validationContext.DisplayName, this.Length);
		}

		/// <summary>
		/// 根据当前的验证特性来验证指定的值。
		/// </summary>
		/// <param name="value">要验证的值。</param>
		/// <param name="validationContext">有关验证操作的上下文信息。</param>
		/// <returns> System.ComponentModel.DataAnnotations.ValidationResult 类的实例。</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			string[] memberNames = new string[] { validationContext.MemberName };
			if (this.Length <= 0) { throw new InvalidOperationException(Strings.MaxLengthAttribute_InvalidMaxLength); }
			if (value == null) { return ValidationResult.Success; }
			int length = 0; string str = value as string;
			if (str != null) { length = str.Length; }
			else { length = ((Array)value).Length; }
			if (length > this.Length) { return new ValidationResult(this.FormatErrorMessage(validationContext), memberNames); }
			return ValidationResult.Success;
		}
	}
}
