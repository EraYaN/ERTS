using ERTS.Dashboard.Utility;
using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private Quadrupel _quad = new Quadrupel();
        //private InputManager _input = new InputManager();

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
            set {
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
        }
    }
}
