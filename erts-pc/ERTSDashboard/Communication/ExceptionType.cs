using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication
{
    public enum ExceptionType : short
    {
        UnknownException = 0x0000,
        InvalidModeException = 0x0001,
        NotCalibratedException = 0x0002,
        ValidationException = 0x0003
    }
}
