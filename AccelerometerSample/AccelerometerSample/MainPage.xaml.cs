using System;
using System.Diagnostics;
using Windows.Devices.Sensors;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace AccelerometerSample
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        Marimo marimo = null;
        Accelerometer accelerometer = null;
        DispatcherTimer timer = null;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // まりも情報のインスタンスを生成して画面にデータバインディングする
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

            // まりもを動かすためのタイマー(約30fpsで動くことを想定)
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
            if (accValue == null || !marimo.IsAccelerometer)
            {
                // 加速度の値が未取得の状態またはセンサーが存在しない場合
                // まりもを下に移動させる
                marimo.Top++;
            }
            else
            {
                // 加速度センサーの値を更新する
                labelX.Text = string.Format("x: {0:0.000}", accValue.AccelerationX);
                labelY.Text = string.Format("y: {0:0.000}", accValue.AccelerationY);
                labelZ.Text = string.Format("z: {0:0.000}", accValue.AccelerationZ);

                // 傾きが大きい方向の向きにまりもを移動させる
                if (Math.Abs(accValue.AccelerationX) < Math.Abs(accValue.AccelerationY))
                {
                    // まりもを上下に移動させる
                    if (accValue.AccelerationY < 0)
                    {
                        marimo.Top++;
                    }
                    else
                    {
                        marimo.Top--;
                    }
                }
                else
                {
                    // まりもを左右に移動させる
                    if (accValue.AccelerationX < 0)
                    {
                        marimo.Left--;
                    }
                    else
                    {
                        marimo.Left++;
                    }
                }
            }

            // まりもが画面外へ出ていかないように値を補正します
            marimo.Top = Math.Min(marimo.Top, screenGrid.ActualHeight - marimo.Size);
            marimo.Top = Math.Max(marimo.Top, 0);
            marimo.Left = Math.Min(marimo.Left, screenGrid.ActualWidth - marimo.Size);
            marimo.Left = Math.Max(marimo.Left, 0);
        }

        private void marimoEllipse_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            marimo.Top += e.Position.Y;
            marimo.Left += e.Position.X;
        }
    }
}
