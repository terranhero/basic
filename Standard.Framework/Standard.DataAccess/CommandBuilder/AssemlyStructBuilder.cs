using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Basic.DataAccess
{
	/// <summary>
	/// 将配置文件作为程序集资源方式
	/// </summary>
	public sealed class AssemblyStructBuilder : AbstractXmlStructBuilder
	{
		private readonly Type acType;
		private readonly string FullName;
		/// <summary>
		/// 初始化AssemblyStructBuilder实例
		/// </summary>
		public AssemblyStructBuilder(Type act, string fileName) { acType = act; FullName = fileName; }

		/// <summary>
		/// 使用指定的流创建一个新的 System.Xml.XmlReader 实例。
		/// 该实例指定配置文件内容信息。
		/// </summary>
		/// <returns>一个用于读取数据流中所含数据的 System.Xml.XmlReader 对象。</returns>
		protected override XmlReader CreateXmlReader()
		{
			Assembly ass = Assembly.GetAssembly(acType);
			System.IO.Stream fileStream = ass.GetManifestResourceStream(FullName);
			if (fileStream != null)
			{
				if (fileStream.Position > 0)
					fileStream.Position = 0;
				return XmlReader.Create(fileStream);
			}
			return XmlReader.Create(new MemoryStream());
		}
	}
}
