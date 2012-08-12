using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RssReaderSample.DataModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class MainPage : RssReaderSample.Common.LayoutAwarePage
    {
        public MainPage()
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
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var model = RssReaderSampleModel.GetDefault();
            await model.LoadAllFeeds();
            this.DefaultViewModel["Groups"] = model.Feeds;
        }

        private void FeedItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FeedItem;
            this.Frame.Navigate(typeof(FeedItemDetailPage), item.Id);
        }

        private void FeedHeader_Click(object sender, RoutedEventArgs e)
        {
            // senderのDataContextに、Feedが入ってる
            var feed = ((FrameworkElement)sender).DataContext as Feed;
            this.Frame.Navigate(typeof(FeedDetailPage), feed.Id);
        }
    }
}
