using Basic.Designer;
using Basic.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Basic.Database
{
    /// <summary>
    /// 表示命令参数
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class TransactParameterInfo : AbstractNotifyChangedDescriptor
    {
        private readonly TransactTableInfo transactTableInfo;

        /// <summary>
        /// 初始化 FunctionParameterInfo 类实例。
        /// </summary>
        /// <param name="tableInfo">拥有此参数的 AbstractCommandElement 类命令实例。</param>
        internal TransactParameterInfo(TransactTableInfo tableInfo)
            : base() { transactTableInfo = tableInfo; }

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public override string GetClassName() { return _Name; }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public override string GetComponentName() { return typeof(TransactTableInfo).Name; }

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
                return typeof(TransactParameterInfo).Name;
            return string.Concat(Name, " : ", ParameterType);
        }

        /// <summary>
        /// 根据数据库列信息，创建查询表中所有数据的命令及其参数
        /// </summary>
        /// <param name="parameter">数据库表结构信息</param>
        /// <param name="name">需要刷新的实体类信息</param>
        public void CreateSqlParameter(IDbDataParameter parameter, string name)
        {
            parameter.ParameterName = name;
            parameter.SourceColumn = _SourceColumn;
            parameter.Precision = (byte)_Precision;
            parameter.Scale = _Scale;
            parameter.Direction = _Direction;
           // parameter.IsNullable = _Nullable;
            ConvertSqlParameterType(parameter, _ParameterType, (byte)_Precision, _Scale);
        }

        /// <summary>
        /// 转换数据库参数类型
        /// </summary>
        /// <param name="sqlParameter">数据库命令执行的参数</param>
        /// <param name="dbType">SqlServer数据库列类型,DbType枚举的值</param>
        /// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
        /// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
        private void ConvertSqlParameterType(IDbDataParameter sqlParameter, DbTypeEnum dbType, byte precision, byte scale)
        {
            switch (dbType)
            {
                case DbTypeEnum.Boolean:
                    sqlParameter.DbType = DbType.Boolean;
                    return;
                case DbTypeEnum.Binary:
                    sqlParameter.DbType = DbType.Binary;
                    return;
                case DbTypeEnum.Char:
                    sqlParameter.DbType = DbType.AnsiString;
                    return;
                case DbTypeEnum.Date:
                    sqlParameter.DbType = DbType.Date;
                    return;
                case DbTypeEnum.DateTime:
                    sqlParameter.DbType = DbType.DateTime;
                    return;
                case DbTypeEnum.DateTime2:
                    sqlParameter.DbType = DbType.DateTime2;
                    return;
                case DbTypeEnum.DateTimeOffset:
                    sqlParameter.DbType = DbType.DateTimeOffset;
                    return;
                case DbTypeEnum.Decimal:
                    sqlParameter.DbType = DbType.Decimal;
                    sqlParameter.Precision = precision;
                    sqlParameter.Scale = scale;
                    return;
                //case DbTypeEnum.Double:
                //    sqlParameter.DbType = DbType.Float;
                //    return;
                //case DbTypeEnum.Guid:
                //    sqlParameter.DbType = DbType.UniqueIdentifier;
                //    return;
                //case DbTypeEnum.Int16:
                //    sqlParameter.DbType = DbType.SmallInt;
                //    return;
                //case DbTypeEnum.Int32:
                //    sqlParameter.DbType = DbType.Int;
                //    return;
                //case DbTypeEnum.Int64:
                //    sqlParameter.DbType = DbType.BigInt;
                //    return;
                //case DbTypeEnum.Image:
                //    sqlParameter.DbType = DbType.VarBinary;
                //    sqlParameter.Size = -1;
                //    return;
                //case DbTypeEnum.NText:
                //    sqlParameter.DbType = DbType.NVarChar;
                //    sqlParameter.Size = -1;
                //    return;
                //case DbTypeEnum.Text:
                //    sqlParameter.DbType = DbType.VarChar;
                //    sqlParameter.Size = -1;
                //    return;
                //case DbTypeEnum.NChar:
                //    sqlParameter.DbType = DbType.NChar;
                //    return;
                //case DbTypeEnum.NVarChar:
                //    sqlParameter.DbType = DbType.NVarChar;
                //    return;
                //case DbTypeEnum.Single:
                //    sqlParameter.DbType = DbType.Real;
                //    return;
                case DbTypeEnum.Time:
                    sqlParameter.DbType = DbType.Time;
                    return;
                case DbTypeEnum.Timestamp:
                    sqlParameter.DbType = DbType.DateTime2;
                    sqlParameter.Size = 3;
                    return;
                //case DbTypeEnum.VarBinary:
                //    sqlParameter.DbType = DbType.VarBinary;
                //    return;
                //case DbTypeEnum.VarChar:
                //    sqlParameter.DbType = DbType.VarChar;
                //    return;
            }
        }
    }
}
