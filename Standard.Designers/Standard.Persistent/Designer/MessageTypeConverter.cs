using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft;

namespace Basic.Designer
{
	/// <summary>
	/// 提供消息转换器选择
	/// </summary>
	public sealed class MessageTypeConverter : StringConverter
	{
		readonly StringCollection strings = new StringCollection();
		/// <summary>
		/// 使用指定的上下文返回此对象是否支持可以从列表中选取的标准值集。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <returns>如果应调用 System.ComponentModel.TypeConverter.GetStandardValues() 来查找对象支持的一组公共值，则为 true；否则，为 false。</returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

		/// <summary>
		/// 当与格式上下文一起提供时，返回此类型转换器设计用于的数据类型的标准值集合。
		/// </summary>
		/// <param name="context">提供格式上下文的 System.ComponentModel.ITypeDescriptorContext，可用来提取有关从中调用此转换器的环境的附加信息。此参数或其属性可以为 null。</param>
		/// <returns>包含标准有效值集的 System.ComponentModel.TypeConverter.StandardValuesCollection；如果数据类型不支持标准值集，则为 null。</returns>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (strings.Count > 0) { return new StandardValuesCollection(strings); }
			EnvDTE.DTE dteClass = (EnvDTE.DTE)context.GetService(typeof(EnvDTE.DTE));
			Assumes.Present(dteClass);
			EnvDTE.Solution solutionClass = dteClass.Solution;
			System.IO.FileInfo fileSolution = new System.IO.FileInfo(solutionClass.FullName);
			System.IO.FileInfo[] fileArray = fileSolution.Directory.GetFiles("*.localresx", System.IO.SearchOption.AllDirectories);
			foreach (System.IO.FileInfo fileLocalResx in fileArray)
			{
				strings.Add(fileLocalResx.Name.Replace(fileLocalResx.Extension, ""));
			}
			return new StandardValuesCollection(strings);
			//ThreadHelper.ThrowIfNotOnUIThread(); 

			//StringCollection strings = new StringCollection();
			//EnvDTE.DTE dteClass = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
			//EnvDTE.Project project = dteClass.ActiveDocument.ProjectItem.ContainingProject;
			//IVsSolution2 vsSolution = (IVsSolution2)context.GetService(typeof(SVsSolution));
			//Assumes.Present(vsSolution);
			//vsSolution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
			//vsSolution.GetGuidOfProject(hierarchy, out Guid projectGuid);
			//IVsHierarchy ivsh = VsShellUtilities.GetHierarchy(context, projectGuid);
			//DynamicTypeService typeService = (DynamicTypeService)context.GetService(typeof(DynamicTypeService));
			//Assumes.Present(typeService);
			//ITypeDiscoveryService discovery = typeService.GetTypeDiscoveryService(ivsh);
			//ICollection types = discovery.GetTypes(typeof(Basic.Messages.IMessageConverter), true);
			//foreach (Type type in types)
			//{
			//	if (typeof(IMessageConverter) != type && typeof(MessageConverter) != type) { strings.Add(type.Name); }
			//}
			//return new StandardValuesCollection(strings);
		}
	}
}
