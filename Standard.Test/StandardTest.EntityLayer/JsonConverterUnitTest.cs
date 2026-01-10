using System.ComponentModel;
using Basic.EntityLayer;
using Basic.Enums;

namespace Standard.EntityLayer.Test
{
	[TestClass]
	public class JsonConverterUnitTest
	{
		[TestMethod]
		public void SystemSerializeCollectionPropertiesFull()
		{
			JsonConverter converter = new JsonConverter(System.Globalization.CultureInfo.CurrentUICulture);
			List<DailyResultEntity> list = new List<DailyResultEntity>(5);
			DailyResultEntity item = new DailyResultEntity()
			{
				EmpKey = 1,
				DepartCode = "0101",
				AllowShift = true,
				GeneralTiming = false,
				ClassKey = 2,
				ShortName = "D",
				ClassName = "白班",
				WorkHour = 8,
			};
			item.Dailies.Add("D0807", new ClassCalencar("D8", CalendarTypes.Holiday));
			item.Dailies.Add("D0806", new ClassCalencar("D8", CalendarTypes.Weekend));
			item.Dailies.Add("D0808", new ClassCalencar("D8", CalendarTypes.AllDay));
			item.Results.Add("R1004", 175M);
			item.Results.Add("R1006", 15201.05M);
			item.Results.Add("R1005", 58.26M);
			list.Add(item);
			string result = System.Text.Json.JsonSerializer.Serialize(list);
			Console.WriteLine(result);
			List<DailyResultEntity> list1 = System.Text.Json.JsonSerializer.Deserialize<List<DailyResultEntity>>(result);
			Assert.IsFalse(list1.Count == 0, result);
		}

		[TestMethod]
		public void SerializeDailySummaryEntityEmpty()
		{
			JsonConverter converter = new JsonConverter(System.Globalization.CultureInfo.CurrentUICulture);
			List<DailySummaryEntity> list = new List<DailySummaryEntity>(5);
			DailySummaryEntity item = new DailySummaryEntity()
			{
				ShiftDate = new DateTime(2025, 08, 07),
				CalendarType = CalendarTypes.AllDay,
				ClassType = ClassTypes.TimingClass,
				PeopleCount = 7,
				ClassKey = 203,
				ShortName = "D",
				ClassName = "夜班",
				CreatedTime = DateTime.Parse("2025-08-11 09:26:59"),
				ModifiedTime = DateTime.Parse("2025-08-11 09:42:58.603")
				//WorkHour = 8,
			};
			list.Add(item);
			string result = converter.Serialize(list, true);
			Console.WriteLine(result);
			List<DailySummaryEntity> list1 = System.Text.Json.JsonSerializer.Deserialize<List<DailySummaryEntity>>(result);
			Assert.IsFalse(list1.Count == 0, result);
		}

		[TestMethod]
		public void SerializeDailySummaryEntityFull()
		{
			JsonConverter converter = new JsonConverter(System.Globalization.CultureInfo.CurrentUICulture);
			List<DailySummaryEntity> list = new List<DailySummaryEntity>(5);
			DailySummaryEntity item = new DailySummaryEntity()
			{
				ShiftDate = new DateTime(2025, 08, 07),
				CalendarType = CalendarTypes.AllDay,
				ClassType = ClassTypes.TimingClass,
				PeopleCount = 7,
				ClassKey = 203,
				ShortName = "D",
				ClassName = "夜班",
				CreatedTime = DateTime.Parse("2025-08-11 09:26:59"),
				ModifiedTime = DateTime.Parse("2025-08-11 09:42:58.603")
				//WorkHour = 8,
			};
			item.Items["CM1"] = 23M;
			list.Add(item);
			string result = converter.Serialize(list, true);
			Console.WriteLine(result);
			List<DailySummaryEntity> list1 = System.Text.Json.JsonSerializer.Deserialize<List<DailySummaryEntity>>(result);
			Assert.IsFalse(list1.Count == 0, result);
		}

