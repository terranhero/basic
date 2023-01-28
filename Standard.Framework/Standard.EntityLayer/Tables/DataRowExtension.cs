using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Basic.Tables
{
	/// <summary>
	/// 扩展 DataRow 类实例的方法
	/// </summary>
	public static class DataRowExtension
	{
		/// <summary>
		/// 确定IStringObjectArray是否包含具有指定键的元素。
		/// </summary>
		/// <param name="row">需要扩展的 DataRow 类实例。</param>
		/// <param name="key">要在IStringObjectArray中定位的键。</param>
		/// <returns>如果IStringObjectArray包含带有该键的元素，则为 true；否则，为 false。</returns>
		public static bool ContainsKey(this DataRow row, string key)
		{
			return row.Table.Columns.Contains(key);
		}

		/// <summary>
		/// 设置指定键的值
		/// </summary>
		/// <param name="row">需要扩展的 DataRow 类实例。</param>
		/// <param name="key">字典中键的名称</param>
		/// <param name="value">需要设置的值</param>
		public static void SetObject(this DataRow row, string key, object value)
		{
			row[key] = value;
		}

		/// <summary>
		/// 获取指定键的Object值。
		/// </summary>
		/// <param name="row">需要扩展的 DataRow 类实例。</param>
		/// <param name="key">字典中键的名称</param>
		/// <returns>指定键的Object值。</returns>
		public static object GetObject(this DataRow row, string key)
		{
			return row[key];
		}
	}
}