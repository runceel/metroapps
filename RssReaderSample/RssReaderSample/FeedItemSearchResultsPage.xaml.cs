using RssReaderSample.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// <summary>
    /// このページには、グローバル検索がこのアプリケーションに指定されている場合に、検索結果が表示されます。
    /// </summary>
    public sealed partial class FeedItemSearchResultsPage : RssReaderSample.Common.LayoutAwarePage
    {

        public FeedItemSearchResultsPage()
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
            var queryText = navigationParameter as String;

            // TODO: アプリケーション固有の検索ロジックです。検索プロセスでは、
            //       結果カテゴリのリストを作成する必要があります。
            //
            //       filterList.Add(new Filter("<フィルター名>", <結果数>));
            //
            //       アクティブな状態で開始するには、3 番目の引数として true を渡すフィルターが最初
            //       のフィルター (通常は "All") のみであることが必要です。アクティブ フィルターの
            //       結果は以下の Filter_SelectionChanged で提供されます。
            ExecuteQuery(queryText);
        }

        public void ExecuteQuery(string queryText)
        {
            var model = RssReaderSampleModel.GetDefault();
            var filterList = new List<Filter>
            {
                new Filter("All", model.SearchByTitle(queryText).ToArray(), true),
            };
            filterList.AddRange(
                model.Feeds
                    .Select(f => new Filter(f.Title, f.SearchByTitle(queryText).ToArray()))
                    .Where(f => f.Count != 0));

            // ビュー モデルを介して結果を通信します
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
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

                // TODO: this.DefaultViewModel["Results"] をバインド可能な Image、Title、および Subtitle の
                //       バインドできる Image、Title、Subtitle、および Description プロパティを持つアイテムのコレクションに設定します
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
        /// 検索結果の表示に使用できるフィルターの 1 つを表すビュー モデルです。
        /// </summary>
        private sealed class Filter : RssReaderSample.Common.BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;

            public Filter(String name, ICollection<FeedItem> searchResults, bool active = false)
            {
                this.Name = name;
                this.SearchResults = searchResults;
                this.Active = active;
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
                get { return _count; }
                private set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Description"); }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            private ICollection<FeedItem> searchResults;

            public ICollection<FeedItem> SearchResults
            {
                get { return this.searchResults; }
                set 
                { 
                    this.SetProperty(ref this.searchResults, value);
                    this.Count = value.Count();
                }
            }


            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }
        }
    }
}
