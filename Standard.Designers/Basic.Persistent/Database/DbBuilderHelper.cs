using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.EntityLayer;
using Basic.Designer;
using Basic.Enums;
using Basic.DataEntities;

namespace Basic.Database
{
	/// <summary>
	/// 数据库实体构建类帮助器
	/// </summary>
	internal static class DbHelper
	{
		/// <summary>
		/// 表示从 DbTypeEnum 枚举类型转换成 .Net 类型辅助方法。
		/// </summary>
		/// <param name="dbType">DbTypeEnum 枚举类型，表示数据库字段类型。</param>
		/// <returns>返回 .Net 类型的字符串表示形式。</returns>
		public static string DbTypeToNetTypeString(DbTypeEnum dbType)
		{
			switch (dbType)
			{
				case DbTypeEnum.Guid:
					return "System.Guid";

				case DbTypeEnum.Boolean:
					return "bool";

				case DbTypeEnum.Int16:
					return "short";

				case DbTypeEnum.Int32:
					return "int";

				case DbTypeEnum.Int64:
					return "long";

				case DbTypeEnum.Decimal:
					return "decimal";

				case DbTypeEnum.Single:
					return "float";

				case DbTypeEnum.Double:
					return "double";

				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
				case DbTypeEnum.Image:
					return "byte[]";

				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Text:
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.NText:
					return "string";

				case DbTypeEnum.Time:
					return "System.TimeSpan";

				case DbTypeEnum.Date:
				case DbTypeEnum.Timestamp:
				case DbTypeEnum.DateTime:
				case DbTypeEnum.DateTime2:
				case DbTypeEnum.DateTimeOffset:
					return "System.DateTime";
			}
			return "string";
		}

		/// <summary>
		/// 表示从 DbTypeEnum 枚举类型转换成 .Net 类型辅助方法。
		/// </summary>
		/// <param name="dbType">DbTypeEnum 枚举类型，表示数据库字段类型。</param>
		/// <returns>返回 .Net 类型。</returns>
		public static Type DbTypeToNetType(DbTypeEnum dbType)
		{
			switch (dbType)
			{
				case DbTypeEnum.Guid:
					return typeof(Guid);

				case DbTypeEnum.Boolean:
					return typeof(bool);

				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
				case DbTypeEnum.Image:
					return typeof(byte[]);

				case DbTypeEnum.Int16:
					return typeof(short);

				case DbTypeEnum.Int32:
					return typeof(int);

				case DbTypeEnum.Int64:
					return typeof(long);

				case DbTypeEnum.Decimal:
					return typeof(decimal);

				case DbTypeEnum.Single:
					return typeof(float);

				case DbTypeEnum.Double:
					return typeof(double);

				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Text:
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.NText:
					return typeof(string);
				case DbTypeEnum.Time:
					return typeof(System.TimeSpan);

				case DbTypeEnum.Date:
				case DbTypeEnum.Timestamp:
				case DbTypeEnum.DateTime:
				case DbTypeEnum.DateTime2:
				case DbTypeEnum.DateTimeOffset:
					return typeof(System.DateTime);
			}
			return typeof(string);
		}
	}

	/// <summary>
	/// 数据库实体构建类帮助器
	/// </summary>
	internal static class DbBuilderHelper
	{
		/// <summary>
		/// 表示从 DbTypeEnum 枚举类型转换成 .Net 类型辅助方法。
		/// </summary>
		/// <param name="dbType">DbTypeEnum 枚举类型，表示数据库字段类型。</param>
		/// <returns>返回 .Net 类型的字符串表示形式。</returns>
		public static string DbTypeToNetTypeString(DbTypeEnum dbType)
		{
			switch (dbType)
			{
				case DbTypeEnum.Guid:
					return "System.Guid";

				case DbTypeEnum.Boolean:
					return "bool";

				case DbTypeEnum.Int16:
					return "short";

				case DbTypeEnum.Int32:
					return "int";

				case DbTypeEnum.Int64:
					return "long";

				case DbTypeEnum.Decimal:
					return "decimal";

				case DbTypeEnum.Single:
					return "float";

				case DbTypeEnum.Double:
					return "double";

				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
				case DbTypeEnum.Image:
					return "byte[]";

				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Text:
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.NText:
					return "string";

				case DbTypeEnum.Time:
					return "System.TimeSpan";

				case DbTypeEnum.Date:
				case DbTypeEnum.Timestamp:
				case DbTypeEnum.DateTime:
				case DbTypeEnum.DateTime2:
				case DbTypeEnum.DateTimeOffset:
					return "System.DateTime";
			}
			return "string";
		}

