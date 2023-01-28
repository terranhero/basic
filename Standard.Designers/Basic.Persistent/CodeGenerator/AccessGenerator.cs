using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Basic.Configuration;
using Basic.Enums;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using VSLangProj80;
using CD = System.CodeDom;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;

namespace Basic.EntityDesigner
{
	/// <summary>
	/// This is the generator class. 
	/// When setting the 'Custom Tool' property of a C#, VB, or J# project item to "XmlClassGenerator", 
	/// the GenerateCode function will get called and will return the contents of the generated file 
	/// to the project system
	/// </summary>
	[System.Runtime.InteropServices.Guid("F34B88B0-2E10-4132-AAA0-6EF6DBF26355")]
	[CodeGeneratorRegistration(typeof(AccessGenerator), "C# Generator", vsContextGuids.vsContextGuidVCSProject)]
	[CodeGeneratorRegistration(typeof(AccessGenerator), "VB Generator", vsContextGuids.vsContextGuidVBProject)]
	[ProvideObject(typeof(AccessGenerator)), ComVisible(true)]
	public sealed class AccessGenerator : IVsSingleFileGenerator, VSOLE.IObjectWithSite, IDisposable
	{
		#region IObjectWithSite,IDisposable
		/// <summary>
		/// GetSite method of IOleObjectWithSite
		/// </summary>
		/// <param name="riid">interface to get</param>
		/// <param name="ppvSite">IntPtr in which to stuff return value</param>
		void VSOLE.IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppvSite)
		{
			if (site == null)
			{
				throw new COMException("object is not sited", VSConstants.E_FAIL);
			}

			IntPtr pUnknownPointer = Marshal.GetIUnknownForObject(site);
			Marshal.QueryInterface(pUnknownPointer, ref riid, out IntPtr intPointer);

			if (intPointer == IntPtr.Zero)
			{
				throw new COMException("site does not support requested interface", VSConstants.E_NOINTERFACE);
			}

			ppvSite = intPointer;
		}

		/// <summary>
		/// SetSite method of IOleObjectWithSite
		/// </summary>
		/// <param name="pUnkSite">site for this object to use</param>
		void VSOLE.IObjectWithSite.SetSite(object pUnkSite)
		{
			site = pUnkSite;
			codeDomProvider = null;
			serviceProvider = null;
		}
		#endregion

		/// <summary>
		/// 释放由组件使用的所有资源。
		/// </summary>
		void IDisposable.Dispose()
		{
			if (codeDomProvider != null) { codeDomProvider.Dispose(); }
			if (serviceProvider != null) { serviceProvider.Dispose(); }
			GC.SuppressFinalize(this);
		}
		private string defaultDesignerExtension = null;

