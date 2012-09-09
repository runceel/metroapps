
namespace ImageProcessingSample.Media.Effects
{
    /// <summary>
    /// グレースケール処理をおこなうクラス
    /// </summary>
    public class GrayscaleEffect : IEffect
    {
        /// <summary>
        /// グレースケール処理をおこなう
        /// </summary>
        /// <param name="width">ビットマップの幅</param>
        /// <param name="height">ビットマップの高さ</param>
        /// <param name="source">処理前のピクセルデータ</param>
        /// <returns>処理後のピクセルデータ</returns>
        public byte[] Effect(int width, int height, byte[] source)
        {
            // ピクセルデータの数を計算する
            int pixelCount = width * height;

            // 処理後のピクセルデータを格納するためのバッファを生成する
            var dest = new byte[source.Length];

            for (int i = 0; i < pixelCount; i++)
            {
                var index = i * 4;

                // 処理前のピクセルから各BGAR要素を取得する
                var b = source[index + 0];
                var g = source[index + 1];
                var r = source[index + 2];
                var a = source[index + 3];

                // 単純平均法で輝度を求める
                var sum = (double)(r + g + b);
                var y = (byte)(sum / 3);

                // 処理後のピクセルデータを出力用バッファへ格納する
                dest[index + 0] = y;
                dest[index + 1] = y;
                dest[index + 2] = y;
                dest[index + 3] = a;
            }

            return dest;
        }
    }
}
