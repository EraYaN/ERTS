using SharpDX;
using SharpDX.DirectInput;
using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Input
{
    public class InputManager : ObservableObject, IDisposable
    {
        public event EventHandler<InputEventArgs> InputEvent;

        DirectInput directInput;
        object usedDeviceLock = new object();
        List<Device> UsedDevices = new List<Device>();
        List<WaitHandle> WaitHandles = new List<WaitHandle>();

        bool isInputEngaged = true;

        public bool IsInputEngaged {
            get { return isInputEngaged; }
        }

        public int BoundDevices {
            get { return UsedDevices.Count; }
        }

        Task threadTask;

        CancellationTokenSource cancelTokenSource;

        public InputManager()
        {
            directInput = new DirectInput();
            StartThread();
            RaisePropertyChanged("IsInputEngaged");
            RaisePropertyChanged("BoundDevices");
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
        public bool IsDeviceInUse(Guid guid)
        {
            if (UsedDevices != null)
                return UsedDevices.Exists(d => d.Information.InstanceGuid == guid);
            else
                return false;
        }

        public void DisengageInput()
        {
            Debug.WriteLine("Disengaged Input.");
            isInputEngaged = false;
            RaisePropertyChanged("IsInputEngaged");
        }

        public void EngageInput()
        {
            Debug.WriteLine("Engaged Input.");
            isInputEngaged = true;
            RaisePropertyChanged("IsInputEngaged");
        }

        //TODO make WindowHandle a class member
        public bool AquireAllDevices(IntPtr WindowHandle)
        {
            List<DeviceInstance> devices = EnumerateControllers();
            return AquireDevices(devices, WindowHandle);
        }

        public bool AquireDevices(IEnumerable<Guid> deviceGuids, IntPtr WindowHandle)
        {
            List<DeviceInstance> devices = EnumerateControllers();
            bool result = true;
            foreach (Guid guid in deviceGuids)
            {
                DeviceInstance di = devices.Find(d => d.InstanceGuid == guid);
                if (di != null)
                {
                    if (!IsDeviceInUse(di))
                    {
                        result &= AquireDevice(di, WindowHandle);
                    }
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        public bool AquireDevices(IEnumerable<DeviceInstance> devices, IntPtr WindowHandle)
        {
            bool result = true;
            foreach (DeviceInstance di in devices)
            {
                if (!IsDeviceInUse(di))
                {
                    result &= AquireDevice(di, WindowHandle);
                }
            }
            return result;
        }

        public void UnaquireDevicesNotInList(List<DeviceInstance> devices)
        {
            List<Device> removedDevices = new List<Device>();
            lock (usedDeviceLock)
            {
                foreach (Device dev in UsedDevices)
                {
                    if (!devices.Exists(d => d.InstanceGuid == dev.Information.InstanceGuid))
                    {
                        dev.Unacquire();
                        removedDevices.Add(dev);
                    }
                }
                foreach (Device dev in removedDevices)
                {
                    UsedDevices.Remove(dev);
                }
            }
            RaisePropertyChanged("BoundDevices");
        }
        public void UnaquireAllDevices()
        {            
            foreach (Device dev in UsedDevices)
            {
                dev.Unacquire();
            }
            UsedDevices.Clear();
            RaisePropertyChanged("BoundDevices");
        }

        public bool AquireDevice(DeviceInstance device, IntPtr WindowHandle)
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
            RaisePropertyChanged("BoundDevices");
            return true;
        }

        public void StopThread()
        {
            if (cancelTokenSource != null)
                cancelTokenSource.Cancel(false);
            threadTask.Wait();
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
                    try
                    {
                        foreach (Device d in UsedDevices)
                        {
                            if (!d.IsDisposed)
                            {
                                d.Poll();
                                if (d.Information.Type == DeviceType.Keyboard)
                                {
                                    var updates = ((Keyboard)d).GetBufferedData();
                                    foreach (KeyboardUpdate state in updates)
                                    {

                                        //Debug.Write(state);
                                        //Debug.WriteLine(String.Format("; Raw: {0}; Key: {1}", state.RawOffset, state.Key));
                                        SendInputEvent(state, d.Information.InstanceGuid);
                                    }
                                }
                                else if (d.Information.Type == DeviceType.Mouse)
                                {
                                    var updates = ((Mouse)d).GetBufferedData();
                                    foreach (MouseUpdate state in updates)
                                    {
                                        //Debug.Write(state);
                                        //Debug.WriteLine(String.Format("; Raw: {0}; IsButtom: {1}", state.RawOffset, state.IsButton));
                                        SendInputEvent(state, d.Information.InstanceGuid);
                                    }
                                }
                                else
                                {
                                    var updates = ((Joystick)d).GetBufferedData();
                                    foreach (JoystickUpdate state in updates)
                                    {
                                        //Debug.Write(state);
                                        //Debug.WriteLine(String.Format("; Raw: {0}; Key: {1}", state.RawOffset, state.Value));
                                        SendInputEvent(state, d.Information.InstanceGuid);
                                    }
                                }

                            }

                        }
                    }
                    catch (SharpDXException e)
                    {
                        //shutdown artifact.
                    }
                    catch (NullReferenceException e)
                    {
                        //shutdown artifact.
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
            OnInput(new InputEventArgs(StateUpdate, DeviceGuid, isInputEngaged));
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
