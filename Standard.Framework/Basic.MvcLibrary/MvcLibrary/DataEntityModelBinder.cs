using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 定义模型联编程序所需的方法。(布尔类型向)
	/// </summary>
	public sealed class DataEntityModelBinder : DefaultModelBinder
	{
		/// <summary>
		///  使用指定的控制器上下文、绑定上下文和指定的属性描述符来绑定指定的属性。
		/// </summary>
		/// <param name="controllerContext">运行控制器的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		/// <param name="bindingContext">绑定模型的上下文。 上下文包含模型对象、模型名称、模型类型、属性筛选器和值提供程序等信息。</param>
		/// <param name="propertyDescriptor">描述要绑定的属性。 该描述符提供组件类型、属性类型和属性值等信息。 它还提供用于获取或设置属性值的方法。</param>
		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			string prefix = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
			if (!bindingContext.ValueProvider.ContainsPrefix(prefix))
			{
				if (this.OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, null))
				{
					this.SetProperty(controllerContext, bindingContext, propertyDescriptor, null);
					this.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, null);
					return;
				}
				return;
			}
			IModelBinder propertyBinder = this.Binders.GetBinder(propertyDescriptor.PropertyType);
			object obj2 = propertyDescriptor.GetValue(bindingContext.Model);
			ModelMetadata metadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
			metadata.Model = obj2;
			ModelBindingContext context2 = new ModelBindingContext()
			{
				ModelMetadata = metadata,
				ModelName = prefix,
				ModelState = bindingContext.ModelState,
				ValueProvider = bindingContext.ValueProvider
			};
			ModelBindingContext context = context2;
			object obj3 = this.GetPropertyValue(controllerContext, context, propertyDescriptor, propertyBinder);
			metadata.Model = obj3;
			ModelState state = bindingContext.ModelState[prefix];
			if ((state == null) || (state.Errors.Count == 0))
			{
				if (this.OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, obj3))
				{
					this.SetProperty(controllerContext, bindingContext, propertyDescriptor, obj3);
					this.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, obj3);
					return;
				}
				return;
			}
			this.SetProperty(controllerContext, bindingContext, propertyDescriptor, obj3);

			foreach (ModelError err in (from err in state.Errors
										where string.IsNullOrEmpty(err.ErrorMessage) && (err.Exception != null)
										select err).ToList<ModelError>())
			{
				for (Exception exception = err.Exception; exception != null; exception = exception.InnerException)
				{
					if (exception is FormatException)
					{
						string errorMessage = controllerContext.Controller.GetString("AbstractEntity_ValueInvalid",
						  new string[] { state.Value.AttemptedValue, metadata.DisplayName });
						state.Errors.Remove(err);
						state.Errors.Add(errorMessage);
						break;
					}
				}
			}
		}

		/// <summary>使用指定的控制器上下文、绑定上下文和属性值来设置指定的属性。</summary>
		/// <param name="controllerContext">运行控制器的上下文。上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		/// <param name="bindingContext">绑定模型的上下文。上下文包含模型对象、模型名称、模型类型、属性筛选器和值提供程序等信息。</param>
		/// <param name="propertyDescriptor">描述要设置的属性。该描述符提供组件类型、属性类型和属性值等信息。它还提供用于获取或设置属性值的方法。</param>
		/// <param name="value">为属性设置的值。</param>
		protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			if (propertyDescriptor.PropertyType == typeof(bool)) { value = value ?? false; }
			base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
		}
		/// <summary>
		///  使用指定的控制器上下文、绑定上下文、属性描述符和属性联编程序来返回属性值。
		/// </summary>
		/// <param name="controllerContext">运行控制器的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		/// <param name="bindingContext">绑定模型的上下文。 上下文包含模型对象、模型名称、模型类型、属性筛选器和值提供程序等信息。</param>
		/// <param name="propertyDescriptor">描述要绑定的属性。 该描述符提供组件类型、属性类型和属性值等信息。 它还提供用于获取或设置属性值的方法。</param>
		/// <param name="propertyBinder">一个对象，提供用于绑定属性的方式。</param>
		/// <returns>一个对象，表示属性值。</returns>
		protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
		{
			NameValueCollection param = controllerContext.HttpContext.Request.Params;
			NameValueCollection form = controllerContext.HttpContext.Request.Form;
			string formValue = form[bindingContext.ModelName];
			if (formValue == null) { formValue = param[bindingContext.ModelName]; }
			Type propertyType = propertyDescriptor.PropertyType;
			if (propertyType.IsArray)
			{
				Type elementType = propertyType.GetElementType();
				if (string.IsNullOrWhiteSpace(formValue) == true)
				{
					Type modelType2 = typeof(List<>).MakeGenericType(elementType);
					IList collection = (IList)CreateModel(controllerContext, bindingContext, modelType2);
					int index = 0;
					while (true)
					{
						formValue = form[string.Concat(bindingContext.ModelName, "[", index, "]")];
						if (formValue == null) { formValue = param[string.Concat(bindingContext.ModelName, "[", index, "]")]; }
						if (string.IsNullOrWhiteSpace(formValue) == true) { break; }
						if (elementType == typeof(Guid)) { collection.Add(new Guid(formValue)); }
						else if (elementType == typeof(DateTime) && DateTime.TryParse(formValue, out DateTime tempDate)) { collection.Add(tempDate); }
						else if (elementType.IsEnum && int.TryParse(formValue, out int tempValue)) { collection.Add(Enum.ToObject(elementType, tempValue)); }
						else { collection.Add(Convert.ChangeType(formValue, elementType, CultureInfo.CurrentCulture)); }
						index++;
					}
					Array array = Array.CreateInstance(elementType, collection.Count);
					collection.CopyTo(array, 0);
					return array;
				}
				else
				{
					Array array = Array.CreateInstance(elementType, 1);
					if (elementType.IsEnum)
						return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
					else
						array.SetValue(Convert.ChangeType(formValue, elementType, CultureInfo.CurrentCulture), 0);
					return array;
				}
			}
			else if (propertyType == typeof(bool))
			{
				return formValue == "1" || formValue == "true" || formValue == "True" || formValue == "on";
			}
			else if (propertyType == typeof(bool?) && formValue != "" && formValue != null)
			{
				return formValue == "1" || formValue == "true" || formValue == "True" || formValue == "on";
			}
			return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
		}
	}
}
