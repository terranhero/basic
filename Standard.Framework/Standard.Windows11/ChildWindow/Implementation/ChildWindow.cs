// ***********************************************************************
// Assembly         : Basic.Windows
// Author           : JACKY
// Created          : 12-19-2014
//
// Last Modified By : JACKY
// Last Modified On : 12-19-2014
// ***********************************************************************
// <copyright file="ChildWindow.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
//using Basic.Windows.Utilities;

namespace Basic.Windows
{
#pragma warning disable 0809
#pragma warning disable 0618

	/// <summary>
	/// Class ChildWindow
	/// </summary>
	[TemplatePart(Name = PART_WindowRoot, Type = typeof(Grid))]
	[TemplatePart(Name = PART_Root, Type = typeof(Grid))]
	public class ChildWindow : System.Windows.Controls.ContentControl
	{
		/// <summary>
		/// The PAR t_ window root
		/// </summary>
		private const string PART_WindowRoot = "PART_WindowRoot";
		/// <summary>
		/// The PAR t_ root
		/// </summary>
		private const string PART_Root = "PART_Root";
		/// <summary>
		/// The PAR t_ window control
		/// </summary>
		private const string PART_WindowControl = "PART_WindowControl";
		/// <summary>
		/// The _horizontal offset
		/// </summary>
		private const int _horizontalOffset = 3;
		/// <summary>
		/// The _vertical offset
		/// </summary>
		private const int _verticalOffset = 3;

		#region Private Members

		/// <summary>
		/// The _root
		/// </summary>
		private Grid _root;
		/// <summary>
		/// The _move transform
		/// </summary>
		private TranslateTransform _moveTransform = new TranslateTransform();
		/// <summary>
		/// The _startup position initialized
		/// </summary>
		private bool _startupPositionInitialized;
		/// <summary>
		/// The _parent container
		/// </summary>
		private FrameworkElement _parentContainer;
		/// <summary>
		/// The _modal layer
		/// </summary>
		private Rectangle _modalLayer = new Rectangle();
		/// <summary>
		/// The _modal layer panel
		/// </summary>
		private Canvas _modalLayerPanel = new Canvas();
		/// <summary>
		/// The _window root
		/// </summary>
		private Grid _windowRoot;
		/// <summary>
		/// The _ignore property changed
		/// </summary>
		private bool _ignorePropertyChanged;
		/// <summary>
		/// The _has window container
		/// </summary>
		private bool _hasWindowContainer;

		#endregion //Private Members

		#region Public Properties

		#region DialogResult

		/// <summary>
		/// The _dialog result
		/// </summary>
		private bool? _dialogResult;
		/// <summary>
		/// Gets or sets a value indicating whether the ChildWindow was accepted or canceled.
		/// </summary>
		/// <value>True if the child window was accepted; false if the child window was
		/// canceled. The default is null.</value>
		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? DialogResult
		{
			get
			{
				return _dialogResult;
			}
			set
			{
				if (_dialogResult != value)
				{
					_dialogResult = value;
					this.Close();
				}
			}
		}

		#endregion //DialogResult

		#region DesignerWindowState
		/// <summary>
		/// The designer window state property
		/// </summary>
		public static readonly DependencyProperty DesignerWindowStateProperty = DependencyProperty.Register("DesignerWindowState",
			typeof(WindowState), typeof(ChildWindow), new PropertyMetadata(WindowState.Closed, OnDesignerWindowStatePropertyChanged));
		/// <summary>
		/// Gets or sets the state of the designer window.
		/// </summary>
		/// <value>The state of the designer window.</value>
		public WindowState DesignerWindowState
		{
			get
			{
				return (WindowState)GetValue(DesignerWindowStateProperty);
			}
			set
			{
				SetValue(DesignerWindowStateProperty, value);
			}
		}

		/// <summary>
		/// Called when [designer window state property changed].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnDesignerWindowStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow childWindow = d as ChildWindow;
			if (childWindow != null)
				childWindow.OnDesignerWindowStatePropertyChanged((WindowState)e.OldValue, (WindowState)e.NewValue);
		}

		/// <summary>
		/// Called when [designer window state property changed].
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnDesignerWindowStatePropertyChanged(WindowState oldValue, WindowState newValue)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				Visibility = newValue == WindowState.Open ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		#endregion //DesignerWindowState

