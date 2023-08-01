namespace Lside_Mixture.Managers
{
    using System;
    using System.Runtime.InteropServices;
    using CTrue.FsConnect;

    /// <summary>
    /// The <see cref="IEngineManager"/> controls the engine in the current aircraft.
    /// </summary>
    /// <remarks>
    /// Supports:
    /// - Get and set heading bug.
    ///
    /// Usage:
    /// Call <see cref="Update"/> to refresh properties with latest values from MSFS.
    /// </remarks>
    public interface IEngineManager : IFsConnectManager
    {
        /// <summary>
        /// Gets the current Mixture, in %.
        /// </summary>
        double Mixture { get; }

        /// <summary>
        /// Sets the engine Mixture, in %.
        /// </summary>
        /// <param name="heading"></param>
        void SetMixture(double mixture);
    }

    public class EngineManager : FsConnectManager, IEngineManager
    {
        private int _eventGroupId;

        private EngineSimVars _engineSimVars = new EngineSimVars();
        private int _engineManagerSimVarsReqId;
        private int _engineManagerSimVarsDefId;

        private int _mixtureSetEventId;

        /// <inheritdoc />
        public double Mixture { get; private set; }

        /// <summary>
        /// Creates a new <see cref="EngineManager"/> instance.
        /// </summary>
        /// <param name="fsConnect"></param>
        public EngineManager(IFsConnect fsConnect)
            : base(fsConnect)
        {
        }

        protected override void RegisterSimVars()
        {
            _engineManagerSimVarsReqId = _fsConnect.GetNextId();
            _engineManagerSimVarsDefId = _fsConnect.RegisterDataDefinition<EngineSimVars>();
        }

        protected override void RegisterEvents()
        {
            _eventGroupId = _fsConnect.GetNextId();
            _mixtureSetEventId = _fsConnect.GetNextId();
            _fsConnect.MapClientEventToSimEvent(_eventGroupId, _mixtureSetEventId, FsEventNameId.Mixture1Set);

            _fsConnect.SetNotificationGroupPriority(_eventGroupId);
        }

        protected override void OnFsDataReceived(object sender, FsDataReceivedEventArgs e)
        {
            if (e.Data.Count == 0) return;
            if (!(e.Data[0] is EngineSimVars)) return;

            _engineSimVars = (EngineSimVars)e.Data[0];
            _resetEvent.Set();
        }

        /// <inheritdoc />
        public override void Update()
        {
            _fsConnect.RequestData(_engineManagerSimVarsReqId, _engineManagerSimVarsDefId);
            WaitForUpdate();

            Mixture = _engineSimVars.Mixture;
        }

        /// <inheritdoc />
        public void SetMixture(double mixture)
        {
            int mix16k = Convert.ToInt32(mixture * 16383.0 / 100.0);
            _fsConnect.TransmitClientEvent(_mixtureSetEventId, (uint)mix16k, _eventGroupId);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        internal struct EngineSimVars
        {
           [SimVar(NameId = FsSimVar.GeneralEngMixtureLeverPosition, Instance = 1, UnitId = FsUnit.PercentScaler16k)]            
           public double Mixture;
        }
    }
}
