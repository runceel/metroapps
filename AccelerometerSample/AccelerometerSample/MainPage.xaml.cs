using System;
using System.Collections.Generic;
using Windows.Devices.Sensors;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace AccelerometerSample
{
    public sealed partial class MainPage : AccelerometerSample.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// ナビゲーション中に渡されたコンテンツを含むページを移入します。
        /// 前のセッションからページを再作成するときは、保存された状態でも提供されています。
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
        /// このページの場合、アプリケーションが中断されたり、ページナビゲーションのキャッシュから
        /// 破棄され関連付けられた状態を維持します。値は、<see cref="SuspensionManager.SessionState"/>の
        /// シリアル化要件に準拠する必要があります。
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }


        Marimo marimo = null;
        Accelerometer accelerometer = null;
        DispatcherTimer timer = null;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // マリモ情報のインスタンスを生成して画面にデータバインディングする
            marimo = new Marimo();
            screenGrid.DataContext = marimo;

            // デフォルトの加速度センサーを取得する
            accelerometer = Accelerometer.GetDefault();

            // 加速度センサーデバイスがない場合はnullが返ってくる
            if (accelerometer == null)
            {
                marimo.IsAccelerometer = false;

                // センサーがないことをユーザーへ通知
                var dlg = new MessageDialog("加速度センサーが接続されていないか、デバイスが認識されていません。");
                await dlg.ShowAsync();
            }
            else
            {
                marimo.IsAccelerometer = true;

                // 加速度センサーのイベントを受け取る
                accelerometer.ReadingChanged += accelerometer_ReadingChanged;
            }

            // マリモを動かすためのタイマー(約30fpsで動くことを想定)
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // 画面を抜けた後なので加速度センサーのイベントを受け取らない
            if (accelerometer != null)
            {
                accelerometer.ReadingChanged -= accelerometer_ReadingChanged;
            }
            accelerometer = null;

            // 画面を抜けた後なのでタイマーを止める
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        AccelerometerReading accValue = null;

        private void accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            accValue = args.Reading;
        }

        void timer_Tick(object sender, object e)
        {
            var top = marimo.Top;
            var left = marimo.Left;

            if (accValue == null || !marimo.IsAccelerometer)
            {
                // 加速度の値が未取得の状態またはセンサーが存在しない場合
                // マリモを下に移動させる
                top++;
            }
            else
            {
                // 加速度センサーの値を更新する
                labelX.Text = string.Format("x: {0:0.000}", accValue.AccelerationX);
                labelY.Text = string.Format("y: {0:0.000}", accValue.AccelerationY);
                labelZ.Text = string.Format("z: {0:0.000}", accValue.AccelerationZ);

                // 傾きが大きい方向の向きにマリモを移動させる
                if (Math.Abs(accValue.AccelerationX) < Math.Abs(accValue.AccelerationY))
                {
                    // マリモを上下に移動させる
                    if (accValue.AccelerationY < 0)
                    {
                        top++;
                    }
                    else
                    {
                        top--;
                    }
                }
                else
                {
                    // マリモを左右に移動させる
                    if (accValue.AccelerationX < 0)
                    {
                        left--;
                    }
                    else
                    {
                        left++;
                    }
                }
            }

            // マリモが画面外へ出ていかないように値を補正します
            top = Math.Min(top, screenGrid.ActualHeight - marimo.Size);
            top = Math.Max(top, 0);
            left = Math.Min(left, screenGrid.ActualWidth - marimo.Size);
            left = Math.Max(left, 0);

            marimo.Top = top;
            marimo.Left = left;
        }

        private void marimoEllipse_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var top = marimo.Top + e.Position.Y;
            var left = marimo.Left + e.Position.X;

            // マリモが画面外へ出ていかないように値を補正します
            top = Math.Min(top, screenGrid.ActualHeight - marimo.Size);
            top = Math.Max(top, 0);
            left = Math.Min(left, screenGrid.ActualWidth - marimo.Size);
            left = Math.Max(left, 0);

            marimo.Top = top;
            marimo.Left = left;
        }
    }
}
