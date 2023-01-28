using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示数据库列信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class GroupNameAttribute : Attribute
	{
		/// <summary>
		/// 初始化GroupNameAttribute类实例
		/// </summary>
		/// <param name="name">资源组名称</param>
		public GroupNameAttribute(string name)
		{
			_Name = name;
		}

		/// <summary>
		/// 初始化GroupNameAttribute类实例
		/// </summary>
		/// <param name="name">资源组名称</param>
		/// <param name="converterName">转换器名称</param>
		public GroupNameAttribute(string name,string converterName)
		{
			_Name = name; _ConverterName = converterName;
		}

		private readonly string _Name;
		private readonly string _ConverterName = null;
		/// <summary>
		/// 资源组名称
		/// </summary>
		public string Name { get { return _Name; } }

		/// <summary>
		/// 指定当前属性显示名称需要转换的转换器名称。
		/// </summary>
		/// <value>属性名称的文本转换器名称。</value>
		public string ConverterName { get { return _ConverterName; } }
	}
}
