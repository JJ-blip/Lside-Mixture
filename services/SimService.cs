namespace Lside_Mixture.Services
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Threading;
    using CTrue.FsConnect;
    using Lside_Mixture.Managers;
    using Lside_Mixture.Models;
    using Serilog;

    /// <summary>
    ///
    /// Exposes:
    ///   Connected, true if connected to MSFS.
    ///
    /// </summary>
    public class SimService : BindableBase, ISimService, INotifyPropertyChanged
    {
        // ms
        private const int SampleRate = 20;

        // flag to ensure only one SimConnect data packet being processed at a time
        private static bool safeToRead = true;

        private static bool simulationIsPaused = false;

        private static SampleModel sampleModel = new SampleModel();

        // timer, task reads data from a SimConnection
        private readonly DispatcherTimer dataReadDispatchTimer = new DispatcherTimer();

        // timer, task establishes connection if disconnected
        private readonly DispatcherTimer connectionDispatchTimer = new DispatcherTimer();

        // Establishes simConnect connection & Update scaneris
        private readonly BackgroundWorker backgroundWorkerConnection = new BackgroundWorker();

        private readonly FsConnect fsConnect = new FsConnect();

        private readonly EngineManager engineManager;

        private int planeInfoDefinitionId;

        private bool running = false;

        private bool connected = false;

        private bool crashed = false;

        public SimService()
        {
            // do a 'Connection check' every 1 sec
            this.connectionDispatchTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            this.connectionDispatchTimer.Tick += new EventHandler(this.ConnectionCheckEventHandler_OnTick);

            // establishes simConnect connection, when required
            this.backgroundWorkerConnection.DoWork += this.BackgroundWorkerConnection_DoWork;
            this.connectionDispatchTimer.Start();

            // Read SimConnect Data every 20 msec
            this.dataReadDispatchTimer.Interval = new TimeSpan(0, 0, 0, 0, SampleRate);
            this.dataReadDispatchTimer.Tick += new EventHandler(this.DataReadEventHandler_OnTick);

            // register the read SimConnect data callback procedure
            this.fsConnect.FsDataReceived += HandleReceivedFsData;
            this.fsConnect.PauseStateChanged += FsConnect_PauseStateChanged;

            this.fsConnect.Crashed += this.FsConnect_Crashed;
            this.fsConnect.FlightLoaded += this.FsConnect_Loaded;

            FsConnect managerfsConnect = new FsConnect();
            managerfsConnect.Connect("TestApp", "localhost", 500, SimConnectProtocol.Ipv4);
            this.engineManager = new EngineManager(managerfsConnect);
            this.engineManager.Initialize();
        }

        private enum Requests
        {
            PlaneInfoRequest = 0,
        }

        public bool Crashed
        {
            get { return this.crashed; }
            private set { this.crashed = value; }
        }

        public bool Connected
        {
            get { return this.connected; }
            private set { this.connected = value; }
        }

        public SampleModel SampleModel {
            get { return sampleModel; }  
        }

        public PlaneInfoResponse MostRecentSample
        {
            get { return mostRecentplaneInfoResponse; }
        }

        /// <summary>
        /// Sets the engine Mixture, in %.
        /// </summary>
        /// <param name="heading"></param>
        public void SetMixture(double mixture)
        {
            engineManager.SetMixture(mixture);
            engineManager.Update();
        }

        private static PlaneInfoResponse mostRecentplaneInfoResponse;

        private static void HandleReceivedFsData(object sender, FsDataReceivedEventArgs e)
        {
            if (simulationIsPaused)
            {
                return;
            }

            if (!safeToRead)
            {
                // already processing a packet, skip this one
                Log.Debug("lost one");
                return;
            }

            safeToRead = false;
            try
            {
                if (e.RequestId == (uint)Requests.PlaneInfoRequest)
                {
                    var planeInfoResponse = (PlaneInfoResponse)e.Data.FirstOrDefault();
                    mostRecentplaneInfoResponse = planeInfoResponse;
                    sampleModel.Handle(planeInfoResponse);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message);
            }

            safeToRead = true;
        }

        private static void FsConnect_PauseStateChanged(object sender, PauseStateChangedEventArgs e)
        {
            simulationIsPaused = e.Paused;
        }

        private void FsConnect_Crashed(object sender, EventArgs e)
        {
            this.crashed = true;
        }

        private void FsConnect_Loaded(object sender, EventArgs e)
        {
            this.crashed = false;
        }

        private void DataReadEventHandler_OnTick(object sender, EventArgs e)
        {
            try
            {
                this.fsConnect.RequestData((int)Requests.PlaneInfoRequest, this.planeInfoDefinitionId);
            }
            catch
            {
            }
        }

        private void ConnectionCheckEventHandler_OnTick(object sender, EventArgs e)
        {
            if (!this.backgroundWorkerConnection.IsBusy)
            {
                this.backgroundWorkerConnection.RunWorkerAsync();
            }

            bool oldConnected = this.Connected;

            if (this.fsConnect.Connected)
            {
                if (!this.running)
                {
                    this.running = true;
                    this.dataReadDispatchTimer.Start();
                }

                this.Connected = true;
            }
            else
            {
                this.Connected = false;
            }

            if (this.Connected != oldConnected)
            {
                this.OnPropertyChanged(nameof(this.Connected));
            }
        }

        // If not connected , Connect
        private void BackgroundWorkerConnection_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!this.fsConnect.Connected)
            {
                try
                {
                    // connect & register data of interest
                    this.fsConnect.Connect("TestApp", "localhost", 500, SimConnectProtocol.Ipv4);
                    this.planeInfoDefinitionId = this.fsConnect.RegisterDataDefinition<PlaneInfoResponse>();
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}
