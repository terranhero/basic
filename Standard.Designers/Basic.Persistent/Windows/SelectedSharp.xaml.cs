using System.Windows;
using System.Windows.Controls;

namespace Basic.Windows
{
    /// <summary>
    /// SelectedAdorner.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedSharp : Control
    {
        static SelectedSharp()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectedSharp), new FrameworkPropertyMetadata(typeof(SelectedSharp)));
        }


        public SelectedSharp()
        {
        }
    }
}
