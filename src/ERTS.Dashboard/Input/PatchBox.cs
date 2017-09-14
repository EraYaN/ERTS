using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Input
{
    public class PatchBox
    {
        InputBinding YawInputBinding;

        public PatchBox()
        {
            YawInputBinding = new InputBinding(new ControlActuator()); //TODO get ControlActuator from config.
            YawInputBinding.BindingActuatedEvent += YawInputBinding_BindingActuatedEvent;
        }

        private void YawInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            //TODO figure out Value scaling and keyboard vs joystick statechange using GetType or something.
            //TODO set value in controller.
            //GlobalData.ctr.SetYaw(e.InnerEvent.StateUpdate.Value);
            Debug.WriteLine("Sending Yaw Input to Controller, by method call.");
        }
    }
}
