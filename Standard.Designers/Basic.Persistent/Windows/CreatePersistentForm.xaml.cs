using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using MEC = Microsoft.Extensions.Configuration;

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
			FileInfo pf = new FileInfo(_ProjectItems.ContainingProject.FullName);
			if (pf.Directory.Exists)
			{
				MEC.IConfigurationBuilder configBuilder = new MEC.ConfigurationBuilder().SetBasePath(pf.Directory.FullName);
				configBuilder.AddJsonFile("appsettings.Development.json").AddJsonFile("database.Development.json");
				configBuilder.AddJsonFile("appsettings.json").AddJsonFile("database.json");
				FileInfo[] files = pf.Directory.GetFiles("*.json");
				foreach (FileInfo file in files)
				{
					if (string.Compare(file.Name, "appsettings.json", true) == 0) { configBuilder.AddJsonFile("appsettings.json"); }
					else if (string.Compare(file.Name, "appsettings.Development.json", true) == 0) { configBuilder.AddJsonFile("appsettings.Development.json"); }
					else if (string.Compare(file.Name, "database.json", true) == 0) { configBuilder.AddJsonFile("database.json"); }
					else if (string.Compare(file.Name, "database.Development.json", true) == 0) { configBuilder.AddJsonFile("database.Development.json"); }
				}
				IConfigurationRoot root = configBuilder.Build();
				IConfigurationSection dbConnections = root.GetSection("Connections");
				ConnectionContext.InitializeConnections(dbConnections);
				//return true;
				return;
			}
			EnvDTE.DTE dteClass = _ProjectItems.ContainingProject.DTE;
			EnvDTE80.SolutionBuild2 solutionBuild2 = dteClass.Solution.SolutionBuild as EnvDTE80.SolutionBuild2;
			foreach (string uniqueName in solutionBuild2.StartupProjects as Array)
			{
				_CommandService.GetProjectOfUniqueName(uniqueName, out IVsHierarchy hierarchy);
				uint itemId = (uint)VSConstants.VSITEMID.Root;
				if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object outProject) >= 0)
				{
					EnvDTE.Project solutionProject = (EnvDTE.Project)outProject;
					FileInfo startProjectFile = new FileInfo(solutionProject.FullName);
					if (startProjectFile.Directory.Exists)
					{
						MEC.IConfigurationBuilder configBuilder = new MEC.ConfigurationBuilder().SetBasePath(pf.Directory.FullName);
						configBuilder.AddJsonFile("appsettings.Development.json").AddJsonFile("database.Development.json");
						configBuilder.AddJsonFile("appsettings.json").AddJsonFile("database.json");
						FileInfo[] files = pf.Directory.GetFiles("*.json");
						foreach (FileInfo file in files)
						{
							if (string.Compare(file.Name, "appsettings.json", true) == 0) { configBuilder.AddJsonFile("appsettings.json"); }
							else if (string.Compare(file.Name, "appsettings.Development.json", true) == 0) { configBuilder.AddJsonFile("appsettings.Development.json"); }
							else if (string.Compare(file.Name, "database.json", true) == 0) { configBuilder.AddJsonFile("database.json"); }
							else if (string.Compare(file.Name, "database.Development.json", true) == 0) { configBuilder.AddJsonFile("database.Development.json"); }
						}
						IConfigurationRoot root = configBuilder.Build();
						IConfigurationSection dbConnections = root.GetSection("Connections");
						ConnectionContext.InitializeConnections(dbConnections);
						//return true;
					}
				}
			}

			//ConnectionConfiguration config = new ConnectionConfiguration(_CommandService);
			//if (config.ReadJson(_ProjectItems.ContainingProject))
			//{
			//	IList<ConnectionInfo> connections = ConnectionContext.GetConnections();
			//	cmbConnections.ItemsSource = connections;
			//	cmbConnections.SelectedItem = ConnectionContext.DefaultConnection;
			//	lstTables.IsEnabled = cmbConnections.IsEnabled = false;
			//	if (_BackgroundWorker.IsBusy == false) { _BackgroundWorker.RunWorkerAsync(ConnectionContext.DefaultConnection); }
			//}
			//else if (config.ReadConfig(_ProjectItems.ContainingProject))
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
