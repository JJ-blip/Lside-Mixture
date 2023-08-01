namespace Gauge
{
    using System;
    using System.ComponentModel;
    public class GaugeViewModel : INotifyPropertyChanged
    {

        private double minValue;
        private double maxValue;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if(PropertyChanged!= null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public GaugeViewModel()
        {
            Angle = -85;
            Value = 0;
        }

        public GaugeViewModel(double minValue, double maxValue)
        {
            Angle = -85;
            Value = 0;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        int _angle;

        // 0 to 170 usage 
        public int Angle
        {
            get
            {
                return _angle;
            }

            private set
            {
                _angle = value;
                NotifyPropertyChanged("Angle");
            }
        }

        // minValue to maxValue usage
        public int ScaledValue
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                Angle = Convert.ToInt32((170.0 / this.maxValue) * (value - this.minValue));
                NotifyPropertyChanged("Value");
            }
        }

        int _value;
        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (value >= 0 && value <= 170)
                {
                    _value = value;
                    Angle = value - 85;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}
