using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using Basic.Collections;
using Basic.Configuration;
using Basic.DataEntities;
using Basic.Designer;
using Basic.Enums;

namespace Basic.Windows
{
	/// <summary>
	/// DesignerEntity.xaml 的交互逻辑
	/// </summary>
	public partial class DesignerEntity : System.Windows.Controls.Primitives.Thumb, IDesignerItem
	{
		private DesignerCanvas designerCanvas;
		private DesignerEntitiesCanvas designerEntities;
		private readonly Cursor moveCursor;
		public DesignerEntity()
		{
			InitializeComponent();
			AssemblyName assemblyName = typeof(DesignerEntity).Assembly.GetName();
			string resourceName = string.Concat("/", assemblyName.Name, ";component/Images/Move.cur");//ThumbnailView.cur,Move.cur
			StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(resourceName, UriKind.RelativeOrAbsolute));
			moveCursor = new Cursor(streamInfo.Stream);
			//base.DragDelta += new DragDeltaEventHandler(DesignerEntity_DragDelta);
			//base.DragCompleted += new DragCompletedEventHandler(DesignerEntity_DragCompleted);
			//base.DragStarted += new DragStartedEventHandler(DesignerEntity_DragStarted);
			//   treeview SelectedItemChanged="TreeView_SelectedItemChanged"
			//TreeViewItem.Selected="TreeView_Selected"

		}

