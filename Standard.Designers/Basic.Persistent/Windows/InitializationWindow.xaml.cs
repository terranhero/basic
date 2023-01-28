using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Basic.DataContexts;
using Basic.Enums;
using Basic.Database;
using Basic.Configuration;

namespace Basic.Windows
{
	/// <summary>
	/// InitializationWindow.xaml 的交互逻辑
	/// </summary>
	public partial class InitializationWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly PersistentConfiguration persistentConfiguration;
		public InitializationWindow(PersistentConfiguration persistent)
		{
			persistentConfiguration = persistent;
			InitializeComponent();
			base.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			if (tvTables.SelectedItem != null)
			{
				DesignTableInfo tableInfo = tvTables.SelectedItem as DesignTableInfo;
				using (IDataContext context = DataContextFactory.CreateDbAccess())
				{
					persistentConfiguration.TableInfo.CopyFrom(tableInfo);
					context.GetColumns(persistentConfiguration.TableInfo);
				}
				this.Close();
			}
		}

		private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				using (IDataContext context = DataContextFactory.CreateDbAccess())
				{
					tvTables.ItemsSource = context.GetTables(ObjectTypeEnum.UserTable);
				}
			}));
		}
	}
}
