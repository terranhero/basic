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
				ClassName = "�װ�",
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
				ClassName = "�װ�",
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
				ClassName = "�װ�",
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
				ClassName = "�װ�",
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
				ClassName = "�װ�",
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


	//
	// ժҪ:
	//     ����������
	[DefaultValue(3)]
	public enum CalendarTypes
	{
		//
		// ժҪ:
		//     ����(ǰ����)�ϰ�
		Morning = 1,
		//
		// ժҪ:
		//     ����(�����)�ϰ�
		Afternoon,
		//
		// ժҪ:
		//     ȫ���ϰ�
		AllDay,
		//
		// ժҪ:
		//     ��ĩ
		Weekend,
		//
		// ժҪ:
		//     �ڼ���
		Holiday
	}
	/// <summary>Ա���Ű���Ϣ</summary>
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

		/// <summary>���ڼ�����</summary>
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
		/// ��ʼ�� DailyResultEntity ���ʵ����
		/// </summary>
		public DailyResultEntity() : base() { }

		/// <summary>
		/// ʹ�ùؼ��ֳ�ʼ�� DailyResultEntity ���ʵ����
		/// </summary>
		/// <param name="pEmpKey">Ա���ؼ���</param>
		public DailyResultEntity(int pEmpKey) : base() { this.m_EmpKey = pEmpKey; }

		/// <summary>
		/// Ա���ؼ���
		/// </summary>
		/// <value>Ա���ؼ���</value>
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
		/// ������˾
		/// </summary>
		/// <value>������˾</value>
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
		/// �Ƿ��ְ�
		/// </summary>
		/// <value>�Ƿ��ְ�</value>
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
		/// �ۺϹ�ʱ
		/// </summary>
		/// <value>�ۺϹ�ʱ</value>
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
		/// Ĭ�ϰ��
		/// </summary>
		/// <value>Ĭ�ϰ��</value>
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
		/// <value>���� ShortName ��ֵ</value>
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
		/// �������
		/// </summary>
		/// <value>�������</value>
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
		/// ���ʱ
		/// </summary>
		/// <value>���ʱ</value>
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