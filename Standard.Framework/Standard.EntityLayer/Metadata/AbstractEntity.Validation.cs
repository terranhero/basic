using Basic.Collections;
using Basic.Properties;
using Basic.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示实体模型基类关于特性验证的扩展
	/// </summary>
	public abstract partial class AbstractEntity : IDataErrorInfo
	{
		private readonly ValidationEntityResult _ValidationResult;
		private readonly EntityValidationContext fieldEntityValidation;

		/// <summary>
		/// 设置当前实体是否需要验证信息
		/// </summary>
		/// <param name="enabled">是否启用实体类中 IDataErrorInfo 的验证特性</param>
		public void SetEnabledValidation(bool enabled = false) { m_EnabledValidation = enabled; }

		private static bool m_GlobalEnabledValidation = false;
		/// <summary>
		/// 在全局范围内是否启用实体模型验证功能。
		/// </summary>
		public static bool GlobalEnabledValidation
		{
			get { return m_GlobalEnabledValidation; }
			set { m_GlobalEnabledValidation = value; }
		}

		#region 异常信息
		/// <summary>
		/// 获取或设置当前实体的命令执行后的异常信息
		/// </summary>
		/// <value>一个 string 类型的值。表示当前实体的命令执行后的异常信息。</value>
		[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		[System.ComponentModel.Browsable(false)]
		public string GetErrorMessage()
		{
			if (HasError())
				return string.Join(Environment.NewLine, _ValidationResult.Select(r => r.ErrorMessage).ToArray());
			return null;
		}

		#endregion

		#region 实体模型错误信息


		/// <summary>
		/// 获取一个指示该实体是否有验证错误的值。
		/// </summary>
		/// <value>如果该实体当前有验证错误，则为 true；否则为 false。</value>
		public bool HasError() { return _ValidationResult.Count > 0; }

		/// <summary>
		/// 清除当前实体验证结果所有错误。
		/// </summary>
		public void ClearError() { _ValidationResult.Clear(); }

		/// <summary>
		/// 将异常添加到当前实体的异常集合中。
		/// </summary>
		/// <param name="errorMessage">异常信息</param>
		public void AddError(string errorMessage) { AddError(null, errorMessage); }

		///// <summary>
		///// 将异常添加到当前实体的异常集合中。
		///// </summary>
		///// <param name="errors">异常信息集合</param>
		//public void AddError(IEnumerable<string> errors) { AddError(null, errors); }

		/// <summary>
		/// 将异常添加到当前实体的异常集合中。
		/// </summary>
		/// <returns>返回实体验证异常信息。</returns>
		public IEnumerable<ValidationPropertyResult> GetError() { return _ValidationResult; }

		/// <summary>
		/// 将异常添加到当前实体的异常集合中。
		/// </summary>
		/// <param name="propertyName">属性名称</param>
		/// <param name="errorMessage">异常信息</param>
		public ValidationPropertyResult AddError(string propertyName, string errorMessage)
		{
			if (m_EnabledValidation == null && !m_GlobalEnabledValidation) { return null; }
			if (m_EnabledValidation.HasValue && !m_EnabledValidation.Value) { return null; }
			if (string.IsNullOrEmpty(propertyName)) { propertyName = NullPropertyName; }
			ValidationPropertyResult propertyResult = null;
			if (!_ValidationResult.TryGetValue(propertyName, out propertyResult))
			{
				propertyResult = new ValidationPropertyResult(this, propertyName);
				_ValidationResult.Add(propertyResult);
			}
			propertyResult.Add(errorMessage, true);
			OnPropertyChanged(propertyName);
			return propertyResult;
		}

		//		/// <summary>
		//		/// 将异常添加到当前实体的异常集合中。
		//		/// </summary>
		//		/// <param name="propertyName">属性名称</param>
		//		/// <param name="errors">异常信息集合</param>
		//		public ValidationPropertyResult AddError(string propertyName, IEnumerable<string> errors)
		//		{
		//#if(NET40)
		//			if (m_EnabledValidation == null && !m_GlobalEnabledValidation) { return null; }
		//#endif
		//			if (m_EnabledValidation.HasValue && !m_EnabledValidation.Value) { return null; }
		//			if (string.IsNullOrEmpty(propertyName)) { propertyName = NullPropertyName; }
		//			ValidationPropertyResult propertyResult = null;
		//			if (!_ValidationResult.TryGetValue(propertyName, out propertyResult))
		//			{
		//				propertyResult = new ValidationPropertyResult(this, propertyName);
		//				_ValidationResult.Add(propertyResult);
		//			}
		//			propertyResult.AddRange(errors);
		//			OnPropertyChanged(propertyName);
		//			return propertyResult;
		//		}
		#endregion

		#region 接口 IDataErrorInfo 的实现
		/// <summary>
		/// 获取指示对象何处出错的错误信息。
		/// </summary>
		/// <value>指示对象何处出错的错误信息。默认值为空字符串 ("")。</value>
		string IDataErrorInfo.Error { get { return string.Empty; } }

		/// <summary>
		/// 获取具有给定名称的属性的错误信息。
		/// </summary>
		/// <param name="propertyName">要获取其错误信息的属性的名称。</param>
		/// <returns>该属性的错误信息。默认值为空字符串 ("")。</returns>
		string IDataErrorInfo.this[string propertyName]
		{
			get
			{
				if (m_EnabledValidation.HasValue && m_EnabledValidation.Value) { return string.Empty; }
				if (m_EnabledValidation.HasValue == false && m_GlobalEnabledValidation)
				{
					if (ValidationProperty(propertyName)) { return string.Empty; }
					return string.Join(Environment.NewLine, _ValidationResult[propertyName]);
				}
				return string.Empty;
			}
		}
		#endregion

		/// <summary>
		/// 验证所有属性的值，确定指定的对象是否有效。
		/// </summary>
		/// <returns>如果对象有效，则为 true；否则为 false。</returns>
		public bool Validation()
		{
			this.ClearError();
			ValidationContext validationContext = new ValidationContext(this, null, null);
			if (fieldEntityValidation.Count == 0)
			{
				foreach (EntityPropertyMeta property in _EntityProperties)
				{
					fieldEntityValidation.Add(new PropertyValidationContext(this, property, validationContext));
				}
			}
			fieldEntityValidation.Validation(_ValidationResult);
			return _ValidationResult.Count == 0;
		}

		/// <summary>
		/// 验证属性的值，确定指定的对象是否有效。
		/// </summary>
		/// <param name="propertyName">当前对象的包含的属性名称。</param>
		/// <returns>如果对象有效，则为 true；否则为 false。</returns>
		public bool ValidationProperty(string propertyName)
		{
			if (fieldEntityValidation.ContainsKey(propertyName))
			{
				PropertyValidationContext propertyValidation = fieldEntityValidation[propertyName];
				propertyValidation.ValidationProperty(_ValidationResult);
				return !_ValidationResult.ContainsKey(propertyName);
			}
			EntityPropertyMeta property = null;
			if (_EntityProperties.TryGetProperty(propertyName, out property))
			{
				PropertyValidationContext propertyValidation = new PropertyValidationContext(this, property);
				fieldEntityValidation.Add(propertyValidation);
				propertyValidation.ValidationProperty(_ValidationResult);
				return !_ValidationResult.ContainsKey(propertyName);
			}
			return true;
		}


		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <param name="info">需要从 XmlReader 流中读取值的EntityPropertyDescriptor。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected virtual bool ReadProperty(System.Xml.XmlReader reader, EntityPropertyMeta info)
		{
			ValidationPropertyResult propertyError = null;
			if (_ValidationResult.ContainsKey(info.Name))
				propertyError = _ValidationResult[info.Name];
			else
				propertyError = new ValidationPropertyResult(this, info.Name);
			propertyError.ReadXml(reader);
			return true;
		}
	}
}
