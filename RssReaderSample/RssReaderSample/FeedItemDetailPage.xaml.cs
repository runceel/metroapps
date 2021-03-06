﻿using RssReaderSample.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
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

// 基本ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234237 を参照してください

namespace RssReaderSample
{
    /// <summary>
    /// 多くのアプリケーションに共通の特性を指定する基本ページ。
    /// </summary>
    public sealed partial class FeedItemDetailPage : RssReaderSample.Common.LayoutAwarePage
    {
        public FeedItemDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // 共有のイベントを購読する
            DataTransferManager.GetForCurrentView().DataRequested += FeedItemDetailPage_DataRequested;
        }

        private void FeedItemDetailPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            // 現在表示中のFeedItemを取得
            var feedItem = (FeedItem)this.DefaultViewModel["FeedItem"];
            // 転送するデータのリクエストを取得
            var request = args.Request;
            // 必要なデータを設定
            request.Data.Properties.Title = feedItem.Title;
            request.Data.Properties.Description = feedItem.Summary;
            request.Data.SetUri(feedItem.Uri);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // イベント購読解除
            DataTransferManager.GetForCurrentView().DataRequested -= FeedItemDetailPage_DataRequested;
            base.OnNavigatedFrom(e);
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
            this.DefaultViewModel["FeedItem"] = RssReaderSampleModel.GetDefault().GetFeedItemById(id);
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="pageState">シリアル化可能な状態で作成される空のディクショナリ。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
