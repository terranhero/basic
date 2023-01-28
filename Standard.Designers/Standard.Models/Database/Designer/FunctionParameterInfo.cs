using System;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;

namespace Basic.Database
{
	/// <summary>
	/// 表示命令参数
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class FunctionParameterInfo : AbstractNotifyChangedDescriptor
	{
		private readonly TableFunctionInfo tableFunctionInfo;

		/// <summary>
		/// 初始化 FunctionParameterInfo 类实例。
		/// </summary>
		/// <param name="functionInfo">拥有此参数的 AbstractCommandElement 类命令实例。</param>
		internal FunctionParameterInfo(TableFunctionInfo functionInfo)
			: base() { tableFunctionInfo = functionInfo; }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return _Name; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return typeof(TableFunctionInfo).Name; }

		private string _Name = string.Empty;
		/// <summary>
		/// 获取或设置数据库参数的名称
		/// </summary>
		[System.ComponentModel.DefaultValue("")]
		[System.ComponentModel.Bindable(true)]
		[Basic.Designer.PackageCategory("PersistentCategory_Content")]
		[Basic.Designer.PackageDescription("PropertyDescription_ParameterName")]
		public string Name
		{
			get { return _Name; }
			set { if (_Name != value) { _Name = value; OnPropertyChanged("Name"); } }
		}

		private string _SourceColumn = string.Empty;
		/// <summary>
		/// 获取或设置源列的名称
		/// </summary>
		[System.ComponentModel.DefaultValue("")]
		[Basic.Designer.PackageCategory("PersistentCategory_Content")]
		[Basic.Designer.PackageDescription("PropertyDescription_SourceColumn")]
		public string SourceColumn
		{
			get { return _SourceColumn; }
			set { if (_SourceColumn != value) { _SourceColumn = value; OnPropertyChanged("SourceColumn"); } }
		}

		private DbTypeEnum _ParameterType = DbTypeEnum.NVarChar;
		/// <summary>
		/// 获取或设置一个值，该值指示参数数据库参数的类型。
		/// </summary>
		[System.ComponentModel.DefaultValue(typeof(DbTypeEnum), "NVarChar")]
		[System.ComponentModel.Bindable(true)]
		[Basic.Designer.PackageCategory("PersistentCategory_Content")]
		[Basic.Designer.PackageDescription("PropertyDescription_ParameterType")]
		public DbTypeEnum ParameterType
		{
			get { return _ParameterType; }
			set { if (_ParameterType != value) { _ParameterType = value; OnPropertyChanged("ParameterType"); } }
		}

		private bool _Nullable = false;
		/// <summary>
		/// 获取或设置一个值，该值指示参数是否接受空值
		/// </summary>
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Bindable(true)]
		[Basic.Designer.PackageCategory("PersistentCategory_Content")]
		[Basic.Designer.PackageDescription("PropertyDescription_Nullable")]
		public bool Nullable
		{
			get { return _Nullable; }
			set { if (_Nullable != value) { _Nullable = value; OnPropertyChanged("Nullable"); } }
		}

		/// <summary>
		/// 判断当前参数是否需要Size属性。
		/// </summary>
		private bool NeedSizeProperty
		{
			get
			{
				return _ParameterType == DbTypeEnum.Binary ||
					_ParameterType == DbTypeEnum.VarBinary ||
					_ParameterType == DbTypeEnum.Char ||
					_ParameterType == DbTypeEnum.VarChar ||
					_ParameterType == DbTypeEnum.NChar ||
					_ParameterType == DbTypeEnum.NVarChar;
			}
		}
		private int _Size = 0;
		/// <summary>
		/// 获取或设置列中数据的最大大小（以字节为单位）。
		/// </summary>
		[System.ComponentModel.DefaultValue(0)]
		[System.ComponentModel.Bindable(true)]
		[Basic.Designer.PackageCategory("PersistentCategory_Content")]
		[Basic.Designer.PackageDescription("PropertyDescription_Size")]
		public int Size
		{
			get { return _Size; }
			set { if (_Size != value) { _Size = value; OnPropertyChanged("Size"); } }
		}

		private int _Precision = 0;
		/// <summary>
		/// 获取或设置列中数据的最大大小（以字节为单位）。
		/// </summary>
		[System.ComponentModel.DefaultValue(0)]
		[System.ComponentModel.Bindable(true)]
		[Basic.Designer.PackageCategory("PersistentCategory_Content")]
		[Basic.Designer.PackageDescription("PropertyDescription_Precision")]
		public int Precision
		{
			get { return _Precision; }
			set { if (_Precision != value) { _Precision = value; OnPropertyChanged("Precision"); } }
		}

		private byte _Scale = 0;
		/// <summary>
		/// 获取或设置数据库参数值解析为的小数位数
		/// </summary>
		[System.ComponentModel.DefaultValue(0)]
		[System.ComponentModel.Bindable(true)]
		[Basic.Designer.PackageCategory("PersistentCategory_Content")]
		[Basic.Designer.PackageDescription("PropertyDescription_Scale")]
		public byte Scale
		{
			get { return _Scale; }
			set { if (_Scale != value) { _Scale = value; OnPropertyChanged("Scale"); } }
		}

		/// <summary>
		/// 参数显示的图标资源
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public string Source
		{
			get
			{
				if (Direction == ParameterDirection.Output)
					return "../Images/Database_OutputParameter.ico";
				else if (Direction == ParameterDirection.ReturnValue)
					return "../Images/Database_ReturnValue.ico";
				return "../Images/Database_InputParameter.ico";
			}
		}

		private ParameterDirection _Direction = ParameterDirection.Input;
		/// <summary>
		/// 获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
		/// </summary>
		[System.ComponentModel.DefaultValue(typeof(ParameterDirection), "Input")]
		[System.ComponentModel.Bindable(true)]
		[Basic.Designer.PackageCategory("PersistentCategory_Content")]
		[Basic.Designer.PackageDescription("PropertyDescription_Direction")]
		public ParameterDirection Direction
		{
			get { return _Direction; }
			set { if (_Direction != value) { _Direction = value; OnPropertyChanged("Direction"); OnPropertyChanged("Source"); } }
		}

		/// <summary>
		/// 返回表示当前 Basic.Database.FunctionParameterInfo 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 Basic.Database.FunctionParameterInfo。</returns>
		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(Name))
				return typeof(FunctionParameterInfo).Name;
			return string.Concat(Name, " : ", ParameterType);
		}
	}
}
