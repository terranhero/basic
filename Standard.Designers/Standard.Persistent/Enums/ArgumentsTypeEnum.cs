using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
    /// <summary>
    /// 表示执行数据库命令参数类型
    /// </summary>
    public enum ArgumentsTypeEnum
    {
        /// <summary>
        /// 单实例模型参数
        /// </summary>
        SingleModel = 1,
        /// <summary>
        /// 模型数组参数
        /// </summary>
        ArrayModels = 2,
        /// <summary>
        /// 按数据库参数
        /// </summary>
        Parameters = 3,
        /// <summary>
        /// 没有参数参数
        /// </summary>
        NoneArguments = 4
    }
}