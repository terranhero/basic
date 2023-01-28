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
using System.Collections;

namespace Basic.Windows
{
	/// <summary>
	/// SelectTabItem.xaml 的交互逻辑
	/// </summary>
	public partial class SelectTabItem : TabItem
	{
		public SelectTabItem()
		{
			InitializeComponent();
		}
		/// <summary>
		/// 设置用于生成 System.Windows.Controls.ItemsControl 的内容的集合。
		/// </summary>
		/// <param name="enumerable"></param>
		public void SetItemsSource(IEnumerable enumerable)
		{
			ckhColumns.ItemsSource = enumerable;
		}

		private void OnCreateInsertCommand(object sender, RoutedEventArgs e)
		{

		}

		private void OnCancelClick(object sender, RoutedEventArgs e)
		{

		}
	}
}
