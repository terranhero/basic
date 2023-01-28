using EnvDTE;

namespace Basic.Configuration
{
	/// <summary>
	/// 扩展 VS 代码模型方法
	/// </summary>
	internal static class CodeDomExtensions
	{
		/// <summary>
		/// 在文件代码模型中查找指定的命名空间
		/// </summary>
		/// <param name="codeElements">可查找的代码元素集合。</param>
		/// <param name="name">需要查找的命名空间名称</param>
		/// <returns>如果类存在则返回类的代码模型。</returns>
		internal static CodeNamespace FindNamespace(this CodeElements codeElements, string name)
		{
			foreach (CodeElement element in codeElements)
			{
				if (element.Kind == vsCMElement.vsCMElementNamespace && element.Name == name)
				{
					return element as CodeNamespace;
				}
			}
			return null;
		}


		/// <summary>
		/// 在文件代码模型中查找指定的命名空间
		/// </summary>
		/// <param name="codeElements">可查找的代码元素集合。</param>
		/// <returns>如果类存在则返回类的代码模型。</returns>
		internal static CodeNamespace FindNamespace(this CodeElements codeElements)
		{
			foreach (CodeElement element in codeElements)
			{
				if (element.Kind == vsCMElement.vsCMElementNamespace)
				{
					return element as CodeNamespace;
				}
			}
			return null;
		}


		/// <summary>
		/// 在文件代码模型中查找指定的类
		/// </summary>
		/// <param name="codeElements">可查找的代码元素集合。</param>
		/// <param name="className">类名称</param>
		/// <returns>如果类存在则返回类的代码模型。</returns>
		internal static CodeProperty FindCodeProperty(this CodeElements codeElements, string propertyName)
		{
			foreach (CodeElement element in codeElements)
			{
				if (element.Kind == vsCMElement.vsCMElementClass)
				{
					CodeClass codeClass = element as CodeClass;
					CodeProperty codeProperty = FindCodeProperty(codeClass.Members, propertyName);
					if (codeProperty != null) { return codeProperty; }
				}
				else if (element.Kind == vsCMElement.vsCMElementProperty && element.Name == propertyName)
				{
					return element as CodeProperty;
				}
			}
			return null;
		}


		/// <summary>
		/// 在文件代码模型中查找指定的类
		/// </summary>
		/// <param name="codeElements">可查找的代码元素集合。</param>
		/// <param name="className">类名称</param>
		/// <returns>如果类存在则返回类的代码模型。</returns>
		internal static EnvDTE80.CodeClass2 FindCodeClass(this CodeElements codeElements, string className)
		{
			foreach (CodeElement element in codeElements)
			{
				if (element.Kind == vsCMElement.vsCMElementClass && element.Name == className)
				{
					return element as EnvDTE80.CodeClass2;
				}
				else if (element.Kind == vsCMElement.vsCMElementClass)
				{
					CodeClass parentClass = element as CodeClass;
					EnvDTE80.CodeClass2 codeClass = FindCodeClass(parentClass.Members, className);
					if (codeClass != null) { return codeClass; }
				}
				else if (element.Kind == vsCMElement.vsCMElementNamespace)
				{
					CodeNamespace codeNs = element as CodeNamespace;
					EnvDTE80.CodeClass2 codeClass = FindCodeClass(codeNs.Members, className);
					if (codeClass != null) { return codeClass; }
				}
			}
			return null;
		}
	}
}
