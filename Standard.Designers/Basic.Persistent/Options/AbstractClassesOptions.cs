using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Basic.DataAccess;
using Basic.EntityLayer;
using Basic.Enums;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Basic.Options
{
	/// <summary>表示允许集成的抽象条件类</summary>
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class AbstractClassesOptions : DialogPage
	{
		private static readonly XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };
		public AbstractClassesOptions() { }

		/// <summary>条件模型基类</summary>
		public BindingList<string> BaseConditions { get; set; } = new BindingList<string>();

		/// <summary>实体模型基类</summary>
		public BindingList<string> BaseEntities { get; set; } = new BindingList<string>();

		/// <summary>实体模型数据持久基类</summary>
		public BindingList<string> BaseAccess { get; set; } = new BindingList<string>();

		public override void LoadSettingsFromStorage()
		{
			IVsSolution vsSolution = GetService(typeof(SVsSolution)) as IVsSolution;
			if (vsSolution != null && vsSolution.GetSolutionInfo(out string slnFonder, out string slnFile, out string slnUserOptions) == VSConstants.S_OK)
			{
				string files = string.Concat(slnFonder, "\\References\\Persistents.settings");
				if (System.IO.File.Exists(files))
				{
					BaseConditions.Clear(); BaseEntities.Clear(); BaseAccess.Clear();
					using (XmlReader reader = XmlReader.Create(files))
					{
						this.ReadXml(reader);
					}
				}
			}
			if (BaseConditions.Count == 0) { BaseConditions.Add(typeof(AbstractCondition).FullName); }
			if (BaseEntities.Count == 0) { BaseEntities.Add(typeof(AbstractEntity).FullName); }
			if (BaseAccess.Count == 0)
			{
				BaseAccess.Add(typeof(AbstractAccess).FullName);
				BaseAccess.Add(typeof(AbstractDbAccess).FullName);
			}
		}

		//public override void LoadSettingsFromXml(IVsSettingsReader reader)
		//{
		//	base.LoadSettingsFromXml(reader);
		//}

		//public override void SaveSettingsToXml(IVsSettingsWriter writer)
		//{
		//	base.SaveSettingsToXml(writer);
		//}

		public override void SaveSettingsToStorage()
		{
			IVsSolution vsSolution = GetService(typeof(SVsSolution)) as IVsSolution;
			if (vsSolution != null && vsSolution.GetSolutionInfo(out string slnFonder, out string slnFile, out string slnUserOptions) == VSConstants.S_OK)
			{
				string files = string.Concat(slnFonder, "\\References\\Persistents.settings");
				using (FileStream stream = new FileStream(files, FileMode.OpenOrCreate, FileAccess.Write))
				{
					using (XmlWriter writer = XmlWriter.Create(stream, settings))
					{
						this.WriteXml(writer);
					}
				}
			}
		}

		protected override IWin32Window Window
		{
			get
			{
				AbstractClassesControl page = new AbstractClassesControl(this);
				page.Dock = DockStyle.Fill;
				page.Initialize();
				return page;
			}
		}

		#region 接口 IXmlSerializable 默认实现
		private const string rootElement = "PersistentConfiguration";
		private const string conditionsElement = "ConditionBaseClasses";
		private const string conditionElement = "ConditionBaseClass";
		private const string modelsElement = "EntityBaseClasses";
		private const string modelElement = "EntityBaseClass";
		private const string accessesElement = "AccessBaseClasses";
		private const string accessElement = "AccessBaseClass";
		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		private void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == conditionElement)
				{
					BaseConditions.Add(reader.ReadString());
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == modelElement)
				{
					BaseEntities.Add(reader.ReadString());
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == accessElement)
				{
					BaseAccess.Add(reader.ReadString());
				}
			}
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		private void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(rootElement);
			writer.WriteStartElement(conditionsElement);
			foreach (string name in BaseConditions)
			{
				writer.WriteElementString(conditionElement, name);
			}
			writer.WriteEndElement();
			writer.WriteStartElement(modelsElement);
			foreach (string name in BaseEntities)
			{
				writer.WriteElementString(modelElement, name);
			}
			writer.WriteEndElement();
			writer.WriteStartElement(accessesElement);
			foreach (string name in BaseAccess)
			{
				writer.WriteElementString(accessElement, name);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		#endregion
	}
}
