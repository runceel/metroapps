using AccelerometerSample.Common;

namespace AccelerometerSample
{
    /// <summary>
    /// マリモの情報を管理するクラス
    /// </summary>
    public class Marimo : BindableBase
    {
        /// <summary>
        /// 加速度の有無
        /// </summary>
        public bool IsAccelerometer { get; set; }

        /// <summary>
        /// マリモのサイズ
        /// </summary>
        private int SizeValue = 50;
        public int Size
        {
            get
            {
                return SizeValue;
            }
            set
            {
                SetProperty<int>(ref SizeValue, value, "Size");
            }
        }

        /// <summary>
        /// マリモの位置(上からの距離)
        /// </summary>
        private double TopValue = 100;
        public double Top
        {
            get
            {
                return TopValue;
            }
            set
            {
                SetProperty<double>(ref TopValue, value, "Top");
            }
        }

        /// <summary>
        /// マリモの位置(左からの距離)
        /// </summary>
        private double LeftValue = 50;
        public double Left
        {
            get
            {
                return LeftValue;
            }
            set
            {
                SetProperty<double>(ref LeftValue, value, "Left");
            }
        }
    }
}
