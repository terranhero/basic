using System.Windows;

namespace Basic.Windows
{
    /// <summary>
    /// 表示 DesignerCanvas 控件内绝对定位的元素
    /// </summary>
    public interface IDesignerItem : IInputElement
    {
        /// <summary>
        /// 当前设计项是否被选中
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// 获取或设置元素参与数据绑定时的数据上下文。
        /// </summary>
        /// <value>要用作数据上下文的对象。</value>
        object DataContext { get; set; }

        /// <summary>
        /// 当前选定项的逻辑值
        /// </summary>
        object SelectedObject { get; }

        /// <summary>
        /// 获取或设置元素的宽度。
        /// </summary>
        /// <value>元素的宽度，单位是device-independent units (1/96th inch per unit)。
        /// 默认值为 System.Double.NaN。
        /// 此值必须大于等于 0.0。有关上限信息，请参见“备注”。</value>
        double Width { get; set; }

        /// <summary>
        /// 获取或设置元素的建议高度。
        /// </summary>
        /// <value>元素的高度（采用device-independent units (1/96th inch per unit)）。
        /// 默认值为 System.Double.NaN。
        /// 此值必须大于等于 0.0。有关上限信息，请参见“备注”。</value>
        double Height { get; set; }
    }
}
