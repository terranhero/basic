using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 实现 ASP.NET MVC 的默认已缓存模型元数据提供程序，此提供程序提供多语言系统。
	/// </summary>
	public sealed class CultureModelMetadataProvider : CachedDataAnnotationsModelMetadataProvider
	{
		/// <summary>
		/// 注册自定义多语言模型提供者
		/// </summary>
		public static void RegisterCultureProvider()
		{
			ModelMetadataProviders.Current = new CultureModelMetadataProvider();
		}

		/// <summary>
		/// 初始化 CultureModelMetadataProvider 类的新实例。
		/// </summary>
		public CultureModelMetadataProvider() : base() { }

		/// <summary>
		/// 返回元数据类的原型实例的容器。
		/// </summary>
		/// <param name="attributes">特性类型。</param>
		/// <param name="containerType">容器类型。</param>
		/// <param name="modelType">模型类型。</param>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>元数据类的原型实例的容器。</returns>
		protected override CachedDataAnnotationsModelMetadata CreateMetadataPrototype(IEnumerable<Attribute> attributes, Type containerType, Type modelType, string propertyName)
		{
			return new CultureModelMetadata(this, containerType, modelType, propertyName, attributes);
		}

		/// <summary>
		/// 基于原型和模型访问器，返回已缓存元数据类的实际实例的容器。
		/// </summary>
		/// <param name="prototype">原型。</param>
		/// <param name="modelAccessor">模型访问器。</param>
		/// <returns>已缓存元数据类的实际实例的容器。</returns>
		protected override CachedDataAnnotationsModelMetadata CreateMetadataFromPrototype(CachedDataAnnotationsModelMetadata prototype, Func<object> modelAccessor)
		{
			return new CultureModelMetadata(prototype, modelAccessor);
		}
	}

	///// <summary>
	///// 提供用于实现已缓存元数据提供程序类。
	///// </summary>
	//public class CacheCultureModelMetadataProvider : AssociatedMetadataProvider
	//{
	//	private readonly ObjectCache modelMetadataCache;
	//	private readonly CacheItemPolicy cacheItemPolicy;
	//	private readonly string typeGuidString;
	//	public CacheCultureModelMetadataProvider()
	//		: base()
	//	{
	//		typeGuidString = GetType().GUID.ToString("N");
	//		modelMetadataCache = MemoryCache.Default;
	//		cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(60.0) };
	//	}

	//	/// <summary>
	//	/// 返回模型的属性列表。
	//	/// </summary>
	//	/// <param name="container">模型容器。</param>
	//	/// <param name="containerType">容器的类型。</param>
	//	/// <returns>模型的属性列表。</returns>
	//	public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
	//	{
	//		if (container != null && container is AbstractEntity)
	//		{
	//			AbstractEntity entity = container as AbstractEntity;
	//			IList<ModelMetadata> list = new List<ModelMetadata>();
	//			EntityPropertyDescriptor[] propertyDescriptors = entity.GetProperties();
	//			foreach (EntityPropertyDescriptor propertyInfo in propertyDescriptors)
	//			{
	//				Func<object> modelAccessor = () => { return propertyInfo.GetValue(container); };
	//				list.Add(base.GetMetadataForProperty(modelAccessor, containerType, propertyInfo));
	//			}
	//			return list;
	//		}
	//		return base.GetMetadataForProperties(container, containerType);
	//	}

	//	/// <summary>
	//	/// 创建属性的模型元数据。
	//	/// </summary>
	//	/// <param name="attributes">特性集。</param>
	//	/// <param name="containerType">容器的类型。</param>
	//	/// <param name="modelAccessor">模型访问器。</param>
	//	/// <param name="modelType">模型的类型。</param>
	//	/// <param name="propertyName">属性的名称。</param>
	//	/// <returns>属性的模型元数据。</returns>
	//	protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType,
	//		Func<object> modelAccessor, Type modelType, string propertyName)
	//	{
	//		CultureInfo cultureInfo = MessageCulture.CultureInfo;
	//		Type type = containerType ?? modelType;
	//		string cacheKey = this.GetCacheKey(cultureInfo, containerType, modelType, propertyName);
	//		CacheCultureModelMetadata local = modelMetadataCache.Get(cacheKey) as CacheCultureModelMetadata;
	//		if (local == null)
	//		{
	//			local = new CacheCultureModelMetadata(this, cultureInfo, containerType, modelAccessor, modelType, propertyName, attributes.ToArray());
	//			modelMetadataCache.Add(cacheKey, local, cacheItemPolicy);
	//		}
	//		return local;
	//	}

	//	/// <summary>
	//	/// 获取缓存关键字
	//	/// </summary>
	//	/// <param name="cultureInfo"></param>
	//	/// <param name="containerType"></param>
	//	/// <param name="modelType"></param>
	//	/// <param name="propertyName"></param>
	//	/// <returns></returns>
	//	private string GetCacheKey(CultureInfo cultureInfo, Type containerType, Type modelType, string propertyName)
	//	{
	//		Type type = containerType ?? modelType;
	//		propertyName = propertyName ?? string.Empty;
	//		return string.Concat(typeGuidString, "_", cultureInfo.LCID, "_", type.GUID, "_", propertyName);
	//	}
	//}
}
