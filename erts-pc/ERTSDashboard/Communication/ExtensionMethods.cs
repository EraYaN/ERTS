using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication {
    public static class ExtensionMethods {
        public static int BasePacketLength(this Command cmd) {
            switch (cmd) {
                case Command.OK:
                case Command.Reset:
                case Command.Refresh:
                case Command.Unsupported:
                case Command.Forbidden:
                case Command.NotFound:
                case Command.TypeMismatch:
                case Command.GetInteger16:
                case Command.GetInteger32:
                case Command.GetBoolean:
                case Command.GetFloat:
                case Command.GetString:
                case Command.GetFloatArray:
                    //no data, id
                    return 4;
                case Command.VersionUnsupported:
                case Command.SetBoolean:
                    //1 byte data, id
                    return 5;
                case Command.SetInteger16:
                    //2 byte int data, id
                    return 6;
                case Command.SetInteger32:
                case Command.SetFloat:
                    //4 byte int data, id
                    return 8;
                case Command.SetString:
                case Command.SetFloatArray:
                case Command.Debug:
                    //n byte string data, id
                    return 6;
                default:
                    return 4;
            }
        }
        public static bool HasNData(this Command cmd) {
            switch (cmd) {
                case Command.OK:
                case Command.Reset:
                case Command.Refresh:
                case Command.Unsupported:
                case Command.Forbidden:
                case Command.NotFound:
                case Command.TypeMismatch:
                case Command.GetInteger16:
                case Command.GetInteger32:
                case Command.GetBoolean:
                case Command.GetFloat:
                case Command.GetString:
                case Command.GetFloatArray:
                case Command.VersionUnsupported:
                case Command.SetBoolean:
                case Command.SetInteger16:
                case Command.SetInteger32:
                case Command.SetFloat:
                    return false;
                case Command.SetString:
                case Command.SetFloatArray:
                case Command.Debug:
                    return true;
                default:
                    return false;
            }
        }
    }
}
