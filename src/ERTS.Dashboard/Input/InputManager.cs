using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Input
{
    public class InputManager : IDisposable
    {
        public event EventHandler<InputEventArgs> InputEvent;

        DirectInput directInput;
        object usedDeviceLock = new object();
        List<Device> UsedDevices = new List<Device>();
        List<WaitHandle> WaitHandles = new List<WaitHandle>();

        bool IsInputEngaged = true;

        Task threadTask;

        //Set for mode 1 RC control
        JoystickOffset Pitch = JoystickOffset.RotationY; //XB360 Right Stick Vertical
        JoystickOffset Yaw = JoystickOffset.X; //XB360 Left Stick Horizontal
        JoystickOffset Roll = JoystickOffset.RotationX; //XB360 Right Stick Horizontal
        JoystickOffset Lift = JoystickOffset.Y; //XB360 Left Stick Vertical

        CancellationTokenSource cancelTokenSource;

        public InputManager()
        {
            directInput = new DirectInput();
            StartThread();
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

        public bool IsDeviceInUse(DeviceInstance device)
        {
            if (UsedDevices != null)
                return UsedDevices.Exists(d => d.Information.InstanceGuid == device.InstanceGuid);
            else
                return false;
        }

        public void DisengageInput()
        {
            Debug.WriteLine("Disengaged Input.");
            IsInputEngaged = false;
        }

        public void EngageInput()
        {
            Debug.WriteLine("Engaged Input.");
            IsInputEngaged = true;
        }

        public bool BindDevice(DeviceInstance device, IntPtr WindowHandle)
        {
            if (IsDeviceInUse(device))
            {
                Debug.WriteLine("Already bound {2}: {0} ({1})", device.InstanceName, device.InstanceGuid, device.Type);
                return false;
            }

            Device boundDevice;
            switch (device.Type)
            {
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
            WaitHandle handle = new AutoResetEvent(false);
            boundDevice.SetNotification(handle);
            boundDevice.Acquire();

            lock (usedDeviceLock)
            {
                WaitHandles.Add(handle);
                UsedDevices.Add(boundDevice);
            }

            return true;
        }

        public void StopThread()
        {
            if (cancelTokenSource != null)
                cancelTokenSource.Cancel(false);
        }

        public void StartThread()
        {
            cancelTokenSource = new CancellationTokenSource();
            threadTask = Task.Factory.StartNew(() => ThreadLoop(cancelTokenSource.Token));
        }

        private void ThreadLoop(CancellationToken cancelToken)
        {
            while (true)
            {
                if (WaitHandles.Count == 0)
                    Thread.Sleep(500);
                if (cancelToken.IsCancellationRequested)
                {
                    lock (usedDeviceLock)
                    {
                        if (UsedDevices != null)
                        {
                            foreach (Device d in UsedDevices)
                            {
                                d.Unacquire();
                            }
                        }
                    }
                    return;
                }

                lock (usedDeviceLock)
                {
                    if (WaitHandles.Count > 0)
                    {
                        WaitHandle.WaitAny(WaitHandles.ToArray(), 1000);
                    }
                    foreach (Device d in UsedDevices)
                    {
                        try
                        {
                            if (!d.IsDisposed)
                            {
                                d.Poll();
                                if (d.Information.Type == DeviceType.Keyboard)
                                {
                                    var updates = ((Keyboard)d).GetBufferedData();
                                    foreach (KeyboardUpdate state in updates)
                                    {
                                       
                                        Debug.Write(state);
                                        Debug.WriteLine(String.Format("; Raw: {0}; Key: {1}", state.RawOffset, state.Key));
                                        SendInputEvent(state, d.Information.InstanceGuid);
                                    }
                                }
                                else if (d.Information.Type == DeviceType.Mouse)
                                {
                                    var updates = ((Mouse)d).GetBufferedData();
                                    foreach (MouseUpdate state in updates)
                                    {
                                        Debug.Write(state);
                                        Debug.WriteLine(String.Format("; Raw: {0}; IsButtom: {1}", state.RawOffset, state.IsButton));
                                        SendInputEvent(state, d.Information.InstanceGuid);
                                    }
                                }
                                else
                                {
                                    var updates = ((Joystick)d).GetBufferedData();
                                    foreach (JoystickUpdate state in updates)
                                    {
                                        Debug.Write(state);
                                        Debug.WriteLine(String.Format("; Raw: {0}; Key: {1}", state.RawOffset, state.Value));
                                        SendInputEvent(state, d.Information.InstanceGuid);
                                    }
                                }

                            }
                        } catch(SharpDXException e)
                        {
                            //shutdown artifact.
                        }
                    }
                }

            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    StopThread();
                    if (threadTask != null)
                        threadTask.Wait();
                    directInput.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                UsedDevices = null;
                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~InputManager()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Event Source
        void SendInputEvent(IStateUpdate StateUpdate, Guid DeviceGuid)
        {
            OnInput(new InputEventArgs(StateUpdate, DeviceGuid, IsInputEngaged));
        }

        protected virtual void OnInput(InputEventArgs e)
        {
            if (InputEvent != null)
            {
                InputEvent(this, e);
            }
        }
        #endregion
    }
}
