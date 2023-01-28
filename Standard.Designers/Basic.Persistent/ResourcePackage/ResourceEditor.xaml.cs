using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using Basic.Designer;
using Microsoft.VisualStudio.Shell;
using STT = System.Threading.Tasks;

namespace Basic.Localizations
{

	/// <summary>
	/// ResourceEditor.xaml 的交互逻辑
	/// </summary>
	public partial class ResourceEditor : DataGrid
	{
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
			InitializeComponent();
			//this.Loaded += new RoutedEventHandler(ResourceEditor_Loaded);
			resourceCollection = collection;
			resourceView = CollectionViewSource.GetDefaultView(resourceCollection);
			ItemsSource = resourceView;
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

	}
}
