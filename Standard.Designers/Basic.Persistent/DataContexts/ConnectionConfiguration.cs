using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Basic.Configuration;
using Basic.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using MEC = Microsoft.Extensions.Configuration;
using SC = System.Configuration;

namespace Basic.DataContexts
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class ConnectionConfiguration
	{
		private readonly PersistentService _Service;
		internal ConnectionConfiguration(PersistentService package)
		{
			_Service = package;
		}
		internal bool ReadJson(EnvDTE.Project project)
		{
			try
			{
				FileInfo fiProject = new FileInfo(project.FullName);
				DirectoryInfo projDirectory = new DirectoryInfo(fiProject.DirectoryName);
				MEC.IConfigurationBuilder projectBuilder = new MEC.ConfigurationBuilder().SetBasePath(projDirectory.FullName);
				projectBuilder.AddJsonFile("appsettings.json", true).AddJsonFile("appsettings.Development.json", true);
				projectBuilder.AddJsonFile("database.json", true).AddJsonFile("database.Development.json", true);
				IConfigurationRoot pRoot = projectBuilder.Build();
				IConfigurationSection dbConnections = pRoot.GetSection("Connections");
				if (dbConnections.GetChildren().Any())
				{
					ConnectionExtension.InitializeConnections(dbConnections);
					return true;
				}

				EnvDTE.DTE dteClass = project.DTE;
				EnvDTE80.SolutionBuild2 solutionBuild2 = dteClass.Solution.SolutionBuild as EnvDTE80.SolutionBuild2;
				foreach (string uniqueName in solutionBuild2.StartupProjects as Array)
				{
					_Service.GetProjectOfUniqueName(uniqueName, out IVsHierarchy hierarchy);
					uint itemId = (uint)VSConstants.VSITEMID.Root;
					if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object outProject) >= 0)
					{
						EnvDTE.Project solutionProject = (EnvDTE.Project)outProject;
						FileInfo startProjectFile = new FileInfo(solutionProject.FullName);
						MEC.IConfigurationBuilder appBuilder = new MEC.ConfigurationBuilder().SetBasePath(startProjectFile.DirectoryName);
						appBuilder.AddJsonFile("appsettings.json", true).AddJsonFile("appsettings.Development.json", true);
						appBuilder.AddJsonFile("database.json", true).AddJsonFile("database.Development.json", true);
						IConfigurationRoot appRoot = appBuilder.Build();
						dbConnections = appRoot.GetSection("Connections");
						if (dbConnections.GetChildren().Any())
						{
							ConnectionExtension.InitializeConnections(dbConnections);
							return true;
						}
						else
						{
							_Service.WriteToOutput("在解决方案中获取配置文件错误，或在此目录\"{0}\"中添加以下配置文件任意一个配置文件", startProjectFile.DirectoryName);
							_Service.WriteToOutput("appsettings.json, appsettings.Development.json, database.json, database.Development.json");
						}
					}
				}
				_Service.WriteToOutput("在解决方案中获取配置文件错误，请在此目录\"{0}\"中添加以下配置文件任意一个配置文件", fiProject.DirectoryName);
				_Service.WriteToOutput("appsettings.json, appsettings.Development.json, database.json, database.Development.json");
			
				return false;
			}
			catch (Exception ex)
			{
				_Service.WriteToOutput(ex.Message);
				_Service.WriteToOutput(ex.StackTrace);
				_Service.WriteToOutput(ex.Source);
				return false;
			}
		}

		internal bool ReadJson(EnvDTE.ProjectItem item = null)
		{
			if (item == null) { return false; }
			return ReadJson(item.ContainingProject);
		}

		//internal bool ReadConfig(EnvDTE.Project project)
		//{
		//	try
		//	{
		//		FileInfo pf = new FileInfo(project.FullName);
		//		if (pf.Directory.Exists)
		//		{
		//			FileInfo[] files = pf.Directory.GetFiles("*.config");
		//			foreach (FileInfo file in files)
		//			{
		//				string fileName = file.Name.ToLower();
		//				if (fileName == "web.config" || fileName == "app.config")
		//				{
		//					InitializeConfiguration(file.FullName);
		//				}
		//				else if (fileName == "database.config" || fileName == "database.development.config")
		//				{
		//					InitializeConfiguration(file.FullName);
		//				}
		//			}
		//		}
		//		EnvDTE.DTE dteClass = project.DTE;
		//		EnvDTE80.SolutionBuild2 solutionBuild2 = dteClass.Solution.SolutionBuild as EnvDTE80.SolutionBuild2;
		//		foreach (string uniqueName in solutionBuild2.StartupProjects as Array)
		//		{
		//			_Service.GetProjectOfUniqueName(uniqueName, out IVsHierarchy hierarchy);
		//			uint itemId = (uint)VSConstants.VSITEMID.Root;
		//			if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object outProject) >= 0)
		//			{
		//				EnvDTE.Project solutionProject = (EnvDTE.Project)outProject;
		//				FileInfo startProjectFile = new FileInfo(solutionProject.FullName);
		//				if (startProjectFile.Directory.Exists)
		//				{
		//					FileInfo[] files = startProjectFile.Directory.GetFiles("*.config");
		//					foreach (FileInfo file in files)
		//					{
		//						string fileName = file.Name.ToLower();
		//						if (fileName == "web.config" || fileName == "app.config")
		//						{
		//							return InitializeConfiguration(file.FullName);
		//						}
		//						else if (fileName == "database.config" || fileName == "database.development.config")
		//						{
		//							return InitializeConfiguration(file.FullName);
		//						}
		//					}
		//				}
		//			}
		//		}
		//		_Service.WriteToOutput("解决方案中无法获取配置文件");
		//		return false;
		//	}
		//	catch (Exception ex)
		//	{
		//		_Service.WriteToOutput(ex.Message);
		//		_Service.WriteToOutput(ex.StackTrace);
		//		_Service.WriteToOutput(ex.Source);
		//		return false;
		//	}
		//}

		/// <summary>从指定配置文件中初始化数据库连接信息</summary>
		/// <param name="fullName">配置文件路径</param>
		internal bool InitializeConfiguration(string fullName)
		{
			string sectionName = ConnectionsSection.ElementName;

			SC.ConfigurationFileMap fileMap = new SC.ConfigurationFileMap(fullName);
			SC.Configuration config = SC.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
			SC.ConfigurationSection section1 = config.GetSection("connections");
			if (section1 != null && section1 is DefaultSection)
			{
				_Service.WriteToOutput("系统读取数据库连接异常，请在 \"{0}\" 文件中添加 配置项", fullName);
				_Service.WriteToOutput("<section name=\"connections\" type=\"System.Configuration.ConnectionStringsSection, System.Configuration\"/>");
				return false;
			}
			else if (section1 != null && section1 is ConnectionStringsSection connectionStrings)
			{
				ConnectionExtension.InitializeConnection(connectionStrings); return true;
			}
			SC.ConfigurationSection section = config.GetSection(string.Concat(ConfigurationGroup.ElementName, "/", sectionName));
			if (section == null) { throw new ConfigurationFileException("Access_Configuration_GroupNotFound", fullName, sectionName); }
			if (section is ConnectionsSection configurationSection)
			{
				ConnectionExtension.InitializeConnection(configurationSection); return true;
			}

			_Service.WriteToOutput("系统读取数据库连接异常，请在 \"{0}\" 文件中添加 配置项", fullName);
			_Service.WriteToOutput("<section name=\"connections\" type=\"System.Configuration.ConnectionStringsSection, System.Configuration\"/>");
			_Service.WriteToOutput("<connections>");
			_Service.WriteToOutput("\t<add name=\"Name\" connectionString=\"Data Source = (local);Initial Catalog=db; integrated security=SSPI;\" providerName=\"System.Data.SqlClient\"/>");
			_Service.WriteToOutput("</connections>");
			return false;
		}

		//internal bool ReadConfig(EnvDTE.ProjectItem item = null)
		//{
		//	try
		//	{
		//		EnvDTE.Project project = item.ContainingProject;
		//		FileInfo pf = new FileInfo(project.FullName);
		//		if (pf.Directory.Exists)
		//		{
		//			FileInfo[] files = pf.Directory.GetFiles("*.config");
		//			foreach (FileInfo file in files)
		//			{
		//				string fileName = file.Name.ToLower();
		//				if (fileName == "web.config" || fileName == "app.config")
		//				{
		//					return InitializeConfiguration(file.FullName);
		//				}
		//				else if (fileName == "database.config" || fileName == "database.development.config")
		//				{
		//					return InitializeConfiguration(file.FullName);
		//				}
		//			}
		//		}
		//		EnvDTE.DTE dteClass = project.DTE;
		//		EnvDTE80.SolutionBuild2 solutionBuild2 = dteClass.Solution.SolutionBuild as EnvDTE80.SolutionBuild2;
		//		foreach (string uniqueName in solutionBuild2.StartupProjects as Array)
		//		{
		//			_Service.GetProjectOfUniqueName(uniqueName, out IVsHierarchy hierarchy);
		//			uint itemId = (uint)VSConstants.VSITEMID.Root;
		//			if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object outProject) >= 0)
		//			{
		//				EnvDTE.Project solutionProject = (EnvDTE.Project)outProject;
		//				FileInfo startProjectFile = new FileInfo(solutionProject.FullName);
		//				if (startProjectFile.Directory.Exists)
		//				{
		//					FileInfo[] files = startProjectFile.Directory.GetFiles("*.config");
		//					foreach (FileInfo file in files)
		//					{
		//						string fileName = file.Name.ToLower();
		//						if (fileName == "web.config" || fileName == "app.config")
		//						{
		//							return InitializeConfiguration(file.FullName);
		//						}
		//						else if (fileName == "database.config" || fileName == "database.development.config")
		//						{
		//							return InitializeConfiguration(file.FullName);
		//						}
		//					}
		//				}
		//			}
		//		}
		//		_Service.WriteToOutput("解决方案中无法获取配置文件");
		//		return false;
		//	}
		//	catch (Exception ex)
		//	{
		//		_Service.WriteToOutput(ex.Message);
		//		_Service.WriteToOutput(ex.StackTrace);
		//		_Service.WriteToOutput(ex.Source);
		//		return false;
		//	}
		//}
	}
}
