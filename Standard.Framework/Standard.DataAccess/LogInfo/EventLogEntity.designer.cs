//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Basic.LogInfo
{
    using Basic.Interfaces;
    using Basic.EntityLayer;
    using Basic.DataAccess;
    using Basic.Enums;
    
    
    #region EventLogCondition Declaration
    /// <summary>
    /// EventLogCondition
    /// </summary>
    [global::System.SerializableAttribute()]
    [global::System.ComponentModel.ToolboxItemAttribute(false)]
    [global::System.Runtime.InteropServices.GuidAttribute("6851FEE9-3829-49FD-B119-239CD8B7A3CB")]
    [global::Basic.EntityLayer.TableMappingAttribute("SYS_EVENTLOG")]
    [global::Basic.EntityLayer.GroupNameAttribute("EventLogEntity", "AccessStrings")]
    public partial class EventLogCondition : global::Basic.EntityLayer.AbstractCondition
    {
        
        private System.Guid m_GuidKey;
        
        private System.Guid m_BatchNo;
        
        private string m_ControllerName;
        
        private string m_ActionName;
        
        private string m_ComputerName;
        
        private string m_LastUser;
        
        private string m_Message;
        
        private string m_Description;
        
        private LogLevel[] m_LogLevel;
        
        private LogResult[] m_ResultType;
        
        private System.DateTime m_BeginDate;
        
        private System.DateTime m_EndDate;
        
        /// <summary>
        /// 初始化 EventLogCondition 类的实例。
        /// </summary>
        public EventLogCondition() : 
                base(12)
        {
        }
        
        /// <summary>
        /// 日志关键字
        /// </summary>
        /// <value>日志关键字</value>
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
                    base.SetBitValue(0);
                    m_GuidKey = value;
                    base.OnPropertyChanged("GuidKey");
                }
            }
        }
        
        /// <summary>
        /// 日志关键字
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasGuidKey
        {
            get
            {
                return base.HasValue(0);
            }
        }
        
        /// <summary>
        /// 日志批次
        /// </summary>
        /// <value>日志批次</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("BATCHNO", DbTypeEnum.Guid, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_BatchNo", "AccessStrings")]
        public System.Guid BatchNo
        {
            get
            {
                return m_BatchNo;
            }
            set
            {
                if ((m_BatchNo != value))
                {
                    base.SetBitValue(1);
                    m_BatchNo = value;
                    base.OnPropertyChanged("BatchNo");
                }
            }
        }
        
        /// <summary>
        /// 日志批次
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasBatchNo
        {
            get
            {
                return base.HasValue(1);
            }
        }
        
        /// <summary>
        /// 页面名称
        /// </summary>
        /// <value>页面名称</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("CONTROLLER", DbTypeEnum.NVarChar, 50, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Controller", "AccessStrings")]
        public string ControllerName
        {
            get
            {
                return m_ControllerName;
            }
            set
            {
                if ((m_ControllerName != value))
                {
                    base.SetBitValue(2);
                    m_ControllerName = value;
                    base.OnPropertyChanged("ControllerName");
                }
            }
        }
        
        /// <summary>
        /// 页面名称
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasControllerName
        {
            get
            {
                return base.HasValue(2);
            }
        }
        
        /// <summary>
        /// 方法名称
        /// </summary>
        /// <value>方法名称</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("ACTION", DbTypeEnum.NVarChar, 50, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Action", "AccessStrings")]
        public string ActionName
        {
            get
            {
                return m_ActionName;
            }
            set
            {
                if ((m_ActionName != value))
                {
                    base.SetBitValue(3);
                    m_ActionName = value;
                    base.OnPropertyChanged("ActionName");
                }
            }
        }
        
        /// <summary>
        /// 方法名称
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasActionName
        {
            get
            {
                return base.HasValue(3);
            }
        }
        
        /// <summary>
        /// 操作计算机
        /// </summary>
        /// <value>操作计算机</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("COMPUTER", DbTypeEnum.NVarChar, 50, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Computer", "AccessStrings")]
        public string ComputerName
        {
            get
            {
                return m_ComputerName;
            }
            set
            {
                if ((m_ComputerName != value))
                {
                    base.SetBitValue(4);
                    m_ComputerName = value;
                    base.OnPropertyChanged("ComputerName");
                }
            }
        }
        
        /// <summary>
        /// 操作计算机
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasComputerName
        {
            get
            {
                return base.HasValue(4);
            }
        }
        
        /// <summary>
        /// 操作用户
        /// </summary>
        /// <value>操作用户</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("USERNAME", DbTypeEnum.NVarChar, 50, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_UserName", "AccessStrings")]
        public string LastUser
        {
            get
            {
                return m_LastUser;
            }
            set
            {
                if ((m_LastUser != value))
                {
                    base.SetBitValue(5);
                    m_LastUser = value;
                    base.OnPropertyChanged("LastUser");
                }
            }
        }
        
        /// <summary>
        /// 操作用户
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasLastUser
        {
            get
            {
                return base.HasValue(5);
            }
        }
        
        /// <summary>
        /// 日志消息，如操作数据的某个字段值
        /// </summary>
        /// <value>日志消息，如操作数据的某个字段值</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("MESSAGE", DbTypeEnum.NVarChar, 200, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Message", "AccessStrings")]
        public string Message
        {
            get
            {
                return m_Message;
            }
            set
            {
                if ((m_Message != value))
                {
                    base.SetBitValue(6);
                    m_Message = value;
                    base.OnPropertyChanged("Message");
                }
            }
        }
        
        /// <summary>
        /// 日志消息，如操作数据的某个字段值
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasMessage
        {
            get
            {
                return base.HasValue(6);
            }
        }
        
        /// <summary>
        /// 描述描述
        /// </summary>
        /// <value>描述描述</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("DESCRIPTION", DbTypeEnum.NVarChar, 2000, true)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Description", "AccessStrings")]
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                if ((m_Description != value))
                {
                    base.SetBitValue(7);
                    m_Description = value;
                    base.OnPropertyChanged("Description");
                }
            }
        }
        
        /// <summary>
        /// 描述描述
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasDescription
        {
            get
            {
                return base.HasValue(7);
            }
        }
        
        /// <summary>
        /// 日志级别
        /// </summary>
        /// <value>日志级别</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("LOGLEVEL", DbTypeEnum.Int32, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_LogLevel", "AccessStrings")]
        public LogLevel[] LogLevel
        {
            get
            {
                return m_LogLevel;
            }
            set
            {
                if ((m_LogLevel != value))
                {
                    base.SetBitValue(8);
                    m_LogLevel = value;
                    base.OnPropertyChanged("LogLevel");
                }
            }
        }
        
        /// <summary>
        /// 日志级别
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasLogLevel
        {
            get
            {
                return base.HasValue(8);
            }
        }
        
        /// <summary>
        /// 操作结果，0表示操作失败，1表示操作成功
        /// </summary>
        /// <value>操作结果，0表示操作失败，1表示操作成功</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("RESULTTYPE", DbTypeEnum.Int32, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_ResultType", "AccessStrings")]
        public LogResult[] ResultType
        {
            get
            {
                return m_ResultType;
            }
            set
            {
                if ((m_ResultType != value))
                {
                    base.SetBitValue(9);
                    m_ResultType = value;
                    base.OnPropertyChanged("ResultType");
                }
            }
        }
        
        /// <summary>
        /// 操作结果，0表示操作失败，1表示操作成功
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasResultType
        {
            get
            {
                return base.HasValue(9);
            }
        }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        /// <value>操作时间</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("BeginDate", DbTypeEnum.DateTime, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_BeginDate", "AccessStrings")]
        public System.DateTime BeginDate
        {
            get
            {
                return m_BeginDate;
            }
            set
            {
                if ((m_BeginDate != value))
                {
                    base.SetBitValue(10);
                    m_BeginDate = value;
                    base.OnPropertyChanged("BeginDate");
                }
            }
        }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasBeginDate
        {
            get
            {
                return base.HasValue(10);
            }
        }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        /// <value>操作时间</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("EndDate", "OPERATIONTIME", DbTypeEnum.DateTime, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_EndDate", "AccessStrings")]
        public System.DateTime EndDate
        {
            get
            {
                return m_EndDate;
            }
            set
            {
                if ((m_EndDate != value))
                {
                    base.SetBitValue(11);
                    m_EndDate = value;
                    base.OnPropertyChanged("EndDate");
                }
            }
        }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        /// <value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>
        public bool HasEndDate
        {
            get
            {
                return base.HasValue(11);
            }
        }
    }
    #endregion
    
    #region EventLogEntity Declaration
    /// <summary>
    /// 记录系统日志
    /// </summary>
    [global::System.SerializableAttribute()]
    [global::System.ComponentModel.ToolboxItemAttribute(false)]
    [global::System.Runtime.InteropServices.GuidAttribute("47ED0FB8-0B6B-4FEF-BA6C-6B099F4D72E4")]
    [global::Basic.EntityLayer.TableMappingAttribute("SYS_EVENTLOG")]
    [global::Basic.EntityLayer.GroupNameAttribute("EventLog", "AccessStrings")]
    public partial class EventLogEntity : global::Basic.EntityLayer.AbstractEntity
    {
        
        private System.Guid m_GuidKey;
        
        private System.Guid m_BatchNo;
        
        private string m_Controller;
        
        private string m_Action;
        
        private string m_Computer;
        
        private string m_UserName;
        
        private string m_Message;
        
        private LogLevel m_LogLevel;
        
        private LogResult m_ResultType;
        
        private System.DateTime m_OperationTime;
        
        /// <summary>
        /// 初始化 EventLogEntity 类的实例。
        /// </summary>
        public EventLogEntity() : 
                base()
        {
        }
        
        /// <summary>
        /// 使用关键字初始化 EventLogEntity 类的实例。
        /// </summary>
        /// <param name="pGuidKey">关键字</param>
        public EventLogEntity(System.Guid pGuidKey) : 
                base()
        {
            this.m_GuidKey = pGuidKey;
        }
        
        /// <summary>
        /// 关键字
        /// </summary>
        /// <value>关键字</value>
        [global::Basic.EntityLayer.PrimaryKeyAttribute()]
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "GUIDKEY", DbTypeEnum.Guid, false)]
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
        
        /// <summary>
        /// 日志批次
        /// </summary>
        /// <value>日志批次</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "BATCHNO", DbTypeEnum.Guid, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_BatchNo", "AccessStrings")]
        public System.Guid BatchNo
        {
            get
            {
                return m_BatchNo;
            }
            set
            {
                if ((m_BatchNo != value))
                {
                    base.OnPropertyChanging("BatchNo");
                    m_BatchNo = value;
                    base.OnPropertyChanged("BatchNo");
                }
            }
        }
        
        /// <summary>
        /// 控制器名称
        /// </summary>
        /// <value>控制器名称</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "CONTROLLER", DbTypeEnum.NVarChar, 50, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Controller", "AccessStrings")]
        public string Controller
        {
            get
            {
                return m_Controller;
            }
            set
            {
                if ((m_Controller != value))
                {
                    base.OnPropertyChanging("Controller");
                    m_Controller = value;
                    base.OnPropertyChanged("Controller");
                }
            }
        }
        
        /// <summary>
        /// 操作名称
        /// </summary>
        /// <value>操作名称</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "ACTION", DbTypeEnum.NVarChar, 50, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Action", "AccessStrings")]
        public string Action
        {
            get
            {
                return m_Action;
            }
            set
            {
                if ((m_Action != value))
                {
                    base.OnPropertyChanging("Action");
                    m_Action = value;
                    base.OnPropertyChanged("Action");
                }
            }
        }
        
        /// <summary>
        /// 计算机名称/IP地址
        /// </summary>
        /// <value>计算机名称/IP地址</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "COMPUTER", DbTypeEnum.NVarChar, 50, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Computer", "AccessStrings")]
        public string Computer
        {
            get
            {
                return m_Computer;
            }
            set
            {
                if ((m_Computer != value))
                {
                    base.OnPropertyChanging("Computer");
                    m_Computer = value;
                    base.OnPropertyChanged("Computer");
                }
            }
        }
        
        /// <summary>
        /// 操作用户名
        /// </summary>
        /// <value>操作用户名</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "USERNAME", DbTypeEnum.NVarChar, 50, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_UserName", "AccessStrings")]
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                if ((m_UserName != value))
                {
                    base.OnPropertyChanging("UserName");
                    m_UserName = value;
                    base.OnPropertyChanged("UserName");
                }
            }
        }
        
        /// <summary>
        /// 操作消息
        /// </summary>
        /// <value>操作消息</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "MESSAGE", DbTypeEnum.NVarChar, 200, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_Message", "AccessStrings")]
        public string Message
        {
            get
            {
                return m_Message;
            }
            set
            {
                if ((m_Message != value))
                {
                    base.OnPropertyChanging("Message");
                    m_Message = value;
                    base.OnPropertyChanged("Message");
                }
            }
        }
        
        /// <summary>
        /// 日志级别
        /// </summary>
        /// <value>日志级别</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "LOGLEVEL", DbTypeEnum.Int32, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_LogLevel", "AccessStrings")]
        public LogLevel LogLevel
        {
            get
            {
                return m_LogLevel;
            }
            set
            {
                if ((m_LogLevel != value))
                {
                    base.OnPropertyChanging("LogLevel");
                    m_LogLevel = value;
                    base.OnPropertyChanged("LogLevel");
                }
            }
        }
        
        /// <summary>
        /// 操作结果
        /// </summary>
        /// <value>操作结果</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "RESULTTYPE", DbTypeEnum.Int32, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_ResultType", "AccessStrings")]
        public LogResult ResultType
        {
            get
            {
                return m_ResultType;
            }
            set
            {
                if ((m_ResultType != value))
                {
                    base.OnPropertyChanging("ResultType");
                    m_ResultType = value;
                    base.OnPropertyChanged("ResultType");
                }
            }
        }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        /// <value>操作时间</value>
        [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "OPERATIONTIME", DbTypeEnum.DateTime, false)]
        [global::Basic.EntityLayer.WebDisplayAttribute("EventLog_OperationTime", "AccessStrings")]
        [global::System.ComponentModel.DataAnnotations.DisplayFormatAttribute(DataFormatString="{0:yyyy-MM-dd HH:mm:ss.fff}")]
        public System.DateTime OperationTime
        {
            get
            {
                return m_OperationTime;
            }
            set
            {
                if ((m_OperationTime != value))
                {
                    base.OnPropertyChanging("OperationTime");
                    m_OperationTime = value;
                    base.OnPropertyChanged("OperationTime");
                }
            }
        }
    }
    #endregion
    
    #region EventLogTable Declaration
    /// <summary>
    /// 记录系统日志
    /// </summary>
    [global::System.SerializableAttribute()]
    [global::System.ComponentModel.ToolboxItemAttribute(false)]
    public partial class EventLogTable : global::Basic.Tables.BaseTableType<EventLogTable.EventLogRow>
    {
        
        private global::System.Data.DataColumn columnGuidKey;
        
        private global::System.Data.DataColumn columnBatchNo;
        
        private global::System.Data.DataColumn columnController;
        
        private global::System.Data.DataColumn columnAction;
        
        private global::System.Data.DataColumn columnComputer;
        
        private global::System.Data.DataColumn columnUserName;
        
        private global::System.Data.DataColumn columnMessage;
        
        private global::System.Data.DataColumn columnLogLevel;
        
        private global::System.Data.DataColumn columnResultType;
        
        private global::System.Data.DataColumn columnOperationTime;
        
        /// <summary>
        /// 初始化类实例。
        /// </summary>
		partial void CustomInitClass();

        /// <summary>
        /// 初始化列信息。
        /// </summary>
		partial void CustomInitColumns();
        
        /// <summary>
        /// 初始化 EventLogRow 类的实例。
        /// </summary>
        public EventLogTable() : 
                base("SYS_EVENTLOG")
        {
        }
        
        /// <summary>
        /// 关键字
        /// </summary>
        public global::System.Data.DataColumn CGuidKey
        {
            get
            {
                return columnGuidKey;
            }
        }
        
        /// <summary>
        /// 日志批次
        /// </summary>
        public global::System.Data.DataColumn CBatchNo
        {
            get
            {
                return columnBatchNo;
            }
        }
        
        /// <summary>
        /// 控制器名称
        /// </summary>
        public global::System.Data.DataColumn CController
        {
            get
            {
                return columnController;
            }
        }
        
        /// <summary>
        /// 操作名称
        /// </summary>
        public global::System.Data.DataColumn CAction
        {
            get
            {
                return columnAction;
            }
        }
        
        /// <summary>
        /// 计算机名称/IP地址
        /// </summary>
        public global::System.Data.DataColumn CComputer
        {
            get
            {
                return columnComputer;
            }
        }
        
        /// <summary>
        /// 操作用户名
        /// </summary>
        public global::System.Data.DataColumn CUserName
        {
            get
            {
                return columnUserName;
            }
        }
        
        /// <summary>
        /// 操作消息
        /// </summary>
        public global::System.Data.DataColumn CMessage
        {
            get
            {
                return columnMessage;
            }
        }
        
        /// <summary>
        /// 日志级别
        /// </summary>
        public global::System.Data.DataColumn CLogLevel
        {
            get
            {
                return columnLogLevel;
            }
        }
        
        /// <summary>
        /// 操作结果
        /// </summary>
        public global::System.Data.DataColumn CResultType
        {
            get
            {
                return columnResultType;
            }
        }
        
        /// <summary>
        /// 操作时间
        /// </summary>
        public global::System.Data.DataColumn COperationTime
        {
            get
            {
                return columnOperationTime;
            }
        }
        
        /// <summary>
        /// 初始化类实例。
        /// </summary>
        protected override void InitClass()
        {
            this.columnGuidKey = new global::System.Data.DataColumn("GUIDKEY", typeof(System.Guid), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnGuidKey);
            this.columnBatchNo = new global::System.Data.DataColumn("BATCHNO", typeof(System.Guid), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnBatchNo);
            this.columnController = new global::System.Data.DataColumn("CONTROLLER", typeof(string), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnController);
            this.columnAction = new global::System.Data.DataColumn("ACTION", typeof(string), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnAction);
            this.columnComputer = new global::System.Data.DataColumn("COMPUTER", typeof(string), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnComputer);
            this.columnUserName = new global::System.Data.DataColumn("USERNAME", typeof(string), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnUserName);
            this.columnMessage = new global::System.Data.DataColumn("MESSAGE", typeof(string), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnMessage);
            this.columnLogLevel = new global::System.Data.DataColumn("LOGLEVEL", typeof(LogLevel), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnLogLevel);
            this.columnResultType = new global::System.Data.DataColumn("RESULTTYPE", typeof(LogResult), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnResultType);
            this.columnOperationTime = new global::System.Data.DataColumn("OPERATIONTIME", typeof(System.DateTime), null, System.Data.MappingType.Element);
            base.Columns.Add(this.columnOperationTime);
            base.Constraints.Add(new global::System.Data.UniqueConstraint("PK_SYS_EVENTLOG", new global::System.Data.DataColumn[] {
                            columnGuidKey}, true));
            this.CustomInitClass();
        }
        
        /// <summary>
        /// 初始化列信息。
        /// </summary>
        protected override void InitColumns()
        {
            this.columnGuidKey = base.Columns["GUIDKEY"];
            this.columnBatchNo = base.Columns["BATCHNO"];
            this.columnController = base.Columns["CONTROLLER"];
            this.columnAction = base.Columns["ACTION"];
            this.columnComputer = base.Columns["COMPUTER"];
            this.columnUserName = base.Columns["USERNAME"];
            this.columnMessage = base.Columns["MESSAGE"];
            this.columnLogLevel = base.Columns["LOGLEVEL"];
            this.columnResultType = base.Columns["RESULTTYPE"];
            this.columnOperationTime = base.Columns["OPERATIONTIME"];
            this.CustomInitColumns();
        }
        
        /// <summary>
        /// 克隆 EventLogTable 的结构，包括所有 EventLogTable 架构和约束。
        /// </summary>
        /// <returns>新的 EventLogTable，与当前的 EventLogTable 具有相同的架构。</returns>
        public override global::System.Data.DataTable Clone()
        {
            EventLogTable cloneTable = ((EventLogTable)(base.Clone()));
            cloneTable.InitColumns();
            return cloneTable;
        }
        
        /// <summary>
        /// 创建 EventLogTable 类实例。
        /// </summary>
        /// <returns>新的 EventLogTable 与当前的 EventLogTable 具有相同的架构。</returns>
        protected override global::System.Data.DataTable CreateInstance()
        {
            return new EventLogTable();
        }
        
        /// <summary>
        /// 从现有的行创建新行。
        /// </summary>
        /// <returns>新的 EventLogTable 与当前的 EventLogTable 具有相同的架构。</returns>
        protected override global::System.Data.DataRow NewRowFromBuilder(global::System.Data.DataRowBuilder builder)
        {
            return new EventLogRow(builder);
        }
        
        /// <summary>
        /// 获取包含指定的主键值的行
        /// </summary>
        /// <param name="guidKey">关键字</param>
        /// <returns>包含指定的主键值的 EventLogRow 对象的数组；否则为空值（如果 EventLogTable 中不存在主键值）。</returns>
        public EventLogRow FindByKey(System.Guid guidKey)
        {
            return base.FindByKey(guidKey);
        }
        
        #region EventLogRow Declaration
        /// <summary>
        /// 记录系统日志
        /// </summary>
        public partial class EventLogRow : global::Basic.Tables.BaseTableRowType
        {
            
            /// <summary>
            /// 具有此行架构的 EventLogTable 类实例。
            /// </summary>
            private EventLogTable tableEventLog;
            
            /// <summary>
            /// 初始化 EventLogRow 类的实例。
            /// </summary>
            internal EventLogRow(System.Data.DataRowBuilder rb) : 
                    base(rb)
            {
                tableEventLog = ((EventLogTable)(base.Table));
            }
            
            /// <summary>
            /// 关键字
            /// </summary>
            [global::Basic.EntityLayer.PrimaryKeyAttribute()]
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "GUIDKEY", DbTypeEnum.Guid, false)]
            public System.Guid GuidKey
            {
                get
                {
                    return ((System.Guid)(this[tableEventLog.CGuidKey]));
                }
                set
                {
                    this[tableEventLog.CGuidKey] = value;
                }
            }
            
            /// <summary>
            /// 日志批次
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "BATCHNO", DbTypeEnum.Guid, false)]
            public System.Guid BatchNo
            {
                get
                {
                    return ((System.Guid)(this[tableEventLog.CBatchNo]));
                }
                set
                {
                    this[tableEventLog.CBatchNo] = value;
                }
            }
            
            /// <summary>
            /// 控制器名称
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "CONTROLLER", DbTypeEnum.NVarChar, 50, false)]
            public string Controller
            {
                get
                {
                    return ((string)(this[tableEventLog.CController]));
                }
                set
                {
                    this[tableEventLog.CController] = value;
                }
            }
            
            /// <summary>
            /// 操作名称
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "ACTION", DbTypeEnum.NVarChar, 50, false)]
            public string Action
            {
                get
                {
                    return ((string)(this[tableEventLog.CAction]));
                }
                set
                {
                    this[tableEventLog.CAction] = value;
                }
            }
            
            /// <summary>
            /// 计算机名称/IP地址
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "COMPUTER", DbTypeEnum.NVarChar, 50, false)]
            public string Computer
            {
                get
                {
                    return ((string)(this[tableEventLog.CComputer]));
                }
                set
                {
                    this[tableEventLog.CComputer] = value;
                }
            }
            
            /// <summary>
            /// 操作用户名
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "USERNAME", DbTypeEnum.NVarChar, 50, false)]
            public string UserName
            {
                get
                {
                    return ((string)(this[tableEventLog.CUserName]));
                }
                set
                {
                    this[tableEventLog.CUserName] = value;
                }
            }
            
            /// <summary>
            /// 操作消息
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "MESSAGE", DbTypeEnum.NVarChar, 200, false)]
            public string Message
            {
                get
                {
                    return ((string)(this[tableEventLog.CMessage]));
                }
                set
                {
                    this[tableEventLog.CMessage] = value;
                }
            }
            
            /// <summary>
            /// 日志级别
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "LOGLEVEL", DbTypeEnum.Int32, false)]
            public LogLevel LogLevel
            {
                get
                {
                    return ((LogLevel)(this[tableEventLog.CLogLevel]));
                }
                set
                {
                    this[tableEventLog.CLogLevel] = value;
                }
            }
            
            /// <summary>
            /// 操作结果
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "RESULTTYPE", DbTypeEnum.Int32, false)]
            public LogResult ResultType
            {
                get
                {
                    return ((LogResult)(this[tableEventLog.CResultType]));
                }
                set
                {
                    this[tableEventLog.CResultType] = value;
                }
            }
            
            /// <summary>
            /// 操作时间
            /// </summary>
            [global::Basic.EntityLayer.ColumnMappingAttribute("T1", "OPERATIONTIME", DbTypeEnum.DateTime, false)]
            public System.DateTime OperationTime
            {
                get
                {
                    return ((System.DateTime)(this[tableEventLog.COperationTime]));
                }
                set
                {
                    this[tableEventLog.COperationTime] = value;
                }
            }
        }
        #endregion
    }
    #endregion
    
    #region EventLogDelEntity Declaration
    /// <summary>
    /// 记录系统日志
    /// </summary>
    [global::System.SerializableAttribute()]
    [global::System.ComponentModel.ToolboxItemAttribute(false)]
    [global::System.Runtime.InteropServices.GuidAttribute("E5BE7F82-1551-4E7C-9233-FA908821AC5B")]
    [global::Basic.EntityLayer.TableMappingAttribute("SYS_EVENTLOG")]
    [global::Basic.EntityLayer.GroupNameAttribute("EventLog", "AccessStrings")]
    public partial class EventLogDelEntity : global::Basic.EntityLayer.AbstractEntity
    {
        
        private System.Guid m_GuidKey;
        
        /// <summary>
        /// 初始化 EventLogDelEntity 类的实例。
        /// </summary>
        public EventLogDelEntity() : 
                base()
        {
        }
        
        /// <summary>
        /// 使用关键字初始化 EventLogDelEntity 类的实例。
        /// </summary>
        /// <param name="pGuidKey">关键字</param>
        public EventLogDelEntity(System.Guid pGuidKey) : 
                base()
        {
            this.m_GuidKey = pGuidKey;
        }
        
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
