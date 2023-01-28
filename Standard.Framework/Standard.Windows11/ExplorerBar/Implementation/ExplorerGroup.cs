using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Basic.Interfaces;

namespace Basic.Windows
{
	/// <summary>
	/// An Expander with animation.
	/// </summary>
	public class ExplorerGroup : HeaderedItemsControl, IKeyTipControl
	{
		static ExplorerGroup()
		{
			MarginProperty.OverrideMetadata(
				 typeof(ExplorerGroup),
				 new FrameworkPropertyMetadata(new Thickness(10, 10, 10, 2)));

			FocusableProperty.OverrideMetadata(typeof(ExplorerGroup),
				 new FrameworkPropertyMetadata(false));

			DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerGroup),
				 new FrameworkPropertyMetadata(typeof(ExplorerGroup)));
		}

		/// <summary>
		/// Gets or sets the custom skin for the control.
		/// </summary>
		public static string Skin { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			ApplySkin();
		}
		/// <summary>
		/// 
		/// </summary>
		public void ApplySkin()
		{
			if (!string.IsNullOrEmpty(Skin))
			{
				Uri uri = new Uri(Skin, UriKind.Absolute);
				ResourceDictionary skin = new ResourceDictionary();
				skin.Source = uri;
				this.Resources = skin;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Brush HeaderBorderBrush
		{
			get { return (Brush)GetValue(HeaderBorderBrushProperty); }
			set { SetValue(HeaderBorderBrushProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty HeaderBorderBrushProperty = DependencyProperty.Register("HeaderBorderBrush",
			 typeof(Brush), typeof(ExplorerGroup), new UIPropertyMetadata(Brushes.Gray));
		/// <summary>
		/// 
		/// </summary>
		public Brush HeaderBackground
		{
			get { return (Brush)GetValue(HeaderBackgroundProperty); }
			set { SetValue(HeaderBackgroundProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground",
			 typeof(Brush), typeof(ExplorerGroup), new UIPropertyMetadata(Brushes.Silver));

		/// <summary>
		/// 
		/// </summary>
		public bool IsMinimized
		{
			get { return (bool)GetValue(IsMinimizedProperty); }
			set { SetValue(IsMinimizedProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register("IsMinimized",
			 typeof(bool), typeof(ExplorerGroup),
			 new UIPropertyMetadata(false, OnMinimizedPropertyChanged));
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		public static void OnMinimizedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			bool minimized = (bool)e.NewValue;
			ExplorerGroup expander = d as ExplorerGroup;
			RoutedEventArgs args = new RoutedEventArgs(minimized ? MinimizedEvent : MaximizedEvent);
			expander.IsEnabled = !minimized;
			expander.RaiseEvent(args);
		}


		/// <summary>
		/// Gets or sets the ImageSource for the image in the header.
		/// </summary>
		public ImageSource Image
		{
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty ImageProperty =
			 DependencyProperty.Register("Image", typeof(ImageSource), typeof(ExplorerGroup), new UIPropertyMetadata(null));


		/// <summary>
		/// 
		/// </summary>
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public event RoutedEventHandler Expanded
		{
			add { AddHandler(ExpandedEvent, value); }
			remove { RemoveHandler(ExpandedEvent, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public event RoutedEventHandler Collapsed
		{
			add { AddHandler(CollapsedEvent, value); }
			remove { RemoveHandler(CollapsedEvent, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public event RoutedEventHandler Minimized
		{
			add { AddHandler(MinimizedEvent, value); }
			remove { RemoveHandler(MinimizedEvent, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public event RoutedEventHandler Maximized
		{
			add { AddHandler(MaximizedEvent, value); }
			remove { RemoveHandler(MaximizedEvent, value); }
		}

		#region dependency properties and routed events definition
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty IsExpandedProperty =
			 DependencyProperty.Register(
			 "IsExpanded",
			 typeof(bool),
			 typeof(ExplorerGroup),
			 new UIPropertyMetadata(true, IsExpandedChanged));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		public static void IsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ExplorerGroup expander = d as ExplorerGroup;
			RoutedEventArgs args = new RoutedEventArgs((bool)e.NewValue ? ExpandedEvent : CollapsedEvent);
			expander.RaiseEvent(args);
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(
			 "ExpandedEvent",
			 RoutingStrategy.Bubble,
			 typeof(RoutedEventHandler),
			 typeof(ExplorerGroup));

		/// <summary>
		/// 
		/// </summary>
		public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(
			 "CollapsedEvent",
			 RoutingStrategy.Bubble,
			 typeof(RoutedEventHandler),
			 typeof(ExplorerGroup));
		/// <summary>
		/// 
		/// </summary>
		public static readonly RoutedEvent MinimizedEvent = EventManager.RegisterRoutedEvent(
			  "MinimizedEvent",
			  RoutingStrategy.Bubble,
			  typeof(RoutedEventHandler),
			  typeof(ExplorerGroup));

		/// <summary>
		/// 
		/// </summary>
		public static readonly RoutedEvent MaximizedEvent = EventManager.RegisterRoutedEvent(
			 "MaximizedEvent",
			 RoutingStrategy.Bubble,
			 typeof(RoutedEventHandler),
			 typeof(ExplorerGroup));

		#endregion


		/// <summary>
		/// Gets or sets the corner radius for the header.
		/// </summary>
		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty CornerRadiusProperty =
			 DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ExplorerGroup), new UIPropertyMetadata(null));



		/// <summary>
		/// Gets or sets the background color of the header on mouse over.
		/// </summary>
		public Brush MouseOverHeaderBackground
		{
			get { return (Brush)GetValue(MouseOverHeaderBackgroundProperty); }
			set { SetValue(MouseOverHeaderBackgroundProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty MouseOverHeaderBackgroundProperty =
			 DependencyProperty.Register("MouseOverHeaderBackground", typeof(Brush), typeof(ExplorerGroup), new UIPropertyMetadata(null));


		/// <summary>
		/// Gets whether the PressedBackground is not null.
		/// </summary>
		public bool HasPressedBackground
		{
			get { return (bool)GetValue(HasPressedBackgroundProperty); }
			set { SetValue(HasPressedBackgroundProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty HasPressedBackgroundProperty =
			 DependencyProperty.Register("HasPressedBackground", typeof(bool), typeof(ExplorerGroup), new UIPropertyMetadata(false));



		/// <summary>
		/// Gets or sets the background color of the header in pressed mode.
		/// </summary>
		public Brush PressedHeaderBackground
		{
			get { return (Brush)GetValue(PressedHeaderBackgroundProperty); }
			set { SetValue(PressedHeaderBackgroundProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty PressedHeaderBackgroundProperty =
			 DependencyProperty.Register("PressedHeaderBackground", typeof(Brush), typeof(ExplorerGroup),
			 new UIPropertyMetadata(null, PressedHeaderBackgroundPropertyChangedCallback));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		public static void PressedHeaderBackgroundPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ExplorerGroup expander = (ExplorerGroup)d;
			expander.HasPressedBackground = e.NewValue != null;
		}
		/// <summary>
		/// 
		/// </summary>
		public Thickness HeaderBorderThickness
		{
			get { return (Thickness)GetValue(HeaderBorderThicknessProperty); }
			set { SetValue(HeaderBorderThicknessProperty, value); }
		}

		// Using a DependencyProperty as the backing store for HeaderBorderThickness.  This enables animation, styling, binding, etc...
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty HeaderBorderThicknessProperty =
			 DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(ExplorerGroup), new UIPropertyMetadata(null));




		/// <summary>
		/// Gets or sets the foreground color of the header on mouse over.
		/// </summary>
		public Brush MouseOverHeaderForeground
		{
			get { return (Brush)GetValue(MouseOverHeaderForegroundProperty); }
			set { SetValue(MouseOverHeaderForegroundProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty MouseOverHeaderForegroundProperty =
			 DependencyProperty.Register("MouseOverHeaderForeground", typeof(Brush), typeof(ExplorerGroup), new UIPropertyMetadata(null));



		/// <summary>
		/// Specifies whether to show a elipse with the expanded/collapsed image.
		/// </summary>
		public bool ShowEllipse
		{
			get { return (bool)GetValue(ShowEllipseProperty); }
			set { SetValue(ShowEllipseProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty ShowEllipseProperty =
			 DependencyProperty.Register("ShowEllipse", typeof(bool), typeof(ExplorerGroup), new UIPropertyMetadata(false));




		/// <summary>
		/// Gets or sets whether animation is possible
		/// </summary>
		public bool CanAnimate
		{
			get { return (bool)GetValue(CanAnimateProperty); }
			set { SetValue(CanAnimateProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty CanAnimateProperty =
			 DependencyProperty.Register("CanAnimate", typeof(bool), typeof(ExplorerGroup), new UIPropertyMetadata(true));



		/// <summary>
		/// 
		/// </summary>
		public bool IsHeaderVisible
		{
			get { return (bool)GetValue(IsHeaderVisibleProperty); }
			set { SetValue(IsHeaderVisibleProperty, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty IsHeaderVisibleProperty =
			 DependencyProperty.Register("IsHeaderVisible", typeof(bool), typeof(ExplorerGroup), new UIPropertyMetadata(true));

		#region IKeyTipControl Members
		/// <summary>
		/// 
		/// </summary>
		void IKeyTipControl.ExecuteKeyTip()
		{
			this.IsExpanded ^= true;
			if (this.IsExpanded)
			{
				//FrameworkElement e = Content as FrameworkElement;
				//if (e != null && e.Focusable) e.Focus();
			}
		}

		#endregion
	}
}
