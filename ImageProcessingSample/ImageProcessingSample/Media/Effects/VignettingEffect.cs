﻿using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;

namespace ImageProcessingSample.Media.Effects
{
    /// <summary>
    /// 周辺光の低出力
    /// </summary>
    public class VignettingEffect : IEffect
    {
        /// <summary>
        /// 周辺光を表現するマスク画像
        /// </summary>
        private WriteableBitmap MaskBitamp { get; set; }

        /// <summary>
        /// マスク画像の不透明度
        /// </summary>
        private double Opacity { get; set; }

        /// <summary>
        /// VignettingEffect クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="maskBitamp">周辺光を表現するマスク画像</param>
        /// <param name="opacity">マスク画像の不透明度を表現する(0.0～1.0 不透明:1.0)</param>
        public VignettingEffect(WriteableBitmap maskBitamp, double opacity)
        {
            MaskBitamp = maskBitamp;
            Opacity = opacity;
        }

        /// <summary>
        /// 口径食処理をおこなう
        /// </summary>
        /// <param name="width">ビットマップの幅</param>
        /// <param name="height">ビットマップの高さ</param>
        /// <param name="source">処理前のピクセルデータ</param>
        /// <returns>処理後のピクセルデータ</returns>
        public byte[] Effect(int width, int height, byte[] source)
        {
            // マスク画像のピクセルデータを取得する
            var mask = MaskBitamp.PixelBuffer.ToArray();

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

                // マスク画像のピクセルの各ARGB要素を取得する
                double mb = mask[index + 0];
                double mg = mask[index + 1];
                double mr = mask[index + 2];
                double ma = mask[index + 3];

                // 処理後のピクセルの各ARGB要素を取得する
                double db, dg, dr, da;

                // マスク画像に適用する透明率を算出する
                double ax = (ma / 255) * Opacity;

                // 指定値画像のピクセルのアルファ値をチェック
                if (ax == 0)
                {
                    // マスク画像が透明なので元画像のARGB値をそのまま代入
                    db = b;
                    dg = g;
                    dr = r;
                    da = a;
                }
                else
                {
                    // アルファ値を元に合成後のARGB値を算出
                    db = (b * (1.0 - ax)) + (mb * ax);
                    dg = (g * (1.0 - ax)) + (mg * ax);
                    dr = (r * (1.0 - ax)) + (mr * ax);
                    da = a;
                }

                // 処理後のバッファへピクセル情報を保存する
                dest[index + 0] = (byte)Math.Min(255, Math.Max(0, db));
                dest[index + 1] = (byte)Math.Min(255, Math.Max(0, dg));
                dest[index + 2] = (byte)Math.Min(255, Math.Max(0, dr));
                dest[index + 3] = (byte)Math.Min(255, Math.Max(0, da));
            }

            return dest;
        }
    }
}
