using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Designer
{
    /// <summary>
    /// 属性排序
    /// </summary>
    public sealed class PropertyOrderAttribute : Attribute
    {
        /// <summary>
        /// 属性顺序编号
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 初始化 PropertyOrderAttribute 类实例
        /// </summary>
        /// <param name="index">编号</param>
        public PropertyOrderAttribute(int index) { Index = index; }
    }
}
