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
        Guid keyboardGuid = new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000");
        Guid joystickGuid = new Guid("241e5050-93f0-11e7-8001-444553540000"); // Lab Joystick, ERWIN_LAPTOP
        //Guid joystickGuid = new Guid("cc161970-b28e-11e7-8001-444553540000"); // Robin Joystick, ERWIN_PC
        //Guid joystickGuid = new Guid("46be56e0-a3a5-11e7-8001-444553540000") // Robin Joystick, ROBIN_LAPTOP
        //Guid xboxPadGuid = new Guid("f211f8e0-8dc4-11e7-800f-444553540000"); // xboxPad 1, ERWIN_PC
        Guid xboxPadGuid = new Guid("c701ce90-a648-11e7-8002-444553540000"); // xboxPad 1, ERWIN_LAPTOP
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
            ControlActuator keyboardEsc = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - Esc", RawOffset = DirectInputRawOffsets.KB_ESCAPE };
            ControlActuator keyboard0 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 0", RawOffset = DirectInputRawOffsets.KB_0 };
            ControlActuator keyboard1 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 1", RawOffset = DirectInputRawOffsets.KB_1 };
            ControlActuator keyboard2 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 2", RawOffset = DirectInputRawOffsets.KB_2 };
            ControlActuator keyboard3 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 3", RawOffset = DirectInputRawOffsets.KB_3 };
            ControlActuator keyboard4 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 4", RawOffset = DirectInputRawOffsets.KB_4 };
            ControlActuator keyboard5 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 5", RawOffset = DirectInputRawOffsets.KB_5 };
            ControlActuator keyboard6 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 6", RawOffset = DirectInputRawOffsets.KB_6 };
            ControlActuator keyboard7 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 7", RawOffset = DirectInputRawOffsets.KB_7 };
            ControlActuator keyboard8 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 8", RawOffset = DirectInputRawOffsets.KB_8 };

            ControlActuator keyboard9 = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - 9", RawOffset = DirectInputRawOffsets.KB_9 };

            ControlActuator keyboardA = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - A", RawOffset = DirectInputRawOffsets.KB_A };
            ControlActuator keyboardZ = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - Z", RawOffset = DirectInputRawOffsets.KB_Z };

            ControlActuator keyboardUpArrow = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - Up Arrow", RawOffset = DirectInputRawOffsets.KB_UP };
            ControlActuator keyboardDownArrow = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - Down Arrow", RawOffset = DirectInputRawOffsets.KB_DOWN };

            ControlActuator keyboardLeftArrow = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - Left Arrow", RawOffset = DirectInputRawOffsets.KB_LEFT };
            ControlActuator keyboardRightArrow = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - Right Arrow", RawOffset = DirectInputRawOffsets.KB_RIGHT };

            ControlActuator keyboardQ = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - Q", RawOffset = DirectInputRawOffsets.KB_Q };
            ControlActuator keyboardW = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - W", RawOffset = DirectInputRawOffsets.KB_W };

            ControlActuator keyboardU = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - U", RawOffset = DirectInputRawOffsets.KB_U };
            ControlActuator keyboardJ = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - J", RawOffset = DirectInputRawOffsets.KB_J };

            ControlActuator keyboardI = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - I", RawOffset = DirectInputRawOffsets.KB_I };
            ControlActuator keyboardK = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - K", RawOffset = DirectInputRawOffsets.KB_K };

            ControlActuator keyboardO = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - O", RawOffset = DirectInputRawOffsets.KB_O };
            ControlActuator keyboardL = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - L", RawOffset = DirectInputRawOffsets.KB_L };

            ControlActuator keyboardY = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - Y", RawOffset = DirectInputRawOffsets.KB_Y };
            ControlActuator keyboardH = new ControlActuator() { DeviceGuid = keyboardGuid, ControlDisplayName = "Keyboard - H", RawOffset = DirectInputRawOffsets.KB_H };

            ControlActuator xboxPadX = new ControlActuator() { DeviceGuid = xboxPadGuid, ControlDisplayName = "Xbox 360 - X", RawOffset = DirectInputRawOffsets.XBOX_LEFTSTICK_X };
            ControlActuator xboxPadY = new ControlActuator() { DeviceGuid = xboxPadGuid, ControlDisplayName = "Xbox 360 - Y", RawOffset = DirectInputRawOffsets.XBOX_LEFTSTICK_Y };
            ControlActuator xboxPadRotX = new ControlActuator() { DeviceGuid = xboxPadGuid, ControlDisplayName = "Xbox 360 - RotationX", RawOffset = DirectInputRawOffsets.XBOX_RIGHTSTICK_X };
            ControlActuator xboxPadRotY = new ControlActuator() { DeviceGuid = xboxPadGuid, ControlDisplayName = "Xbox 360 - RotationY", RawOffset = DirectInputRawOffsets.XBOX_RIGHTSTICK_Y };            

            ControlActuator joystickX = new ControlActuator() { DeviceGuid = joystickGuid, ControlDisplayName = "Logitech Extreme 3D - X", RawOffset = DirectInputRawOffsets.LE3D_X };
            ControlActuator joystickY = new ControlActuator() { DeviceGuid = joystickGuid, ControlDisplayName = "Logitech Extreme 3D - Y", RawOffset = DirectInputRawOffsets.LE3D_Y };
            ControlActuator joystickZ = new ControlActuator() { DeviceGuid = joystickGuid, ControlDisplayName = "Logitech Extreme 3D - Z", RawOffset = DirectInputRawOffsets.LE3D_Z };
            ControlActuator joystickThrottle = new ControlActuator() { DeviceGuid = joystickGuid, ControlDisplayName = "Logitech Extreme 3D - Throttle", RawOffset = DirectInputRawOffsets.LE3D_THROTTLE };
            ControlActuator joystickTrigger = new ControlActuator() { DeviceGuid = joystickGuid, ControlDisplayName = "Logitech Extreme 3D - Trigger", RawOffset = DirectInputRawOffsets.LE3D_TRIGGER };
            
            InputBinding LiftInputBinding = new InputBinding(new ControlActuator[] { xboxPadY, joystickThrottle }, "Lift");
            LiftInputBinding.BindingActuatedEvent += LiftInputBinding_BindingActuatedEvent;
            InputBindings.Add("Lift", LiftInputBinding);

            InputBinding RollInputBinding = new InputBinding(new ControlActuator[] { xboxPadRotX, joystickX }, "RollRate");
            RollInputBinding.BindingActuatedEvent += RollInputBinding_BindingActuatedEvent;
            InputBindings.Add("Roll", RollInputBinding);

            InputBinding PitchInputBinding = new InputBinding(new ControlActuator[] { xboxPadRotY, joystickY }, "PitchRate");
            PitchInputBinding.BindingActuatedEvent += PitchInputBinding_BindingActuatedEvent;
            InputBindings.Add("Pitch", PitchInputBinding);

            InputBinding YawInputBinding = new InputBinding(new ControlActuator[] { xboxPadX, joystickZ }, "YawRate");
            YawInputBinding.BindingActuatedEvent += YawInputBinding_BindingActuatedEvent;
            InputBindings.Add("Yaw", YawInputBinding);

            InputBinding AbortInputBinding = new InputBinding(new ControlActuator[] { keyboardEsc, joystickTrigger }, "Abort");
            AbortInputBinding.BindingActuatedEvent += AbortInputBinding_BindingActuatedEvent;
            InputBindings.Add("Abort", AbortInputBinding);

            InputBinding LiftTrimUpBinding = new InputBinding(new ControlActuator[] { keyboardA }, "LiftTrimUp");
            LiftTrimUpBinding.BindingActuatedEvent += LiftTrimUpBinding_BindingActuatedEvent;
            InputBindings.Add("LiftTrimUp", LiftTrimUpBinding);

            InputBinding LiftTrimDownBinding = new InputBinding(new ControlActuator[] { keyboardZ }, "LiftTrimDown");
            LiftTrimDownBinding.BindingActuatedEvent += LiftTrimDownBinding_BindingActuatedEvent;
            InputBindings.Add("LiftTrimDown", LiftTrimDownBinding);

            InputBinding RollTrimUpBinding = new InputBinding(new ControlActuator[] { keyboardLeftArrow }, "RollTrimUp");
            RollTrimUpBinding.BindingActuatedEvent += RollTrimUpBinding_BindingActuatedEvent;
            InputBindings.Add("RollTrimUp", RollTrimUpBinding);

            InputBinding RollTrimDownBinding = new InputBinding(new ControlActuator[] { keyboardRightArrow }, "RollTrimDown");
            RollTrimDownBinding.BindingActuatedEvent += RollTrimDownBinding_BindingActuatedEvent;
            InputBindings.Add("RollTrimDown", RollTrimDownBinding);

            InputBinding PitchTrimUpBinding = new InputBinding(new ControlActuator[] { keyboardDownArrow }, "PitchTrimUp");
            PitchTrimUpBinding.BindingActuatedEvent += PitchTrimUpBinding_BindingActuatedEvent;
            InputBindings.Add("PitchTrimUp", PitchTrimUpBinding);

            InputBinding PitchTrimDownBinding = new InputBinding(new ControlActuator[] { keyboardUpArrow }, "PitchTrimDown");
            PitchTrimDownBinding.BindingActuatedEvent += PitchTrimDownBinding_BindingActuatedEvent;
            InputBindings.Add("PitchTrimDown", PitchTrimDownBinding);

            InputBinding YawTrimUpBinding = new InputBinding(new ControlActuator[] { keyboardW }, "YawTrimUp");
            YawTrimUpBinding.BindingActuatedEvent += YawTrimUpBinding_BindingActuatedEvent;
            InputBindings.Add("YawTrimUp", YawTrimUpBinding);

            InputBinding YawTrimDownBinding = new InputBinding(new ControlActuator[] { keyboardQ }, "YawTrimDown");
            YawTrimDownBinding.BindingActuatedEvent += YawTrimDownBinding_BindingActuatedEvent;
            InputBindings.Add("YawTrimDown", YawTrimDownBinding);

            InputBinding PYawUpBinding = new InputBinding(new ControlActuator[] { keyboardU }, "PYawUp");
            PYawUpBinding.BindingActuatedEvent += PYawUpBinding_BindingActuatedEvent;
            InputBindings.Add("PYawUp", PYawUpBinding);

            InputBinding PYawDownBinding = new InputBinding(new ControlActuator[] { keyboardJ }, "PYawDown");
            PYawDownBinding.BindingActuatedEvent += PYawDownBinding_BindingActuatedEvent;
            InputBindings.Add("PYawDown", PYawDownBinding);

            InputBinding P1RollPitchUpBinding = new InputBinding(new ControlActuator[] { keyboardI }, "P1RollPitchUp");
            P1RollPitchUpBinding.BindingActuatedEvent += P1RollPitchUpBinding_BindingActuatedEvent;
            InputBindings.Add("P1RollPitchUp", P1RollPitchUpBinding);

            InputBinding P1RollPitchDownBinding = new InputBinding(new ControlActuator[] { keyboardK }, "P1RollPitchDown");
            P1RollPitchDownBinding.BindingActuatedEvent += P1RollPitchDownBinding_BindingActuatedEvent;
            InputBindings.Add("P1RollPitchDown", P1RollPitchDownBinding);

            InputBinding P2RollPitchUpBinding = new InputBinding(new ControlActuator[] { keyboardO }, "P2RollPitchUp");
            P2RollPitchUpBinding.BindingActuatedEvent += P2RollPitchUpBinding_BindingActuatedEvent;
            InputBindings.Add("P2RollPitchUp", P2RollPitchUpBinding);

            InputBinding P2RollPitchDownBinding = new InputBinding(new ControlActuator[] { keyboardL }, "P2RollPitchDown");
            P2RollPitchDownBinding.BindingActuatedEvent += P2RollPitchDownBinding_BindingActuatedEvent;
            InputBindings.Add("P2RollPitchDown", P2RollPitchDownBinding);

            InputBinding PLiftUpBinding = new InputBinding(new ControlActuator[] { keyboardY }, "PLiftUp");
            PLiftUpBinding.BindingActuatedEvent += PLiftUpBinding_BindingActuatedEvent;
            InputBindings.Add("PLiftUp", PLiftUpBinding);

            InputBinding PLiftDownBinding = new InputBinding(new ControlActuator[] { keyboardH }, "PLiftDown");
            PLiftDownBinding.BindingActuatedEvent += PLiftDownBinding_BindingActuatedEvent;
            InputBindings.Add("PLiftDown", PLiftDownBinding);

            InputBinding ModeSafeBinding = new InputBinding(new ControlActuator[] { keyboard0 }, "ModeSafe");
            ModeSafeBinding.BindingActuatedEvent += ModeSafeBinding_BindingActuatedEvent;
            InputBindings.Add("ModeSafe", ModeSafeBinding);

            InputBinding ModePanicBinding = new InputBinding(new ControlActuator[] { keyboard1 }, "ModePanic");
            ModePanicBinding.BindingActuatedEvent += ModePanicBinding_BindingActuatedEvent;
            InputBindings.Add("ModePanic", ModePanicBinding);

            InputBinding ModeManualBinding = new InputBinding(new ControlActuator[] { keyboard2 }, "ModeManual");
            ModeManualBinding.BindingActuatedEvent += ModeManualBinding_BindingActuatedEvent;
            InputBindings.Add("ModeManual", ModeManualBinding);

            InputBinding ModeCalibrationBinding = new InputBinding(new ControlActuator[] { keyboard3 }, "ModeCalibration");
            ModeCalibrationBinding.BindingActuatedEvent += ModeCalibrationBinding_BindingActuatedEvent;
            InputBindings.Add("ModeCalibration", ModeCalibrationBinding);

            InputBinding ModeYawControlBinding = new InputBinding(new ControlActuator[] { keyboard4 }, "ModeYawControl");
            ModeYawControlBinding.BindingActuatedEvent += ModeYawControlBinding_BindingActuatedEvent;
            InputBindings.Add("ModeYawControl", ModeYawControlBinding);

            InputBinding ModeFullControlBinding = new InputBinding(new ControlActuator[] { keyboard5 }, "ModeFullControl");
            ModeFullControlBinding.BindingActuatedEvent += ModeFullControlBinding_BindingActuatedEvent;
            InputBindings.Add("ModeFullControl", ModeFullControlBinding);

            InputBinding RawSwitchBinding = new InputBinding(new ControlActuator[] { keyboard6 }, "RawSwitch");
            RawSwitchBinding.BindingActuatedEvent += RawSwitchBinding_BindingActuatedEvent;
            InputBindings.Add("RawSwitch", RawSwitchBinding);

            InputBinding ModeHeightBinding = new InputBinding(new ControlActuator[] { keyboard7 }, "ModeHeight");
            ModeHeightBinding.BindingActuatedEvent += ModeHeightBinding_BindingActuatedEvent;
            InputBindings.Add("ModeHeight", ModeHeightBinding);

            InputBinding WirelessSwitchBinding = new InputBinding(new ControlActuator[] { keyboard8 }, "WirelessSwitch");
            WirelessSwitchBinding.BindingActuatedEvent += WirelessSwitchBinding_BindingActuatedEvent;
            InputBindings.Add("WirelessSwitch", WirelessSwitchBinding);

            InputBinding ModeFlashDumpBinding = new InputBinding(new ControlActuator[] { keyboard7 }, "ModeFlashDump");
            ModeFlashDumpBinding.BindingActuatedEvent += ModeFlashDumpBinding_BindingActuatedEvent;
            InputBindings.Add("ModeFlashDump", ModeFlashDumpBinding);

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

        private void PYawUpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustPYaw(true);
        }

        private void PYawDownBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustPYaw(false);
        }

        private void P1RollPitchUpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustP1RollPitch(true);
        }

        private void P1RollPitchDownBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustP1RollPitch(false);
        }

        private void P2RollPitchUpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustP2RollPitch(true);
        }

        private void P2RollPitchDownBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustP2RollPitch(false);
        }

        private void PLiftUpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustPLift(true);
        }

        private void PLiftDownBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.AdjustPLift(false);
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

        private void RawSwitchBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.RawSwitch();
        }

        private void ModeHeightBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.ModeSwitch(FlightMode.Height);
        }

        private void WirelessSwitchBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.WirelessSwitch();
        }

        private void ModeFlashDumpBinding_BindingActuatedEvent(object sender, BindingActuatedEventArgs e)
        {
            if (e.InnerEvent.StateUpdate.Value > KeyThreshold)
                GlobalData.ctr.StartFlashDump();
        }
        
    }
}
