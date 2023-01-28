namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public static class InputTags
	{
		/// <summary>员工选择标签</summary>
		public static string EmployeeChooser { internal get; set; } = "employee-chooser";

		/// <summary>单选按钮组</summary>
		public static string RadioGroup { internal get; set; } = "el-radio-group";
		/// <summary>单选按钮</summary>
		public static string RadioButton { internal get; set; } = "el-radio-button";
		/// <summary>单选按钮</summary>
		public static string Radio { internal get; set; } = "el-radio";

		/// <summary>Checkbox组</summary>
		public static string CheckboxGroup { internal get; set; } = "el-checkbox-group";
		/// <summary>Checkbox按钮</summary>
		public static string CheckboxButton { internal get; set; } = "el-checkbox-button";
		/// <summary>Checkbox复选</summary>
		public static string Checkbox { internal get; set; } = "el-checkbox";

		/// <summary>Select单选</summary>
		public static string Select { internal get; set; } = "select";
		/// <summary>Select单选</summary>
		public static string OptionGroup { internal get; set; } = "optgroup";
		/// <summary>Select单选</summary>
		public static string Option { internal get; set; } = "option";

		/// <summary>Select单选</summary>
		public static string ElSelect { internal get; set; } = "el-select";
		/// <summary>Select单选</summary>
		public static string ElOptionGroup { internal get; set; } = "el-option-group";
		/// <summary>Select单选</summary>
		public static string ElOption { internal get; set; } = "el-option";

		/// <summary>级联选择</summary>
		public static string Cascader { internal get; set; } = "el-cascader";

		/// <summary>输入</summary>
		public static string Input { internal get; set; } = "el-input";

		/// <summary>数组</summary>
		public static string Number { internal get; set; } = "el-input-number";

		/// <summary>日期</summary>
		public static string Date { internal get; set; } = "el-date-picker";

		/// <summary>弹出按钮</summary>
		public static string PopoverButton { internal get; set; } = "popover-button";

	}

	/// <summary></summary>
	public static class FormTags
	{
		/// <summary>表单</summary>
		public static string Element { internal get; set; } = "basic-form";
		/// <summary>表单</summary>
		public static string Form { internal get; set; } = "el-form";
		/// <summary>表单项</summary>
		public static string FormItem { internal get; set; } = "el-form-item";
		/// <summary>表单列</summary>
		public static string FormCell { internal get; set; } = "mt-cell";
		/// <summary>表单行</summary>
		public static string Row { internal get; set; } = "el-row";
	}
}