		/// <summary>
		/// Implements the IVsSingleFileGenerator.DefaultExtension method. 
		/// Returns the extension of the generated file
		/// </summary>
		/// <param name="pbstrDefaultExtension">Out parameter, will hold the extension that is to be given to the output file name. The returned extension must include a leading period</param>
		/// <returns>S_OK if successful, E_FAIL if not</returns>
		int IVsSingleFileGenerator.DefaultExtension(out string pbstrDefaultExtension)
		{
			if (string.IsNullOrWhiteSpace(defaultDesignerExtension))
			{
				CodeDomProvider codeDom = GetCodeProvider();
				Trace.Assert(codeDom != null, "CodeDomProvider is NULL.");
				defaultDesignerExtension = codeDom.FileExtension;
				if (defaultDesignerExtension != null && defaultDesignerExtension.Length > 0)
				{
					defaultDesignerExtension = string.Concat("Access.designer.", defaultDesignerExtension.TrimStart(new char[] { '.' }));
				}
			}
			pbstrDefaultExtension = defaultDesignerExtension;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Implements the IVsSingleFileGenerator.Generate method.
		/// Executes the transformation and returns the newly generated output file, whenever a custom tool is loaded, or the input file is saved
		/// </summary>
		/// <param name="wszInputFilePath">The full path of the input file. May be a null reference (Nothing in Visual Basic) in future releases of Visual Studio, so generators should not rely on this value</param>
		/// <param name="bstrInputFileContents">The contents of the input file. This is either a UNICODE BSTR (if the input file is text) or a binary BSTR (if the input file is binary). If the input file is a text file, the project system automatically converts the BSTR to UNICODE</param>
		/// <param name="wszDefaultNamespace">This parameter is meaningful only for custom tools that generate code. It represents the namespace into which the generated code will be placed. If the parameter is not a null reference (Nothing in Visual Basic) and not empty, the custom tool can use the following syntax to enclose the generated code</param>
		/// <param name="rgbOutputFileContents">[out] Returns an array of bytes to be written to the generated file. You must include UNICODE or UTF-8 signature bytes in the returned byte array, as this is a raw stream. The memory for rgbOutputFileContents must be allocated using the .NET Framework call, System.Runtime.InteropServices.AllocCoTaskMem, or the equivalent Win32 system call, CoTaskMemAlloc. The project system is responsible for freeing this memory</param>
		/// <param name="pcbOutput">[out] Returns the count of bytes in the rgbOutputFileContent array</param>
		/// <param name="pGenerateProgress">A reference to the IVsGeneratorProgress interface through which the generator can report its progress to the project system</param>
		/// <returns>If the method succeeds, it returns S_OK. If it fails, it returns E_FAIL</returns>
		int IVsSingleFileGenerator.Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
			IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
		{
			try
			{
				//AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((sender, args) =>
				//{
				//	Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				//	Assembly ass = assemblies.FirstOrDefault(m => m.GetName().Name == args.Name || m.GetName().FullName == args.Name);
				//	if (ass != null) { return ass; }
				//	int index = args.Name.IndexOf(',');
				//	string fileName = args.Name.Substring(0, index);
				//	string assFolder = Path.GetDirectoryName(typeof(PersistentPackage).Assembly.Location);
				//	string filePath = string.Concat(assFolder, "/", fileName, ".dll");
				//	if (File.Exists(filePath)) { return Assembly.LoadFile(filePath); }
				//	return ass;
				//});
				pcbOutput = 0;
				if (bstrInputFileContents == null) { throw new ArgumentNullException(bstrInputFileContents); }
				ServiceProvider serviceProvider = GetServiceProvider();
				ProjectItem item = serviceProvider.GetService(typeof(ProjectItem)) as ProjectItem;
				Assumes.Present(item);
				pGenerateProgress.Progress(0, 100);
				PersistentConfiguration persistent = new PersistentConfiguration();
				using (StringReader reader = new StringReader(bstrInputFileContents))
				{
					persistent.ReadXml(reader);
					if (persistent.DataCommands.Count == 0 && persistent.DataEntities.Count == 0)
					{ return VSConstants.S_FALSE; }
				}
				pGenerateProgress.Progress(10, 100);
				CodeGeneratorOptions options = new CodeGeneratorOptions
				{
					BlankLinesBetweenMembers = true,
					BracingStyle = "C"// "C";
				};
				CodeDomProvider provider = GetCodeProvider();
				pGenerateProgress.Progress(20, 100);
				GenerateDataEntityCode(wszInputFilePath, item, persistent, options, provider);
				pGenerateProgress.Progress(40, 100);
				byte[] bytes = GenerateAccessCode(wszInputFilePath, item, persistent, options, provider);
				pGenerateProgress.Progress(60, 100);
				GenerateContextCode(wszInputFilePath, item, persistent, options, provider);
				pGenerateProgress.Progress(80, 100);
				GenerateConfigurationCode(wszInputFilePath, item, persistent);
				pGenerateProgress.Progress(100, 100);
				item.ContainingProject.Save();
				//EnvDTE.DTE dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
				//EnvDTE.Window window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
				//EnvDTE.OutputWindow outputWindow = window as EnvDTE.OutputWindow;

				if (bytes == null)
				{
					// This signals that GenerateCode() has failed. Tasklist items have been put up in GenerateCode()
					rgbOutputFileContents = null;
					pcbOutput = 0;

					// Return E_FAIL to inform Visual Studio that the generator has failed (so that no file gets generated)
					return VSConstants.E_FAIL;
				}
				else
				{
					// The contract between IVsSingleFileGenerator implementors and consumers is that 
					// any output returned from IVsSingleFileGenerator.Generate() is returned through  
					// memory allocated via CoTaskMemAlloc(). Therefore, we have to convert the 
					// byte[] array returned from GenerateCode() into an unmanaged blob.  

					int outputLength = bytes.Length;
					rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
					Marshal.Copy(bytes, 0, rgbOutputFileContents[0], outputLength);
					pcbOutput = (uint)outputLength;
					return VSConstants.S_OK;
				}
			}
			catch (Exception ex)
			{
				pcbOutput = 0;
				ThreadHelper.ThrowIfNotOnUIThread();
				pGenerateProgress.GeneratorError(0, (uint)ex.HResult, ex.Message, 0, 0);
				return VSConstants.E_FAIL;
			}
		}

		/// <summary>
		/// Demand-creates a ServiceProvider
		/// </summary>
		internal ServiceProvider GetServiceProvider()
		{
			if (serviceProvider != null) { return serviceProvider; }
			serviceProvider = new ServiceProvider(site as VSOLE.IServiceProvider);
			return serviceProvider;
		}

		private object site = null;
		private CodeDomProvider codeDomProvider = null;
		private ServiceProvider serviceProvider = null;
		/// <summary>
		/// Returns a CodeDomProvider object for the language of the project containing
		/// the project item the generator was called on
		/// </summary>
		/// <returns>A CodeDomProvider object</returns>
		private CodeDomProvider GetCodeProvider()
		{
			if (codeDomProvider == null)
			{
				ServiceProvider serviceProvider = GetServiceProvider();
				if (serviceProvider.GetService(typeof(SVSMDCodeDomProvider)) is IVSMDCodeDomProvider provider)
				{
					codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
				}
				else { codeDomProvider = CodeDomProvider.CreateProvider("C#"); }
			}
			return codeDomProvider;
		}

		private int GetTargetFramework(Project project)
		{
			foreach (EnvDTE.Property pty in project.Properties)
			{
				if (pty.Name == "TargetFramework") { return Convert.ToInt32(pty.Value); }
			}
			return 262144;
		}

		private void GenerateConfigurationCode(string wszInputFilePath, ProjectItem item, PersistentConfiguration persistent)
		{
			FileInfo file = new FileInfo(wszInputFilePath);
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t"
			};
			//ProjectItem sqlItem = null, oracleItem = null;
			foreach (ConnectionTypeEnum connectionType in persistent.SupportDatabases)
			{
				string extension = ConnectionTypeExtension.GetExtension(connectionType);
				string filePath = string.Concat(file.DirectoryName, @"\", persistent.TableName, ".", extension);
				using (XmlWriter writer = XmlWriter.Create(filePath, settings))
				{
					persistent.GenerateConfiguration(writer, connectionType);
				}
				ProjectItem projectItem = null;
				foreach (ProjectItem subItem in item.ProjectItems)
				{
					Property property = subItem.Properties.Item("FullPath");
					if (property != null && Convert.ToString(property.Value) == filePath)
					{
						projectItem = subItem; break;
					}
				}
				if (projectItem == null)
				{
					projectItem = item.ProjectItems.AddFromFile(filePath);
					GetGeneratorInfo(projectItem, item.Name);
					if (persistent.ResxMode == ResxModeEnum.Resource)
					{
						EnvDTE.Property property = projectItem.Properties.Item("ItemType");
						property.Value = "Resource";
					}
					else if (persistent.ResxMode == ResxModeEnum.AssemlyResource)
					{
						EnvDTE.Property property = projectItem.Properties.Item("BuildAction");
						property.Value = prjBuildAction.prjBuildActionEmbeddedResource;
					}
				}
				else
				{
					if (persistent.ResxMode == ResxModeEnum.Resource)
					{
						EnvDTE.Property property = projectItem.Properties.Item("ItemType");
						if (!object.Equals(property.Value, "Resource")) { property.Value = "Resource"; }
					}
					else if (persistent.ResxMode == ResxModeEnum.AssemlyResource)
					{
						EnvDTE.Property property = projectItem.Properties.Item("BuildAction");
						property.Value = prjBuildAction.prjBuildActionEmbeddedResource;
					}
				}
			}
		}

		private void GenerateContextCode(string wszInputFilePath, ProjectItem item, PersistentConfiguration persistent,
			CodeGeneratorOptions options, CodeDomProvider provider)
		{
			FileInfo file = new FileInfo(wszInputFilePath);
			string acFileName = persistent.ContextName;
			string contextFullName = string.Concat(file.DirectoryName, @"\", acFileName, ".", provider.FileExtension);
			string designerFullName = string.Concat(file.DirectoryName, @"\", acFileName, ".designer.", provider.FileExtension);
			ProjectItem contextItem = null, designerItem = null;
			if (!persistent.GenerateContext)
			{
				foreach (ProjectItem subItem in item.ProjectItems)
				{
					EnvDTE.Property property = subItem.Properties.Item("FullPath");
					if (property != null && Convert.ToString(property.Value) == contextFullName) { contextItem = subItem; continue; }
					else if (property != null && Convert.ToString(property.Value) == designerFullName) { designerItem = subItem; continue; }
				}
				if (designerItem != null) { designerItem.Remove(); }
				if (contextItem != null) { contextItem.Remove(); }
				return;
			}
			CD.CodeCompileUnit contextComplieUnit = new CD.CodeCompileUnit();
			CD.CodeCompileUnit designerComplieUnit = new CD.CodeCompileUnit();
			persistent.WriteContextDesignerCode(designerComplieUnit, provider);
			persistent.WriteContextCode(contextComplieUnit, provider);
			using (StreamWriter writer = new StreamWriter(designerFullName, false, Encoding.Unicode))
			{
				provider.GenerateCodeFromCompileUnit(designerComplieUnit, writer, options);
			}
			foreach (ProjectItem subItem in item.ProjectItems)
			{
				EnvDTE.Property property = subItem.Properties.Item("FullPath");
				if (property != null && Convert.ToString(property.Value) == contextFullName) { contextItem = subItem; continue; }
				else if (property != null && Convert.ToString(property.Value) == designerFullName) { designerItem = subItem; continue; }
			}
			if (designerItem == null)
			{
				designerItem = item.ProjectItems.AddFromFile(designerFullName);
				if (designerItem == null) { return; }
				GetGeneratorInfo(designerItem, item.Name);
			}
			if (contextItem == null)
			{
				if (!File.Exists(contextFullName))
				{
					using (StreamWriter writer = new StreamWriter(contextFullName, false, Encoding.Unicode))
					{
						provider.GenerateCodeFromCompileUnit(contextComplieUnit, writer, options);
					}
				}
				contextItem = item.ProjectItems.AddFromFile(contextFullName);
				if (contextItem == null) { return; }
				GetGeneratorInfo(contextItem, item.Name);
			}
		}

		private byte[] GenerateAccessCode(string wszInputFilePath, ProjectItem item, PersistentConfiguration persistent,
			CodeGeneratorOptions options, CodeDomProvider provider)
		{
			Project project = item.ContainingProject;
			FileInfo file = new FileInfo(wszInputFilePath);
			string acFileName = persistent.AccessName;
			string acFullName = string.Concat(file.DirectoryName, @"\", acFileName, ".", provider.FileExtension);

			string moduleName = null;
			if (persistent.ResxMode == ResxModeEnum.Resource)
			{
				FileInfo projectFile = new FileInfo(project.FullName);
				if (file.DirectoryName != projectFile.DirectoryName)
					moduleName = file.DirectoryName.Replace(projectFile.DirectoryName + "\\", "").Replace("\\", "/");
			}
			else if (persistent.ResxMode == ResxModeEnum.AssemlyResource)
			{
				foreach (Property prop in item.Properties)
				{
					if (prop.Name == "DefaultNamespace") { moduleName = (string)prop.Value; }
				}
				EnvDTE.ProjectItems items = (item.Collection as EnvDTE.ProjectItems);
				if (items.Parent is ProjectItem folder)
				{
					foreach (Property prop in folder.Properties)
					{
						if (prop.Name == "DefaultNamespace") { moduleName = (string)prop.Value; }
					}
				}
				else if (items.Parent is Project proj)
				{
					foreach (Property prop in proj.Properties)
					{
						if (prop.Name == "DefaultNamespace") { moduleName = (string)prop.Value; }
					}
				}
			}

			CD.CodeCompileUnit accessComplieUnit = new CD.CodeCompileUnit();
			CD.CodeCompileUnit designerComplieUnit = new CD.CodeCompileUnit();
			int targetFramework = GetTargetFramework(project);
			persistent.WriteAccessDesignerCode(designerComplieUnit, moduleName, targetFramework);
			persistent.WriteAccessCode(accessComplieUnit);
			byte[] bytes = GenerateCodeFromCompileUnit(designerComplieUnit, options);

			ProjectItem accessItem = null;
			foreach (ProjectItem subItem in item.ProjectItems)
			{
				EnvDTE.Property property = subItem.Properties.Item("FullPath");
				if (property != null && Convert.ToString(property.Value) == acFullName) { accessItem = subItem; continue; }
			}

			//ProjectItem accessItem = dte.Solution.FindProjectItem(acFullName);
			if (accessItem == null)
			{
				using (StreamWriter writer = new StreamWriter(acFullName, false, Encoding.Unicode))
				{
					provider.GenerateCodeFromCompileUnit(accessComplieUnit, writer, options);
				}
				accessItem = item.ProjectItems.AddFromFile(acFullName);
				if (accessItem != null)
				{
					GetGeneratorInfo(accessItem, item.Name);
				}
			}
			return bytes;
		}

		private void GetGeneratorInfo(ProjectItem item, string dependentUpon)
		{
			foreach (EnvDTE.Property property in item.Properties)
			{
				if (property.Name == "AutoGen") { property.Value = "True"; }
				else if (property.Name == "DependentUpon")
				{
					property.Value = dependentUpon;
				}
			}
		}

		private void GenerateDataEntityCode(string wszInputFilePath, ProjectItem item, PersistentConfiguration persistent,
			CodeGeneratorOptions options, CodeDomProvider provider)
		{
			FileInfo file = new FileInfo(wszInputFilePath);
			string entityFileName = file.Name.Replace(file.Extension, "");
			//string entityFileName = persistent.EntityName;
			string entityFullName = string.Concat(file.DirectoryName, @"\", entityFileName, ".", provider.FileExtension);
			string designerFullName = string.Concat(file.DirectoryName, @"\", entityFileName, ".designer.", provider.FileExtension);
			EnvDTE.ProjectItems items = item.ProjectItems;
			EnvDTE.DTE dte = item.DTE;
			if (persistent.Project.NotEmpty)
			{
				ProjectItem entityDesignerFile = dte.Solution.FindProjectItem(designerFullName);
				if (entityDesignerFile != null) { entityDesignerFile.Remove(); }
				ProjectItem entityFile = dte.Solution.FindProjectItem(entityFullName);
				if (entityFile != null) { entityFile.Remove(); }

				Guid guid = persistent.ProjectGuid;
				IVsSolution vsSolution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
				vsSolution.GetProjectOfGuid(ref guid, out IVsHierarchy hierarchy);
				uint itemId = (uint)VSConstants.VSITEMID.Root;
				if (hierarchy != null && hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object outProject) >= 0)
				{
					EnvDTE.Project entityProject = (EnvDTE.Project)outProject;
					FileInfo entityFileInfo = new FileInfo(entityProject.FullName);
					string entityFolder = string.Concat(entityFileInfo.DirectoryName, "\\", persistent.EntityFolder);
					items = entityProject.ProjectItems;
					if (!string.IsNullOrWhiteSpace(persistent.EntityFolder))
					{
						string[] folders = persistent.EntityFolder.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
						EnvDTE.ProjectItem item1 = FindFolderItem(entityProject.ProjectItems, folders, 0);
						if (item1 != null) { items = item1.ProjectItems; }
					}
					entityFullName = string.Concat(entityFolder, entityFileName, ".", provider.FileExtension);
					designerFullName = string.Concat(entityFolder, entityFileName, ".designer.", provider.FileExtension);
					CD.CodeCompileUnit designerComplieUnit = new CD.CodeCompileUnit();
					persistent.WriteEntityDesignerCode(designerComplieUnit);
					using (StreamWriter writer = new StreamWriter(designerFullName, false, Encoding.Unicode))
					{
						provider.GenerateCodeFromCompileUnit(designerComplieUnit, writer, options);
					}
					ProjectItem entityItem = dte.Solution.FindProjectItem(entityFullName);
					if (entityItem == null)
					{
						CD.CodeCompileUnit entityComplieUnit = new CD.CodeCompileUnit();
						persistent.WriteEntityCode(entityComplieUnit);
						using (StreamWriter writer = new StreamWriter(entityFullName, false, Encoding.Unicode))
						{
							provider.GenerateCodeFromCompileUnit(entityComplieUnit, writer, options);
						}
						entityItem = items.AddFromFile(entityFullName);
						if (entityItem != null)
						{
							GetGeneratorInfo(entityItem, item.Name);
							ProjectItem itemDesign = items.AddFromFile(designerFullName);
							if (itemDesign != null) { GetGeneratorInfo(itemDesign, entityItem.Name); }
						}
						entityProject.Save();
					}
				}
			}
			else
			{
				CD.CodeCompileUnit designerComplieUnit = new CD.CodeCompileUnit();
				persistent.WriteEntityDesignerCode(designerComplieUnit);
				using (StreamWriter writer = new StreamWriter(designerFullName, false, Encoding.Unicode))
				{
					provider.GenerateCodeFromCompileUnit(designerComplieUnit, writer, options);
				}
				ProjectItem entityItem = dte.Solution.FindProjectItem(entityFullName);
				if (entityItem == null)
				{
					CD.CodeCompileUnit entityComplieUnit = new CD.CodeCompileUnit();
					persistent.WriteEntityCode(entityComplieUnit);
					using (StreamWriter writer = new StreamWriter(entityFullName, false, Encoding.Unicode))
					{
						provider.GenerateCodeFromCompileUnit(entityComplieUnit, writer, options);
					}
					entityItem = items.AddFromFile(entityFullName);
					if (entityItem != null)
					{
						GetGeneratorInfo(entityItem, item.Name);
						var itemDesign = items.AddFromFile(designerFullName);
						if (itemDesign != null) { GetGeneratorInfo(itemDesign, item.Name); }
					}
				}
			}
		}

		/// <summary>
		/// 生成设计时代码
		/// </summary>
		/// <param name="compileUnit"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		private byte[] GenerateCodeFromCompileUnit(CD.CodeCompileUnit compileUnit, CodeGeneratorOptions options)
		{
			StringBuilder builder = new StringBuilder(5000);
			using (StringWriter writer = new StringWriter(builder))
			{
				CodeDomProvider provider = GetCodeProvider();
				provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
				writer.Flush();
				//Get the Encoding used by the writer. We're getting the WindowsCodePage encoding, 
				//which may not work with all languages
				Encoding enc = Encoding.GetEncoding(writer.Encoding.WindowsCodePage);
				//Get the preamble (byte-order mark) for our encoding
				byte[] preamble = enc.GetPreamble();
				int preambleLength = preamble.Length;

				//Convert the writer contents to a byte array
				byte[] body = enc.GetBytes(writer.ToString());

				//Prepend the preamble to body (store result in resized preamble array)
				Array.Resize<byte>(ref preamble, preambleLength + body.Length);
				Array.Copy(body, 0, preamble, preambleLength, body.Length);
				//Return the combined byte array
				return preamble;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		/// <param name="folderArray"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private ProjectItem FindFolderItem(ProjectItems items, string[] folderArray, int index)
		{
			if (folderArray.Length > index)
			{
				foreach (ProjectItem item in items)
				{
					if (item.Name == folderArray[index] && (folderArray.Length - 1) == index)
					{ return item; }
					else if (item.Name == folderArray[index] && folderArray.Length - 1 > index)
					{
						return FindFolderItem(item.ProjectItems, folderArray, index + 1);
					}
				}
			}
			return null;
		}
	}
}
