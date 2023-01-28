using Basic.EntityLayer;
using Basic.Messages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Basic.Validations
{
	/// <summary>
	/// 指定数据字段中允许的中国居民身份证号码。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class IdentityCardAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
	{
		/// <summary>
		/// 初始化 WebIdentityCardAttribute 类的新实例。
		/// </summary>
		public IdentityCardAttribute() : this(false) { }

		/// <summary>
		/// 使用带参参数，初始化 WebIdentityCardAttribute 类的新实例。
		/// </summary>
		/// <param name="allowEmpty">一个 boolean 类型的值，该值指示是否允许空字符串。</param>
		public IdentityCardAttribute(bool allowEmpty) : base() { AllowEmpty = allowEmpty; }

		/// <summary>
		/// 获取或设置一个布尔类型的值，该值指示是否允许空字符串。
		/// </summary>
		/// <value>如果允许空字符串或零，则为 true；否则为 false。默认值为 false。</value>
		public bool AllowEmpty { get; private set; }

		/// <summary>
		/// 确定对象的指定值是否有效。
		/// </summary>
		/// <param name="value">要验证的对象的值。</param>
		/// <returns>如果指定的值有效，则为 true；否则，为 false。</returns>
		public override bool IsValid(object value)
		{
			if (value == null) { return false; }
			string str = value as string;
			if (this.AllowEmpty && str == string.Empty) { return true; }
			else if (!this.AllowEmpty && str == string.Empty) { return false; }
			else if (!string.IsNullOrEmpty(str))
			{
				if (str.Length != 15 || str.Length != 18) { return false; }
				if (str.Length == 15) { return CheckIDCard15(str); }
				else { return CheckIDCard18(str); }
			}
			return false;
		}

		/// <summary>  
		/// 18位身份证号码验证  
		/// </summary>  
		private bool CheckIDCard18(string idNumber)
		{
			//不符合18位中国居民身份证要求。
			if (!Regex.IsMatch(idNumber, @"^\d{17}(\d|X|x)$")) { return false; }
			//提取身份证出生日期
			int birthYear = Convert.ToInt32(idNumber.Substring(6, 4));
			int birthMonth = Convert.ToInt32(idNumber.Substring(10, 2));
			int birthDay = Convert.ToInt32(idNumber.Substring(12, 2));
			if (birthYear > DateTime.Today.Year) { return false; }
			if (birthMonth > 12) { return false; }
			if (birthDay <= 0 || birthDay > DateTime.DaysInMonth(birthYear, birthMonth)) { return false; }

			int[] weightingFactor = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
			char[] varifyCode = new char[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

			char[] idCharArray = idNumber.ToUpper().ToCharArray();
			int sum = 0;
			for (int index = 0; index < 17; index++)
			{
				sum += ((int)idCharArray[index] - 48) * weightingFactor[index];
			}
			char lastChar = varifyCode[sum % 11];
			return lastChar == idCharArray[17];
		}

		/// <summary>  
		/// 18位身份证号码验证  
		/// </summary>  
		private bool CheckIDCard15(string idNumber)
		{
			//不符合18位中国居民身份证要求。
			if (!Regex.IsMatch(idNumber, @"^\d{15}$")) { return false; }
			//提取身份证出生日期
			int birthYear = 1900 + Convert.ToInt32(idNumber.Substring(6, 2));
			int birthMonth = Convert.ToInt32(idNumber.Substring(8, 2));
			int birthDay = Convert.ToInt32(idNumber.Substring(10, 2));
			if (birthYear > DateTime.Today.Year) { return false; }
			if (birthMonth > 12) { return false; }
			if (birthDay <= 0 || birthDay > DateTime.DaysInMonth(birthYear, birthMonth)) { return false; }
			return true;
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
				string keyName = string.Concat(gna.Name, "_", validationContext.MemberName, "_IdentityCard");
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
