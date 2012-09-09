using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture; 
using Windows.Storage.Streams;
using System;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Pickers;

using ImageProcessingSample.Media.Effects;


// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace ImageProcessingSample
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、通常、ページを構成するために使用します。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        // アプリバーの表示に連動して、エフェクト選択バーを表示させる
        private void AppBar_Opened_1(object sender, object e)
        {
            OpenEffectBarStoryboard.Begin();
        }

        // アプリバーの非表示に連動して、エフェクト選択バーを非表示にする
        private void AppBar_Closed_1(object sender, object e)
        {
            CloseEffectBarStoryboard.Begin();
        }

        private async Task<WriteableBitmap> CreateBitmapAsync(StorageFile file)
        {
            // ファイルストリームを開きます
            var stream = await file.OpenReadAsync();

            // 非同期で新しいデコーダーを生成し、ストリームからピクセルデータをデコードする
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var pixelData = await decoder.GetPixelDataAsync();
            var bytes = pixelData.DetachPixelData();

            // WriteableBitmapオブジェクトを生成し、デコード済みのピクセルデータを上書きする
            var bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
            using (var pixelStream = bitmap.PixelBuffer.AsStream())
            {
                await pixelStream.WriteAsync(bytes, 0, bytes.Length);
            }

            return bitmap;
        }


        // アプリ内で保持する加工後の画像
        WriteableBitmap dstBitmap = null;

        // アプリ内で保持する元の画像
        WriteableBitmap srcBitmap = null;

        // アプリバーの撮影ボタンから画像ファイルを開く
        private async void buttonPhoto_Click(object sender, RoutedEventArgs e)
        {
            // エラーが発生した時の文言
            var errorMessage = default(string);

            // 撮影ダイアログを表示して、撮影後の画像ファイルを取得する
            var capture = new CameraCaptureUI();
            try
            {
                var file = await capture.CaptureFileAsync(CameraCaptureUIMode.Photo);
                if (file != null)
                {
                    // ファイルを読み込んで画像を表示する
                    srcBitmap = await CreateBitmapAsync(file);

                    // 元画像をImageコントロールへ表示する
                    effectedImage.Source = srcBitmap;
                }
                else
                {
                    errorMessage = "撮影処理がキャンセルされました";
                }
            }
            catch (Exception ex)
            {
                errorMessage = "カメラデバイスの起動に失敗しました\n" + ex.Message;
            }

            // エラーが発生した場合はメッセージダイアログでユーザーに通知する
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var msgDlg = new MessageDialog(errorMessage);
                await msgDlg.ShowAsync();
            }
        }

        // 非同期でストレージファイルを開きます
        private async Task<StorageFile> GetStorageFileAsync()
        {
            // FileOpenPickerのインスタンスを生成する
            var picker = new FileOpenPicker();
            // ファイルピッカーが最初に表示するディレクトリを、ピクチャーライブラリに設定する
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            // ファイルピッカーの表示モードをサムネイル表示モードに設定する
            picker.ViewMode = PickerViewMode.Thumbnail;

            // ファイルピッカーで表示する拡張子を表示する
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            // ファイルピッカーを表示してファイルを1つ開く
            return await picker.PickSingleFileAsync();
        }

        // アプリバーのAddボタンから画像ファイルを開く
        private async void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            // ファイル選択ピッカーからStorageFileオブジェクトを取得します
            var file = await GetStorageFileAsync();
            if (file != null)
            {
                // ファイルストリームからWriteableBitmapオブジェクトを作成します
                srcBitmap = await CreateBitmapAsync(file);

                // 元画像をImageコントロールへ表示する
                effectedImage.Source = srcBitmap;
            }
        }

        private void buttonNormal_Click(object sender, RoutedEventArgs e)
        {
            // まだ元画像が読み込みされていなければ何もせずに終了する
            if (srcBitmap == null)
            {
                return;
            }

            // Imageコントロールへ表示する
            effectedImage.Source = srcBitmap;
        }

        private void buttonMonochrome_Click(object sender, RoutedEventArgs e)
        {
            // まだ元画像が読み込みされていなければ何もせずに終了する
            if (srcBitmap == null)
            {
                return;
            }

            // 画像をグレイスケール化する
            dstBitmap = srcBitmap.EffectGrayscale();

            // 加工後の画像をImageコントロールへ表示する
            effectedImage.Source = dstBitmap;
        }
    }
}
