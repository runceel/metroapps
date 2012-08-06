using RssReaderSample.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// アイテム ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234233 を参照してください

namespace RssReaderSample
{
    /// <summary>
    /// アイテムのコレクションのプレビューを表示するページです。このページは、分割アプリケーションで使用できる
    /// グループを表示し、その 1 つを選択するために使用されます。
    /// </summary>
    public sealed partial class ItemsPage : RssReaderSample.Common.LayoutAwarePage
    {
        public ItemsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Items"] = sampleDataGroups;
        }

        /// <summary>
        /// アイテムがクリックされたときに呼び出されます。
        /// </summary>
        /// <param name="sender">クリックされたアイテムを表示する GridView (アプリケーションがスナップ
        /// されている場合は ListView) です。</param>
        /// <param name="e">クリックされたアイテムを説明するイベント データ。</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 適切な移動先のページに移動し、新しいページを構成します。
            // このとき、必要な情報をナビゲーション パラメーターとして渡します
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(SplitPage), groupId);
        }
    }
}
