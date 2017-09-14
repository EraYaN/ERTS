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


        public FlightMode Mode
        {
            get { return GlobalData.ctr.Mode; }
            set
            {
                GlobalData.ctr.Mode = value;
                RaisePropertyChanged("ModeString");
                RaisePropertyChanged("ModeDescriptionString");
            }
        }

        public int Voltage
        {
            get { if (GlobalData.ctr != null)
                    return GlobalData.ctr.Voltage;
                else
                    return -1;
            }
            set
            {
                if (GlobalData.ctr != null)
                {
                    GlobalData.ctr.Voltage = value;
                    RaisePropertyChanged("VoltageString");
                }
            }
        }

        public string ModeString { get { return ((int)Mode).ToString(); } }
        public string ModeDescriptionString { get { return Mode.GetDescription(); } }
        public string VoltageString { get { return Voltage.ToString() + " mV"; } }

        public string LiftString { get { return GlobalData.ctr.Lift.ToString("N2"); } }

        public string RollString { get { return GlobalData.ctr.RollRate.ToString("N2") + " 1/s"; } }
        public string PitchString { get { return GlobalData.ctr.PitchRate.ToString("N2") + " 1/s"; } }

        public string YawString { get { return GlobalData.ctr.YawRate.ToString("N2") + " 1/s"; } }

        public double Lift { get { return GlobalData.ctr.Lift * 98; } }
        public double Roll { get { return GlobalData.ctr.RollRate * 40 + 40; } }
        public double Pitch { get { return GlobalData.ctr.PitchRate * 35 + 35; } }
        public double Yaw { get { return GlobalData.ctr.YawRate * 180; } }

        

        public MainViewModel()
        {
            Mode = FlightMode.None;
            Voltage = 15000;
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
            }
            if (e.PropertyName == "RollRate")
            {
                RaisePropertyChanged("RollString");
                RaisePropertyChanged("Roll");
            }
            if (e.PropertyName == "PitchRate")
            {
                RaisePropertyChanged("PitchString");
                RaisePropertyChanged("Pitch");
            }
            if (e.PropertyName == "YawRate")
            {
                RaisePropertyChanged("YawString");
                RaisePropertyChanged("Yaw");
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
