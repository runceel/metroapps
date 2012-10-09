using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace ImageProcessingSample.Media.Effects
{
    public static class WriteableBitmapExtensions
    {
        /// <summary>
        /// バイト配列からWriteableBitmapを生成する
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="array">ピクセルデータ</param>
        /// <returns>WriteableBitmapオブジェクト</returns>
        public static WriteableBitmap FromArray(int width, int height, byte[] array)
        {
            // 出力用のWriteableBitmapオブジェクトを生成する
            var bitmap = new WriteableBitmap(width, height);
            // WriteableBitmapへバイト配列のピクセルデータをコピーする
            using (var pixelStream = bitmap.PixelBuffer.AsStream())
            {
                pixelStream.Seek(0, SeekOrigin.Begin);
                pixelStream.Write(array, 0, array.Length);
            }
            return bitmap;
        }

        /// <summary>
        /// 画像処理をおこなう
        /// </summary>
        /// <param name="bmp">元になるWriteableBitampオブジェクト</param>
        /// <param name="effecter">エフェクト処理クラス</param>
        /// <returns>WriteableBitampオブジェクト</returns>
        private static WriteableBitmap Effect(this WriteableBitmap bmp, IEffect effector)
        {
            // WriteableBitampのピクセルデータをバイト配列に変換する
            var pixels = bmp.PixelBuffer.ToArray();

            // 画像処理をおこなう
            pixels = effector.Effect(bmp.PixelWidth, bmp.PixelHeight, pixels);

            // バイト配列からピクセルを作成する
            return WriteableBitmapExtensions.FromArray(bmp.PixelWidth, bmp.PixelHeight, pixels);
        }

        /// <summary>
        /// 連続して画像処理をおこなう
        /// </summary>
        /// <param name="bmp">元になるWriteableBitampオブジェクト</param>
        /// <param name="effecter">エフェクト処理クラスのリスト</param>
        /// <returns>WriteableBitampオブジェクト</returns>
        private static WriteableBitmap Effect(this WriteableBitmap bmp, IEnumerable<IEffect> effectors)
        {
            // WriteableBitampのピクセルデータをバイト配列に変換する
            var pixels = bmp.PixelBuffer.ToArray();

            foreach (var effector in effectors)
            {
                // 画像処理をおこなう
                pixels = effector.Effect(bmp.PixelWidth, bmp.PixelHeight, pixels);
            }

            // バイト配列からピクセルを作成する
            return WriteableBitmapExtensions.FromArray(bmp.PixelWidth, bmp.PixelHeight, pixels);
        }

        /// <summary>
        /// グレースケール(モノクロ)処理をしたWriteableBitampオブジェクトを返す
        /// </summary>
        /// <param name="bitmap">元になるWriteableBitampオブジェクト</param>
        /// <returns>WriteableBitampオブジェクト</returns>
        public static WriteableBitmap EffectGrayscale(this WriteableBitmap bmp)
        {
            return bmp.Effect(new GrayscaleEffect());
        }

        public static WriteableBitmap EffectSepia(this WriteableBitmap bmp)
        {
            return bmp.Effect(new SepiaEffect());
        }

        public static WriteableBitmap EffectContrast(this WriteableBitmap bmp, double contrast)
        {
            return bmp.Effect(new ContrastEffect(contrast));
        }

        public static WriteableBitmap EffectSaturatio(this WriteableBitmap bmp, double saturatio)
        {
            return bmp.Effect(new SaturationEffect(saturatio));
        }

        public static WriteableBitmap EffectToycamera(this WriteableBitmap bmp, WriteableBitmap maskBmp)
        {
            // マスク画像を元画像のサイズにリサイズする
            var resizedBmp = maskBmp.Resize(bmp.PixelWidth, bmp.PixelHeight);

            // コントラスト調整、彩度調整、口径食風処理のエフェクトオブジェクトを作成
            var effectors = new List<IEffect>();
            effectors.Add(new ContrastEffect(0.8));
            effectors.Add(new SaturationEffect(0.7));
            effectors.Add(new VignettingEffect(resizedBmp, 0.8));

            // 複数のエフェクト処理を実行する
            return Effect(bmp, effectors);
        }

        public static WriteableBitmap EffectVignetting(this WriteableBitmap bmp, WriteableBitmap maskBmp, double vignetting)
        {
            // マスク画像を元画像のサイズにリサイズする
            var resizedBmp = maskBmp.Resize(bmp.PixelWidth, bmp.PixelHeight);

            return Effect(bmp, new VignettingEffect(resizedBmp, vignetting));
        }

        public static WriteableBitmap EffectBakumatsu(this WriteableBitmap bmp, WriteableBitmap maskBmp)
        {
            // マスク画像を元画像のサイズにリサイズする
            var resizedBmp = maskBmp.Resize(bmp.PixelWidth, bmp.PixelHeight);

            return Effect(bmp, new BakumatsuEffect(resizedBmp));
        }

        /// <summary>
        /// リサイズする
        /// </summary>
        /// <param name="bmp">WriteableBitmapオブジェクト</param>
        /// <param name="destWidth">変形後の幅</param>
        /// <param name="destHeight">変形後の高さ</param>
        /// <returns>リサイズ後のWriteableBitmapオブジェクト</returns>
        public static WriteableBitmap Resize(this WriteableBitmap bmp, int destWidth, int destHeight)
        {
            // 加工前のWriteableBitampオブジェクトからピクセルデータ等を取得する
            var srcWidth = bmp.PixelWidth;
            var srcHeight = bmp.PixelHeight;
            if ((srcWidth == destWidth) && (srcHeight == destHeight))
            {
                // リサイズする必要がないのでそのままビットマップを返す
                return bmp;
            }

            var srcPixels = bmp.PixelBuffer.ToArray();
            int pixelCount = destWidth * destHeight;
            var destPixels = new byte[4 * pixelCount];

            var xs = (float)srcWidth / destWidth;
            var ys = (float)srcHeight / destHeight;

            for (var y = 0; y < destHeight; y++)
            {
                for (var x = 0; x < destWidth; x++)
                {
                    var index = (y * destWidth + x) * 4;

                    var sx = x * xs;
                    var sy = y * ys;
                    var x0 = (int)sx;
                    var y0 = (int)sy;

                    var srcIndex = (y0 * srcWidth + x0) * 4;

                    destPixels[index + 0] = srcPixels[srcIndex + 0];
                    destPixels[index + 1] = srcPixels[srcIndex + 1];
                    destPixels[index + 2] = srcPixels[srcIndex + 2];
                    destPixels[index + 3] = srcPixels[srcIndex + 3];
                }
            }

            // ピクセルデータからWriteableBitmapオブジェクトを生成する
            return WriteableBitmapExtensions.FromArray(destWidth, destHeight, destPixels);
        }
    }
}
