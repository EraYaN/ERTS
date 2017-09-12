using MicroMvvm;
using System;

namespace ERTS.Dashboard.Input
{
    [Serializable]
    public class ControlActuator : ObservableObject
    {
        public int RawOffset;
        public Guid DeviceGuid;
        public string Name;
        public string ControlDisplayName;    
    }
}
