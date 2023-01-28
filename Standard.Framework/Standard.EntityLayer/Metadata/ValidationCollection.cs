using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Basic.Collections
{
	internal sealed class ValidationCollection : Collection<ValidationAttribute>
	{
		/// <summary>
		/// 初始化为空的 Basic.Collections.EntityPropertyCollection 类的新实例。
		/// </summary>
		internal ValidationCollection() : base() { }

		/// <summary>
		/// <![CDATA[对 ValidationCollection 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 EntityPropertyCollection 的每个元素执行的 Action<ValidationAttribute> 委托。]]></param>
		public void ForEach(Action<ValidationAttribute> action)
		{
			if (action == null) { throw new ArgumentNullException("action"); }
			for (int index = 0; index < Items.Count; index++) { action(Items[index]); }
		}
	}
}
