using System.ComponentModel;

namespace ERTS.Dashboard.Communication.Enumerations
{
    /// <summary>
    /// The Quad flight mode, 4 bits
    /// </summary>
    public enum FlightMode : byte
    {
        [Description("Safe")]
        Safe = 0x0,
        [Description("Panic")]
        Panic = 0x1,
        [Description("Manual control")]
        Manual = 0x2,
        [Description("Calibration")]
        Calibration = 0x3,
        [Description("Yaw control")]
        YawControl = 0x4,
        [Description("Full control")]
        FullControl = 0x5,
        [Description("Raw")]
        Raw = 0x6,
        [Description("Height control")]
        Height = 0x7,
        [Description("Wireless control")]
        Wireless = 0x8,
        [Description("Dump flash")]
        DumpFlash = 0x9,
        [Description("No mode")]
        None = 0xF
    }
}
