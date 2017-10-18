using ERTS.Dashboard.Communication;
using ERTS.Dashboard.Communication.Data;
using ERTS.Dashboard.Communication.Enumerations;
using ERTS.Dashboard.Helpers;
using ERTS.Dashboard.Utility;
using MicroMvvm;
using System;
using System.Diagnostics;
using System.IO;

namespace ERTS.Dashboard.Control
{
    public class Controller : ObservableObject, IDisposable
    {
        const double RC_EXPO = 0.5;
        const double RC_RATE = 1;
        const double TRIM_STEP = 0.01;
        const double TRIM_MAX = 0.5;
        const double TRIM_MIN = -0.5;

        const double P_STEP = 1.0;
        const double P_MAX = 1000.0;
        const double P_MIN = 1.0;

        public const int FLASH_MAX_ADDRESS = 0x01FFFF;

        MultimediaTimer RCTimer;

        public bool HasSeenZeroLift { get; set; }
        public bool HasSeenZeroRoll { get; set; }
        public bool HasSeenZeroPitch { get; set; }
        public bool HasSeenZeroYaw { get; set; }

        public FlightMode Mode { get; set; }
        public double BatteryVoltage { get; set; }
        public short Phi { get; set; }
        public short Theta { get; set; }
        public short Psi { get; set; }

        public double LoopTime { get; set; }

        public double Lift { get; set; }

        public double RollRate { get; set; }
        public double PitchRate { get; set; }
        public double YawRate { get; set; }

        public double LiftTrim { get; set; }
        public double RollTrim { get; set; }
        public double PitchTrim { get; set; }
        public double YawTrim { get; set; }

        public double PYaw { get; set; }
        public double PHeight { get; set; }
        public double P1RollPitch { get; set; }
        public double P2RollPitch { get; set; }
        public double PLift { get; set; }

        public double FlashPosition {
            get {
                return dumpPosition;
            }
        }

        public bool FlashFileIsOpen {
            get {
                return flashFile != null;
            }
        }

        BinaryWriter flashFile;
        int dumpPosition = 0;
        //int counter=0;


        public Controller()
        {
            PYaw = GlobalData.cfg.StartPYaw;
            PHeight = GlobalData.cfg.StartPHeight;
            P1RollPitch = GlobalData.cfg.StartP1RollPitch;
            P2RollPitch = GlobalData.cfg.StartP2RollPitch;
            PLift = GlobalData.cfg.StartPLift;

            HasSeenZeroLift = false;
            HasSeenZeroRoll = false;
            HasSeenZeroPitch = false;
            HasSeenZeroYaw = false;

            RCTimer = new MultimediaTimer(GlobalData.cfg.RCInterval);

            RCTimer.Elapsed += RCTimer_Elapsed;
            RCTimer.Start();
            Debug.WriteLine(String.Format("Started RC interval timer with an interval of {0} ms.", GlobalData.cfg.RCInterval));

            if (GlobalData.com != null)
                GlobalData.com.PacketReceived += Com_PacketReceivedEvent;
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
                    //testing.
                    break;
                case MessageType.FlashData:
                    HandleFlashData((FlashData)p.Data);
                    break;
                case MessageType.ModeSwitch:
                case MessageType.ActuationParameters:
                case MessageType.ControllerParameters:
                case MessageType.MiscParameters:
                case MessageType.Reset:
                case MessageType.Kill:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentException("Received Packet is unsupported at this time.", "Controller");
            }
        }

