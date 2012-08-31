using RssReaderSample.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 検索コントラクトのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234240 を参照してください

namespace RssReaderSample
{
    // TODO: マニフェストを編集して検索を有効にします
    //
    // パッケージ マニフェストは自動的に更新されません。パッケージ マニフェスト ファイルを
    // 開き、検索のアクティベーションのサポートが有効になっていることを確認してください。
    /// <summary>
    /// このページには、グローバル検索がこのアプリケーションに指定されている場合に、検索結果が表示されます。
    /// </summary>
    public sealed partial class FeedItemSearchResultsPage : RssReaderSample.Common.LayoutAwarePage
    {

        public FeedItemSearchResultsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // 検索クエリの変更イベントを購読
            SearchPane.GetForCurrentView().QueryChanged += FeedItemSearchResultsPage_QueryChanged;
        }

        private DateTimeOffset queryChangedCalled;
        private async void FeedItemSearchResultsPage_QueryChanged(SearchPane sender, SearchPaneQueryChangedEventArgs args)
        {
            queryChangedCalled = DateTimeOffset.Now;
            await Task.Delay(200);
            if (DateTimeOffset.Now - queryChangedCalled < TimeSpan.FromMilliseconds(100))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(args.QueryText))
            {
                return;
            }

            this.ExecuteQuery(args.QueryText);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // 検索クエリの変更イベントの購読を解除
            SearchPane.GetForCurrentView().QueryChanged -= FeedItemSearchResultsPage_QueryChanged;
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
            var queryText = navigationParameter as String;
            this.ExecuteQuery(queryText);
        }

        /// <summary>
        /// スナップ ビューステートの ComboBox を使用してフィルターが選択されたときに呼び出されます。
        /// </summary>
        /// <param name="sender">ComboBox インスタンス。</param>
        /// <param name="e">選択されたフィルターがどのように変更されたかを説明するイベント データ。</param>
        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 選択されたフィルターを確認します
            var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
            if (selectedFilter != null)
            {
                // 対応する Filter オブジェクト内に結果をミラー化し、
                // いない場合に RadioButton 表現を使用して変更を反映できるようにします
                selectedFilter.Active = true;

                // 検索結果をDefaultViewModelへ設定する
                this.DefaultViewModel["Results"] = selectedFilter.SearchResults;

                // 結果が見つかったことを確認します
                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
                }
            }

            // 検索結果がない場合は、情報テキストを表示します。
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        /// <summary>
        /// スナップ化されていない場合に RadioButton を使用してフィルターが選択されたときに呼び出されます。
        /// </summary>
        /// <param name="sender">選択された RadioButton インスタンス。</param>
        /// <param name="e">RadioButton がどのように選択されたかを説明するイベント データ。</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // 対応する ComboBox で使用される CollectionViewSource に変更をミラー化して
            // スナップ化されたときに変更が反映されるようにします
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        /// <summary>
        /// 検索を行います。
        /// </summary>
        /// <param name="queryText">検索に使用するクエリ</param>
        public void ExecuteQuery(string queryText)
        {
            // モデルを取得
            var model = RssReaderSampleModel.GetDefault();

            var filterList = new List<Filter>();
            // すべての検索結果
            filterList.Add(new Filter("All", model.SearchByTitle(queryText).ToArray(), true));
            // フィードのタイトルごとの検索結果
            filterList.AddRange(
                model.Feeds.Select(f =>
                    new Filter(f.Title, f.SearchByTitle(queryText).ToArray())));

            // ビュー モデルを介して結果を通信します
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
        }


        /// <summary>
        /// 検索結果の表示に使用できるフィルターの 1 つを表すビュー モデルです。
        /// </summary>
        private sealed class Filter : RssReaderSample.Common.BindableBase
        {
            private String _name;
            private bool _active;
            private FeedItem[] _searchResults;

            public Filter(String name, FeedItem[] searchResults, bool active = false)
            {
                this.Name = name;
                this.SearchResults = searchResults;
                this.Active = active;
            }

            public FeedItem[] SearchResults
            {
                get { return this._searchResults; }
                set { this.SetProperty(ref this._searchResults, value); this.OnPropertyChanged("Description"); }
            }

            public override String ToString()
            {
                return Description;
            }

            public String Name
            {
                get { return _name; }
                set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Description"); }
            }

            public int Count
            {
                get { return this.SearchResults.Length; }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", _name, this.Count); }
            }
        }
    }
}
