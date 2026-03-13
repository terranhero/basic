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
	/// PersistentForm.xaml 的交互逻辑
	/// </summary>
	public class CreatePersistent : System.Windows.Controls.Control
	{
		static CreatePersistent()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CreatePersistent),
				new FrameworkPropertyMetadata(typeof(CreatePersistent)));
		}

		private readonly EnvDTE.ProjectItems _ProjectItems;
		private readonly BackgroundWorker _BackgroundWorker;
		private readonly string _DefaultNamespace, _FullPath;
		private readonly PersistentService _CommandService;
		public CreatePersistent(PersistentService commandService, EnvDTE.Project project, EnvDTE.ProjectItem item)
		{
			//this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSaveExecuted));
			_CommandService = commandService;
			if (item != null) { _ProjectItems = item.ProjectItems; }
			else { _ProjectItems = project.ProjectItems; }
			_BackgroundWorker = new BackgroundWorker();
			_BackgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
			_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
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
			if (_cmbConnections != null && _lstTables != null)
			{
				_lstTables.IsEnabled = _cmbConnections.IsEnabled = true;
				_lstTables.ItemsSource = (ObservableCollection<DesignTableInfo>)e.Result;
			}
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			ConnectionInfo info = (ConnectionInfo)e.Argument;
			using (IDataContext context = DataContextFactory.CreateDbAccess(info))
			{
				e.Result = new ObservableCollection<DesignTableInfo>(context.GetTables(ObjectTypeEnum.UserTable));
			}
		}


		private System.Windows.Controls.ComboBox _cmbConnections;
		private System.Windows.Controls.ListBox _lstTables;


		//this.lstTables = ((System.Windows.Controls.ListBox)(target));

		private System.Windows.Controls.TextBox txtEntityName;
		private Microsoft.VisualStudio.PlatformUI.DialogButton btnOk;
		private Microsoft.VisualStudio.PlatformUI.DialogButton btnCancel;

		#region 重载 OnApplyTemplate 方法，绑定 PART_TREEVIEW 事件
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			// 查找模板中的控件
			if (GetTemplateChild("cmbConnections") is ComboBox cmbConnections)
			{
				_cmbConnections = cmbConnections;
				_cmbConnections.SelectionChanged += new SelectionChangedEventHandler(OnConnectionsSelectionChanged);
			}
			if (GetTemplateChild("lstTables") is ListBox lists)
			{
				_lstTables = lists;
				_lstTables.SelectionChanged += new SelectionChangedEventHandler(OnTablesSelectionChanged);
			}
			if (GetTemplateChild("btnOk") is Microsoft.VisualStudio.PlatformUI.DialogButton button)
			{
				button.Command = ApplicationCommands.Save;
			}
			if (GetTemplateChild("txtEntityName") is TextBox box)
			{
				txtEntityName = box;
			}
		}
		#endregion

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (_lstTables != null && _cmbConnections != null && _lstTables.SelectedItem != null)
			{
				PersistentConfiguration persistent = new PersistentConfiguration();
				ConnectionInfo info = (ConnectionInfo)_cmbConnections.SelectedItem;
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
				//persistent.ImportNameSpaces.Add("Basic.Interfaces");
				//persistent.ImportNameSpaces.Add("Basic.EntityLayer");
				//persistent.ImportNameSpaces.Add("Basic.Enums");
				DesignTableInfo tableInfo = _lstTables.SelectedItem as DesignTableInfo;
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
				item.Open(); //this.Close();
			}
		}

		//protected override void OnClosing(CancelEventArgs e)
		//{
		//	if (_BackgroundWorker.IsBusy == true) { e.Cancel = true; }
		//	base.OnClosing(e);
		//}

		private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//ConnectionConfiguration config = new ConnectionConfiguration(_CommandService);
			//if (config.ReadJson(_ProjectItems.ContainingProject))
			//{
			//	IList<ConnectionInfo> connections = ConnectionContext.GetConnections();
			//	cmbConnections.ItemsSource = connections;
			//	cmbConnections.SelectedItem = ConnectionContext.DefaultConnection;
			//	lstTables.IsEnabled = cmbConnections.IsEnabled = false;
			//	if (_BackgroundWorker.IsBusy == false) { _BackgroundWorker.RunWorkerAsync(ConnectionContext.DefaultConnection); }
			//}
		}

		private void OnConnectionsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_cmbConnections.SelectedItem == null) { return; }
			ConnectionInfo info = (ConnectionInfo)_cmbConnections.SelectedItem;
			_lstTables.IsEnabled = _cmbConnections.IsEnabled = false;
			if (_BackgroundWorker.IsBusy == false) { _BackgroundWorker.RunWorkerAsync(info); }
		}

		private void OnTablesSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (_lstTables != null && _lstTables.SelectedItem != null && txtEntityName != null)
			{
				txtEntityName.DataContext = _lstTables.SelectedItem;
				if (btnOk != null) { btnOk.IsEnabled = true; }
			}
		}
	}
}
