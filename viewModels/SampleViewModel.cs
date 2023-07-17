using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lside_Mixture.viewModels
{
    public class SampleViewModel
    {
        public SampleViewModel() { }

        public int Altitude { get; set; }
        public int RPM { get; set; }
        public double EGT { get; set; }
        public int Throttle { get; set; }
        public int Mixture { get; set;}
    }
}
