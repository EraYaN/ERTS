using ERTS.Dashboard.Communication.Enumerations;
using ERTS.Dashboard.Input.Enumerations;
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
        const int KeyThreshold = 64;
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
            //TODO get ControlActuator[] from config.
            ControlActuator keyboardEsc = new ControlActuator() { DeviceGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"), ControlDisplayName = "Keyboard - Esc", RawOffset = DirectInputRawOffsets.KB_ESCAPE };
            ControlActuator keyboard0 = new ControlActuator() { DeviceGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"), ControlDisplayName = "Keyboard - 0", RawOffset = DirectInputRawOffsets.KB_0 };
            ControlActuator keyboard1 = new ControlActuator() { DeviceGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"), ControlDisplayName = "Keyboard - 1", RawOffset = DirectInputRawOffsets.KB_1 };
            ControlActuator keyboard2 = new ControlActuator() { DeviceGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"), ControlDisplayName = "Keyboard - 2", RawOffset = DirectInputRawOffsets.KB_2 };
            ControlActuator keyboard3 = new ControlActuator() { DeviceGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"), ControlDisplayName = "Keyboard - 3", RawOffset = DirectInputRawOffsets.KB_3 };
            ControlActuator keyboard4 = new ControlActuator() { DeviceGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"), ControlDisplayName = "Keyboard - 4", RawOffset = DirectInputRawOffsets.KB_4 };
            ControlActuator keyboard5 = new ControlActuator() { DeviceGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"), ControlDisplayName = "Keyboard - 5", RawOffset = DirectInputRawOffsets.KB_5 };
            ControlActuator joyPadX = new ControlActuator() { DeviceGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"), ControlDisplayName = "Xbox 360 - X", RawOffset = DirectInputRawOffsets.XBOX_LEFTSTICK_X };
            ControlActuator joyPadY = new ControlActuator() { DeviceGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"), ControlDisplayName = "Xbox 360 - Y", RawOffset = DirectInputRawOffsets.XBOX_LEFTSTICK_Y };
            ControlActuator joyPadRotX = new ControlActuator() { DeviceGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"), ControlDisplayName = "Xbox 360 - RotationX", RawOffset = DirectInputRawOffsets.XBOX_RIGHTSTICK_X };
            ControlActuator joyPadRotY = new ControlActuator() { DeviceGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"), ControlDisplayName = "Xbox 360 - RotationY", RawOffset = DirectInputRawOffsets.XBOX_RIGHTSTICK_Y };

           

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

            InputBinding LiftTrimUpBinding = new InputBinding(new ControlActuator[] { }, "LiftTrimUp");
            LiftTrimUpBinding.BindingActuatedEvent += LiftTrimUpBinding_BindingActuatedEvent;
            InputBindings.Add("LiftTrimUp", LiftTrimUpBinding);

            InputBinding LiftTrimDownBinding = new InputBinding(new ControlActuator[] { }, "LiftTrimDown");
            LiftTrimUpBinding.BindingActuatedEvent += LiftTrimDownBinding_BindingActuatedEvent;
            InputBindings.Add("LiftTrimDown", LiftTrimUpBinding);

            InputBinding RollTrimUpBinding = new InputBinding(new ControlActuator[] { }, "RollTrimUp");
            RollTrimUpBinding.BindingActuatedEvent += RollTrimUpBinding_BindingActuatedEvent;
            InputBindings.Add("RollTrimUp", RollTrimUpBinding);

            InputBinding RollTrimDownBinding = new InputBinding(new ControlActuator[] { }, "RollTrimDown");
            RollTrimUpBinding.BindingActuatedEvent += RollTrimDownBinding_BindingActuatedEvent;
            InputBindings.Add("RollTrimDown", RollTrimUpBinding);

            InputBinding PitchTrimUpBinding = new InputBinding(new ControlActuator[] { }, "PitchTrimUp");
            PitchTrimUpBinding.BindingActuatedEvent += PitchTrimUpBinding_BindingActuatedEvent;
            InputBindings.Add("PitchTrimUp", PitchTrimUpBinding);

            InputBinding PitchTrimDownBinding = new InputBinding(new ControlActuator[] { }, "PitchTrimDown");
            PitchTrimUpBinding.BindingActuatedEvent += PitchTrimDownBinding_BindingActuatedEvent;
            InputBindings.Add("PitchTrimDown", PitchTrimUpBinding);

            InputBinding YawTrimUpBinding = new InputBinding(new ControlActuator[] { }, "YawTrimUp");
            YawTrimUpBinding.BindingActuatedEvent += YawTrimUpBinding_BindingActuatedEvent;
            InputBindings.Add("YawTrimUp", YawTrimUpBinding);

            InputBinding YawTrimDownBinding = new InputBinding(new ControlActuator[] { }, "YawTrimDown");
            YawTrimUpBinding.BindingActuatedEvent += YawTrimDownBinding_BindingActuatedEvent;
            InputBindings.Add("YawTrimDown", YawTrimUpBinding);

            InputBinding ModeSafeBinding = new InputBinding(new ControlActuator[] { keyboard0 }, "ModeSafe");
            ModeSafeBinding.BindingActuatedEvent += ModeSafeBinding_BindingActuatedEvent;
            InputBindings.Add("ModeSafe", ModeSafeBinding);

            InputBinding ModePanicBinding = new InputBinding(new ControlActuator[] { keyboard1 }, "ModePanic");
            ModePanicBinding.BindingActuatedEvent += ModePanicBinding_BindingActuatedEvent;
            InputBindings.Add("ModePanic", ModePanicBinding);

            InputBinding ModeManualBinding = new InputBinding(new ControlActuator[] { keyboard2 }, "ModeManual");
            ModeManualBinding.BindingActuatedEvent += ModeManualBinding_BindingActuatedEvent;
            InputBindings.Add("ModeManual", ModeManualBinding);

            InputBinding ModeCalibrationBinding = new InputBinding(new ControlActuator[] { keyboard4 }, "ModeCalibration");
            ModeCalibrationBinding.BindingActuatedEvent += ModeCalibrationBinding_BindingActuatedEvent;
            InputBindings.Add("ModeCalibration", ModeCalibrationBinding);

            InputBinding ModeYawControlBinding = new InputBinding(new ControlActuator[] { keyboard4 }, "ModeYawControl");
            ModeYawControlBinding.BindingActuatedEvent += ModeYawControlBinding_BindingActuatedEvent;
            InputBindings.Add("ModeYawControl", ModeYawControlBinding);

            InputBinding ModeFullControlBinding = new InputBinding(new ControlActuator[] { keyboard5 }, "ModeFullControl");
            ModeFullControlBinding.BindingActuatedEvent += ModeFullControlBinding_BindingActuatedEvent;
            InputBindings.Add("ModeFullControl", ModeFullControlBinding);

        }
        private void LiftInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            GlobalData.ctr.SetLift((e.InnerEvent.StateUpdate.Value / 65536.0 - 0.5) * -1 + 0.5);
        }

        private void RollInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            GlobalData.ctr.SetRoll((e.InnerEvent.StateUpdate.Value / 65536.0 - 0.5) * 2);
        }

        private void PitchInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            GlobalData.ctr.SetPitch((e.InnerEvent.StateUpdate.Value / 65536.0 - 0.5) * 2);
        }

        private void YawInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            GlobalData.ctr.SetYaw((e.InnerEvent.StateUpdate.Value / 65536.0 - 0.5) * 2);
        }

        private void AbortInputBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if(e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.Abort();
        }

        private void LiftTrimUpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustLiftTrim(true);
        }

        private void LiftTrimDownBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustLiftTrim(false);
        }

        private void RollTrimUpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustRollTrim(true);
        }

        private void RollTrimDownBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustRollTrim(false);
        }

        private void PitchTrimUpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustPitchTrim(true);
        }

        private void PitchTrimDownBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustPitchTrim(false);
        }

        private void YawTrimUpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustYawTrim(true);
        }

        private void YawTrimDownBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustYawTrim(false);
        }

        private void ModeSafeBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.ModeSwitch(FlightMode.Safe);
        }

        private void ModePanicBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.ModeSwitch(FlightMode.Panic);
        }

        private void ModeManualBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.ModeSwitch(FlightMode.Manual);
        }
        private void ModeCalibrationBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.ModeSwitch(FlightMode.Calibration);
        }
        
        private void ModeYawControlBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.ModeSwitch(FlightMode.YawControl);
        }

        private void ModeFullControlBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.ModeSwitch(FlightMode.FullControl);
        }
        
    }
}
