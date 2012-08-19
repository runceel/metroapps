using HelloWorldApp.DataModel;
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

// 基本ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234237 を参照してください

namespace HelloWorldApp
{
    /// <summary>
    /// 多くのアプリケーションに共通の特性を指定する基本ページ。
    /// </summary>
    public sealed partial class MainPage : HelloWorldApp.Common.LayoutAwarePage
    {
        public MainPage()
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
            // DefaultViewModelのHelloWorldModelをキーにしてHelloWorldModelのインスタンスを設定する
            this.DefaultViewModel["HelloWorldModel"] = HelloWorldModel.GetDefault();
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="pageState">シリアル化可能な状態で作成される空のディクショナリ。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void buttonGreet_Click(object sender, RoutedEventArgs e)
        {
            // 出力メッセージのフォーマットを格納するための変数
            string format = null;

            // 選択項目に応じて出力メッセージのフォーマットを設定する
            switch ((string)comboBoxTime.SelectedValue)
            {
                case "朝":
                    format = "おはようございます。{0}さん。";
                    break;
                case "昼":
                    format = "こんにちは。{0}さん。";
                    break;
                case "晩":
                    format = "こんばんは。{0}さん。";
                    break;
                default:
                    // 朝と昼と晩しかありえない
                    throw new InvalidOperationException("不正な値");
            }

            // 出力メッセージをテキストブロックに設定する
            textBlockMessage.Text = string.Format(format, textBoxName.Text);
        }
    }
}
