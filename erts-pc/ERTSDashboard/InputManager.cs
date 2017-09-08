using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ERTS.Dashboard
{
    class InputManager
    {
        DirectInput directInput;
        object usedDeviceLock = new object();
        List<Device> UsedDevices = new List<Device>();

        //Set for mode 1 RC control
        JoystickOffset Pitch = JoystickOffset.RotationY; //XB360 Right Stick Vertical
        JoystickOffset Yaw = JoystickOffset.X; //XB360 Left Stick Horizontal
        JoystickOffset Roll = JoystickOffset.RotationX; //XB360 Right Stick Horizontal
        JoystickOffset Lift = JoystickOffset.Y; //XB360 Left Stick Vertical

        CancellationTokenSource cancelTokenSource;

        public InputManager()
        {
            directInput = new DirectInput();            
        }

        public List<DeviceInstance> EnumerateControllers()
        {
            List<DeviceInstance> devices = new List<DeviceInstance>();

            devices.AddRange(directInput.GetDevices(DeviceType.Keyboard, DeviceEnumerationFlags.AttachedOnly));
            devices.AddRange(directInput.GetDevices(DeviceType.Mouse, DeviceEnumerationFlags.AttachedOnly));
            devices.AddRange(directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly));
            devices.AddRange(directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly));
            //TODO Add setting to allow all types.
            //devices.AddRange(directInput.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AttachedOnly));

            foreach (DeviceInstance deviceInstance in devices)
            {
                Debug.WriteLine(deviceInstance.Type.ToString() + ": " + deviceInstance.InstanceName + " " + deviceInstance.InstanceGuid, "ControllerList");
            }
            return devices;
        }


        public bool BindDevice(DeviceInstance device, IntPtr WindowHandle)
        {
            if (UsedDevices.Exists(d => d.Information.InstanceGuid == device.InstanceGuid)){
                Debug.WriteLine("Already bound {2}: {0} ({1})", device.InstanceName, device.InstanceGuid, device.Type);
                return false;
            }
            
            Device boundDevice;
            switch (device.Type) {
                case DeviceType.Keyboard:
                    boundDevice = new Keyboard(directInput);
                    break;
                case DeviceType.Mouse:
                    boundDevice = new Mouse(directInput);
                    break;
                default:
                    boundDevice = new Joystick(directInput, device.InstanceGuid);
                    break;
            }

            Debug.WriteLine("Bound {2}: {0} ({1})", boundDevice.Information.InstanceName, boundDevice.Information.InstanceGuid, boundDevice.Information.Type);
            // Query all suported ForceFeedback effects
            var allEffects = boundDevice.GetEffects();
            foreach (var effectInfo in allEffects)
                Debug.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            boundDevice.Properties.BufferSize = 128;

            // Acquire the device

            boundDevice.SetCooperativeLevel(WindowHandle, CooperativeLevel.NonExclusive | CooperativeLevel.Background);
            boundDevice.Acquire();

            lock (usedDeviceLock)
            {
                UsedDevices.Add(boundDevice);
            }

            return true;
        }

        public void StopThread()
        {
            if (cancelTokenSource !=null)
                cancelTokenSource.Cancel(false);
        }

        public void StartThread()
        {
            cancelTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => ThreadLoop(cancelTokenSource.Token));
        }

        private void ThreadLoop(CancellationToken cancelToken)
        {
            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    lock (usedDeviceLock) {
                        foreach (Device d in UsedDevices)
                        {
                            d.Unacquire();
                        }
                    }
                    return;
                }

                lock (usedDeviceLock)
                {
                    foreach (Device d in UsedDevices)
                    {                        
                        if (!d.IsDisposed)
                        {
                            d.Poll();
                            if (d.Information.Type == DeviceType.Keyboard)
                            {
                                var updates = ((Keyboard)d).GetBufferedData();
                                foreach (var state in updates)
                                {                                    
                                    Debug.Write(state);
                                    Debug.WriteLine(String.Format("; Raw: {0}; Key: {1}", state.RawOffset, state.Key));
                                }
                            } else if (d.Information.Type == DeviceType.Mouse)
                            {
                                var updates = ((Mouse)d).GetBufferedData();
                                foreach (var state in updates)
                                {
                                    Debug.Write(state);
                                    Debug.WriteLine(String.Format("; Raw: {0}; IsButtom: {1}", state.RawOffset, state.IsButton));
                                }
                            } else
                            {
                                var updates = ((Joystick)d).GetBufferedData();
                                foreach (var state in updates)
                                {
                                    Debug.Write(state);
                                    Debug.WriteLine(String.Format("; Raw: {0}; Key: {1}", state.RawOffset, state.Value));
                                }
                            }

                        }
                    }
                } 
            
            }
        }
    }
}
