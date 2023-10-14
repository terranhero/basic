using Basic.Enums;

namespace Basic.EntityLayer
{
	/// <summary>员工基本信息抽象类</summary>
	[InnerJoin("PDM_EMPLOYEE", "EMP", "EMP.EMPKEY=T1.EMPKEY")]
	[InnerJoin("SYS_ORGANIZATIONPERMISSION", "SOP", "EMP.ORGKEY=SOP.ORGKEY AND EMP.POSKEY=SOP.POSKEY")]
	[InnerJoin("SYS_POSITION", "POS", "EMP.POSKEY=POS.POSKEY")]
	[InnerJoin("SYS_ENUMITEM", "SOP", "EMP.STATUSKEY=ESK.ITEMKEY")]
	[JoinParameter("SOP.GROUPKEY={%WEBGROUPKEY%}", "WEBGROUPKEY", DbTypeEnum.Guid)]
	[JoinOrder("SOP.NODECODE", "EMP.EMPLOYEECODE")]
	public abstract class AEmployeeInfo : AbstractEntity
	{
		#region 构造函数及其初始化类信息
		/// <summary>
		/// 使用默认的构造函数创建 AEmployeeInfo 类实例
		/// </summary>
		protected AEmployeeInfo() : base() { }

		/// <summary>
		/// 初始化 AEmployeeInfo 类实例，此构造函数仅供子类调用。
		/// </summary>
		/// <param name="enabledValidation">是否启用实体类中 IDataErrorInfo 的验证特性。</param>
		protected AEmployeeInfo(bool enabledValidation) : base(enabledValidation) { }
		#endregion

		/// <summary>员工主键</summary>
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "EMPKEY", DbTypeEnum.Int32, true)]
		public int JoinKey { get; set; }

		/// <summary>员工工号</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_EmployeeCode", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "EMPLOYEECODE", DbTypeEnum.NVarChar, 20, true)]
		[global::Basic.EntityLayer.JoinFieldAttribute("EMP", "EMPLOYEECODE", DbTypeEnum.NVarChar, 20)]
		public virtual string EmployeeCode { get; set; }

		/// <summary>员工姓名</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_ChineseName", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "CHINESENAME", DbTypeEnum.NVarChar, 50, true)]
		[global::Basic.EntityLayer.JoinFieldAttribute("EMP", "CHINESENAME", DbTypeEnum.NVarChar, 50)]
		public virtual string ChineseName { get; set; }

		/// <summary>英文名</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_ChineseName", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "ENGLISHNAME", DbTypeEnum.NVarChar, 50, true)]
		[global::Basic.EntityLayer.JoinFieldAttribute("EMP", "ENGLISHNAME", DbTypeEnum.NVarChar, 50)]
		public virtual string EnglishName { get; set; }

		/// <summary>所属公司</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_CorpKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "CORPKEY", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.JoinFieldAttribute("EMP", "CORPKEY", DbTypeEnum.Int32)]
		public virtual int CorpKey { get; set; }

		/// <summary>所属公司</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_CorpKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "CORPNAME", DbTypeEnum.NVarChar, 100, false)]
		[global::Basic.EntityLayer.JoinFieldAttribute("EMP", "CORPNAME", DbTypeEnum.NVarChar, 100)]
		public virtual string CorpName { get; set; }

		/// <summary>所属组织</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_OrgKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "ORGKEY", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.JoinFieldAttribute("EMP", "ORGKEY", DbTypeEnum.Int32)]
		public virtual int OrgKey { get; set; }

		/// <summary>组织代码</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_OrgKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("SOP", "NODECODE", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.JoinFieldAttribute("SOP", "NODECODE", DbTypeEnum.NVarChar, 50)]
		public virtual string NodeCode { get; set; }

		/// <summary>所属部门</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_DepartName", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("SOP", "DEPARTNAME", DbTypeEnum.NVarChar, 200, true)]
		[global::Basic.EntityLayer.JoinFieldAttribute("SOP", "DEPARTNAME", DbTypeEnum.NVarChar, 200)]
		public virtual string DepartName { get; set; }

		/// <summary>所属组织</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_OrgKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("SOP", "NODENAME", DbTypeEnum.NVarChar, 100, true)]
		[global::Basic.EntityLayer.JoinFieldAttribute("SOP", "NODENAME", DbTypeEnum.NVarChar, 100)]
		public virtual string NodeText { get; set; }

		/// <summary>所属组织</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_OrgKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("SOP", "ORGANIZATIONNAME", DbTypeEnum.NVarChar, 100, true)]
		public virtual string OrganizationName { get { return NodeText; } }

		/// <summary>员工职位</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_PosKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "POSKEY", DbTypeEnum.Int32, false)]
		public virtual int PosKey { get; set; }

		/// <summary>职位类别</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_JobType", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("POS", "JOBTYPE", DbTypeEnum.Int32, false)]
		public virtual int JobType { get; set; }

		/// <summary>员工在职状态(在职，观察期，离职)</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_OnJob", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "ONJOB", DbTypeEnum.Int16, false)]
		public virtual int OnJob { get; set; }

		/// <summary>员工在职状态(在职，观察期，离职)</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_JoinDate", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "JOINDATE", DbTypeEnum.Date, false)]
		public virtual System.DateTime JoinDate { get; set; }

		/// <summary>员工性别(男/女)</summary>
		/// <value>属性 Sstatustext 的值</value>
		[global::Basic.EntityLayer.WebDisplay("Employee_GenderKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "GENDERKEY", DbTypeEnum.Int32, true)]
		public virtual int GenderKey { get; set; }

		/// <summary>员工类型（试用，正式）</summary>
		/// <value>属性 Sstatustext 的值</value>
		[global::Basic.EntityLayer.WebDisplay("Employee_StatusKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "STATUSKEY", DbTypeEnum.Int32, true)]
		public virtual int StatusKey { get; set; }

		/// <summary>员工类型（试用，正式）</summary>
		/// <value>属性 Sstatustext 的值</value>
		[global::Basic.EntityLayer.WebDisplay("Employee_StatusKey", "PersonnelStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("EMP", "STATUSTEXT", DbTypeEnum.NVarChar, 100, true)]
		public virtual string StatusText { get; set; }
	}

	/// <summary>员工连续工作多日</summary>
	public partial class ConsecutiveWorkEntity : AEmployeeInfo
	{
		/// <summary>员工主键</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("DailyNormal_EmpKey", "TimeStrings")]
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "EMPKEY", DbTypeEnum.Int32, true)]
		public int EmpKey { get; set; }

		/// <summary>员工在职状态(在职，观察期，离职)</summary>
		[global::Basic.EntityLayer.WebDisplay("Employee_AtChecked", "PersonnelStrings")]
		public bool AtChecked { get; set; }

		/// <summary>开始日期</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("DailyNormal_BeginDate", "TimeStrings")]
		public DateTime BeginDate { get; set; }

		/// <summary>结束日期</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("DailyNormal_EndDate", "TimeStrings")]
		public DateTime EndDate { get; set; }

		/// <summary>连续工作天数</summary>
		[global::Basic.EntityLayer.WebDisplayAttribute("DailyNormal_WorkDays", "TimeStrings")]
		public int WorkDays { get; set; }

	}
}
