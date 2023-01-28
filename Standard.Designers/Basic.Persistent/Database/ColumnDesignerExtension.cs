using Basic.Designer;
using Basic.EntityLayer;
using Basic.DataEntities;
using Basic.Configuration;
using System.Data;
using Basic.Enums;

namespace Basic.Database
{
	/// <summary>
	/// 表示数据库表对象的设计时类型
	/// </summary>
	internal static class ColumnDesignerExtension
	{
		/// <summary>
		/// 将 DesignTableInfo 对象的内容复制到当前实例。
		/// </summary>
		/// <param name="entity">标识需要复制的 DesignTableInfo 类实例。</param>
		public static void CopyFrom(this ColumnDesignerInfo column, DataEntityPropertyElement property)
		{
			column.PrimaryKey = property.PrimaryKey;
			column.Comment = property.Comment;
			column.Computed = property.Computed;
			column.DbType = property.DbType;
			column.DefaultValue = property.DefaultValue;
			column.Name = property.Column;
			column.Nullable = property.Nullable;
			column.Precision = property.Precision;
			column.Scale = property.Scale;
			column.Size = property.Size;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateParameter(this ColumnDesignerInfo column, CommandParameter parameter)
		{
			parameter.Name = column.Name;
			parameter.SourceColumn = column.Name;
			parameter.ParameterType = column.DbType;
			switch (column.DbType)
			{
				case DbTypeEnum.Decimal:
					parameter.Precision = column.Precision;
					parameter.Scale = (byte)column.Scale;
					parameter.Size = 0;
					break;
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
					parameter.Precision = 0;
					parameter.Scale = 0;
					parameter.Size = column.Size;
					break;
				default:
					parameter.Precision = 0;
					parameter.Scale = 0;
					parameter.Size = 0;
					break;
			}
			parameter.Direction = ParameterDirection.Input;
			parameter.Nullable = column.Nullable;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateProperty(this ColumnDesignerInfo column, DataConditionPropertyElement property)
		{
			column.CreateProperty(property, false);
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateProperty(this ColumnDesignerInfo column, DataConditionPropertyElement property, bool updated)
		{
			if (!updated)
				property.Name = StringHelper.GetPascalCase(column.Name);
			property.Type = StringHelper.DbTypeToNetType(column.DbType);
			property.PrimaryKey = column.PrimaryKey;
			property.Comment = column.Comment;
			property.Profix = column.Alias;
			property.Column = column.Name;
			property.DbType = column.DbType;
			property.Nullable = column.Nullable;
			property.Precision = column.Precision;
			property.Scale = column.Scale;
			property.Size = column.Size;
			property.DefaultValue = column.DefaultValue;
			property.Computed = column.Computed;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		public static void CreateProperty(this ColumnDesignerInfo column, DataEntityPropertyElement property)
		{
			column.CreateProperty(property, false);
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateProperty(this ColumnDesignerInfo column, DataEntityPropertyElement property, bool updated)
		{
			if (!updated)
				property.Name = StringHelper.GetPascalCase(column.Name);
			property.Type = StringHelper.DbTypeToNetType(column.DbType);
			property.PrimaryKey = column.PrimaryKey;
			property.Comment = column.Comment;
			property.Profix = column.Alias;
			property.Column = column.Name;
			property.DbType = column.DbType;
			property.Nullable = column.Nullable;
			property.Precision = column.Precision;
			property.Scale = column.Scale;
			property.Size = column.Size;
			property.DefaultValue = column.DefaultValue;
			property.Computed = column.Computed;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateProperty(this FunctionParameterInfo parameter, DataConditionPropertyElement property)
		{
			parameter.CreateProperty(property, false);
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="parameter">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateProperty(this FunctionParameterInfo parameter, DataConditionPropertyElement property, bool updated)
		{
			if (!updated)
				property.Name = StringHelper.GetPascalCase(parameter.Name);
			property.Type = StringHelper.DbTypeToNetType(parameter.ParameterType);
			property.PrimaryKey = false;
			property.Column = parameter.Name;
			property.DbType = parameter.ParameterType;
			property.Nullable = parameter.Nullable;
			property.Precision = (byte)parameter.Precision;
			property.Scale = parameter.Scale;
			property.Size = parameter.Size;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateParameter(this FunctionParameterInfo column, CommandParameter parameter)
		{
			parameter.Name = column.Name;
			parameter.SourceColumn = column.Name;
			parameter.ParameterType = column.ParameterType;
			switch (column.ParameterType)
			{
				case DbTypeEnum.Decimal:
					parameter.Precision = column.Precision;
					parameter.Scale = (byte)column.Scale;
					parameter.Size = 0;
					break;
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
					parameter.Precision = 0;
					parameter.Scale = 0;
					parameter.Size = column.Size;
					break;
				default:
					parameter.Precision = 0;
					parameter.Scale = 0;
					parameter.Size = 0;
					break;
			}
			parameter.Direction = ParameterDirection.Input;
			parameter.Nullable = column.Nullable;
		}
	}
}
