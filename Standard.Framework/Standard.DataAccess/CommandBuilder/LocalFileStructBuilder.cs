using System.IO;
using System.Xml;


namespace Basic.DataAccess
{
	/// <summary>
	/// 配置文件以文件形式存于当前目录
	/// </summary>
	public sealed class LocalFileStructBuilder : AbstractXmlStructBuilder
	{
		private readonly string FileName = null;
		/// <summary>
		/// 创建FileStructBuilder实例
		/// </summary>
		public LocalFileStructBuilder(string fileName) { FileName = fileName; }

		/// <summary>
		/// 使用指定的流创建一个新的 System.Xml.XmlReader 实例。
		/// 该实例指定配置文件内容信息。
		/// </summary>
		/// <returns>一个用于读取数据流中所含数据的 System.Xml.XmlReader 对象。</returns>
		protected override XmlReader CreateXmlReader()
		{
			return XmlReader.Create(FileName);
		}
	}
}
