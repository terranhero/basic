using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示表单域输入组件</summary>
	public class EmployeeChooser : Input<EmployeeChooser>
	{
		/// <summary>表示空按钮</summary>
		internal static EmployeeChooser Empty { get { return new EmployeeChooser(true); } }
	
		private readonly IBasicContext basicContext;
		/// <summary>初始化 EmployeeChooser 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal protected EmployeeChooser(IBasicContext basic, EntityPropertyMeta meta)
			: base(basic, meta, InputTags.EmployeeChooser) { basicContext = basic; }

		/// <summary>初始化空按钮实例，此按钮不输出。</summary>
		private EmployeeChooser(bool empty) : base(empty) { }

		/// <summary>对应列的最小宽度，与 width 的区别是 width 是固定的，min-width 会把剩余宽度按比例分配给设置了 min-width 的列。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public EmployeeChooser MinWidth(string width) { SetAttr("min-width", width); return this; }

		/// <summary>对应列的最小宽度，与 width 的区别是 width 是固定的，min-width 会把剩余宽度按比例分配给设置了 min-width 的列</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public EmployeeChooser MinWidth(int width) { SetAttr("min-width", string.Concat(width, "px")); return this; }

		/// <summary>设置工号字段</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public EmployeeChooser EmployeeCode(string value) { SetProp("code.sync", value); return this; }

		/// <summary>设置工号字段</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public EmployeeChooser EmployeeName(string value) { SetProp("name.sync", value); return this; }

		/// <summary>使用员工多选功能</summary>
		/// <returns>返回当前对象。</returns>
		public EmployeeChooser Multiple() { SetProp("multiple", true); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="error">自定义错误对象, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public EmployeeChooser SetError(string error) { SetProp("error", error); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="model"></param>
		/// <param name="error">自定义错误对象, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public EmployeeChooser SetError(string model, string error)
		{
			if (string.IsNullOrWhiteSpace(model) == true) { SetProp("error", error); return this; }
			else { SetProp("error", string.Concat(model, ".", error)); return this; }
		}

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="span">表示24栅格系统所占列数</param>
		/// <returns>返回当前对象。</returns>
		public EmployeeChooser SetSpan(int span) { AddClass(string.Concat("el-col-", span)); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public EmployeeChooser Action(string action) { SetAttr("url", basicContext.Action(action, null)); return this; }

		/// <summary>设置执行请求</summary>
		/// <returns>返回当前按钮实例</returns>
		public EmployeeChooser Action(string action, string controller) { SetAttr("url", basicContext.Action(action, controller)); return this; }

		/// <summary>设置执行请求。</summary>
		/// <param name="action">操作方法的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public EmployeeChooser Action(string action, string controller, object routeValues)
		{
			SetAttr("url", basicContext.Action(action, controller, routeValues)); return this;
		}

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		public EmployeeChooser Change(string callback) { Event("change", callback); return this; }

		/// <summary>将指定的事件和回调方法添加到该按钮的事件列表中。</summary>
		/// <param name="evt">包含要添加的事件的名称的字符串</param>
		/// <param name="callback">包含要分配给事件的回调方法</param>
		/// <returns>返回当前按钮实例</returns>
		public new EmployeeChooser Event(string evt, string callback) { base.Event(evt, callback); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的样式列表中。</summary>
		/// <param name="key">包含要添加的样式的名称的字符串</param>
		/// <param name="value">包含要分配给样式的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new EmployeeChooser Style(string key, string value) { base.Style(key, value); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new EmployeeChooser Attr<TP>(string key, TP value) { base.Attr(key, value); return this; }

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <param name="key">包含要添加的属性的名称的字符串</param>
		/// <param name="value">包含要分配给属性的值的字符串</param>
		/// <returns>返回当前按钮实例</returns>
		public new EmployeeChooser Prop<TP>(string key, TP value) { base.Prop(key, value); return this; }
	}
}