		#region 属性 IsSelected 定义
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerEntity),
			new PropertyMetadata(false, OnIsSelectedChanged));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)base.GetValue(IsSelectedProperty); }
			set { base.SetValue(IsSelectedProperty, value); }
		}
		private Adorner adorner;
		private void ShowAdorner(bool shower)
		{
			if (adorner == null)
			{
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
				if (adornerLayer != null)
				{
					adorner = new ResizeAdorner(this);
					adornerLayer.Add(adorner);
					adorner.Visibility = Visibility.Visible;
				}
			}
			else if (shower)
			{
				adorner.Visibility = Visibility.Visible;
			}
			else
			{
				adorner.Visibility = Visibility.Hidden;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DesignerEntity control = d as DesignerEntity;

			if (control.IsSelected)
			{
				control.ShowAdorner(true);
				//control.BorderBrush = Brushes.DarkRed;
			}
			else
			{
				control.ShowAdorner(false);
				//control.BorderBrush = Brushes.Gray;
				if (control.SelectedItem != null)
					control.SelectedItem.IsSelected = false;
			}
		}
		#endregion

		#region 属性 SelectedType 定义
		private static readonly DependencyPropertyKey SelectedTypePropertyKey = DependencyProperty.RegisterReadOnly("SelectedType",
			typeof(SelectedTypeEnum), typeof(DesignerEntity), new PropertyMetadata(SelectedTypeEnum.None));
		public static readonly DependencyProperty SelectedTypeProperty = SelectedTypePropertyKey.DependencyProperty;
		/// <summary>
		/// 当控件已经被选择后确定当前选择的子控件的类型。
		/// </summary>
		public SelectedTypeEnum SelectedType
		{
			get { return (SelectedTypeEnum)base.GetValue(SelectedTypeProperty); }
		}
		#endregion

		#region 属性 SelectedItem 定义
		private static readonly DependencyPropertyKey SelectedItemPropertyKey = DependencyProperty.RegisterReadOnly("SelectedItem",
			typeof(TreeViewItem), typeof(DesignerEntity), new PropertyMetadata(null));
		public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;
		/// <summary>
		/// 当前选定项所属的 TreeViewItem 对象。
		/// </summary>
		public TreeViewItem SelectedItem
		{
			get { return (TreeViewItem)base.GetValue(SelectedItemProperty); }
		}
		#endregion

		#region 属性 SelectedObject 定义
		private static readonly DependencyPropertyKey SelectedObjectPropertyKey = DependencyProperty.RegisterReadOnly("SelectedObject",
			typeof(object), typeof(DesignerEntity), new PropertyMetadata(null));
		public static readonly DependencyProperty SelectedObjectProperty = SelectedObjectPropertyKey.DependencyProperty;

		/// <summary>
		/// 当前选定项的逻辑值
		/// </summary>
		public object SelectedObject
		{
			get { return base.GetValue(SelectedObjectProperty); }
		}
		#endregion

		#region 自定义选择事件
		/// <summary>
		/// 命令创建事件委托
		/// </summary>
		public static readonly RoutedEvent EditCommandEvent = EventManager.RegisterRoutedEvent("EditCommand",
		RoutingStrategy.Direct, typeof(EditCommandHanlder), typeof(DesignerEntity));

		public event EditCommandHanlder EditCommand
		{
			add { AddHandler(EditCommandEvent, value); }
			remove { RemoveHandler(EditCommandEvent, value); }
		}

		/// <summary>
		/// 命令创建事件委托
		/// </summary>
		public static readonly RoutedEvent CreateCommandEvent = EventManager.RegisterRoutedEvent("CreateCommand",
		RoutingStrategy.Direct, typeof(CreateCommandHanlder), typeof(DesignerEntity));

		public event CreateCommandHanlder CreateCommand
		{
			add { AddHandler(DesignerEntity.CreateCommandEvent, value); }
			remove { RemoveHandler(DesignerEntity.CreateCommandEvent, value); }
		}
		private void RaiseCreateCommandEvent(CreateCommandEventArgs eventArgs)
		{
			RaiseEvent(eventArgs);
		}

		public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged",
		RoutingStrategy.Bubble, typeof(CommandChengedHandler), typeof(DesignerEntity));
		public event CommandChengedHandler SelectionChanged
		{
			add { AddHandler(DesignerEntity.SelectionChangedEvent, value); }
			remove { RemoveHandler(DesignerEntity.SelectionChangedEvent, value); }
		}

		// This method raises the Tap event
		private void RaiseSelectionChangedEvent(CommandChangedEventArgs eventArgs)
		{
			//windowCommands.SetSelectedObjects(eventArgs.SelectedValues);
			RaiseEvent(eventArgs);
		}
		#endregion

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
				//double left = Canvas.GetLeft(this);
				//double top = Canvas.GetTop(this);
				//if (double.IsNaN(left)) { Canvas.SetLeft(this, 0); }
				//if (double.IsNaN(top)) { Canvas.SetTop(this, 0); }
				designerCanvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;
				designerEntities = ItemsControl.ItemsControlFromItemContainer(this) as DesignerEntitiesCanvas;
			}
		}

		/// <summary>
		/// 引发 System.Windows.Controls.Control.MouseDoubleClick 路由事件。
		/// </summary>
		/// <param name="e">事件数据。</param>
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			FrameworkElement element = e.OriginalSource as FrameworkElement;
			while (element != this)
			{
				element = VisualTreeHelper.GetParent(element) as FrameworkElement;
				if (element is CommandNode || element is PropertyNode) { break; }
				else if (element is TreeViewItem)
				{
					TreeViewItem item = element as TreeViewItem;
					if (item.ItemsSource is DataEntityPropertyCollection)
					{
						item.IsSelected = true;
						base.SetValue(SelectedObjectPropertyKey, item.DataContext);
						break;
					}
					else if (item.ItemsSource is DataConditionPropertyCollection)
					{
						item.IsSelected = true;
						base.SetValue(SelectedObjectPropertyKey, item.DataContext);
						break;
					}
					else if (item.ItemsSource is DataCommandCollection)
					{
						item.IsSelected = true;
						base.SetValue(SelectedObjectPropertyKey, item.DataContext);
						break;
					}
				}
			}
			base.OnMouseDoubleClick(e);
		}

		/// <summary>
		/// 每当未处理的 System.Windows.FrameworkElement.ContextMenuOpening 路由事件在其路由中到达此类时调用。
		/// 实现此方法可为此事件添加类处理。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.RoutedEventArgs。</param>
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			this.Focus();
			FrameworkElement element = e.OriginalSource as FrameworkElement;
			while (element != this)
			{
				element = VisualTreeHelper.GetParent(element) as FrameworkElement;
				if (element is CommandNode)
				{
					TreeViewItem item = VisualTreeHelper.GetParent(element) as TreeViewItem;
					base.SetValue(SelectedObjectPropertyKey, item.DataContext);
					item.IsSelected = true;
					break;
				}
				else if (element is PropertyNode)
				{
					TreeViewItem item = VisualTreeHelper.GetParent(element) as TreeViewItem;
					base.SetValue(SelectedObjectPropertyKey, item.DataContext);
					item.IsSelected = true;
					break;
				}
				else if (element is TreeViewItem)
				{
					TreeViewItem item = element as TreeViewItem;
					if (item.ItemsSource is DataEntityPropertyCollection)
					{
						item.IsSelected = true;
						base.SetValue(SelectedObjectPropertyKey, item.DataContext);
						break;
					}
					else if (item.ItemsSource is DataConditionPropertyCollection)
					{
						item.IsSelected = true;
						base.SetValue(SelectedObjectPropertyKey, item.DataContext);
						break;
					}
					else if (item.ItemsSource is DataCommandCollection)
					{
						item.IsSelected = true;
						base.SetValue(SelectedObjectPropertyKey, item.DataContext);
						break;
					}
				}
			}
			base.OnContextMenuOpening(e);
		}

		/// <summary>
		/// 在此元素上引发未处理的 System.Windows.UIElement.MouseLeftButtonDown 路由事件时，调用此方法。实现此方法可为此事件添加类处理。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.Input.MouseButtonEventArgs。事件数据将报告已按下了鼠标左键。</param>
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			ArrayList newList = new ArrayList { new ObjectDescriptor<DataEntityElement>(DataContext as DataEntityElement) };
			CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
			RaiseSelectionChangedEvent(eventArgs);
			if (SelectedItem != null) { SelectedItem.IsSelected = false; }
			base.OnMouseLeftButtonDown(e);
		}

		/// <summary>
		/// 当未处理的 System.Windows.Input.Keyboard.PreviewKeyDown 附加事件在其路由中到达派生自此类的元素时，调用此方法。
		/// 实现此方法可为此事件添加类处理。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.Input.KeyEventArgs。</param>
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			KeyboardDevice kd = e.KeyboardDevice;
			if (IsSelected && kd.Modifiers == ModifierKeys.Control && SelectedObject != null && SelectedObject is DataEntityPropertyElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataEntityPropertyElement property = SelectedObject as DataEntityPropertyElement;
				int index = entity.Properties.IndexOf(property);
				int count = entity.Properties.Count;
				if (e.Key == Key.Up && index >= 1)
				{
					e.Handled = true;
					entity.Properties.Move(index, index - 1);
				}
				else if (e.Key == Key.Down && index < count - 1)
				{
					e.Handled = true;
					entity.Properties.Move(index, index + 1);
				}
			}
			else if (IsSelected && kd.Modifiers == ModifierKeys.Control && SelectedObject != null && SelectedObject is DataConditionPropertyElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataConditionPropertyElement property = SelectedObject as DataConditionPropertyElement;
				int index = entity.Condition.Arguments.IndexOf(property);
				int count = entity.Condition.Arguments.Count;
				if (e.Key == Key.Up && index >= 1)
				{
					e.Handled = true;
					entity.Condition.Arguments.Move(index, index - 1);
				}
				else if (e.Key == Key.Down && index < count - 1)
				{
					e.Handled = true;
					entity.Condition.Arguments.Move(index, index + 1);
				}
			}
			else if (IsSelected && kd.Modifiers == ModifierKeys.Control && SelectedObject != null && SelectedObject is DataCommandElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataCommandElement command = SelectedObject as DataCommandElement;
				int index = entity.DataCommands.IndexOf(command);
				int count = entity.DataCommands.Count;
				if (e.Key == Key.Up && index >= 1)
				{
					e.Handled = true;
					entity.DataCommands.Move(index, index - 1);
				}
				else if (e.Key == Key.Down && index < count - 1)
				{
					e.Handled = true;
					entity.DataCommands.Move(index, index + 1);
				}
			}
			else if (IsSelected && e.Key == Key.Up && SelectedObject is DataEntityPropertyElement)
			{
				DependencyObject parentItem = VisualTreeHelper.GetParent(SelectedItem);
				TreeViewItem headerItems = ItemsControl.ItemsControlFromItemContainer(SelectedItem) as TreeViewItem;
				int index = headerItems.Items.IndexOf(SelectedObject);
				if (index > 0)
				{
					TreeViewItem nextItem = VisualTreeHelper.GetChild(parentItem, index - 1) as TreeViewItem;
					nextItem.IsSelected = true;
					e.Handled = true;
				}
			}
			else if (IsSelected && e.Key == Key.Down && SelectedObject != null)
			{
				DependencyObject parentItem = VisualTreeHelper.GetParent(SelectedItem);
				TreeViewItem headerItems = ItemsControl.ItemsControlFromItemContainer(SelectedItem) as TreeViewItem;
				int index = headerItems.Items.IndexOf(SelectedObject); int maxIndex = headerItems.Items.Count - 1;
				if (index < maxIndex)
				{
					TreeViewItem nextItem = VisualTreeHelper.GetChild(parentItem, index + 1) as TreeViewItem;
					nextItem.IsSelected = true;
					e.Handled = true;
				}
			}
			else if (IsSelected && e.Key == Key.Left) { Canvas.SetLeft(this, Canvas.GetLeft(this) - 1); }
			else if (IsSelected && e.Key == Key.Right) { Canvas.SetLeft(this, Canvas.GetLeft(this) + 1); }
			else if (IsSelected && e.Key == Key.Up) { Canvas.SetTop(this, Canvas.GetTop(this) - 1); }
			else if (IsSelected && e.Key == Key.Down) { Canvas.SetTop(this, Canvas.GetTop(this) + 1); }
			base.OnPreviewKeyDown(e);
		}

		/// <summary>
		/// 当 System.Windows.Controls.Primitives.Thumb 控件具有逻辑焦点和鼠标捕获时，
		/// 随着鼠标位置更改发生一次或多次。
		/// </summary>
		/// <param name="sender">附加此事件处理程序的对象。</param>
		/// <param name="e">事件数据。</param>
		private void DesignerEntity_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (e.OriginalSource is DesignerEntity)
			{
				//double left = Canvas.GetLeft(this);
				//double top = Canvas.GetTop(this);

				//Canvas.SetLeft(this, left + e.HorizontalChange);
				//Canvas.SetTop(this, top + e.VerticalChange);
				if (designerCanvas != null) { designerCanvas.InvalidateMeasure(); }
			}
		}

		private void DesignerEntity_DragStarted(object sender, DragStartedEventArgs e)
		{
			this.Cursor = moveCursor;
		}

		private void DesignerEntity_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			this.Cursor = Cursors.Arrow;
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			base.SetValue(SelectedObjectPropertyKey, e.NewValue);
			ArrayList newList = new ArrayList();
			if (e.NewValue is DataEntityPropertyElement)
			{
				newList.Add(new EntityPropertyDescriptor(e.NewValue as DataEntityPropertyElement));
				CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
				RaiseSelectionChangedEvent(eventArgs);
			}
			else if (e.NewValue is DataConditionPropertyElement)
			{
				newList.Add(new EntityPropertyDescriptor(e.NewValue as DataConditionPropertyElement));
				CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
				RaiseSelectionChangedEvent(eventArgs);
			}
			else if (e.NewValue is StaticCommandElement)
			{
				newList.Add(new StaticCommandDescriptor(e.NewValue as StaticCommandElement));
				CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
				RaiseSelectionChangedEvent(eventArgs);
			}
			else if (e.NewValue is DynamicCommandElement)
			{
				newList.Add(new DynamicCommandDescriptor(e.NewValue as DynamicCommandElement));
				CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
				RaiseSelectionChangedEvent(eventArgs);
			}
			else if (e.NewValue is TreeViewItem)
			{
				TreeViewItem item = e.NewValue as TreeViewItem;
				if (item.ItemsSource is DataEntityPropertyCollection)
				{
					item.IsSelected = true;
					base.SetValue(SelectedObjectPropertyKey, item.DataContext);
					newList.Add(new ObjectDescriptor<DataEntityElement>(DataContext as DataEntityElement));
					CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
					RaiseSelectionChangedEvent(eventArgs);
				}
				else if (item.ItemsSource is DataConditionPropertyCollection)
				{
					item.IsSelected = true;
					base.SetValue(SelectedObjectPropertyKey, item.DataContext);
					newList.Add(new ObjectDescriptor<DataEntityElement>(DataContext as DataEntityElement));
					CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
					RaiseSelectionChangedEvent(eventArgs);
				}
				else if (item.ItemsSource is DataCommandCollection)
				{
					item.IsSelected = true;
					base.SetValue(SelectedObjectPropertyKey, DataContext);
					newList.Add(new ObjectDescriptor<DataEntityElement>(DataContext as DataEntityElement));
					CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
					RaiseSelectionChangedEvent(eventArgs);
				}
			}
			else
			{
				newList.Add(new ObjectDescriptor<DataEntityElement>(DataContext as DataEntityElement));
				CommandChangedEventArgs eventArgs = new CommandChangedEventArgs(SelectionChangedEvent, newList);
				RaiseSelectionChangedEvent(eventArgs);
			}
		}

		private void TreeView_Selected(object sender, RoutedEventArgs e)
		{
			base.SetValue(SelectedItemPropertyKey, e.OriginalSource as TreeViewItem);
		}

		/// <summary>
		/// 移除当前选定项
		/// </summary>
		/// <returns></returns>
		public bool Remove()
		{
			if (SelectedObject is DataConditionPropertyElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataConditionPropertyElement property = SelectedObject as DataConditionPropertyElement;
				string confirmMessage = string.Format("确定要删除\"{0}\"类型的\"{1}\"属性？", entity.Condition.ClassName, property.Name);
				if (designerCanvas.Confirm(confirmMessage))//确定
				{
					DependencyObject parentItem = VisualTreeHelper.GetParent(SelectedItem);
					int visualIndex = VisualTreeHelper.GetChildrenCount(parentItem) - 1;
					TreeViewItem tviCondition = ItemsControl.ItemsControlFromItemContainer(SelectedItem) as TreeViewItem;
					int index = tviCondition.Items.IndexOf(property); int maxIndex = tviCondition.Items.Count - 1;
					TreeViewItem nextItem = null;
					if (index == maxIndex && maxIndex > 1 && visualIndex == maxIndex)
						nextItem = VisualTreeHelper.GetChild(parentItem, index - 1) as TreeViewItem;
					else if (index < maxIndex && visualIndex == maxIndex)
						nextItem = VisualTreeHelper.GetChild(parentItem, index + 1) as TreeViewItem;
					if (nextItem != null) { nextItem.IsSelected = true; }
					return entity.Condition.Arguments.Remove(property);
				}
				return true;
			}
			else if (SelectedObject is DataEntityPropertyElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataEntityPropertyElement property = SelectedObject as DataEntityPropertyElement;
				string confirmMessage = string.Format("确定要删除\"{0}\"类型的\"{1}\"属性？", entity.ClassName, property.Name);
				if (designerCanvas.Confirm(confirmMessage))//确定
				{
					DependencyObject parentItem = VisualTreeHelper.GetParent(SelectedItem);
					int visualIndex = VisualTreeHelper.GetChildrenCount(parentItem) - 1;
					TreeViewItem tviDataEntity = ItemsControl.ItemsControlFromItemContainer(SelectedItem) as TreeViewItem;
					int index = tviDataEntity.Items.IndexOf(property); int maxIndex = tviDataEntity.Items.Count - 1;
					TreeViewItem nextItem = null;
					if (index == maxIndex && maxIndex > 1 && visualIndex == maxIndex)
						nextItem = VisualTreeHelper.GetChild(parentItem, index - 1) as TreeViewItem;
					else if (index < maxIndex && visualIndex == maxIndex)
						nextItem = VisualTreeHelper.GetChild(parentItem, index + 1) as TreeViewItem;
					if (nextItem != null) { nextItem.IsSelected = true; }
					return entity.Properties.Remove(property);
				}
				return true;
			}
			else if (SelectedObject is DataCommandElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataCommandElement command = SelectedObject as DataCommandElement;
				string confirmMessage = string.Format("确定要删除\"{0}\"命令？", command.Name);
				if (designerCanvas.Confirm(confirmMessage))//确定
				{
					DependencyObject parentItem = VisualTreeHelper.GetParent(SelectedItem);
					int visualIndex = VisualTreeHelper.GetChildrenCount(parentItem) - 1;
					TreeViewItem tviCommands = ItemsControl.ItemsControlFromItemContainer(SelectedItem) as TreeViewItem;
					int index = tviCommands.Items.IndexOf(command); int maxIndex = tviCommands.Items.Count - 1;
					TreeViewItem nextItem = null;
					if (index == maxIndex && maxIndex > 1 && visualIndex == maxIndex)
						nextItem = VisualTreeHelper.GetChild(parentItem, index - 1) as TreeViewItem;
					else if (index < maxIndex && visualIndex == maxIndex)
						nextItem = VisualTreeHelper.GetChild(parentItem, index + 1) as TreeViewItem;
					if (nextItem != null) { nextItem.IsSelected = true; }
					return entity.DataCommands.Remove(command);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public void CreateProperty(object sender, EventArgs e)
		{
			if (SelectedObject is DataConditionPropertyElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataConditionPropertyElement property = new DataConditionPropertyElement(entity.Condition);
				entity.Condition.Arguments.Add(property);
			}
			else if (SelectedObject is DataEntityPropertyElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataEntityPropertyElement property = new DataEntityPropertyElement(entity);
				entity.Properties.Add(property);
			}
			else if (SelectedObject is DataEntityElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataEntityPropertyElement property = new DataEntityPropertyElement(entity);
				entity.Properties.Add(property);
			}
			else if (SelectedObject is DataConditionElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataConditionPropertyElement property = new DataConditionPropertyElement(entity.Condition);
				entity.Condition.Arguments.Add(property);
			}
			else if (SelectedObject == null && IsFocused)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataEntityPropertyElement property = new DataEntityPropertyElement(entity);
				entity.Properties.Add(property);
			}
		}

		/// <summary>
		/// 插入新属性
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void InsertProperty(object sender, EventArgs e)
		{
			if (SelectedObject is DataConditionPropertyElement)
			{
				DataEntityElement entity = DataContext as DataEntityElement;
				DataConditionPropertyElement oldValue = SelectedObject as DataConditionPropertyElement;
				DataConditionPropertyElement property = new DataConditionPropertyElement(entity.Condition);
				int index = entity.Condition.Arguments.IndexOf(oldValue);
				entity.Condition.Arguments.Insert(index, property);
			}
			else if (SelectedObject is DataEntityPropertyElement)
			{
				DataEntityPropertyElement oldValue = SelectedObject as DataEntityPropertyElement;
				DataEntityElement entity = DataContext as DataEntityElement;
				DataEntityPropertyElement property = new DataEntityPropertyElement(entity);
				int index = entity.Properties.IndexOf(oldValue);
				entity.Properties.Insert(index, property);
			}
		}
	}
}
