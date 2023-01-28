using System;
using System.Linq;
using System.Collections.Generic;
using Basic.EntityLayer;
using Basic.Messages;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 枚举类型列信息
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataGridEnumColumn<T> : DataGridColumn<T, Enum> where T : class
	{
		private Dictionary<object, string> enumDictionary = null;
		/// <summary>
		/// 初始化 DataGridJsonColumn 类实例
		/// </summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		/// <param name="field">当前列字段名或属性名</param>
		/// <param name="valueFunc">目标值计算公式(从一个表达式，标识包含要呈现的属性的对象)</param>
		/// <param name="metaData">当前列对应的属性元数据</param>
		internal protected DataGridEnumColumn(IBasicContext context, string field, Func<T, Enum> valueFunc, EntityPropertyMeta metaData)
			: base(context, field, valueFunc, metaData)
		{
			CreateValueAndLocalText(metaData.PropertyType);
		}

		/// <summary>
		/// 创建枚举多语言资源信息
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		private void CreateValueAndLocalText(Type enumType)
		{
			string enumName = enumType.Name;
			string converterName = null;
			if (Attribute.IsDefined(enumType, typeof(WebDisplayConverterAttribute)))
			{
				WebDisplayConverterAttribute wdca = (WebDisplayConverterAttribute)Attribute.GetCustomAttribute(enumType, typeof(WebDisplayConverterAttribute));
				converterName = wdca.ConverterName;
			}
			Array valueArray = Enum.GetValues(enumType);
			enumDictionary = new Dictionary<object, string>(valueArray.Length);
			string text = null;
			foreach (object value in valueArray)
			{
				string name = Enum.GetName(enumType, value);
				string itemName = string.Concat(enumName, "_", name);
				if (string.IsNullOrWhiteSpace(converterName))
					text = _Context.GetString(itemName);
				else
					text = _Context.GetString(converterName, itemName);
				if (!string.IsNullOrWhiteSpace(text) && text != itemName)
					enumDictionary[value] = text;
				else
					enumDictionary[value] = name;
			}
		}

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override string GetString(T model)
		{
			Enum result = _ValueFunc(model);
			Type eType = _PropertyMeta.PropertyType;
			string converterName = null;
			if (Attribute.IsDefined(eType, typeof(WebDisplayConverterAttribute)))
			{
				WebDisplayConverterAttribute wdca = (WebDisplayConverterAttribute)Attribute.GetCustomAttribute(eType, typeof(WebDisplayConverterAttribute));
				converterName = wdca.ConverterName;
			}
			string enumName = eType.Name;
			string name = result.ToString();
			if (name.IndexOf(",") >= 0)
			{
				List<string> names = new List<string>(10);
				foreach (string item in name.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					string itemName = string.Concat(enumName, "_", item.Trim());
					names.Add(_Context.GetString(converterName, itemName, item));
				}
				return string.Join(", ", names.ToArray());
			}
			else
			{
				if (enumDictionary.ContainsKey(result)) { return enumDictionary[result]; }
			}
			return string.Empty;
		}
	}
}
