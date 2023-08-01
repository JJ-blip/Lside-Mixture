namespace Lside_Mixture.Models
{
    using Lside_Mixture.Services;

    // Model expose data for consumption by WPF.It implements INotifyPropertyChanged.
    public class SampleModel : BindableBase
    {
        public PlaneInfoResponse planeInfoResponse { get; private set; }

        public void Handle(PlaneInfoResponse response)
        {
            this.planeInfoResponse = response;
            this.OnPropertyChanged("PlaneInfoResponse");
        }
    }
}
