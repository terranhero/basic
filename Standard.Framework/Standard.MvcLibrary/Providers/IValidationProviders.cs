using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Basic.Collections;
using Basic.EntityLayer;
using Microsoft.AspNetCore.Html;
using BV = Basic.Validations;

namespace Basic.MvcLibrary
{
	/// <summary>表示实体模型验证提供者</summary>
	/// <typeparam name="T"></typeparam>
#if NET6_0_OR_GREATER
	public interface IValidationProviders<T> : IDisposable, IAsyncDisposable
#else
	public interface IValidationProviders<T> : IDisposable
#endif
	{
		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		IHtmlContent PropertyBluredFor<TR>(Expression<Func<T, TR>> expression);

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		IHtmlContent PropertyChangedFor<TR>(Expression<Func<T, TR>> expression);

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		IHtmlContent PropertyBluredFor<TR>(Expression<Func<T, TR>> expression, bool withComma);

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		IHtmlContent PropertyChangedFor<TR>(Expression<Func<T, TR>> expression, bool withComma);

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="ending">自定义输出结尾。</param>
		IHtmlContent PropertyBluredFor<TR>(Expression<Func<T, TR>> expression, string ending);

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="ending">自定义输出结尾。</param>
		IHtmlContent PropertyChangedFor<TR>(Expression<Func<T, TR>> expression, string ending);
	}

	/// <summary>表示实体模型验证提供者</summary>
	public class ValidationProviders<T> : IValidationProviders<T>, System.IDisposable
	{
		private readonly T _model;
		private readonly System.Globalization.CultureInfo _cultureInfo;
		private readonly EntityPropertyCollection mProperties;

		/// <summary>初始化 ValidationProvider 类实例</summary>
		public ValidationProviders(T model, System.Globalization.CultureInfo ci)
		{
			_model = model; _cultureInfo = ci;
			EntityPropertyProvidor.TryGetProperties(typeof(T), out mProperties);
		}

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		public IHtmlContent PropertyBluredFor<TR>(Expression<Func<T, TR>> expression, bool withComma)
		{
			string validation = GetPropertyValidation(expression, "blur", withComma ? "," : "");
			return validation != null ? new HtmlString(validation) : HtmlString.Empty;
		}

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		public IHtmlContent PropertyChangedFor<TR>(Expression<Func<T, TR>> expression, bool withComma)
		{
			string validation = GetPropertyValidation(expression, "change", withComma ? "," : "");
			return validation != null ? new HtmlString(validation) : HtmlString.Empty;
		}

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="ending">自定义输出结尾。</param>
		public IHtmlContent PropertyBluredFor<TR>(Expression<Func<T, TR>> expression, string ending)
		{
			string validation = GetPropertyValidation(expression, "blur", ending);
			return validation != null ? new HtmlString(validation) : HtmlString.Empty;
		}

