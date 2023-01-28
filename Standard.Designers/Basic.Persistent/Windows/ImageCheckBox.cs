using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Basic.Windows
{
	/// <summary>
	/// 表示TreeView节点上的表
	/// </summary>
	public class ImageCheckBox : CheckBox
	{
		static ImageCheckBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageCheckBox), new FrameworkPropertyMetadata(typeof(ImageCheckBox)));
		}

		#region 属性 ImageSource 定义
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource",
			typeof(ImageSource), typeof(ImageCheckBox), new PropertyMetadata(null));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public ImageSource ImageSource
		{
			get { return (ImageSource)base.GetValue(ImageSourceProperty); }
			set { base.SetValue(ImageSourceProperty, value); }
		}
		#endregion
	}
}
