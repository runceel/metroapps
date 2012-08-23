using RssReaderSample.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="navigationParameter">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたパラメーター値。
        /// </param>
        /// <param name="pageState">前のセッションでこのページによって保存された状態の
        /// ディクショナリ。ページに初めてアクセスするとき、状態は null になります。</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
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
    }
}
