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
        const double TRIM_STEP = 0.01;
        const double TRIM_MAX = 0.5;
        const double TRIM_MIN = -0.5;

        Timer RCTimer;
        public FlightMode Mode { get; set; }
        public double BatteryVoltage { get; set; }

        public double LoopTime { get; set; }

        public double Lift { get; set; }

        public double RollRate { get; set; }
        public double PitchRate { get; set; }
        public double YawRate { get; set; }

        public double LiftTrim { get; set; }
        public double RollTrim { get; set; }
        public double PitchTrim { get; set; }
        public double YawTrim { get; set; }

        public Controller()
        {
            RCTimer = new Timer(GlobalData.cfg.RCInterval);
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
            //Debug.WriteLine("Processing Telemetry....");
            //Debug.WriteLine(data.ToString());
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
            Debug.WriteLine(String.Format("Processing Exception of type {0} with message: {1}",data.ExceptionType, data.Message));

        }
        #endregion

        #region Control Methods
        public void ModeSwitch(FlightMode mode)
        {
            Debug.WriteLine(String.Format("Switching mode to {0}....",mode));
            GlobalData.com.ModeSwitch(mode);
        }
        public void Abort()
        {
            Debug.WriteLine("Aborting....");
            ModeSwitch(FlightMode.Panic);
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

        public void AdjustLiftTrim(bool? Direction)
        {
            if (Direction == null)
            {
                LiftTrim = 0;
            }
            else if (Direction == true) {
                LiftTrim = Math.Min(TRIM_MAX, LiftTrim + TRIM_STEP);
            }
            else if (Direction == false)
            {
                LiftTrim = Math.Max(TRIM_MIN, LiftTrim - TRIM_STEP);
            }
            RaisePropertyChanged("LiftTrim");
            Debug.WriteLine(String.Format("Set LiftTrim to {0}",LiftTrim));
        }
        public void AdjustRollTrim(bool? Direction)
        {
            if (Direction == null)
            {
                RollTrim = 0;
            }
            else if (Direction == true)
            {
                RollTrim = Math.Min(TRIM_MAX, RollTrim + TRIM_STEP);
            }
            else if (Direction == false)
            {
                RollTrim = Math.Max(TRIM_MIN, RollTrim - TRIM_STEP);
            }
            RaisePropertyChanged("RollTrim");
            Debug.WriteLine(String.Format("Set RollTrim to {0}", RollTrim));
        }
        public void AdjustPitchTrim(bool? Direction)
        {
            if (Direction == null)
            {
                PitchTrim = 0;
            }
            else if (Direction == true)
            {
                PitchTrim = Math.Min(TRIM_MAX, PitchTrim + TRIM_STEP);
            }
            else if (Direction == false)
            {
                PitchTrim = Math.Max(TRIM_MIN, PitchTrim - TRIM_STEP);
            }
            RaisePropertyChanged("PitchTrim");
            Debug.WriteLine(String.Format("Set PitchTrim to {0}", PitchTrim));
        }
        public void AdjustYawTrim(bool? Direction)
        {
            if (Direction == null)
            {
                YawTrim = 0;
            }
            else if (Direction == true)
            {
                YawTrim = Math.Min(TRIM_MAX, YawTrim + TRIM_STEP);
            }
            else if (Direction == false)
            {
                YawTrim = Math.Max(TRIM_MIN, YawTrim - TRIM_STEP);
            }
            RaisePropertyChanged("YawTrim");
            Debug.WriteLine(String.Format("Set YawTrim to {0}", YawTrim));
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
