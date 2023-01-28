using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Routing;

namespace Basic.MvcLibrary
{
	internal static class TypeHelper
	{
		public static RouteValueDictionary ObjectToDictionary(object value)
		{
			RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
			if (value != null)
			{
				PropertyHelper[] properties = PropertyHelper.GetProperties(value);
				foreach (PropertyHelper propertyHelper in properties)
				{
					routeValueDictionary.Add(propertyHelper.Name, propertyHelper.GetValue(value));
				}
			}

			return routeValueDictionary;
		}

		public static RouteValueDictionary ObjectToDictionaryUncached(object value)
		{
			RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
			if (value != null)
			{
				PropertyHelper[] properties = PropertyHelper.GetProperties(value);
				foreach (PropertyHelper propertyHelper in properties)
				{
					routeValueDictionary.Add(propertyHelper.Name, propertyHelper.GetValue(value));
				}
			}

			return routeValueDictionary;
		}

		public static void AddAnonymousObjectToDictionary(IDictionary<string, object> dictionary, object value)
		{
			foreach (KeyValuePair<string, object> item in ObjectToDictionary(value))
			{
				dictionary.Add(item);
			}
		}

		public static bool IsAnonymousType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), inherit: false) && type.IsGenericType && type.Name.Contains("AnonymousType") && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase)))
			{
				return (type.Attributes & TypeAttributes.NotPublic) == 0;
			}

			return false;
		}
	}
}
#if false // 反编译日志
缓存中的 58 项
------------------
解析: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
找到单个程序集: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
从以下位置加载: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll"
------------------
解析: "System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
找到单个程序集: "System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
从以下位置加载: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Web.dll"
------------------
解析: "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
找到单个程序集: "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
从以下位置加载: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Core.dll"
------------------
解析: "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
找到单个程序集: "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
从以下位置加载: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll"
------------------
解析: "System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
找到单个程序集: "System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
从以下位置加载: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Xml.Linq.dll"
------------------
解析: "System.Web.Razor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
找到单个程序集: "System.Web.Razor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
从以下位置加载: "D:\HRMS-V5.0\HRMS-V5.0\PD_04_Trunk Code\packages\Microsoft.AspNet.Razor.3.2.7\lib\net45\System.Web.Razor.dll"
------------------
解析: "System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
找到单个程序集: "System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
从以下位置加载: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.ComponentModel.DataAnnotations.dll"
------------------
解析: "System.Data.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
无法按名称“System.Data.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089”查找 
------------------
解析: "Microsoft.Web.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
找到单个程序集: "Microsoft.Web.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
从以下位置加载: "D:\HRMS-V5.0\HRMS-V5.0\PD_04_Trunk Code\packages\Microsoft.Web.Infrastructure.1.0.0.0\lib\net40\Microsoft.Web.Infrastructure.dll"
------------------
解析: "System.Web.WebPages.Deployment, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
找到单个程序集: "System.Web.WebPages.Deployment, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
从以下位置加载: "D:\HRMS-V5.0\HRMS-V5.0\PD_04_Trunk Code\packages\Microsoft.AspNet.WebPages.3.2.7\lib\net45\System.Web.WebPages.Deployment.dll"
------------------
解析: "Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
找到单个程序集: "Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
从以下位置加载: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\Microsoft.CSharp.dll"
------------------
解析: "System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
找到单个程序集: "System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
从以下位置加载: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Web.ApplicationServices.dll"
#endif
