using System;
using System.Linq.Expressions;
using Basic.Collections;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示 bc-form 组件</summary>
	public interface IBasicForm : IDisposable
	{
		/// <summary>输出行</summary>
		/// <returns></returns>
		GridRow RowFor();

	}

	/// <summary>表示 bc-form 组件</summary>
	public interface IBasicForm<TM> : IBasicForm
	{
		/// <summary>输出行</summary>
		/// <returns></returns>
		FormCell CellFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>输出行</summary>
		/// <returns></returns>
		BasicFormItem ItemFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>输出行</summary>
		/// <returns></returns>
		BasicFormItem ItemErrorFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 span 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Span DisplayFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 span 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <param name="format">格式化内容</param>
		/// <returns></returns>
		Span DisplayFor<TP>(Expression<Func<TM, TP>> expression, string format);

		/// <summary>生成 label 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Label LabelFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 label 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Label LabelFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-input 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputText InputTextFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-input 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputText InputTextFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-inputnumber 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputNumber InputNumberFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-inputnumber 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputNumber InputNumberFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-date-picker 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputDate InputDateFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-date-picker 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputDate InputDateFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Cascader CascaderFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Cascader CascaderFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Select SelectFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Select SelectFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Select ElSelectFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Select ElSelectFor<TP>(int code, Expression<Func<TM, TP>> expression);
	}

	/// <summary>表示 bc-form 组件</summary>
	public sealed class BasicForm<TM> : InputProvider<TM>, IBasicForm<TM>
	{
		private readonly EntityPropertyCollection mProperties;
		private readonly IBasicContext basicContext;
		private readonly TagHtmlWriter writer;
		/// <summary>初始化 BasicForm 类实例</summary>
		/// <param name="bh">表示数据的上下文请求</param>
		internal BasicForm(IBasicContext bh) : base(bh)
		{
			basicContext = bh; EntityPropertyProvidor.TryGetProperties(typeof(TM), out mProperties);
			writer = new TagHtmlWriter(FormTags.Element);
			SetAttr("label-width", "80px");
		}

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public BasicForm<TM> Action(string action) { Attr("action", basicContext.Action(action, null)); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public BasicForm<TM> Action(string action, string controller) { Attr("action", basicContext.Action(action, controller)); return this; }

		/// <summary>设置执行请求。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public BasicForm<TM> Action(string action, string controller, object routeValues)
		{
			Attr("url", basicContext.Action(action, controller, routeValues)); return this;
		}

		/// <summary>生成 employee-chooser 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		public EmployeeChooser EmployeeChooserFor<TP>(Expression<Func<TM, TP>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			if (EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta meta))
			{
				return new EmployeeChooser(basicContext, meta).Model(vModel, name) as EmployeeChooser;
			}
			return EmployeeChooser.Empty;
		}

		/// <summary>任一表单项被校验后触发。</summary>
		/// <param name="func">被校验的表单项 prop 值，校验是否通过，错误消息（如果存在）validate:function(value,success,msg){}</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> OnValidate(string func) { Event("validate", func); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> Summary(string text) { Attr("summary", text); return this; }

		/// <summary>表单绑定的模型错误消息</summary>
		private string _Errors;

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="errors">自定义错误对象, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> SetErrors(string errors) { _Errors = errors; Prop("errors", errors); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="errors">自定义错误对象, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> Errors(string errors) { _Errors = errors; Prop("errors", errors); return this; }

		internal string vModel;
		/// <summary>表单绑定的模型名称</summary>
		public string ModelName { get; private set; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="model">表单数据对象, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public new BasicForm<TM> Model(string model) { ModelName = vModel = model; Prop("model", model); base.Model(model); return this; }

		/// <summary>表单绑定的模型验证</summary>
		private string _Rules;

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="rules">表单验证规则, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> Rules(string rules) { _Rules = rules; Prop("rules", rules); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="rules">表单验证规则, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> SetRules(string rules) { _Rules = rules; Prop("rules", rules); return this; }

		/// <summary>添加 Css 类</summary>
		/// <param name="className"></param>
		public new BasicForm<TM> AddClass(string className)
		{
			base.AddClass(className);
			return this;
		}

		/// <summary>设置表格请求数据的路径。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> InlineToTrue() { Prop("inline", "true"); return this; }

		/// <summary>设置表单域标签至靠左位置。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> LabelToLeft() { SetAttr("label-position", "left"); return this; }

		/// <summary>设置表单域标签至靠右位置。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> LabelToRight() { SetAttr("label-position", "right"); return this; }

		/// <summary>设置表单域标签至靠上位置，即在输入控件上方。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> LabelToTop() { SetAttr("label-position", "top"); return this; }

		/// <summary>表单域标签的宽度，支持 auto。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> LabelWidth(string width) { SetAttr("label-width", width); return this; }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置标签宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置标签宽度</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> LabelWidth(bool condition, int tWidth, int fWidth) { return condition ? LabelWidth(tWidth) : LabelWidth(fWidth); }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置标签宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置标签宽度</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> LabelWidth(bool condition, string tWidth, string fWidth) { return condition ? LabelWidth(tWidth) : LabelWidth(fWidth); }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> LabelWidth(int width) { SetAttr("label-width", string.Concat(width, "px")); return this; }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="suffix">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> LabelSuffix(string suffix) { SetAttr("label-suffix", suffix); return this; }

		/// <summary>用于控制该表单内组件的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> SizeToMedium() { SetAttr("size", "medium"); return this; }

		/// <summary>用于控制该表单内组件的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> SizeToSmall() { SetAttr("size", "small"); return this; }

		/// <summary>用于控制该表单内组件的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> SizeToMini() { SetAttr("size", "mini"); return this; }

		/// <summary>设置表单为只读模式</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> ReadOnly() { return AddClass("readonly"); }

		/// <summary>表格数据源关键字列名。</summary>
		/// <param name="key">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> RowKey(string key) { SetAttr("row-key", key); return this; }

		/// <summary>表格数据源时间戳列名。</summary>
		/// <param name="ts">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> Timestamp(string ts) { SetAttr("timestamp", ts); return this; }

		/// <summary>隐藏必填字段的标签旁边的红色星号。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> HideRequiredAsterisk() { Prop("hide-required-asterisk", true); return this; }

		/// <summary>隐藏校验错误信息。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> HideMessage() { Prop("show-message", false); return this; }

		/// <summary>以行内形式展示校验信息。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicForm<TM> InlineMessage() { Prop("inline-message", true); return this; }

		/// <summary>开始输出 form 组件标记</summary>
		public BasicForm<TM> Begin()
		{
			writer.MergeOptions(this);
			writer.RenderBeginTag(basicContext.Writer);
			return this;
		}

		/// <summary>输出行</summary>
		/// <returns></returns>
		public GridRow RowFor()
		{
			return new GridRow(basicContext);
		}

		/// <summary>释放非托管资源</summary>
		protected override void Dispose()
		{
			writer.RenderEndTag(basicContext.Writer);
		}

		/// <summary>释放非托管资源</summary>
		void IDisposable.Dispose() { this.Dispose(); }

		/// <summary>输出行</summary>
		/// <returns></returns>
		public FormCell CellFor<TP>(Expression<Func<TM, TP>> expression)
		{
			FormCell item = new FormCell(basicContext);
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta meta);

			if (meta.Display != null)
			{
				WebDisplayAttribute wda = meta.Display;
				string converterName = wda.ConverterName;
				string text = basicContext.GetString(converterName, wda.DisplayName);
				item.Title(text);
			}
			else if (string.IsNullOrWhiteSpace(meta.DisplayName))
				item.Title(meta.Name);
			else
				item.Title(meta.DisplayName);

			item.SetPropValue(meta.Name);

			return item;
		}

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
		public BasicFormItem HelperItemFor<TP>(Expression<Func<TM, TP>> expression, bool showError)
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

			if (showError && string.IsNullOrWhiteSpace(_Errors) == false) { item.SetError(string.Concat(_Errors, ".", meta.Name)); }
			else if (showError && string.IsNullOrWhiteSpace(_Errors) == true) { item.SetError(meta.Name); }
			return item;
		}

		/// <summary>将字符串写入文本流</summary>
		/// <param name="value">要写入的字符串</param>
		public void Write(string value) { basicContext.Write(value); }

		/// <summary>通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本流</summary>
		/// <param name="value">要写入的对象</param>
		public void Write(object value) { basicContext.Write(value); }

		/// <summary>将行终止符写入文本流</summary>
		public void WriteLine() { basicContext.WriteLine(); }

		/// <summary>将字符串写入文本流，后跟行终止符</summary>
		/// <param name="value">要写入的字符串。 如果 value 为 null，则只写入行终止符</param>
		public void WriteLine(string value) { basicContext.WriteLine(value); }

		/// <summary>通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本流，后跟行终止符。</summary>
		/// <param name="value">要写入的对象。 如果 value 为 null，则只写入行终止符</param>
		public void WriteLine(object value) { basicContext.WriteLine(value); }


		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		public new BasicForm<TM> Event(string evt, string callback) { base.Event(evt, callback); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new BasicForm<TM> Style(string key, string value) { base.Style(key, value); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new BasicForm<TM> Attr<TP>(string key, TP value) { base.Attr(key, value); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new BasicForm<TM> Prop<TP>(string key, TP value) { base.Prop(key, value); return this; }
	}

	/// <summary></summary>
	public static partial class BasicFormExtension
	{
		/// <summary>初始化 IGridView 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="id">表示当前 GridView ID</param>
		public static BasicForm<T> Form<T>(this IBasicContext bh, string id) where T : class
		{
			BasicForm<T> view = new BasicForm<T>(bh);
			view.Id(id).Ref(id);
			return view;
		}

		/// <summary>初始化 IGridView 类实例</summary>
		/// <param name="bh"></param>
		public static BasicForm<T> Form<T>(this IBasicContext bh) where T : class
		{
			return new BasicForm<T>(bh);
		}
	}
}
