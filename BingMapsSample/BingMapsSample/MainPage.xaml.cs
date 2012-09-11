using Bing.Maps;
using System;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BingMapsSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        Geolocator geolocator = null;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Geolocatorオブジェクトを生成する
            if (geolocator == null)
            {
                geolocator = new Geolocator();
            }
            // PositionChangedイベントを発生させる移動距離(単位:メートル)
            geolocator.MovementThreshold = 20;
            // 位置が変わったら発生するPositionChangedイベントにイベントハンドラを関連付ける
            geolocator.PositionChanged += geolocator_PositionChanged;
            // 状態が変わったら発生するStatusChangedイベントにイベントハンドラを関連付ける
            geolocator.StatusChanged += geolocator_StatusChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            geolocator.PositionChanged -= geolocator_PositionChanged;
        }

        async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            // 位置情報から地理的情報を取得する
            Geocoordinate coordinate = args.Position.Coordinate;

            // 緯度・経度・精度のログを出力する
            var msg = string.Format("緯度:{0} 経度:{1} 精度:{2}",
                        coordinate.Latitude, coordinate.Longitude, coordinate.Accuracy);
            System.Diagnostics.Debug.WriteLine(msg);

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                map.Center = new Location(coordinate.Latitude, coordinate.Longitude);
                AddPushPin(coordinate);
            });
        }

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            Debug.WriteLine("StatusChanged: " + args.Status);

            switch (args.Status)
            {
                case PositionStatus.NotInitialized:
                    // 初期化前の状態。GetGeopositionAsyncメソッドを実行していないか
                    // PositionChangedイベントハンドラが設定されていない状態です。
                    break;

                case PositionStatus.Initializing:
                    // 初期化中の状態。
                    break;

                case PositionStatus.Ready:
                    // 位置情報を扱える状態。
                    break;

                case PositionStatus.NoData:
                    // どのロケーションプロバイダーのどの位置情報も使えない状態。
                    // GetGeopositionAsyncメソッドを実行した直後か
                    // PositionChangedイベントハンドラが設定されている状態で、
                    // 位置情報が扱えるようになるとReadyへ状態遷移します。
                    break;

                case PositionStatus.Disabled:
                    // ユーザーが「位置情報」にアクセスする許可を与えていない状態。
                    break;
            }
        }


        private void btnClicked(object sender, RoutedEventArgs e)
        {
            GetAddress();
        }

        private void AddPushPin(Geocoordinate coordinate)
        {
            var pin = new Pushpin();
            pin.Background = new SolidColorBrush(Colors.Purple);
            MapLayer.SetPosition(pin, 
                new Location(coordinate.Latitude, coordinate.Longitude));
            map.Children.Add(pin);
        }

        private async void GetAddress()
        {
            // 現在位置を取得する
            var geolocator = new Geolocator();
            var pos = await geolocator.GetGeopositionAsync();

            // アドレス情報を取得する
            var address = pos.CivicAddress;

            // 現在位置の国名を取得する
            var country = address.Country;
            
            // 現在位置の都道府県を取得する
            var state = address.State;

            // 現在位置の市を取得する
            var city = address.City;

            // 現在位置の郵便番号を取得する
            var postalCode = address.PostalCode;
        }

        private async void GetAccuracy()
        {
            // 現在位置を取得する
            var geolocator = new Geolocator();
            var pos = await geolocator.GetGeopositionAsync();

            // 精度(単位:メートル)を取得する
            var accuracy = pos.Coordinate.Accuracy;

            // ログを出力する
            System.Diagnostics.Debug.WriteLine(accuracy);

            // 出力：1609
        }

        private async void GetPositionHigh()
        {
            var geolocator = new Geolocator();
            // Defaultと比較して、正確な位置情報を取得するように設定
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            // 現在の位置情報を取得する
            var pos = await geolocator.GetGeopositionAsync();

            // 位置情報から地理的情報を取得する
            Geocoordinate coordinate = pos.Coordinate;

            // 緯度・経度のログを出力する
            var msg = string.Format("緯度:{0} 経度:{1}", 
                        coordinate.Latitude, coordinate.Longitude);
            System.Diagnostics.Debug.WriteLine(msg);

            // 出力：緯度:35.685001 経度:139.751404
        }

        private async void GetPosition()
        {
            var geolocator = new Geolocator();
            // 現在の位置情報を取得する
            var pos = await geolocator.GetGeopositionAsync();

            // 位置情報から地理的情報を取得する
            Geocoordinate coordinate = pos.Coordinate;

            // 緯度・経度のログを出力する
            var msg = string.Format("緯度:{0} 経度:{1}",
                        coordinate.Latitude, coordinate.Longitude);

            // 出力：緯度:35.685001 経度:139.751404
        }


        private async void GetPositionAll()
        {
            var geolocator = new Geolocator();

            // 現在の位置情報を取得する
            var pos = await geolocator.GetGeopositionAsync();

            // 現在位置の国名を取得する
            var country = pos.CivicAddress.Country;

            // 現在位置の郵便番号を取得する
            var postalCode = pos.CivicAddress.PostalCode;

            // 測位した現在地の精度を取得する(単位：メートル)
            var accuracy = pos.Coordinate.Accuracy;

            // 現在の経度を取得する
            var longitude = pos.Coordinate.Longitude;

            // 現在の緯度を取得する
            var latitude = pos.Coordinate.Latitude;

            System.Diagnostics.Debug.WriteLine("country:{0}", country);
            System.Diagnostics.Debug.WriteLine("postalCode:{0}", postalCode);
            System.Diagnostics.Debug.WriteLine("accuracy:{0}", accuracy);
            System.Diagnostics.Debug.WriteLine("longitude:{0}", longitude);
            System.Diagnostics.Debug.WriteLine("latitude:{0}", latitude);
        }

        private void SetCenter()
        {
            // 中央に大阪駅を表示させる
            map.Center = new Location(34.701189, 135.496016);
        }

        private void SetZoom10()
        {
            map.ZoomLevel = 10;
        }

        private void SetZoom18()
        {
            map.ZoomLevel = 18;
        }
    }
}
