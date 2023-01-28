using System.Collections;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 表示一个CheckBoxList列表，用户可从该列表中选择多个项。
	/// </summary>
	public class CheckBoxList : System.Web.Mvc.MultiSelectList
	{
		/// <summary>
		/// 使用要包含在列表中的指定项来初始化 CheckBoxList 类的新实例。
		/// </summary>
		/// <param name="items">各个项。</param>
		/// <exception cref="System.ArgumentNullException">items 参数为 null。</exception>
		public CheckBoxList(IEnumerable items) : this(items, null, null, null) { }

		/// <summary>
		/// 使用要包含在列表中的指定项和选定的值来初始化 CheckBoxList 类的新实例。
		/// </summary>
		/// <param name="items">各个项。</param>
		/// <param name="selectedValues">选定的值。</param>
		/// <exception cref="System.ArgumentNullException">items 参数为 null。</exception>
		public CheckBoxList(IEnumerable items, IEnumerable selectedValues) : this(items, null, null, selectedValues) { }

		/// <summary>
		/// 使用要包含在列表中的项、数据值字段和数据文本字段来初始化 CheckBoxList 类的新实例。
		/// </summary>
		/// <param name="items">各个项。</param>
		/// <param name="dataValueField">数据值字段。</param>
		/// <param name="dataTextField">数据文本字段。</param>
		/// <exception cref="System.ArgumentNullException">items 参数为 null。</exception>
		public CheckBoxList(IEnumerable items, string dataValueField, string dataTextField)
			: this(items, dataValueField, dataTextField, null) { }

		/// <summary>
		/// 使用要包含在列表中的项、数据值字段、数据文本字段和选定的值来初始化 CheckBoxList 类的新实例。
		/// </summary>
		/// <param name="items">各个项。</param>
		/// <param name="dataValueField">数据值字段。</param>
		/// <param name="dataTextField">数据文本字段。</param>
		/// <param name="selectedValues">选定的值。</param>
		/// <exception cref="System.ArgumentNullException">items 参数为 null。</exception>
		public CheckBoxList(IEnumerable items, string dataValueField, string dataTextField, IEnumerable selectedValues)
			: base(items, dataValueField, dataTextField, selectedValues)
		{
			RepeatColumns = 0;
			RepeatLayout = RepeatLayout.Flow;
		}

		/// <summary>
		/// 获取或设置要在 ListControl 控件中显示的列数。
		/// </summary>
		public virtual int RepeatColumns { get; set; }

		/// <summary>
		/// 获取或设置一个值，该值指定是否将使用 table 元素、 ul 元素、 ol 元素或 span 元素来呈现列表。 
		/// </summary>
		public virtual RepeatLayout RepeatLayout { get; set; }
	}
}
