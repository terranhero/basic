using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using Basic.Messages;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj80;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;

namespace Basic.Localizations
{
	/// <summary>
	/// 资源代码生成器
	/// </summary>
	[System.Runtime.InteropServices.Guid(Consts.guidGeneratorString)]
	[CodeGeneratorRegistration(typeof(ResourceGenerator), "C# Generator", vsContextGuids.vsContextGuidVCSProject)]
	[CodeGeneratorRegistration(typeof(ResourceGenerator), "VB Generator", vsContextGuids.vsContextGuidVBProject)]
	[ProvideObject(typeof(ResourceGenerator)), ComVisible(true)]
	public sealed class ResourceGenerator : IVsSingleFileGenerator, IObjectWithSite, IDisposable
	{
		private object site = null;
		private CodeDomProvider codeDomProvider = null;
		private ServiceProvider serviceProvider = null;
		private string defaultDesignerExtension = null;
		private IVsGeneratorProgress vsGeneratorProgress;
		//private string inputFilePath;
		/// <summary>
		/// File-path for the input file
		/// </summary>
		//private string defaultNamespace;

		public int DefaultExtension(out string pbstrDefaultExtension)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			try
			{
				if (string.IsNullOrWhiteSpace(defaultDesignerExtension))
				{
					CodeDomProvider codeDom = GetCodeProvider();
					Debug.Assert(codeDom != null, "CodeDomProvider is NULL.");
					defaultDesignerExtension = codeDom.FileExtension;
					if (defaultDesignerExtension != null && defaultDesignerExtension.Length > 0)
					{
						defaultDesignerExtension = string.Concat(".designer.", defaultDesignerExtension.TrimStart(new char[] { '.' }));
					}
				}
				pbstrDefaultExtension = defaultDesignerExtension;
				return VSConstants.S_OK;
			}
			catch (Exception e)
			{
				GeneratorError(4, Strings.GetDefaultExtensionFailed, 1, 1);
				GeneratorError(4, e.ToString(), 1, 1);
				pbstrDefaultExtension = string.Empty;
				return VSConstants.E_FAIL;
			}
		}

