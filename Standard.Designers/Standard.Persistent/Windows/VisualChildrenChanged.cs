using System;
using System.Windows;

namespace Basic.Windows
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void VisualChildrenChangedHandler(object sender, VisualChildrenChangedEventArgs e);

	public class VisualChildrenChangedEventArgs : RoutedEventArgs
	{
		// Fields DependencyObject visualAdded, DependencyObject visualRemoved
		private readonly DependencyObject visualAdded;
		private readonly DependencyObject visualRemoved;

		public VisualChildrenChangedEventArgs(RoutedEvent routedEvent, DependencyObject added, DependencyObject removed)
		{
			visualAdded = added;
			visualRemoved = removed;
			base.RoutedEvent = routedEvent;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			VisualChildrenChangedHandler handler = (VisualChildrenChangedHandler)genericHandler;
			handler(genericTarget, (VisualChildrenChangedEventArgs)this);
		}

		/// <summary>
		/// 
		/// </summary>
		public DependencyObject VisualAdded { get { return this.visualAdded; } }

		/// <summary>
		/// 
		/// </summary>
		public DependencyObject VisualRemoved { get { return this.visualRemoved; } }
	}
}
