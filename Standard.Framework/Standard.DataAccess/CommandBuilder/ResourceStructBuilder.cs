using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.DataAccess;
using System.Xml;
using System.Resources;
using System.IO;

namespace Basic.DataAccess
{
	/// <summary>
	/// 将资源编译为资源(Resource)
	/// </summary>
	public sealed class ResourceStructBuilder : AbstractXmlStructBuilder
	{
		private readonly string keyName;
		private readonly Type accessType;

		/// <summary>
		/// 初始化AssemblyStructBuilder实例
		/// </summary>
		/// <param name="ci"></param>
		/// <param name="act"></param>
		internal ResourceStructBuilder(Type act, ConfigurationInfo ci)
		{
			accessType = act; keyName = ci.GetResourceFile();
		}

		/// <summary>
		/// 使用指定的流创建一个新的 System.Xml.XmlReader 实例。
		/// 该实例指定配置文件内容信息。
		/// </summary>
		/// <returns>一个用于读取数据流中所含数据的 System.Xml.XmlReader 对象。</returns>
		protected override XmlReader CreateXmlReader()
		{
			System.Reflection.AssemblyName an = accessType.Assembly.GetName();
			string resourceName = string.Concat(an.Name, ".g");
			ResourceManager rm = new ResourceManager(resourceName, accessType.Assembly);
			if (keyName != null)
			{
				UnmanagedMemoryStream stream = rm.GetStream(keyName.ToLower());
				if (stream != null) { return XmlReader.Create(stream); }
			}
			return XmlReader.Create(new MemoryStream());
		}
	}
}
