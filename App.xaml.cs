namespace Lside_Mixture
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Lside_Mixture.Services;
    using Lside_Mixture.Views;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public partial class App : Application
    {
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            this.Services = ConfigureServices();
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

        }

        /// <summary>
        /// Gets the current application
        ///  note: Not a Lamdba, just a expression-bodied member 'syntax' defining a read-only Property.
        /// </summary>
        public static new App Current => (App)Application.Current;

        // IOC provider
        public IServiceProvider Services { get; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.SetupExceptionHandling();

            MainWindow window = new MainWindow();
            window.Show();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ISimService, SimService>();

            return services.BuildServiceProvider();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                this.LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            this.DispatcherUnhandledException += (s, e) =>
            {
                this.LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                this.LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception e, string source)
        {
            MessageBox.Show($"{source} - {e.Message}");
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string logout = "\n\n" + DateTime.Now.ToString() + "\n" + System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion + "\n" +
                e.Message + "\n" + e.Source + "\n" + e.StackTrace;
            System.IO.File.AppendAllText(@"./log.txt", logout);
        }
    }
}
