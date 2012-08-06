using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReaderSample.Common;

namespace RssReaderSample.DataModel
{
    public class RssReaderSampleModel : BindableBase
    {
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
        public async void LoadAllFeeds()
        {
            // すべてのフィードの読み込みを行う
            await Task.WhenAll(this.Feeds.Select(f => f.Load()));
        }

    }
}