		[TestMethod]
		public void SerializeCollectionPropertiesEmpty()
		{
			JsonConverter converter = new JsonConverter(System.Globalization.CultureInfo.CurrentUICulture);
			List<DailyResultEntity> list = new List<DailyResultEntity>(5);
			DailyResultEntity item = new DailyResultEntity()
			{
				EmpKey = 1,
				DepartCode = "0101",
				AllowShift = true,
				GeneralTiming = false,
				ClassKey = 2,
				ShortName = "D",
				ClassName = "白班",
				WorkHour = 8,
			};
			list.Add(item);
			string result = converter.Serialize(list, true);
			Console.WriteLine(result);
			List<DailyResultEntity> list1 = System.Text.Json.JsonSerializer.Deserialize<List<DailyResultEntity>>(result);
			Assert.IsFalse(list1.Count == 0, result);
		}

		[TestMethod]
		public void SerializeCollectionPropertiesFull()
		{
			JsonConverter converter = new JsonConverter(System.Globalization.CultureInfo.CurrentUICulture);
			List<DailyResultEntity> list = new List<DailyResultEntity>(5);
			DailyResultEntity item = new DailyResultEntity()
			{
				EmpKey = 1,
				DepartCode = "0101",
				AllowShift = true,
				GeneralTiming = false,
				ClassKey = 2,
				ShortName = "D",
				ClassName = "白班",
				WorkHour = 8,
			};
			item.Dailies.Add("D0807", new ClassCalencar("D8", CalendarTypes.Holiday));
			item.Dailies.Add("D0806", new ClassCalencar("D8", CalendarTypes.Weekend));
			item.Dailies.Add("D0808", new ClassCalencar("D8", CalendarTypes.AllDay));
			item.Results.Add("R1004", 175M);
			item.Results.Add("R1006", 15201.05M);
			item.Results.Add("R1005", 58.26M);
			list.Add(item);
			string result = converter.Serialize(list, true);
			Console.WriteLine(result);
			List<DailyResultEntity> list1 = System.Text.Json.JsonSerializer.Deserialize<List<DailyResultEntity>>(result);
			Assert.IsFalse(list1.Count == 0, result);
		}
		[TestMethod]
		public void SerializeCollectionPropertiesOneEmpty1()
		{
			JsonConverter converter = new JsonConverter(System.Globalization.CultureInfo.CurrentUICulture);
			List<DailyResultEntity> list = new List<DailyResultEntity>(5);
			DailyResultEntity item = new DailyResultEntity()
			{
				EmpKey = 1,
				DepartCode = "0101",
				AllowShift = true,
				GeneralTiming = false,
				ClassKey = 2,
				ShortName = "D",
				ClassName = "白班",
				WorkHour = 8,
			};
			item.Results.Add("R1004", 175M);
			item.Results.Add("R1006", 15201.05M);
			item.Results.Add("R1005", 58.26M);
			list.Add(item);
			string result = converter.Serialize(list, true);
			Console.WriteLine(result);
			List<DailyResultEntity> list1 = System.Text.Json.JsonSerializer.Deserialize<List<DailyResultEntity>>(result);
			Assert.IsFalse(list1.Count == 0, result);
		}
		[TestMethod]
		public void SerializeCollectionPropertiesOneEmpty2()
		{
			JsonConverter converter = new JsonConverter(System.Globalization.CultureInfo.CurrentUICulture);
			List<DailyResultEntity> list = new List<DailyResultEntity>(5);
			DailyResultEntity item = new DailyResultEntity()
			{
				EmpKey = 1,
				DepartCode = "0101",
				AllowShift = true,
				GeneralTiming = false,
				ClassKey = 2,
				ShortName = "D",
				ClassName = "白班",
				WorkHour = 8,
			};
			item.Dailies.Add("D0807", new ClassCalencar("D8", CalendarTypes.Holiday));
			item.Dailies.Add("D0806", new ClassCalencar("D8", CalendarTypes.Weekend));
			item.Dailies.Add("D0808", new ClassCalencar("D8", CalendarTypes.AllDay));
			list.Add(item);
			string result = converter.Serialize(list, true);
			Console.WriteLine(result);
			List<DailyResultEntity> list1 = System.Text.Json.JsonSerializer.Deserialize<List<DailyResultEntity>>(result);
			Assert.IsFalse(list1.Count == 0, result);
		}
	}


