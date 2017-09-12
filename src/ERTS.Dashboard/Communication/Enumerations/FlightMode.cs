namespace ERTS.Dashboard.Communication.Enumerations
{
    /// <summary>
    /// The Quad flight mode, 4 bits
    /// </summary>
    public enum FlightMode : byte
    {
        Safe = 0x0,
        Panic = 0x1,
        Manual = 0x2,
        Calibration = 0x3,
        YawControl = 0x4,
        FullControl = 0x5,
        Raw = 0x6,
        Height = 0x7,
        Wireless = 0x8,
        None = 0xF
    }
}
