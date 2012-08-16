using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RssReaderSample.Common;
using Windows.Storage;

namespace RssReaderSample.DataModel
{
    public class RssReaderSampleModel : BindableBase
    {
        private static readonly string SaveFileName = "feeds.xml";

        private static RssReaderSampleModel defaultInstance = new RssReaderSampleModel();
        /// <summary>
        /// デフォルトのインスタンスの取得を行います。
        /// </summary>
        /// <returns>RssReaderSampleModelのデフォルトのインスタンス</returns>
        public static RssReaderSampleModel GetDefault() { return defaultInstance; }

        // テスト用にダミーデータを2つ持たせておく
        private ObservableCollection<Feed> feeds = new ObservableCollection<Feed>
        {
            new Feed { Uri = new Uri("http://d.hatena.ne.jp/okazuki/rss") },
            new Feed { Uri = new Uri("http://d.hatena.ne.jp/ch3cooh393/rss") },
        };
        /// <summary>
        /// RssReaderに登録しているフィードを取得または設定します。
        /// </summary>
        public ObservableCollection<Feed> Feeds
        {
            get { return this.feeds; }
            set { this.SetProperty(ref this.feeds, value); }
        }

        /// <summary>
        /// フィードの読み込みを行います。
        /// </summary>
        public async Task LoadAllFeeds()
        {
            // すべてのフィードの読み込みを行う
            await Task.WhenAll(this.Feeds.Select(f => f.Load()));
        }

        /// <summary>
        /// すべてのFeedItemからId指定でFeedItemを取得します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>IDが一致したFeedItem。みつからない場合はnullを返します。</returns>
        public FeedItem GetFeedItemById(string id)
        {
            return this.Feeds.SelectMany(f => f.FeedItems)
                .FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// すべてのFeedからId指定でFeedを取得します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>IDが一致したFeed。みつからない場合はnullを返します。</returns>
        public Feed GetFeedById(string id)
        {
            return this.Feeds.FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// 引数で指定したuriをフィードに登録する。uriが正しいフォーマットではない場合falseを返します。
        /// </summary>
        /// <param name="uri">登録するフィードのuri</param>
        /// <returns>登録に成功したかどうかを返します。</returns>
        public async Task<bool> AddAndLoadFeed(string uri)
        {
            Uri feedUri = null;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out feedUri))
            {
                return false;
            }

            var feed = new Feed { Uri = feedUri };
            await feed.Load();
            this.Feeds.Add(feed);
            return true;
        }

        public async Task Load()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SaveFileName);
                using (var s = await file.OpenStreamForReadAsync())
                using (var ms = new MemoryStream())
                {
                    this.Load(s);
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Load(Stream stream)
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(ObservableCollection<Feed>));
                var data = serializer.ReadObject(stream) as ObservableCollection<Feed>;
                if (data == null)
                {
                    return;
                }

                this.Feeds.Clear();
                foreach (var feed in data)
                {
                    this.Feeds.Add(feed);
                }
            }
            catch (XmlException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task Save()
        {
            var file = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(SaveFileName, CreationCollisionOption.ReplaceExisting);
            using (var s = await file.OpenStreamForWriteAsync())
            {
                this.Save(s);
            }
        }

        public void Save(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<Feed>));
            serializer.WriteObject(stream, this.Feeds);
        }

    }
}
