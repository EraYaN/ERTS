using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication {
    public static class ComVars {
        //Booleans
        public const byte ONBOARD_LED1 = 0x01;
        public const byte ONBOARD_LED2 = 0x02;
        public const byte ONBOARD_LED3 = 0x03;
        public const byte ONBOARD_LED4 = 0x04;

        //Int16s

        //Int32s
        public const byte CONFIG_UPDATETICKINTERVAL = 0x43;

        //Floats        
        public const byte DATA_POWER = 0x60;
        public const byte DATA_CURRENT = 0x61;
        public const byte DATA_DUTY = 0x62;
        public const byte DATA_REALVOLTAGE = 0x63;
        public const byte DATA_REALCURRENT = 0x64;
        public const byte CONFIG_TICKTIME = 0x65;

        //FloatArray
        public const byte DATA_5DPOINT = 0x70;
    }
}
