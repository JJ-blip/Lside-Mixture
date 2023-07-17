namespace Lside_Mixture
{
    using System.Windows;
    using Gauge;
    using Lside_Mixture.Services;
    using Lside_Mixture.ViewModels;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly ISimService simservice = App.Current.Services.GetService<ISimService>();

        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
           InitializeComponent();

            this.viewModel = new MainWindowViewModel();
            this.viewModel.SampleViewModel = new SampleViewModel();
            this.viewModel.GaugeViewModel = new GaugeViewModel();

            this.DataContext = this.viewModel;
        }
    }
}
