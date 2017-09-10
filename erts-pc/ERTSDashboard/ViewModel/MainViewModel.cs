using ERTS.Dashboard.Utility;
using MicroMvvm;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ERTS.Dashboard.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private Quadrupel _quad = new Quadrupel();

        private List<DeviceInstance> _devices;

        public List<DeviceInstance> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                RaisePropertyChanged("Devices");
            }
        }

        private DeviceInstance _selectedDevice;

        public DeviceInstance SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                RaisePropertyChanged("SelectedDevice");
            }
        }


        public Modes Mode
        {
            get { return _quad.Mode; }
            set
            {
                _quad.Mode = value;
                RaisePropertyChanged("ModeString");
                RaisePropertyChanged("ModeDescriptionString");
            }
        }

        public int Voltage
        {
            get { return _quad.Voltage; }
            set
            {
                _quad.Voltage = value;
                RaisePropertyChanged("VoltageString");
            }
        }

        public string ModeString { get { return ((int)Mode).ToString(); } }
        public string ModeDescriptionString { get { return Mode.GetDescription(); } }
        public string VoltageString { get { return Voltage.ToString() + " mV"; } }

        //public string RollString { get { return _input.Roll.ToString() + "°"; } }
        //public string PitchString { get { return _input.Pitch.ToString() + "°"; } }
        //public string YawString { get { return _input.Yaw.ToString() + "°"; } }
        //public string LiftString { get { return _input.Lift.ToString(); } }

        public MainViewModel()
        {
            Mode = Modes.Wireless;
            Voltage = 15000;
            Devices = GlobalData.input.EnumerateControllers();
            SelectedDevice = null;
        }

        void BindInputExecute(object obj)
        {
            if (SelectedDevice != null)
            {
                GlobalData.input.BindDevice((DeviceInstance)SelectedDevice, new WindowInteropHelper(obj as Window).Handle);
            }
        }

        bool CanBindInputExecute(object obj)
        {
            return SelectedDevice != null && !GlobalData.input.IsDeviceInUse(SelectedDevice);
        }

        public ICommand BindInput { get { return new RelayCommand<MainWindow>(BindInputExecute, CanBindInputExecute); } }

        public void OnWindowClosing (object sender, EventArgs e)
        {
            if (GlobalData.input != null) GlobalData.input.StopThread();
        }
    }
}
