using HelloWorldApp.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
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
using HelloWorldApp.DataModel;
using Windows.Storage;
using System.Diagnostics;
using System.Xml;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace HelloWorldApp
{
    /// <summary>
    /// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
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
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            try
            {
                // アプリケーションデータの保存しているファイルを取得
                var applicationData = await ApplicationData.Current.LocalFolder.GetFileAsync(
                    "applicationData.xml");
                // 読み取り専用でファイルを開いてHelloWorldModelに読み込ませる
                using (var s = await applicationData.OpenStreamForReadAsync())
                {
                    HelloWorldModel.LoadFromStream(s);
                }
            }
            catch (FileNotFoundException)
            {
                // ファイルが無い(初回起動)なので何もしない
                Debug.WriteLine("ファイル無し");
            }
            catch (XmlException)
            {
                // ファイルが破損しているので何もしない
                Debug.WriteLine("ファイルフォーマットが不正");
            }

            // Create a Frame to act as the navigation context and associate it with
            // a SuspensionManager key
            var rootFrame = new Frame();
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                await SuspensionManager.RestoreAsync();
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
                // を構成します
                if (!rootFrame.Navigate(typeof(MainPage)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // フレームを現在のウィンドウに配置し、アクティブであることを確認します
            Window.Current.Content = rootFrame;
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

            // アプリケーションデータの保存用ファイルを作成する。
            // 既存ファイルは置き換える。
            var applicationData = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "applicationData.xml",
                CreationCollisionOption.ReplaceExisting);

            // 書き込み専用でファイルを開いてHelloWorldModelを保存する
            using (var s = await applicationData.OpenStreamForWriteAsync())
            {
                HelloWorldModel.SaveToStream(s);
            }

            deferral.Complete();
        }
    }
}
