using System.ComponentModel;

namespace AccelerometerSample
{
    /// <summary>
    /// マリモの情報を管理するクラス
    /// </summary>
    public class Marimo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty(ref double storage, double value, string info)
        {
            if (storage == value) return;
            else storage = value;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// 加速度の有無
        /// </summary>
        public bool IsAccelerometer { get; set; }

        /// <summary>
        /// マリモのサイズ
        /// </summary>
        private double SizeValue = 100;
        public double Size
        {
            get { return SizeValue; }
            set { SetProperty(ref SizeValue, value, "Size"); }
        }

        /// <summary>
        /// マリモの位置(上からの距離)
        /// </summary>
        private double TopValue = 100;
        public double Top
        {
            get { return TopValue; }
            set { SetProperty(ref TopValue, value, "Top"); }
        }

        /// <summary>
        /// マリモの位置(左からの距離)
        /// </summary>
        private double LeftValue = 50;
        public double Left
        {
            get { return LeftValue; }
            set { SetProperty(ref LeftValue, value, "Left"); }
        }
    }
}
