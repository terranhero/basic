using Basic.Configuration;
using Basic.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Builders
{
	/// <summary>
	/// MVC 项目中视图构建器。
	/// </summary>
	internal abstract class AbstractMvcViewBuilder : AbstractViewBuilder
	{
		private readonly ObservableCollection<DropDownFile> _TemplateFiles;
		private readonly ObservableCollection<MvcView> _Views;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		/// <summary>
		/// 初始化 AbstractMvcViewBuilder 类实例
		/// </summary>
		protected AbstractMvcViewBuilder(PersistentService commandService, EnvDTE.ProjectItem item)
			: base(commandService, item.ContainingProject)
		{
			_ProjectItem = item;
			_TemplateFiles = new ObservableCollection<DropDownFile>();
			_Views = new ObservableCollection<MvcView>();
			InitilizeTemplates();
			InilitilizeViews(_Views);
		}

		/// <summary>
		/// 模版名称
		/// </summary>
		protected override string TemplateName { get { return ""; } }

		/// <summary>
		/// 表示配置文件中实体类集合
		/// </summary>
		public ObservableCollection<DropDownFile> TemplateFiles { get { return _TemplateFiles; } }

		/// <summary>
		/// 表示配置文件中实体类集合
		/// </summary>
		public ObservableCollection<MvcView> Views { get { return _Views; } }

		/// <summary>
		/// 
		/// </summary>
		public virtual void InitilizeTemplates()
		{
			System.IO.FileInfo projectFileInfo = new System.IO.FileInfo(_ProjectItem.ContainingProject.FullName);
			System.IO.DirectoryInfo[] directories = projectFileInfo.Directory.GetDirectories("Views\\Shared", System.IO.SearchOption.AllDirectories);
			foreach (System.IO.DirectoryInfo directoryInfo in directories)
			{
				System.IO.FileInfo[] files = directoryInfo.GetFiles("*.cshtml", System.IO.SearchOption.TopDirectoryOnly);
				CreateCshtmlFileInfos(files, projectFileInfo);
			}
			directories = projectFileInfo.Directory.GetDirectories("Master", System.IO.SearchOption.AllDirectories);
			foreach (System.IO.DirectoryInfo directoryInfo in directories)
			{
				System.IO.FileInfo[] files = directoryInfo.GetFiles("*.cshtml", System.IO.SearchOption.TopDirectoryOnly);
				CreateCshtmlFileInfos(files, projectFileInfo);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="views"></param>
		protected abstract void InilitilizeViews(ObservableCollection<MvcView> views);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileInfoArray"></param>
		/// <param name="projectFileInfo"></param>
		protected virtual void CreateCshtmlFileInfos(System.IO.FileInfo[] fileInfoArray, System.IO.FileInfo projectFileInfo)
		{
			foreach (System.IO.FileInfo file in fileInfoArray)
			{
				string name = file.FullName.Replace(projectFileInfo.Directory.FullName, "~").Replace("\\", "/");
				_TemplateFiles.Add(new DropDownFile(name, file.FullName));
			}
		}
	}
}
