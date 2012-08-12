using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using RssReaderSample.Common;
using Windows.Web.Syndication;
using System.Diagnostics;
using Windows.Data.Xml.Dom;
using Windows.Storage;

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

        public async Task Load()
        {
            try
            {
#if OFFLINE
                var feed = new SyndicationFeed();
                var file = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(this.Uri.ToString().EndsWith("okazuki/rss") ? 
                        "ms-appx:///Assets/okazuki_rss.xml" :
                        "ms-appx:///Assets/ch3cooh393_rss.xml"));
                feed.LoadFromXml(await XmlDocument.LoadFromFileAsync(file));
#else
                // フィードを読み込み、FeedクラスとFeedItemを組み立てる。
                var client = new SyndicationClient();
                var feed = await client.RetrieveFeedAsync(this.Uri);
#endif
                this.Title = feed.Title.Text;
                this.LastUpdatedTime = feed.LastUpdatedTime;

                var items = feed.Items.Select(i =>
                    new FeedItem
                    {
                        Title = i.Title.Text,
                        PublishedDate = i.PublishedDate,
                        Summary = i.Summary.Text,
                        Uri = i.Links.First().Uri
                    });
                this.FeedItems.Clear();
                foreach (var item in items)
                {
                    this.FeedItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                // エラーが発生した場合はダミーのデータを設定する
                Debug.WriteLine(ex);
                this.Title = "読み込みに失敗しました";
            }
        }

    }
}
