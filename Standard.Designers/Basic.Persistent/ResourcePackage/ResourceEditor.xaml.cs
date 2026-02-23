using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Basic.Localizations
{
    /// <summary>
    /// ResourceEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceEditor : DataGrid
    {
        static ResourceEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResourceEditor), new FrameworkPropertyMetadata(typeof(ResourceEditor)));
        }

        private class LocalizationCollectionView : CollectionView
        {
            private readonly LocalizationCollection resourceCollection;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="resourceCollection"></param>
            public LocalizationCollectionView(LocalizationCollection resxCollection)
                : base(resxCollection) { resourceCollection = resxCollection; }

            /// <summary>
            /// 资源组名称集合
            /// </summary>
            public GroupNameCollection GroupNames { get { return resourceCollection.GroupNames; } }

            /// <summary>
            /// 表示当前本地化资源文件支持的本地化信息。
            /// </summary>
            public ObservableCollection<CultureInfo> CultureInfos { get { return resourceCollection.CultureInfos; } }

            /// <summary>
            /// 表示当前本地化资源文件可用的区域信息。
            /// </summary>
            public ObservableCollection<CultureInfo> EnabledCultureInfos { get { return resourceCollection.EnabledCultureInfos; } }
        }
        private readonly LocalizationCollection resourceCollection;
        private readonly ICollectionView resourceView;
        public LocalizationCollection Localizations { get { return resourceCollection; } }

        /// <summary>
        /// 资源编辑器
        /// </summary>
        public ResourceEditor(LocalizationCollection collection)
        {
            //this.Loaded += new RoutedEventHandler(ResourceEditor_Loaded);
            resourceCollection = collection;
            resourceView = CollectionViewSource.GetDefaultView(resourceCollection);
            ItemsSource = resourceView;
            // this.Columns.Add
            DataGridColumnBuilder.AddAllColumns(this);
        }

        /// <summary>
        /// 初始化 LocalizationResxResource 类实例
        /// </summary>
        /// <param name="resxData">需要添加的资源信息。</param>
        public void Add(LocalizationItem resxData)
        {
            if (!resourceCollection.ContainsName(resxData.Name))
            {
                LocalizationItem newresx = resourceCollection.Add(resxData.Group, resxData.Name, resxData.Value);
                foreach (CultureInfo culture in resourceCollection.CultureInfos)
                {
                    newresx[culture.Name] = resxData[culture.Name];
                }
            }
        }

        //private void ResourceEditor_Loaded(object sender, RoutedEventArgs e)
        //{
        //	//resourceView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
        //	base.ItemsSource = resourceView;
        //}

        public void Grouping()
        {
            resourceView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
        }

        public void UnGroup()
        {
            resourceView.GroupDescriptions.Clear();
        }

        /// <summary>
        /// 引发 System.Windows.Controls.DataGrid.LoadingRow 事件。
        /// </summary>
        /// <param name="e">事件的相关数据。</param>
        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            if (box.SelectedItem != null)
            {
                string groupName = (string)box.SelectedItem;
                resourceView.Filter = new Predicate<object>((model) =>
                {
                    LocalizationItem res = model as LocalizationItem;
                    return res.Group == groupName;
                });
            }
            else
                resourceView.Filter = null;
        }

        private void OnCanExecutedFind(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = SelectedItem != null && resourceView.Filter == null;
        }

        private void OnExecutedFind(object sender, ExecutedRoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                LocalizationItem item = (LocalizationItem)SelectedItem;
                resourceView.Filter = new Predicate<object>((model) =>
                {
                    LocalizationItem res = model as LocalizationItem;
                    return res.Group == item.Group;
                });
            }
        }

        private void OnCanExecutedUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = resourceView.Filter != null;
        }

        private void OnExecutedUndo(object sender, ExecutedRoutedEventArgs e)
        {
            resourceView.Filter = null;
        }

        public static class DataGridColumnBuilder
        {
            /// <summary>
            /// 创建组名列
            /// </summary>
            public static DataGridTextColumn CreateGroupColumn()
            {
                return new DataGridTextColumn
                {
                    Header = "组名",
                    Width = 150,
                    MinWidth = 100,
                    CanUserSort = false,
                    Binding = new Binding("Group")
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidatesOnExceptions = true
                    },
                    ElementStyle = CreateTextBlockStyle(),
                    EditingElementStyle = CreateTextBoxStyle()
                };
            }

            /// <summary>
            /// 创建名称列
            /// </summary>
            public static DataGridTextColumn CreateNameColumn()
            {
                return new DataGridTextColumn
                {
                    Header = "名称",
                    Width = 230,
                    MinWidth = 150,
                    CanUserSort = true,  // 默认允许排序
                    Binding = new Binding("Name")
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidatesOnExceptions = true
                    },
                    ElementStyle = CreateTextBlockStyle(),
                    EditingElementStyle = CreateTextBoxStyle()
                };
            }

            /// <summary>
            /// 创建资源值列
            /// </summary>
            public static DataGridTextColumn CreateValueColumn()
            {
                return new DataGridTextColumn
                {
                    Header = "资源值",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),  // Width="*"
                    MinWidth = 100,
                    CanUserSort = false,
                    Binding = new Binding("Value")
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidatesOnExceptions = true
                    },
                    ElementStyle = CreateTextBlockStyle(),
                    EditingElementStyle = CreateTextBoxStyle(includeMargin: false)  // Value列没有Margin设置
                };
            }

            /// <summary>
            /// 创建注释列
            /// </summary>
            public static DataGridTextColumn CreateCommentColumn()
            {
                return new DataGridTextColumn
                {
                    Header = "注释",
                    Width = 100,
                    MinWidth = 100,
                    CanUserSort = false,
                    Binding = new Binding("Comment")
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidatesOnExceptions = true
                    },
                    ElementStyle = CreateTextBlockStyle(),
                    EditingElementStyle = CreateTextBoxStyle(includeMargin: false)  // 注释列没有Margin设置
                };
            }

            /// <summary>
            /// 创建通用的TextBlock样式（用于显示模式）
            /// </summary>
            private static Style CreateTextBlockStyle()
            {
                var style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
                style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left));
                style.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(6)));
                return style;
            }

            /// <summary>
            /// 创建通用的TextBox样式（用于编辑模式）
            /// </summary>
            /// <param name="includeMargin">是否包含Margin设置</param>
            private static Style CreateTextBoxStyle(bool includeMargin = true)
            {
                var style = new Style(typeof(TextBox));
                style.Setters.Add(new Setter(TextBox.VerticalAlignmentProperty, VerticalAlignment.Center));
                style.Setters.Add(new Setter(TextBox.PaddingProperty, new Thickness(4, 5, 4, 5)));

                if (includeMargin)
                {
                    style.Setters.Add(new Setter(TextBox.MarginProperty, new Thickness(0)));
                }

                return style;
            }

            /// <summary>
            /// 批量添加所有列到DataGrid
            /// </summary>
            public static void AddAllColumns(DataGrid dataGrid)
            {
                dataGrid.Columns.Clear();
                dataGrid.Columns.Add(CreateGroupColumn());
                dataGrid.Columns.Add(CreateNameColumn());
                dataGrid.Columns.Add(CreateValueColumn());
                dataGrid.Columns.Add(CreateCommentColumn());
            }
        }
    }
}
