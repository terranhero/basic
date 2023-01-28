using Basic.EntityLayer;
using System.Windows;
using System.Windows.Data;

namespace Basic.Primitives
{
    /// <summary>
    /// 实体类 AbstractEntity 方法扩展。
    /// </summary>
    public static class AbstractEntityExternsion
    {
        /// <summary>
        /// 设置实体属性的值
        /// </summary>
        /// <param name="entity">需要扩展 AbstractEntity 类实例。</param>
        /// <param name="elements">需要显示验证异常的控件集合。</param>
        public static bool Validation(this AbstractEntity entity, System.Windows.Controls.UIElementCollection elements)
        {
            foreach (UIElement element in elements)
            {
                //Binding.GetXmlNamespaceManager();
            }
            return false;
        }
    }
}
