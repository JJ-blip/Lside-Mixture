namespace Lside_Mixture.Views
{
    using System.Drawing;
    using System.Windows;
    using Lside_Mixture.ViewModels;
    using ScottPlot;
    using ScottPlot.Plottable;
    using System.Windows.Controls;
    using System.ComponentModel;


    /// <summary>
    /// Interaction logic for PeakEGTWindow.xaml
    /// </summary>
    public partial class PeakEGTWindow : Window
    {
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        public PeakEGTWindow()
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
            new PeakEGTViewModel(this);
        }

        private void backgroundWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
        }
        }
}
