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

// ユーザー コントロールのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace RssReaderSample
{
    public sealed partial class AddFeedFlyout : UserControl
    {
        public event Action AddFeedFinished;
        private void RaiseAddFeedFinished()
        {
            var h = this.AddFeedFinished;
            if (h != null)
            {
                h();
            }
        }

        public AddFeedFlyout()
        {
            this.InitializeComponent();
        }

        private async void AddFeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (await RssReaderSampleModel.GetDefault().AddAndLoadFeed(this.textBoxUri.Text))
            {
                var popup = this.Parent as Popup;
                popup.IsOpen = false;
                this.RaiseAddFeedFinished();
                return;
            }

            this.textBlockMessage.Text = "URLを入力してください";
        }
    }
}
