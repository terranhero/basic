using System;
using System.IO;
using Basic.Configuration;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Basic.DataContexts
{
	/// <summary>
	/// 
	/// </summary>
	public static class DbConnectionContext1
	{
		private static ServiceProvider provider;
		private static EnvDTE.DTE dteClass;
		private static IVsUIShell uiShell;
		private static ErrorListProvider errorListProvider;
		private static IVsSolution vsSolution;
		static DbConnectionContext1()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			provider = ServiceProvider.GlobalProvider;
			dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
			Assumes.Present(dteClass);
			uiShell = (IVsUIShell)provider.GetService(typeof(SVsUIShell));
			vsSolution = (IVsSolution)provider.GetService(typeof(SVsSolution));
			Assumes.Present(vsSolution);
		}
		internal static void Initilize(AsyncPackage package)
		{
			errorListProvider = new ErrorListProvider(package);
		}

		internal static bool ReadConfig(EnvDTE.Project project)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			try
			{
				FileInfo pf = new FileInfo(project.FullName);
				if (pf.Directory.Exists)
				{
					FileInfo[] files = pf.Directory.GetFiles("*.config");
					foreach (FileInfo file in files)
					{
						string fileName = file.Name.ToLower();
						if (fileName == "web.config" || fileName == "app.config")
						{
							ConnectionContext.InitializeConfiguration(file.FullName);
							return true;
						}
					}
				}
				EnvDTE80.SolutionBuild2 solutionBuild2 = dteClass.Solution.SolutionBuild as EnvDTE80.SolutionBuild2;
				foreach (string uniqueName in solutionBuild2.StartupProjects as Array)
				{
					vsSolution.GetProjectOfUniqueName(uniqueName, out IVsHierarchy hierarchy);
					uint itemId = (uint)VSConstants.VSITEMID.Root;
					if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object outProject) >= 0)
					{
						EnvDTE.Project solutionProject = (EnvDTE.Project)outProject;
						FileInfo startProjectFile = new FileInfo(solutionProject.FullName);
						if (startProjectFile.Directory.Exists)
						{
							FileInfo[] files = startProjectFile.Directory.GetFiles("*.config");
							foreach (FileInfo file in files)
							{
								string fileName = file.Name.ToLower();
								if (fileName == "web.config" || fileName == "app.config")
								{
									ConnectionContext.InitializeConfiguration(file.FullName);
									return true;
								}
							}
						}
					}
				}
				WriteError(null, "解决方案中无法获取配置文件", null);
				return false;
			}
			catch (Exception ex)
			{
				WriteError(null, ex);
				return false;
			}
		}

		internal static bool ReadConfig(EnvDTE.ProjectItem item = null)
		{
			try
			{
				EnvDTE.Project project = item.ContainingProject;
				FileInfo pf = new FileInfo(project.FullName);
				if (pf.Directory.Exists)
				{
					FileInfo[] files = pf.Directory.GetFiles("*.config");
					foreach (FileInfo file in files)
					{
						string fileName = file.Name.ToLower();
						if (fileName == "web.config" || fileName == "app.config")
						{
							ConnectionContext.InitializeConfiguration(file.FullName);
							return true;
						}
					}
				}
				EnvDTE80.SolutionBuild2 solutionBuild2 = dteClass.Solution.SolutionBuild as EnvDTE80.SolutionBuild2;
				foreach (string uniqueName in solutionBuild2.StartupProjects as Array)
				{
					IVsHierarchy hierarchy;
					vsSolution.GetProjectOfUniqueName(uniqueName, out hierarchy);
					object outProject = null;
					uint itemId = (uint)VSConstants.VSITEMID.Root;
					if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out outProject) >= 0)
					{
						EnvDTE.Project solutionProject = (EnvDTE.Project)outProject;
						FileInfo startProjectFile = new FileInfo(solutionProject.FullName);
						if (startProjectFile.Directory.Exists)
						{
							FileInfo[] files = startProjectFile.Directory.GetFiles("*.config");
							foreach (FileInfo file in files)
							{
								string fileName = file.Name.ToLower();
								if (fileName == "web.config" || fileName == "app.config")
								{
									ConnectionContext.InitializeConfiguration(file.FullName);
									return true;
								}
							}
						}
					}
				}
				WriteError(item, "解决方案中无法获取配置文件", null);
				return false;
			}
			catch (Exception ex)
			{
				WriteError(item, ex);
				return false;
			}
		}

		internal static void SetWaitCursor() { if (uiShell != null) { uiShell.SetWaitCursor(); } }
		internal static void WriteError(EnvDTE.ProjectItem item, string msg, string helpLink, int column = 0, int line = 0)
		{
			if (item == null)
				item = dteClass.ActiveDocument.ProjectItem;
			IVsHierarchy hierarchy;
			vsSolution.GetProjectOfUniqueName(item.ContainingProject.UniqueName, out hierarchy);
			ErrorTask task = new ErrorTask();
			task.Category = TaskCategory.User;
			task.ErrorCategory = TaskErrorCategory.Error;
			task.Text = msg;
			task.HelpKeyword = helpLink;
			task.Document = item.Name;
			task.Column = column;
			task.Line = line;
			task.CanDelete = true;
			task.HierarchyItem = hierarchy;
			errorListProvider.Tasks.Add(task);
			errorListProvider.Show();
		}

		internal static void WriteError(EnvDTE.ProjectItem item, Exception ex)
		{
			if (item == null)
				item = dteClass.ActiveDocument.ProjectItem;
			IVsHierarchy hierarchy;
			vsSolution.GetProjectOfUniqueName(item.ContainingProject.UniqueName, out hierarchy);
			ErrorTask task = new ErrorTask
			{
				Category = TaskCategory.User,
				ErrorCategory = TaskErrorCategory.Error,
				Text = ex.Message,
				HelpKeyword = ex.HelpLink,
				Document = item.Name,
				Column = 0,
				Line = 0,
				CanDelete = true,
				HierarchyItem = hierarchy
			};
			errorListProvider.Tasks.Add(task);
			errorListProvider.Show();
		}

		internal static void WriteError(EnvDTE.ProjectItem item, System.Xml.XmlException ex)
		{
			if (item == null)
				item = dteClass.ActiveDocument.ProjectItem;
			IVsHierarchy hierarchy;
			vsSolution.GetProjectOfUniqueName(item.ContainingProject.UniqueName, out hierarchy);
			ErrorTask task = new ErrorTask
			{
				Category = TaskCategory.User,
				ErrorCategory = TaskErrorCategory.Error,
				Text = ex.Message,
				HelpKeyword = ex.HelpLink,
				Document = item.Name,
				Column = ex.LinePosition,
				Line = ex.LineNumber,
				CanDelete = true,
				HierarchyItem = hierarchy
			};
			errorListProvider.Tasks.Add(task);
			errorListProvider.Show();
		}
	}
}
