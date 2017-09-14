using ERTS.Dashboard.Communication.Enumerations;
using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Control
{
    public class Controller : ObservableObject, IDisposable
    {
        const double RC_EXPO = 0.5;
        const double RC_RATE = 1;
        public FlightMode Mode { get; set; }
        public int Voltage { get; set; }

        public double Lift { get; set; }

        public double RollRate { get; set; }
        public double PitchRate { get; set; }
        public double YawRate { get; set; }

        public Controller()
        {
            
        }

        
        #region Control Methods
        public void Abort()
        {
            Debug.WriteLine("Aborting....");
        }
        public void SetLift(double _Lift)
        {
            Lift = GetRcRate(_Lift,0,1);
            RaisePropertyChanged("Lift");
        }
        public void SetRoll(double _RollRate)
        {
            RollRate = GetRcRate(_RollRate);
            RaisePropertyChanged("RollRate");
        }
        public void SetPitch(double _PitchRate)
        {
            PitchRate = GetRcRate(_PitchRate);
            RaisePropertyChanged("PitchRate");
        }
        public void SetYaw(double _YawRate)
        {
            YawRate = GetRcRate(_YawRate);
            RaisePropertyChanged("YawRate");
        }
        #endregion

        double GetRcRate(double _input, double expo = RC_EXPO, double rate = RC_RATE)
        {
            double rcCommand = _input;
            if (RC_EXPO > 0)
                rcCommand = rcCommand * Math.Pow(Math.Abs(rcCommand),3) * expo + rcCommand * (1 - expo);
            double rcRate = rcCommand * rate;
            return rcRate;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Controller() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
