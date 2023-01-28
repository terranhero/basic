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
using System.Collections.Specialized;
using Basic.Windows;

namespace Basic.Windows
{
	/// <summary>
	/// DesignerTablesCanvas.xaml 的交互逻辑
	/// </summary>
	public partial class DesignerTablesCanvas : ItemsControl
	{
		private DesignerCanvas designerCanvas;
		public DesignerTablesCanvas() { InitializeComponent(); }

		/// <summary>
		/// 在派生类中重写后，每当应用程序代码或内部进程调用 System.Windows.FrameworkElement.ApplyTemplate()，都将调用此方法。
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (Template != null)
				designerCanvas = Template.FindName("PART_Canvas", this) as DesignerCanvas;
		}

		/// <summary>
		/// 创建或标识用于显示给定项的元素。
		/// </summary>
		/// <returns>用于显示给定项的元素。</returns>
		protected override DependencyObject GetContainerForItemOverride()
		{
			if (base.ItemTemplate != null)
				return base.ItemTemplate.LoadContent();
			return new DesignerTable();
		}

		#region 属性 SelectedItem 定义
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(DesignerTable),
			typeof(DesignerTablesCanvas), new PropertyMetadata(OnSelectedItemChanged));

		/// <summary>
		/// 
		/// </summary>
		public DesignerTable SelectedItem
		{
			get { return (DesignerTable)base.GetValue(SelectedItemProperty); }
			set { base.SetValue(SelectedItemProperty, value); }
		}

		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DesignerTablesCanvas canvas = d as DesignerTablesCanvas;
			DesignerTable newItem = e.NewValue as DesignerTable;
			DesignerTable oldItem = e.OldValue as DesignerTable;
			if (newItem != null) { newItem.IsSelected = true; }
			if (oldItem != null) { oldItem.IsSelected = false; }
		}

		/// <summary>
		/// 当未处理的 System.Windows.Input.Mouse.PreviewMouseDown 附加路由事件在其路由中到达派生自此类的元素时，调用此方法。
		/// 实现此方法可为此事件添加类处理。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.Input.MouseButtonEventArgs。事件数据将报告已按下了一个或多个鼠标按钮。</param>
		protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			UIElement element = e.MouseDevice.Target as UIElement;
			if (element == designerCanvas) { SelectedItem = null; }
			else
			{
				while (element != null && element != designerCanvas)
				{
					element = VisualTreeHelper.GetParent(element) as UIElement;
					if (element is DesignerTable)
					{
						SelectedItem = element as DesignerTable;
						base.OnPreviewMouseDown(e);
						return;
					}
				}
			}
			SelectedItem = null;
			base.OnPreviewMouseDown(e);
		}
		#endregion

		/// <summary>
		/// 准备指定元素以显示指定项。
		/// </summary>
		/// <param name="element">用于显示指定项的元素。</param>
		/// <param name="item">指定项。</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
		}

		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			base.OnChildDesiredSizeChanged(child);
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}

		private void DesignerCanvas_VisualChildrenChanged(object sender, VisualChildrenChangedEventArgs e)
		{
			DesignerCanvas canvas = sender as DesignerCanvas;
			int item = canvas.Children.Count;
		}
	}
}
