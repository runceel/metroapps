using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AccelerometerSample
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// マリモの情報
        /// </summary>
        Marimo marimo = null;

        /// <summary>
        /// マリモを自然落下させるためのアニメーションタイマー
        /// </summary>
        Windows.UI.Xaml.DispatcherTimer timer = null;

        /// <summary>
        /// 加速度センサー
        /// </summary>
        Windows.Devices.Sensors.Accelerometer accelerometer = null;

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、通常、ページを構成するために使用します。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // マリモの情報がない場合はMarimoクラスのインスタンス生成をおこなう
            if (marimo == null)
            {
                marimo = new Marimo();
            }

            // marimoEllipseとマリモの情報をデータバインディングする
            marimoEllipse.DataContext = marimo;

            // マリモを動かすためのタイマー(約30fpsで動くことを想定)
            timer = new Windows.UI.Xaml.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromMilliseconds(33);
            timer.Tick += timer_Tick;
            timer.Start();

            // デフォルトの加速度センサーを取得する
            accelerometer = Windows.Devices.Sensors.Accelerometer.GetDefault();
            // デフォルトの加速度センサーが取得できなければnullを返します
            if (accelerometer != null)
            {
                // ReadingChangedイベントハンドラを設定する
                accelerometer.ReadingChanged += accelerometer_ReadingChanged;

                // 加速度センサーが搭載されているのでtrue
                marimo.IsAccelerometer = true;
            }
            else
            {
                // 加速度センサーが搭載されていないのでfalse
                marimo.IsAccelerometer = false;
            }
        }

        // 加速度センサーの結果値
        Windows.Devices.Sensors.AccelerometerReading accelerometerValue = null;

        // 加速度センサーの値が変更される度に通知される
        void accelerometer_ReadingChanged(Windows.Devices.Sensors.Accelerometer sender, 
            Windows.Devices.Sensors.AccelerometerReadingChangedEventArgs args)
        {
            // 加速度センサーの読み取り結果を保存する
            accelerometerValue = args.Reading;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            // 画面を抜けた後なので加速度センサーのイベントを受け取らない
            if (accelerometer != null)
            {
                accelerometer.ReadingChanged -= accelerometer_ReadingChanged;
            }
        }


        void timer_Tick(object sender, object e)
        {
            if (accelerometerValue == null || !marimo.IsAccelerometer)
            {
                // ラベルの情報を更新する
                labelError.Visibility = Windows.UI.Xaml.Visibility.Visible;
                labelX.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                labelY.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                labelZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                labelError.Text = "センサー：無し";

                // 加速度の値が未取得の状態またはセンサーが存在しない場合
                // マリモを下に移動させる
                marimo.Top++;
            }
            else
            {
                // ラベルの情報を更新する
                labelError.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                labelX.Visibility = Windows.UI.Xaml.Visibility.Visible;
                labelY.Visibility = Windows.UI.Xaml.Visibility.Visible;
                labelZ.Visibility = Windows.UI.Xaml.Visibility.Visible;

                // 加速度センサーの値を更新する
                labelX.Text = string.Format("x: {0:0.000}", accelerometerValue.AccelerationX);
                labelY.Text = string.Format("y: {0:0.000}", accelerometerValue.AccelerationY);
                labelZ.Text = string.Format("z: {0:0.000}", accelerometerValue.AccelerationZ);

                // 傾きが大きい方向の向きにマリモを移動させる
                if (Math.Abs(accelerometerValue.AccelerationX) < Math.Abs(accelerometerValue.AccelerationY))
                {
                    // マリモを上下に移動させる
                    if (accelerometerValue.AccelerationY < 0)
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
                    // マリモを左右に移動させる
                    if (accelerometerValue.AccelerationX < 0)
                    {
                        marimo.Left--;
                    }
                    else
                    {
                        marimo.Left++;
                    }
                }
            }

            // マリモが画面外へ出ていかないように値を補正します
            marimo.Top = Math.Min(marimo.Top, screenGrid.ActualHeight - marimo.Size);
            marimo.Top = Math.Max(marimo.Top, 0);
            marimo.Left = Math.Min(marimo.Left, screenGrid.ActualWidth - marimo.Size);
            marimo.Left = Math.Max(marimo.Left, 0);
        }

        private void marimoEllipse_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            marimo.Top += e.Position.Y;
            marimo.Left += e.Position.X;
        }
    }
}
