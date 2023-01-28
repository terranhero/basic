using Basic.EntityLayer;
using System;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 定义用于为实现 System.Web.Mvc.IModelBinder 接口的类动态实现模型绑定的方法。
	/// </summary>
	public sealed class DataEntityModelBinderProvider : IModelBinderProvider
	{

		/// <summary>
		/// 注册 DataEntityModelBinderProvider 实体模型绑定提供程序。
		/// </summary>
		public static void RegisterModelBinderProvider()
		{
			ModelBinderProviders.BinderProviders.Add(new DataEntityModelBinderProvider());
		}
		/// <summary>
		/// 返回指定类型的模型联编程序。
		/// </summary>
		/// <param name="modelType">模型的类型。</param>
		/// <returns>指定类型的模型联编程序。</returns>
		public IModelBinder GetBinder(Type modelType)
		{
			if (modelType.IsArray && modelType.GetElementType().IsSubclassOf(typeof(AbstractEntity)))
				return new DataEntityModelBinder();
			else if (modelType.IsSubclassOf(typeof(AbstractEntity)))
				return new DataEntityModelBinder();
			return new DefaultModelBinder();
		}
	}
}
