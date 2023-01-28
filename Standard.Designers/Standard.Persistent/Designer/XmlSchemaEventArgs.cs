using System.Xml.Schema;
namespace Basic.Designer
{
	/// <summary>
	/// 表示将处理 XML 架构验证事件和 ValidationEventArgs 的回调方法。
	/// </summary>
	/// <param name="sender">事件源。</param>
	/// <param name="e">类型为 XmlSchemaEventArgs 的事件数据。</param>
	public delegate void XmlSchemaEventHandler(object sender, XmlSchemaEventArgs e);

	/// <summary>
	/// 返回与 XmlSchemaEventHandler 相关的详细信息。
	/// </summary>
	public sealed class XmlSchemaEventArgs : System.EventArgs
	{
		/// <summary>
		/// 初始化 XmlSchemaEventArgs 类实例
		/// </summary>
		/// <param name="ex">与事件关联的 XmlSchemaException 类实例。</param>
		public XmlSchemaEventArgs(XmlSchemaException ex) : this(ex, XmlSeverityType.Error) { }

		/// <summary>
		/// 初始化 XmlSchemaEventArgs 类实例
		/// </summary>
		/// <param name="ex">与事件关联的 XmlSchemaException 类实例。</param>
		/// <param name="severity">事件的严重度。</param>
		public XmlSchemaEventArgs(XmlSchemaException ex, XmlSeverityType severity)
		{
			this.Exception = ex;
			this.Severity = severity;
		}

		/// <summary>
		/// 获取与该验证事件关联的 XmlSchemaException。
		/// </summary>
		public XmlSchemaException Exception { get; private set; }

		/// <summary>
		/// 获取与验证事件对应的文本说明。
		/// </summary>
		public string Message { get { return Exception.Message; } }

		/// <summary>
		/// 获取验证事件的严重度。
		/// </summary>
		public XmlSeverityType Severity { get; private set; }
	}
}
