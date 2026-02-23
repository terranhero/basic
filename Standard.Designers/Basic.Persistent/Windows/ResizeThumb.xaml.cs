using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Basic.Windows
{
    /// <summary>
    /// ResizeThumb.xaml 的交互逻辑
    /// </summary>
    public partial class ResizeThumb : System.Windows.Controls.Primitives.Thumb
    {
        private Control item;
        private DesignerCanvas designerCanvas;
        static ResizeThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeThumb), new FrameworkPropertyMetadata(typeof(ResizeThumb)));
        }

        public ResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
            DragStarted += new DragStartedEventHandler(ResizeThumb_DragStarted);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.item = DataContext as Control;

            if (this.item != null)
            {
                this.designerCanvas = VisualTreeHelper.GetParent(this.item) as DesignerCanvas;
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.item != null && this.designerCanvas != null)
            {
                double minTop = double.MaxValue;
                double minDeltaHorizontal = double.MaxValue;
                double minDeltaVertical = double.MaxValue;
                double dragDeltaVertical, dragDeltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                        item.Height = item.ActualHeight - dragDeltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                        Canvas.SetTop(item, Canvas.GetTop(item) + dragDeltaVertical);
                        item.Height = item.ActualHeight - dragDeltaVertical;
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (item.MinWidth < item.ActualWidth - e.HorizontalChange)
                        {
                            Canvas.SetLeft(item, Canvas.GetLeft(item) + e.HorizontalChange);
                            item.Width = item.ActualWidth - e.HorizontalChange;
                        }
                        break;
                    case HorizontalAlignment.Right:
                        if (item.MinWidth < item.ActualWidth + e.HorizontalChange)
                        {
                            dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                            item.Width = item.ActualWidth - dragDeltaHorizontal;
                        }
                        break;
                }
                if (designerCanvas != null) { designerCanvas.InvalidateMeasure(); }
                e.Handled = true;
            }
        }
    }
}
