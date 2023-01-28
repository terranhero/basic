using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Basic.Windows
{
	public class DragThumb : Thumb
	{
		public DragThumb()
		{
			base.DragDelta += new DragDeltaEventHandler(DragThumb_DragDelta);
		}

		void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			Control designerItem = this.DataContext as Control;
			DesignerCanvas designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;
			if (designerItem != null && designer != null && designerItem.IsFocused)
			{
				double minLeft = double.MaxValue;
				double minTop = double.MaxValue;

				double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
				double deltaVertical = Math.Max(-minTop, e.VerticalChange);

				designer.InvalidateMeasure();
				e.Handled = true;
			}
		}
	}
}
