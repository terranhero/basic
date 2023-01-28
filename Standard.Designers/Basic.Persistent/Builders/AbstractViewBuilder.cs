using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Basic.Collections;
using Basic.Configuration;
using Basic.DataEntities;
using Basic.EntityLayer;
using Microsoft.VisualStudio.TextTemplating;

namespace Basic.Builders
{
	/// <summary>
	/// 表示抽象视图构建器
	/// </summary>
	[Serializable]
	public abstract class AbstractViewBuilder : AbstractPropertyChanged, ITextTemplatingEngineHost, ITextTemplatingSessionHost
	{
		[System.NonSerialized()]
		private readonly EnvDTE.Project _Project;
		[System.NonSerialized()]
		private readonly ObservableCollection<DropDownFile> _Files;
		[System.NonSerialized()]
		internal protected readonly PersistentConfiguration _Persistent;
		[System.NonSerialized()]
		internal protected readonly AbstractEntityColllection _AbstractEntities;
		[System.NonSerialized()]
		protected readonly PersistentService _CommandService;
		[System.NonSerialized()]
		protected readonly CodeDomProvider _Provider;

		/// <summary>
		/// 模版文件夹
		/// </summary>
		internal const string TemplateFolder = "Templates";
		private readonly string _AssemblyPath;
		private readonly string _AssemblyFile;
		private readonly string _DefaultNamespance;
		/// <summary>
		/// 初始化 AbstractViewBuilder 类实例
		/// </summary>
		protected AbstractViewBuilder(PersistentService commandService, EnvDTE.Project project)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			_CommandService = commandService;
			_Files = new ObservableCollection<DropDownFile>();
			_Persistent = new PersistentConfiguration();
			_Persistent.DataEntities.CollectionChanged += new NotifyCollectionChangedEventHandler(DataEntities_CollectionChanged);
			_AbstractEntities = new AbstractEntityColllection(_Persistent);
			_Project = project;
			_Provider = _CommandService.CreateCodeProvider(project);
			_AssemblyFile = typeof(AbstractViewBuilder).Assembly.Location;
			_AssemblyPath = Path.GetDirectoryName(_AssemblyFile);
			EnvDTE.Property nsProperty = _Project.Properties.Item("DefaultNamespace");
			if (nsProperty != null) { _DefaultNamespance = (string)nsProperty.Value; }
		}

