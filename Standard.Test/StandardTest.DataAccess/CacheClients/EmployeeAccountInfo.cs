using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.Enums;

namespace Standard.CacheClients
{

	/// <summary>
	/// 用户类型枚举
	/// </summary>
	[System.ComponentModel.DefaultValue(1), System.Flags()]
	[Basic.EntityLayer.WebDisplayConverter("AdminStrings")]
	public enum UserKinds
	{
		/// <summary>普通用户</summary>
		SpaceUsers = 1,

		/// <summary>管理用户</summary>
		SystemUsers = 3,

		/// <summary>系统管理员</summary>
		Administrators = 99,
	}

	/// <summary>
	/// 员工账号表
	/// </summary>
	[global::System.SerializableAttribute()]
	[global::System.ComponentModel.ToolboxItemAttribute(false)]
	[global::System.Runtime.InteropServices.GuidAttribute("1B7DC5E6-46FF-455D-A106-45A9D494F8B3")]
	[global::Basic.EntityLayer.TableMappingAttribute("SYS_LOGINUSER")]
	[global::Basic.EntityLayer.GroupNameAttribute("Mobile", "MobileStrings")]
	public partial class EmployeeAccountInfo : global::Basic.EntityLayer.AbstractEntity
	{

		private System.Guid m_UserKey;

		private int m_EmpKey;

		private string m_LoginName;

		private string m_EmployeeCode;

		private string m_ChineseName;

		private int m_CorpKey;

		private int m_OrgKey;

		private string m_NodeName;

		private int m_PosKey;

		private System.Guid m_GroupKey;

		private System.Guid m_RoleKey;

		private string m_Password;

		private bool m_Enabled;

		private UserKinds m_UserKind;

		private System.DateTime m_EffectiveDate;

		private string m_MobilePhone;

		private string m_EmailAddress;

		private string m_WeiXinCode;

		private string m_WorkCode;

		private string m_DingCode;

		private string m_SlipCypher;

		private string m_Avatar;

		/// <summary>
		/// 初始化类实例。
		/// </summary>
		partial void InitializationClass();


		/// <summary>
		/// 初始化 EmployeeAccountInfo 类的实例。
		/// </summary>
		public EmployeeAccountInfo() :
				base()
		{
			this.InitializationClass();
		}

		/// <summary>
		/// 使用关键字初始化 EmployeeAccountInfo 类的实例。
		/// </summary>
		/// <param name="pUserKey">登录用户关键字</param>
		public EmployeeAccountInfo(System.Guid pUserKey) :
				base()
		{
			this.m_UserKey = pUserKey;
			this.InitializationClass();
		}

