using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Basic.Designer
{
    internal sealed class ProjectInfoConverter : TypeConverter
	{
		/// <summary>
		/// 返回此对象是否支持属性。
		/// </summary>
		/// <param name="context"> 一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <returns>如果应调用 System.ComponentModel.TypeConverter.GetProperties(System.Object) 
		/// 来查找此对象的属性，则为 true；否则为 false。</returns>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
