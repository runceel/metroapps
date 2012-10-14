using ImageProcessingSample.Media.Effects;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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

        private async Task<WriteableBitmap> CreateBitmapAsync(StorageFile file)
        {
            // デコード後の画像のサイズを格納する
            int width, height;
            // デコード後のピクセルデータを格納する
            var bytes = default(byte[]);

            // ファイルストリームを開きます
            using (var stream = await file.OpenReadAsync())
            {
                // 非同期で新しいデコーダーを生成する
                var decoder = await BitmapDecoder.CreateAsync(stream);
                // 幅と高さを取得する
                width = (int)decoder.PixelWidth;
                height = (int)decoder.PixelHeight;
                // デコード後のピクセルデータを取得する
                var pixelData = await decoder.GetPixelDataAsync();
                bytes = pixelData.DetachPixelData();
            }

            // WriteableBitmapオブジェクトを生成し、デコード済みのピクセルデータを上書きする
            var bitmap = new WriteableBitmap(width, height);
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

            // ファイルピッカーで表示する拡張子を追加する
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

        // エフェクト選択バーが表示されている状態であるか？のフラグ
        bool isOpenEditBar = false;

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (isOpenEditBar)
            {
                // エフェクト選択バーを非表示にする
                CloseEffectBarStoryboard.Begin();
                isOpenEditBar = false;
            }
            else
            {
                // エフェクト選択バーを表示する
                OpenEffectBarStoryboard.Begin();
                isOpenEditBar = true;
            }
        }

        // 処理前の画像をImageコントロールに表示する
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

        // グレースケール処理した画像をImageコントロールに表示する
        private void buttonMonochrome_Click(object sender, RoutedEventArgs e)
        {
            if (srcBitmap == null) return;

            // 画像をグレースケール化する
            dstBitmap = srcBitmap.EffectGrayscale();

            // 加工後の画像をImageコントロールへ表示する
            effectedImage.Source = dstBitmap;
        }

        // セピア調処理した画像をImageコントロールに表示する
        private void buttonSepia_Click(object sender, RoutedEventArgs e)
        {
            if (srcBitmap == null) return;

            // 画像をセピア調化処理する
            dstBitmap = srcBitmap.EffectSepia();

            // 加工後の画像をImageコントロールへ表示する
            effectedImage.Source = dstBitmap;
        }

        // 幕末写真風処理した画像をImageコントロールに表示する
        private async void buttonBakumatsu_Click(object sender, RoutedEventArgs e)
        {
            if (srcBitmap == null) return;

            // 古紙画像をアプリ内のリソースから読み出す
            var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/bakumatsu.jpg"));
            var bakumatsuBmp = await CreateBitmapAsync(imageFile);

            // 画像を幕末写真風に処理する
            dstBitmap = srcBitmap.EffectBakumatsu(bakumatsuBmp);

            // 加工後の画像をImageコントロールへ表示する
            effectedImage.Source = dstBitmap;
        }

        // トイカメラ写真風処理した画像をImageコントロールに表示する
        private async void buttonToycamera_Click(object sender, RoutedEventArgs e)
        {
            if (srcBitmap == null) return;

            // 口径色を表現した画像をアプリ内のリソースから読み出す
            var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/vignetting_gradation.png"));
            var vignettingBmp = await CreateBitmapAsync(imageFile);

            // 画像をトイカメラ風に処理する
            dstBitmap = srcBitmap.EffectToycamera(vignettingBmp);

            // 加工後の画像をImageコントロールへ表示する
            effectedImage.Source = dstBitmap;
        }

        private async void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            // まだ画像がエフェクト処理されていなければメソッドを終了する
            if (dstBitmap == null)
            {
                return;
            }

            // ファイル保存用ダイアログのインスタンスを生成する
            var picker = new FileSavePicker();

            // 選択可能な拡張子とデフォルトで選択されている拡張子を指定する
            picker.FileTypeChoices.Add("JPEGファイル", new List<string> { ".jpg", ".jpeg" });
            picker.DefaultFileExtension = ".jpg";
            
            // 現在時刻から保存先のデフォルトのファイル名を付ける
            var fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            picker.SuggestedFileName = fileName;

            // 画像なのでデフォルトの保存先をピクチャーライブラリにする
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            
            // ファイル保存用ダイアログで保存先のファイルを取得する
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                // 保存したい画像のピクセルデータを取り出す
                var bytes = new byte[dstBitmap.PixelBuffer.Length];
                using (var strm = dstBitmap.PixelBuffer.AsStream())
                {
                    strm.Position = 0;
                    strm.Read(bytes, 0, bytes.Length);
                }

                // ユーザーが指定したファイルのストリームを開く
                var writeStrm = await file.OpenAsync(FileAccessMode.ReadWrite);
                
                // JPEGのエンコーダーを使ってピクセルデータをエンコードして、
                // ストリームへ書き出す
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, writeStrm);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                    (uint)dstBitmap.PixelWidth, (uint)dstBitmap.PixelHeight, 96, 96, bytes);
                await encoder.FlushAsync();
            }
        }
    }
}
