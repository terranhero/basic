using System;

namespace Basic.Interfaces
{
	/// <summary>表示 select 标签需要的Option数据源</summary>
	public interface ISelectOption
	{
		/// <summary>选项的值</summary>
		string Value { get; }

		/// <summary>选项的文本</summary>
		string Text { get; }

		/// <summary>选项的扩展属性</summary>
		string Attributes { get; }

		/// <summary>表示此项包装到的optgroup HTML元素。
		/// 在选择列表中，支持具有相同名称的多个组。
		/// 将它们与参考等式进行比较</summary>
		SelectGroup Group { get; }

		/// <summary>获取或设置一个值，该值指示已禁用</summary>
		bool Disabled { get; set; }

		/// <summary>获取或设置一个值，该值指示 ISelectOption 已选择.</summary>
		/// <value>如果选择了该项，则为true；否则为false。</value>
		bool Selected { get; set; }

	}

	/// <summary>表示 select 标签需要的Option数据源</summary>
	public sealed class SelectGroup
	{
		/// <summary>初始化 SelectGroup 类实例</summary>
		/// <param name="key">分组关键字</param>
		/// <param name="text">分组标题</param>
		/// <param name="disabled">当前分组是否有效</param>
		public SelectGroup(int key, string text, bool disabled) { Value = Convert.ToString(key); Name = text; Disabled = disabled; }

		/// <summary>初始化 SelectGroup 类实例</summary>
		/// <param name="key">分组关键字</param>
		/// <param name="text">分组标题</param>
		public SelectGroup(int key, string text) { Value = Convert.ToString(key); Name = text; }

		/// <summary>初始化 SelectGroup 类实例</summary>
		/// <param name="key">分组关键字</param>
		/// <param name="text">分组标题</param>
		/// <param name="disabled">当前分组是否有效</param>
		public SelectGroup(string key, string text, bool disabled) { Value = key; Name = text; Disabled = disabled; }

		/// <summary>初始化 SelectGroup 类实例</summary>
		/// <param name="key">分组关键字</param>
		/// <param name="text">分组标题</param>
		public SelectGroup(string key, string text) { Value = key; Name = text; }

		/// <summary>选项的值</summary>
		public string Value { get; private set; }

		/// <summary>选项的文本</summary>
		public string Name { get; private set; }

		/// <summary>
		/// 获取或设置一个值，该值指示 SelectGroup 已禁用
		/// </summary>
		public bool Disabled { get; set; }

		/// <summary></summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (!string.IsNullOrWhiteSpace(Value)) { return Value.GetHashCode(); }
			else if (!string.IsNullOrWhiteSpace(Name)) { return Name.GetHashCode(); }
			return base.GetHashCode();
		}
	}

}
