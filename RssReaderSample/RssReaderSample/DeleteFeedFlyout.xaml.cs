using RssReaderSample.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace RssReaderSample
{
    public sealed partial class DeleteFeedFlyout : UserControl
    {
        // 削除が完了したことを外部に通知するイベント
        public event EventHandler DeleteFeedFinished;
        private void RaiseDeleteFeedFinished()
        {
            var h = this.DeleteFeedFinished;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        public DeleteFeedFlyout()
        {
            this.InitializeComponent();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            // DataContextにあるFeedを取り出して削除
            var feed = this.DataContext as Feed;
            RssReaderSampleModel.GetDefault().Feeds.Remove(feed);
            // 削除完了イベントを通知
            this.RaiseDeleteFeedFinished();
        }
    }
}