		#region FocusedElement
		/// <summary>
		/// The focused element property
		/// </summary>
		public static readonly DependencyProperty FocusedElementProperty = DependencyProperty.Register("FocusedElement", typeof(FrameworkElement), typeof(ChildWindow), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the focused element.
		/// </summary>
		/// <value>The focused element.</value>
		public FrameworkElement FocusedElement
		{
			get
			{
				return (FrameworkElement)GetValue(FocusedElementProperty);
			}
			set
			{
				SetValue(FocusedElementProperty, value);
			}
		}

		#endregion

		#region IsModal
		/// <summary>
		/// The is modal property
		/// </summary>
		public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register("IsModal",
			typeof(bool), typeof(ChildWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsModalPropertyChanged)));
		/// <summary>
		/// Gets or sets a value indicating whether this instance is modal.
		/// </summary>
		/// <value><c>true</c> if this instance is modal; otherwise, <c>false</c>.</value>
		public bool IsModal
		{
			get
			{
				return (bool)GetValue(IsModalProperty);
			}
			set
			{
				SetValue(IsModalProperty, value);
			}
		}

		/// <summary>
		/// Called when [is modal property changed].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnIsModalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow childWindow = d as ChildWindow;
			if (childWindow != null)
				childWindow.OnIsModalChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		internal event EventHandler<EventArgs> IsModalChanged;

		/// <summary>
		/// Called when [is modal changed].
		/// </summary>
		/// <param name="oldValue">if set to <c>true</c> [old value].</param>
		/// <param name="newValue">if set to <c>true</c> [new value].</param>
		private void OnIsModalChanged(bool oldValue, bool newValue)
		{
			EventHandler<EventArgs> handler = IsModalChanged;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}

