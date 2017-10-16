using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;

namespace ERTS.Dashboard.Configuration {
    /// <summary>
    /// The ViewModel for the Configuration Dialog
    /// </summary>
    public class ConfigurationViewModel : ObservableObject {
        #region Construction
        /// <summary>
        /// Constructs the default instance of a SongViewModel
        /// </summary>
        public ConfigurationViewModel() {
            // _song = new Song { ArtistName = "None", SongTitle = "None" };
        }
        #endregion

        #region Members
        ObservableCollection<string> _comPorts = new ObservableCollection<string>();
        ObservableCollection<int> _baudRates = new ObservableCollection<int>() { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 256000, 921600 };
        #endregion

        #region Properties
        public ObservableCollection<string> ComPorts {
            get {
                return _comPorts;
            }
            set {
                _comPorts = value;
                RaisePropertyChanged("ComPorts");
            }
        }

        public ObservableCollection<int> BaudRates {
            get {
                return _baudRates;
            }
            set {
                _baudRates = value;
                RaisePropertyChanged("BaudRates");
            }
        }
        public string ComPort {
            get {
                if (GlobalData.cfg != null) {
                    return GlobalData.cfg.ComPort;
                }
                return String.Empty;
            }
            set {
                if (GlobalData.cfg != null) {
                    if (GlobalData.cfg.ComPort != value) {
                        GlobalData.cfg.ComPort = value;
                        RaisePropertyChanged("ComPort");
                    }
                }
            }
        }
        public int BaudRate {
            get {
                if (GlobalData.cfg != null) {
                    return GlobalData.cfg.BaudRate;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null) {
                    if (GlobalData.cfg.BaudRate != value) {
                        GlobalData.cfg.BaudRate = value;
                        RaisePropertyChanged("BaudRate");
                    }
                }
            }
        }
        
        public int BatteryThreshold {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.BatteryThreshold;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.BatteryThreshold != value)
                    {
                        GlobalData.cfg.BatteryThreshold = value;
                        RaisePropertyChanged("BatteryThreshold");
                    }
                }
            }
        }

        public int PanicDecrement {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.PanicDecrement;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.PanicDecrement != value)
                    {
                        GlobalData.cfg.PanicDecrement = value;
                        RaisePropertyChanged("PanicDecrement");
                    }
                }
            }
        }
        
        public int RCInterval {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.RCInterval;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.RCInterval != value)
                    {
                        GlobalData.cfg.RCInterval = value;
                        RaisePropertyChanged("RCInterval");
                    }
                }
            }
        }

        public int TelemetryDivider {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.TelemetryDivider;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.TelemetryDivider != value)
                    {
                        GlobalData.cfg.TelemetryDivider = value;
                        RaisePropertyChanged("TelemetryDivider");
                    }
                }
            }
        }

        public int LogDivider {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.LogDivider;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.LogDivider != value)
                    {
                        GlobalData.cfg.LogDivider = value;
                        RaisePropertyChanged("LogDivider");
                    }
                }
            }
        }

        public int PacketCheckResendInterval {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.PacketCheckResendInterval;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.PacketCheckResendInterval != value)
                    {
                        GlobalData.cfg.PacketCheckResendInterval = value;
                        RaisePropertyChanged("PacketCheckResendInterval");
                    }
                }
            }
        }

        public int PacketResendInterval {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.PacketResendInterval;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.PacketResendInterval != value)
                    {
                        GlobalData.cfg.PacketResendInterval = value;
                        RaisePropertyChanged("PacketResendInterval");
                    }
                }
            }
        }

        public int PacketRetransmissionCount {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.PacketRetransmissionCount;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.PacketRetransmissionCount != value)
                    {
                        GlobalData.cfg.PacketRetransmissionCount = value;
                        RaisePropertyChanged("PacketRetransmissionCount");
                    }
                }
            }
        }

        public bool KillAfterRetransmissionFail {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.KillAfterRetransmissionFail;
                }
                return false;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.KillAfterRetransmissionFail != value)
                    {
                        GlobalData.cfg.KillAfterRetransmissionFail = value;
                        RaisePropertyChanged("KillAfterRetransmissionFail");
                    }
                }
            }
        }

        public double LiftDeadzone {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.LiftDeadzone;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.LiftDeadzone != value)
                    {
                        GlobalData.cfg.LiftDeadzone = value;
                        RaisePropertyChanged("LiftDeadzone");
                    }
                }
            }
        }

        public double RollDeadzone {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.RollDeadzone;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.RollDeadzone != value)
                    {
                        GlobalData.cfg.RollDeadzone = value;
                        RaisePropertyChanged("RollDeadzone");
                    }
                }
            }
        }

        public double PitchDeadzone {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.PitchDeadzone;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.PitchDeadzone != value)
                    {
                        GlobalData.cfg.PitchDeadzone = value;
                        RaisePropertyChanged("PitchDeadzone");
                    }
                }
            }
        }

        public double YawDeadzone {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.YawDeadzone;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.YawDeadzone != value)
                    {
                        GlobalData.cfg.YawDeadzone = value;
                        RaisePropertyChanged("YawDeadzone");
                    }
                }
            }
        }

        public double StartPYaw {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.StartPYaw;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.StartPYaw != value)
                    {
                        GlobalData.cfg.StartPYaw = value;
                        RaisePropertyChanged("StartPYaw");
                    }
                }
            }
        }

        public double StartPHeight {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.StartPHeight;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.StartPHeight != value)
                    {
                        GlobalData.cfg.StartPHeight = value;
                        RaisePropertyChanged("StartPHeight");
                    }
                }
            }
        }

        public double StartP1RollPitch {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.StartP1RollPitch;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.StartP1RollPitch != value)
                    {
                        GlobalData.cfg.StartP1RollPitch = value;
                        RaisePropertyChanged("StartP1RollPitch");
                    }
                }
            }
        }

        public double StartP2RollPitch {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.StartP2RollPitch;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.StartP2RollPitch != value)
                    {
                        GlobalData.cfg.StartP2RollPitch = value;
                        RaisePropertyChanged("StartP2RollPitch");
                    }
                }
            }
        }

        public double StartPLift {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.StartPLift;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.StartPLift != value)
                    {
                        GlobalData.cfg.StartPLift = value;
                        RaisePropertyChanged("StartPLift");
                    }
                }
            }
        }


        #endregion

    }
}
