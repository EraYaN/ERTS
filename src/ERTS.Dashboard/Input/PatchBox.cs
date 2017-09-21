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
        //axis
        /*InputBinding LiftInputBinding;
        InputBinding RollInputBinding;
        InputBinding PitchInputBinding;
        InputBinding YawInputBinding;*/

        //buttons
        /*InputBinding AbortInputBinding;*/

        //collection
        Dictionary<string, InputBinding> InputBindings = new Dictionary<string, InputBinding>();

        public IEnumerable<Guid> DeviceGuids {
            get {
                return InputBindings.Values.SelectMany(ca => ca.Controls).Select(control => control.DeviceGuid).Distinct();
            }
        }

        public PatchBox()
        {
            ControlActuator keyboardEsc = new ControlActuator() { DeviceGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"), ControlDisplayName = "Keyboard - Esc", RawOffset = 1 }; //TODO get ControlActuator[] from config.
            ControlActuator joyPadX = new ControlActuator() { DeviceGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"), ControlDisplayName = "Xbox 360 - X", RawOffset = 0 };
            ControlActuator joyPadY = new ControlActuator() { DeviceGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"), ControlDisplayName = "Xbox 360 - Y", RawOffset = 4 };
            ControlActuator joyPadRotX = new ControlActuator() { DeviceGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"), ControlDisplayName = "Xbox 360 - RotationX", RawOffset = 12 };
            ControlActuator joyPadRotY = new ControlActuator() { DeviceGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"), ControlDisplayName = "Xbox 360 - RotationY", RawOffset = 16 };

           

            InputBinding LiftInputBinding = new InputBinding(new ControlActuator[] { joyPadY }, "Lift");
            LiftInputBinding.BindingActuatedEvent += LiftInputBinding_BindingActuatedEvent;
            InputBindings.Add("Lift", LiftInputBinding);

            InputBinding RollInputBinding = new InputBinding(new ControlActuator[] { joyPadRotX }, "RollRate");
            RollInputBinding.BindingActuatedEvent += RollInputBinding_BindingActuatedEvent;
            InputBindings.Add("Roll", RollInputBinding);

            InputBinding PitchInputBinding = new InputBinding(new ControlActuator[] { joyPadRotY }, "PitchRate");
            PitchInputBinding.BindingActuatedEvent += PitchInputBinding_BindingActuatedEvent;
            InputBindings.Add("Pitch", PitchInputBinding);

            InputBinding YawInputBinding = new InputBinding(new ControlActuator[] { joyPadX }, "YawRate");
            YawInputBinding.BindingActuatedEvent += YawInputBinding_BindingActuatedEvent;
            InputBindings.Add("Yaw", YawInputBinding);

            InputBinding AbortInputBinding = new InputBinding(new ControlActuator[] { keyboardEsc }, "Abort");
            AbortInputBinding.BindingActuatedEvent += AbortInputBinding_BindingActuatedEvent;
            InputBindings.Add("Abort", AbortInputBinding);

        }
        private void LiftInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            GlobalData.ctr.SetLift((e.InnerEvent.StateUpdate.Value / 65536.0 - 0.5) * -1 + 0.5);
            //Debug.WriteLine("Sending Lift Input to Controller, by method call.");
        }

        private void RollInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            GlobalData.ctr.SetRoll((e.InnerEvent.StateUpdate.Value / 65536.0 - 0.5) * 2);
            //Debug.WriteLine("Sending Roll Input to Controller, by method call.");
        }

        private void PitchInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            GlobalData.ctr.SetPitch((e.InnerEvent.StateUpdate.Value / 65536.0 - 0.5) * 2);
            //Debug.WriteLine("Sending Pitch Input to Controller, by method call.");
        }

        private void YawInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            GlobalData.ctr.SetYaw((e.InnerEvent.StateUpdate.Value / 65536.0 - 0.5) * 2);
            //Debug.WriteLine("Sending Yaw Input to Controller, by method call.");
        }

        private void AbortInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if(e.InnerEvent.StateUpdate.Value > 64)
                GlobalData.ctr.Abort();
            //Debug.WriteLine("Sending Yaw Input to Controller, by method call.");
        }


    }
}
