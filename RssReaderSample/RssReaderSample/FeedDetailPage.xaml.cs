using RssReaderSample.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// グループ詳細ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234229 を参照してください

namespace RssReaderSample
{
    /// <summary>
    /// 単一のグループ内のアイテムのプレビューを含め、グループの概要を表示する
    /// ページです。
    /// </summary>
    public sealed partial class FeedDetailPage : RssReaderSample.Common.LayoutAwarePage
    {
        public FeedDetailPage()
        {
            this.InitializeComponent();
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
            var feed = RssReaderSampleModel.GetDefault().GetFeedById(id);
            this.DefaultViewModel["Group"] = feed;
            this.DefaultViewModel["Items"] = feed.FeedItems;
        }
    }
}
