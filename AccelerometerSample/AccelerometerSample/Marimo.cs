using System.ComponentModel;

namespace AccelerometerSample
{
    public class Marimo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
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
        /// まりものサイズ
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
                if (value != SizeValue)
                {
                    SizeValue = value;
                    NotifyPropertyChanged("Size");
                }
            }
        }

        /// <summary>
        /// まりもの位置(上からの距離)
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
                if (value != TopValue)
                {
                    TopValue = value;
                    NotifyPropertyChanged("Top");
                }
            }
        }

        /// <summary>
        /// まりもの位置(左からの距離)
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
                if (value != LeftValue)
                {
                    LeftValue = value;
                    NotifyPropertyChanged("Left");
                }
            }
        }
    }
}
