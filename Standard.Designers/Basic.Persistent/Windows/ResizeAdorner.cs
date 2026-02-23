using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Basic.Windows
{
	public class ResizeAdorner : Adorner
	{
		private readonly VisualCollection visuals;
		private readonly SelectedSharp sharp;

		protected override int VisualChildrenCount { get { return this.visuals.Count; } }

		public ResizeAdorner(UIElement designerItem)
			: base(designerItem)
		{
			this.sharp = new SelectedSharp();
			this.visuals = new VisualCollection(this);
			this.visuals.Add(this.sharp);
			this.sharp.DataContext = designerItem;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			this.sharp.Arrange(new Rect(arrangeBounds));
			return arrangeBounds;
		}

		protected override Visual GetVisualChild(int index)
		{
			return this.visuals[index];
		}
	}
}
