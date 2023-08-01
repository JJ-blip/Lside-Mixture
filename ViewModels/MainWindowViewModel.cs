namespace Lside_Mixture.ViewModels
{
    using System.ComponentModel;
    using Serilog;
    using Gauge;
    using Lside_Mixture.Services;
    using Microsoft.Extensions.DependencyInjection;

    public class MainWindowViewModel : BindableBase
    {
        private readonly ISimService simService = App.Current.Services.GetService<ISimService>();

        private bool updatable = false;

        private bool connected = false;

        public MainWindowViewModel() 
        {
            this.connected = false;
            this.updatable = false;

            ((INotifyPropertyChanged)this.simService).PropertyChanged += this.Connected_PropertyChanged;
        }


        public SampleViewModel SampleViewModel { get; set; }

        public GaugeViewModel GaugeViewModel { get; set; }

        public string Version
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                string myversion = fvi.FileVersion;
                return myversion;
            }
        }

        public bool Connected
        {
            get
            {
                return this.connected;
            }

            set
            {
                this.connected = value;
                this.OnPropertyChanged("ConnectedColor");
                this.OnPropertyChanged("ConnectedString");
            }
        }

        public string ConnectedString
        {
            get
            {
                if (this.Connected)
                {
                    return "Connected";
                }
                else
                {
                    return "Disconnected";
                }
            }
        }

        public string ConnectedColor
        {
            get
            {
                if (!this.Connected)
                {
                    return "#FFE63946";
                }
                else
                {
                    return "#ff02c39a";
                }
            }
        }

        public bool UpdateAvailable
        {
            get
            {
                return this.updatable;
            }

            set
            {
                this.updatable = value;
                this.OnPropertyChanged();
            }
        }

        private void Connected_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Connected":
                    {
                        if (this.simService.Connected)
                        {
                            this.Connected = true;
                        }
                        else
                        {
                            this.Connected = false;
                        }

                        break;
                    }
            }
        }
    }
}