		/// <summary>向当前视图文件输出属性的验证表达式</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="ending">自定义输出结尾。</param>
		public IHtmlContent PropertyChangedFor<TR>(Expression<Func<T, TR>> expression, string ending)
		{
			string validation = GetPropertyValidation(expression, "change", ending);
			return validation != null ? new HtmlString(validation) : HtmlString.Empty;
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public IHtmlContent PropertyBluredFor<TR>(Expression<Func<T, TR>> expression)
		{
			string validation = GetPropertyValidation(expression, "blur", "");
			return validation != null ? new HtmlString(validation) : HtmlString.Empty;
		}

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public IHtmlContent PropertyChangedFor<TR>(Expression<Func<T, TR>> expression)
		{
			string validation = GetPropertyValidation(expression, "change", "");
			return validation != null ? new HtmlString(validation) : HtmlString.Empty;
		}

		/// <summary></summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="expression"></param>
		/// <param name="trigger">验证触发器(change 或 blur)</param>
		/// <param name="ending">自定义输出结尾。</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0031:使用 null 传播", Justification = "<挂起>")]
		private string GetPropertyValidation<TR>(Expression<Func<T, TR>> expression, string trigger, string ending)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression != null ? memberExpression.Member.Name : null;
			if (mProperties.TryGetProperty(name, out EntityPropertyMeta meta))
			{
				List<string> messages = new List<string>(5);
				foreach (ValidationAttribute attribute in meta.Validations)
				{
					if (attribute is RequiredAttribute || attribute is BV.RequiredAttribute || attribute is BV.BoolRequiredAttribute)
					{
						string msg = GetSourceValue(meta, "Required");
						messages.Add($"{{required: true, message: \"{msg}\", trigger: \"{trigger}\"}}");
					}
					else if (attribute is StringLengthAttribute || attribute is BV.StringLengthAttribute)
					{
						StringLengthAttribute sla = attribute as StringLengthAttribute;
						string msg;
						if (sla.MinimumLength > 0)
							msg = GetSourceValue(meta, "StringLengthIncludingMinimum");
						else
							msg = GetSourceValue(meta, "StringLength");

						msg = string.Format(msg, sla.MaximumLength, sla.MinimumLength);

						messages.Add($"{{min: {sla.MinimumLength}, max: {sla.MaximumLength}, message: \"{msg}\", trigger: \"{trigger}\"}}");
					}
					else if (attribute is RangeAttribute || attribute is BV.RangeAttribute)
					{
						RangeAttribute ra = attribute as RangeAttribute;
						string msg = GetSourceValue(meta, "Range");
						if (meta.PropertyType == typeof(byte) || meta.PropertyType == typeof(short) || meta.PropertyType == typeof(int) || meta.PropertyType == typeof(long))
						{
							messages.Add($"{{min: {ra.Minimum}, max: {ra.Maximum}, type:\"integer\", message: \"{msg}\", trigger: \"{trigger}\"}}");
						}
						else if (meta.PropertyType == typeof(decimal) || meta.PropertyType == typeof(float) || meta.PropertyType == typeof(double))
						{
							messages.Add($"{{min: {ra.Minimum}, max: {ra.Maximum}, type:\"number\", message: \"{msg}\", trigger: \"{trigger}\"}}");
						}
						else
						{
							messages.Add($"{{min: {ra.Minimum}, max: {ra.Maximum}, message: \"{msg}\", trigger: \"{trigger}\"}}");
						}
					}
					else if (attribute is MaxLengthAttribute || attribute is BV.MaxLengthAttribute)
					{
						MaxLengthAttribute ra = attribute as MaxLengthAttribute;
						string msg = GetSourceValue(meta, "MaxLength");
						messages.Add($"{{max: {ra.Length}, message: \"{msg}\", trigger: \"{trigger}\"}}");
					}
					else if (attribute is RegularExpressionAttribute || attribute is BV.RegularExpressionAttribute)
					{
						RegularExpressionAttribute rea = attribute as RegularExpressionAttribute;
						string msg = GetSourceValue(meta, "RegularExpression");
						messages.Add($"{{pattern: /{rea.Pattern}/, message: \"{msg}\", trigger: \"{trigger}\"}}");
					}
				}
				if (messages.Count > 0) { return string.Concat(meta.Name, ":[", string.Join(",", messages), "]", ending); }
				return null;
			}
			return null;
		}

		/// <summary>获取资源值</summary>
		/// <param name="meta">属性元信息</param>
		/// <param name="suffix">验证资源后缀名称</param>
		private string GetSourceValue(EntityPropertyMeta meta, string suffix)
		{
			WebDisplayAttribute wda = meta.Display;
			if (wda != null)
			{
				string value1 = Messages.MessageContext.GetString(wda.ConverterName, string.Concat(wda.DisplayName, "_", suffix), _cultureInfo);
				return System.Web.HttpUtility.JavaScriptStringEncode(value1);
			}
			string converter = null; string className = meta.ContainerType.Name;
			GroupNameAttribute gna = meta.ContainerType.GetCustomAttribute<GroupNameAttribute>();
			if (gna != null) { className = gna.Name; converter = gna.ConverterName; }
			string value = Messages.MessageContext.GetString(converter, string.Concat(className, "_", meta.Name, "_", suffix), _cultureInfo);
			return System.Web.HttpUtility.JavaScriptStringEncode(value);
		}

		/// <summary></summary>
		void System.IDisposable.Dispose() { }

#if NET6_0_OR_GREATER
		/// <summary></summary>
		System.Threading.Tasks.ValueTask IAsyncDisposable.DisposeAsync() { return System.Threading.Tasks.ValueTask.CompletedTask; }
#endif
	}

}
