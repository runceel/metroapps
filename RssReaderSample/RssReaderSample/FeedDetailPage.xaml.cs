using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RssReaderSample.DataModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// グループ詳細ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234229 を参照してください

namespace RssReaderSample
{
    /// <summary>
    /// 単一のグループ内のアイテムのプレビューを含め、グループの概要を表示する
    /// ページです。
    /// </summary>
    public sealed partial class FeedDetailPage : RssReaderSample.Common.LayoutAwarePage
    {
        public FeedDetailPage()
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var id = (string)navigationParameter;
            var feed = RssReaderSampleModel.GetDefault().GetFeedById(id);
            this.DefaultViewModel["Group"] = feed;
            this.DefaultViewModel["Items"] = feed.FeedItems;
        }

        private void FeedItem_Click(object sender, ItemClickEventArgs e)
        {
            var feedItem = e.ClickedItem as FeedItem;
            this.Frame.Navigate(typeof(FeedItemDetailPage), feedItem.Id);
        }
    }
}
