using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Basic.Properties;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Reflection;

namespace Basic.Windows
{
	/// <summary>
	/// 表示显示图像的控件(显示资源图像)。
	/// </summary>
	[System.ComponentModel.ToolboxItem(false)]
	public sealed class PropertyNode : Control
	{
		static PropertyNode() { DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyNode), new FrameworkPropertyMetadata(typeof(PropertyNode))); }

		#region 属性 PrimaryKey 定义
		public static readonly DependencyProperty PrimaryKeyProperty = DependencyProperty.Register("PrimaryKey",
			typeof(bool), typeof(PropertyNode), new PropertyMetadata(false));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool PrimaryKey
		{
			get { return (bool)base.GetValue(PrimaryKeyProperty); }
			set { base.SetValue(PrimaryKeyProperty, value); }
		}
		#endregion
	}


}
