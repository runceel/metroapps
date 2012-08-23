using HelloWorldApp.Common;
using HelloWorldApp.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// グリッド アプリケーション テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234226 を参照してください

namespace HelloWorldApp
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
            
            if (rootFrame == null)
            {
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
                catch (FileNotFoundException ex)
                {
                    // ファイルが無い(初回起動)なので何もしない
                    Debug.WriteLine("ファイル無し" + ex);
                }
                catch (XmlException ex)
                {
                    // ファイルが破損しているので何もしない
                    Debug.WriteLine("ファイルフォーマットが不正" + ex);
                }

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
