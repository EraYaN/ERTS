using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication
{
    public enum FlightMode
    {
        Safe = 0x00,
        Panic = 0x01,
        Manual = 0x02,
        Calibration = 0x03,
        YawControl = 0x04,
        FullControl = 0x05,
        Raw = 0x06,
        Height = 0x07,
        Wireless = 0x08,
        Unknown = 0xFF
    }
}