	#region DailySummaryEntity Declaration
	[DefaultValue(1)]
	public enum ClassTypes
	{
		//
		// 摘要:
		//     休息班别
		RestClass = 1,
		//
		// 摘要:
		//     定时班别
		TimingClass,
		//
		// 摘要:
		//     不定时班别
		TimeToTime
	}
	/// <summary>
	/// 每日汇总数据
	/// </summary>
	[global::System.SerializableAttribute()]
	[global::System.ComponentModel.ToolboxItemAttribute(false)]
	[global::System.Runtime.InteropServices.GuidAttribute("770AA627-D1E4-489E-AB02-5E3E34BD61DA")]
	[global::Basic.EntityLayer.TableMappingAttribute("PSM_DAILYSUMMARY")]
	[global::Basic.EntityLayer.GroupNameAttribute("DailySummary", "SalaryStrings")]
	public partial class DailySummaryEntity : global::Basic.EntityLayer.AbstractEntity
	{

		/// <summary></summary>
		[Basic.EntityLayer.PropertyCollection]
		public Dictionary<string, decimal> Items { get { return _Workloads; } }

		private Dictionary<string, decimal> _Workloads = new Dictionary<string, decimal>(100);
		/// <summary></summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetValue(string name, decimal value)
		{
			if (_Workloads.ContainsKey(name)) { _Workloads[name] = value; }
			else { _Workloads.Add(name, value); }
		}

		private System.DateTime m_ShiftDate;

		private int m_ClassKey;

		private ClassTypes m_ClassType;

		private string m_ClassCode;

		private string m_ShortName;

		private string m_ClassName;

		private CalendarTypes m_CalendarType;

		private decimal m_TotalTonne;

		private decimal m_TotalTimes;

		private decimal m_TotalNumber;

		private decimal m_PeopleCount;

		private decimal m_CostSubtotal;

		private decimal m_CostAverage;

		private string m_UserName;

		private System.DateTime m_CreatedTime;

		private System.DateTime m_ModifiedTime;

		/// <summary>
		/// 初始化类实例。
		/// </summary>
		partial void InitializationClass();


		/// <summary>
		/// 初始化 DailySummaryEntity 类的实例。
		/// </summary>
		public DailySummaryEntity() :
				base()
		{
			this.InitializationClass();
		}

		/// <summary>
		/// 使用关键字初始化 DailySummaryEntity 类的实例。
		/// </summary>
		/// <param name="pShiftDate">排班日期</param>
		/// <param name="pClassKey">出勤班别</param>
		public DailySummaryEntity(System.DateTime pShiftDate, int pClassKey) :
				base()
		{
			this.m_ShiftDate = pShiftDate;
			this.m_ClassKey = pClassKey;
			this.InitializationClass();
		}

		/// <summary>
		/// 排班日期
		/// </summary>
		/// <value>排班日期</value>
		[global::Basic.EntityLayer.PrimaryKeyAttribute()]
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "SHIFTDATE", DbTypeEnum.Date, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_ShiftDate", "SalaryStrings")]
		[global::System.ComponentModel.DataAnnotations.DisplayFormatAttribute(DataFormatString = "{0:yyyy-MM-dd}")]
		public System.DateTime ShiftDate
		{
			get
			{
				return m_ShiftDate;
			}
			set
			{
				if ((m_ShiftDate != value))
				{
					base.OnPropertyChanging("ShiftDate");
					m_ShiftDate = value;
					base.OnPropertyChanged("ShiftDate");
				}
			}
		}

