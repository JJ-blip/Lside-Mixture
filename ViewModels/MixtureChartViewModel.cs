namespace Lside_Mixture.ViewModels
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using Lside_Mixture.Services;
    using Lside_Mixture.Views;
    using Microsoft.Extensions.DependencyInjection;

    public class MixtureChartViewModel : BindableBase
    {
        private readonly ISimService simservice = App.Current.Services.GetService<ISimService>();

        // The path to the csv file of slip data
        private readonly string path;

        public MixtureChartViewModel(MixtureChartWindow mixtureChartWindow)
        {
            this.MixtureChartWindow = mixtureChartWindow;

            this.CaptureAndGraphMixture();
            DataTable dt = this.GetDataTable(mixtureArray);
            this.BuildArrays(dt);
        }

        public MixtureChartWindow MixtureChartWindow { get; }

        public PlaneInfoResponse[] MixtureArray
        {
            get { return mixtureArray; }
            private set { SetProperty(ref mixtureArray, value, "MixtureArray"); }
        }

        public void BuildArrays(DataTable dt)
        {
            DataRow[] rows = dt.Select();

            double[] altitude = rows.Select(row => row[1].ToString()).ToArray()
                 .Select(d => Convert.ToDouble(d)).ToArray();
            this.MixtureChartWindow.Altitude = new PlotData<double>("Altitude", altitude);

            double[] rpm = rows.Select(row => row[2].ToString()).ToArray()
                .Select(d => Convert.ToDouble(d)).ToArray();
            this.MixtureChartWindow.RPM = new PlotData<double>("RPM", rpm);

            double[] egt = rows.Select(row => row[3].ToString()).ToArray()
                 .Select(d => Convert.ToDouble(d)).ToArray();
            this.MixtureChartWindow.EGT = new PlotData<double>("EGT", egt);

            double[] throttle = rows.Select(row => row[4].ToString()).ToArray()
                .Select(d => Convert.ToDouble(d)).ToArray();
            this.MixtureChartWindow.Throttle = new PlotData<double>("Throttle", throttle);

            double[] mixture = rows.Select(row => row[5].ToString()).ToArray()
                .Select(d => Convert.ToDouble(d)).ToArray();
            this.MixtureChartWindow.Mixture = new PlotData<double>("Mixture", mixture);

        }

        private PlaneInfoResponse planeInfoResponseSample;

        private PlaneInfoResponse[] mixtureArray;

        private void CaptureAndGraphMixture()
        {
            // leans mixture in 5% steps capturing egt etc
            // stops after peak (or rpm drops)

            MixtureArray = new PlaneInfoResponse[20];
            MixtureArray[0] = simservice.MostRecentSample;
            var initialMixture = MixtureArray[0].Mixture;
            var mix = initialMixture;

            int idx = 1;
            while (mix > 15)
            {
                mix = mix - 5;
                simservice.SetMixture(mix);
                Thread.Yield();
                Thread.Sleep(5000);
                MixtureArray[idx++] = simservice.MostRecentSample;
            }
            simservice.SetMixture(initialMixture);
            MixtureArray = MixtureArray.Where(x => x.Mixture != 0).ToArray();
        }

        private DataTable GetDataTable(PlaneInfoResponse[] mixtureArray)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Title"));
            dt.Columns.Add(new DataColumn("Altitude"));
            dt.Columns.Add(new DataColumn("RPM"));
            dt.Columns.Add(new DataColumn("EGT"));
            dt.Columns.Add(new DataColumn("Throttle"));
            dt.Columns.Add(new DataColumn("Mixture"));

            for (int rowidx = 0; rowidx < MixtureArray.Length; rowidx++)
            {
                DataRow row = dt.NewRow();
                row["Title"] = mixtureArray[rowidx].Title;
                row["Altitude"] = mixtureArray[rowidx].Altitude;
                row["RPM"] = mixtureArray[rowidx].RPM;
                row["EGT"] = mixtureArray[rowidx].EGT;
                row["Throttle"] = mixtureArray[rowidx].Throttle;
                row["Mixture"] = mixtureArray[rowidx].Mixture;
                dt.Rows.Add(row);
            }
            return dt;
        }

    }
}
