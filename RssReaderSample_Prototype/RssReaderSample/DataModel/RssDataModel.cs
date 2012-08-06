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
using Windows.ApplicationModel;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Web.Syndication;

namespace RssReaderSample.DataModel
{
    public class RssReaderSampleModel
    {
        private static RssReaderSampleModel defaultInstance;

        public static RssReaderSampleModel GetDefault()
        {
            if (defaultInstance == null)
            {
                defaultInstance = new RssReaderSampleModel();
            }

            return defaultInstance;
        }

        private ObservableCollection<Blog> blogs = new ObservableCollection<Blog>();

        public ObservableCollection<Blog> Blogs
        {
            get { return this.blogs; }
        }

        public async Task LoadFeeds()
        {
            await Task.WhenAll(this.Blogs.Select(b => b.LoadFeed()));
        }

        public async Task SaveFeeds()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "localfeeds.xml", CreationCollisionOption.ReplaceExisting);
            
        }

        public static async Task LoadBlogs()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "blogs.xml", CreationCollisionOption.OpenIfExists);
                using (var s = await file.OpenStreamForWriteAsync())
                {
                    GetDefault().LoadBlogs(s);
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            catch (XmlException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void LoadBlogs(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<Blog>));
            var blogs = serializer.ReadObject(stream) as ObservableCollection<Blog>;
            this.Blogs.Clear();
            foreach (var blog in blogs)
            {
                this.Blogs.Add(blog);
            }
        }

        public static async Task SaveBlogs()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "blogs.xml", CreationCollisionOption.ReplaceExisting);
            using (var s = await file.OpenStreamForWriteAsync())
            {
                await GetDefault().SaveBlogs(s);
            }
        }

        public async Task SaveBlogs(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<Blog>));

            var ms = new MemoryStream();
            serializer.WriteObject(ms, this.Blogs);
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(stream);
        }
    }

    public class FeedItem : BindableBase
    {
        public static FeedItem ErrorFeedItem(string feedUri)
        {
            return new FeedItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = feedUri,
                Image = new Uri("ms-appdata:///Assets/LightGray.png", UriKind.Absolute),
                Link = new Uri("about:blank")
            };
        }

        private string id;
        public string Id
        {
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }

        private string title;
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        private string subtitle;
        public string Subtitle
        {
            get { return this.subtitle; }
            set { this.SetProperty(ref this.subtitle, value); }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set { this.SetProperty(ref this.description, value); }
        }

        public string content;
        public string Content
        {
            get { return this.content; }
            set { this.SetProperty(ref this.content, value); }
        }

        private Uri image;
        public Uri Image
        {
            get { return this.image; }
            set { this.SetProperty(ref this.image, value); }
        }

        private Uri link;
        public Uri Link
        {
            get { return this.link; }
            set { this.SetProperty(ref this.link, value); }
        }
    }

    public class Blog : BindableBase
    {
        private string id = Guid.NewGuid().ToString();

        public string Id
        {
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }

        private string title;
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        private string subtitle;
        public string Subtitle
        {
            get { return this.subtitle; }
            set { this.SetProperty(ref this.subtitle, value); }
        }

        private Uri image;
        public Uri Image
        {
            get { return this.image; }
            set { this.SetProperty(ref this.image, value); }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set { this.SetProperty(ref this.description, value); }
        }

        private ObservableCollection<FeedItem> feeds = new ObservableCollection<FeedItem>();
        public ObservableCollection<FeedItem> Feeds
        {
            get { return this.feeds; }
            set { this.SetProperty(ref this.feeds, value); }
        }

        private string feedUri;
        public string FeedUri
        {
            get { return this.feedUri; }
            set { this.SetProperty(ref this.feedUri, value); }
        }

        public async Task LoadFeed()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile == null || connectionProfile.GetNetworkConnectivityLevel() != 
                NetworkConnectivityLevel.InternetAccess)
            {
                if (!this.Feeds.Any() && string.IsNullOrWhiteSpace(this.Title))
                {
                    this.CreateErrorState();
                }

                return;
            }

            try
            {
#if OFFLINE
                await Task.Delay(1);
                var r = new Random();
                this.Title = "XXXXXXのBlog";
                this.Subtitle = "晴れ時々曇り";
                this.Image = new Uri("ms-appdata:///Assets/DarkGray.png");
                this.Description = "日々思ったことをつらつらと書いていきます。心は空模様";

                var feeds = Enumerable.Range(1, r.Next(10) + 5).Select(i =>
                    new FeedItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "sample title" + i,
                        Subtitle = "subtitle " + i,
                        Description = i + "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                        Image = new Uri("Assets/DarkGray.png", UriKind.Relative),
                        Content = "春はあけぼのやふやふ白くなり行く山際少しあかりて…………………………………………………………………………………………………………………………………………………………………………………。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。",
                        Link = new Uri("about:blank")
                    });
                this.Feeds.Clear();
                foreach (var feed in feeds)
                {
                    this.Feeds.Add(feed);
                }
#else
                var reader = new SyndicationClient();
                var url = new Uri(this.FeedUri, UriKind.Absolute);
                var syndicationFeeds = await reader.RetrieveFeedAsync(url);
                this.Title = syndicationFeeds.Title.Text;
                this.Subtitle = syndicationFeeds.Subtitle.Text;
                this.Image = syndicationFeeds.ImageUri;

                var r = new Random();
                var items = await Task.Run(() => syndicationFeeds.Items.Select(item =>
                    new FeedItem
                    {
                        Id = item.Id,
                        Title = item.Title.Text,
                        Subtitle = item.PublishedDate.ToString(),
                        Image = new Uri(
                            r.Next() % 2 == 0 ? "ms-appx:///Assets/LightGray.png" : "ms-appx:///Assets/DarkGray.png", UriKind.Absolute),
                        Description = item.Summary.Text,
                        Link = item.Links.First().Uri,
                    }));
                this.Feeds.Clear();
                foreach (var item in items)
                {
                    this.Feeds.Add(item);
                }
#endif
            }
            catch (Exception)
            {
                CreateErrorState();
            }
        }

        private void CreateErrorState()
        {
            this.Feeds.Clear();
            this.Title = "読み込みに失敗しました";
            this.Subtitle = this.FeedUri.ToString();
            this.Feeds.Add(FeedItem.ErrorFeedItem(this.FeedUri));
        }
    }
}
