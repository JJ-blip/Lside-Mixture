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
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MixtureChart.xaml
    /// </summary>
    public partial class MixtureChartWindow : Window
    {
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        private ScatterPlot mixturePlot;
        private ScatterPlot rpmPlot;

        // rpm
        private ScottPlot.Renderable.Axis yAxis2;

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

            if (this.EGT.Data.Length > 1)
            {
                var inProcess = (TextBox)this.FindName("InProcessTextBlock");
                inProcess.Visibility = Visibility.Collapsed;

                var plt = this.wpfPlot.Plot;

                plt.YLabel("EGT F");
                plt.YAxis2.Color(Color.Red);
                plt.XLabel($"Mixture %");

                // Mixture data is plotted inverted with display sign inverted
                this.Mixture.Data = this.Mixture.Data.Select(val => -1 * val).ToArray();
                plt.XAxis.TickLabelNotation(invertSign: true);

                this.mixturePlot = this.AddScatter(plt, this.Mixture, this.EGT);
                this.mixturePlot.XAxisIndex = 0;
                this.mixturePlot.Color = Color.Red;

                this.rpmPlot = this.AddScatter(plt, this.Mixture, this.RPM);
                this.rpmPlot.Color = Color.Blue;
                this.yAxis2 = this.wpfPlot.Plot.AddAxis(ScottPlot.Renderable.Edge.Left, null, "RPM");
                this.rpmPlot.YAxisIndex = yAxis2.AxisIndex;
                this.yAxis2.Color(this.rpmPlot.Color);
                this.rpmPlot.MarkerShape = MarkerShape.triDown;

                double peakEGT = this.EGT.Data.Max();
                double mixtureAtPeakEGT = this.Mixture.Data[this.EGT.Data.ToList().IndexOf(peakEGT)];
                plt.AddHorizontalLine(peakEGT - 50, this.mixturePlot.Color, style: LineStyle.Dash, label: "EGT - 50 F");

                var hline = plt.AddHorizontalLine(peakEGT);
                hline.Color = this.mixturePlot.Color;
                hline.LineWidth = 1;
                hline.PositionLabel = true;
                hline.PositionLabelBackground = hline.Color;
                hline.DragEnabled = true;

                var vline = plt.AddVerticalLine(mixtureAtPeakEGT);
                vline.LineWidth = 1;
                vline.PositionLabel = true;
                vline.PositionLabelBackground = hline.Color;
                vline.DragEnabled = true;

                var ypos = Convert.ToInt32(this.EGT.Data.Min()) + 20;
                plt.AddText("Rich", -95, ypos, size: 14, color: Color.Red);
                plt.AddText("Lean", -30, ypos, size: 14, color: Color.Red);

                plt.Legend();

                this.wpfPlot.Plot.AxisAuto();
                this.wpfPlot.Refresh();
            }
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
