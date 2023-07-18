namespace Lside_Mixture
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Gauge;
    using Lside_Mixture.Services;
    using Lside_Mixture.ViewModels;
    using Microsoft.Extensions.DependencyInjection;
    using Octokit;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string updateUri;

        private readonly ISimService simservice = App.Current.Services.GetService<ISimService>();

        private readonly MainWindowViewModel viewModel;

        // main window - drag & drop
        private bool mouseDown;

        public MainWindow()
        {
           InitializeComponent();

            this.viewModel = new MainWindowViewModel();
            this.viewModel.SampleViewModel = new SampleViewModel();

            this.viewModel.GaugeViewModel = new GaugeViewModel();

            // defined within the MainWindow.xml file
            SampleStackPanel.DataContext = this.viewModel.SampleViewModel;
            GuageStackPanel.DataContext = this.viewModel.GaugeViewModel;

        }

        private void SampleViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "PlaneInfoResponse":
                    {
                    }
                    break;
            }
        }

        // displays Graph
        private void ButtonGraph_Click(object sender, RoutedEventArgs e)
        {

        }

        // Leans the plane at optimum mixture
        private void ButtonLean_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(updateUri);
        }

        /** Git Hub Updater, amends displayed URL in the Main Window. **/
        private void BackgroundWorkerUpdate_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var client = new GitHubClient(new Octokit.ProductHeaderValue("Lside-Mixture"));
            var releases = client.Repository.Release.GetAll("jj-blip", "lside-Mixture").Result;
            var latest = releases[0];
            var tagVersion = latest.TagName.Remove(0, 1);

            var currentVersion = new Version(this.viewModel.Version);
            var latestGitVersion = new Version(tagVersion);
            var versionDifference = latestGitVersion.CompareTo(currentVersion);

            this.viewModel.UpdateAvailable = versionDifference > 0;
            updateUri = latest.HtmlUrl;
        }

        /** MainWindow drag and drop  **/
        private void Header_LoadedHandler(object sender, RoutedEventArgs e)
        {
            this.InitHeader(sender as TextBlock);
        }

        private void InitHeader(TextBlock header)
        {
            header.MouseUp += new MouseButtonEventHandler(this.OnMouseUp);
            header.MouseLeftButtonDown += new MouseButtonEventHandler(this.MouseLeftButtonDown);
            header.MouseMove += new MouseEventHandler(this.OnMouseMove);
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.mouseDown = true;
            }

            this.DragMove();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.mouseDown = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.mouseDown)
            {
                var mouseX = e.GetPosition(this).X;
                var width = this.RestoreBounds.Width;
                var x = mouseX - (width / 2);

                if (x < 0)
                {
                    x = 0;
                }
                else
                if (x + width > SystemParameters.PrimaryScreenWidth)
                {
                    x = SystemParameters.PrimaryScreenWidth - width;
                }

                this.WindowState = WindowState.Normal;
                this.Left = x;
                this.Top = 0;
                this.DragMove();
            }
        }

        /* Handlers for UI */

        private void Button_Hide_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
