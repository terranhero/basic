using Basic.EntityLayer;
using Basic.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 提供用于缓存 System.Web.Mvc.DataAnnotationsModelMetadata 的容器。
	/// </summary>
	public sealed class CultureModelMetadata : CachedDataAnnotationsModelMetadata
	{
		private readonly SortedList<int, string> displayNameDirectory;
		private readonly StringLengthAttribute _StringLength;
		private readonly GroupNameAttribute _GroupName;
		/// <summary>
		/// 使用原型和模型访问器来初始化 CultureModelMetadata 类的新实例。
		/// </summary>
		/// <param name="prototype">原型。</param>
		/// <param name="modelAccessor">模型访问器。</param>
		public CultureModelMetadata(CachedDataAnnotationsModelMetadata prototype, Func<object> modelAccessor)
			: base(prototype, modelAccessor) { displayNameDirectory = new SortedList<int, string>(10); }

		/// <summary>
		/// 使用提供程序、容器类型、模型类型、属性名称和特性来初始化 CultureModelMetadata 类的新实例。
		/// </summary>
		/// <param name="provider">提供程序。</param>
		/// <param name="containerType">容器类型。</param>
		/// <param name="modelType">模型类型。</param>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="attributes">特性。</param>
		public CultureModelMetadata(CachedDataAnnotationsModelMetadataProvider provider, Type containerType,
			Type modelType, string propertyName, IEnumerable<Attribute> attributes)
			: base(provider, containerType, modelType, propertyName, attributes)
		{
			displayNameDirectory = new SortedList<int, string>(10);
			foreach (Attribute attr in attributes)
			{
				if (attr is Basic.Validations.StringLengthAttribute) { _StringLength = attr as Basic.Validations.StringLengthAttribute; }
				else if (attr is StringLengthAttribute) { _StringLength = attr as StringLengthAttribute; }
				else if (attr is GroupNameAttribute) { _GroupName = attr as GroupNameAttribute; }
			}
		}

		/// <summary>
		/// 获取当前模型属性的显示特性。
		/// </summary>
		public WebDisplayAttribute WebDisplay
		{
			get
			{
				if (base.PrototypeCache.DisplayName is WebDisplayAttribute)
					return base.PrototypeCache.DisplayName as WebDisplayAttribute;
				return null;
			}
		}

		/// <summary>
		/// 获取当前模型属性的显示特性。
		/// </summary>
		public StringLengthAttribute StringLength { get { return _StringLength; } }

		/// <summary>
		/// 获取当前模型属性的显示特性。
		/// </summary>
		public GroupNameAttribute GroupName { get { return _GroupName; } }

		/// <summary>
		/// 获取模型的显示名称。如果该值已缓存，则返回已缓存的值；否则，将从模型元数据中检索该值并将该值存储在缓存中。
		/// </summary>
		/// <returns>模型的显示名称。</returns>
		protected override string ComputeDisplayName()
		{
			CultureInfo clientCultureInfo = MessageCulture.CultureInfo;
			if (displayNameDirectory.ContainsKey(clientCultureInfo.LCID))
				return displayNameDirectory[clientCultureInfo.LCID];
			string displayname = base.ComputeDisplayName();
			if (string.IsNullOrWhiteSpace(displayname))
				return displayNameDirectory[clientCultureInfo.LCID] = displayname;
			if (base.PrototypeCache.DisplayName is WebDisplayAttribute)
			{
				WebDisplayAttribute wda = base.PrototypeCache.DisplayName as WebDisplayAttribute;
				if (wda.DisplayName == displayname)
				{
					displayname = MessageContext.GetString(wda.ConverterName, wda.DisplayName, clientCultureInfo);
					displayNameDirectory[clientCultureInfo.LCID] = displayname;
				}
			}
			return displayname;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string ComputeWatermark()
		{
			WebDisplayAttribute wda = WebDisplay;
			if (wda != null && !string.IsNullOrWhiteSpace(wda.Prompt))
			{
				CultureInfo clientCultureInfo = MessageCulture.CultureInfo;
				return MessageContext.GetString(wda.ConverterName, wda.Prompt, clientCultureInfo);
			}
			return base.ComputeWatermark();
		}
	}
}
