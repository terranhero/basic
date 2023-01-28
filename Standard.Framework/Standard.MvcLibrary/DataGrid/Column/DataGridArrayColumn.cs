using System;
using System.Collections.Generic;
using System.Linq;
using Basic.EntityLayer;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示数组类列
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TR"></typeparam>
	public sealed class DataGridArrayColumn<T, TR> : DataGridArrayColumn<T>
		where T : class
		where TR : struct, IComparable<TR>, IEquatable<TR>
	{
		internal readonly Func<T, IEnumerable<PropertyMeta<TR>>> _ValueFunc;
		internal readonly EntityPropertyMeta _PropertyMeta;

		//private PropertyMeta<TR>[]
		/// <summary>
		/// 初始化 DataGridArrayColumn 列信息
		/// </summary>
		/// <param name="httpContext">用作包含有关某个 HTTP 请求的 HTTP 特定信息的类。</param>
		/// <param name="valueFunc">目标值计算公式(从一个表达式，标识包含要呈现的属性的对象)</param>
		/// <param name="metaData">当前列对应的属性元数据</param>
		/// <param name="fields"></param>
		internal DataGridArrayColumn(IBasicContext httpContext, Func<T, IEnumerable<PropertyMeta<TR>>> valueFunc,
			EntityPropertyMeta metaData, IDictionary<string, string> fields)
			: base(httpContext, fields) { _ValueFunc = valueFunc; _PropertyMeta = metaData; }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public IEnumerable<PropertyMeta<TR>> GetModelValue(T model) { return _ValueFunc(model); }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override object GetValue(T model) { return _ValueFunc(model); }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override string GetString(T model) { return ""; }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override IDictionary<string, object> GetValues(object model)
		{
			IEnumerable<PropertyMeta<TR>> result1 = GetModelValue((T)model);
			var tt = from property in result1
					 join field in Fields on property.Key equals field.Key
					 select property;
			return tt.ToDictionary(m => m.Key, m => (object)m.Value);
		}

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override IDictionary<string, string> GetStrings(object model)
		{
			IEnumerable<PropertyMeta<TR>> result1 = GetModelValue((T)model);
			var tt = from property in result1
					 join field in Fields on property.Key equals field.Key
					 select property;
			return tt.ToDictionary(m => m.Key, m =>
			{
				object result = m.Value;
				if (string.IsNullOrWhiteSpace(Format) == false) { return string.Format(Format, result); }
				if (result is DateTime) { return this.GetDateTimeString(Convert.ToDateTime(result)); }
				if (result is TimeSpan) { return GetTimeString((TimeSpan)Convert.ChangeType(result, typeof(TimeSpan))); }
				return Convert.ToString(result, System.Globalization.CultureInfo.CurrentCulture);
			});
		}

		/// <summary>
		/// 获取日期时间格式化返回值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		internal string GetDateTimeString(DateTime value)
		{
			if (value == DateTime.MinValue || value == DateTimeConverter.MinValue)
				return string.Empty;
			else if (!string.IsNullOrWhiteSpace(_PropertyMeta.DisplayFormatString))
				return string.Format(_PropertyMeta.DisplayFormatString, value);
			else if (value.Hour > 0 || value.Minute > 0 || value.Second > 0)
				return string.Format("{0:yyyy-MM-dd HH:mm:ss}", value);
			else
				return string.Format("{0:yyyy-MM-dd}", value);
		}

		/// <summary>
		/// 获取日期时间格式化返回值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		internal string GetTimeString(TimeSpan value)
		{
			if (!string.IsNullOrWhiteSpace(_PropertyMeta.DisplayFormatString))
				return string.Format(_PropertyMeta.DisplayFormatString, value);
			else if (value.Seconds > 0)
				return string.Format("{0:HH:mm:ss}", value);
			else
				return string.Format("{0:HH:mm}", value);
		}
	}

	/// <summary>
	/// 表示数组类列
	/// </summary>
	public abstract class DataGridArrayColumn<T> : DataGridColumn<T>
	{
		private readonly IDictionary<string, string> _Fields;
		/// <summary>
		/// 初始化 DataGridColumn 类实例
		/// </summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		/// <param name="fields">数组列字段信息</param>
		protected DataGridArrayColumn(IBasicContext context, IDictionary<string, string> fields) : base(context) { _Fields = fields; }

		/// <summary>
		/// 数组列字段列表
		/// </summary>
		public IDictionary<string, string> Fields { get { return _Fields; } }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public abstract IDictionary<string, object> GetValues(object model);

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public abstract IDictionary<string, string> GetStrings(object model);
	}
}
