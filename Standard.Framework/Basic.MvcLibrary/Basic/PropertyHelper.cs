using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Basic.MvcLibrary
{
	internal class PropertyHelper
	{
		private delegate TValue ByRefFunc<TDeclaringType, TValue>(ref TDeclaringType arg);

		private static ConcurrentDictionary<Type, PropertyHelper[]> _reflectionCache = new ConcurrentDictionary<Type, PropertyHelper[]>();

		private Func<object, object> _valueGetter;

		private static readonly MethodInfo _callPropertyGetterOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertyGetter", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly MethodInfo _callPropertyGetterByReferenceOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertyGetterByReference", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly MethodInfo _callPropertySetterOpenGenericMethod = typeof(PropertyHelper).GetMethod("CallPropertySetter", BindingFlags.Static | BindingFlags.NonPublic);

		public virtual string Name
		{
			get;
			protected set;
		}

		public PropertyHelper(PropertyInfo property)
		{
			Name = property.Name;
			_valueGetter = MakeFastPropertyGetter(property);
		}

		public static Action<TDeclaringType, object> MakeFastPropertySetter<TDeclaringType>(PropertyInfo propertyInfo) where TDeclaringType : class
		{
			MethodInfo setMethod = propertyInfo.GetSetMethod();
			Type reflectedType = propertyInfo.ReflectedType;
			Type parameterType = setMethod.GetParameters()[0].ParameterType;
			Delegate firstArgument = setMethod.CreateDelegate(typeof(Action<,>).MakeGenericType(reflectedType, parameterType));
			MethodInfo method = _callPropertySetterOpenGenericMethod.MakeGenericMethod(reflectedType, parameterType);
			return (Action<TDeclaringType, object>)Delegate.CreateDelegate(typeof(Action<TDeclaringType, object>), firstArgument, method);
		}

		public object GetValue(object instance)
		{
			return _valueGetter(instance);
		}

		public static PropertyHelper[] GetProperties(object instance)
		{
			return GetProperties(instance, CreateInstance, _reflectionCache);
		}

		public static Func<object, object> MakeFastPropertyGetter(PropertyInfo propertyInfo)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			Type reflectedType = getMethod.ReflectedType;
			Type returnType = getMethod.ReturnType;
			Delegate @delegate;
			if (reflectedType.IsValueType)
			{
				Delegate firstArgument = getMethod.CreateDelegate(typeof(ByRefFunc<,>).MakeGenericType(reflectedType, returnType));
				MethodInfo method = _callPropertyGetterByReferenceOpenGenericMethod.MakeGenericMethod(reflectedType, returnType);
				@delegate = Delegate.CreateDelegate(typeof(Func<object, object>), firstArgument, method);
			}
			else
			{
				Delegate firstArgument2 = getMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(reflectedType, returnType));
				MethodInfo method2 = _callPropertyGetterOpenGenericMethod.MakeGenericMethod(reflectedType, returnType);
				@delegate = Delegate.CreateDelegate(typeof(Func<object, object>), firstArgument2, method2);
			}

			return (Func<object, object>)@delegate;
		}

		private static PropertyHelper CreateInstance(PropertyInfo property)
		{
			return new PropertyHelper(property);
		}

		private static object CallPropertyGetter<TDeclaringType, TValue>(Func<TDeclaringType, TValue> getter, object @this)
		{
			return getter((TDeclaringType)@this);
		}

		private static object CallPropertyGetterByReference<TDeclaringType, TValue>(ByRefFunc<TDeclaringType, TValue> getter, object @this)
		{
			TDeclaringType arg = (TDeclaringType)@this;
			return getter(ref arg);
		}

		private static void CallPropertySetter<TDeclaringType, TValue>(Action<TDeclaringType, TValue> setter, object @this, object value)
		{
			setter((TDeclaringType)@this, (TValue)value);
		}

		protected static PropertyHelper[] GetProperties(object instance, Func<PropertyInfo, PropertyHelper> createPropertyHelper, ConcurrentDictionary<Type, PropertyHelper[]> cache)
		{
			Type type = instance.GetType();
			if (!cache.TryGetValue(type, out PropertyHelper[] value))
			{
				IEnumerable<PropertyInfo> enumerable = from prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
													   where prop.GetIndexParameters().Length == 0 && prop.GetMethod != null
													   select prop;
				List<PropertyHelper> list = new List<PropertyHelper>();
				foreach (PropertyInfo item2 in enumerable)
				{
					PropertyHelper item = createPropertyHelper(item2);
					list.Add(item);
				}

				value = list.ToArray();
				cache.TryAdd(type, value);
			}

			return value;
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
