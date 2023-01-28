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
using System.Reflection;
using System.Windows.Resources;
using System.Windows.Controls.Primitives;

namespace Basic.Windows
{
	/// <summary>
	/// DesignerTable.xaml 的交互逻辑
	/// </summary>
	public partial class DesignerTable : System.Windows.Controls.Primitives.Thumb
	{
		private DesignerCanvas designerCanvas;
		private readonly Cursor moveCursor;
		public DesignerTable()
		{
			InitializeComponent();
			AssemblyName assemblyName = typeof(DesignerTable).Assembly.GetName();
			string resourceName = string.Concat("/", assemblyName.Name, ";component/Images/Move.cur");//ThumbnailView.cur,Move.cur
			StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(resourceName, UriKind.RelativeOrAbsolute));
			moveCursor = new Cursor(streamInfo.Stream);
		}

		/// <summary>
		/// 当此元素的父级在可视化树中更改时调用。
		/// 重写 System.Windows.UIElement.OnVisualParentChanged(System.Windows.DependencyObject)。
		/// </summary>
		/// <param name="oldParent">旧父元素。可以为 null，指示元素未曾有过可视化父级。</param>
		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			base.OnVisualParentChanged(oldParent);
			if (oldParent == null)
			{
				double left = Canvas.GetLeft(this);
				double top = Canvas.GetTop(this);
				Random random = new Random();
				if (double.IsNaN(left)) { Canvas.SetLeft(this, random.NextDouble() * 100); }
				if (double.IsNaN(top)) { Canvas.SetTop(this, 0); }
				designerCanvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;
			}
		}

		#region 属性 IsSelected 定义
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected",
			typeof(bool), typeof(DesignerTable), new PropertyMetadata(false, OnIsSelectedChanged));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)base.GetValue(IsSelectedProperty); }
			set { base.SetValue(IsSelectedProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DesignerTable control = d as DesignerTable;

			if (control.IsSelected)
				control.BorderBrush = Brushes.DarkRed;
			else
				control.BorderBrush = Brushes.Gray;
		}
		#endregion

		/// <summary>
		/// 当 System.Windows.Controls.Primitives.Thumb 控件具有逻辑焦点和鼠标捕获时，
		/// 随着鼠标位置更改发生一次或多次。
		/// </summary>
		/// <param name="sender">附加此事件处理程序的对象。</param>
		/// <param name="e">事件数据。</param>
		private void DesignerTable_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (e.OriginalSource is DesignerTable)
			{
				double left = Canvas.GetLeft(this);
				double top = Canvas.GetTop(this);
				if (left + e.HorizontalChange >= 0)
					Canvas.SetLeft(this, left + e.HorizontalChange);
				if (top + e.VerticalChange >= 0)
					Canvas.SetTop(this, top + e.VerticalChange);
				if (designerCanvas != null) { designerCanvas.InvalidateMeasure(); }
			}
		}

		private void DesignerTable_DragStarted(object sender, DragStartedEventArgs e)
		{
			this.Cursor = moveCursor;
		}

		private void DesignerTable_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			this.Cursor = Cursors.Arrow;
		}
	}
}
