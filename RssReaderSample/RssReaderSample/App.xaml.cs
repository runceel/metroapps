using RssReaderSample.Common;
using RssReaderSample.DataModel;
using RssReaderSample.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// グリッド アプリケーション テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234226 を参照してください

namespace RssReaderSample
{
    /// <summary>
    /// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
        /// 最初の行であり、main() または WinMain() と論理的に等価です。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ ポイントは、
        /// アプリケーションが特定のファイルを開くために呼び出されたときに
        /// 検索結果やその他の情報を表示するために使用されます。
        /// </summary>
        /// <param name="args">起動要求とプロセスの詳細を表示します。</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
            // ウィンドウがアクティブであることだけを確認してください
            if (args.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                await RssReaderSampleModel.GetDefault().RestoreAsync();
            }

            if (rootFrame == null)
            {
                // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
                rootFrame = new Frame();
                //フレームを SuspensionManager キーに関連付けます                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // 必要な場合のみ、保存されたセッション状態を復元します
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //状態の復元に何か問題があります。
                        //状態がないものとして続行します
                    }
                }

                // フレームを現在のウィンドウに配置します
                Window.Current.Content = rootFrame;
            }

            // 引数がある場合はタイルからの実行と判断
            if (!string.IsNullOrEmpty(args.Arguments))
            {
                // タイルに紐づくフィードの取得
                var feed = RssReaderSampleModel.GetDefault().GetFeedById(args.Arguments);
                if (feed != null)
                {
                    // フィードが取得できた場合は読み込んでフィードの詳細画面へ遷移する
                    await feed.LoadAsync();
                    FeedTileUtils.UpdateFeedTile(feed);
                    rootFrame.Navigate(typeof(FeedDetailPage), args.Arguments);
                }
            }

            if (rootFrame.Content == null)
            {
                // ナビゲーション スタックが復元されていない場合、最初のページに移動します。
                // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
                // を構成します
                if (!rootFrame.Navigate(typeof(MainPage)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // 現在のウィンドウがアクティブであることを確認します
            Window.Current.Activate();
        }

        /// <summary>
        /// アプリケーションの実行が中断されたときに呼び出されます。アプリケーションの状態は、
        /// アプリケーションが終了されるのか、メモリの内容がそのままで再開されるのか
        /// わからない状態で保存されます。
        /// </summary>
        /// <param name="sender">中断要求の送信元。</param>
        /// <param name="e">中断要求の詳細。</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            await RssReaderSampleModel.GetDefault().SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// 検索結果を表示するためにアプリケーションがアクティブになるときに呼び出されます。
        /// </summary>
        /// <param name="args">アクティブ化要求に関する詳細を表示します。</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // TODO: アプリケーションが既に実行されている場合は、検索時間を短縮するために OnWindowCreated で
            // Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted イベントを登録します
            if (args.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                await RssReaderSampleModel.GetDefault().RestoreAsync();
            }

            // ウィンドウで Frame ナビゲーションがまだ使用されていない場合は、独自の Frame を挿入します
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // アプリケーションにトップレベルのフレームが含まれていない場合は、これが 
            // 最初に起動される可能性があります。通常は、App.xaml.cs のこのメソッドおよび 
            // OnLaunched で共通のメソッドを呼び出すことができます。
            if (frame == null)
            {
                // ナビゲーション コンテキストとして動作するフレームを作成し、
                // SuspensionManager キーに関連付けます
                frame = new Frame();
                RssReaderSample.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // 必要な場合のみ、保存されたセッション状態を復元します
                    try
                    {
                        await RssReaderSample.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (RssReaderSample.Common.SuspensionManagerException)
                    {
                        // 状態の復元に何か問題があります。
                        // 状態がないものとして続行します
                    }
                }
            }

            // 検索クエリが空の場合
            if (string.IsNullOrWhiteSpace(args.QueryText))
            {
                // MainPageへの遷移が必要な場合は画面遷移を行う
                if (CheckNavigationNeed(frame.Content))
                {
                    frame.Navigate(typeof(MainPage));
                }

                Window.Current.Content = frame;
                Window.Current.Activate();
                return;
            }

            // 現在検索結果表示ページが表示されているか確認する
            var page = frame.Content as FeedItemSearchResultsPage;
            if (page == null)
            {
                // 検索結果表示ページじゃない場合は検索結果表示ページに遷移
                frame.Navigate(typeof(FeedItemSearchResultsPage), args.QueryText);
                return;
            }
            else
            {
                // 検索結果表示ページなら検索処理を実行
                page.ExecuteQuery(args.QueryText);
            }

            Window.Current.Content = frame;

            // 現在のウィンドウがアクティブであることを確認します
            Window.Current.Activate();
        }

        /// <summary>
        /// MainPageへ遷移の必要があるかどうか判断します。
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static bool CheckNavigationNeed(object content)
        {
            // 初回起動時には遷移が必要
            if (content == null)
            {
                return true;
            }

            // MainPageかFeedItemSearchResultsPageの場合は遷移は不要
            if (content.GetType() == typeof(MainPage) || content.GetType() == typeof(FeedItemSearchResultsPage))
            {
                return false;
            }

            // それ以外のときは遷移が必要
            return true;
        }
    }
}
