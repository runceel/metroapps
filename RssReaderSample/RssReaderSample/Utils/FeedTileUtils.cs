using RssReaderSample.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace RssReaderSample.Utils
{
    static class FeedTileUtils
    {
        public static async Task<bool> CreateTileIfNotExist(Feed feed)
        {
            // 既にタイルが存在する場合は何もしない
            if (SecondaryTile.Exists(feed.Id))
            {
                return false;
            }

            // セカンダリタイルを作成
            var tile = new SecondaryTile(
                // タイルのId
                feed.Id,
                // タイルの短い名前
                feed.Title,
                // タイルの表示名
                feed.Title,
                // タイルからアプリケーションを起動したときに渡される引数
                feed.Id,
                // タイルの名前の表示方法を指定
                TileOptions.ShowNameOnLogo,
                // タイルのロゴを指定
                new Uri("ms-appx:///Assets/Logo.png"));
            // ユーザーにタイルの作成をリクエスト
            return await tile.RequestCreateAsync();
        }

        public static void UpdateFeedTile(Feed feed)
        {
            // タイルが存在しない場合は何もしない
            if (!SecondaryTile.Exists(feed.Id))
            {
                return;
            }

            // フィード内に記事が無い場合は何もしない
            if (!feed.FeedItems.Any())
            {
                return;
            }

            // 大きなテキスト1つと折り返し3行表示されるテキストを持つテンプレートを取得
            var template = TileUpdateManager.GetTemplateContent(
                TileTemplateType.TileSquareText02);
            // textタグに表示内容を設定
            var texts = template.GetElementsByTagName("text");
            texts[0].InnerText = feed.Title;
            texts[1].InnerText = feed.FeedItems.First().Title;

            // タイルを更新するオブジェクトを取得
            var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(feed.Id);
            // 有効期間5分の通知を設定
            updater.Update(new TileNotification(template)
            {
                ExpirationTime = DateTimeOffset.Now.AddMinutes(5)
            });
        }

        public static void UpdateFeedTiles(IEnumerable<Feed> feeds)
        {
            // すべてのフィードを更新
            foreach (var feed in feeds)
            {
                UpdateFeedTile(feed);
            }
        }
    }
}