        private void RCTimer_Elapsed(object sender, EventArgs e)
        {
            if (GlobalData.com != null)
            {
                /*if (counter >= 2000)
                {
                    return;
                }
                counter++;    */
                if (HasSeenZeroLift && HasSeenZeroPitch && HasSeenZeroRoll && HasSeenZeroYaw)
                {
                    GlobalData.com.RemoteControl(
                        Convert.ToUInt16(Math.Round((Lift + LiftTrim).Clamp(0, 1) * UInt16.MaxValue)),
                        Convert.ToInt16(Math.Round((RollRate + RollTrim).Clamp(-1, 1) * Int16.MaxValue)),
                        Convert.ToInt16(Math.Round((PitchRate + PitchTrim).Clamp(-1, 1) * Int16.MaxValue)),
                        Convert.ToInt16(Math.Round((YawRate + YawTrim).Clamp(-1, 1) * Int16.MaxValue)));
                }
                else
                {
                    GlobalData.com.RemoteControl(0, 0, 0, 0);
                }
                //GlobalData.com.RemoteControl(Convert.ToUInt16(counter), 0, 0 ,0);
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
            Phi = data.Phi;
            RaisePropertyChanged("Phi");
            Theta = data.Theta;
            RaisePropertyChanged("Theta");
            Psi = data.Psi;
            RaisePropertyChanged("Psi");

        }

        public void HandleFlashData(FlashData data)
        {
            if (flashFile != null)
            {
                int packetPosition = data.SequenceNumber * FlashData.MAX_DATA_LENGTH;
                if (packetPosition > FLASH_MAX_ADDRESS)
                {
                    Debug.WriteLine("Received FlashDump packet had a sequence number that was too high.");
                    return;
                }
                if (flashFile.BaseStream.Position != packetPosition)
                {
                    Debug.WriteLine(String.Format("Seeking in flash dump file from {0} to {1}, might miss a packet.", flashFile.BaseStream.Position, packetPosition));
                    flashFile.Seek(packetPosition, SeekOrigin.Begin);
                }
                if (data.SequenceNumber > Math.Floor((double)FLASH_MAX_ADDRESS / FlashData.MAX_DATA_LENGTH))
                {
                    //last packet
                    flashFile.Write(data.FlashBytes,0, FLASH_MAX_ADDRESS % FlashData.MAX_DATA_LENGTH);

                    EndFlashDump();
                }
                else
                {
                    flashFile.Write(data.FlashBytes, 0, FlashData.MAX_DATA_LENGTH);
                }
                dumpPosition = packetPosition;
                RaisePropertyChanged("FlashPosition");
                RaisePropertyChanged("FlashFileIsOpen");
            }
        }

        public void HandleAcknowledge(AcknowledgeData data)
        {
            Debug.WriteLine(String.Format("Processing Acknowledge {0}....", data.Number));
            GlobalData.com.HandleAcknowledgement(data.Number);
        }
        public void HandleException(ExceptionData data)
        {
            Debug.WriteLine(String.Format("Processing Exception of type {0} with message: {1}.\n", data.ExceptionType, data.Message));
        }

        #endregion

        #region Control Methods

        public void StartFlashDump()
        {
            if (flashFile != null)
            {
                flashFile.Close();
                flashFile = null;
                Debug.WriteLine("Restarting flash dump....");
            }
            else
            {
                Debug.WriteLine("Starting flash dump....");
            }            
            GlobalData.com.ModeSwitch(FlightMode.DumpFlash);
            FileStream fs = File.Open(String.Format("flash-{0}.bin", DateTime.Now.Ticks), FileMode.Create);
            fs.SetLength(FLASH_MAX_ADDRESS);
            flashFile = new BinaryWriter(fs);

            dumpPosition = 0;
            RaisePropertyChanged("FlashPosition");

            RaisePropertyChanged("FlashFileIsOpen");
        }

        public void EndFlashDump()
        {
            Debug.WriteLine("Ending flash dump....");
            GlobalData.com.ModeSwitch(FlightMode.Safe);
            if (flashFile != null)
            {
                flashFile.Close();
                flashFile = null;
            }

            dumpPosition = FLASH_MAX_ADDRESS;
            RaisePropertyChanged("FlashPosition");

            RaisePropertyChanged("FlashFileIsOpen");
        }

        public void ModeSwitch(FlightMode mode)
        {
            Debug.WriteLine(String.Format("Switching mode to {0}....", mode));
            GlobalData.com.ModeSwitch(mode);
        }
        public void Abort()
        {
            Debug.WriteLine("Aborting....");
            ModeSwitch(FlightMode.Panic);
        }
        public void SetLift(double _Lift)
        {
            Lift = GetRcRate(_Lift, 0, 1, GlobalData.cfg.LiftDeadzone);
            if (Lift == 0)
                HasSeenZeroLift = true;
            RaisePropertyChanged("Lift");
        }
        public void SetRoll(double _RollRate)
        {
            RollRate = GetRcRate(_RollRate, RC_EXPO, RC_RATE, GlobalData.cfg.RollDeadzone);
            if (RollRate == 0)
                HasSeenZeroRoll = true;
            RaisePropertyChanged("RollRate");
        }
        public void SetPitch(double _PitchRate)
        {
            PitchRate = GetRcRate(_PitchRate, RC_EXPO, RC_RATE, GlobalData.cfg.PitchDeadzone);
            if (PitchRate == 0)
                HasSeenZeroPitch = true;
            RaisePropertyChanged("PitchRate");
        }
        public void SetYaw(double _YawRate)
        {
            YawRate = GetRcRate(_YawRate, RC_EXPO, RC_RATE, GlobalData.cfg.YawDeadzone);
            if (YawRate == 0)
                HasSeenZeroYaw = true;
            RaisePropertyChanged("YawRate");
        }
        public void RawSwitch()
        {
            throw new NotImplementedException();
        }
        public void WirelessSwitch()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Trim and Control Adjustment
        public void AdjustLiftTrim(bool? Direction)
        {
            if (Direction == null)
            {
                LiftTrim = 0;
            }
            else if (Direction == true)
            {
                LiftTrim = Math.Min(TRIM_MAX, LiftTrim + TRIM_STEP);
            }
            else if (Direction == false)
            {
                LiftTrim = Math.Max(TRIM_MIN, LiftTrim - TRIM_STEP);
            }
            RaisePropertyChanged("LiftTrim");
            Debug.WriteLine(String.Format("Set LiftTrim to {0}", LiftTrim));
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

        public void AdjustPYaw(bool? Direction)
        {
            if (Direction == null)
            {
                PYaw = 0;
            }
            else if (Direction == true)
            {
                PYaw = Math.Min(P_MAX, PYaw + P_STEP);
            }
            else if (Direction == false)
            {
                PYaw = Math.Max(P_MIN, PYaw - P_STEP);
            }
            RaisePropertyChanged("PYaw");
            Debug.WriteLine(String.Format("Set PYaw to {0}", PYaw));
            SendControllerParameters();
        }
        public void AdjustPHeight(bool? Direction)
        {
            if (Direction == null)
            {
                PHeight = 0;
            }
            else if (Direction == true)
            {
                PHeight = Math.Min(P_MAX, PHeight + P_STEP);
            }
            else if (Direction == false)
            {
                PHeight = Math.Max(P_MIN, PHeight - P_STEP);
            }
            RaisePropertyChanged("PHeight");
            Debug.WriteLine(String.Format("Set PHeight to {0}", PHeight));
            SendControllerParameters();
        }

        public void AdjustP1RollPitch(bool? Direction)
        {
            if (Direction == null)
            {
                P1RollPitch = 0;
            }
            else if (Direction == true)
            {
                P1RollPitch = Math.Min(P_MAX, P1RollPitch + P_STEP);
            }
            else if (Direction == false)
            {
                P1RollPitch = Math.Max(P_MIN, P1RollPitch - P_STEP);
            }
            RaisePropertyChanged("P1RollPitch");
            Debug.WriteLine(String.Format("Set P1RollPitch to {0}", P1RollPitch));
            SendControllerParameters();
        }

        public void AdjustP2RollPitch(bool? Direction)
        {
            if (Direction == null)
            {
                P2RollPitch = 0;
            }
            else if (Direction == true)
            {
                P2RollPitch = Math.Min(P_MAX, P2RollPitch + P_STEP);
            }
            else if (Direction == false)
            {
                P2RollPitch = Math.Max(P_MIN, P2RollPitch - P_STEP);
            }
            RaisePropertyChanged("P2RollPitch");
            Debug.WriteLine(String.Format("Set P2RollPitch to {0}", P2RollPitch));
            SendControllerParameters();
        }

        public void AdjustPLift(bool? Direction)
        {
            if (Direction == null)
            {
                PLift = 0;
            }
            else if (Direction == true)
            {
                PLift = Math.Min(P_MAX, PLift + P_STEP);
            }
            else if (Direction == false)
            {
                PLift = Math.Max(P_MIN, PLift - P_STEP);
            }
            RaisePropertyChanged("PLift");
            Debug.WriteLine(String.Format("Set PLift to {0}", PLift));
            SendControllerParameters();
        }

        public void SendMiscParameters()
        {
            GlobalData.com.MiscParameters(Convert.ToUInt16(GlobalData.cfg.PanicDecrement), Convert.ToUInt16(GlobalData.cfg.RCInterval), Convert.ToUInt16(GlobalData.cfg.LogDivider), Convert.ToUInt16(GlobalData.cfg.BatteryThreshold), Convert.ToUInt16(GlobalData.cfg.TelemetryDivider));
        }

        public void SendControllerParameters()
        {
            GlobalData.com.ControllerParameters(Convert.ToUInt16(PYaw), Convert.ToUInt16(PHeight), Convert.ToUInt16(P1RollPitch), Convert.ToUInt16(P2RollPitch), Convert.ToUInt16(PLift));
        }

        public void SendActuationParameters()
        {
            //GlobalData.com.ActuationParameters(Convert.ToUInt16(PYaw), Convert.ToUInt16(PHeight), Convert.ToUInt16(P1RollPitch), Convert.ToUInt16(P2RollPitch), Convert.ToUInt16(PLift));
        }

        public void SendAllParameters()
        {
            SendMiscParameters();
            SendControllerParameters();
            SendActuationParameters();

        }

        #endregion

        double GetRcRate(double _input, double expo = RC_EXPO, double rate = RC_RATE, double deadzone = 0)
        {
            double rcCommand = _input.Deadzone(-deadzone / 100, deadzone / 100);

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
                    RCTimer.Stop();
                    RCTimer.Dispose();
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
