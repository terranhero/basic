using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections.Generic;
using Basic.EntityLayer;
using Basic.Enums;

namespace Basic.Designer
{
	/// <summary>
	/// 代码生成器辅助类
	/// </summary>
	public static class StringHelper
	{
		private readonly static SortedSet<string> keyWords;
		static StringHelper()
		{
			keyWords = new SortedSet<string>(new string[] { "Valid",  "Text", "Code", "Name", "Key", "Time",
				"Date", "User", "Type", "Enum","Enabled","Number","Order" });
		}
		/// <summary>
		/// 判断字符串是否都是大写字符组成
		/// </summary>
		/// <param name="value">需要判断的字符串资源</param>
		/// <returns>如果都是大写则返回true，否则返回false</returns>
		public static bool IsUpper(string value)
		{
			return Regex.IsMatch(value, "^[A-Z]+$");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		private static bool ReplaceReservedKeywords(StringBuilder builder)
		{
			string result = builder.ToString();
			foreach (string keyWord in keyWords)
			{
				if (result.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					builder.Replace(keyWord.ToLower(), keyWord);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 将数据库全大写名称转化成小写的字符串
		/// </summary>
		/// <param name="value">数据库字段或表大写名称</param>
		/// <returns>返回转换成功的字符串</returns>
		public static string GetLowerCase(string value)
		{
			if (value == null) { return value; }
			return value.ToLowerInvariant();
		}


		/// <summary>
		/// 将数据库全大写名称转化成大写的字符串
		/// </summary>
		/// <param name="value">数据库字段或表大写名称</param>
		/// <returns>返回转换成功的字符串</returns>
		public static string GetUpperCase(string value)
		{
			if (value == null) { return value; }
			return value.ToUpperInvariant();
		}

		/// <summary>
		/// 将数据库全大写名称转化成第一个字母大写，其他字符小写的字符串
		/// </summary>
		/// <param name="value">数据库字段或表大写名称</param>
		/// <returns>返回转换成功的字符串</returns>
		public static string GetCamelCase(string value)
		{
			if (value == null) { return value; }
			if (value.IndexOf('_') >= 0)
			{
				StringBuilder builder = new StringBuilder(50);
				string[] strArray = value.Split('_');
				foreach (string str in strArray)
				{
					if (Regex.IsMatch(str, "^[A-Z0-9]+$"))
					{
						builder.Append(str[0]);
						builder.Append(str.Substring(1, str.Length - 1).ToLower());
						ReplaceReservedKeywords(builder);
					}
					else
					{
						builder.Append(str);
					}
				}
				return builder.ToString();
			}
			else
			{
				StringBuilder builder = new StringBuilder(50);
				if (Regex.IsMatch(value, "^[A-Z]+$"))
				{
					builder.Append(value.ToLowerInvariant());
					char firstChar = builder[0];
					char newChar = Char.ToUpperInvariant(firstChar);
					builder.Replace(firstChar, newChar, 0, 1);
					ReplaceReservedKeywords(builder);
				}
				else
				{
					builder.Append(value);
				}
				return builder.ToString();
			}
		}

		/// <summary>
		/// 将数据库全大写名称转化成第一个字母大写，其他字符小写的字符串
		/// </summary>
		/// <param name="value">数据库字段或表大写名称</param>
		/// <returns>返回转换成功的字符串</returns>
		public static string GetPascalCase(string value)
		{
			if (value == null) { return value; }
			if (value.IndexOf('_') >= 0)
			{
				StringBuilder builder = new StringBuilder(50);
				string[] strArray = value.Split('_');
				foreach (string str in strArray)
				{
					if (Regex.IsMatch(str, "^[A-Z0-9]+$"))
					{
						builder.Append(str[0]);
						builder.Append(str.Substring(1, str.Length - 1).ToLower());
						ReplaceReservedKeywords(builder);
					}
					else
					{
						builder.Append(str);
					}
				}
				return builder.ToString();
			}
			else
			{
				StringBuilder builder = new StringBuilder(50);
				if (Regex.IsMatch(value, "^[A-Z]+$"))
				{
					builder.Append(value.ToLowerInvariant());
					char firstChar = builder[0];
					char newChar = Char.ToUpperInvariant(firstChar);
					builder.Replace(firstChar, newChar, 0, 1);
					ReplaceReservedKeywords(builder);
				}
				else
				{
					builder.Append(value);
				}
				return builder.ToString();
			}
		}

		///// <summary>
		///// 将数据库全大写名称转化成第一个字母大写，其他字符小写的字符串
		///// </summary>
		///// <param name="name">数据库字段或表大写名称</param>
		///// <returns>返回转换成功的字符串</returns>
		//public static string GetCsName(string name)
		//{
		//    StringBuilder builder = new StringBuilder(name.ToLowerInvariant(), 50);
		//    char firstChar = builder[0];
		//    char newChar = Char.ToUpperInvariant(firstChar);
		//    builder.Replace(firstChar, newChar, 0, 1);
		//    return builder.ToString();
		//}

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
}
