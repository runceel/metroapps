using RssReaderSample.Common;
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
        public async Task LoadAllFeedsAsync()
        {
            // すべてのフィードの読み込みを行う
            await Task.WhenAll(this.Feeds.Select(f => f.LoadAsync()));
        }

        /// <summary>
        /// すべてのFeedからid指定でFeedItemを取得します。
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
        /// 引数で渡されたフィードを追加して読み込みを行います。
        /// </summary>
        /// <param name="feedUri">フィードのUri</param>
        /// <returns></returns>
        public async Task CreateFeedAsync(Uri feedUri)
        {
            var feed = new Feed { Title = "登録処理中",  Uri = feedUri };
            this.Feeds.Add(feed);
            await feed.LoadAsync();
        }

        /// <summary>
        /// フィードデータの復元を行います。
        /// </summary>
        /// <returns></returns>
        public async Task RestoreAsync()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SaveFileName);
                using (var s = await file.OpenStreamForReadAsync())
                {
                    await this.RestoreAsync(s);
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// ストリームからフィードデータの復元を行います。
        /// </summary>
        /// <param name="stream"></param>
        public async Task RestoreAsync(Stream stream)
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(ObservableCollection<Feed>));
                var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var data = serializer.ReadObject(ms) as ObservableCollection<Feed>;
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

        /// <summary>
        /// フィードの保存を行います。
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            var file = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(SaveFileName, CreationCollisionOption.ReplaceExisting);
            using (var s = await file.OpenStreamForWriteAsync())
            {
                await this.SaveAsync(s);
            }
        }

        /// <summary>
        /// ストリームにフィードの保存を行います。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task SaveAsync(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<Feed>));
            // 一旦メモリ上に保存する
            var ms = new MemoryStream();
            serializer.WriteObject(ms, this.Feeds);
            // メモリから渡されたストリームへ保存する
            ms.Seek(0, SeekOrigin.Begin);
            await ms.CopyToAsync(stream);
        }


    }
}
