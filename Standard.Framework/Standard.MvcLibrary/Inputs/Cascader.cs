using System.Text.RegularExpressions;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示级联选择器</summary>
	public class Cascader : Input<Cascader>
	{
		/// <summary>初始化 Cascader 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal protected Cascader(IBasicContext basic, EntityPropertyMeta meta) : base(basic, meta, InputTags.Cascader) { }

		/// <summary>初始化空实例，此不输出。</summary>
		private Cascader(bool empty) : base(empty) { }

		/// <summary>表示空实例</summary>
		public static Cascader Empty { get { return new Cascader(true); } }

		/// <summary>可选项数据源，键名可通过 Props 属性配置,(array)</summary>
		/// <example><![CDATA[[{value: 'zhinan',label: '指南',children:[{...}]}]]></example>
		/// <returns>返回当前对象。</returns>
		public Cascader Options(string value)
		{
			if (string.IsNullOrWhiteSpace(value) == true) { return this; }
			SetProp("options", value); return this;
		}

		/// <summary>配置选项，具体见下表。(object)</summary>
		/// <example><![CDATA[ ]]></example>
		/// <returns>返回当前对象。</returns>
		public Cascader SetProps(string value)
		{
			if (string.IsNullOrWhiteSpace(value) == true) { return this; }
			SetProp("props", value); return this;
		}

		/// <summary>多选模式下是否折叠Tag </summary>
		/// <returns>返回当前对象。</returns>
		public Cascader CollapseTags() { SetProp("collapse-tags", true); return this; }

		/// <summary>选项分隔符 </summary>
		/// <returns>返回当前对象。</returns>
		public Cascader Separator(string value) { SetProp("separator", value); return this; }

		/// <summary>是否可清空</summary>
		/// <returns>返回当前对象。</returns>
		public Cascader Clearable() { SetProp("clearable", true); return this; }

		/// <summary>输入框中是否显示选中值的完整路径，</summary>
		/// <param name="value">输入框是否显示选中值的完整路径，默认值为 true</param>
		/// <returns>返回当前对象。</returns>
		public Cascader ShowAllLevels(bool value) { SetProp("show-all-levels", value); return this; }

		/// <summary>输入框中是否显示选中值的完整路径，默认值为 true</summary>
		/// <returns>返回当前对象。</returns>
		public Cascader HideAllLevels() { SetProp("show-all-levels", false); return this; }

		/// <summary>是否可搜索选项，</summary>
		/// <param name="value">是否可搜索选项，默认值为 true</param>
		/// <returns>返回当前对象。</returns>
		public Cascader Filterable(bool value) { SetProp("filterable", value); return this; }

		/// <summary>输出开始标签</summary>
		/// <returns>返回当前对象</returns>
		public new Cascader Begin() { return base.Begin() as Cascader; }

	}
}
