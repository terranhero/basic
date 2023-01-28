using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示表单域输入组件</summary>
	public class InputNumber : Input<InputNumber>
	{
		/// <summary>初始化 InputNumber 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal protected InputNumber(IBasicContext basic, EntityPropertyMeta meta) : base(basic, meta, InputTags.Number) { }

		/// <summary>初始化空按钮实例，此按钮不输出。</summary>
		private InputNumber(bool empty) : base(empty) { }

		/// <summary>表示空数字控件</summary>
		public static InputNumber Empty { get { return new InputNumber(true); } }

		/// <summary>原生属性，设置最大值</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Max(int value) { Prop("max", value); return this; }

		/// <summary>原生属性，设置最小值</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Min(int value) { Prop("min", value); return this; }

		/// <summary>原生属性，设置输入字段的合法数字间隔</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Step(float value) { Prop("step", value); return this; }

		/// <summary>原生属性，设置输入字段的合法数字间隔</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Step(double value) { Prop("step", value); return this; }

		/// <summary>原生属性，设置输入字段的合法数字间隔</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Step(decimal value) { Prop("step", value); return this; }

		/// <summary>原生属性，设置输入字段的合法数字间隔</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Step(int value) { Prop("step", value); return this; }

		/// <summary>输入框关联的label文字</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Label(string label) { Prop("label", label); return this; }

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Form(string name) { Prop("form", name); return this; }

		/// <summary>是否只能输入 step 的倍数</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber StepStrictly() { Prop("step-strictly", true); return this; }

		/// <summary>数值精度</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber Precision(int value) { Prop("precision", value); return this; }

		/// <summary>数值精度</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber HideControls() { Prop("controls", false); return this; }

		/// <summary>数值精度</summary>
		/// <returns>返回当前对象。</returns>
		public InputNumber ControlsToRight() { Attr("controls-position", "right"); return this; }
	}
}
