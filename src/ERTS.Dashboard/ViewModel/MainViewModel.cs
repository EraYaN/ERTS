using ERTS.Dashboard.Communication.Enumerations;
using ERTS.Dashboard.Utility;
using MicroMvvm;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<DeviceInstance> _devices;

        public List<DeviceInstance> Devices {
            get { return _devices; }
            set {
                _devices = value;
                RaisePropertyChanged("Devices");
            }
        }

        private DeviceInstance _selectedDevice;

        public DeviceInstance SelectedDevice {
            get { return _selectedDevice; }
            set {
                _selectedDevice = value;
                RaisePropertyChanged("SelectedDevice");
            }
        }


        public FlightMode Mode {
            get {
                if (GlobalData.ctr != null)
                {
                    return GlobalData.ctr.Mode;
                }
                else
                {
                    return FlightMode.None;
                }
            }
        }

        public double Voltage {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.Voltage;
                else
                    return -1;
            }
        }

        public string ModeString { get { return ((int)Mode).ToString("X"); } }
        public string ModeDescriptionString { get { return Mode.GetDescription(); } }
        public string VoltageString { get { return Voltage.ToString() + " mV"; } }

        public string LiftString {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.Lift.ToString("N2");
                else
                    return "-";
            }
        }

        public string RollString {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.RollRate.ToString("N2") + " 1/s";
                else
                    return "-";
            }
        }
        public string PitchString {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.PitchRate.ToString("N2") + " 1/s";
                else
                    return "-";
            }
        }

        public string YawString {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.YawRate.ToString("N2") + " 1/s";
                else
                    return "-";
            }
        }

        public double Lift {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.Lift * 98;
                else
                    return 0;
            }
        }
        public double Roll {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.RollRate * 40 + 40;
                else
                    return 40;
            }
        }
        public double Pitch {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.PitchRate * 35 + 35;
                else
                    return 35;
            }
        }
        public double Yaw {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.YawRate * 180;
                else
                    return 0;
            }
        }



        public MainViewModel()
        {
            

            
        }

        public void Init()
        {
            if (GlobalData.input != null)
                Devices = GlobalData.input.EnumerateControllers();
            else
                Devices = new List<DeviceInstance>();
            SelectedDevice = null;

            if (GlobalData.ctr != null)
                GlobalData.ctr.PropertyChanged += Ctr_PropertyChanged;
        }

        private void Ctr_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Lift")
            {
                RaisePropertyChanged("LiftString");
                RaisePropertyChanged("Lift");
                return;
            }
            else if (e.PropertyName == "RollRate")
            {
                RaisePropertyChanged("RollString");
                RaisePropertyChanged("Roll");
                return;
            }
            else if (e.PropertyName == "PitchRate")
            {
                RaisePropertyChanged("PitchString");
                RaisePropertyChanged("Pitch");
                return;
            }
            else if (e.PropertyName == "YawRate")
            {
                RaisePropertyChanged("YawString");
                RaisePropertyChanged("Yaw");
                return;
            }
            else if (e.PropertyName == "Mode")
            {
                RaisePropertyChanged("ModeString");
                RaisePropertyChanged("ModeDescriptionString");
                return;
            }
            else if (e.PropertyName == "Voltage")
            {
                RaisePropertyChanged("VoltageString");
                return;
            }
            else
            {
                Debug.WriteLine("Got unsupported binding name from Controller "+ e.PropertyName + ".");
            }

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

    }
}
