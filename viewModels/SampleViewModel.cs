namespace Lside_Mixture.ViewModels
{
    using System;
    using System.ComponentModel;
    using Lside_Mixture.Models;
    using Lside_Mixture.Services;
    using Microsoft.Extensions.DependencyInjection;

    public class SampleViewModel : BindableBase
    {
        private readonly ISimService simservice = App.Current.Services.GetService<ISimService>();

        public SampleViewModel() 
        {
            simservice.SampleModel.PropertyChanged += this.ModelPropertyChanged;
        }

        public string Name
        { 
            get { return name; }
            set { SetProperty(ref name, value, "Name"); } 
        }

        public int Altitude
        {
            get { return altitude; }
            set { SetProperty(ref altitude, value, "Altitude"); }
        }

        public int RPM
        {
            get { return rpm; }
            set { SetProperty(ref rpm, value, "RPM"); }
        }
        public int EGT
        {
            get { return egt; }
            set { SetProperty(ref egt, value, "EGT"); }
        }

        public int Throttle
        {
            get { return throttle; }
            set { SetProperty(ref throttle, value, "Throttle"); }
        }

        public int Mixture
        {
            get { return mixture; }
            set { SetProperty(ref mixture, value, "Mixture"); }
        }

        public void update(PlaneInfoResponse planeInfo)
        {
            this.Name = planeInfo.Title;
            this.Altitude = Convert.ToInt32(planeInfo.Altitude);
            this.RPM = Convert.ToInt32(planeInfo.RPM);
            this.EGT = Convert.ToInt32(planeInfo.EGT);
            this.Throttle = Convert.ToInt32(planeInfo.Throttle);
            this.Mixture = Convert.ToInt32(planeInfo.Mixture);
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "PlaneInfoResponse":
                    {
                        this.update(((SampleModel)sender).planeInfoResponse);
                    }
                    break;
            }
        }

        private string name;
        private int altitude;
        private int rpm;
        private int egt;
        private int throttle;
        private int mixture;

    }
}