		/// <summary>
		/// 登录用户关键字
		/// </summary>
		/// <value>登录用户关键字</value>
		[global::Basic.EntityLayer.PrimaryKeyAttribute()]
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "USERKEY", DbTypeEnum.Guid, false)]
		public System.Guid UserKey
		{
			get
			{
				return m_UserKey;
			}
			set
			{
				if ((m_UserKey != value))
				{
					base.OnPropertyChanging("UserKey");
					m_UserKey = value;
					base.OnPropertyChanged("UserKey");
				}
			}
		}

		/// <summary>
		/// 员工关键字
		/// </summary>
		/// <value>员工关键字</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "EMPKEY", DbTypeEnum.Int32, false)]
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
		/// 登录用户名称
		/// </summary>
		/// <value>登录用户名称</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "LOGINNAME", DbTypeEnum.NVarChar, 50, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("LoginUser_LoginName", "AdminStrings")]
		[global::Basic.Validations.RequiredAttribute()]
		[global::Basic.Validations.StringLengthAttribute(50)]
		public string LoginName
		{
			get
			{
				return m_LoginName;
			}
			set
			{
				if ((m_LoginName != value))
				{
					base.OnPropertyChanging("LoginName");
					m_LoginName = value;
					base.OnPropertyChanged("LoginName");
				}
			}
		}

		/// <summary>
		/// 员工工号
		/// </summary>
		/// <value>员工工号</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "EMPLOYEECODE", DbTypeEnum.NVarChar, 20, false)]
		public string EmployeeCode
		{
			get
			{
				return m_EmployeeCode;
			}
			set
			{
				if ((m_EmployeeCode != value))
				{
					base.OnPropertyChanging("EmployeeCode");
					m_EmployeeCode = value;
					base.OnPropertyChanged("EmployeeCode");
				}
			}
		}

		/// <summary>
		/// 中文姓名
		/// </summary>
		/// <value>中文姓名</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "CHINESENAME", DbTypeEnum.NVarChar, 50, false)]
		public string ChineseName
		{
			get
			{
				return m_ChineseName;
			}
			set
			{
				if ((m_ChineseName != value))
				{
					base.OnPropertyChanging("ChineseName");
					m_ChineseName = value;
					base.OnPropertyChanged("ChineseName");
				}
			}
		}

		/// <summary>
		/// Property: CorpKey
		/// </summary>
		/// <value>属性 CorpKey 的值</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "CORPKEY", "COMPANYKEY", DbTypeEnum.Int32, false)]
		public int CorpKey
		{
			get
			{
				return m_CorpKey;
			}
			set
			{
				if ((m_CorpKey != value))
				{
					base.OnPropertyChanging("CorpKey");
					m_CorpKey = value;
					base.OnPropertyChanged("CorpKey");
				}
			}
		}

		/// <summary>
		/// 组织关键字
		/// </summary>
		/// <value>组织关键字</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "ORGKEY", DbTypeEnum.Int32, false)]
		public int OrgKey
		{
			get
			{
				return m_OrgKey;
			}
			set
			{
				if ((m_OrgKey != value))
				{
					base.OnPropertyChanging("OrgKey");
					m_OrgKey = value;
					base.OnPropertyChanged("OrgKey");
				}
			}
		}

		/// <summary>
		/// 组织机构名称
		/// </summary>
		/// <value>组织机构名称</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "NODENAME", "SHORTNAME", DbTypeEnum.NVarChar, 50, false)]
		public string NodeName
		{
			get
			{
				return m_NodeName;
			}
			set
			{
				if ((m_NodeName != value))
				{
					base.OnPropertyChanging("NodeName");
					m_NodeName = value;
					base.OnPropertyChanged("NodeName");
				}
			}
		}

		/// <summary>
		/// 职位关键字
		/// </summary>
		/// <value>职位关键字</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T2", "POSKEY", DbTypeEnum.Int32, false)]
		public int PosKey
		{
			get
			{
				return m_PosKey;
			}
			set
			{
				if ((m_PosKey != value))
				{
					base.OnPropertyChanging("PosKey");
					m_PosKey = value;
					base.OnPropertyChanged("PosKey");
				}
			}
		}

		/// <summary>
		/// 用户群组
		/// </summary>
		/// <value>用户群组</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "GROUPKEY", DbTypeEnum.Guid, false)]
		public System.Guid GroupKey
		{
			get
			{
				return m_GroupKey;
			}
			set
			{
				if ((m_GroupKey != value))
				{
					base.OnPropertyChanging("GroupKey");
					m_GroupKey = value;
					base.OnPropertyChanged("GroupKey");
				}
			}
		}

		/// <summary>
		/// 用户角色
		/// </summary>
		/// <value>用户角色</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "ROLEKEY", DbTypeEnum.Guid, false)]
		public System.Guid RoleKey
		{
			get
			{
				return m_RoleKey;
			}
			set
			{
				if ((m_RoleKey != value))
				{
					base.OnPropertyChanging("RoleKey");
					m_RoleKey = value;
					base.OnPropertyChanged("RoleKey");
				}
			}
		}

		/// <summary>
		/// 密码
		/// </summary>
		/// <value>密码</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "PASSWORD", DbTypeEnum.VarChar, 256, false)]
		public string Password
		{
			get
			{
				return m_Password;
			}
			set
			{
				if ((m_Password != value))
				{
					base.OnPropertyChanging("Password");
					m_Password = value;
					base.OnPropertyChanged("Password");
				}
			}
		}

		/// <summary>
		/// 是否有效
		/// </summary>
		/// <value>是否有效</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "ENABLED", DbTypeEnum.Boolean, false)]
		public bool Enabled
		{
			get
			{
				return m_Enabled;
			}
			set
			{
				if ((m_Enabled != value))
				{
					base.OnPropertyChanging("Enabled");
					m_Enabled = value;
					base.OnPropertyChanged("Enabled");
				}
			}
		}

		/// <summary>
		/// 用户类型1:管理用户,0:普通用户
		/// </summary>
		/// <value>用户类型1:管理用户,0:普通用户</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute(null, "USERKIND", DbTypeEnum.Int32, false)]
		[global::Basic.EntityLayer.WebDisplayAttribute("LoginUser_UserKind", "AdminStrings")]
		[global::Basic.Validations.RequiredAttribute()]
		public UserKinds UserKind
		{
			get
			{
				return m_UserKind;
			}
			set
			{
				if ((m_UserKind != value))
				{
					base.OnPropertyChanging("UserKind");
					m_UserKind = value;
					base.OnPropertyChanged("UserKind");
				}
			}
		}

		/// <summary>
		/// 有效期，指账号的有效期限
		/// </summary>
		/// <value>有效期，指账号的有效期限</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "EFFECTIVEDATE", DbTypeEnum.Date, false)]
		public System.DateTime EffectiveDate
		{
			get
			{
				return m_EffectiveDate;
			}
			set
			{
				if ((m_EffectiveDate != value))
				{
					base.OnPropertyChanging("EffectiveDate");
					m_EffectiveDate = value;
					base.OnPropertyChanged("EffectiveDate");
				}
			}
		}

		/// <summary>
		/// 手机号码
		/// </summary>
		/// <value>手机号码</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "MOBILEPHONE", DbTypeEnum.VarChar, 20, true)]
		public string MobilePhone
		{
			get
			{
				return m_MobilePhone;
			}
			set
			{
				if ((m_MobilePhone != value))
				{
					base.OnPropertyChanging("MobilePhone");
					m_MobilePhone = value;
					base.OnPropertyChanged("MobilePhone");
				}
			}
		}

		/// <summary>
		/// 常用邮箱
		/// </summary>
		/// <value>常用邮箱</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "EMAILADDRESS", DbTypeEnum.VarChar, 100, true)]
		public string EmailAddress
		{
			get
			{
				return m_EmailAddress;
			}
			set
			{
				if ((m_EmailAddress != value))
				{
					base.OnPropertyChanging("EmailAddress");
					m_EmailAddress = value;
					base.OnPropertyChanged("EmailAddress");
				}
			}
		}

		/// <summary>
		/// 微信公众号员工账号(OPENID)
		/// </summary>
		/// <value>微信公众号员工账号(OPENID)</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "WEIXINCODE", DbTypeEnum.VarChar, 100, true)]
		public string WeiXinCode
		{
			get
			{
				return m_WeiXinCode;
			}
			set
			{
				if ((m_WeiXinCode != value))
				{
					base.OnPropertyChanging("WeiXinCode");
					m_WeiXinCode = value;
					base.OnPropertyChanged("WeiXinCode");
				}
			}
		}

		/// <summary>
		/// 企业微信的员工账号
		/// </summary>
		/// <value>企业微信的员工账号</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "WORKCODE", DbTypeEnum.VarChar, 100, true)]
		public string WorkCode
		{
			get
			{
				return m_WorkCode;
			}
			set
			{
				if ((m_WorkCode != value))
				{
					base.OnPropertyChanging("WorkCode");
					m_WorkCode = value;
					base.OnPropertyChanged("WorkCode");
				}
			}
		}

		/// <summary>
		/// 阿里钉钉的员工账号
		/// </summary>
		/// <value>阿里钉钉的员工账号</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "DINGCODE", DbTypeEnum.VarChar, 100, true)]
		public string DingCode
		{
			get
			{
				return m_DingCode;
			}
			set
			{
				if ((m_DingCode != value))
				{
					base.OnPropertyChanging("DingCode");
					m_DingCode = value;
					base.OnPropertyChanged("DingCode");
				}
			}
		}

		/// <summary>
		/// 薪资单密码
		/// </summary>
		/// <value>薪资单密码</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "SLIPCYPHER", DbTypeEnum.VarChar, 100, true)]
		public string SlipCypher
		{
			get
			{
				return m_SlipCypher;
			}
			set
			{
				if ((m_SlipCypher != value))
				{
					base.OnPropertyChanging("SlipCypher");
					m_SlipCypher = value;
					base.OnPropertyChanged("SlipCypher");
				}
			}
		}

		/// <summary>
		/// 头像链接地址
		/// </summary>
		/// <value>头像链接地址</value>
		[global::Basic.EntityLayer.ColumnMappingAttribute("T1", "AVATAR", DbTypeEnum.VarChar, 500, true)]
		public string Avatar
		{
			get
			{
				return m_Avatar;
			}
			set
			{
				if ((m_Avatar != value))
				{
					base.OnPropertyChanging("Avatar");
					m_Avatar = value;
					base.OnPropertyChanged("Avatar");
				}
			}
		}
	}
}
