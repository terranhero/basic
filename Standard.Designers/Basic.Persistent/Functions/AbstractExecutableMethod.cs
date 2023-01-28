using System.Xml;
using Basic.Configuration;
using Basic.Designer;
using Basic.Enums;

namespace Basic.Functions
{
	/// <summary>
	/// 表示基类数据库命令的可执行方法
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	[System.ComponentModel.TypeConverter(typeof(NamespacesConverter))]
	public abstract class AbstractExecutableMethod : AbstractCustomTypeDescriptor
	{
		protected internal readonly DataCommandElement dataCommand;
		/// <summary>
		/// 初始化 AbstractExecutableMethod 类实例。
		/// </summary>
		protected AbstractExecutableMethod(DataCommandElement element) { dataCommand = element; }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return "DataCommand"; }

		#region 类 AbstractExecutableMethod 抽象方法
		/// <summary>
		/// 方法名称
		/// </summary>
		public abstract string MethodName { get; }

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeMethod">表示需要写入代码的方法</param>
		protected internal abstract void WriteCode(System.CodeDom.CodeMemberMethod codeMethod);
		#endregion

		#region 接口 IXmlSerializabl 实现
		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace
		{
			get { return null; }
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader) { return false; }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer) { }

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
		}
		#endregion
	}
}
