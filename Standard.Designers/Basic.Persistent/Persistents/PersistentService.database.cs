using System;
using Basic.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Shell;
using MDC = Microsoft.Data.ConnectionUI;
using SC = System.Configuration;

namespace Basic.Configuration
{
    /// <summary>数据持久命令服务类</summary>
    public sealed partial class PersistentService
    {
        #region 修改项目配置文件信息(添加/更新数据库连接)
        private void OnShowConfiguration(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            OleMenuCommand menu = sender as OleMenuCommand;
            EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
            menu.Enabled = menu.Visible = false;
            if (!(dteClass.ToolWindows.SolutionExplorer.SelectedItems is Array array) || array.Length == 0) { return; }
            foreach (EnvDTE.UIHierarchyItem item in array)
            {
                if (item.Object is EnvDTE.ProjectItem)
                {
                    EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
                    if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".config"))
                    {
                        menu.Enabled = menu.Visible = true; return;
                    }
                    else if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".json"))
                    {
                        menu.Enabled = menu.Visible = true; return;
                    }
                }
            }
        }

        private void OnCanResetConnection(object sender, EventArgs e)
        {
            OleMenuCommand menu = sender as OleMenuCommand;
            EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
            Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
            menu.Enabled = menu.Visible = false;
            if (array == null || array.Length == 0) { return; }
            foreach (EnvDTE.UIHierarchyItem item in array)
            {
                if (item.Object is EnvDTE.ProjectItem pItem)
                {
                    if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".config"))
                    {
                        menu.Enabled = menu.Visible = true; return;
                    }
                }
            }
        }

        private void OnResetConnection(object sender, EventArgs e)
        {
            EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
            Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
            if (array == null || array.Length == 0) { return; }
            string kindPhysicalFile = EnvDTE.Constants.vsProjectItemKindPhysicalFile;
            foreach (EnvDTE.UIHierarchyItem uihItem in array)
            {
                if (uihItem.Object is EnvDTE.ProjectItem projectItem)
                {
                    if (projectItem.Kind == kindPhysicalFile && projectItem.Name.EndsWith(".config", StringComparison.CurrentCultureIgnoreCase))
                    {
                        EnvDTE.Property pFullPath = projectItem.Properties.Item("FullPath");
                        ConnectionContext.InitializeConfiguration((string)pFullPath.Value);
                    }
                }
            }
        }

        private void OnCanAddConnection(object sender, EventArgs e)
        {
            OleMenuCommand menu = sender as OleMenuCommand;
            EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
            Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
            menu.Enabled = menu.Visible = false;
            if (array == null || array.Length == 0) { return; }
            foreach (EnvDTE.UIHierarchyItem item in array)
            {
                if (item.Object is EnvDTE.ProjectItem)
                {
                    EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
                    if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".config"))
                    {
                        menu.Enabled = menu.Visible = true; return;
                    }
                    else if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".json"))
                    {
                        menu.Enabled = menu.Visible = true; return;
                    }
                }
            }
        }

        private void OnAddConnection(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
            Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
            if (array == null || array.Length == 0) { return; }
            string kindPhysicalFile = EnvDTE.Constants.vsProjectItemKindPhysicalFile;
            foreach (EnvDTE.UIHierarchyItem uihItem in array)
            {
                if (uihItem.Object is EnvDTE.ProjectItem)
                {
                    EnvDTE.ProjectItem projectItem = uihItem.Object as EnvDTE.ProjectItem;
                    if (projectItem.Kind == kindPhysicalFile && projectItem.Name.EndsWith(".config", StringComparison.CurrentCultureIgnoreCase))
                    {
                        MDC.DataConnectionDialog dlgDataConnection = new MDC.DataConnectionDialog();
                        dlgDataConnection.DataSources.Add(MDC.DataSource.AccessDataSource); // Access 
                        dlgDataConnection.DataSources.Add(MDC.DataSource.OracleDataSource); // Oracle 
                        dlgDataConnection.DataSources.Add(MDC.DataSource.SqlDataSource); // Sql Server
                        dlgDataConnection.DataSources.Add(MDC.DataSource.SqlFileDataSource); // Sql File Server

                        dlgDataConnection.SelectedDataSource = MDC.DataSource.SqlDataSource;   // 初始化
                        dlgDataConnection.SelectedDataProvider = MDC.DataProvider.SqlDataProvider;
                        if (MDC.DataConnectionDialog.Show(dlgDataConnection) == System.Windows.Forms.DialogResult.OK)
                        {
                            EnvDTE.Property pFullPath = projectItem.Properties.Item("FullPath");
                            //Type type = dlgDataConnection.SelectedDataProvider.TargetConnectionType;
                            //string name = dlgDataConnection.SelectedDataSource.Name;
                            string connection = dlgDataConnection.ConnectionString;
                            SaveConfiguration((string)pFullPath.Value, connection, dlgDataConnection.SelectedDataProvider);
                        }
                    }
                    else if (projectItem.Kind == kindPhysicalFile && projectItem.Name.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
                    {
                        MDC.DataConnectionDialog dlgDataConnection = new MDC.DataConnectionDialog();
                        dlgDataConnection.DataSources.Add(MDC.DataSource.AccessDataSource); // Access 
                        dlgDataConnection.DataSources.Add(MDC.DataSource.OracleDataSource); // Oracle 
                        dlgDataConnection.DataSources.Add(MDC.DataSource.SqlDataSource); // Sql Server
                        dlgDataConnection.DataSources.Add(MDC.DataSource.SqlFileDataSource); // Sql File Server

                        dlgDataConnection.SelectedDataSource = MDC.DataSource.SqlDataSource; // 初始化
                        dlgDataConnection.SelectedDataProvider = MDC.DataProvider.SqlDataProvider;
                        if (MDC.DataConnectionDialog.Show(dlgDataConnection) == System.Windows.Forms.DialogResult.OK)
                        {
                            EnvDTE.Property pFullPath = projectItem.Properties.Item("FullPath");
                            //Type type = dlgDataConnection.SelectedDataProvider.TargetConnectionType;
                            //string name = dlgDataConnection.SelectedDataSource.Name;
                            string connection = dlgDataConnection.ConnectionString;
                            SaveJsonConfiguration((string)pFullPath.Value, connection, dlgDataConnection.SelectedDataProvider);
                        }
                    }
                }
            }
        }

        private void SaveJsonConfiguration(string fullName, string connection, MDC.DataProvider dataProvider)
        {
            IConfigurationBuilder jsonBuilder = new ConfigurationBuilder().AddJsonFile(fullName);
            IConfigurationSection section = jsonBuilder.Build().GetRequiredSection("Connections");
            JsonConnectionsSection sectionConnections = new JsonConnectionsSection();
            section.Bind(sectionConnections);
            ConnectionContext.InitializeConnections(section);

            // JsonConfigurationWriter.WriteConfiguration(root, connection, dataProvider);
            JsonConnectionSection element = new JsonConnectionSection
            {
                Name = string.Concat("Connection_", sectionConnections.Connections.Count)
            };
            if (string.IsNullOrEmpty(sectionConnections.DefaultName))
                sectionConnections.DefaultName = element.Name;
            if (dataProvider == MDC.DataProvider.OracleDataProvider)
                element.ConnectionType = ConnectionType.OracleConnection;
            else if (dataProvider == MDC.DataProvider.OdbcDataProvider)
                element.ConnectionType = ConnectionType.OdbcConnection;
            else if (dataProvider == MDC.DataProvider.OleDBDataProvider)
                element.ConnectionType = ConnectionType.OleDbConnection;
            else { element.ConnectionType = ConnectionType.SqlConnection; }

            //DbConnectionBuilder builder = new DbConnectionBuilder(connection);
            //foreach (string key in builder.Keys)
            //{
            //    ConnectionItem item = new ConnectionItem() { Name = key, Value = builder[key] };
            //    if (string.Compare(item.Name, "Password", true) == 0) { item.Value = ConfigurationAlgorithm.Encryption(item.Value); }
            //    element.Add(item);
            //}

            //sectionConnections.Connections.Add(element);
            //configuration.Save(SC.ConfigurationSaveMode.Modified);
        }

        private void SaveConfiguration(string fullName, string connection, MDC.DataProvider dataProvider)
        {
            SC.Configuration configuration = ReadConfigurationFile(fullName);
            SC.ConfigurationSectionGroup sectionGroup = GetSectionGroup(configuration);
            ConnectionsSection sectionConnections = GetConnectionsSection(sectionGroup);
            ConnectionElement element = new ConnectionElement
            {
                Name = string.Concat("Connection_", sectionConnections.Connections.Count)
            };
            if (string.IsNullOrEmpty(sectionConnections.DefaultName))
                sectionConnections.DefaultName = element.Name;
            if (dataProvider == MDC.DataProvider.OracleDataProvider)
                element.ConnectionType = ConnectionType.OracleConnection;
            else if (dataProvider == MDC.DataProvider.OdbcDataProvider)
                element.ConnectionType = ConnectionType.OdbcConnection;
            else if (dataProvider == MDC.DataProvider.OleDBDataProvider)
                element.ConnectionType = ConnectionType.OleDbConnection;
            else { element.ConnectionType = ConnectionType.SqlConnection; }

            DbConnectionBuilder builder = new DbConnectionBuilder(connection);
            foreach (string key in builder.Keys)
            {
                ConnectionItem item = new ConnectionItem() { Name = key, Value = builder[key] };
                if (string.Compare(item.Name, "Password", true) == 0) { item.Value = ConfigurationAlgorithm.Encryption(item.Value); }
                element.Values.Add(item);
            }

            sectionConnections.Connections.Add(element);
            configuration.Save(SC.ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// 读取配置文件信息。
        /// </summary>
        /// <param name="fileConfiguration"></param>
        private SC.Configuration ReadConfigurationFile(string fileConfiguration)
        {
            SC.ConfigurationFileMap fileMap = new SC.ConfigurationFileMap(fileConfiguration);
            return SC.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
        }

        /// <summary>从配置文件中读取 ConnectionsSection 节</summary>
        /// <param name="configuration">项目配置文件</param>
        /// <returns>返回 ConnectionsSection 配置节</returns>
        private ConfigurationGroup GetSectionGroup(SC.Configuration configuration)
        {
            SC.ConfigurationSectionGroup group = configuration.GetSectionGroup(ConfigurationGroup.ElementName);
            if (group is ConfigurationGroup groupSection)
            {
                return groupSection;
            }
            groupSection = new ConfigurationGroup();
            configuration.SectionGroups.Add(ConfigurationGroup.ElementName, groupSection);
            return groupSection;
        }

        /// <summary>
        /// 读取配置文件信息。
        /// </summary>
        /// <param name="groupConfiguration"></param>
        private ConnectionsSection GetConnectionsSection(SC.ConfigurationSectionGroup group)
        {
            SC.ConfigurationSection section = group.Sections[ConnectionsSection.ElementName];
            ConnectionsSection connections = section as ConnectionsSection;
            if (section == null)
            {
                connections = new ConnectionsSection();
                group.Sections.Add(ConnectionsSection.ElementName, connections);
            }
            return connections;
        }
        #endregion
    }
}
