using System;

namespace ImageProcessingSample.Media.Effects
{
    public class SepiaEffect : IEffect
    {
        /// <summary>
        /// セピア調変換処理をおこなう
        /// </summary>
        /// <param name="width">ビットマップの幅</param>
        /// <param name="height">ビットマップの高さ</param>
        /// <param name="source">処理前のピクセルデータ</param>
        /// <returns>処理後のピクセルデータ</returns>
        public byte[] Effect(int width, int height, byte[] source)
        {
            int pixelCount = width * height;
            var dest = new byte[source.Length];

            for (int i = 0; i < pixelCount; i++)
            {
                var index = i * 4;

                // 処理前のピクセルの各ARGB要素を取得する
                var b = source[index + 0];
                var g = source[index + 1];
                var r = source[index + 2];
                var a = source[index + 3];

                // 単純平均法で輝度を求める
                var y = (double)(r + g + b) / 3;

                // 輝度に対して指定の比率を掛けてセピア調に変換する
                var db = y * 0.4;
                var dg = y * 0.7;
                var dr = y;

                // 処理後のバッファへピクセル情報を保存する
                dest[index + 0] = (byte)Math.Min(255, Math.Max(0, db));
                dest[index + 1] = (byte)Math.Min(255, Math.Max(0, dg));
                dest[index + 2] = (byte)Math.Min(255, Math.Max(0, dr));
                dest[index + 3] = a;
            }

            return dest;
        }
    }
}
