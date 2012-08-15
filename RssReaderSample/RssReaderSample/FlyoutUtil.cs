using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RssReaderSample
{
    static class FlyoutUtil
    {
        /// <summary>
        /// Flyoutの表示場所を計算する
        /// </summary>
        /// <param name="target">表示場所の起点となるAppBar上のボタン</param>
        /// <param name="flyout">表示対象のUserControl</param>
        /// <returns>表示場所</returns>
        public static Point CalcFlyoutPosition(FrameworkElement target, UserControl flyout)
        {
            // 座標変換オブジェクトを取得
            var transform = target.TransformToVisual(null);
            // flyoutのサイズから表示場所を算出
            var point = transform.TransformPoint(
                new Point(
                    -flyout.Width + target.ActualWidth,
                    -flyout.Height - 20));
            // 左端により過ぎている場合は20にする
            point.X = Math.Max(point.X, 20);
            return point;
        }
    }
}