			if (!_hasWindowContainer)
			{
				if (newValue)
				{
					KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
					ShowModalLayer();
				}
				else
				{
					KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Continue);
					HideModalLayer();
				}
			}
		}

		#endregion //IsModal

		#region OverlayBrush (Obsolete)

		/// <summary>
		/// The overlay brush property
		/// </summary>
		[Obsolete("This property is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
		public static readonly DependencyProperty OverlayBrushProperty = DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(ChildWindow), new PropertyMetadata(Brushes.Gray, OnOverlayBrushChanged));
		/// <summary>
		/// Gets or sets the overlay brush.
		/// </summary>
		/// <value>The overlay brush.</value>
		[Obsolete("This property is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
		public Brush OverlayBrush
		{
			get
			{
				return (Brush)GetValue(OverlayBrushProperty);
			}
			set
			{
				SetValue(OverlayBrushProperty, value);
			}
		}

		/// <summary>
		/// Called when [overlay brush changed].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnOverlayBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow childWindow = d as ChildWindow;
			if (childWindow != null)
				childWindow.OnOverlayBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
		}

		/// <summary>
		/// Called when [overlay brush changed].
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		[Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
		protected virtual void OnOverlayBrushChanged(Brush oldValue, Brush newValue)
		{
			_modalLayer.Fill = newValue;
		}

		#endregion //OverlayBrush

		#region OverlayOpacity (Obsolete)

		/// <summary>
		/// The overlay opacity property
		/// </summary>
		[Obsolete("This property is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
		public static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register("OverlayOpacity", typeof(double), typeof(ChildWindow), new PropertyMetadata(0.5, OnOverlayOpacityChanged));
		/// <summary>
		/// Gets or sets the overlay opacity.
		/// </summary>
		/// <value>The overlay opacity.</value>
		[Obsolete("This property is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
		public double OverlayOpacity
		{
			get
			{
				return (double)GetValue(OverlayOpacityProperty);
			}
			set
			{
				SetValue(OverlayOpacityProperty, value);
			}
		}

		/// <summary>
		/// Called when [overlay opacity changed].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnOverlayOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow childWindow = d as ChildWindow;
			if (childWindow != null)
				childWindow.OnOverlayOpacityChanged((double)e.OldValue, (double)e.NewValue);
		}

		/// <summary>
		/// Called when [overlay opacity changed].
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		[Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
		protected virtual void OnOverlayOpacityChanged(double oldValue, double newValue)
		{
			_modalLayer.Opacity = newValue;
		}

		#endregion //OverlayOpacity

		#region WindowStartupLocation

		/// <summary>
		/// The window startup location property
		/// </summary>
		public static readonly DependencyProperty WindowStartupLocationProperty = DependencyProperty.Register("WindowStartupLocation", typeof(WindowStartupLocation), typeof(ChildWindow), new UIPropertyMetadata(WindowStartupLocation.Manual, OnWindowStartupLocationChanged));
		/// <summary>
		/// Gets or sets the window startup location.
		/// </summary>
		/// <value>The window startup location.</value>
		public WindowStartupLocation WindowStartupLocation
		{
			get
			{
				return (WindowStartupLocation)GetValue(WindowStartupLocationProperty);
			}
			set
			{
				SetValue(WindowStartupLocationProperty, value);
			}
		}

		/// <summary>
		/// Called when [window startup location changed].
		/// </summary>
		/// <param name="o">The o.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnWindowStartupLocationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow childWindow = o as ChildWindow;
			if (childWindow != null)
				childWindow.OnWindowStartupLocationChanged((WindowStartupLocation)e.OldValue, (WindowStartupLocation)e.NewValue);
		}

		/// <summary>
		/// Called when [window startup location changed].
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnWindowStartupLocationChanged(WindowStartupLocation oldValue, WindowStartupLocation newValue)
		{
			// TODO: Add your property changed side-effects. Descendants can override as well.
		}

		#endregion //WindowStartupLocation

		#region WindowState

		/// <summary>
		/// The window state property
		/// </summary>
		public static readonly DependencyProperty WindowStateProperty = DependencyProperty.Register("WindowState", typeof(WindowState), typeof(ChildWindow),
			new FrameworkPropertyMetadata(WindowState.Closed, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnWindowStatePropertyChanged));
		/// <summary>
		/// Gets or sets the state of the window.
		/// </summary>
		/// <value>The state of the window.</value>
		public WindowState WindowState
		{
			get
			{
				return (WindowState)GetValue(WindowStateProperty);
			}
			set
			{
				SetValue(WindowStateProperty, value);
			}
		}

		/// <summary>
		/// Called when [window state property changed].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnWindowStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow childWindow = d as ChildWindow;
			if (childWindow != null)
				childWindow.OnWindowStatePropertyChanged((WindowState)e.OldValue, (WindowState)e.NewValue);
		}

		/// <summary>
		/// Called when [window state property changed].
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnWindowStatePropertyChanged(WindowState oldValue, WindowState newValue)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (!_ignorePropertyChanged)
					SetWindowState(newValue);
			}
			else
			{
				Visibility = DesignerWindowState == WindowState.Open ? Visibility.Visible : System.Windows.Visibility.Collapsed;
			}
		}

		#endregion //WindowState

		#endregion //Public Properties

		#region Constructors

		/// <summary>
		/// Initializes static members of the <see cref="ChildWindow" /> class.
		/// </summary>
		static ChildWindow() { DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow), new FrameworkPropertyMetadata(typeof(ChildWindow))); }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChildWindow" /> class.
		/// </summary>
		public ChildWindow()
		{
			DesignerWindowState = WindowState.Open;

			_modalLayer.Fill = OverlayBrush;
			_modalLayer.Opacity = OverlayOpacity;
		}

		#endregion //Constructors

		#region Base Class Overrides

		/// <summary>
		/// Called when [apply template].
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			//this.UpdateBlockMouseInputsPanel();

			_windowRoot = this.GetTemplateChild(PART_WindowRoot) as Grid;
			if (_windowRoot != null)
			{
				_windowRoot.RenderTransform = _moveTransform;
			}
			_hasWindowContainer = false;//= (VisualTreeHelper.GetParent(this) as WindowContainer) != null;

			if (!_hasWindowContainer)
			{
				_parentContainer = VisualTreeHelper.GetParent(this) as FrameworkElement;
				if (_parentContainer != null)
				{
					_parentContainer.LayoutUpdated += ParentContainer_LayoutUpdated;
					_parentContainer.SizeChanged += ParentContainer_SizeChanged;

					//this is for XBAP applications only. When inside an XBAP the parent container has no height or width until it has loaded. Therefore
					//we need to handle the loaded event and reposition the window.
					if (System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
					{
						_parentContainer.Loaded += (o, e) =>
						{
							ExecuteOpen();
						};
					}
				}

				this.Unloaded += new RoutedEventHandler(ChildWindow_Unloaded);

				//initialize our modal background width/height
				_modalLayer.Height = _parentContainer.ActualHeight;
				_modalLayer.Width = _parentContainer.ActualWidth;

				_root = this.GetTemplateChild(PART_Root) as Grid;

#if VS2008
      FocusVisualStyle = null;
#else
				Style focusStyle = (_root != null) ? _root.Resources["FocusVisualStyle"] as Style : null;
				if (focusStyle != null)
				{
					Setter focusStyleDataContext = new Setter(Control.DataContextProperty, this);
					focusStyle.Setters.Add(focusStyleDataContext);
					FocusVisualStyle = focusStyle;
				}
#endif
				if (_root != null)
				{
					_root.Children.Add(_modalLayerPanel);
				}
			}
		}

		/// <summary>
		/// 每当未处理的 <see cref="E:System.Windows.UIElement.GotFocus" /> 事件在其路由中到达此元素时调用。
		/// </summary>
		/// <param name="e">包含事件数据的 <see cref="T:System.Windows.RoutedEventArgs" />。</param>
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			Action action = () =>
			{
				if (FocusedElement != null)
					FocusedElement.Focus();
			};

			Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
		}

		/// <summary>
		/// 当未处理的 <see cref="E:System.Windows.Input.Keyboard.KeyDown" /> 附加事件在其路由中到达派生自此类的元素时，调用此方法。实现此方法可为此事件添加类处理。
		/// </summary>
		/// <param name="e">包含事件数据的 <see cref="T:System.Windows.Input.KeyEventArgs" />。</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (WindowState == WindowState.Open)
			{
				//switch (e.Key)
				//{
				//   case Key.Left:
				//      this.Left -= _horizontalOffset;
				//      e.Handled = true;
				//      break;

				//   case Key.Right:
				//      this.Left += _horizontalOffset;
				//      e.Handled = true;
				//      break;

				//   case Key.Down:
				//      this.Top += _verticalOffset;
				//      e.Handled = true;
				//      break;

				//   case Key.Up:
				//      this.Top -= _verticalOffset;
				//      e.Handled = true;
				//      break;
				//}
			}
		}

		#endregion //Base Class Overrides

		#region Left Property

		/// <summary>
		/// The left property
		/// </summary>
		public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double),
			typeof(ChildWindow), new PropertyMetadata(0.0, new PropertyChangedCallback(OnLeftPropertyChanged), OnCoerceLeft));
		/// <summary>
		/// Gets or sets the left.
		/// </summary>
		/// <value>The left.</value>
		public double Left
		{
			get
			{
				return (double)GetValue(LeftProperty);
			}
			set
			{
				SetValue(LeftProperty, value);
			}
		}

		/// <summary>
		/// Called when [coerce left].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="basevalue">The basevalue.</param>
		/// <returns>System.Object.</returns>
		private static object OnCoerceLeft(DependencyObject d, object basevalue)
		{
			if (basevalue == DependencyProperty.UnsetValue)
				return basevalue;

			var windowControl = (ChildWindow)d;
			if (windowControl == null)
				return basevalue;

			return windowControl.OnCoerceLeft(basevalue);
		}

		/// <summary>
		/// Called when [coerce left].
		/// </summary>
		/// <param name="newValue">The new value.</param>
		/// <returns>System.Object.</returns>
		private object OnCoerceLeft(object newValue)
		{
			var value = (double)newValue;
			if (object.Equals(value, double.NaN))
				return 0.0;

			return newValue;
		}

		/// <summary>
		/// Called when [left property changed].
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnLeftPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow window = obj as ChildWindow;
			if (window != null)
				window.OnLeftPropertyChanged((double)e.OldValue, (double)e.NewValue);
		}

		internal event EventHandler<EventArgs> LeftChanged;

		/// <summary>
		/// Called when [left property changed].
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnLeftPropertyChanged(double oldValue, double newValue)
		{
			EventHandler<EventArgs> handler = LeftChanged;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

		#endregion //Left

		#region Top Property

		/// <summary>
		/// The top property
		/// </summary>
		public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double),
			typeof(ChildWindow), new PropertyMetadata(0.0, new PropertyChangedCallback(OnTopPropertyChanged), OnCoerceTop));
		/// <summary>
		/// Gets or sets the top.
		/// </summary>
		/// <value>The top.</value>
		public double Top
		{
			get
			{
				return (double)GetValue(TopProperty);
			}
			set
			{
				SetValue(TopProperty, value);
			}
		}

		/// <summary>
		/// Called when [coerce top].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="basevalue">The basevalue.</param>
		/// <returns>System.Object.</returns>
		private static object OnCoerceTop(DependencyObject d, object basevalue)
		{
			if (basevalue == DependencyProperty.UnsetValue)
				return basevalue;

			var windowControl = (ChildWindow)d;
			if (windowControl == null)
				return basevalue;

			return windowControl.OnCoerceTop(basevalue);
		}

		/// <summary>
		/// Called when [coerce top].
		/// </summary>
		/// <param name="newValue">The new value.</param>
		/// <returns>System.Object.</returns>
		private object OnCoerceTop(object newValue)
		{
			var value = (double)newValue;
			if (object.Equals(value, double.NaN))
				return 0.0;

			return newValue;
		}

		/// <summary>
		/// Called when [top property changed].
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnTopPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow window = obj as ChildWindow;
			if (window != null)
				window.OnTopPropertyChanged((double)e.OldValue, (double)e.NewValue);
		}

		/// <summary>
		/// Called when [top property changed].
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnTopPropertyChanged(double oldValue, double newValue)
		{
			EventHandler<EventArgs> handler = TopChanged;
			if (handler != null) { handler(this, EventArgs.Empty); }
		}

		internal event EventHandler<EventArgs> TopChanged;

		#endregion //TopProperty

		#region Event Handlers

		/// <summary>
		/// Handles the LayoutUpdated event of the ParentContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		[Obsolete("This method is obsolete and should no longer be used.")]
		private void ParentContainer_LayoutUpdated(object sender, EventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;

			//we only want to set the start position if this is the first time the control has bee initialized
			if (!_startupPositionInitialized)
			{
				ExecuteOpen();
				_startupPositionInitialized = true;
			}
		}

		/// <summary>
		/// Handles the Unloaded event of the ChildWindow control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
		[Obsolete("This method is obsolete and should no longer be used.")]
		private void ChildWindow_Unloaded(object sender, RoutedEventArgs e)
		{
			if (_parentContainer != null)
			{
				_parentContainer.LayoutUpdated -= ParentContainer_LayoutUpdated;
				_parentContainer.SizeChanged -= ParentContainer_SizeChanged;

				//this is for XBAP applications only. When inside an XBAP the parent container has no height or width until it has loaded. Therefore
				//we need to handle the loaded event and reposition the window.
				if (System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
				{
					_parentContainer.Loaded -= (o, ev) =>
					{
						ExecuteOpen();
					};
				}
			}
		}

		/// <summary>
		/// Handles the SizeChanged event of the ParentContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="SizeChangedEventArgs" /> instance containing the event data.</param>
		[Obsolete("This method is obsolete and should no longer be used.")]
		void ParentContainer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			//resize our modal layer
			_modalLayer.Height = e.NewSize.Height;
			_modalLayer.Width = e.NewSize.Width;

			//reposition our window
			Left = GetRestrictedLeft();
			Top = GetRestrictedTop();
		}



		#endregion //Event Handlers

		#region Methods

		#region Private



		/// <summary>
		/// Gets the restricted left.
		/// </summary>
		/// <returns>System.Double.</returns>
		[Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.GetRestrictedLeft() instead.")]
		private double GetRestrictedLeft()
		{
			if (Left < 0)
				return 0;

			if ((_parentContainer != null) && (_windowRoot != null))
			{
				if (Left + _windowRoot.ActualWidth > _parentContainer.ActualWidth && _parentContainer.ActualWidth != 0)
				{
					double left = _parentContainer.ActualWidth - _windowRoot.ActualWidth;
					return left < 0 ? 0 : left;
				}
			}

			return Left;
		}

		/// <summary>
		/// Gets the restricted top.
		/// </summary>
		/// <returns>System.Double.</returns>
		[Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.GetRestrictedTop() instead.")]
		private double GetRestrictedTop()
		{
			if (Top < 0)
				return 0;

			if ((_parentContainer != null) && (_windowRoot != null))
			{
				if (Top + _windowRoot.ActualHeight > _parentContainer.ActualHeight && _parentContainer.ActualHeight != 0)
				{
					double top = _parentContainer.ActualHeight - _windowRoot.ActualHeight;
					return top < 0 ? 0 : top;
				}
			}

			return Top;
		}

		/// <summary>
		/// Sets the state of the window.
		/// </summary>
		/// <param name="state">The state.</param>
		private void SetWindowState(WindowState state)
		{
			switch (state)
			{
				case WindowState.Closed:
					{
						ExecuteClose();
						break;
					}
				case WindowState.Open:
					{
						ExecuteOpen();
						break;
					}
			}
		}

		/// <summary>
		/// Executes the close.
		/// </summary>
		private void ExecuteClose()
		{
			CancelEventArgs e = new CancelEventArgs();
			OnClosing(e);

			if (!e.Cancel)
			{
				if (!_dialogResult.HasValue)
					_dialogResult = false;

				OnClosed(EventArgs.Empty);
			}
			else
			{
				CancelClose();
			}
		}

		/// <summary>
		/// Cancels the close.
		/// </summary>
		private void CancelClose()
		{
			_dialogResult = null; //when the close is cancelled, DialogResult should be null

			_ignorePropertyChanged = true;
			WindowState = WindowState.Open; //now reset the window state to open because the close was cancelled
			_ignorePropertyChanged = false;
		}

		/// <summary>
		/// Executes the open.
		/// </summary>
		private void ExecuteOpen()
		{
			_dialogResult = null; //reset the dialogResult to null each time the window is opened

			if (!_hasWindowContainer)
				if (WindowStartupLocation == WindowStartupLocation.Center)
					CenterChildWindow();

			if (!_hasWindowContainer)
				BringToFront();
		}

		/// <summary>
		/// Brings to front.
		/// </summary>
		[Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.BringToFront() instead.")]
		private void BringToFront()
		{
			int index = 0;

			if (_parentContainer != null)
				index = (int)_parentContainer.GetValue(Canvas.ZIndexProperty);

			SetValue(Canvas.ZIndexProperty, ++index);

			if (IsModal)
				Canvas.SetZIndex(_modalLayerPanel, index - 2);
		}

		/// <summary>
		/// Centers the child window.
		/// </summary>
		[Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.CenterChild() instead.")]
		private void CenterChildWindow()
		{
			if ((_parentContainer != null) && (_windowRoot != null))
			{
				Left = (_parentContainer.ActualWidth - _windowRoot.ActualWidth) / 2.0;
				Top = (_parentContainer.ActualHeight - _windowRoot.ActualHeight) / 2.0;
			}
		}

		/// <summary>
		/// Shows the modal layer.
		/// </summary>
		[Obsolete("This method is obsolete and should no longer be used.")]
		private void ShowModalLayer()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (!_modalLayerPanel.Children.Contains(_modalLayer))
					_modalLayerPanel.Children.Add(_modalLayer);

				_modalLayer.Visibility = System.Windows.Visibility.Visible;
			}
		}

		/// <summary>
		/// Hides the modal layer.
		/// </summary>
		[Obsolete("This method is obsolete and should no longer be used.")]
		private void HideModalLayer()
		{
			_modalLayer.Visibility = System.Windows.Visibility.Collapsed;
		}

		/// <summary>
		/// Processes the move.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		[Obsolete("This method is obsolete and should no longer be used. Use the ChildWindow in a WindowContainer instead.")]
		private void ProcessMove(double x, double y)
		{
			_moveTransform.X += x;
			_moveTransform.Y += y;

			InvalidateArrange();
		}

		#endregion //Private

		#region Public

		/// <summary>
		/// Shows this instance.
		/// </summary>
		public void Show()
		{
			WindowState = WindowState.Open;
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public void Close()
		{
			WindowState = WindowState.Closed;
		}

		#endregion //Public

		#endregion //Methods

		#region Events

		/// <summary>
		/// Occurs when the ChildWindow is closed.
		/// </summary>
		public event EventHandler Closed;
		/// <summary>
		/// Raises the <see cref="E:Closed" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected virtual void OnClosed(EventArgs e)
		{
			if (Closed != null)
				Closed(this, e);
		}

		/// <summary>
		/// Occurs when the ChildWindow is closing.
		/// </summary>
		public event EventHandler<CancelEventArgs> Closing;
		/// <summary>
		/// Raises the <see cref="E:Closing" /> event.
		/// </summary>
		/// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
		protected virtual void OnClosing(CancelEventArgs e)
		{
			if (Closing != null)
				Closing(this, e);
		}

		#endregion //Events

	}

#pragma warning restore 0809
#pragma warning restore 0618
}