		/// <summary>
		/// 出勤班别
		/// </summary>
		/// <value>出勤班别</value>
		[global::Basic.EntityLayer.PrimaryKeyAttribute()]
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "CLASSKEY", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_ClassKey", "SalaryStrings")]
		public int ClassKey
		{
			get
			{
				return m_ClassKey;
			}
			set
			{
				if ((m_ClassKey != value))
				{
					base.OnPropertyChanging("ClassKey");
					m_ClassKey = value;
					base.OnPropertyChanged("ClassKey");
				}
			}
		}

		/// <summary>
		/// 班别类型1:休息,2:定时,3:不定时
		/// </summary>
		/// <value>班别类型1:休息,2:定时,3:不定时</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "CLASSTYPE", DbTypeEnum.Int16, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_ClassType", "SalaryStrings")]
		public ClassTypes ClassType
		{
			get
			{
				return m_ClassType;
			}
			set
			{
				if ((m_ClassType != value))
				{
					base.OnPropertyChanging("ClassType");
					m_ClassType = value;
					base.OnPropertyChanged("ClassType");
				}
			}
		}

		/// <summary>
		/// 班别代码
		/// </summary>
		/// <value>班别代码</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "CLASSCODE", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_ClassCode", "SalaryStrings")]
		public string ClassCode
		{
			get
			{
				return m_ClassCode;
			}
			set
			{
				if ((m_ClassCode != value))
				{
					base.OnPropertyChanging("ClassCode");
					m_ClassCode = value;
					base.OnPropertyChanged("ClassCode");
				}
			}
		}

		/// <summary>
		/// Property: ShortName
		/// </summary>
		/// <value>属性 ShortName 的值</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "SHORTNAME", DbTypeEnum.NVarChar, 10, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_ShortName", "SalaryStrings")]
		public string ShortName
		{
			get
			{
				return m_ShortName;
			}
			set
			{
				if ((m_ShortName != value))
				{
					base.OnPropertyChanging("ShortName");
					m_ShortName = value;
					base.OnPropertyChanged("ShortName");
				}
			}
		}

		/// <summary>
		/// 班别名称
		/// </summary>
		/// <value>班别名称</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "CLASSNAME", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_ClassName", "SalaryStrings")]
		public string ClassName
		{
			get
			{
				return m_ClassName;
			}
			set
			{
				if ((m_ClassName != value))
				{
					base.OnPropertyChanging("ClassName");
					m_ClassName = value;
					base.OnPropertyChanged("ClassName");
				}
			}
		}

		/// <summary>
		/// 行事历类型
		/// </summary>
		/// <value>行事历类型</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "CALENDARTYPE", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("EmployeeSchedule_CalendarType", "SalaryStrings")]
		public CalendarTypes CalendarType
		{
			get
			{
				return m_CalendarType;
			}
			set
			{
				if ((m_CalendarType != value))
				{
					base.OnPropertyChanging("CalendarType");
					m_CalendarType = value;
					base.OnPropertyChanged("CalendarType");
				}
			}
		}

		/// <summary>
		/// 总吨位
		/// </summary>
		/// <value>总吨位</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute(null, "TOTALTONNE", DbTypeEnum.Decimal, 18, 4, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_TotalTonne", "SalaryStrings")]
		public decimal TotalTonne
		{
			get
			{
				return m_TotalTonne;
			}
			set
			{
				if ((m_TotalTonne != value))
				{
					base.OnPropertyChanging("TotalTonne");
					m_TotalTonne = value;
					base.OnPropertyChanged("TotalTonne");
				}
			}
		}

