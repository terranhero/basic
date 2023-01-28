using System.Collections.Generic;
using Basic.Localizations;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Basic.Editors
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly LocalizationCollection resourceCollection;
		public MainPage()
		{
			resourceCollection = new LocalizationCollection();
			this.InitializeComponent();
			dgResx.ItemsSource = resourceCollection;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
			if (e.Parameter is IReadOnlyList<IStorageItem> files)
			{
				IStorageItem item = files[0];
				resourceCollection.Load(item.Path);
			}
		}
	}
}
