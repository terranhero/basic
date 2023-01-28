using System;
using System.Collections.ObjectModel;
using System.IO;
using Basic.Configuration;
using Basic.DataEntities;
using Basic.EntityLayer;
using RazorEngineCore;

namespace Basic.Builders
{
	internal sealed class MvcViewBuilder : AbstractPropertyChanged
	{
		private readonly EnvDTE.ProjectItem _ProjectItem;
		private readonly string _AssemblyPath;
		private readonly string _AssemblyFile;
		private readonly ObservableCollection<string> _Models;
		private readonly ObservableCollection<DropDownFile> _dpdlFiles;
		private readonly ObservableCollection<DropDownFile> _TemplateFiles;
		/// <summary>模版文件夹</summary>
		private const string TemplateFolder = "Templates";
		private readonly PersistentService _CommandService;

		/// <summary>初始化 AbstractMvcViewBuilder 类实例</summary>
		internal MvcViewBuilder(PersistentService commandService, EnvDTE.ProjectItem item)
		{
			_CommandService = commandService; _ProjectItem = item;
			_AssemblyFile = typeof(MvcViewBuilder).Assembly.Location;
			_AssemblyPath = Path.GetDirectoryName(_AssemblyFile);
			_TemplateFiles = new ObservableCollection<DropDownFile>();
			_dpdlFiles = new ObservableCollection<DropDownFile>();
			_Models = new ObservableCollection<string>();

		}

		/// <summary>模板文件列表</summary>
		public ObservableCollection<DropDownFile> Templates { get { return _TemplateFiles; } }

		/// <summary>模板文件</summary>
		public string SelectedTemplate
		{
			get { return _TemplateFile; }
			set
			{
				_TemplateFile = value;
				OnPropertyChanged("SelectedTemplate");
				OnPropertyChanged("OkEnabled");
			}
		}
		private string _TemplateFile { get; set; }

		/// <summary>模板文件列表</summary>
		public ObservableCollection<string> Models { get { return _Models; } }

		/// <summary>模板文件列表</summary>
		public ObservableCollection<DropDownFile> Files { get { return _dpdlFiles; } }

		/// <summary>模型文件文件</summary>
		public string SelectedFile
		{
			get { return _SelectedFile; }
			set
			{
				_SelectedFile = value; OnPropertyChanged("SelectedFile");
				InitlizeModels(value);
			}
		}
		private string _SelectedFile { get; set; }

