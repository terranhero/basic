using System;
using System.Linq.Expressions;
using System.Web;
using Basic.Collections;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>输入控件输出辅助类</summary>
	public abstract class InputProvider<TM> : Options
	{
		private readonly IBasicContext basicContext;

		/// <summary>输入控件输出辅助类</summary>
		protected InputProvider(IBasicContext bh) { basicContext = bh; }

		/// <summary>表示输出流上下文信息</summary>
		public IBasicContext Basic { get { return basicContext; } }


		private string vModel = string.Empty;
		/// <summary>数据源。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public InputProvider<TM> Model(string value) { vModel = value; return this; }

		/// <summary>生成显示文本</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public string TextFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				WebDisplayAttribute wda = meta.Display;
				if (wda == null) { return string.Empty; }
				return basicContext.GetString(wda.ConverterName, wda.DisplayName);
			}
			return string.Empty;
		}

		/// <summary>生成显示文本</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public string TextFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return string.Empty; }
			return TextFor(expression);
		}

		/// <summary>生成 label 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Span DisplayFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new Span(basicContext, meta).Value();
			}
			else { throw new Exception("不存在属性：" + name); }
		}
		/// <summary>生成 span 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <param name="format">格式化内容</param>
		/// <returns></returns>
		public Span DisplayFor<TP>(Expression<Func<TM, TP>> expression, string format)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new Span(basicContext, meta).Format(format).Value();
			}
			else { throw new Exception("不存在属性：" + name); }
		}

		/// <summary>生成 label 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Label LabelFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new Label(basicContext, meta).For(name).Text();
			}
			return Label.Empty;
		}

		/// <summary>生成 label 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Label LabelFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return Label.Empty; }
			return LabelFor(expression);
		}

		/// <summary>生成 el-input 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public InputText InputTextFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new InputText(basicContext, meta).Model(vModel, name) as InputText;
			}
			return InputText.Empty;
		}

		/// <summary>生成 el-input 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public InputText InputTextFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return InputText.Empty; }
			return InputTextFor(expression);
		}

		/// <summary>生成 el-inputnumber 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public InputNumber InputNumberFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new InputNumber(basicContext, meta).Model(vModel, name) as InputNumber;
			}
			return InputNumber.Empty;
		}

		/// <summary>生成 el-inputnumber 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public InputNumber InputNumberFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return InputNumber.Empty; }
			return InputNumberFor(expression);
		}

		/// <summary>生成 el-date-picker 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public InputDate InputDateFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				InputDate input = new InputDate(basicContext, meta).Model(vModel, name);
				string format = meta.DisplayFormatString;
				if (format != null) { format = format.Replace("{0:", "").Replace("}", ""); input.Format(format).ValueFormat(format); }
				return input;
			}
			return InputDate.Empty;
		}

		/// <summary>生成 el-date-picker 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public InputDate InputDateFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return InputDate.Empty; }
			return InputDateFor(expression);
		}

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Cascader CascaderFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new Cascader(basicContext, meta).Model(vModel, name) as Cascader;
			}
			return Cascader.Empty;
		}

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Cascader CascaderFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return Cascader.Empty; }
			return CascaderFor(expression);
		}

		/// <summary>生成 select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Select SelectFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new Select(basicContext, meta).Model(vModel, name) as Select;
			}
			return Select.Empty;
		}

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Select SelectFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return Select.Empty; }
			return SelectFor(expression);
		}

		/// <summary>生成 el-select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Select ElSelectFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new Select(basicContext, meta, true).Model(vModel, name) as Select;
			}
			return Select.Empty;
		}

		/// <summary>生成 el-select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public Select ElSelectFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return Select.Empty; }
			return ElSelectFor(expression);
		}

		/// <summary>生成 select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public RadioList RadioListFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new RadioList(basicContext, meta).Model(vModel, name) as RadioList;
			}
			return RadioList.Empty;
		}

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public RadioList RadioListFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return RadioList.Empty; }
			return RadioListFor(expression);
		}

		/// <summary>生成 select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public CheckList CheckListFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new CheckList(basicContext, meta).Model(vModel, name) as CheckList;
			}
			return CheckList.Empty;
		}

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public CheckList CheckListFor<TP>(int code, Expression<Func<TM, TP>> expression)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return CheckList.Empty; }
			return CheckListFor(expression);
		}
	}
}
