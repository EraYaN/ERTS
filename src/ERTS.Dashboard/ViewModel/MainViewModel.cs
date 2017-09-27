using ERTS.Dashboard.Communication.Enumerations;
using ERTS.Dashboard.Utility;
using MicroMvvm;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

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
                    return GlobalData.ctr.BatteryVoltage;
                else
                    return -1;
            }
        }

        public string ModeString { get { return ((int)Mode).ToString("X"); } }
        public string ModeDescriptionString { get { return Mode.GetDescription(); } }
        public string VoltageString { get { return Voltage.ToString() + " V"; } }

        public string LiftString {
            get {
                if (GlobalData.ctr != null)
                    return String.Format("{0:N2} ({1:+0.00;-0.00;+0})", GlobalData.ctr.Lift, GlobalData.ctr.LiftTrim);
                else
                    return "-";
            }
        }

        public string RollString {
            get {
                if (GlobalData.ctr != null)
                    return String.Format("{0:N2} ({1:+0.00;-0.00;+0}) 1/s", GlobalData.ctr.RollRate, GlobalData.ctr.RollTrim);
                else
                    return "-";
            }
        }
        public string PitchString {
            get {
                if (GlobalData.ctr != null)
                    return String.Format("{0:N2} ({1:+0.00;-0.00;+0}) 1/s", GlobalData.ctr.PitchRate, GlobalData.ctr.PitchTrim);
                else
                    return "-";
            }
        }

        public string YawString {
            get {
                if (GlobalData.ctr != null)
                    return String.Format("{0:N2} ({1:+0.00;-0.00;+0}) 1/s", GlobalData.ctr.YawRate, GlobalData.ctr.YawTrim);
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

        public double LiftTrimSize {
            get {
                if (GlobalData.ctr != null)
                    return Math.Min(0,Math.Abs(GlobalData.ctr.LiftTrim) * 98);
                else
                    return 0;
            }
        }
        public double LiftTrimOffset {
            get {
                if (GlobalData.ctr != null)
                {
                    if (GlobalData.ctr.LiftTrim > 0)
                    {
                        return (GlobalData.ctr.Lift) * 98 + 1;
                    }
                    else
                    {
                        return Math.Min(0, Math.Abs(GlobalData.ctr.Lift-GlobalData.ctr.LiftTrim) * 98) + 1;
                    }
                }
                else
                    return 0;
            }
        }
        public double RollTrim {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.RollTrim * 40 + 40;
                else
                    return 40;
            }
        }
        public double PitchTrim {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.PitchTrim * 35 + 35;
                else
                    return 35;
            }
        }
        public double YawTrim {
            get {
                if (GlobalData.ctr != null)
                    return GlobalData.ctr.YawTrim * 180;
                else
                    return 0;
            }
        }

        public string WindowTitle {
            get {
                Assembly currAss = Assembly.GetExecutingAssembly();
                string fileVersion = FileVersionInfo.GetVersionInfo(currAss.Location).FileVersion;
                string processorArchitecture = currAss.GetName().ProcessorArchitecture.ToString();
#if DEBUG
                string Branch = "Debug";
#else
                string Branch = "Release";
#endif
                return String.Format("{3} v{0} {1} {2} by Erwin de Haan, Robin Hes & Casper van Wezel", fileVersion, processorArchitecture, Branch, currAss.FullName);
            }
        }

        public string DebugInfo {
            get {
                return VersionInfo;
            }
        }

        public string VersionInfo {
            get {
                StringBuilder sb = new StringBuilder();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly a in assemblies)
                {
                    AssemblyName an = a.GetName();
                    if (a.GlobalAssemblyCache == false)
                        sb.AppendLine(String.Format("{0} v{1} {2}", an.Name, an.Version, an.ProcessorArchitecture));
                }

                return sb.ToString();
            }
        }
        public string SerialPortStatus {
            get {
                if (GlobalData.com == null)
                    return "NULL";
                if (GlobalData.com.IsOpen)
                {
                    return GlobalData.com.BytesInRBuffer + "|" + GlobalData.com.BytesInTBuffer;
                }
                else
                {
                    return "NC";
                }
            }
        }

        public Brush SerialPortStatusColor {
            get {
                if (GlobalData.com == null)
                    return Brushes.Red;
                if (GlobalData.com.IsOpen)
                {
                    int b = GlobalData.com.BytesInRBuffer + GlobalData.com.BytesInTBuffer;
                    if (b == 0)
                    {
                        return Brushes.Green;
                    }
                    else if (b > 0 && b <= 2)
                    {
                        return Brushes.LightGreen;
                    }
                    else
                    {
                        return Brushes.Orange;
                    }
                }
                else
                {
                    return Brushes.OrangeRed;
                }
            }
        }

        public string LoopTimeString {
            get {
                if (GlobalData.ctr != null)
                {
                    if (GlobalData.ctr.LoopTime == -1)
                        return "xx ms";
                    else
                        return string.Format("{0:f1} ms", GlobalData.ctr.LoopTime);
                }
                else
                {
                    return "ctr is null";
                }
            }
        }

        public string InputStatus {
            get {
                if (GlobalData.input != null)
                {
                    if (GlobalData.input.IsInputEngaged)
                        return "Engaged";
                    else
                        return "Disengaged";
                }
                else
                {
                    return "input is null";
                }
            }
        }
        public string InputDevicesStatus {
            get {
                if (GlobalData.input != null)
                {
                    return string.Format("{0} devices bound", GlobalData.input.BoundDevices);
                }
                else
                {
                    return "input is null";
                }
            }
        }
        public MainViewModel()
        {
            
        }

        public void InitStageOne()
        {
            if (GlobalData.input != null)
                Devices = GlobalData.input.EnumerateControllers();
            else
                Devices = new List<DeviceInstance>();
            SelectedDevice = null;

            if (GlobalData.input != null)
                GlobalData.input.PropertyChanged += Input_PropertyChanged;
        }

        public void InitStageTwo()
        {
            if (GlobalData.ctr != null)
                GlobalData.ctr.PropertyChanged += Ctr_PropertyChanged;
            if (GlobalData.com != null)
                GlobalData.com.PropertyChanged += Com_PropertyChanged;
        }

        private void Input_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsInputEngaged")
            {
                RaisePropertyChanged("InputStatus");
                return;
            }
            else if (e.PropertyName == "BoundDevices")
            {
                RaisePropertyChanged("InputDevicesStatus");
                return;
            }
            else
            {
                Debug.WriteLine("Got unsupported binding name from InputManager " + e.PropertyName + ".");
            }
        }

        private void Com_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsOpen" || e.PropertyName == "BytesInRBuffer" || e.PropertyName == "BytesInTBuffer")
            {
                RaisePropertyChanged("SerialPortStatus");
                RaisePropertyChanged("SerialPortStatusColor");
                return;
            }
            else
            {
                Debug.WriteLine("Got unsupported binding name from InputManager " + e.PropertyName + ".");
            }
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
            else if (e.PropertyName == "BatteryVoltage")
            {
                RaisePropertyChanged("VoltageString");
                return;
            }
            else if (e.PropertyName == "LoopTime")
            {
                RaisePropertyChanged("LoopTimeString");
            }
            else if (e.PropertyName == "LiftTrim")
            {
                RaisePropertyChanged("LiftTrimSize");
                RaisePropertyChanged("LiftTrimOffset");
                RaisePropertyChanged("LiftString");
            }
            else if (e.PropertyName == "RollTrim")
            {
                RaisePropertyChanged("RollTrim");
                RaisePropertyChanged("RollString");
            }
            else if (e.PropertyName == "PitchTrim")
            {
                RaisePropertyChanged("PitchTrim");
                RaisePropertyChanged("PitchString");
            }
            else if (e.PropertyName == "YawTrim")
            {
                RaisePropertyChanged("YawTrim");
                RaisePropertyChanged("YawString");
            }
            else
            {
                Debug.WriteLine("Got unsupported binding name from Controller " + e.PropertyName + ".");
            }

        }

        void BindInputExecute(object obj)
        {
            if (SelectedDevice != null)
            {
                GlobalData.input.AquireDevice((DeviceInstance)SelectedDevice, new WindowInteropHelper(obj as Window).Handle);
            }
        }

        bool CanBindInputExecute(object obj)
        {
            return SelectedDevice != null && !GlobalData.input.IsDeviceInUse(SelectedDevice);
        }

        public ICommand BindInput { get { return new RelayCommand<MainWindow>(BindInputExecute, CanBindInputExecute); } }

    }
}
