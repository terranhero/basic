using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Basic.Collections;
using Basic.Configuration;
using Basic.Database;
using Basic.DataContexts;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.Windows
{
	/// <summary>PersistentForm.xaml 的交互逻辑</summary>
	public class CreatePersistent : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		static CreatePersistent()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CreatePersistent),
				new FrameworkPropertyMetadata(typeof(CreatePersistent)));
		}

		private readonly ObservableCollection<DesignTableInfo> tables = new ObservableCollection<DesignTableInfo>();
		private readonly Pagination<ConnectionInfo> connections = new Pagination<ConnectionInfo>();
		private readonly ConnectionInfo defaultConnection;
		//private readonly EnvDTE.ProjectItems _ProjectItems;
		private readonly BackgroundWorker _BackgroundWorker = new BackgroundWorker();
		//private readonly string _DefaultNamespace, _FullPath;
		public CreatePersistent(IList<ConnectionInfo> conns, ConnectionInfo dc)
		{
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSaveExecuted));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnCloseExecuted));
			connections.AddRange(conns);
			defaultConnection = dc;
			_BackgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
			_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			foreach (DesignTableInfo conn in (DesignTableInfo[])e.Result)
			{
				tables.Add(conn);
			}
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			ConnectionInfo info = (ConnectionInfo)e.Argument;
			using (IDataContext context = DataContextFactory.CreateDbAccess(info))
			{
				e.Result = context.GetTables(ObjectTypeEnum.UserTable);
			}
		}

		#region 属性 SelectedTable 定义
		public static readonly DependencyProperty SelectedTableProperty = DependencyProperty.Register("SelectedTable",
			typeof(DesignTableInfo), typeof(CreatePersistent), new PropertyMetadata(null));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public DesignTableInfo SelectedTable
		{
			get { return (DesignTableInfo)base.GetValue(SelectedTableProperty); }
			set { base.SetValue(SelectedTableProperty, value); }
		}
		#endregion


		#region 属性 ConnectionInfo 定义
		public static readonly DependencyProperty ConnectionProperty = DependencyProperty.Register("Connection",
			typeof(ConnectionInfo), typeof(CreatePersistent), new PropertyMetadata(null));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public ConnectionInfo Connection
		{
			get { return (ConnectionInfo)base.GetValue(ConnectionProperty); }
			set { base.SetValue(ConnectionProperty, value); }
		}
		#endregion

		#region 属性 CanSave 定义
		public static readonly DependencyProperty CanSaveProperty = DependencyProperty.Register("CanSave",
			typeof(bool), typeof(CreatePersistent), new PropertyMetadata(false));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool CanSave
		{
			get { return (bool)base.GetValue(CanSaveProperty); }
			set { base.SetValue(CanSaveProperty, value); }
		}
		#endregion

		#region 重载 OnApplyTemplate 方法，绑定 PART_TREEVIEW 事件
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			// 查找模板中的控件
			if (GetTemplateChild("cmbConnections") is ComboBox cmbConnections)
			{
				cmbConnections.ItemsSource = connections;
				cmbConnections.SelectionChanged += new SelectionChangedEventHandler(OnConnectionsSelectionChanged);
				cmbConnections.SelectedItem = defaultConnection;
				// if (_BackgroundWorker.IsBusy == false) { _BackgroundWorker.RunWorkerAsync(info); }
			}
			if (GetTemplateChild("lstTables") is ListBox lstTables)
			{
				lstTables.ItemsSource = tables;
				lstTables.SelectionChanged += new SelectionChangedEventHandler(OnTablesSelectionChanged);
			}
		}
		#endregion

		private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			//if (_lstTables != null && _cmbConnections != null && _lstTables.SelectedItem != null)
			//{
			//    PersistentConfiguration persistent = new PersistentConfiguration();
			//    ConnectionInfo info = (ConnectionInfo)_cmbConnections.SelectedItem;
			//    if (info.ConnectionType == ConnectionType.SqlConnection)
			//        persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.SQLSERVER };
			//    else if (info.ConnectionType == ConnectionType.OracleConnection)
			//        persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.ORACLE };
			//    else if (info.ConnectionType == ConnectionType.MySqlConnection)
			//        persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.MYSQL };
			//    else if (info.ConnectionType == ConnectionType.Db2Connection)
			//        persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.DB2 };
			//    else if (info.ConnectionType == ConnectionType.NpgSqlConnection)
			//        persistent.SupportDatabases = new ConnectionTypeEnum[] { ConnectionTypeEnum.NPGSQL };

			//    persistent.Namespace = _DefaultNamespace;
			//    persistent.EntityNamespace = _DefaultNamespace;
			//    persistent.ImportNameSpaces.Add(typeof(IPagination).Namespace);
			//    persistent.ImportNameSpaces.Add(typeof(AbstractEntity).Namespace);
			//    persistent.ImportNameSpaces.Add(typeof(DbTypeEnum).Namespace);
			//    //persistent.ImportNameSpaces.Add("Basic.Interfaces");
			//    //persistent.ImportNameSpaces.Add("Basic.EntityLayer");
			//    //persistent.ImportNameSpaces.Add("Basic.Enums");
			//    DesignTableInfo tableInfo = _lstTables.SelectedItem as DesignTableInfo;
			//    using (IDataContext context = DataContextFactory.CreateDbAccess(info))
			//    {
			//        persistent.TableInfo.CopyFrom(tableInfo);
			//        context.GetColumns(persistent.TableInfo);
			//        persistent.GroupName = persistent.TableInfo.EntityName;
			//        persistent.PublicGroupName = persistent.TableInfo.EntityName;
			//    }
			//    string filePath = string.Concat(_FullPath, persistent.TableInfo.EntityName, ".dpdl");
			//    using (XmlWriter writer = XmlWriter.Create(filePath))
			//    {
			//        persistent.WriteXml(writer);
			//    }
			//EnvDTE.ProjectItem item = _ProjectItems.AddFromFile(filePath);
			//EnvDTE.Property property = item.Properties.Item("CustomTool");
			//property.Value = "AccessGenerator";
			//item.Open(); 
			this.DialogResult = true;
			this.Close();
			//}
		}

		public PersistentConfiguration GetPersistentContent(string defaultNamespance)
		{
			PersistentConfiguration persistent = new PersistentConfiguration();
			ConnectionInfo info = Connection;
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

			persistent.Namespace = defaultNamespance;
			persistent.EntityNamespace = defaultNamespance;
			persistent.ImportNameSpaces.Add(typeof(IPagination).Namespace);
			persistent.ImportNameSpaces.Add(typeof(AbstractEntity).Namespace);
			persistent.ImportNameSpaces.Add(typeof(DbTypeEnum).Namespace);
			//persistent.ImportNameSpaces.Add("Basic.Interfaces");
			//persistent.ImportNameSpaces.Add("Basic.EntityLayer");
			//persistent.ImportNameSpaces.Add("Basic.Enums");
			DesignTableInfo tableInfo = SelectedTable;
			using (IDataContext context = DataContextFactory.CreateDbAccess(info))
			{
				persistent.TableInfo.CopyFrom(tableInfo);
				context.GetColumns(persistent.TableInfo);
				persistent.GroupName = persistent.TableInfo.EntityName;
				persistent.PublicGroupName = persistent.TableInfo.EntityName;
			}
			return persistent;
		}

		private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_BackgroundWorker.IsBusy == true) { e.Cancel = true; }
			base.OnClosing(e);
		}

		private void OnConnectionsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cmbConnections = sender as ComboBox;
			if (cmbConnections.SelectedItem == null) { return; }
			ConnectionInfo info = (ConnectionInfo)cmbConnections.SelectedItem; Connection = info;
			if (_BackgroundWorker.IsBusy == false) { _BackgroundWorker.RunWorkerAsync(info); }
		}

		private void OnTablesSelectionChanged(object sender, RoutedEventArgs e)
		{
			ListBox lstTables = sender as ListBox;
			if (lstTables.SelectedItem == null) { return; }
			SelectedTable = (DesignTableInfo)lstTables.SelectedItem;
			CanSave = true;
		}
	}
}