		/// <summary>
		/// 总次数
		/// </summary>
		/// <value>总次数</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute(null, "TOTALTIMES", DbTypeEnum.Decimal, 18, 4, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_TotalTimes", "SalaryStrings")]
		public decimal TotalTimes
		{
			get
			{
				return m_TotalTimes;
			}
			set
			{
				if ((m_TotalTimes != value))
				{
					base.OnPropertyChanging("TotalTimes");
					m_TotalTimes = value;
					base.OnPropertyChanged("TotalTimes");
				}
			}
		}

		/// <summary>
		/// 总只数
		/// </summary>
		/// <value>总只数</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "TOTALNUMBER", DbTypeEnum.Decimal, 18, 4, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_TotalNumber", "SalaryStrings")]
		public decimal TotalNumber
		{
			get
			{
				return m_TotalNumber;
			}
			set
			{
				if ((m_TotalNumber != value))
				{
					base.OnPropertyChanging("TotalNumber");
					m_TotalNumber = value;
					base.OnPropertyChanged("TotalNumber");
				}
			}
		}

		/// <summary>
		/// 出勤人数
		/// </summary>
		/// <value>出勤人数</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "PEOPLECOUNT", DbTypeEnum.Decimal, 9, 2, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_PeopleCount", "SalaryStrings")]
		public decimal PeopleCount
		{
			get
			{
				return m_PeopleCount;
			}
			set
			{
				if ((m_PeopleCount != value))
				{
					base.OnPropertyChanging("PeopleCount");
					m_PeopleCount = value;
					base.OnPropertyChanged("PeopleCount");
				}
			}
		}

		/// <summary>
		/// 金额小计
		/// </summary>
		/// <value>金额小计</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "COSTSUBTOTAL", DbTypeEnum.Decimal, 18, 4, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_CostSubtotal", "SalaryStrings")]
		public decimal CostSubtotal
		{
			get
			{
				return m_CostSubtotal;
			}
			set
			{
				if ((m_CostSubtotal != value))
				{
					base.OnPropertyChanging("CostSubtotal");
					m_CostSubtotal = value;
					base.OnPropertyChanged("CostSubtotal");
				}
			}
		}

		/// <summary>
		/// 人均金额
		/// </summary>
		/// <value>人均金额</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "COSTAVERAGE", DbTypeEnum.Decimal, 18, 4, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("DailySummary_CostAverage", "SalaryStrings")]
		public decimal CostAverage
		{
			get
			{
				return m_CostAverage;
			}
			set
			{
				if ((m_CostAverage != value))
				{
					base.OnPropertyChanging("CostAverage");
					m_CostAverage = value;
					base.OnPropertyChanged("CostAverage");
				}
			}
		}

