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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace HelloWorldApp
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : HelloWorldApp.Common.LayoutAwarePage
    {
        public MainPage()
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
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
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
