
using Basic.Interfaces;
using System;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Configuration;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 扩展属性类型为Enum类型的方法
	/// </summary>
	public static class EnumExtension
	{
		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <typeparam name="TP">模型属性</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		private static string PrivateEnumFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, TP>(expression, html.ViewData);
			if (metadata.Model != null && metadata.ModelType.IsEnum)
			{
				string enumName = metadata.ModelType.Name;
				string converterName = null;
				if (Attribute.IsDefined(metadata.ModelType, typeof(WebDisplayConverterAttribute)))
				{
					WebDisplayConverterAttribute wdca = (WebDisplayConverterAttribute)Attribute.GetCustomAttribute(metadata.ModelType, typeof(WebDisplayConverterAttribute));
					converterName = wdca.ConverterName;
				}
				string name = metadata.Model.ToString();
				if (name.IndexOf(",") >= 0)
				{
					List<string> names = new List<string>(10);
					foreach (string item in name.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					{
						string itemName = string.Concat(enumName, "_", item.Trim());
						names.Add(html.GetEnumText(converterName, itemName, item));
					}
					return string.Join(", ", names.ToArray());
				}
				else
				{
					string itemName = string.Concat(enumName, "_", name);
					return html.GetEnumText(converterName, itemName, name);
				}
			}
			else if (metadata.Model != null) { return Convert.ToString(metadata.Model); }
			return string.Empty;
		}

		/// <summary>
		/// 获取枚举值显示文本
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="enumValue">如果不存在枚举资源指则反悔枚举值。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		private static string GetEnumText(this System.Web.Mvc.HtmlHelper html, string converterName, string name, string enumValue)
		{
			string text = html.GetString(converterName, name);
			if (text == name) { return enumValue; }
			return text;
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <typeparam name="TP">模型属性</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		public static MvcHtmlString EnumFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression)
		{
			return MvcHtmlString.Create(html.AttributeEncode(html.PrivateEnumFor<TM, TP>(expression)));
		}
	}
}
