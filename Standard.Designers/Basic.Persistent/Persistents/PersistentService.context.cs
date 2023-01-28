using System;
using System.ComponentModel.Design;
using System.Xml;
using Basic.Converters;
using Basic.Enums;
using Basic.Localizations;
using Basic.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

//VSLangProj.prjKindVBProject for VB.NET projects.
//VSLangProj.prjKindCSharpProject for C# projects.
//VSLangProj.prjKindVSAProject for VSA projects (macros projects).
//VSLangProj2.prjKindVJSharpProject for Visual J# projects.
//VSLangProj2.prjKindSDEVBProject for VB.NET Smart Device projects (Visual Studio .NET 2002/2003 only, see below for Visual Studio 2005).
//VSLangProj2.prjKindSDECSharpProject for C# projects  (Visual Studio .NET 2002/2003 only, see below for Visual Studio 2005).
//{7D353B21-6E36-11D2-B35A-0000F81F0C06} for "Enterprise Projects" (Visual Studio .NET 2002/2003 only).
//{54435603-DBB4-11D2-8724-00A0C9A8B90C} for Setup projects.
//EnvDTE.Constants.vsProjectKindSolutionItems for the Solution Items folder of the Solution Explorer.
//EnvDTE.Constants.vsProjectKindMisc for the Miscellaneous Files folder of the Solution Explorer.

namespace Basic.Configuration
{
	/// <summary>数据持久命令服务类</summary>
	public sealed partial class PersistentService
	{
		internal static readonly string LocalizedExtension = ".localresx";
		#region 解决方案快捷菜单命令
		/// <summary>
		/// 快捷菜单Guid字符串
		/// </summary>
		public const string PersistentGuidString = "F7015F70-C95E-4E28-B76C-FBE57087E854";
		/// <summary>
		/// 表示数据持久类菜单标识符。
		/// </summary>
		public static readonly Guid PersistentGuid = new Guid(PersistentGuidString);

		/// <summary>表示数据持久类菜单ID</summary>

		/// <summary>
		/// 数据持久类快捷菜单更新静态/动态命令ID
		/// </summary>
		public static readonly CommandID ShowContextCodeID = new CommandID(PersistentGuid, 0x0601);

		/// <summary>
		/// 数据持久类快捷菜单更新静态/动态命令ID
		/// </summary>
		public static readonly CommandID ShowAccessCodeID = new CommandID(PersistentGuid, 0x0602);

		/// <summary>
		/// 表示数据持久类菜单ID
		/// </summary>
		public static readonly CommandID ConverterID = new CommandID(PersistentGuid, 0x0101);

		public static readonly CommandID MenuID = new CommandID(PersistentGuid, 0x1000);

		/// <summary>表示数据持久类菜单ID</summary>
		public static readonly CommandID ConnectionGroupID = new CommandID(PersistentGuid, 0x0500);

		/// <summary>表示数据库连接字符串ID</summary>
		public static readonly CommandID AddConnectionID = new CommandID(PersistentGuid, 0x0511);

		/// <summary>表示数据库连接字符串ID</summary>
		public static readonly CommandID ResetConnectionID = new CommandID(PersistentGuid, 0x0512);

		/// <summary>
		/// 表示数据持久类菜单ID
		/// </summary>
		public static readonly CommandID ConverterAllID = new CommandID(PersistentGuid, 0x0201);

		/// <summary>
		/// 表示Mvc项目添加经典页面类型命令ID
		/// </summary>
		public static readonly CommandID AddClassicViewID = new CommandID(PersistentGuid, 0x0401);

		/// <summary>
		/// 表示Mvc项目添加经典页面类型命令ID
		/// </summary>
		public static readonly CommandID AddMvcViewID = new CommandID(PersistentGuid, 0x0402);

		/// <summary>表示Mvc项目添加经典页面类型命令ID</summary>
		public static readonly CommandID AddControllerID = new CommandID(PersistentGuid, 0x0403);

