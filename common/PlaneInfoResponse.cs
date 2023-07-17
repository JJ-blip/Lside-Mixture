namespace Lside_Mixture.Services
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// This structure must match 1 : 1 with the SimService.definition list contents.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PlaneInfoResponse
    {
        // Title
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Type;

        // Altitude 
        // RPM
        // EGT 
        // Throttle
        // Mixture 

        public override string ToString()
        {
            return $"response OnGround:{this.OnGround}, ";
        }
    }
}