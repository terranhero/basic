using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
    /// <summary>
    /// 映射浏览器数据对象的绑定。 
    /// 这个类提供了一个模型绑定器的具体实现。
    /// </summary>
    public sealed class DataEntityArrayModelBinder : IModelBinder
    {
        /// <summary>
        /// 使用指定的控制器上下文和绑定上下文将模型绑定到一个值。
        /// </summary>
        /// <param name="controllerContext">控制器上下文。</param>
        /// <param name="bindingContext">绑定上下文。</param>
        /// <returns>绑定值。</returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type elementType = bindingContext.ModelType.GetElementType();
            Type type3 = typeof(List<>).MakeGenericType(new Type[] { elementType });
            object collection = this.CreateModel(controllerContext, bindingContext, type3);
            ModelBindingContext context = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => collection, type3),
                ModelName = bindingContext.ModelName,
                ModelState = bindingContext.ModelState,
                PropertyFilter = bindingContext.PropertyFilter,
                ValueProvider = bindingContext.ValueProvider
            };
            IList list = (IList)this.UpdateCollection(controllerContext, context, elementType);
            if (list == null) { return null; }
            Array array = Array.CreateInstance(elementType, list.Count);
            list.CopyTo(array, 0);
            return array;
        }

        /// <summary>使用指定的控制器上下文和绑定上下文来创建指定的模型类型。</summary>
        /// <returns>指定类型的数据对象。</returns>
        /// <param name="controllerContext">运行控制器的上下文。上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
        /// <param name="bindingContext">绑定模型的上下文。上下文包含模型对象、模型名称、模型类型、属性筛选器和值提供程序等信息。</param>
        /// <param name="modelType">要返回的模型对象的类型。</param>
        internal object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            Type type = modelType;
            if (modelType.IsGenericType)
            {
                Type genericTypeDefinition = modelType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(IDictionary<,>))
                {
                    type = typeof(Dictionary<,>).MakeGenericType(modelType.GetGenericArguments());
                }
                else if (((genericTypeDefinition == typeof(IEnumerable<>)) || (genericTypeDefinition == typeof(ICollection<>))) || (genericTypeDefinition == typeof(IList<>)))
                {
                    type = typeof(List<>).MakeGenericType(modelType.GetGenericArguments());
                }
            }
            return Activator.CreateInstance(type);
        }

        internal object UpdateCollection(ControllerContext controllerContext, ModelBindingContext bindingContext, Type elementType)
        {
            bool flag = true;
            // IEnumerable<string> enumerable;
            //GetIndexes(bindingContext, out flag, out enumerable);
            IModelBinder binder = ModelBinders.Binders.GetBinder(elementType);
            List<object> newContents = new List<object>();
            //foreach (string str in enumerable)
            //{
            string prefix = CreateSubIndexName(bindingContext.ModelName, "0");
            if (!bindingContext.ValueProvider.ContainsPrefix(prefix))
            {
                if (!flag)
                {
                    return null;
                }
                return null;
            }
            ModelBindingContext context2 = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, elementType),
                ModelName = prefix,
                ModelState = bindingContext.ModelState,
                PropertyFilter = bindingContext.PropertyFilter,
                ValueProvider = bindingContext.ValueProvider
            };
            ModelBindingContext context = context2;
            object obj2 = binder.BindModel(controllerContext, context);
            // AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, prefix, elementType, obj2);
            newContents.Add(obj2);
            //}
            if (newContents.Count == 0)
            {
                return null;
            }
            object model = bindingContext.Model;
            //CollectionHelpers.ReplaceCollection(elementType, model, newContents);
            return model;
        }

        /// <summary>基于组成较大索引的组件类别创建索引（子索引），其中指定的索引值为字符串。</summary>
        /// <returns>子索引的名称。</returns>
        /// <param name="prefix">子索引的前缀。</param>
        /// <param name="index">索引值。</param>
        private string CreateSubIndexName(string prefix, string index)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", new object[] { prefix, index });
        }
    }
}
