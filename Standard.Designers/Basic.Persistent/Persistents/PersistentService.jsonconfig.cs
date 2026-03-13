using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using MEC = Microsoft.Extensions.Configuration;

namespace Basic.Configuration
{
	/// <summary>数据持久命令服务类</summary>
	public sealed partial class PersistentService
	{
		internal bool ReadJson(EnvDTE.Project project)
		{
			try
			{
				System.IO.FileInfo fiProject = new System.IO.FileInfo(project.FullName);
				//System.IO.DirectoryInfo projDirectory = new System.IO.DirectoryInfo(fiProject.DirectoryName);
				MEC.IConfigurationBuilder projectBuilder = new MEC.ConfigurationBuilder().SetBasePath(fiProject.DirectoryName);
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
					GetProjectOfUniqueName(uniqueName, out IVsHierarchy hierarchy);
					uint itemId = (uint)VSConstants.VSITEMID.Root;
					if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object outProject) >= 0)
					{
						EnvDTE.Project solutionProject = (EnvDTE.Project)outProject;
						System.IO.FileInfo startProjectFile = new System.IO.FileInfo(solutionProject.FullName);
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
							WriteToOutput("在解决方案中获取配置文件错误，或在此目录\"{0}\"中添加以下配置文件任意一个配置文件", startProjectFile.DirectoryName);
							WriteToOutput("appsettings.json, appsettings.Development.json, database.json, database.Development.json");
						}
					}
				}
				WriteToOutput("在解决方案中获取配置文件错误，请在此目录\"{0}\"中添加以下配置文件任意一个配置文件", fiProject.DirectoryName);
				WriteToOutput("appsettings.json, appsettings.Development.json, database.json, database.Development.json");

				return false;
			}
			catch (Exception ex)
			{
				WriteToOutput(ex.Message);
				WriteToOutput(ex.StackTrace);
				WriteToOutput(ex.Source);
				return false;
			}
		}

		internal bool ReadJson(EnvDTE.ProjectItem item = null)
		{
			if (item == null) { return false; }
			return ReadJson(item.ContainingProject);
		}
	}
}
