using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
    /// <summary>
    /// 特性的显示类型
    /// </summary>
    public enum DisplayTypeEnum : byte
    {
        /// <summary>
        /// 采用 System.ComponentModel.DisplayNameAttribute 子类作为显示特性。
        /// 此特性允许系统在显示时做文本本地化转换。
        /// </summary>
        WebDisplayAttribute = 1,
        /// <summary>
        /// 采用 System.ComponentModel.DataAnnotations.DisplayAttribute 类作为显示特性。
        /// </summary>
        DisplayAttribute = 2
    }
}