namespace Lside_Mixture.ViewModels
{
    using Lside_Mixture.Services;

    public class SampleViewModel : BindableBase
    {
        public SampleViewModel() 
        {
            Name = "Name...";
            Altitude = 10;
        }

        public string Name{ get; set; }
        public int Altitude { get; set; }
        public int RPM { get; set; }
        public double EGT { get; set; }
        public int Throttle { get; set; }
        public int Mixture { get; set;}
    }
}
