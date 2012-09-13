using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
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
    }
}
