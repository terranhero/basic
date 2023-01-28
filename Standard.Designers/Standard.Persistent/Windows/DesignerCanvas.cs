using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using Basic.Properties;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Basic.Windows
{
	/// <summary>
	/// 定义一个区域，在该区域中可以使用相对于 DesignerCanvas 区域的坐标显式定位子元素。
	/// </summary>
	public sealed class DesignerCanvas : UniformGrid
	{
		private readonly IVsUIShell iVsUiShell;
		/// <summary>
		/// 元素间的空隙
		/// </summary>
		private const double Gap = 10D;
		/// <summary>
		/// 初始化 Basic.Controls.DesignerCanvas 类的一个新实例。
		/// </summary>
		public DesignerCanvas()
			: base()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Focusable = false;
			iVsUiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
		}

		#region 属性 SelectedItem 定义
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(IDesignerItem),
			typeof(DesignerCanvas), new PropertyMetadata(OnSelectedItemChanged));

		/// <summary>
		/// 
		/// </summary>
		public IDesignerItem SelectedItem
		{
			get { return (IDesignerItem)base.GetValue(SelectedItemProperty); }
			set { base.SetValue(SelectedItemProperty, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is IDesignerItem newItem) { newItem.IsSelected = true; }
			if (e.OldValue is IDesignerItem oldItem) { oldItem.IsSelected = false; }
		}
		#endregion

		/// <summary>
		/// 设置当前鼠标为等待光标
		/// </summary>
		public void SetWaitCursor() { if (iVsUiShell != null) { iVsUiShell.SetWaitCursor(); } }

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="message">一个 String，用于指定要显示的文本。</param>
		/// <param name="title">一个 String，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 int 值，用于指定用户单击了哪个消息框按钮。</returns>
		public int ShowMessage(string message, string title = "Basic.Persistent")
		{
			int result = 0; Guid tempGuid = Guid.Empty;
			if (iVsUiShell != null)
			{
				iVsUiShell.ShowMessageBox(0, ref tempGuid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 0, out result);
			}
			return result;
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="message">一个 String，用于指定要显示的文本。</param>
		/// <param name="title">一个 String，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 int 值，用于指定用户单击了哪个消息框按钮。</returns>
		public bool Confirm(string message, string title = "Basic.Persistent")
		{
			if (string.IsNullOrWhiteSpace(title))
				title = DesignerStrings.ResourceManager.GetString("Package_Description");
			int result = 0; Guid tempGuid = Guid.Empty;
			if (iVsUiShell != null)
			{
				iVsUiShell.ShowMessageBox(0, ref tempGuid, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_QUERY, 0, out result);
			}
			return result == 1;//
		}

		/// <summary>
		/// 显示属性窗口
		/// </summary>
		public void ShowPropertyWindow()
		{
			Guid guid = new Guid("{EEFA5220-E298-11D0-8F78-00A0C9110057}");
			iVsUiShell.FindToolWindow(0x80000, ref guid, out IVsWindowFrame ppWindowFrame);
			ppWindowFrame.Show();
		}

		/// <summary>
		/// 重新调整子元素的大小时支持布局行为。
		/// </summary>
		/// <param name="child">重新调整其大小的子元素。</param>
		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			base.OnChildDesiredSizeChanged(child);
		}

		#region 自定义选择事件
		/// <summary>
		/// 命令创建事件委托
		/// </summary>
		public static readonly RoutedEvent VisualChildrenChangedEvent = EventManager.RegisterRoutedEvent("VisualChildrenChanged",
		RoutingStrategy.Direct, typeof(VisualChildrenChangedHandler), typeof(DesignerCanvas));

		public event VisualChildrenChangedHandler VisualChildrenChanged
		{
			add { AddHandler(DesignerCanvas.VisualChildrenChangedEvent, value); }
			remove { RemoveHandler(DesignerCanvas.VisualChildrenChangedEvent, value); }
		}
		private void RaiseVisualChildrenChangedEvent(VisualChildrenChangedEventArgs eventArgs)
		{
			RaiseEvent(eventArgs);
		}
		#endregion

		/// <summary>
		/// 当修改可见对象的 System.Windows.Media.VisualCollection 时调用。
		/// </summary>
		/// <param name="visualAdded">已添加到集合中的 System.Windows.Media.Visual。</param>
		/// <param name="visualRemoved">从集合中移除的 System.Windows.Media.Visual。</param>
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			RaiseVisualChildrenChangedEvent(new VisualChildrenChangedEventArgs(DesignerCanvas.VisualChildrenChangedEvent, visualAdded, visualRemoved));
		}

		/// <summary>按行排列元素</summary>
		/// <param name="lineElements">第一行元素排列</param>
		/// <param name="positions">表示上一行元素右下角位置坐标</param>
		private void ArrangeLine(IList<UIElement> lineElements, List<Point> positions)
		{
			double left = Gap, top = Gap; int index = 0;
			List<Point> points = new List<Point>(positions);
			if (points.Count > 0) { top = points.Max(m => m.Y) + Gap; }
			positions.Clear(); bool isEmpty = points.Count == 0;
			foreach (UIElement element in lineElements)
			{
				double prevX = 0;
				if (isEmpty == false)
				{
					Point pos = points.Count > index ? points[index] : points[points.Count - 1];
					prevX = pos.X; //top = pos.Y + Gap;
				}
				element.Arrange(new Rect(left, top, element.DesiredSize.Width, element.DesiredSize.Height));
				left += element.DesiredSize.Width + Gap; index++;
				positions.Add(new Point(left, top + element.DesiredSize.Height + Gap));
				if (isEmpty == false) { left = Math.Max(left, prevX); }
			}
		}

		/// <summary>
		/// 排列的内容 System.Windows.Controls.WrapPanel 元素。
		/// </summary>
		/// <param name="finalSize">System.Windows.Size ，应使用此元素来排列子元素</param>
		/// <returns>System.Windows.Size ，它表示的排列的大小 System.Windows.Controls.WrapPanel 元素与其子项。</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			int count = InternalChildren.Count;
			int _columns = 0; double childWidth = 0; double num = finalSize.Width - 10.0d;
			foreach (UIElement internalChild in InternalChildren)
			{
				childWidth = childWidth + (internalChild.DesiredSize.Width + Gap); _columns++;
				if (num <= childWidth) { _columns--; break; }
			}
			if (_columns == 0) { _columns = 4; }
			int _rows = (count + (_columns - 1)) / _columns; if (_rows == 0) { _rows = 1; }
			Rect finalRect = new Rect(0.0, 0.0, finalSize.Width / _columns, finalSize.Height / _rows);
			double width = finalRect.Width;

			finalRect.Y += Gap; double lineHeight = 0.0; int lineChildren = 0;
			finalRect.X += Gap;
			foreach (UIElement internalChild in InternalChildren)
			{
				finalRect.Width = internalChild.DesiredSize.Width;
				finalRect.Height = internalChild.DesiredSize.Height;
				internalChild.Arrange(finalRect); lineChildren++;
				if (lineHeight <= internalChild.DesiredSize.Height) { lineHeight = internalChild.DesiredSize.Height; }
				finalRect.X += width;
				if (_columns == lineChildren)
				{
					finalRect.Y += lineHeight + Gap;
					lineChildren = 0; finalRect.X = Gap;
				}
			}
			return finalSize;
		}

		/// <summary>
		/// 测量 DesignerCanvas 的子元素，以便准备在 DesignerCanvas.ArrangeOverride(System.Windows.Size) 过程中排列它们。
		/// </summary>
		/// <param name="constraint">不应超过的上限 System.Windows.Size。</param>
		/// <returns>一个 System.Windows.Size，表示排列子内容所需的大小。</returns>
		protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
		{
			//int count = InternalChildren.Count;
			//int _columns = 0; double childWidth = 0; double num = constraint.Width;
			//foreach (UIElement internalChild in InternalChildren)
			//{
			//	internalChild.Measure(constraint);
			//	childWidth = childWidth + (internalChild.DesiredSize.Width + Gap); _columns++;
			//	if (num <= childWidth) { _columns--; break; }
			//}
			//if (_columns == 0) { _columns = 4; }
			//int _rows = (count + (_columns - 1)) / _columns; if (_rows == 0) { _rows = 1; }

			//Size availableSize = new Size(constraint.Width / (double)_columns, constraint.Height / (double)_rows);
			//double num = 0.0;
			//double num2 = 0.0;
			//int i = 0;
			//for (int count = base.InternalChildren.Count; i < count; i++)
			//{
			//	UIElement uIElement = base.InternalChildren[i];
			//	uIElement.Measure(availableSize);
			//	Size desiredSize = uIElement.DesiredSize;
			//	if (num < desiredSize.Width)
			//	{
			//		num = desiredSize.Width;
			//	}

			//	if (num2 < desiredSize.Height)
			//	{
			//		num2 = desiredSize.Height;
			//	}
			//}

			//return new Size(num * (double)_columns, num2 * (double)_rows);

			Size finalSize = base.MeasureOverride(constraint);
			finalSize.Height += 100;
			return finalSize;
		}
	}
}
