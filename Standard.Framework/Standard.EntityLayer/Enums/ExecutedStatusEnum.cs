using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
    /// <summary>
    /// 表示新值命令执行的方式
    /// </summary>
    public enum ExecutedStatusEnum : byte
    {
        /// <summary>
        /// 每次执行命令都执行一次新值命令，此为默认值。
        /// </summary>
        EveryTime = 1,
        /// <summary>
        /// 如果传入命令的参数是实体数组，则命令仅仅开始执行一次。
        /// 以后没有采用序列自动递增。
        /// </summary>
        OnlyOnce = 2,
    }
}