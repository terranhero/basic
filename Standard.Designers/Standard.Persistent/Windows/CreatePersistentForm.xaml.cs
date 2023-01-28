using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
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
	public partial class CreatePersistentForm : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly EnvDTE.ProjectItems _ProjectItems;
		private readonly BackgroundWorker _BackgroundWorker;
		private readonly string _DefaultNamespace, _FullPath;
		private readonly PersistentService _CommandService;
		public CreatePersistentForm(PersistentService commandService, EnvDTE.Project project, EnvDTE.ProjectItem item)
		{
			_CommandService = commandService;
			if (item != null) { _ProjectItems = item.ProjectItems; }
			else { _ProjectItems = project.ProjectItems; }
			_BackgroundWorker = new BackgroundWorker();
			_BackgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
			_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
			InitializeComponent();
			if (item == null)
			{
				EnvDTE.Property propertyNamespace = project.Properties.Item("DefaultNamespace");
				EnvDTE.Property propertyFullPath = project.Properties.Item("FullPath");
				_DefaultNamespace = (string)propertyNamespace.Value;
				_FullPath = (string)propertyFullPath.Value;
			}
			else
			{
				EnvDTE.Property propertyNamespace = item.Properties.Item("DefaultNamespace");
				EnvDTE.Property propertyFullPath = item.Properties.Item("FullPath");
				_DefaultNamespace = (string)propertyNamespace.Value;
				_FullPath = (string)propertyFullPath.Value;
			}
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			lstTables.IsEnabled = cmbConnections.IsEnabled = true;
			lstTables.ItemsSource = (ObservableCollection<DesignTableInfo>)e.Result;
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			ConnectionInfo info = (ConnectionInfo)e.Argument;
			using (IDataContext context = DataContextFactory.CreateDbAccess(info))
			{
				e.Result = new ObservableCollection<DesignTableInfo>(context.GetTables(ObjectTypeEnum.UserTable));
			}
		}

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (lstTables.SelectedItem != null)
			{
				PersistentConfiguration persistent = new PersistentConfiguration();
				ConnectionInfo info = (ConnectionInfo)cmbConnections.SelectedItem;
				if (info.ConnectionType == ConnectionType.SqlConnection)
					persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.SQLSERVER };
				else if (info.ConnectionType == ConnectionType.OracleConnection)
					persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.ORACLE };
				else if (info.ConnectionType == ConnectionType.MySqlConnection)
					persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.MYSQL };
				else if (info.ConnectionType == ConnectionType.Db2Connection)
					persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.DB2 };
				else if (info.ConnectionType == ConnectionType.NpgSqlConnection)
					persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.NPGSQL };

				persistent.Namespace = _DefaultNamespace;
				persistent.EntityNamespace = _DefaultNamespace;
				persistent.ImportNameSpaces.Add(typeof(IPagination).Namespace);
				persistent.ImportNameSpaces.Add(typeof(AbstractEntity).Namespace);
				persistent.ImportNameSpaces.Add(typeof(DbTypeEnum).Namespace);
				DesignTableInfo tableInfo = lstTables.SelectedItem as DesignTableInfo;
				using (IDataContext context = DataContextFactory.CreateDbAccess(info))
				{
					persistent.TableInfo.CopyFrom(tableInfo);
					context.GetColumns(persistent.TableInfo);
					persistent.GroupName = persistent.TableInfo.EntityName;
					persistent.PublicGroupName = persistent.TableInfo.EntityName;
				}
				string filePath = string.Concat(_FullPath, persistent.TableInfo.EntityName, ".dpdl");
				using (XmlWriter writer = XmlWriter.Create(filePath))
				{
					persistent.WriteXml(writer);
				}
				EnvDTE.ProjectItem item = _ProjectItems.AddFromFile(filePath);
				EnvDTE.Property property = item.Properties.Item("CustomTool");
				property.Value = "AccessGenerator";
				item.Open(); this.Close();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_BackgroundWorker.IsBusy == true) { e.Cancel = true; }
			base.OnClosing(e);
		}

		private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
		{
			ConnectionConfiguration config = new ConnectionConfiguration(_CommandService);
			if (config.ReadConfig(_ProjectItems.ContainingProject))
			{
				IList<ConnectionInfo> connections = ConnectionContext.GetConnections();
				cmbConnections.ItemsSource = connections;
				cmbConnections.SelectedItem = ConnectionContext.DefaultConnection;
				lstTables.IsEnabled = cmbConnections.IsEnabled = false;
				if (_BackgroundWorker.IsBusy == false) { _BackgroundWorker.RunWorkerAsync(ConnectionContext.DefaultConnection); }
			}
		}

		private void OnConnectionsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbConnections.SelectedItem == null) { return; }
			ConnectionInfo info = (ConnectionInfo)cmbConnections.SelectedItem;
			lstTables.IsEnabled = cmbConnections.IsEnabled = false;
			if (_BackgroundWorker.IsBusy == false) { _BackgroundWorker.RunWorkerAsync(info); }
		}

		private void OnTablesSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (lstTables.SelectedItem != null)
			{
				txtEntityName.DataContext = lstTables.SelectedItem;
				btnOk.IsEnabled = true;
			}
		}
	}
}
