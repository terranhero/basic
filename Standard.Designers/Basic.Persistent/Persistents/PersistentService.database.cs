using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Basic.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MDC = Microsoft.Data.ConnectionUI;
using MEC = Microsoft.Extensions.Configuration;
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
					else if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".json"))
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
						ConnectionExtension.InitializeConfiguration((string)pFullPath.Value);
					}
					else if (projectItem.Kind == kindPhysicalFile && projectItem.Name.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
					{
						EnvDTE.Property pFullPath = projectItem.Properties.Item("FullPath");
						//FileInfo jsonFileInfo = new FileInfo((string)pFullPath.Value);
						MEC.IConfigurationBuilder appBuilder = new MEC.ConfigurationBuilder();
						IConfigurationRoot appRoot = appBuilder.AddJsonFile((string)pFullPath.Value, true).Build();
						IConfigurationSection dbConnections = appRoot.GetSection("Connections");
						if (dbConnections.GetChildren().Any())
						{
							ConnectionExtension.InitializeConnections(dbConnections);
						}
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

		/// <summary>
		/// 将一个新的数据库连接写入指定的 JSON 配置文件。
		/// </summary>
		/// <param name="fullName">JSON 配置文件完整路径（例如 appsettings.json、database.json）。</param>
		/// <param name="connection">由数据连接对话框生成的数据库连接字符串。</param>
		/// <param name="dataProvider">所选数据提供程序，用于决定连接类型。</param>
		/// <remarks>
		/// 使用 Newtonsoft.Json 以 <see cref="CommentHandling.Load"/> 方式解析文件，向
		/// <c>Connections/Connections</c> 节点追加一个新连接后再整体写回；
		/// 文件中的注释、其它配置节、缩进风格与编码均会保留。
		/// </remarks>
		private void SaveJsonConfiguration(string fullName, string connection, MDC.DataProvider dataProvider)
		{
			try
			{
				if (string.IsNullOrEmpty(fullName)) { ShowMessage("配置文件路径为空，无法保存数据库连接。"); return; }
				if (!File.Exists(fullName)) { ShowMessage(string.Concat("配置文件不存在：", fullName)); return; }

				// 1) 读取 JSON 文本（保留原文件编码）并解析为 JSON 对象（保留注释）
				Encoding encoding = DetectJsonFileEncoding(fullName);
				string json = File.ReadAllText(fullName, encoding);
				JObject root;
				try { root = LoadJsonObject(json); }
				catch (Exception ex)
				{
					ShowMessage(string.Concat("当前 JSON 配置文件结构不受支持，无法自动写入数据库连接。", ex.Message));
					return;
				}

				// 2) 定位（或创建）Connections/Connections 节点
				JObject connections = GetOrCreateObject(root, "Connections");
				JObject items = GetOrCreateObject(connections, "Connections");

				// 3) 计算新连接名称（沿用 Connection_{index} 规则，并保证不重名）
				HashSet<string> connectionNames = new HashSet<string>(
					items.Properties().Select(p => p.Name), StringComparer.OrdinalIgnoreCase);
				string connectionName;
				int index = 0;
				do { connectionName = string.Concat("Connection_", index++); } while (connectionNames.Contains(connectionName));

				// 4) 构造新的连接配置
				JsonConnectionSection element = new JsonConnectionSection { Name = connectionName };
				if (dataProvider == MDC.DataProvider.OracleDataProvider) { element.ConnectionType = ConnectionType.OracleConnection; }
				else if (dataProvider == MDC.DataProvider.OdbcDataProvider) { element.ConnectionType = ConnectionType.OdbcConnection; }
				else if (dataProvider == MDC.DataProvider.OleDBDataProvider) { element.ConnectionType = ConnectionType.OleDbConnection; }
				else { element.ConnectionType = ConnectionType.SqlConnection; }

				DbConnectionBuilder builder = new DbConnectionBuilder(connection);
				foreach (string key in builder.Keys)
				{
					if (string.IsNullOrWhiteSpace(key)) { continue; }
					string value = builder[key];
					if (string.Compare(key, "Password", true) == 0) { value = ConfigurationAlgorithm.Encryption(value); }
					element[key] = value;
				}
				items[connectionName] = BuildConnectionToken(element);

				// 5) 补充 DefaultName（置于 Connections 节点首位）
				if (string.IsNullOrWhiteSpace(connections.Value<string>("DefaultName")))
				{
					connections.AddFirst(new JProperty("DefaultName", connectionName));
				}

				// 6) 写回文件（保留原文件的缩进风格、换行符与编码）
				string newLine = json.Contains("\r\n") ? "\r\n" : Environment.NewLine;
				File.WriteAllText(fullName, SerializeJson(root, newLine), encoding);
				WriteToOutput(string.Concat("已向配置文件\"", fullName, "\"添加数据库连接\"", connectionName, "\"。"));
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message);
			}
		}

		#region JSON 读写辅助方法

		/// <summary>根据 BOM 检测 JSON 文件编码；无 BOM 时按 UTF-8（无 BOM）处理。</summary>
		private static Encoding DetectJsonFileEncoding(string fullName)
		{
			try
			{
				byte[] head = new byte[3];
				using (FileStream stream = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					int count = stream.Read(head, 0, head.Length);
					if (count >= 3 && head[0] == 0xEF && head[1] == 0xBB && head[2] == 0xBF) { return new UTF8Encoding(true); }
					if (count >= 2 && head[0] == 0xFF && head[1] == 0xFE) { return Encoding.Unicode; }
					if (count >= 2 && head[0] == 0xFE && head[1] == 0xFF) { return Encoding.BigEndianUnicode; }
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("检测 JSON 文件编码失败：" + ex);
			}
			return new UTF8Encoding(false);
		}

		/// <summary>将 JSON 文本解析为可写的对象，同时保留文件中的注释。</summary>
		private static JObject LoadJsonObject(string json)
		{
			if (string.IsNullOrWhiteSpace(json)) { return new JObject(); }
			JsonLoadSettings settings = new JsonLoadSettings { CommentHandling = CommentHandling.Load };
			using (StringReader textReader = new StringReader(json))
			using (JsonTextReader jsonReader = new JsonTextReader(textReader) { DateParseHandling = DateParseHandling.None })
			{
				return JObject.Load(jsonReader, settings);
			}
		}

		/// <summary>获取指定名称的子对象；不存在时创建后返回。</summary>
		private static JObject GetOrCreateObject(JObject parent, string name)
		{
			if (parent[name] is JObject existing) { return existing; }
			JObject created = new JObject();
			parent[name] = created;
			return created;
		}

		/// <summary>将连接配置转换为 JSON 对象（ConnectionType 在最前，Version 作为数字输出）。</summary>
		private static JObject BuildConnectionToken(JsonConnectionSection element)
		{
			JObject token = new JObject { ["ConnectionType"] = element.ConnectionType.ToString() };
			foreach (KeyValuePair<string, string> pair in element)
			{
				if (string.IsNullOrWhiteSpace(pair.Key)) { continue; }
				if (string.Equals(pair.Key, "ConnectionType", StringComparison.OrdinalIgnoreCase)) { continue; }
				{
					token[pair.Key] = pair.Value;
				}
			}
			return token;
		}

		/// <summary>以制表符缩进序列化 JSON 对象，并统一换行符为文件原有风格（结尾保留一个换行符）。</summary>
		private static string SerializeJson(JObject root, string newLine)
		{
			StringBuilder buffer = new StringBuilder();
			using (StringWriter textWriter = new StringWriter(buffer))
			{
				using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter))
				{
					jsonWriter.Formatting = Formatting.Indented;
					jsonWriter.Indentation = 1;
					jsonWriter.IndentChar = '\t';
					root.WriteTo(jsonWriter);
				}
			}
			// JsonTextWriter 的换行符取决于运行环境，这里统一为文件原有风格
			string normalized = buffer.ToString().Replace("\r\n", "\n").Replace("\r", "\n");
			if (newLine == "\r\n") { normalized = normalized.Replace("\n", "\r\n"); }
			return string.Concat(normalized.TrimEnd('\r', '\n'), newLine);
		}

		#endregion

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