		/// <summary>
		/// Returns a CodeDomProvider object for the language of the project containing
		/// the project item the generator was called on
		/// </summary>
		/// <returns>A CodeDomProvider object</returns>
		private CodeDomProvider GetCodeProvider()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (codeDomProvider == null)
			{
				//Query for IVSMDCodeDomProvider/SVSMDCodeDomProvider for this project type
				if (GetService(typeof(SVSMDCodeDomProvider)) is IVSMDCodeDomProvider provider)
				{
					codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
				}
				else
				{
					//In the case where no language specific CodeDom is available, fall back to C#
					codeDomProvider = CodeDomProvider.CreateProvider("C#");
				}
			}
			return codeDomProvider;
		}

		/// <summary>
		/// Method to get a service by its Type
		/// </summary>
		/// <param name="serviceType">Type of service to retrieve</param>
		/// <returns>An object that implements the requested service</returns>
		private object GetService(Type serviceType)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return SiteServiceProvider.GetService(serviceType);
		}

		/// <summary>
		/// Demand-creates a ServiceProvider
		/// </summary>
		internal ServiceProvider SiteServiceProvider
		{
			get
			{
				ThreadHelper.ThrowIfNotOnUIThread();
				if (serviceProvider == null)
				{
					serviceProvider = new ServiceProvider(site as VSOLE.IServiceProvider);
					Debug.Assert(serviceProvider != null, "Unable to get ServiceProvider from site object.");
				}
				return serviceProvider;
			}
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="code">表示需要写入代码的命名空间</param>
		private void WriteEntityDesignerCode(CodeCompileUnit codeComplieUnit, string wszInputFilePath)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			codeComplieUnit.UserData.Add("AllowLateBound", false);
			// Option Explicit On (controls whether variable declarations are required)
			codeComplieUnit.UserData.Add("RequireVariableDeclaration", true);
			FileInfo fileInfo = new FileInfo(wszInputFilePath);
			string className = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

			EnvDTE.ProjectItem projectItem = GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			EnvDTE.ProjectItem folderItem = projectItem.Collection.Parent as EnvDTE.ProjectItem;
			EnvDTE.Property property = folderItem.Properties.Item("DefaultNamespace");
			string outputNamespace = Convert.ToString(property.Value);
			string folderNamespace = outputNamespace;
			EnvDTE.Property itemNamespace = projectItem.Properties.Item("CustomToolNamespace");
			if (itemNamespace != null && Convert.ToString(itemNamespace.Value) != "")
				outputNamespace = Convert.ToString(itemNamespace.Value);
			CodeNamespace codeNamespace = new CodeNamespace(outputNamespace);
			string resourceFullName = string.Concat(folderNamespace, ".", fileInfo.Name.Replace(fileInfo.Extension, string.Empty));
			codeComplieUnit.Namespaces.Add(codeNamespace);
			CodeTypeDeclaration converterClass = new CodeTypeDeclaration(className)
			{
				IsClass = true,
				TypeAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed
			};
			converterClass.Attributes |= MemberAttributes.Public;
			codeNamespace.Types.Add(converterClass);
			converterClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			converterClass.Comments.Add(new CodeCommentStatement(className, true));
			converterClass.Comments.Add(new CodeCommentStatement("</summary>", true));
			converterClass.BaseTypes.Add(new CodeTypeReference(typeof(ResourceManager), CodeTypeReferenceOptions.GlobalReference));
			converterClass.BaseTypes.Add(new CodeTypeReference(typeof(IMessageConverter), CodeTypeReferenceOptions.GlobalReference));
			CodeBaseReferenceExpression baseReference = new CodeBaseReferenceExpression();

			CodeTypeReference toolboxItemTypeReference = new CodeTypeReference(typeof(ToolboxItemAttribute),
				CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration toolboxItemAttribute = new CodeAttributeDeclaration(toolboxItemTypeReference);
			toolboxItemAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(false)));
			converterClass.CustomAttributes.Add(toolboxItemAttribute);

			CodeConstructor constructor = new CodeConstructor();
			constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", className), true));
			constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			constructor.BaseConstructorArgs.Add(new CodePrimitiveExpression(resourceFullName));
			CodeTypeOfExpression typeofExpression = new CodeTypeOfExpression(className);
			constructor.BaseConstructorArgs.Add(new CodeFieldReferenceExpression(typeofExpression, "Assembly"));

			CodeAssignStatement assignIgnoreCase = new CodeAssignStatement
			{
				Left = new CodePropertyReferenceExpression(baseReference, "IgnoreCase"),
				Right = new CodePrimitiveExpression(true)
			};
			constructor.Statements.Add(assignIgnoreCase);
			////base.BaseNameField = "Basic.Message.Message";
			//CodeAssignStatement nameFieldAssign = new CodeAssignStatement();
			//nameFieldAssign.Left = new CodeFieldReferenceExpression(baseReference, "BaseNameField");
			//nameFieldAssign.Right = new CodePrimitiveExpression(resourceFullName);
			//constructor.Statements.Add(nameFieldAssign);

			////base.MainAssembly = typeof(MessageConverter).Assembly;
			//CodeAssignStatement assemblyAssign = new CodeAssignStatement();
			//assemblyAssign.Left = new CodeFieldReferenceExpression(baseReference, "MainAssembly");
			//CodeTypeOfExpression typeofExpression = new CodeTypeOfExpression(className);
			//assemblyAssign.Right = new CodeFieldReferenceExpression(typeofExpression, "Assembly");
			//constructor.Statements.Add(assemblyAssign);

			converterClass.Members.Add(constructor);

			CodeMemberProperty nameProperty = new CodeMemberProperty
			{
				Name = "Name",
				HasGet = true,
				Type = new CodeTypeReference(typeof(string)),
				Attributes = MemberAttributes.Public | MemberAttributes.Final
			};
			nameProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
			nameProperty.Comments.Add(new CodeCommentStatement("获取当前资源转换器的名称。", true));
			nameProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

			CodeFieldReferenceExpression typeNameExpression = new CodeFieldReferenceExpression(typeofExpression, "Name");
			CodeMethodReturnStatement propertyReturn = new CodeMethodReturnStatement(typeNameExpression);
			nameProperty.GetStatements.Add(propertyReturn);
			converterClass.Members.Add(nameProperty);

			CodeMethodInvokeExpression baseGetStringInvoke = new CodeMethodInvokeExpression(baseReference, "GetString");

			CodeMemberMethod getStringMethod = new CodeMemberMethod();
			converterClass.Members.Add(getStringMethod);
			getStringMethod.Name = "GetString";
			getStringMethod.ReturnType = new CodeTypeReference(typeof(string));
			getStringMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			getStringMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
			baseGetStringInvoke.Parameters.Add(new CodeVariableReferenceExpression("name"));
			getStringMethod.Parameters.Add(new CodeParameterDeclarationExpression("params object[] ", "args"));

			CodeVariableDeclarationStatement sourceDeclaration = new CodeVariableDeclarationStatement(typeof(string), "source")
			{
				InitExpression = baseGetStringInvoke
			};
			getStringMethod.Statements.Add(sourceDeclaration);
			CodeConditionStatement sourceCondition = new CodeConditionStatement();
			CodeMethodReferenceExpression stringIsNull = new CodeMethodReferenceExpression
			{
				TargetObject = new CodeTypeReferenceExpression(typeof(string)),
				MethodName = "IsNullOrEmpty"
			};
			sourceCondition.Condition = new CodeMethodInvokeExpression(stringIsNull, new CodeVariableReferenceExpression("source"));
			sourceCondition.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("name")));
			getStringMethod.Statements.Add(sourceCondition);
			CodeConditionStatement ifCondition = new CodeConditionStatement();
			CodeBinaryOperatorExpression argsIsNull = new CodeBinaryOperatorExpression
			{
				Left = new CodeVariableReferenceExpression("args"),
				Operator = CodeBinaryOperatorType.ValueEquality,
				Right = new CodePrimitiveExpression(null)
			};
			CodeBinaryOperatorExpression argsLength = new CodeBinaryOperatorExpression
			{
				Left = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("args"), "Length"),
				Operator = CodeBinaryOperatorType.ValueEquality,
				Right = new CodePrimitiveExpression(0)
			};
			ifCondition.Condition = new CodeBinaryOperatorExpression(argsIsNull, CodeBinaryOperatorType.BooleanOr, argsLength);
			ifCondition.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("source")));
			getStringMethod.Statements.Add(ifCondition);

			CodeTypeReferenceExpression type = new CodeTypeReferenceExpression(typeof(string));
			CodeMethodInvokeExpression formatResult = new CodeMethodInvokeExpression(type, "Format");
			formatResult.Parameters.Add(new CodeVariableReferenceExpression("source"));
			formatResult.Parameters.Add(new CodeVariableReferenceExpression("args"));
			getStringMethod.Statements.Add(new CodeMethodReturnStatement(formatResult));
			getStringMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			getStringMethod.Comments.Add(new CodeCommentStatement("返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值。", true));
			getStringMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			getStringMethod.Comments.Add(new CodeCommentStatement("<param name=\"name\">资源名称</param>", true));
			getStringMethod.Comments.Add(new CodeCommentStatement("<param name=\"args\">一个对象数组，其中包含零个或多个要设置格式的对象。</param>", true));
			getStringMethod.Comments.Add(new CodeCommentStatement("<returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>", true));

			CodeMemberMethod getStringMethodOver = new CodeMemberMethod
			{
				Name = "GetString",
				ReturnType = new CodeTypeReference(typeof(string)),
				Attributes = MemberAttributes.Public | MemberAttributes.Final
			};
			getStringMethodOver.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
			getStringMethodOver.Parameters.Add(new CodeParameterDeclarationExpression(typeof(CultureInfo), "culture"));
			getStringMethodOver.Parameters.Add(new CodeParameterDeclarationExpression("params object[] ", "args"));
			converterClass.Members.Add(getStringMethodOver);
			CodeMethodInvokeExpression getStringInvoke = new CodeMethodInvokeExpression(baseReference, "GetString");
			getStringInvoke.Parameters.Add(new CodeVariableReferenceExpression("name"));
			getStringInvoke.Parameters.Add(new CodeVariableReferenceExpression("culture"));
			CodeVariableDeclarationStatement sourceDeclarationOver = new CodeVariableDeclarationStatement(typeof(string), "source")
			{
				InitExpression = getStringInvoke
			};
			getStringMethodOver.Statements.Add(sourceDeclarationOver);
			getStringMethodOver.Statements.Add(sourceCondition);
			getStringMethodOver.Statements.Add(ifCondition);
			getStringMethodOver.Statements.Add(new CodeMethodReturnStatement(formatResult));
			getStringMethodOver.Comments.Add(new CodeCommentStatement("<summary>", true));
			getStringMethodOver.Comments.Add(new CodeCommentStatement("返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值。", true));
			getStringMethodOver.Comments.Add(new CodeCommentStatement("</summary>", true));
			getStringMethodOver.Comments.Add(new CodeCommentStatement("<param name=\"name\">资源名称</param>", true));
			StringBuilder cultureBuilder = new StringBuilder(300);
			cultureBuilder.AppendLine("<param name=\"culture\">");
			cultureBuilder.AppendLine("System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的");
			cultureBuilder.AppendLine("System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的");
			cultureBuilder.AppendLine("System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。");
			cultureBuilder.AppendLine("</param>");
			getStringMethodOver.Comments.Add(new CodeCommentStatement(cultureBuilder.ToString(), true));
			getStringMethodOver.Comments.Add(new CodeCommentStatement("<param name=\"args\">一个对象数组，其中包含零个或多个要设置格式的对象。</param>", true));
			getStringMethodOver.Comments.Add(new CodeCommentStatement("<returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>", true));
		}

		public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (bstrInputFileContents == null)
			{
				throw new ArgumentNullException(bstrInputFileContents);
			}
			//inputFilePath = wszInputFilePath;
			//defaultNamespace = wszDefaultNamespace;
			vsGeneratorProgress = pGenerateProgress;
			CodeCompileUnit codeComplieUnit = new CodeCompileUnit();
			WriteEntityDesignerCode(codeComplieUnit, wszInputFilePath);
			CodeGeneratorOptions options = new CodeGeneratorOptions
			{
				BlankLinesBetweenMembers = true,
				BracingStyle = "C"// "C";
			};
			StringBuilder builder = new StringBuilder(5000);
			using (StringWriter writer = new StringWriter(builder))
			{
				CodeDomProvider provider = GetCodeProvider();
				provider.GenerateCodeFromCompileUnit(codeComplieUnit, writer, options);
				writer.Flush();
				//Get the Encoding used by the writer. We're getting the WindowsCodePage encoding, 
				//which may not work with all languages
				Encoding enc = Encoding.GetEncoding(writer.Encoding.WindowsCodePage);
				//Get the preamble (byte-order mark) for our encoding
				byte[] bytes = enc.GetPreamble();
				int preambleLength = bytes.Length;

				//Convert the writer contents to a byte array
				byte[] body = enc.GetBytes(writer.ToString());

				//Prepend the preamble to body (store result in resized preamble array)
				Array.Resize<byte>(ref bytes, preambleLength + body.Length);
				Array.Copy(body, 0, bytes, preambleLength, body.Length);
				//Return the combined byte array
				int outputLength = bytes.Length;
				rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
				Marshal.Copy(bytes, 0, rgbOutputFileContents[0], outputLength);
				pcbOutput = (uint)outputLength;
				return VSConstants.S_OK;
			}
		}

		/// <summary>
		/// GetSite method of IOleObjectWithSite
		/// </summary>
		/// <param name="riid">interface to get</param>
		/// <param name="ppvSite">IntPtr in which to stuff return value</param>
		public void GetSite(ref Guid riid, out IntPtr ppvSite)
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
		public void SetSite(object pUnkSite)
		{
			site = pUnkSite;
			codeDomProvider = null;
			serviceProvider = null;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (codeDomProvider != null) { codeDomProvider.Dispose(); }
			if (serviceProvider != null) { serviceProvider.Dispose(); }
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Method that will communicate an error via the shell callback mechanism
		/// </summary>
		/// <param name="level">Level or severity</param>
		/// <param name="message">Text displayed to the user</param>
		/// <param name="line">Line number of error</param>
		/// <param name="column">Column number of error</param>
		private void GeneratorError(int level, string message, int line, int column)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (vsGeneratorProgress != null)
			{
				vsGeneratorProgress.GeneratorError(0, (uint)level, message, (uint)line, (uint)column);
			}
		}

		///// <summary>
		///// Method that will communicate a warning via the shell callback mechanism
		///// </summary>
		///// <param name="level">Level or severity</param>
		///// <param name="message">Text displayed to the user</param>
		///// <param name="line">Line number of warning</param>
		///// <param name="column">Column number of warning</param>
		//private void GeneratorWarning(uint level, string message, uint line, uint column)
		//{
		//	ThreadHelper.ThrowIfNotOnUIThread();
		//	if (vsGeneratorProgress != null)
		//	{
		//		vsGeneratorProgress.GeneratorError(1, level, message, line, column);
		//	}
		//}
	}
}
