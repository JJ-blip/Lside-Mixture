namespace Lside_Mixture.Views
{
    using System;
    using System.Linq;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows;
    using Lside_Mixture.ViewModels;
    using ScottPlot;
    using ScottPlot.Plottable;

    /// <summary>
    /// Interaction logic for MixtureChart.xaml
    /// </summary>
    public partial class MixtureChartWindow : Window
    {
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        private ScatterPlot altitudePlot;
        private ScatterPlot egtPlot;
        private ScatterPlot throttlePlot;
        private ScatterPlot mixturePlot;
        private ScatterPlot rpmPlot;

        // rpm
        private ScottPlot.Renderable.Axis yAxis2;
        // temp
        private ScottPlot.Renderable.Axis yAxis3;
        // feet
        private ScottPlot.Renderable.Axis yAxis4;

        public MixtureChartWindow()
        {
            InitializeComponent();

            this.backgroundWorker.DoWork += this.BackgroundWorker_DoWork;
            this.backgroundWorker.RunWorkerCompleted += this.backgroundWorker_Completed;

            if (!this.backgroundWorker.IsBusy)
            {
                this.backgroundWorker.RunWorkerAsync();
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // long running
            new MixtureChartViewModel(this);
        }

        private void backgroundWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work

            var plt = this.wpfPlot.Plot;

            plt.YLabel(this.EGT.Title);
            plt.XLabel($"Mixture %");

            this.mixturePlot = this.AddScatter(plt, this.Mixture, this.EGT);
            this.mixturePlot.XAxisIndex = 0;

            this.rpmPlot = this.AddScatter(plt, this.Mixture, this.RPM);
            this.yAxis2 = this.wpfPlot.Plot.AddAxis(ScottPlot.Renderable.Edge.Left);
            this.rpmPlot.YAxisIndex = yAxis2.AxisIndex;
            this.yAxis2.Color(this.rpmPlot.Color);
            this.rpmPlot.MarkerShape = MarkerShape.triDown;


            double peakEGT = this.EGT.Data.Max();
            plt.AddHorizontalLine(peakEGT - 50, this.mixturePlot.Color, style: LineStyle.Dash,  label: "EGT - 50");
            int peakEGTMinus50 = Convert.ToInt32(peakEGT - 50);
            string txt = String.Format("Peak EGT -50 is {0}", peakEGTMinus50);

            /*
            var fancy = plt.AddAnnotation(txt, Alignment.UpperLeft);
            fancy.MarginX = 20;
            fancy.MarginY = 40;
            fancy.Font.Size = 12;
            //fancy.Font.Name = "Impact";
            fancy.Font.Color = Color.Black;
            fancy.Shadow = false;
            fancy.BorderWidth = 2;
            fancy.BorderColor = Color.Magenta;
            */

            plt.Legend();

            this.wpfPlot.Plot.AxisAuto();
            this.wpfPlot.Refresh();
        }

        public PlotData<double> Altitude { get; internal set; } = new PlotData<double>("Altitude", new double[1]);

        public PlotData<double> EGT { get; internal set; } = new PlotData<double>("EGT", new double[1]);

        public PlotData<double> Throttle { get; internal set; } = new PlotData<double>("Throttle", new double[1]);

        public PlotData<double> Mixture { get; internal set; } = new PlotData<double>("Mixture", new double[1]);

        public PlotData<double> RPM { get; internal set; } = new PlotData<double>("RPM", new double[1]);

        private ScatterPlot AddScatter(Plot plot, PlotData<double> xPlotData, PlotData<double> yPlotData)
        {
            return plot.AddScatter(xPlotData.Data, yPlotData.Data, label: yPlotData.Title);
        }
    }
}