		/// <summary>表示Mvc项目添加经典页面类型命令ID</summary>
		public static readonly CommandID AddWpfClassicFormID = new CommandID(PersistentGuid, 0x0410);

		/// <summary>表示Mvc项目添加经典页面类型命令ID</summary>
		public static readonly CommandID AddWpfFormID = new CommandID(PersistentGuid, 0x0411);

		/// <summary>添加数据持久类命令ID</summary>
		public static readonly CommandID AddPersistentID = new CommandID(PersistentGuid, 0x0412);

		/// <summary>添加本地化资源命令ID</summary>
		public static readonly CommandID AddLocalizationID = new CommandID(PersistentGuid, 0x0413);

		#endregion

		#region 解决方案快捷菜单扩展
		private string GetProjectTypeGuids(EnvDTE.Project project)
		{
			string guids = null;
			IVsSolution solution = GetVsSolution();
			solution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
			if (hierarchy is IVsAggregatableProject project2) { project2.GetAggregateProjectTypeGuids(out guids); }
			return guids != null ? guids.ToUpper() : guids;
		}

		/// <summary>
		/// 获取项目类型
		/// </summary>
		/// <param name="project">表示当前解决方案中选择的 EnvDTE.Project 对象</param>
		/// <returns></returns>
		private ProjectTypes GetProjectType(EnvDTE.Project project)
		{
			//GetProjectTypeGuids()
			//VSLangProj.prjProjectType projectType = VSLangProj.prjProjectType.prjProjectTypeLocal;
			EnvDTE.Property pProjectType = project.Properties.Item("ProjectType");
			string strProjectType = Convert.ToString(pProjectType.Value);
			if (string.IsNullOrWhiteSpace(strProjectType) == false)
			{
				VSLangProj.prjProjectType projectType = (VSLangProj.prjProjectType)pProjectType.Value;
				if (projectType == VSLangProj.prjProjectType.prjProjectTypeWeb)
				{
					return ProjectTypes.WebApplication;
				}
			}
			EnvDTE.Property pOutputType = project.Properties.Item("OutputType");
			VSLangProj.prjOutputType outpytType = (VSLangProj.prjOutputType)pOutputType.Value;
			//VS2012 or higher is a bug.
			if (outpytType == VSLangProj.prjOutputType.prjOutputTypeWinExe)
			{
				foreach (string extenderName in project.ExtenderNames as Array)
				{
					object obj = project.get_Extender(extenderName);
					if (extenderName == "WPFProjectExtender" && obj != null)    //VS2012 or higher WPF Project
					{
						return ProjectTypes.WpfApplication;
					}
				}
				string guids = GetProjectTypeGuids(project);
				if (string.IsNullOrWhiteSpace(guids) == true) { return ProjectTypes.MvcApplication; } //ASP.NET Mvc Core
				else if (guids.Contains("{E24C65DC-7377-472B-9ABA-BC803B73C61A}")) { return ProjectTypes.Website; }
				else if (guids.Contains("{603C0E0B-DB56-11DC-BE95-000D561079B0}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 1.0
				else if (guids.Contains("{F85E285D-A4E0-4152-9332-AB1D724D3325}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 2.0
				else if (guids.Contains("{E53F8FEA-EAE0-44A6-8774-FFD645390401}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 3.0
				else if (guids.Contains("{E3E379DF-F4C6-4180-9B81-6769533ABE47}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 4.0
				else if (guids.Contains("{349C5851-65DF-11DA-9384-00065B846F21}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 5.0
				return ProjectTypes.WindowsApplication;
			}
			else if (outpytType == VSLangProj.prjOutputType.prjOutputTypeLibrary)
			{
				//ShowMessage(project.Name, "VSLangProj.prjOutputType.prjOutputTypeLibrary");
				foreach (string extenderName in project.ExtenderNames as Array)
				{
					object obj = project.get_Extender(extenderName);
					if (extenderName == "WebApplication" && obj != null)    //VS2012 or higher Web Project
					{
						string guids = GetProjectTypeGuids(project);
						if (string.IsNullOrWhiteSpace(guids) == true) { return ProjectTypes.MvcApplication; } //ASP.NET Mvc Core
						else if (guids.Contains("{E24C65DC-7377-472B-9ABA-BC803B73C61A}")) { return ProjectTypes.Website; }
						else if (guids.Contains("{603C0E0B-DB56-11DC-BE95-000D561079B0}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 1.0
						else if (guids.Contains("{F85E285D-A4E0-4152-9332-AB1D724D3325}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 2.0
						else if (guids.Contains("{E53F8FEA-EAE0-44A6-8774-FFD645390401}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 3.0
						else if (guids.Contains("{E3E379DF-F4C6-4180-9B81-6769533ABE47}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 4.0
						else if (guids.Contains("{349C5851-65DF-11DA-9384-00065B846F21}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 5.0
						return ProjectTypes.WebApplication;
					}
					else if (extenderName == "SilverlightProject" && obj != null) //VS2012 or higher Silverlight Project
					{
						return ProjectTypes.SilverlightLibrary;
					}
				}
				return ProjectTypes.ClassLibrary;
			}
			else if (outpytType == VSLangProj.prjOutputType.prjOutputTypeExe)   //Console Application
			{
				string guids = GetProjectTypeGuids(project);
				if (string.IsNullOrWhiteSpace(guids) == true) { return ProjectTypes.MvcApplication; } //ASP.NET Mvc Core
				else if (guids.Contains("{E24C65DC-7377-472B-9ABA-BC803B73C61A}")) { return ProjectTypes.Website; }
				else if (guids.Contains("{603C0E0B-DB56-11DC-BE95-000D561079B0}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 1.0
				else if (guids.Contains("{F85E285D-A4E0-4152-9332-AB1D724D3325}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 2.0
				else if (guids.Contains("{E53F8FEA-EAE0-44A6-8774-FFD645390401}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 3.0
				else if (guids.Contains("{E3E379DF-F4C6-4180-9B81-6769533ABE47}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 4.0
				else if (guids.Contains("{349C5851-65DF-11DA-9384-00065B846F21}")) { return ProjectTypes.MvcApplication; } //ASP.NET MVC 5.0
				return ProjectTypes.ConsoleApplication;
			}
			return ProjectTypes.ClassLibrary;
		}

		private void OnCanAddPersistent(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pitem = uitem.Object as EnvDTE.ProjectItem;
					menu.Visible = menu.Enabled = pitem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder;
				}
				else if (uitem.Object is EnvDTE.Project)
				{
					menu.Visible = menu.Enabled = true;
				}
			}
		}

		private void OnAddPersistent(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pitem = uitem.Object as EnvDTE.ProjectItem;
					if (pitem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
					{
						CreatePersistentForm view = new CreatePersistentForm(this, pitem.ContainingProject, pitem);
						view.ShowModal();
					}
				}
				else if (uitem.Object is EnvDTE.Project)
				{
					EnvDTE.Project project = uitem.Object as EnvDTE.Project;
					CreatePersistentForm view = new CreatePersistentForm(this, project, null);
					view.ShowModal();
				}
			}
		}

		private void OnCanAddLocalization(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pitem = uitem.Object as EnvDTE.ProjectItem;
					menu.Visible = menu.Enabled = pitem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder;
				}
				else if (uitem.Object is EnvDTE.Project)
				{
					menu.Visible = menu.Enabled = true;
				}
			}
		}

		private void OnAddLocalization(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem pitem)
				{
					if (pitem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
					{
						AddLocalizationFile(pitem.ProjectItems);
					}
				}
				else if (uitem.Object is EnvDTE.Project project) { AddLocalizationFile(project.ProjectItems); }
			}
		}

		private void AddLocalizationFile(EnvDTE.ProjectItems items)
		{
			EnvDTE.Project project = items.ContainingProject;
			string fullPath = "";

			if (items.Parent is EnvDTE.ProjectItem item)
			{
				EnvDTE.Property propertyFullPath = item.Properties.Item("FullPath");
				fullPath = (string)propertyFullPath.Value;
			}
			else
			{
				EnvDTE.Property propertyFullPath = project.Properties.Item("FullPath");
				fullPath = (string)propertyFullPath.Value;
			}
			CreateLocalizationDialog dialog = new CreateLocalizationDialog(fullPath, "Strings");
			if (dialog.ShowModal() == true)
			{
				string fileName = dialog.FileName;
				if (fileName.EndsWith(LocalizedExtension) == false) { fileName = string.Concat(fileName, LocalizedExtension); }
				string filePath = string.Concat(fullPath, "\\", fileName);

				if (string.IsNullOrEmpty(filePath)) { this.ShowMessage("创建本地化资源文件失败"); return; }
				LocalizationCollection collection = new LocalizationCollection();
				using (XmlWriter writer = XmlWriter.Create(filePath))
				{
					collection.WriteSchema(writer);
					collection.WriteXml(writer);
				}
				EnvDTE.ProjectItem itemLocalization = items.AddFromFile(filePath);
				EnvDTE.Property property = itemLocalization.Properties.Item("CustomTool");
				property.Value = "ResourceGenerator";
				itemLocalization.Open();
			}
		}

		private void OnCanAddController(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem item = uitem.Object as EnvDTE.ProjectItem;
					if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
					{
						ProjectTypes projectType = GetProjectType(item.ContainingProject);
						menu.Visible = menu.Enabled = projectType == ProjectTypes.MvcApplication;
					}
				}
				else if (uitem.Object is EnvDTE.Project)
				{
					ProjectTypes projectType = GetProjectType(uitem.Object as EnvDTE.Project);
					menu.Visible = menu.Enabled = projectType == ProjectTypes.MvcApplication;
				}
			}
		}

		private void OnAddController(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem item = uitem.Object as EnvDTE.ProjectItem;
					ProjectTypes projectType = GetProjectType(item.ContainingProject);
					if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder && projectType == ProjectTypes.MvcApplication)
					{
						CreateController controllerWindow = new CreateController(this, item.ContainingProject, item);
						controllerWindow.ShowModal();
					}
				}
				else if (uitem.Object is EnvDTE.Project)
				{
					EnvDTE.Project project = uitem.Object as EnvDTE.Project;
					ProjectTypes projectType = GetProjectType(project);
					if (projectType == ProjectTypes.MvcApplication)
					{
						CreateController controllerWindow = new CreateController(this, project, null);
						controllerWindow.ShowModal();
					}
				}
			}
		}

		private void OnCanAddClassicView(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem item = uitem.Object as EnvDTE.ProjectItem;
					EnvDTE.Property propertyFullPath = item.Properties.Item("FullPath");
					string fullPath = (string)propertyFullPath.Value;
					if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder && fullPath.Contains(@"\Views\"))
					{
						ProjectTypes projectType = GetProjectType(item.ContainingProject);
						menu.Visible = menu.Enabled = projectType == ProjectTypes.MvcApplication;
					}
				}
				//else if (item.Object is EnvDTE.Project)
				//{
				//    ProjectTypeEnum projectType = GetProjectType(item.Object as EnvDTE.Project);
				//    menu.Visible = menu.Enabled = projectType == ProjectTypeEnum.MvcApplication;
				//}
				//VSLangProj.VSProject vsProject = project.Object as VSLangProj.VSProject;
			}
		}

		private void OnAddClassicView(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					//EnvDTE.ProjectItem item = uitem.Object as EnvDTE.ProjectItem;
					CreateClassicView viewWindow = new CreateClassicView(this, uitem.Object as EnvDTE.ProjectItem);
					viewWindow.ShowModal();
				}
			}
		}

		private void OnCanAddMvcView(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem item = uitem.Object as EnvDTE.ProjectItem;
					EnvDTE.Property propertyFullPath = item.Properties.Item("FullPath");
					string fullPath = (string)propertyFullPath.Value;
					if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder && fullPath.Contains(@"\Views\"))
					{
						ProjectTypes projectType = GetProjectType(item.ContainingProject);
						menu.Visible = menu.Enabled = projectType == ProjectTypes.MvcApplication;
					}
				}
			}
		}

		private void OnAddMvcView(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					//EnvDTE.ProjectItem item = uitem.Object as EnvDTE.ProjectItem;
					MvcViewWindow viewWindow = new MvcViewWindow(this, uitem.Object as EnvDTE.ProjectItem);
					viewWindow.ShowModal();
				}
			}
		}

		private void OnCanAddWpfForm(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pitem = uitem.Object as EnvDTE.ProjectItem;
					if (pitem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
					{
						ProjectTypes projectType = GetProjectType(pitem.ContainingProject);
						menu.Visible = menu.Enabled = projectType == ProjectTypes.WpfApplication || projectType == ProjectTypes.ClassLibrary;
					}
				}
				else if (uitem.Object is EnvDTE.Project)
				{
					ProjectTypes projectType = GetProjectType(uitem.Object as EnvDTE.Project);
					menu.Visible = menu.Enabled = projectType == ProjectTypes.WpfApplication || projectType == ProjectTypes.ClassLibrary;
				}
			}
		}

		private void OnAddWpfForm(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem uitem in array)
			{
				if (uitem.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pitem = uitem.Object as EnvDTE.ProjectItem;
					if (pitem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
					{
						ProjectTypes projectType = GetProjectType(pitem.ContainingProject);
						if (projectType == ProjectTypes.WpfApplication || projectType == ProjectTypes.ClassLibrary)
						{
							CreateWpfView view = new CreateWpfView(this, pitem.ContainingProject, pitem);
							view.ShowModal();
						}
					}
				}
				else if (uitem.Object is EnvDTE.Project)
				{
					EnvDTE.Project project = uitem.Object as EnvDTE.Project;
					ProjectTypes projectType = GetProjectType(project);
					if (projectType == ProjectTypes.WpfApplication || projectType == ProjectTypes.ClassLibrary)
					{
						CreateWpfView view = new CreateWpfView(this, project, null);
						view.ShowModal();
					}
				}
			}
		}

		private void OnCanConvert(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			string[] pattenArray = fileExtension.Split(';');
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				if (item.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
					if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
					{
						foreach (string extension in pattenArray)
						{
							if (pItem.Name.EndsWith(extension)) { menu.Enabled = menu.Visible = true; return; }
							else { menu.Enabled = menu.Visible = false; continue; }
						}
					}
				}
			}
		}

		private void OnConvert(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				if (item.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
					if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile &&
						 (pItem.Name.EndsWith(".sqlf") || pItem.Name.EndsWith(".oraf") || pItem.Name.EndsWith(".olef")
								|| pItem.Name.EndsWith(".odbcf") || pItem.Name.EndsWith(".db2f") || pItem.Name.EndsWith(".litf")))
					{
						EnvDTE.Property propertyFullPath = pItem.Properties.Item("FullPath");
						ConverterConfiguration converter = new ConverterConfiguration((string)propertyFullPath.Value);
						using (XmlWriter writer = XmlWriter.Create((string)propertyFullPath.Value, new XmlWriterSettings() { Indent = true }))
						{
							converter.WriteXml(writer);
						}
					}
				}
			}
		}

		private void OnCanConvertAll(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			string[] pattenArray = pattern.Split(';');
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				if (item.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
					if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder || pItem.Kind == EnvDTE.Constants.vsProjectItemKindSolutionItems)
					{
						EnvDTE.Property propertyFullPath = pItem.Properties.Item("FullPath");
						System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo((string)propertyFullPath.Value);
						foreach (string strPattern in pattenArray)
						{
							System.IO.FileInfo[] files = directoryInfo.GetFiles(strPattern, System.IO.SearchOption.AllDirectories);
							if (files == null || files.Length == 0) { continue; }
							foreach (System.IO.FileInfo fileInfo in files)
							{
								EnvDTE.ProjectItem tempItem = dteClass.Solution.FindProjectItem(fileInfo.FullName);
								if (tempItem != null) { menu.Enabled = menu.Visible = true; return; }
							}
						}
					}
				}
				else if (item.Object is EnvDTE.Project)
				{
					EnvDTE.Project project = item.Object as EnvDTE.Project;
					if (project.Kind == VSLangProj.PrjKind.prjKindCSharpProject || project.Kind == VSLangProj.PrjKind.prjKindVBProject)
					{
						EnvDTE.Property propertyFullPath = project.Properties.Item("FullPath");
						System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo((string)propertyFullPath.Value);
						foreach (string strPattern in pattenArray)
						{
							System.IO.FileInfo[] files = directoryInfo.GetFiles(strPattern, System.IO.SearchOption.AllDirectories);
							if (files == null || files.Length == 0) { continue; }
							foreach (System.IO.FileInfo fileInfo in files)
							{
								EnvDTE.ProjectItem tempItem = dteClass.Solution.FindProjectItem(fileInfo.FullName);
								if (tempItem != null) { menu.Enabled = menu.Visible = true; return; }
							}
						}
					}
				}
			}

		}

		private void OnConvertAll(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			string[] pattenArray = pattern.Split(';');
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				if (item.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
					if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder || pItem.Kind == EnvDTE.Constants.vsProjectItemKindSolutionItems)
					{
						EnvDTE.Property propertyFullPath = pItem.Properties.Item("FullPath");
						System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo((string)propertyFullPath.Value);
						foreach (string strPattern in pattenArray)
						{
							System.IO.FileInfo[] files = directoryInfo.GetFiles(strPattern, System.IO.SearchOption.AllDirectories);
							if (files == null || files.Length == 0) { continue; }
							foreach (System.IO.FileInfo fileInfo in files)
							{
								EnvDTE.ProjectItem tempItem = dteClass.Solution.FindProjectItem(fileInfo.FullName);
								if (tempItem == null) { continue; }
								if (tempItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
								{
									propertyFullPath = tempItem.Properties.Item("FullPath");
									ConverterConfiguration converter = new ConverterConfiguration((string)propertyFullPath.Value);
									using (XmlWriter writer = XmlWriter.Create((string)propertyFullPath.Value, new XmlWriterSettings() { Indent = true }))
									{
										converter.WriteXml(writer);
									}
								}
							}
						}
					}
				}
				else if (item.Object is EnvDTE.Project)
				{
					EnvDTE.Project project = item.Object as EnvDTE.Project;
					if (project.Kind == VSLangProj.PrjKind.prjKindCSharpProject || project.Kind == VSLangProj.PrjKind.prjKindVBProject)
					{
						EnvDTE.Property propertyFullPath = project.Properties.Item("FullPath");
						System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo((string)propertyFullPath.Value);
						foreach (string strPattern in pattenArray)
						{
							System.IO.FileInfo[] files = directoryInfo.GetFiles(strPattern, System.IO.SearchOption.AllDirectories);
							if (files == null || files.Length == 0) { continue; }
							foreach (System.IO.FileInfo fileInfo in files)
							{
								EnvDTE.ProjectItem tempItem = dteClass.Solution.FindProjectItem(fileInfo.FullName);
								if (tempItem == null) { continue; }
								if (tempItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
								{
									propertyFullPath = tempItem.Properties.Item("FullPath");
									ConverterConfiguration converter = new ConverterConfiguration((string)propertyFullPath.Value);
									using (XmlWriter writer = XmlWriter.Create((string)propertyFullPath.Value, new XmlWriterSettings() { Indent = true }))
									{
										converter.WriteXml(writer);
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion

	}
}