		/// <summary>模型文件文件</summary>
		public string ModelName
		{
			get { return _ModelName; }
			set
			{
				_ModelName = value;
				OnPropertyChanged("ModelName");
			}
		}
		private string _ModelName { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool OkEnabled { get; private set; }

		/// <summary>初始化模板文件</summary>
		internal void InitlizeTemplates()
		{
			string tempPath = Path.Combine(_AssemblyPath, TemplateFolder);
			DirectoryInfo folder = new DirectoryInfo(tempPath);
			tempPath += "\\";
			foreach (FileInfo t4File in folder.GetFiles("*.razor", SearchOption.AllDirectories))
			{
				string fileName = t4File.FullName.Replace(tempPath, "");
				_TemplateFiles.Add(new DropDownFile(fileName, t4File.FullName));
			}

			foreach (FileInfo t4File in folder.GetFiles("*.cshtml", SearchOption.AllDirectories))
			{
				string fileName = t4File.FullName.Replace(tempPath, "");
				_TemplateFiles.Add(new DropDownFile(fileName, t4File.FullName));
			}

			//VSLangProj.VSProject vsProject = _Project.Object as VSLangProj.VSProject;
			//foreach (VSLangProj.Reference reference in vsProject.References)
			//{
			//	if (reference.Type == VSLangProj.prjReferenceType.prjReferenceTypeAssembly && reference.SourceProject != null)
			//	{
			//		EnvDTE.Project project = reference.SourceProject;
			//		fileInfo = new System.IO.FileInfo(project.FullName);
			//		dpdlFiles = fileInfo.Directory.GetFiles("*.dpdl", System.IO.SearchOption.AllDirectories);
			//		foreach (System.IO.FileInfo dpdlFile in dpdlFiles)
			//		{
			//			string name = dpdlFile.FullName.Replace(fileInfo.Directory.FullName, project.Name);
			//			_Files.Add(new DropDownFile(name, dpdlFile.FullName));
			//		}
			//	}
			//}
		}

		/// <summary>初始化模板文件</summary>
		/// <param name="dpdlPath"></param>
		internal void InitlizeModels(string dpdlPath)
		{
			PersistentConfiguration persistent = new PersistentConfiguration();
			using (StreamReader reader = new StreamReader(dpdlPath))
			{
				persistent.ReadXml(reader);
			}

			_Models.Clear(); ModelName = null;
			foreach (DataEntityElement entity in persistent.DataEntities)
			{
				_Models.Add(entity.FullName);
				if (entity.Condition.Arguments.Count > 0)
					_Models.Add(entity.Condition.FullName);
				else if (entity.BaseCondition != typeof(AbstractCondition).FullName)
					_Models.Add(entity.Condition.FullName);
			}

			//IVsSolution2 vsSolution = await _CommandService.GetServiceAsync(typeof(SVsSolution)) as IVsSolution2;
			//vsSolution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
			//vsSolution.GetGuidOfProject(hierarchy, out Guid projectGuid);
			//IVsHierarchy ivsh = VsShellUtilities.GetHierarchy(_CommandService.AsyncPackage, projectGuid);
			//DynamicTypeService typeService = await _CommandService.GetServiceAsync(typeof(DynamicTypeService)) as DynamicTypeService;
			//ITypeDiscoveryService discovery = typeService.GetTypeDiscoveryService(ivsh);
			//ICollection types = discovery.GetTypes(typeof(object), false);
			//foreach (Type type in types) { _Models.Add(type.Name); }
		}

		/// <summary>初始化模板文件</summary>
		internal void InitlizeFiles()
		{
			EnvDTE.Project project = _ProjectItem.ContainingProject;
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(project.FullName);
			System.IO.FileInfo[] files = fileInfo.Directory.GetFiles("*.dpdl", System.IO.SearchOption.AllDirectories);
			foreach (System.IO.FileInfo dpdlFile in files)
			{
				string name = dpdlFile.FullName.Replace(fileInfo.Directory.FullName, project.Name);
				_dpdlFiles.Add(new DropDownFile(name, dpdlFile.FullName));
			}

			VSLangProj.VSProject vsProject = project.Object as VSLangProj.VSProject;
			foreach (VSLangProj.Reference reference in vsProject.References)
			{
				if (reference.Type == VSLangProj.prjReferenceType.prjReferenceTypeAssembly && reference.SourceProject != null)
				{
					EnvDTE.Project proj = reference.SourceProject;
					fileInfo = new System.IO.FileInfo(proj.FullName);
					files = fileInfo.Directory.GetFiles("*.dpdl", SearchOption.AllDirectories);
					foreach (System.IO.FileInfo dpdlFile in files)
					{
						string name = dpdlFile.FullName.Replace(fileInfo.Directory.FullName, proj.Name);
						_dpdlFiles.Add(new DropDownFile(name, dpdlFile.FullName));
					}
				}
			}

			//IVsSolution2 vsSolution = await _CommandService.GetServiceAsync(typeof(SVsSolution)) as IVsSolution2;
			//vsSolution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
			//vsSolution.GetGuidOfProject(hierarchy, out Guid projectGuid);
			//IVsHierarchy ivsh = VsShellUtilities.GetHierarchy(_CommandService.AsyncPackage, projectGuid);
			//DynamicTypeService typeService = await _CommandService.GetServiceAsync(typeof(DynamicTypeService)) as DynamicTypeService;
			//ITypeDiscoveryService discovery = typeService.GetTypeDiscoveryService(ivsh);
			//ICollection types = discovery.GetTypes(typeof(object), false);
			//foreach (Type type in types) { _Models.Add(type.Name); }
		}

		/// <summary></summary>
		/// <returns></returns>
		internal string GetRazorContent(string razorFile, Type type)
		{
			try
			{
				if (Razor.TryGetTemplate(razorFile, out IRazorEngineCompiledTemplate template))
				{
					string result = template.Run(type);
					OkEnabled = true; OnPropertyChanged("OkEnabled");
					return result;
				}
				else
				{
					using (StreamReader reader = new StreamReader(razorFile))
					{
						string content = reader.ReadToEnd();
						template = Razor.CompileTemplate(razorFile, content);
						string result = template.Run(type);
						OkEnabled = true; OnPropertyChanged("OkEnabled");
						return result;
					}
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}
