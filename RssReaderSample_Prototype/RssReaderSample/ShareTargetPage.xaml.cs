using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 共有ターゲット コントラクトのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234241 を参照してください

namespace RssReaderSample
{
    /// <summary>
    /// このページを使用すると、他のアプリケーションがこのアプリケーションを介してコンテンツを共有できます。
    /// </summary>
    public sealed partial class ShareTargetPage : RssReaderSample.Common.LayoutAwarePage
    {
        /// <summary>
        /// 共有操作について、Windows と通信するためのチャネルを提供します。
        /// </summary>
        private Windows.ApplicationModel.DataTransfer.ShareTarget.ShareOperation _shareOperation;

        public ShareTargetPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 他のアプリケーションがこのアプリケーションを介してコンテンツの共有を求めた場合に呼び出されます。
        /// </summary>
        /// <param name="args">Windows と連携して処理するために使用されるアクティベーション データ。</param>
        public async void Activate(ShareTargetActivatedEventArgs args)
        {
            this._shareOperation = args.ShareOperation;

            // ビュー モデルを使用して、共有されるコンテンツのメタデータを通信します
            var shareProperties = this._shareOperation.Data.Properties;
            var thumbnailImage = new BitmapImage();
            this.DefaultViewModel["Title"] = shareProperties.Title;
            this.DefaultViewModel["Description"] = shareProperties.Description;
            this.DefaultViewModel["Image"] = thumbnailImage;
            this.DefaultViewModel["Sharing"] = false;
            this.DefaultViewModel["ShowImage"] = false;
            this.DefaultViewModel["Comment"] = String.Empty;
            this.DefaultViewModel["SupportsComment"] = true;
            Window.Current.Content = this;
            Window.Current.Activate();

            // 共有されるコンテンツの縮小版イメージをバックグラウンドで更新します
            if (shareProperties.Thumbnail != null)
            {
                var stream = await shareProperties.Thumbnail.OpenReadAsync();
                thumbnailImage.SetSource(stream);
                this.DefaultViewModel["ShowImage"] = true;
            }
        }

        /// <summary>
        /// ユーザーが [共有] をクリックしたときに呼び出されます。
        /// </summary>
        /// <param name="sender">共有を開始するときに使用される Button インスタンス。</param>
        /// <param name="e">ボタンがどのようにクリックされたかを説明するイベント データ。</param>
        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            this.DefaultViewModel["Sharing"] = true;
            this._shareOperation.ReportStarted();

            // TODO: this._shareOperation.Data を使用して共有シナリオに適した
            //       作業を実行します。通常は、カスタム ユーザー インターフェイス要素を介して
            //       through custom user interface elements added to this page such as 
            //       this.DefaultViewModel["Comment"]

            this._shareOperation.ReportCompleted();
        }
    }
}
