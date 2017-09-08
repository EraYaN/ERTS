using System.ComponentModel;


namespace ERTS.Dashboard
{
    public enum Modes
    {   
        [Description("Safe")]
        Safe = 0,
        [Description("Panic")]
        Panic = 1,
        [Description("Manual control")]
        Manual = 2,
        [Description("Calibration")]
        Calibration = 3,
        [Description("Yaw control")]
        Yaw = 4,
        [Description("Full control")]
        Full = 5,
        [Description("Raw")]
        Raw = 6,
        [Description("Height control")]
        Height = 7,
        [Description("Wireless control")]
        Wireless = 8
    }

    class Quadrupel
    {
        public Modes Mode { get; set; }
        public int Voltage { get; set; }

        public Quadrupel()
        {

        }
    }
}
