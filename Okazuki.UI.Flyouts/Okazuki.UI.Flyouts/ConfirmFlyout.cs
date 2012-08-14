namespace Okazuki.UI.Flyouts
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Okazuki.UI.Flyouts.Internal;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;

    /// <summary>
    /// BottomAppBarのボタンを押したときにユーザーに確認を求めるFlyoutを表示します
    /// </summary>
    public class ConfirmFlyout : NotificationObject
    {
        private Popup currentPopup;

        private ObservableCollection<IUICommand> commands = new ObservableCollection<IUICommand>();

        /// <summary>
        /// Flyoutに表示するコマンド
        /// </summary>
        public ObservableCollection<IUICommand> Commands
        {
            get { return this.commands; }
            set { this.SetProperty(ref this.commands, value); }
        }

        private Color background = Colors.Purple;

        /// <summary>
        /// Flyoutの背景色
        /// </summary>
        public Color Background
        {
            get { return this.background; }
            set { this.SetProperty(ref this.background, value); }
        }

        /// <summary>
        /// targetで指定したボタンにmessageで指定したメッセージを表示します。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        /// <returns>押されたボタンに対応するコマンド。何も押されなかった場合はnullが返ります。</returns>
        public async Task<IUICommand> ShowAsync(FrameworkElement target, string message)
        {
            var content = new MessageWindow { Message = message };
            return await ShowAsync(target, content);
        }

        /// <summary>
        /// targetで指定したボタンにcontentで指定したUserControlを表示します。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="content"></param>
        /// <returns>押されたボタンに対応するコマンド。何も押されなかった場合はnullが返ります。</returns>
        public async Task<IUICommand> ShowAsync(FrameworkElement target, FrameworkElement content)
        {
            return await this.ShowAsyncInternal(target, content);
        }

        private async Task<IUICommand> ShowAsyncInternal(FrameworkElement target, FrameworkElement content)
        {
            if (this.currentPopup != null)
            {
                throw new InvalidOperationException("already called");
            }

            // 表示をホストするUserControlを作成
            var host = new FlyoutHost
            {
                DataContext = this,
                Pane = content,
            };

            // 領域外をクリックされると消えるPopupを作成
            this.currentPopup = new Popup
            {
                Child = host,
                IsLightDismissEnabled = true,
            };

            // ポップアップを画面外に表示します。
            this.ShowCurrentPopup();
            Canvas.SetTop(this.currentPopup, -1000);
            Canvas.SetLeft(this.currentPopup, -1000);

            await host.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // 画面外に表示したのでActualWidthとActualHeightが取得可能になるため
                // 正しい表示位置をサイズから算出します。
                var gt = target.TransformToVisual(null);
                var point = gt.TransformPoint(new Point());
                point.X += target.ActualWidth - host.ActualWidth;
                point.Y -= 20 + host.ActualHeight;

                // 画面外に出るようなら微調整
                if (point.X < 20)
                {
                    point.X = 20;
                }

                // 正しい位置を設定
                Canvas.SetTop(this.currentPopup, point.Y);
                Canvas.SetLeft(this.currentPopup, point.X);
            });

            // ボタンが押されるか閉じられると値を返すTaskを作る
            var taskSource = new TaskCompletionSource<IUICommand>();
            host.Completed += result =>
            {
                // 押されたボタンを返す
                this.Cleanup();
                taskSource.SetResult(result);
            };
            this.currentPopup.Closed += (_, __) =>
            {
                if (!taskSource.Task.IsCompleted)
                {
                    // ボタンが押されなかったのでnullを返す
                    taskSource.SetResult(null);
                }
            };

            return await taskSource.Task;
        }

        private void ShowCurrentPopup()
        {
            if (this.currentPopup == null)
            {
                return;
            }

            this.currentPopup.Opened += (_, __) =>
            {
                Window.Current.Activated += Current_Activated;
                Window.Current.SizeChanged += Current_SizeChanged;
            };
            this.currentPopup.Closed += (_, __) =>
            {
                Window.Current.Activated -= Current_Activated;
                Window.Current.SizeChanged -= Current_SizeChanged;
            };
            this.currentPopup.IsOpen = true;
        }

        private void Cleanup()
        {
            if (this.currentPopup == null)
            {
                return;
            }

            this.currentPopup.IsOpen = false;
            this.currentPopup = null;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.Cleanup();
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                this.Cleanup();
            }
        }
    }
}
