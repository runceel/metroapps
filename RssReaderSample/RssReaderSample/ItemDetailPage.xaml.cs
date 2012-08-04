using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RssReaderSample.DataModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// アイテム詳細ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234232 を参照してください

namespace RssReaderSample
{
    /// <summary>
    /// グループ内の単一のアイテムに関する詳細情報を表示するページです。同じグループに属する他の
    /// アイテムにフリップするジェスチャを使用できます。
    /// </summary>
    public sealed partial class ItemDetailPage : RssReaderSample.Common.LayoutAwarePage
    {
        public ItemDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += this.DateRequested;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DataTransferManager.GetForCurrentView().DataRequested -= this.DateRequested;
        }

        private void DateRequested(object sender, DataRequestedEventArgs e)
        {
            var selectedItem = flipView.SelectedItem as FeedItem;
            e.Request.Data.Properties.Title = selectedItem.Title;
            e.Request.Data.Properties.Description = selectedItem.Subtitle;
            e.Request.Data.SetUri(selectedItem.Link);
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var model = RssReaderSampleModel.GetDefault();

            var id = navigationParameter as string;

            var blog = model.Blogs.Single(b => b.Feeds.Any(i => i.Id == id));
            this.DefaultViewModel["Blog"] = blog;
            this.itemsViewSource.View.MoveCurrentTo(blog.Feeds.Single(i => i.Id == id));
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
