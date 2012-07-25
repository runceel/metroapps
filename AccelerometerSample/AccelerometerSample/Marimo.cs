using AccelerometerSample.Common;

namespace AccelerometerSample
{
    /// <summary>
    /// マリモの情報を管理するクラス
    /// </summary>
    public class Marimo : BindableBase
    {
        /// <summary>
        /// 加速度の有無を取得または設定します。
        /// </summary>
        public bool IsAccelerometer { get; set; }

        private int size = 50;
        /// <summary>
        /// マリモのサイズを取得または設定します。
        /// </summary>
        public int Size
        {
            get { return this.size; }
            set { this.SetProperty<int>(ref this.size, value); }
        }

        private double top = 100;
        /// <summary>
        /// マリモの画面上からの位置を取得または設定します。
        /// </summary>
        public double Top
        {
            get { return this.top; }
            set { this.SetProperty<double>(ref this.top, value); }
        }

        private double left = 50;
        /// <summary>
        /// マリモの画面左からの位置を取得または設定します。
        /// </summary>
        public double Left
        {
            get { return this.left; }
            set { this.SetProperty<double>(ref this.left, value); }
        }
    }
}
