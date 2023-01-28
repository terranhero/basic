using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Xml;
using Basic.Exceptions;
using Basic.Collections;
using System.Xml.Serialization;
using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractXmlStructBuilder : CommandBuilder
	{
		/// <summary>
		/// 初始化 AssemblyStructBuilder 实例
		/// </summary>
		public AbstractXmlStructBuilder() { }

		/// <summary>
		/// 使用指定的流创建一个新的 System.Xml.XmlReader 实例。
		/// 该实例指定配置文件内容信息。
		/// </summary>
		/// <returns>一个用于读取数据流中所含数据的 System.Xml.XmlReader 对象。</returns>
		protected abstract XmlReader CreateXmlReader();

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		public override void Dispose() { }

		/// <summary>
		/// 根据命令名称创建数据库命令
		/// </summary>
		/// <param name="tableInfo">当前配置文件的缓存</param>
		internal override void CreateDataCommand(TableConfiguration tableInfo)
		{
			using (XmlReader reader = CreateXmlReader())
			{
				tableInfo.ReadXml(reader);
			}
		}
	}
}
