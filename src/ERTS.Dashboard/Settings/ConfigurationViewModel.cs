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
        public ObservableCollection<string> Comports {
            get {
                return _comPorts;
            }
            set {
                _comPorts = value;
            }
        }

        public ObservableCollection<int> BaudRates {
            get {
                return _baudRates;
            }
            set {
                _baudRates = value;
            }
        }
        public string Comport {
            get {
                if (GlobalData.cfg != null) {
                    return GlobalData.cfg.Comport;
                }
                return String.Empty;
            }
            set {
                if (GlobalData.cfg != null) {
                    if (GlobalData.cfg.Comport != value) {
                        GlobalData.cfg.Comport = value;
                        RaisePropertyChanged("Comport");
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

        public string KillControlPrimaryString {
            get {
                return "test";
            }
        }

        /*public bool InterpolateOnSave {
            get {
                if (GlobalData.cfg != null) {
                    return GlobalData.cfg.InterpolateOnSave;
                }
                return false;
            }
            set {
                if (GlobalData.cfg != null) {
                    if (GlobalData.cfg.InterpolateOnSave != value) {
                        GlobalData.cfg.InterpolateOnSave = value;
                        RaisePropertyChanged("InterpolateOnSave");
                    }
                }
            }
        }

        public double TickTime {
            get {
                if (GlobalData.cfg != null) {
                    return GlobalData.cfg.TickTime;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null) {
                    if (GlobalData.cfg.TickTime != value) {
                        GlobalData.cfg.TickTime = value;
                        RaisePropertyChanged("TickTime");
                    }
                }
            }
        }
        public int UpdateTickInterval {
            get {
                if (GlobalData.cfg != null) {
                    return GlobalData.cfg.UpdateTickInterval;
                }
                return 0;
            }
            set {
                if (GlobalData.cfg != null) {
                    if (GlobalData.cfg.UpdateTickInterval != value) {
                        GlobalData.cfg.UpdateTickInterval = value;
                        RaisePropertyChanged("UpdateTickInterval");
                    }
                }
            }
        }*/

        #endregion

    }
}
