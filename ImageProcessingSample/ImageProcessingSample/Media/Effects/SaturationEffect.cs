using System;

namespace ImageProcessingSample.Media.Effects
{
    /// <summary>
    /// 彩度調整処理をおこなうクラス
    /// </summary>
    public class SaturationEffect : IEffect
    {
        private double Saturation { get; set; }

        /// <summary>
        /// BakumatsuEffect クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="saturation">彩度を表現する(0.0～1.0 標準:0.5)</param>
        public SaturationEffect(double saturation)
        {
            Saturation = saturation * 2;
        }

        /// <summary>
        /// 彩度調整処理をおこなう
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
                double b = source[index + 0];
                double g = source[index + 1];
                double r = source[index + 2];
                double a = source[index + 3];

                // 単純平均法で輝度を求める
                double y = (b + g + r) / 3;

                // 各成分の値に輝度の分を引き、色の
                b = y + Saturation * (b - y);
                g = y + Saturation * (g - y);
                r = y + Saturation * (r - y);

                // 処理後のバッファへピクセル情報を保存する
                dest[index + 0] = (byte)Math.Min(255, Math.Max(0, b));
                dest[index + 1] = (byte)Math.Min(255, Math.Max(0, g));
                dest[index + 2] = (byte)Math.Min(255, Math.Max(0, r));
                dest[index + 3] = source[index + 3];
            }

            return dest;
        }
    }
}
