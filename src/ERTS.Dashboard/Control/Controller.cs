using ERTS.Dashboard.Communication;
using ERTS.Dashboard.Communication.Data;
using ERTS.Dashboard.Communication.Enumerations;
using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ERTS.Dashboard.Control
{
    public class Controller : ObservableObject, IDisposable
    {
        const double RC_EXPO = 0.5;
        const double RC_RATE = 1;

        Timer RCTimer;
        public FlightMode Mode { get; set; }
        public double BatteryVoltage { get; set; }

        public double LoopTime { get; set; }

        public double Lift { get; set; }

        public double RollRate { get; set; }
        public double PitchRate { get; set; }
        public double YawRate { get; set; }

        public Controller()
        {
            RCTimer = new Timer(1000); //TODO put in Config
            RCTimer.Elapsed += RCTimer_Elapsed;
            RCTimer.Start();
            if (GlobalData.com != null)
                GlobalData.com.PacketReceivedEvent += Com_PacketReceivedEvent;
        }

        private void Com_PacketReceivedEvent(object sender, Communication.PacketReceivedEventArgs e)
        {
            Packet p = e.ReceivedPacket;
            switch (p.Type)
            {
                case MessageType.Acknowledge:
                    HandleAcknowledge((AcknowledgeData)p.Data);
                    break;
                case MessageType.Telemetry:
                    HandleTelemetry((TelemetryData)p.Data);
                    break;
                case MessageType.Exception:
                    HandleException((ExceptionData)p.Data);
                    break;
                case MessageType.RemoteControl:
                case MessageType.ModeSwitch:
                case MessageType.SetControllerRollPID:
                case MessageType.SetControllerPitchPID:
                case MessageType.SetControllerYawPID:
                case MessageType.SetControllerHeightPID:
                case MessageType.SetMessageFrequencies:
                case MessageType.Reset:
                case MessageType.Kill:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentException("Received Packet is unsupported at this time.", "Controller");


            }
        }

        private void RCTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (GlobalData.com != null)
            {
                //Scaling factors are -4, just for overflow safety, and no averse effect.
                GlobalData.com.RemoteControl(Convert.ToUInt16(Math.Round(Lift * 65532.0)), Convert.ToInt16(Math.Round(RollRate * 32764.0)),
                    Convert.ToInt16(Math.Round(PitchRate * 32764.0)), Convert.ToInt16(Math.Round(YawRate * 32764.0)));
            }
        }

        #region Communication Methods
        public void HandleTelemetry(TelemetryData data)
        {
            Debug.WriteLine("Processing Telemetry....");
            Debug.WriteLine(data.ToString());
            BatteryVoltage = data.BatteryVoltage / 100.0;
            RaisePropertyChanged("BatteryVoltage");
            LoopTime = data.LoopTime / 1000.0;
            RaisePropertyChanged("LoopTime");
            Mode = data.FlightMode;
            RaisePropertyChanged("Mode");

        }
        public void HandleAcknowledge(AcknowledgeData data)
        {
            Debug.WriteLine(String.Format("Processing Acknowledge {0}....",data.Number));
        }
        public void HandleException(ExceptionData data)
        {
            Debug.WriteLine(String.Format("Processing Exception of type {0}....",data.ExceptionType));

        }
        #endregion

        #region Control Methods
        public void Abort()
        {
            Debug.WriteLine("Aborting....");
            GlobalData.com.ModeSwitch(FlightMode.Panic);
        }
        public void SetLift(double _Lift)
        {
            Lift = GetRcRate(_Lift, 0, 1);
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
                rcCommand = rcCommand * Math.Pow(Math.Abs(rcCommand), 3) * expo + rcCommand * (1 - expo); // Courtesy of CleanFlight/BetaFlight
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
