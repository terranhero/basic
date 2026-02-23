using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Basic.Collections;
using Basic.Configuration;
using Basic.Database;
using Basic.DataEntities;
using Basic.Designer;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;

namespace Basic.Windows
{
	/// <summary>
	/// DesignerEntitiesxaml 的交互逻辑
	/// </summary>
	public partial class DesignerEntitiesCanvas : ItemsControl
	{
		static DesignerEntitiesCanvas()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerEntitiesCanvas),
				new FrameworkPropertyMetadata(typeof(DesignerEntitiesCanvas)));
		}
		private DesignerCanvas _DesignerCanvas;
		//private ContextMenu designerMenu;
		private readonly PersistentConfiguration _Persistent;
		private readonly DesignTableInfo _TableInfo;
		private readonly DataEntityElementCollection _DataEntities;
		private readonly PersistentPane _PersistentPane;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		private readonly EnvDTE.DTE dteClass;
		private readonly IVsPersistDocData vsPersistDocData;
		private readonly string _FileName;
		public DesignerEntitiesCanvas(PersistentPane pane, PersistentConfiguration configurationPersistent)
		{
			_PersistentPane = pane;
			_ProjectItem = pane.ProjectItem; dteClass = _ProjectItem.DTE;
			_FileName = pane.FileName;
			_Persistent = configurationPersistent;
			_TableInfo = configurationPersistent.TableInfo;
			DataContext = configurationPersistent;
			ItemsSource = _DataEntities = _Persistent.DataEntities;
			vsPersistDocData = pane;

			//VSColorTheme.ThemeChanged += new ThemeChangedEventHandler(VSColorTheme_ThemeChanged);
			System.Drawing.Color wbc = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
			Background = new SolidColorBrush(Color.FromArgb(wbc.A, wbc.R, wbc.G, wbc.B));
			System.Drawing.Color wtc = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
			Foreground = new SolidColorBrush(Color.FromArgb(wtc.A, wtc.R, wtc.G, wtc.B));
		}

		/// <summary>获取数据库表设计文件</summary>
		/// <returns></returns>
		public PersistentConfiguration GetPersistent() { return _Persistent; }

		#region 属性 SelectedItem 定义
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem",
			typeof(DesignerEntity), typeof(DesignerEntitiesCanvas), new PropertyMetadata(OnSelectedItemChanged));

		/// <summary>
		/// 
		/// </summary>
		public DesignerEntity SelectedItem
		{
			get { return (DesignerEntity)base.GetValue(SelectedItemProperty); }
			set { base.SetValue(SelectedItemProperty, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is DesignerEntity newItem) { newItem.IsSelected = true; }
			if (e.OldValue is DesignerEntity oldItem) { oldItem.IsSelected = false; }
		}
		#endregion

		#region 方法重载
		/// <summary>
		/// 在派生类中重写后，每当应用程序代码或内部进程调用 System.Windows.FrameworkElement.ApplyTemplate()，都将调用此方法。
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (Template != null)
			{
				_DesignerCanvas = Template.FindName("PART_Canvas", this) as DesignerCanvas;
				//ItemsControl items = Template.FindName("PART_ITEMS", this) as ItemsControl;
				//_DesignerCanvas = items.Template.FindName("PART_ITEMS", items) as DesignerCanvas;
				_DesignerCanvas.VisualChildrenChanged += new VisualChildrenChangedHandler(DesignerCanvas_VisualChildrenChanged);
			}
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			return base.ArrangeOverride(arrangeBounds);
		}

		/// <summary></summary>
		private void DesignerEntity_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (SelectedItem == null) { return; }
			if (!(SelectedItem is DesignerEntity item)) { return; }
			if (item.SelectedObject is DataEntityPropertyElement)
			{
				DataEntityPropertyElement property = item.SelectedObject as DataEntityPropertyElement;
				_PersistentPane.EditDataEntityCode(item.DataContext as DataEntityElement, property);
			}
			else if (item.SelectedObject is DataConditionPropertyElement)
			{
				DataConditionPropertyElement property = item.SelectedObject as DataConditionPropertyElement;
				_PersistentPane.EditConditionCode(item.DataContext as DataEntityElement, property);
			}
			else if (item.SelectedObject is DataCommandElement)
			{
				_PersistentPane.EditCommandCode(item.SelectedObject as DataCommandElement, item.DataContext as DataEntityElement);
			}
			else if (item.SelectedObject is DataEntityElement)
			{
				_PersistentPane.EditDataEntityCode(item.DataContext as DataEntityElement, null);
			}
			else if (item.SelectedObject is DataConditionElement)
			{
				_PersistentPane.EditConditionCode(item.DataContext as DataEntityElement, null);
			}
		}

		/// <summary>
		/// 每当未处理的 ContextMenuOpening 路由事件在其路由中到达此类时调用。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.RoutedEventArgs。</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD001", Justification = "<挂起>")]
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			try
			{
				e.Handled = true;
				Point point = PointToScreen(Mouse.GetPosition(this));
				_PersistentPane.ShowContextMenu((int)point.X, (int)point.Y);
			}
			catch (Exception ex)
			{
				_PersistentPane.ShowMessage(ex.Message);
			}
		}

		/// <summary>
		/// 当未处理的 System.Windows.Input.Mouse.PreviewMouseDown 附加路由事件在其路由中到达派生自此类的元素时，调用此方法。
		/// 实现此方法可为此事件添加类处理。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.Input.MouseButtonEventArgs。事件数据将报告已按下了一个或多个鼠标按钮。</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD001", Justification = "<挂起>")]
		protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			this.Focus();
			UIElement element = VisualTreeHelper.GetParent(e.MouseDevice.Target as UIElement) as UIElement;
			if (element == this) { SelectedItem = null; _PersistentPane.SetSelectedObjects(_Persistent.GetSelectedObjects); }
			else
			{
				while (element != null && element != this)
				{
					element = VisualTreeHelper.GetParent(element) as UIElement;
					if (element is DesignerEntity) { _DesignerCanvas.SelectedItem = SelectedItem = element as DesignerEntity; break; }
				}
			}
			base.OnPreviewMouseDown(e);
		}

		/// <summary>创建或标识用于显示给定项的元素。</summary>
		/// <returns>用于显示给定项的元素。</returns>
		protected override DependencyObject GetContainerForItemOverride()
		{
			// base.GetContainerForItemOverride();
			if (base.ItemTemplate != null)
			{
				DesignerEntity entity = (DesignerEntity)base.ItemTemplate.LoadContent();
				entity.MouseDoubleClick += new MouseButtonEventHandler(DesignerEntity_MouseDoubleClick);
				entity.SelectionChanged += new CommandChengedHandler(DesignerEntity_SelectionChanged);
				return entity;
			}
			else
			{
				DesignerEntity entity = new DesignerEntity();
				entity.MouseDoubleClick += new MouseButtonEventHandler(DesignerEntity_MouseDoubleClick);
				entity.SelectionChanged += new CommandChengedHandler(DesignerEntity_SelectionChanged);
				return entity;
			}
		}

		/// <summary>
		/// 准备指定元素以显示指定项。
		/// </summary>
		/// <param name="element">用于显示指定项的元素。</param>
		/// <param name="item">指定项。</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
		}

		private void DesignerCanvas_VisualChildrenChanged(object sender, VisualChildrenChangedEventArgs e)
		{
			DesignerCanvas canvas = sender as DesignerCanvas;
			if (e.VisualAdded != null) { Canvas.SetZIndex(e.VisualAdded as UIElement, canvas.Children.Count + 1); }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD001", Justification = "<挂起>")]
		private void DesignerEntity_SelectionChanged(object sender, CommandChangedEventArgs e)
		{
			_PersistentPane.SetSelectedObjects(e.SelectedValues);
		}
		#endregion
	}
}