		/// <summary>
		/// 表示从 DbTypeEnum 枚举类型转换成 .Net 类型辅助方法。
		/// </summary>
		/// <param name="dbType">DbTypeEnum 枚举类型，表示数据库字段类型。</param>
		/// <returns>返回 .Net 类型。</returns>
		public static Type DbTypeToNetType(DbTypeEnum dbType)
		{
			switch (dbType)
			{
				case DbTypeEnum.Guid:
					return typeof(Guid);

				case DbTypeEnum.Boolean:
					return typeof(bool);

				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
				case DbTypeEnum.Image:
					return typeof(byte[]);

				case DbTypeEnum.Int16:
					return typeof(short);

				case DbTypeEnum.Int32:
					return typeof(int);

				case DbTypeEnum.Int64:
					return typeof(long);

				case DbTypeEnum.Decimal:
					return typeof(decimal);

				case DbTypeEnum.Single:
					return typeof(float);

				case DbTypeEnum.Double:
					return typeof(double);

				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Text:
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.NText:
					return typeof(string);
				case DbTypeEnum.Time:
					return typeof(System.TimeSpan);

				case DbTypeEnum.Date:
				case DbTypeEnum.Timestamp:
				case DbTypeEnum.DateTime:
				case DbTypeEnum.DateTime2:
				case DbTypeEnum.DateTimeOffset:
					return typeof(System.DateTime);
			}
			return typeof(string);
		}

