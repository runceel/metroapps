using RssReaderSample.DataModel;
using RssReaderSample.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
        private string navigationFeedId;

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
            await Refresh();

            var model = RssReaderSampleModel.GetDefault();
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

        private void AddFeedButton_Click(object sender, RoutedEventArgs e)
        {
            // フライアウトを作成
            var addFeedView = new AddFeedFlyout();
            var popup = FlyoutUtils.CreateFlyout(this.BottomAppBar, (Button)sender, addFeedView);

            // フィードの登録が終わったらAppBarを閉じる（連動してフライアウトも閉じられます）
            addFeedView.AddFeedFinished += (_, __) =>
            {
                this.BottomAppBar.IsOpen = false;
            };

            // フライアウトを表示
            popup.IsOpen = true;
        }

        private async void RefreshFeedButton_Click(object sender, RoutedEventArgs e)
        {
            // 再読み込み処理を呼ぶ
            await Refresh();
        }

        /// <summary>
        /// 再読み込み処理
        /// </summary>
        /// <returns></returns>
        private async Task Refresh()
        {
            // プログレスバーを表示します。
            var grid = this.Content as Grid;
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(100, 200, 200, 200)),
                Child = new ProgressBar
                {
                    IsIndeterminate = true
                }
            };
            Grid.SetRowSpan(border, 2);
            grid.Children.Add(border);

            // 操作不可に設定
            this.BottomAppBar.IsOpen = false;
            this.BottomAppBar.IsEnabled = false;
            this.IsEnabled = false;

            try
            {
                // データの読み込み
                await RssReaderSampleModel.GetDefault().LoadAllFeedsAsync();
            }
            finally
            {
                // データの読み込みが完了したので後始末
                grid.Children.Remove(border);
                this.BottomAppBar.IsEnabled = true;
                this.IsEnabled = true;
            }
        }
    }
}
