using System;
using System.Linq.Expressions;
using Basic.Collections;
using Basic.EntityLayer;
using BM = Basic.MvcLibrary;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public static class ComplexSearchProvider
	{
		/// <summary>初始化 IGridView 类实例</summary>
		/// <param name="bh"></param>
		public static ComplexSearch<T> ComplexSearch<T>(this IBasicContext bh) where T : class
		{
			return new ComplexSearch<T>(bh);
		}

		/// <summary>初始化 IGridView 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="code"></param>
		public static ComplexSearch<T> ComplexSearch<T>(this IBasicContext bh, int code) where T : class
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(bh, code);
			if (isAuthorization == false) { return BM.ComplexSearch<T>.Empty(bh); }
			return new ComplexSearch<T>(bh);
		}
	}

	/// <summary>表示高级查询条件组合</summary>
	/// <typeparam name="TM"></typeparam>
	public sealed class ComplexSearch<TM> : InputProvider<TM> where TM : class
	{
		private readonly EntityPropertyCollection mProperties;
		private readonly IBasicContext basicContext;
		private readonly TagHtmlWriter tagBuilder;
		/// <summary>初始化 BasicForm 类实例</summary>
		/// <param name="bh">表示数据的上下文请求</param>
		internal ComplexSearch(IBasicContext bh) : base(bh)
		{
			basicContext = bh; EntityPropertyProvidor.TryGetProperties(typeof(TM), out mProperties);
			tagBuilder = new TagHtmlWriter(InputTags.PopoverButton);
			SetAttr("label-width", "80px");
		}
		private readonly bool mEmpty = false;
		/// <summary>初始化 BasicForm 类实例</summary>
		/// <param name="bh">表示数据的上下文请求</param>
		/// <param name="empty"></param>
		private ComplexSearch(IBasicContext bh, bool empty) : base(bh)
		{
			basicContext = bh; EntityPropertyProvidor.TryGetProperties(typeof(TM), out mProperties);
			tagBuilder = new TagHtmlWriter(InputTags.PopoverButton); mEmpty = empty;
		}

		/// <summary>表示空按钮</summary>
		internal static ComplexSearch<TM> Empty(IBasicContext bh) { return new ComplexSearch<TM>(bh, true); }

		internal string vModel;
		/// <summary>表单绑定的模型名称</summary>
		public string ModelName { get; private set; }


		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public ComplexSearch<TM> Action(string action) { SetAttr("url", basicContext.Action(action, null)); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public ComplexSearch<TM> Action(string action, string controller) { SetAttr("url", basicContext.Action(action, controller)); return this; }

		/// <summary>设置执行请求。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public ComplexSearch<TM> Action(string action, string controller, object routeValues)
		{
			SetAttr("url", basicContext.Action(action, controller, routeValues)); return this;
		}

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="model">表单数据对象, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public new ComplexSearch<TM> Model(string model) { ModelName = vModel = model; SetProp("model", model); base.Model(model); return this; }

		/// <summary>表单域标签的宽度，支持 auto。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> LabelWidth(string width) { SetAttr("label-width", width); return this; }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> LabelWidth(int width) { SetAttr("label-width", string.Concat(width, "px")); return this; }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置标签宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置标签宽度</param>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> LabelWidth(bool condition, int tWidth, int fWidth) { return condition ? LabelWidth(tWidth) : LabelWidth(fWidth); }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置标签宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置标签宽度</param>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> LabelWidth(bool condition, string tWidth, string fWidth) { return condition ? LabelWidth(tWidth) : LabelWidth(fWidth); }

		/// <summary>用指定的长度初始化 System.Web.UI.WebControls.Unit 结构的新实例。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> Width(string width) { SetAttr("min-width", width); return this; }

		/// <summary>用像素为高度的数字初始化 Width 的新值</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> Width(int width) { SetAttr("width", string.Concat(width, "px")); return this; }

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> Width(bool condition, int tWidth, int fWidth) { return condition ? Width(tWidth) : Width(fWidth); }

		/// <summary>用像素为高度的数字初始化 Width 的新值。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置宽度</param>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> Width(bool condition, string tWidth, string fWidth) { return condition ? Width(tWidth) : Width(fWidth); }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> SizeToMedium() { SetAttr("size", "medium"); return this; }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> SizeToSmall() { SetAttr("size", "small"); return this; }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> SizeToMini() { SetAttr("size", "mini"); return this; }

		/// <summary>禁用</summary>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> Disabled() { SetProp("disabled", true); return this; }

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> Name(string id) { SetAttr("name", id); SetAttr("ref", id); return this; }

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> ReadOnly() { SetProp("readonly", true); return this; }

		/// <summary>输入框的tabindex</summary>
		/// <returns>返回当前对象。</returns>
		public ComplexSearch<TM> TabIndex(string value) { SetAttr("tabindex", value); return this; }
		//resize 控制是否能被用户缩放  string none, both, horizontal, vertical    —

		/// <summary>输出行</summary>
		/// <returns></returns>
		public GridRow RowFor() { return new GridRow(basicContext); }

		/// <summary>输出行</summary>
		/// <returns></returns>
		public BasicFormItem ItemErrorFor<TP>(Expression<Func<TM, TP>> expression)
		{
			return this.HelperItemFor(expression, true);
		}

		/// <summary>输出行</summary>
		/// <returns></returns>
		public BasicFormItem ItemFor<TP>(Expression<Func<TM, TP>> expression)
		{
			return this.HelperItemFor(expression, false);
		}

		/// <summary>输出行</summary>
		/// <returns></returns>
		private BasicFormItem HelperItemFor<TP>(Expression<Func<TM, TP>> expression, bool showError)
		{
			BasicFormItem item = new BasicFormItem(basicContext);
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta meta);

			if (meta.Display != null)
			{
				WebDisplayAttribute wda = meta.Display;
				string converterName = wda.ConverterName;
				string text = basicContext.GetString(converterName, wda.DisplayName);
				item.SetLabel(text);
			}
			else if (string.IsNullOrWhiteSpace(meta.DisplayName))
				item.SetLabel(meta.Name);
			else
				item.SetLabel(meta.DisplayName);

			item.SetPropValue(meta.Name);

			if (showError) { item.SetError(string.Concat(vModel, ".", meta.Name)); }
			return item;
		}

		/// <summary>输出开始标签</summary>Class1.cs
		/// <returns>返回当前对象</returns>
		public ComplexSearch<TM> Begin()
		{
			if (mEmpty == true) { return this; }
			tagBuilder.MergeOptions(this);
			tagBuilder.RenderBeginTag(basicContext.Writer);
			return this;
		}

		/// <summary>释放非托管资源</summary>
		protected override void Dispose()
		{
			if (mEmpty == true) { return; }
			tagBuilder.RenderEndTag(basicContext.Writer);
		}

		private string mInnerHtml;
		/// <summary>初始化 ComplexSearch 类实例</summary>
		internal void InnerHtml(string html) { mInnerHtml = html; }

		/// <summary>显示输入标签的字符串表示形式(HTML)</summary>
		/// <returns>返回按钮的 Html 字符串</returns>
		public override string ToString()
		{
			if (mEmpty == true) { return string.Empty; }
			tagBuilder.MergeOptions(this);
			tagBuilder.InnerHtml = mInnerHtml;
			return tagBuilder.ToString();
		}
	}
}
