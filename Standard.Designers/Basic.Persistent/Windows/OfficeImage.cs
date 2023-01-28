using System.Drawing;
using System.Resources;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Basic.Windows
{
    /// <summary>
    /// 表示显示图像的控件(显示资源图像)。
    /// </summary>
    [System.ComponentModel.ToolboxItem(false)]
    public sealed class OfficeImage : System.Windows.Controls.Image
    {
        public OfficeImage() { MaxWidth = 16; MaxHeight = 16; }

        #region 属性 Icon 定义
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
            typeof(string), typeof(OfficeImage), new PropertyMetadata(null, OnIconChanged));
        /// <summary>
        /// 判断当前控件是否已经选择。
        /// </summary>
        public string Icon
        {
            get { return (string)base.GetValue(IconProperty); }
            set { base.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OfficeImage control = d as OfficeImage;
            if (e.NewValue != null && e.NewValue != e.OldValue)
            {
                string images = (string)e.NewValue;
                if (images != null)
                {
                    BitmapImage image = new BitmapImage();
                    ResourceManager res = new ResourceManager("Basic.Designer.OfficeImages", typeof(OfficeImage).Assembly);
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                    {
                        image.BeginInit();
                        object obj = res.GetObject(images);
                        if (obj is Icon) { (obj as Icon).Save(stream); }
                        image.StreamSource = stream;
                        image.EndInit();
                    }
                    control.Source = image;
                }
            }
        }
        #endregion
    }
}
