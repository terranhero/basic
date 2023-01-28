using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Basic.Designer
{
	/// <summary>
	/// 属性选择器容器
	/// </summary>
	public interface IPropertySelectionContainer
	{
		/// <summary>
		/// 返回当前实例的可选择对象。
		/// </summary>
		/// <value>当前实例的 System.Collections.ICollection 类可选择对象。</value>
		ICollection GetSelectedObjects { get; }

		/// <summary>
		/// 设置当前实例添加入 System.Collections.IList 的可选择对象中。
		/// </summary>
		/// <param name="selectionList">选择器可选择对象集合</param>
		void SetSelectedObjects(IList selectionList);
	}
}
