using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 标识 DesignerItem 控件单击或右击鼠标时选择项的类型。
	/// </summary>
	public enum SelectedTypeEnum : byte
	{
		None = 0,
		DataEntityItem = 1,
		ConditionItem = 2,
		CommandItem = 3,
		EntityProperty = 4,
		ConditionProperty = 5,
		DataCommand = 6,
	}
}