		/// <summary>
		/// 操作用户
		/// </summary>
		/// <value>操作用户</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "USERNAME", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Payroll_UserName", "SalaryStrings")]
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
		/// 创建时间
		/// </summary>
		/// <value>创建时间</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "CREATEDTIME", DbTypeEnum.DateTime, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Payroll_CreatedTime", "SalaryStrings")]
		[global::System.ComponentModel.DataAnnotations.DisplayFormatAttribute(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
		public System.DateTime CreatedTime
		{
			get
			{
				return m_CreatedTime;
			}
			set
			{
				if ((m_CreatedTime != value))
				{
					base.OnPropertyChanging("CreatedTime");
					m_CreatedTime = value;
					base.OnPropertyChanged("CreatedTime");
				}
			}
		}

		/// <summary>
		/// 修改时间
		/// </summary>
		/// <value>修改时间</value>
		[global::System.ComponentModel.DataAnnotations.TimestampAttribute()]
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "MODIFIEDTIME", DbTypeEnum.Timestamp, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Payroll_ModifiedTime", "SalaryStrings")]
		[global::System.ComponentModel.DataAnnotations.DisplayFormatAttribute(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}")]
		public System.DateTime ModifiedTime
		{
			get
			{
				return m_ModifiedTime;
			}
			set
			{
				if ((m_ModifiedTime != value))
				{
					base.OnPropertyChanging("ModifiedTime");
					m_ModifiedTime = value;
					base.OnPropertyChanged("ModifiedTime");
				}
			}
		}
	}
	#endregion

	//
	// 摘要:
	//     行事历类型
	[DefaultValue(3)]
	public enum CalendarTypes
	{
		//
		// 摘要:
		//     上午(前半天)上班
		Morning = 1,
		//
		// 摘要:
		//     下午(后半天)上班
		Afternoon,
		//
		// 摘要:
		//     全天上班
		AllDay,
		//
		// 摘要:
		//     周末
		Weekend,
		//
		// 摘要:
		//     节假日
		Holiday
	}
	/// <summary>员工排班信息</summary>
	public class ClassCalencar
	{
		public ClassCalencar(string clsName, CalendarTypes type)
		{
			ClassName = clsName; Calendar = type;
		}

		public string ClassName { private set; get; }
		public CalendarTypes Calendar { private set; get; }
	}

	/// <summary>
	/// DailyResultEntity
	/// </summary>
	[global::System.SerializableAttribute()]
	[global::System.ComponentModel.ToolboxItemAttribute(false)]
	[global::System.Runtime.InteropServices.GuidAttribute("C2B10B0B-20A9-4432-8E4D-948549B8FB33")]
	[global::Basic.EntityLayer.TableMappingAttribute("PDM_EMPLOYEE")]
	[global::Basic.EntityLayer.GroupNameAttribute("AtGroup", "TimeStrings")]
	public partial class DailyResultEntity : global::Basic.EntityLayer.AbstractEntity
	{

		/// <summary>考勤计算组</summary>
		[global::Basic.EntityLayer.PrimaryKeyAttribute()]
		[global::Basic.EntityLayer.ColumnMappingAttribute(null, "GROUPKEY", DbTypeEnum.Guid, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("AtGroup_GroupKey", "TimeStrings")]
		public System.Guid GroupKey { get; set; }

		/// <summary></summary>
		[Basic.EntityLayer.PropertyCollection]
		public Dictionary<string, ClassCalencar> Dailies { get { return _Dailies; } }
		private Dictionary<string, ClassCalencar> _Dailies = new Dictionary<string, ClassCalencar>(50);

		/// <summary></summary>
		/// <param name="name"></param>
		/// <param name="className"></param>
		/// <param name="type"></param>
		public void SetDaily(string name, string className, CalendarTypes type)
		{
			if (_Dailies.ContainsKey(name)) { _Dailies[name] = new ClassCalencar(className, type); }
			else { _Dailies.Add(name, new ClassCalencar(className, type)); }
		}

		/// <summary></summary>
		[Basic.EntityLayer.PropertyCollection]
		public Dictionary<string, decimal> Results { get { return _Results; } }
		private Dictionary<string, decimal> _Results = new Dictionary<string, decimal>(50);
		/// <summary></summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetResult(string name, decimal value)
		{
			if (_Results.ContainsKey(name)) { _Results[name] = value; }
			else { _Results.Add(name, value); }
		}

		private int m_EmpKey;

		private string m_DepartCode;

		private bool m_AllowShift;

		private bool m_GeneralTiming;

		private int m_ClassKey;

		private string m_ShortName;

		private string m_ClassName;

		private decimal m_WorkHour;

		/// <summary>
		/// 初始化 DailyResultEntity 类的实例。
		/// </summary>
		public DailyResultEntity() : base() { }

		/// <summary>
		/// 使用关键字初始化 DailyResultEntity 类的实例。
		/// </summary>
		/// <param name="pEmpKey">员工关键字</param>
		public DailyResultEntity(int pEmpKey) : base() { this.m_EmpKey = pEmpKey; }

		/// <summary>
		/// 员工关键字
		/// </summary>
		/// <value>员工关键字</value>
		[global::Basic.EntityLayer.PrimaryKeyAttribute()]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "EMPKEY", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("AtGroup_EmpKey", "TimeStrings")]
		public int EmpKey
		{
			get
			{
				return m_EmpKey;
			}
			set
			{
				if ((m_EmpKey != value))
				{
					base.OnPropertyChanging("EmpKey");
					m_EmpKey = value;
					base.OnPropertyChanged("EmpKey");
				}
			}
		}

		/// <summary>
		/// 所属公司
		/// </summary>
		/// <value>所属公司</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T3", "DEPARTCODE", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Attendance_Departcode", "TimeStrings")]
		public string DepartCode
		{
			get
			{
				return m_DepartCode;
			}
			set
			{
				if ((m_DepartCode != value))
				{
					base.OnPropertyChanging("DepartCode");
					m_DepartCode = value;
					base.OnPropertyChanged("DepartCode");
				}
			}
		}

		/// <summary>
		/// 是否轮班
		/// </summary>
		/// <value>是否轮班</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "ALLOWSHIFT", DbTypeEnum.Boolean, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Attendance_AllowShift", "TimeStrings")]
		public bool AllowShift
		{
			get
			{
				return m_AllowShift;
			}
			set
			{
				if ((m_AllowShift != value))
				{
					base.OnPropertyChanging("AllowShift");
					m_AllowShift = value;
					base.OnPropertyChanged("AllowShift");
				}
			}
		}

		/// <summary>
		/// 综合工时
		/// </summary>
		/// <value>综合工时</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "GENERALTIMING", DbTypeEnum.Boolean, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Attendance_GeneralTiming", "TimeStrings")]
		public bool GeneralTiming
		{
			get
			{
				return m_GeneralTiming;
			}
			set
			{
				if ((m_GeneralTiming != value))
				{
					base.OnPropertyChanging("GeneralTiming");
					m_GeneralTiming = value;
					base.OnPropertyChanged("GeneralTiming");
				}
			}
		}

		/// <summary>
		/// 默认班别
		/// </summary>
		/// <value>默认班别</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute(null, "CLASSKEY", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Attendance_ClassKey", "TimeStrings")]
		public int ClassKey
		{
			get
			{
				return m_ClassKey;
			}
			set
			{
				if ((m_ClassKey != value))
				{
					base.OnPropertyChanging("ClassKey");
					m_ClassKey = value;
					base.OnPropertyChanged("ClassKey");
				}
			}
		}

		/// <summary>
		/// Property: ShortName
		/// </summary>
		/// <value>属性 ShortName 的值</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T4", "SHORTNAME", DbTypeEnum.NVarChar, 10, true)]
		[global::Basic.EntityLayer.WebDisplayAttribute("AtGroup_ShortName", "TimeStrings")]
		public string ShortName
		{
			get
			{
				return m_ShortName;
			}
			set
			{
				if ((m_ShortName != value))
				{
					base.OnPropertyChanging("ShortName");
					m_ShortName = value;
					base.OnPropertyChanged("ShortName");
				}
			}
		}

		/// <summary>
		/// 班别名称
		/// </summary>
		/// <value>班别名称</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T4", "CLASSNAME", DbTypeEnum.NVarChar, 50, true)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Attendance_ClassKey", "TimeStrings")]
		public string ClassName
		{
			get
			{
				return m_ClassName;
			}
			set
			{
				if ((m_ClassName != value))
				{
					base.OnPropertyChanging("ClassName");
					m_ClassName = value;
					base.OnPropertyChanged("ClassName");
				}
			}
		}

		/// <summary>
		/// 额定工时
		/// </summary>
		/// <value>额定工时</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute(null, "WORKHOUR", DbTypeEnum.Decimal, 9, 2, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("Attendance_WorkHour", "TimeStrings")]
		public decimal WorkHour
		{
			get
			{
				return m_WorkHour;
			}
			set
			{
				if ((m_WorkHour != value))
				{
					base.OnPropertyChanging("WorkHour");
					m_WorkHour = value;
					base.OnPropertyChanged("WorkHour");
				}
			}
		}
	}
}