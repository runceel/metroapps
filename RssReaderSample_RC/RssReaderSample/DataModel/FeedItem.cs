using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReaderSample.Common;

namespace RssReaderSample.DataModel
{
    public class FeedItem : BindableBase
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
        /// アイテムのタイトルを取得または設定します。
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        private Uri uri;
        /// <summary>
        /// アイテムのUriを取得または設定します。
        /// </summary>
        public Uri Uri
        {
            get { return this.uri; }
            set { this.SetProperty(ref this.uri, value); }
        }

        private DateTimeOffset publishedDate;
        /// <summary>
        /// アイテムの公開日を取得または設定します。
        /// </summary>
        public DateTimeOffset PublishedDate
        {
            get { return this.publishedDate; }
            set { this.SetProperty(ref this.publishedDate, value); }
        }

        private string summary;
        /// <summary>
        /// アイテムの概要を取得または設定します。
        /// </summary>
        public string Summary
        {
            get { return this.summary; }
            set { this.SetProperty(ref this.summary, value); }
        }

    }
}
