using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Basic.Collections;
using Basic.Configuration;
using Basic.Database;
using Basic.DataContexts;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.Windows
{
	/// <summary>
	/// CreatePersistentWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ColumnsWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly DesignColumnCollection _columns;
		private readonly PersistentService _CommandService;
		public ColumnsWindow(PersistentService commandService, DesignColumnCollection columns)
		{
			_CommandService = commandService; _columns = columns;
			InitializeComponent();
		}


		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
		}

		private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
		{
			dgColumns.ItemsSource = _columns;
		}
	}
}
