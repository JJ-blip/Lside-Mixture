namespace Lside_Mixture.ViewModels
{
    using Gauge;
    using Lside_Mixture.Services;

    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel() { }

        public SampleViewModel SampleViewModel { get; set; }

        public GaugeViewModel GaugeViewModel { get; set; }
    }
}
