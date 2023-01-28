using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Basic.Windows
{
	/// <summary>
	///  表示 System.Windows.Controls.Menu 内某个可选择的项。
	/// </summary>
	public sealed class DesignerMenuItem : MenuItem
	{
		/// <summary>
		/// 初始化 Basic.Controls.DesignerMenuItem 类的一个新实例。
		/// </summary>
		public DesignerMenuItem() : base() { }

		/// <summary>
		/// 获取一个值，该值指示当前菜单项的 Basic.Controls.DesignerMenuItem.IsEnabled 属性是否为 true。
		/// </summary>
		/// <value>如果可以选中 Basic.Controls.DesignerMenuItem，则为 true；否则为 false。</value>
		protected override bool IsEnabledCore
		{
			get
			{
				bool isEnabeld = base.IsEnabledCore;
				//if (isEnabeld) { Visibility = System.Windows.Visibility.Visible; }
				//else { Visibility = System.Windows.Visibility.Collapsed; }
				return isEnabeld;
			}
		}
	}
}
