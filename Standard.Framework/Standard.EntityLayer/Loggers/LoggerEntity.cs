using Basic.Enums;

namespace Basic.Loggers
{
	/// <summary>记录系统日志</summary>
	[global::System.Runtime.InteropServices.GuidAttribute("33ECA939-0B5F-46E0-89B8-7734ABFC200F")]
	[global::Basic.EntityLayer.TableMappingAttribute("SYS_EVENTLOG")]
	[global::Basic.EntityLayer.GroupNameAttribute("EventLog", "AccessStrings")]
	public partial class LoggerEntity : global::Basic.EntityLayer.AbstractEntity
	{
		/// <summary>初始化 LoggerEntity 类的实例。</summary>
		public LoggerEntity() : base() { }

		/// <summary>初始化 LoggerEntity 类的实例。</summary>
		public LoggerEntity(System.Guid guid) : base() { GuidKey = guid; }

		/// <summary>关键字</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "GUIDKEY", DbTypeEnum.Guid, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_GuidKey", "AccessStrings")]
		public System.Guid GuidKey { get; set; }

		/// <summary>日志批次</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "BATCHNO", DbTypeEnum.Guid, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_BatchNo", "AccessStrings")]
		public System.Guid BatchNo { get; set; }

		/// <summary>控制器名称</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "CONTROLLER", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Controller", "AccessStrings")]
		public string Controller { get; set; }

		/// <summary>操作名称</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "ACTION", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Action", "AccessStrings")]
		public string Action { get; set; }

		/// <summary></summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_ControllerAction", "AccessStrings")]
		public string ControllerAction { get { return Controller == Action ? "Controller" : string.Concat(Controller, "/", Action); } }

		/// <summary>计算机名称/IP地址</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "COMPUTER", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Computer", "AccessStrings")]
		public string Computer { get; set; }

		/// <summary>操作用户名</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "USERNAME", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_UserName", "AccessStrings")]
		public string UserName { get; set; }

		/// <summary>操作消息</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "MESSAGE", DbTypeEnum.NVarChar, 200, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Message", "AccessStrings")]
		public string Message { get; set; }

		/// <summary>日志级别</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "LOGLEVEL", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_LogLevel", "AccessStrings")]
		public LogLevel LogLevel { get; set; }

		/// <summary>操作结果</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "RESULTTYPE", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_ResultType", "AccessStrings")]
		public LogResult ResultType { get; set; }

		/// <summary>操作时间</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "OPERATIONTIME", DbTypeEnum.DateTime, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_OperationTime", "AccessStrings")]
		[global::System.ComponentModel.DataAnnotations.DisplayFormatAttribute(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss.fff}")]
		public System.DateTime OperationTime { get; set; }
	}

	/// <summary>日志查询条件</summary>
	[global::System.Runtime.InteropServices.GuidAttribute("8CCB6352-246A-498D-94CA-795E634677C1")]
	[global::Basic.EntityLayer.TableMappingAttribute("SYS_EVENTLOG")]
	[global::Basic.EntityLayer.GroupNameAttribute("EventLogEntity", "AccessStrings")]
	public partial class LoggerCondition : global::Basic.EntityLayer.AbstractCondition
	{

		/// <summary>
		/// 初始化 LoggerCondition 类的实例。
		/// </summary>
		public LoggerCondition() : base(12) { }

		/// <summary>日志批次</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_BatchNo", "AccessStrings")]
		public System.Guid? BatchNo { get; set; }

		/// <summary>页面名称</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Controller", "AccessStrings")]
		public string ControllerName { get; set; }

		/// <summary>方法名称</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Action", "AccessStrings")]
		public string ActionName { get; set; }

		/// <summary>操作计算机</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Computer", "AccessStrings")]
		public string ComputerName { get; set; }

		/// <summary>操作用户</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_UserName", "AccessStrings")]
		public string LastUser { get; set; }

		/// <summary>日志消息，如操作数据的某个字段值</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Message", "AccessStrings")]
		public string Message { get; set; }

		/// <summary>日志级别</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_LogLevel", "AccessStrings")]
		public LogLevel[] LogLevel { get; set; }

		/// <summary>操作结果，0表示操作失败，1表示操作成功</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_ResultType", "AccessStrings")]
		public LogResult[] ResultType { get; set; }

		/// <summary>操作时间</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_BeginDate", "AccessStrings")]
		public System.DateTime BeginDate { get; set; }

		/// <summary>操作时间</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_EndDate", "AccessStrings")]
		public System.DateTime EndDate { get; set; }
	}

	#region EventLogDelEntity Declaration
	/// <summary>
	/// 记录系统日志
	/// </summary>
	[global::System.SerializableAttribute()]
	[global::System.ComponentModel.ToolboxItemAttribute(false)]
	[global::System.Runtime.InteropServices.GuidAttribute("E5BE7F82-1551-4E7C-9233-FA908821AC5B")]
	[global::Basic.EntityLayer.TableMappingAttribute("SYS_EVENTLOG")]
	[global::Basic.EntityLayer.GroupNameAttribute("EventLog", "AccessStrings")]
	public partial class LoggerDelEntity : global::Basic.EntityLayer.AbstractEntity
	{

		private System.Guid m_GuidKey;

		/// <summary>
		/// 初始化 EventLogDelEntity 类的实例。
		/// </summary>
		public LoggerDelEntity() : base() { }

		/// <summary>
		/// 使用关键字初始化 EventLogDelEntity 类的实例。
		/// </summary>
		/// <param name="pGuidKey">关键字</param>
		public LoggerDelEntity(System.Guid pGuidKey) : base() { this.m_GuidKey = pGuidKey; }

		/// <summary>
		/// 关键字
		/// </summary>
		/// <value>关键字</value>
		[global::Basic.EntityLayer.PrimaryKeyAttribute()]
		[global::Basic.EntityLayer.ColumnMappingAttribute("GUIDKEY", DbTypeEnum.Guid, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EventLog_GuidKey", "AccessStrings")]
		public System.Guid GuidKey
		{
			get
			{
				return m_GuidKey;
			}
			set
			{
				if ((m_GuidKey != value))
				{
					base.OnPropertyChanging("GuidKey");
					m_GuidKey = value;
					base.OnPropertyChanged("GuidKey");
				}
			}
		}
	}
	#endregion
}
