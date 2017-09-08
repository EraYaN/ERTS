using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;

namespace ERTS.Dashboard {
    /// <summary>
    /// The ViewModel for the Settings Dialog
    /// </summary>
    public class ConfigurationViewModel : ObservableObject {
        #region Construction
        /// <summary>
        /// Constructs the default instance of a SongViewModel
        /// </summary>
        public ConfigurationViewModel() {
            // _song = new Song { ArtistName = "Unknown", SongTitle = "Unknown" };
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
                if (Data.cfg != null) {
                    return Data.cfg.Comport;
                }
                return String.Empty;
            }
            set {
                if (Data.cfg != null) {
                    if (Data.cfg.Comport != value) {
                        Data.cfg.Comport = value;
                        RaisePropertyChanged("Comport");
                    }
                }
            }
        }
        public int BaudRate {
            get {
                if (Data.cfg != null) {
                    return Data.cfg.BaudRate;
                }
                return 0;
            }
            set {
                if (Data.cfg != null) {
                    if (Data.cfg.BaudRate != value) {
                        Data.cfg.BaudRate = value;
                        RaisePropertyChanged("BaudRate");
                    }
                }
            }
        }

        public bool InterpolateOnSave {
            get {
                if (Data.cfg != null) {
                    return Data.cfg.InterpolateOnSave;
                }
                return false;
            }
            set {
                if (Data.cfg != null) {
                    if (Data.cfg.InterpolateOnSave != value) {
                        Data.cfg.InterpolateOnSave = value;
                        RaisePropertyChanged("InterpolateOnSave");
                    }
                }
            }
        }

        public double TickTime {
            get {
                if (Data.cfg != null) {
                    return Data.cfg.TickTime;
                }
                return 0;
            }
            set {
                if (Data.cfg != null) {
                    if (Data.cfg.TickTime != value) {
                        Data.cfg.TickTime = value;
                        RaisePropertyChanged("TickTime");
                    }
                }
            }
        }
        public int UpdateTickInterval {
            get {
                if (Data.cfg != null) {
                    return Data.cfg.UpdateTickInterval;
                }
                return 0;
            }
            set {
                if (Data.cfg != null) {
                    if (Data.cfg.UpdateTickInterval != value) {
                        Data.cfg.UpdateTickInterval = value;
                        RaisePropertyChanged("UpdateTickInterval");
                    }
                }
            }
        }
        
        #endregion

    }
}
