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
    public sealed partial class AddFeedFlyout : UserControl
    {
        // フィードの登録処理が完了したことを通知するイベント
        public event EventHandler<EventArgs> AddFeedFinished;
        private void RaiseAddFeedFinished()
        {
            var h = this.AddFeedFinished;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        public AddFeedFlyout()
        {
            this.InitializeComponent();
        }

        private void AddFeedButton_Click(object sender, RoutedEventArgs e)
        {
            // 入力値を確認する
            Uri feedUri;
            if (!Uri.TryCreate(this.textBoxUri.Text, UriKind.Absolute, out feedUri))
            {
                // Uriとして正しくない場合はメッセージを設定して終了
                this.textBlockMessage.Text = "登録するフィードのURLを入力してください";
                return;
            }

            // Uriが作成できた場合は追加処理を行う。
            // 非同期処理を待たないことを明示するために、Taskを変数で受けています。
            var noWaitTask = RssReaderSampleModel.GetDefault().CreateFeedAsync(feedUri);
            // フィードの登録処理が終わったことを外部に通知する
            this.RaiseAddFeedFinished();
        }
    }
}
