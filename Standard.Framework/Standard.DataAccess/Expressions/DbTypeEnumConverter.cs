using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.EntityLayer;

using Basic.Enums;

namespace Basic.Expressions
{
    /// <summary>
    /// DbTypeEnum 转换器
    /// </summary>
    internal static class DbTypeEnumConverter
    {
        /// <summary>
        /// 将 DataTypeEnum 枚举类型转换成 DbTypeEnum 枚举类型。
        /// </summary>
        /// <param name="dataType">DataTypeEnum 枚举实例。</param>
        /// <returns>返回 DbTypeEnum 实例。</returns>
        public static DbTypeEnum ConvertFrom(DataTypeEnum dataType)
        {
            int value = (int)dataType;
            return (DbTypeEnum)value;
        }

        /// <summary>
        /// 将 DbTypeEnum 枚举类型转换成 DataTypeEnum 枚举类型。
        /// </summary>
        /// <param name="dbType">DataTypeEnum 枚举实例。</param>
        /// <returns>返回 DbTypeEnum 实例。</returns>
        public static DataTypeEnum ConvertTo(DbTypeEnum dbType)
        {
            int value = (int)dbType;
            return (DataTypeEnum)value;
        }
    }
}
