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

        public int TargetLoopTime {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.TargetLoopTime;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.TargetLoopTime != value)
                    {
                        GlobalData.cfg.TargetLoopTime = value;
                        RaisePropertyChanged("TargetLoopTime");
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

        public int TelemetryInterval {
            get {
                if (GlobalData.cfg != null)
                {
                    return GlobalData.cfg.TelemetryInterval;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null)
                {
                    if (GlobalData.cfg.TelemetryInterval != value)
                    {
                        GlobalData.cfg.TelemetryInterval = value;
                        RaisePropertyChanged("TelemetryInterval");
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


        #endregion

    }
}
