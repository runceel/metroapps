using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Okazuki.UI.Flyouts
{
    /// <summary>
    /// ソフトウェアキーボード等にも対応したBottomAppBar上のボタンの上に表示するフライアウト用のPopupコントロールを作成します。
    /// </summary>
    public static class FlyoutUtils
    {
        // フライアウトを表示する際の左右の画面マージン
        private static readonly int RightAndLeftMargin = 20;
        // フライアウトを表示する際の下の画面マージン
        private static readonly int BottomMargin = 20;

        /// <summary>
        /// BottomAppBar上にフラウアウトを表示するためのPopupを作成します。
        /// </summary>
        /// <typeparam name="appBar">フライアウトを表示する対象のコントロールの乗っているBottomAppBar</typeparam>
        /// <typeparam name="TTarget">上部にフライアウトを表示するターゲットの型</typeparam>
        /// <typeparam name="TFlyoutContent">フライアウト内に表示するコンテンツの型</typeparam>
        /// <typeparam name="closedAction">フライアウトが閉じる時の処理</typeparam>
        /// <param name="target">上部にフラウアウトを表示するターゲット</param>
        /// <param name="content">フライアウト内に表示するコンテンツ。WidthとHeightが明示的に指定されている必要があります。</param>
        /// <returns>フライアウト表示用のPopup</returns>
        public static Popup CreateFlyout<TTarget, TFlyoutContent>(AppBar appBar, TTarget target, TFlyoutContent content)
            where TTarget : FrameworkElement
            where TFlyoutContent : FrameworkElement
        {
            // 余白を考慮した画面幅
            content.Width = Math.Min(Window.Current.Bounds.Width - RightAndLeftMargin * 2, content.Width);

            var popup = new Popup
            {
                // コンテンツを子としてもつPopup
                Child = content,
                // Popup外を操作された場合に自動で閉じるようにする
                IsLightDismissEnabled = true
            };

            var pt = CalcuratePosition(target, content);
            // Popupの表示位置を設定
            Canvas.SetTop(popup, pt.Y);
            Canvas.SetLeft(popup, pt.X);

            // ソフトウェアキーボードの表示時の再計算処理を行う。
            var pane = InputPane.GetForCurrentView();
            TypedEventHandler<InputPane, InputPaneVisibilityEventArgs> inputPaneVisibilityChanged = (_, e) =>
            {
                if (e.EnsuredFocusedElementInView)
                {
                    // ソフトウェアキーボードが隠れるときはフライアウトの位置をリセットする
                    Canvas.SetTop(popup, pt.Y);
                    Canvas.SetLeft(popup, pt.X);
                    return;
                }

                // ソフトウェアキーボードが表示された時
                Canvas.SetTop(popup, pt.Y - e.OccludedRect.Height);
                Canvas.SetLeft(popup, pt.X);
            };

            // フライアウトが表示されたらソフトウェアキーボードの状態監視を行う。
            popup.Opened += (_, __) =>
            {
                pane.Showing += inputPaneVisibilityChanged;
                pane.Hiding += inputPaneVisibilityChanged;
            };

            // フライアウトが表示されなくなったタイミングでソフトウェアキーボードの状態監視を辞める。
            popup.Closed += (_, __) =>
            {
                pane.Showing -= inputPaneVisibilityChanged;
                pane.Hiding -= inputPaneVisibilityChanged;
            };

            // AppBarが閉じられたらフライアウトも閉じる
            EventHandler<object> closed = null;
            closed = (_, __) =>
            {
                popup.IsOpen = false;
                appBar.Closed -= closed;
            };
            appBar.Closed += closed;

            return popup;
        }

        private static Point CalcuratePosition<TTarget, TFlyoutContent>(TTarget target, TFlyoutContent content)
            where TTarget : FrameworkElement
            where TFlyoutContent : FrameworkElement
        {
            // targetを起点とした座標変換を行うクラスを作成
            var gt = target.TransformToVisual(null);
            // targetの左上の座標を取得
            var pt = gt.TransformPoint(new Point());
            // フライアウトを表示する左端の座標を計算
            pt.X += target.ActualWidth - content.Width - RightAndLeftMargin;
            pt.X = Math.Max(RightAndLeftMargin, pt.X);
            // フラウアウトを表示する上端の座標を計算
            pt.Y -= content.Height + BottomMargin;

            return pt;
        }
    }
}
