using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Input
{
   public  class BindingActuatedEventArgs : EventArgs
    {        
        public readonly InputEventArgs InnerEvent;
        
        public BindingActuatedEventArgs(InputEventArgs _InnerEvent)
        {
            InnerEvent = _InnerEvent;
        }
    }
}
