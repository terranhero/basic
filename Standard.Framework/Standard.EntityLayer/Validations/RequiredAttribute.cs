using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Basic.EntityLayer;
using Basic.Messages;

namespace Basic.Validations
{
	/// <summary>
	/// 指定需要数据字段值。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class RequiredAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
	{
		/// <summary>
		/// 初始化 RequiredAttribute 类的新实例。
		/// </summary>
		public RequiredAttribute() : this(false) { }

		/// <summary>
		/// 使用带参参数，初始化 RequiredAttribute 类的新实例。
		/// </summary>
		/// <param name="allowEmpty">一个 boolean 类型的值，该值指示是否允许空字符串。</param>
		public RequiredAttribute(bool allowEmpty) : base() { AllowEmpty = allowEmpty; }

		/// <summary>
		/// 获取或设置一个布尔类型的值，该值指示如果属性类型是字符型的是否允许空字符串，
		/// 如果属性类型是数字类型的是否允许为零。
		/// 如果属性类型是GUID类型的是否允许为"00000000-0000-0000-0000-000000000000"。
		/// </summary>
		/// <value>如果允许空字符串或零，则为 true；否则为 false。默认值为 false。</value>
		public bool AllowEmpty { get; set; }

		/// <summary>
		/// 基于发生错误的数据字段对错误消息应用格式设置。
		/// </summary>
		/// <param name="validationContext"> 有关验证操作的上下文信息。</param>
		/// <returns>带有格式的错误消息的实例。</returns>
		private string FormatErrorMessage(ValidationContext validationContext)
		{
			GroupNameAttribute gna = (GroupNameAttribute)GetCustomAttribute(validationContext.ObjectType, typeof(GroupNameAttribute));
			if (gna != null)
			{
				string keyName = string.Concat(gna.Name, "_", validationContext.MemberName, "_Required");
				string value = MessageContext.GetString(gna.ConverterName, keyName, CultureInfo.CurrentUICulture);
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
			string[] memberNames = new string[] { validationContext.MemberName };
			if (value == null) { return new ValidationResult(this.FormatErrorMessage(validationContext), memberNames); }
			if (value is string && string.Equals(value, string.Empty) && !AllowEmpty) { return new ValidationResult(this.FormatErrorMessage(validationContext), memberNames); }
			else if (object.Equals(0, value) && !AllowEmpty) { return new ValidationResult(this.FormatErrorMessage(validationContext), memberNames); }
			else if (object.Equals(Guid.Empty, value) && !AllowEmpty) { return new ValidationResult(this.FormatErrorMessage(validationContext), memberNames); }
			return ValidationResult.Success;
		}
	}
}
