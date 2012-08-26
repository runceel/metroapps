using RssReaderSample.DataModel;
using RssReaderSample.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.StartScreen;
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
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="navigationParameter">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたパラメーター値。
        /// </param>
        /// <param name="pageState">前のセッションでこのページによって保存された状態の
        /// ディクショナリ。ページに初めてアクセスするとき、状態は null になります。</param>
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

        private void DeleteFeedButton_Click(object sender, RoutedEventArgs e)
        {
            // 現在のフィードをDataContextに設定してフライアウトを作成
            var deleteFeedView = new DeleteFeedFlyout
            {
                DataContext = this.DefaultViewModel["Group"]
            };
            var popup = FlyoutUtils.CreateFlyout(this.BottomAppBar, (Button)sender, deleteFeedView);

            // 削除完了通知がきたらPopupを閉じて前の画面に戻る
            deleteFeedView.DeleteFeedFinished += (_, __) =>
            {
                popup.IsOpen = false;
                this.Frame.GoBack();
            };

            // Popupを表示する
            popup.IsOpen = true;
        }

        private async void PinFeedButton_Click(object sender, RoutedEventArgs e)
        {
            // フィードを取得
            var feed = this.DefaultViewModel["Group"] as Feed;
            // すでにタイルが存在する場合は何もしない
            if (SecondaryTile.Exists(feed.Id))
            {
                return;
            }

            // デフォルトのロゴでフィードのIDをタイルのIDに設定してタイルを作成
            var tile = new SecondaryTile(
                feed.Id,
                feed.Title,
                feed.Title,
                feed.Id,
                TileOptions.ShowNameOnLogo,
                new Uri("ms-appx:///Assets/Logo.png"));
            await tile.RequestCreateAsync();
        }
    }
}
