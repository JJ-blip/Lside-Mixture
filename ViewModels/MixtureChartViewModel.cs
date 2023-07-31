﻿namespace Lside_Mixture.ViewModels
{
    using System;
    using System.Data;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Lside_Mixture.Services;
    using Lside_Mixture.Utils;
    using Lside_Mixture.Views;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public class MixtureChartViewModel : BindableBase
    {
        // max acceptable drop in RPM
        private const int RpmDropThreshold = 200;
        // max acceptable drom in Peak EGT, from observed peak.
        private const int EgtDropThreshold = 100;
        // difference between 2 successive reading, need to be below this value to be 'stable'
        private const double TempStabilisedThreshold = 2;

        private const int EgtChangeWaitDelayMSec = 10000;
        private readonly ISimService simservice = App.Current.Services.GetService<ISimService>();

        private BoundedQueue<double> EgtSamples;

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

            MixtureArray = new PlaneInfoResponse[50];
            MixtureArray[0] = simservice.MostRecentSample;
            var initialMixture = MixtureArray[0].Mixture;
            var mix = initialMixture;

            int idx = 1;
            while (mix > 15 && this.RpmOk() && this.EgtOk() && idx < (MixtureArray.Length - 1))
            {
                EgtSamples = new BoundedQueue<double>(5);

                mix = mix - 3;
                simservice.SetMixture(mix);

                // practically never stabilises in 5 s , thus we always waits 5 seconds
                int waitDelay = new TaskUtil().WaitUntil(EgtChangeWaitDelayMSec, EGTStabilised);

                {
                    Log.Information(String.Format("reading {0:0000} F at {1:00}% after {2} sec", simservice.MostRecentSample.EGT, simservice.MostRecentSample.Mixture, waitDelay));
                }

                MixtureArray[idx++] = simservice.MostRecentSample;
            }
            simservice.SetMixture(initialMixture);
            MixtureArray = MixtureArray.Where(x => x.Mixture != 0).ToArray();
        }

        // return true if RPM has not dropped significantly from its peak
        private bool RpmOk()
        {
            double[] rpmArray = MixtureArray.Where(x => x.Mixture != 0).ToArray().Select(item => item.RPM).ToArray();
            double peak = rpmArray.Max();
            if ((rpmArray.Length == 1) || peak - rpmArray[rpmArray.Length -1] < RpmDropThreshold)
            {
                // not enough data, or most current value has dropped too much
                return true;
            }
            Log.Debug("RPM has dropped to {0:0} from its {1:0} peak", rpmArray[rpmArray.Length - 1], peak );
            return false;
        }

        private bool EgtOk()
        {
            double[] egtArray = MixtureArray.Where(x => x.Mixture != 0).ToArray().Select(item => item.EGT).ToArray();
            double peak = egtArray.Max();
            if ((egtArray.Length == 1) || peak - egtArray[egtArray.Length - 1] < EgtDropThreshold)
            {
                // not enough data, or most current value has dropped too much
                return true;
            }
            Log.Debug("EGT has dropped to {0:0} from its {1:0} peak", egtArray[egtArray.Length - 1], peak);
            return false;
        }

        // EGT takes a long time (too long) to stabalise, so in practice, waiting 5 seconds just hives an indicative temperature, RPM is quicker 
        private bool EGTStabilised()
        {
            bool stabilised = false;

            if (EgtSamples.Count() < 3)
            {
                // need 3 samples at least 1 second apart
                EgtSamples.Enqueue(simservice.MostRecentSample.EGT);
                return false;
            }

            EgtSamples.Enqueue(simservice.MostRecentSample.EGT);
            var diff = Math.Abs(EgtSamples.ElementAt(EgtSamples.Count() - 1) - EgtSamples.ElementAt(EgtSamples.Count() - 2));
            if (diff < TempStabilisedThreshold)
            {
                // last 2
                stabilised = true;
            }

            return stabilised;
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