		private void DataEntities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (DataEntityElement entity in e.NewItems)
				{
					_AbstractEntities.Add(entity);
					if (entity.Condition.Arguments.Count > 0 || entity.BaseCondition != typeof(AbstractCondition).FullName)
						_AbstractEntities.Add(entity.Condition);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (DataEntityElement entity in e.OldItems)
				{
					_AbstractEntities.Remove(entity);
					_AbstractEntities.Remove(entity.Condition);
				}
				foreach (DataEntityElement entity in e.NewItems)
				{
					_AbstractEntities.Add(entity);
					if (entity.Condition.Arguments.Count > 0 || entity.BaseCondition != typeof(AbstractCondition).FullName)
						_AbstractEntities.Add(entity.Condition);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (DataEntityElement entity in e.OldItems)
				{
					_AbstractEntities.Remove(entity);
					_AbstractEntities.Remove(entity.Condition);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				_AbstractEntities.Clear();
			}
		}

		/// <summary>
		/// 表示程序集的路径
		/// </summary>
		public string AssemblyPath { get { return _AssemblyPath; } }

		/// <summary>
		/// 当前项的默认命名空间
		/// </summary>
		public virtual string DefaultNamespance { get { return _DefaultNamespance; } }

		/// <summary>
		/// 表示数据持久类设计文件内容
		/// </summary>

		public PersistentConfiguration Persistent { get { return _Persistent; } }

		/// <summary>
		/// 表示配置文件中实体类集合
		/// </summary>
		public AbstractEntityColllection Entities { get { return _AbstractEntities; } }

		/// <summary>
		/// 表示配置文件中实体类集合
		/// </summary>
		public DataEntityElementCollection DataEntities { get { return _Persistent.DataEntities; } }

		/// <summary>
		/// 模版名称
		/// </summary>
		protected abstract string TemplateName { get; }
		/// <summary>
		/// 生成 CodeDOM 可编译代码单元
		/// </summary>
		/// <param name="codeComplieUnit">需要生成的 CodeCompileUnit 类实例。</param>
		/// <param name="defaultNamespace">生成类的默认命名空间。</param>
		public virtual void GenerateCode(StreamWriter writer, string defaultNamespace)
		{
			string fileName = string.Concat(TemplateName, ".", _Provider.FileExtension, ".tt");
			_TemplateFile = Path.Combine(_AssemblyPath, TemplateFolder, fileName);
			if (!File.Exists(_TemplateFile)) { _CommandService.ShowMessage("模版文件不存在！" + _TemplateFile); return; }
			ITextTemplatingSession session = CreateSession();
			session.Clear();
			session.Add("defaultNamespance", defaultNamespace);
			//session.Add("builder", this);
			//session.Add("persistent", _Persistent);
			string input = File.ReadAllText(_TemplateFile);
			string output = new Engine().ProcessTemplate(input, this);
			if (errorsValue.Count > 0)
			{
				foreach (System.CodeDom.Compiler.CompilerError error in errorsValue)
				{
					writer.WriteLine(error.ErrorText);
				}
				return;
			}
			writer.Write(output);
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual void InitlizeFiles()
		{
			string pname = _Project.Name;
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(_Project.FullName);
			System.IO.FileInfo[] dpdlFiles = fileInfo.Directory.GetFiles("*.dpdl", System.IO.SearchOption.AllDirectories);
			foreach (System.IO.FileInfo dpdlFile in dpdlFiles)
			{
				string name = dpdlFile.FullName.Replace(fileInfo.Directory.FullName, pname);
				_Files.Add(new DropDownFile(name, dpdlFile.FullName));
			}

			VSLangProj.VSProject vsProject = _Project.Object as VSLangProj.VSProject;
			foreach (VSLangProj.Reference reference in vsProject.References)
			{
				if (reference.Type == VSLangProj.prjReferenceType.prjReferenceTypeAssembly && reference.SourceProject != null)
				{
					EnvDTE.Project project = reference.SourceProject;
					fileInfo = new System.IO.FileInfo(project.FullName);
					dpdlFiles = fileInfo.Directory.GetFiles("*.dpdl", System.IO.SearchOption.AllDirectories);
					foreach (System.IO.FileInfo dpdlFile in dpdlFiles)
					{
						string name = dpdlFile.FullName.Replace(fileInfo.Directory.FullName, project.Name);
						_Files.Add(new DropDownFile(name, dpdlFile.FullName));
					}
				}
			}
		}

		/// <summary>
		/// 表示数据持久类文件名称含路径。
		/// </summary>
		public ObservableCollection<DropDownFile> Files { get { return _Files; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		protected virtual void ReadPersistent(string fileName)
		{
			FileInfo fileinfo = new FileInfo(fileName);
			_Persistent.ClearContent();
			using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(_FileName))
			{
				_Persistent.ReadXml(reader);
			}
			Controller = _Persistent.TableInfo.EntityName;
		}

		private string _FileName = null;
		/// <summary>
		/// 表示数据持久类文件名称含路径。
		/// </summary>
		public string FileName
		{
			get { return _FileName; }
			set
			{
				if (_FileName != value)
				{
					_FileName = value;
					ReadPersistent(_FileName);
					OnPropertyChanged("FileName");
					OnPropertyChanged("SelectedFile");
					OnPropertyChanged("OkEnabled");
				}
			}
		}

		/// <summary>
		/// 判断文件是否已选择。
		/// </summary>
		public bool SelectedFile { get { return !string.IsNullOrEmpty(_FileName); } }

		private string _Controller = null;
		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string Controller
		{
			get { return _Controller; }
			set { if (_Controller != value) { _Controller = value; OnPropertyChanged("Controller"); OnPropertyChanged("OkEnabled"); } }
		}

		private bool _ControllerVisibled = true;
		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public virtual bool ControllerVisibled
		{
			get { return _ControllerVisibled; }
			set { if (_ControllerVisibled != value) { _ControllerVisibled = value; OnPropertyChanged("ControllerVisibled"); } }
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public virtual bool OkEnabled
		{
			get
			{
				if (_ControllerVisibled) { return !string.IsNullOrEmpty(_FileName) && !string.IsNullOrEmpty(_Controller); }
				return !string.IsNullOrEmpty(_FileName);
			}
		}

		#region ITextTemplatingSessionHost
		private ITextTemplatingSession _Session;

		/// <summary>
		///  Create a Session object that can be used to transmit information into a template.The new Session becomes the current Session.
		/// </summary>
		/// <returns>A new Session</returns>
		public ITextTemplatingSession CreateSession()
		{
			if (_Session == null)
				_Session = new TextTemplatingSession(Guid.NewGuid());
			return _Session;
		}

		/// <summary>
		/// The current Session.
		/// </summary>
		public ITextTemplatingSession Session { get { return _Session; } set { _Session = value; } }
		#endregion

		#region ITextTemplatingEngineHost
		/// <summary>
		/// Called by the Engine to ask for the value of a specified option. Return null if you do not know.
		/// </summary>
		/// <param name="optionName">The name of an option.</param>
		/// <returns>Null to select the default value for this option. Otherwise, an appropriate value for the option.</returns>
		public object GetHostOption(string optionName)
		{
			if (optionName == "CacheAssemblies") { return true; }
			return null;
		}

		/// <summary>
		///  Acquires the text that corresponds to a request to include a partial text template file.
		/// </summary>
		/// <param name="requestFileName">The name of the partial text template file to acquire.</param>
		/// <param name="content">A System.String that contains the acquired text or System.String.Empty if the file could not be found.</param>
		/// <param name="location">
		/// A System.String that contains the location of the acquired text. If the host
		/// searches the registry for the location of include files or if the host searches
		/// multiple locations by default, the host can return the final path of the
		/// include file in this parameter. The host can set the location to System.String.Empty
		/// if the file could not be found or if the host is not file-system based.
		/// </param>
		/// <returns>true to indicate that the host was able to acquire the text; otherwise, false.</returns>
		public bool LoadIncludeText(string requestFileName, out string content, out string location)
		{
			content = string.Empty;
			location = string.Empty;

			if (File.Exists(requestFileName))
			{
				content = File.ReadAllText(requestFileName);
				return true;
			}
			else
			{
				return false;
			}
		}

		internal System.CodeDom.Compiler.CompilerErrorCollection errorsValue;
		/// <summary>
		/// Receives a collection of errors and warnings from the transformation engine.
		/// </summary>
		/// <param name="errors">The System.CodeDom.Compiler.CompilerErrorCollection being passed to the host from the engine.</param>
		public void LogErrors(System.CodeDom.Compiler.CompilerErrorCollection errors)
		{
			errorsValue = errors;
		}

		/// <summary>
		/// Provides an application domain to run the generated transformation class.
		/// </summary>
		/// <param name="content">The contents of the text template file to be processed.</param>
		/// <returns>An System.AppDomain that compiles and executes the generated transformation class.</returns>
		public AppDomain ProvideTemplatingAppDomain(string content)
		{
			return AppDomain.CurrentDomain;
		}

		/// <summary>
		/// Allows a host to provide additional information about the location of an assembly.
		/// </summary>
		/// <param name="assemblyReference"></param>
		/// <returns></returns>
		public string ResolveAssemblyReference(string assemblyReference)
		{
			if (File.Exists(assemblyReference)) { return assemblyReference; }

			string candidate = Path.Combine(Path.GetDirectoryName(_AssemblyPath), assemblyReference);
			if (File.Exists(candidate)) { return candidate; }
			return candidate;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="processorName"></param>
		/// <returns></returns>
		public Type ResolveDirectiveProcessor(string processorName)
		{
			if (string.Compare(processorName, "XYZ", StringComparison.OrdinalIgnoreCase) == 0)
			{
				//return typeof();
			}
			throw new Exception("Directive Processor not found");
		}

		public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
		{
			if (directiveId == null)
			{
				throw new ArgumentNullException("the directiveId cannot be null");
			}
			if (processorName == null)
			{
				throw new ArgumentNullException("the processorName cannot be null");
			}
			if (parameterName == null)
			{
				throw new ArgumentNullException("the parameterName cannot be null");
			}
			return String.Empty;
		}

		public string ResolvePath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("the file name cannot be null");
			}
			if (File.Exists(path))
			{
				return path;
			}
			string candidate = Path.Combine(Path.GetDirectoryName(this.TemplateFile), path);
			if (File.Exists(candidate))
			{
				return candidate;
			}
			return path;
		}

		public void SetFileExtension(string extension)
		{
		}

		public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
		{
		}

		public IList<string> StandardAssemblyReferences
		{
			get
			{
				return new string[] { typeof(System.Uri).Assembly.Location, typeof(Queryable).Assembly.Location, _AssemblyFile };
			}
		}

		public IList<string> StandardImports
		{
			get { return new string[] { "System" }; }
		}

		private string _TemplateFile;
		/// <summary>
		/// 
		/// </summary>
		public string TemplateFile { get { return _TemplateFile; } set { _TemplateFile = value; } }

		//private string fileExtensionValue = ".cs";
		//public string FileExtension
		//{
		//	get { return fileExtensionValue; }
		//}
		#endregion
	}
}
