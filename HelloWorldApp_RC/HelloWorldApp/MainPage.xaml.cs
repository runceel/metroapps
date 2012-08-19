using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HelloWorldApp.DataModel;
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
            var model = HelloWorldModel.GetDefault();
            // 中断データがある場合は読み込む
            if (pageState != null)
            {
                // Timeのデータがあれば取得してHelloWorldModelに設定する
                object time = null;
                if (pageState.TryGetValue("Time", out time))
                {
                    model.Time = (string)time;
                }

                // Messageのデータがあれば取得してHelloWorldModelに設定する
                object message = null;
                if (pageState.TryGetValue("Message", out message))
                {
                    model.Message = (string)message;
                }
            }
            // DefaultViewModelのHelloWorldModelをキーにしてHelloWorldModelのインスタンスを設定する
            this.DefaultViewModel["HelloWorldModel"] = model;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            // HelloWorldModelのTimeとMessageをpageStateに保存する
            var model = HelloWorldModel.GetDefault();
            pageState["Time"] = model.Time;
            pageState["Message"] = model.Message;
        }

        private void buttonGreet_Click(object sender, RoutedEventArgs e)
        {
            HelloWorldModel.GetDefault().Greet();
        }
    }
}
