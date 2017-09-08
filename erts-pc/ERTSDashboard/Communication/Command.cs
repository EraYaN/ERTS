using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication {
    public enum Command : byte {
        //Control codes
        OK = 0xF1,
        Reset = 0xF2,
        Refresh = 0xF3,
        Forbidden = 0xF4,
        NotFound = 0xF5,
        TypeMismatch = 0xF6,
        VersionUnsupported = 0xFE,
        Unsupported = 0xFF,
        //Int16 commands
        SetInteger16 = 0x13,
        GetInteger16 = 0x15,
        //Int32 commands
        SetInteger32 = 0x23,
        GetInteger32 = 0x25,
        //Boolean commands
        SetBoolean = 0x33,
        GetBoolean = 0x35,
        //Float commands
        SetFloat = 0x43,
        GetFloat = 0x45,
        //String commands
        SetString = 0x53,
        GetString = 0x55,
        //Array commands
        SetFloatArray = 0x63,
        GetFloatArray = 0x65,
        //Function commands
        Debug = 0xE6
    }
}