		/// <summary>
		/// 创建实体类属性。
		/// </summary>
		/// <param name="entity">需要创建属性的 DataEntityElement 类实例。</param>
		/// <param name="column">数据库列信息，一个 DesignColumnInfo 类实例。</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateAbstractProperty(DataEntityElement entity, DesignColumnInfo column, bool updated = false)
		{
			string propertyName = column.PropertyName;
			if (entity.Properties.TryGetValue(propertyName, out DataEntityPropertyElement property) == false)
			{
				property = new DataEntityPropertyElement(entity);
				((DataEntityPropertyElement)null).Column = column.Name;
				if (!updated)
					((DataEntityPropertyElement)null).Name = propertyName;
				entity.Properties.Add(null);
			}
			if (!updated)
				((DataEntityPropertyElement)null).Name = propertyName;
			((DataEntityPropertyElement)null).Type = DbTypeToNetType(column.DbType);
			((DataEntityPropertyElement)null).PrimaryKey = column.PrimaryKey;
			((DataEntityPropertyElement)null).Comment = column.Comment;
			((DataEntityPropertyElement)null).Column = column.Name;
			((DataEntityPropertyElement)null).DbType = column.DbType;
			((DataEntityPropertyElement)null).Nullable = column.Nullable;
			((DataEntityPropertyElement)null).Precision = column.Precision;
			((DataEntityPropertyElement)null).Scale = column.Scale;
			((DataEntityPropertyElement)null).Size = column.Size;

			if (!((DataEntityPropertyElement)null).Nullable && !((DataEntityPropertyElement)null).PrimaryKey)
			{
				AbstractAttribute abstractValidation = null;
				if (!((DataEntityPropertyElement)null).Attributes.TryGetValue(RequiredValidation.XmlElementName, out abstractValidation))
				{
					abstractValidation = new RequiredValidation(null);
					((DataEntityPropertyElement)null).Attributes.Add(abstractValidation);
				}
			}
			if (((DataEntityPropertyElement)null).Type == typeof(string))
			{
				AbstractAttribute abstractValidation = null;
				if (!((DataEntityPropertyElement)null).Attributes.TryGetValue(StringLengthValidation.XmlElementName, out abstractValidation))
				{
					abstractValidation = new StringLengthValidation(null);
					((DataEntityPropertyElement)null).Attributes.Add(abstractValidation);
				}
				StringLengthValidation stringValidation = abstractValidation as StringLengthValidation;
				stringValidation.MaximumLength = column.Size;
			}
			if (((DataEntityPropertyElement)null).Type == typeof(DateTime) || ((DataEntityPropertyElement)null).Type == typeof(DateTimeOffset) || ((DataEntityPropertyElement)null).Type == typeof(TimeSpan))
			{
				AbstractAttribute abstractDisplay = null;
				if (!((DataEntityPropertyElement)null).Attributes.TryGetValue(DisplayFormat.XmlElementName, out abstractDisplay))
				{
					abstractDisplay = new DisplayFormat(null);
					((DataEntityPropertyElement)null).Attributes.Add(abstractDisplay);
				}
				DisplayFormat displayFormat = abstractDisplay as DisplayFormat;
				if (!string.IsNullOrWhiteSpace(displayFormat.DataFormatString)) { return; }
				switch (((DataEntityPropertyElement)null).DbType)
				{
					case DbTypeEnum.DateTime:
					case DbTypeEnum.DateTimeOffset:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}";
						break;
					case DbTypeEnum.Date:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd}";
						break;
					case DbTypeEnum.Time:
						displayFormat.DataFormatString = @"{0:hh\:mm\:ss}";
						break;
					case DbTypeEnum.Timestamp:
					case DbTypeEnum.DateTime2:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd HH:mm:ss.fff}";
						break;
				}
			}
		}

		/// <summary>
		/// 创建实体属性信息。
		/// </summary>
		/// <param name="property">表示一个 DataEntityPropertyElement 类实例的实体属性。</param>
		/// <param name="column">数据库列信息，一个 DesignColumnInfo 类实例。</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateAbstractProperty(DataEntityPropertyElement property, DesignColumnInfo column, bool updated)
		{
			if (!updated)
				property.Name = column.PropertyName;
			property.Type = DbTypeToNetType(column.DbType);
			property.PrimaryKey = column.PrimaryKey;
			property.Comment = column.Comment;

			property.Column = column.Name;
			property.DbType = column.DbType;
			property.Nullable = column.Nullable;
			property.Precision = column.Precision;
			property.Scale = column.Scale;
			property.Size = column.Size;

			if (!property.Nullable && !property.PrimaryKey)
			{
				if (property.DbType != DbTypeEnum.Boolean)
				{
					AbstractAttribute abstractValidation = null;
					if (!property.Attributes.TryGetValue(RequiredValidation.XmlElementName, out abstractValidation))
					{
						abstractValidation = new RequiredValidation(property);
						property.Attributes.Add(abstractValidation);
					}
				}
			}
			if (property.Type == typeof(string))
			{
				AbstractAttribute abstractValidation = null;
				if (!property.Attributes.TryGetValue(StringLengthValidation.XmlElementName, out abstractValidation))
				{
					abstractValidation = new StringLengthValidation(property);
					property.Attributes.Add(abstractValidation);
				}
				StringLengthValidation stringValidation = abstractValidation as StringLengthValidation;
				stringValidation.MaximumLength = column.Size;
			}
			if (property.Type == typeof(DateTime) || property.Type == typeof(DateTimeOffset) || property.Type == typeof(TimeSpan))
			{
				AbstractAttribute abstractDisplay = null;
				if (!property.Attributes.TryGetValue(DisplayFormat.XmlElementName, out abstractDisplay))
				{
					abstractDisplay = new DisplayFormat(property);
					property.Attributes.Add(abstractDisplay);
				}
				DisplayFormat displayFormat = abstractDisplay as DisplayFormat;
				if (!string.IsNullOrWhiteSpace(displayFormat.DataFormatString)) { return; }
				switch (property.DbType)
				{
					case DbTypeEnum.DateTime:
					case DbTypeEnum.DateTimeOffset:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}";
						break;
					case DbTypeEnum.Date:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd}";
						break;
					case DbTypeEnum.Time:
						displayFormat.DataFormatString = @"{0:hh\:mm\:ss}";
						break;
					case DbTypeEnum.Timestamp:
					case DbTypeEnum.DateTime2:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd HH:mm:ss.fff}";
						break;
				}
			}
		}

		/// <summary>
		/// 创建条件实体属性信息。
		/// </summary>
		/// <param name="property">表示一个 DataConditionPropertyElement 类实例的实体属性。</param>
		/// <param name="column">数据库列信息，一个 DesignColumnInfo 类实例。</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateAbstractProperty(DataConditionPropertyElement property, DesignColumnInfo column, bool updated = false)
		{
			if (!updated)
				property.Name = column.PropertyName;
			property.Type = DbTypeToNetType(column.DbType);
			property.PrimaryKey = column.PrimaryKey;
			property.Comment = column.Comment;

			property.Column = column.Name;
			property.DbType = column.DbType;
			property.Nullable = column.Nullable;
			property.Precision = column.Precision;
			property.Scale = column.Scale;
			property.Size = column.Size;
		}

		/// <summary>
		/// 创建条件实体属性信息。
		/// </summary>
		/// <param name="property">表示一个 DataConditionPropertyElement 类实例的实体属性。</param>
		/// <param name="column">数据库列信息，一个 DesignColumnInfo 类实例。</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateAbstractProperty(DataConditionPropertyElement property, ProcedureParameter column, bool updated = false)
		{
			if (!updated)
				property.Name = StringHelper.GetPascalCase(column.Name);
			property.Type = DbTypeToNetType(column.DbType);

			property.Column = column.Name;
			property.DbType = column.DbType;
			property.Nullable = column.Nullable;
			property.Precision = (byte)column.Precision;
			property.Scale = (byte)column.Scale;
			property.Size = column.Size;
		}
	}
}