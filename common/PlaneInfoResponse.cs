namespace Lside_Mixture.Services
{
    using System.Runtime.InteropServices;
    using CTrue.FsConnect;

    /// <summary>
    /// This structure must match 1 : 1 with the SimService.definition list contents.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PlaneInfoResponse
    {
        // Title
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Title;

        [SimVar(NameId = FsSimVar.PlaneAltitude, UnitId = FsUnit.Feet)]
        public double Altitude;

        [SimVar(NameId = FsSimVar.GeneralEngRpm, Instance = 1, UnitId = FsUnit.Rpm)]
        public double RPM;

        [SimVar(NameId = FsSimVar.EngExhaustGasTemperature, Instance = 1, UnitId = FsUnit.Fahrenheit)]
        public double EGT;

        [SimVar(NameId = FsSimVar.GeneralEngThrottleLeverPosition, Instance = 1, UnitId = FsUnit.Percentage)]
        public double Throttle;

        [SimVar(NameId = FsSimVar.GeneralEngMixtureLeverPosition, Instance = 1, UnitId = FsUnit.Percentage)]
        public double Mixture;

        public override string ToString()
       {
            return $"response Altitude:{this.Altitude},  RPM:{this.RPM}, EGT:{this.EGT}, Throttle:{this.Throttle}, Mixture:{this.Mixture}, ";
       }
    }
}