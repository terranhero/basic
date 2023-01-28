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
	/// <summary></summary>
	public static class ValidationProvider
	{
		/// <summary>初始化 Toolbar 类实例</summary>
		public static IValidationProvider<T> ValidationInfo<T>(this IBasicContext<T> basic)
		{
			return new ValidationProvider<T>(basic);
		}
	}

	/// <summary>表示实体模型验证提供者</summary>
	public interface IValidationProvider<T> : System.IDisposable
	{
		/// <summary>表示上下文信息</summary>
		IBasicContext Basic { get; }

		/// <summary>向当前视图文件输出字符串</summary>
		/// <param name="value">需要输出的字符串</param>
		void WriteString(string value);

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

		/// <summary>向当前视图文件输出字符串</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		void PropertyBlured<TR>(Expression<Func<T, TR>> expression);

		/// <summary>向当前视图文件输出字符串</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		void PropertyChanged<TR>(Expression<Func<T, TR>> expression);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		void PropertyBlured<TR>(Expression<Func<T, TR>> expression, bool withComma);

		/// <summary>向当前视图文件输出指定属性</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		void PropertyChanged<TR>(Expression<Func<T, TR>> expression, bool withComma);

		/// <summary>向当前视图文件输出所有属性</summary>
		void Properties();
	}

	/// <summary>表示实体模型验证提供者</summary>
	public sealed class ValidationProvider<T> : IValidationProvider<T>, System.IDisposable
	{
		private readonly IBasicContext basicContext;
		private readonly EntityPropertyCollection mProperties;

		/// <summary>初始化 ValidationProvider 类实例</summary>
		internal ValidationProvider(IBasicContext bc)
		{
			basicContext = bc;
			EntityPropertyProvidor.TryGetProperties(typeof(T), out mProperties);
		}

		/// <summary>表示上下文信息</summary>
		public IBasicContext Basic { get { return basicContext; } }

		/// <summary>向当前视图文件输出字符串</summary>
		/// <param name="value">需要输出的字符串</param>
		public void WriteString(string value) { basicContext.Write(value); }

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

		/// <summary>向当前视图文件输出属性验证</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public void PropertyBlured<TR>(Expression<Func<T, TR>> expression) { PropertyBlured(expression, false); }

		/// <summary>向当前视图文件输出属性验证，并在结尾添加逗号</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		public void PropertyBlured<TR>(Expression<Func<T, TR>> expression, bool withComma)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression != null ? memberExpression.Member.Name : null;
			if (mProperties.TryGetProperty(name, out EntityPropertyMeta meta))
			{
				Property(meta, "blur", withComma);
			}
		}

		/// <summary>向当前视图文件输出属性验证</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		public void PropertyChanged<TR>(Expression<Func<T, TR>> expression) { PropertyChanged(expression, false); }

		/// <summary>向当前视图文件输出属性验证，并在结尾添加逗号</summary>
		/// <param name="expression">需要输出的Lambda表达式。</param>
		/// <param name="withComma">是否输出逗号。</param>
		public void PropertyChanged<TR>(Expression<Func<T, TR>> expression, bool withComma)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression != null ? memberExpression.Member.Name : null;
			if (mProperties.TryGetProperty(name, out EntityPropertyMeta meta))
			{
				Property(meta, "change", withComma);
			}
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

		/// <summary>输出属性验证信息</summary>
		/// <param name="meta"></param>
		/// <param name="trigger">表示出发验证的事件(change/blur)</param>
		/// <param name="withComma"></param>
		private void Property(EntityPropertyMeta meta, string trigger, bool withComma)
		{
			if (meta.Validations.Count == 0) { return; }
			List<string> messages = new List<string>(5);
			basicContext.Write(string.Concat(meta.Name, ":["));

			foreach (ValidationAttribute attribute in meta.Validations)
			{
				if (attribute is RequiredAttribute || attribute is BV.RequiredAttribute || attribute is BV.BoolRequiredAttribute)
				{
					string msg = GetSourceValue(meta, "Required");
					messages.Add(string.Concat($"{{required: true, message: \"{msg}\", trigger: \"{trigger}\"}}"));
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

					messages.Add(string.Concat($"{{min: {sla.MinimumLength}, max: {sla.MaximumLength}, message: \"{msg}\", trigger: \"{trigger}\"}}"));
				}
				//else if (attribute is DataTypeAttribute || attribute is BV.DataTypeAttribute || attribute is BV.DataTypeAttribute)
				//	InitializeDataTypeMessage(metadata, culInfo, attribute as DataTypeAttribute);
				else if (attribute is RangeAttribute || attribute is BV.RangeAttribute)
				{
					RangeAttribute ra = attribute as RangeAttribute;
					string msg = GetSourceValue(meta, "Range");
					messages.Add(string.Concat($"{{min: {ra.Minimum}, max: {ra.Maximum}, message: \"{msg}\", trigger: \"{trigger}\"}}"));
				}
				//else if (attribute is BV.CompareAttribute || attribute is BV.CompareAttribute)
				//	InitializeCompareMessage(metadata, culInfo, attribute);
				else if (attribute is RegularExpressionAttribute || attribute is BV.RegularExpressionAttribute)
				{
					RegularExpressionAttribute rea = attribute as RegularExpressionAttribute;
					string msg = GetSourceValue(meta, "RegularExpression");
					messages.Add(string.Concat($"{{pattern: /{rea.Pattern}/, message: \"{msg}\", trigger: \"{trigger}\"}}"));
				}
			}
			basicContext.Write(string.Join(",", messages));
			basicContext.Write("]"); basicContext.WriteLine(withComma == true ? "," : "");
		}

		/// <summary>获取资源值</summary>
		/// <param name="meta">属性元信息</param>
		/// <param name="suffix">验证资源后缀名称</param>
		private string GetSourceValue(EntityPropertyMeta meta, string suffix)
		{
			WebDisplayAttribute wda = meta.Display;
			if (wda != null)
			{
				string value1 = basicContext.GetString(wda.ConverterName, string.Concat(wda.DisplayName, "_", suffix));
				return System.Web.HttpUtility.JavaScriptStringEncode(value1);
			}
			string converter = null; string className = meta.ContainerType.Name;
			GroupNameAttribute gna = meta.ContainerType.GetCustomAttribute<GroupNameAttribute>();
			if (gna != null) { className = gna.Name; converter = gna.ConverterName; }
			string value = basicContext.GetString(converter, string.Concat(className, "_", meta.Name, "_", suffix));
			return System.Web.HttpUtility.JavaScriptStringEncode(value);
		}

		/// <summary>向当前视图文件输出所有属性</summary>
		public void Properties()
		{
			int count = mProperties.Count; int index = 1;
			foreach (EntityPropertyMeta meta in mProperties)
			{
				if (meta.IsReadOnly) { continue; }
				else if (meta.Ignore) { continue; }
				else if (basicContext.Model != null)
				{
					object obj = meta.GetValue(basicContext.Model);
					if (string.IsNullOrWhiteSpace(meta.DisplayFormatString) == false)
					{
						string str = string.Format(meta.DisplayFormatString, obj);
						basicContext.Write(string.Concat(meta.Name, ":\"", str, "\""));
					}
					else if (meta.PropertyType.IsEnum)
					{
						basicContext.Write(string.Concat(meta.Name, ":\"", Convert.ToInt32(obj), "\""));
					}
					else if (meta.PropertyType == typeof(DateTime))
					{
						DateTime dt = Convert.ToDateTime(obj);
						if (dt.Second == 0 && dt.Minute == 0 && dt.Hour == 0)
						{
							basicContext.Write(string.Format("{0}:\"{1:yyyy-MM-dd}\"", meta.Name, dt));
						}
						else { basicContext.Write(string.Concat(meta.Name, ":\"", obj, "\"")); }
					}
					else
					{
						basicContext.Write(string.Concat(meta.Name, ":\"", obj, "\""));
					}
					if (index != count) { basicContext.WriteLine(","); index++; }
					else { basicContext.WriteLine(); }
				}
				else
				{
					basicContext.Write(string.Concat(meta.Name, ":\"\""));
					if (index != count) { basicContext.WriteLine(","); index++; }
					else { basicContext.WriteLine(); }
				}
			}
		}

		/// <summary></summary>
		void System.IDisposable.Dispose()
		{
		}
	}
}
