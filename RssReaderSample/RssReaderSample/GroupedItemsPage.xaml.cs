using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RssReaderSample.DataModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// グループ化されたアイテム ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234231 を参照してください

namespace RssReaderSample
{
    /// <summary>
    /// グループ化されたアイテムのコレクションを表示するページです。
    /// </summary>
    public sealed partial class GroupedItemsPage : RssReaderSample.Common.LayoutAwarePage
    {
        public GroupedItemsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will 5rbe null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            this.DefaultViewModel["RssModel"] = RssReaderSampleModel.GetDefault();
        }

        /// <summary>
        /// グループ ヘッダーがクリックされたときに呼び出されます。
        /// </summary>
        /// <param name="sender">ボタンは、選択されたグループのグループ ヘッダーとして使用されます。</param>
        /// <param name="e">クリックがどのように開始されたかを説明するイベント データ。</param>
        private void Header_Click(object sender, RoutedEventArgs e)
        {
            var blog = ((FrameworkElement)sender).DataContext as Blog;
            this.Frame.Navigate(typeof(GroupDetailPage), blog.Id);
        }

        /// <summary>
        /// グループ内のアイテムがクリックされたときに呼び出されます。
        /// </summary>
        /// <param name="sender">クリックされたアイテムを表示する GridView (アプリケーションがスナップ
        /// されている場合は ListView) です。</param>
        /// <param name="e">クリックされたアイテムを説明するイベント データ。</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FeedItem;
            if (item == null)
            {
                return;
            }

            this.Frame.Navigate(typeof(ItemDetailPage), item.Id);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlogManagePage));
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await RssReaderSampleModel.GetDefault().LoadFeeds();
        }
    }
}
