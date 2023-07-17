using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gauge
{
    /// <summary>
    /// Interaction logic for UserControlGauge.xaml
    /// </summary>
    public partial class UserControlGauge : UserControl
    {
        public UserControlGauge()
        {
            Angle = -85;
            Value = 0;
            InitializeComponent();
        }

        int _maxValue = 170;
        public int maxValue
        {
            get
            {
                return _maxValue;
            }

            set
            {
                _maxValue = value;
            }
        }

        int _value;
        public int Value
        {
            get
            {
                return Convert.ToInt32(_value * maxValue / 170);
            }

            set
            {
                if (value >= 0 && value <= maxValue)
                {
                    _value = value;
                    Angle = Convert.ToInt32(value * 170 / maxValue);
                }
            }
        }

        // dependent property
        int _angle;
        public int Angle
        {
            get
            {
                return _angle;
            }

            private set
            {
                _angle = value;
            }
        }
    }
}
