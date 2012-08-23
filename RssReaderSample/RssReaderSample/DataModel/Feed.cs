using RssReaderSample.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReaderSample.DataModel
{
    public class Feed : BindableBase
    {
        private string id = Guid.NewGuid().ToString();
        /// <summary>
        /// 識別子を取得または設定します。
        /// </summary>
        public string Id
        {
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }
        private string title;
        /// <summary>
        /// フィードのタイトルを取得または設定します。
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }
        private Uri uri;
        /// <summary>
        /// フィードのUriを取得または設定します。
        /// </summary>
        public Uri Uri
        {
            get { return this.uri; }
            set { this.SetProperty(ref this.uri, value); }
        }
        private DateTimeOffset lastUpdatedTime;
        /// <summary>
        /// フィードの最終更新日時を取得または設定します。
        /// </summary>
        public DateTimeOffset LastUpdatedTime
        {
            get { return this.lastUpdatedTime; }
            set { this.SetProperty(ref this.lastUpdatedTime, value); }
        }
        private ObservableCollection<FeedItem> feedItems = new ObservableCollection<FeedItem>();
        /// <summary>
        /// フィードのアイテムを取得または設定します。
        /// </summary>
        public ObservableCollection<FeedItem> FeedItems
        {
            get { return this.feedItems; }
            set { this.SetProperty(ref this.feedItems, value); }
        }
    }
}
