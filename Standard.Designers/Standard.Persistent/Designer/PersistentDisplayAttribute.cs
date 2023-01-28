using System;
using System.ComponentModel;
using Basic.Properties;

namespace Basic.Designer
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class PersistentDisplayAttribute : DisplayNameAttribute
	{
		/// <summary>
		/// 使用显示名称初始化 Basic.Designer.PersistentDisplayAttribute 类的新实例。
		/// </summary>
		/// <param name="displayName">显示名称。</param>
		public PersistentDisplayAttribute(string displayName) : base(displayName) { }

		/// <summary>
		/// 获取属性、不采用此特性中存储的任何参数的公共 void 方法的显示名称。
		/// </summary>
		public override string DisplayName
		{
			get
			{
				return StringUtils.GetString(base.DisplayNameValue);
			}
		}
	}
}
