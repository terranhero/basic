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
	/// 
	/// </summary>
	public class LinkButton : Button
	{
		static LinkButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkButton), new FrameworkPropertyMetadata(typeof(LinkButton)));
		}
	}
}
