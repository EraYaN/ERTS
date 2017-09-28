using EraYaN.Serial;
using ERTS.Dashboard.Communication.Data;
using ERTS.Dashboard.Communication.Enumerations;
using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace ERTS.Dashboard.Communication
{
    public class CommunicationInterface : ObservableObject, IDisposable
    {
        SerialInterface serial;
        string serialPort;
        int baudRate;
        public event EventHandler<PacketReceivedEventArgs> PacketReceivedEvent;

        bool isReceivingPacket = false;
        int bufferIndex = 0;
        byte[] packetBuffer = new byte[Packet.MAX_PACKET_SIZE];
        ushort lastTwoBytes = 0;
        uint LastAckNumber = 1;

        Random random = new Random();

        Timer PacketTimer;

        List<SentPacket> sentPackets = new List<SentPacket>();
        object sentPacketsLockObject = new object();

        #region Status Properties
        public bool IsOpen {
            get { return serial.IsOpen; }
        }

        public int BytesInRBuffer {
            get { return serial.BytesInRBuffer; }
        }

        public int BytesInTBuffer {
            get { return serial.BytesInTBuffer; }
        }

        public int UnacknowlegdedPackets {
            get {
                lock (sentPacketsLockObject)
                {
                    return sentPackets.Count;
                }
            }
        }

        int _bytesReceived = 0;
        public int BytesReceived {
            get {
                return _bytesReceived;
            }
            set {
                if (_bytesReceived != value)
                {
                    _bytesReceived = value;
                    RaisePropertyChanged("BytesReceived");
                }
            }
        }
        int _bytesSent = 0;
        public int BytesSent {
            get {
                return _bytesSent;
            }
            set {
                if (_bytesSent != value)
                {
                    _bytesSent = value;
                    RaisePropertyChanged("BytesSent");
                }
            }
        }
        #endregion

        public CommunicationInterface(string SerialPort, int BaudRate)
        {
            serialPort = SerialPort;
            baudRate = BaudRate;
            serial = new SerialInterface(SerialPort, BaudRate);
            serial.SerialDataEvent += com_SerialDataEvent;
            if (serial.OpenPort() != 0)
            {
                Debug.WriteLine(serial.lastError);
            }
            RaisePropertyChanged("IsOpen");
            PacketTimer = new Timer(GlobalData.cfg.PacketCheckResendInterval);
            PacketTimer.Elapsed += PacketTimer_Elapsed;
            PacketTimer.Start();

        }

        private void PacketTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (sentPacketsLockObject)
            {
                if (sentPackets.Count > 0)
                {
                    List<SentPacket> removedPackets = new List<SentPacket>();
                    var overDuePackets = sentPackets.Where(item => (item.Timestamp.Ticks < DateTime.Now.Ticks - GlobalData.cfg.PacketResendInterval * TimeSpan.TicksPerMillisecond && item.NumberOfTries > 1) || (item.Timestamp.Ticks < DateTime.Now.Ticks - GlobalData.cfg.PacketResendInterval * 10 * TimeSpan.TicksPerMillisecond && item.NumberOfTries == 1));
                    foreach (SentPacket sp in overDuePackets)
                    {
                        uint newAckNumber = NextAcknowlegdementNumber();
                        sp.PrepareForNextTry(newAckNumber);
                        if (sp.NumberOfTries <= GlobalData.cfg.PacketRetransmissionCount)
                        {
                            Debug.WriteLine(String.Format("Retransmitting packet ({0}).", sp.NumberOfTries));
                            SendPacket(sp.Packet, true);
                        }
                        else
                        {
                            if (GlobalData.cfg.KillAfterRetransmissionFail)
                            {
                                //TODO implement killing packets
                                Debug.WriteLine(String.Format("Packet retransmissions failed after {0} attempts, trying to kill quad.", GlobalData.cfg.PacketRetransmissionCount));
                                //Kill();
                            }
                            removedPackets.Add(sp);
                        }
                    }
                    foreach (SentPacket sp in removedPackets)
                    {
                        sentPackets.Remove(sp);
                    }
                    RaisePropertyChanged("UnacknowlegdedPackets");
                }
            }
        }

        uint NextAcknowlegdementNumber()
        {
            return ++LastAckNumber;
        }

        public bool HandleAcknowledgement(uint ackNumber)
        {
            lock (sentPacketsLockObject)
            {
                return sentPackets.RemoveAll(p => p.HasAckNumber(ackNumber)) > 0;
            }
        }

        void com_SerialDataEvent(object sender, SerialDataEventArgs e)
        {
            BytesReceived++;
            RaisePropertyChanged("BytesInRBuffer");
            //Debug.WriteLine(e.Data,"COMMBYTE");
            lastTwoBytes = (ushort)(lastTwoBytes << 8 | e.Data);
            if (isReceivingPacket == false)
            {
                if (lastTwoBytes == Packet.START_SEQUENCE)
                {
                    packetBuffer[0] = ((Packet.START_SEQUENCE & 0xFF00) >> 8);
                    packetBuffer[1] = (Packet.START_SEQUENCE & 0x00FF);
                    bufferIndex = 2;
                    isReceivingPacket = true;
                }
                else
                {
                    if (e.Data != 0xFE)
                        Debug.WriteLine(String.Format("Looking for packet start with {0:X4}", lastTwoBytes));
                }
            }
            else
            {
                packetBuffer[bufferIndex] = e.Data;
                bufferIndex++;
                if (bufferIndex == 3) // Received three bytes, check MessageType
                {
                    if (!Enum.IsDefined(typeof(MessageType), packetBuffer[2]))
                    {
                        //TODO Send exception
                        Debug.WriteLine(String.Format("Packet had bad type: {0}", PacketToStringArray(packetBuffer)));
                        isReceivingPacket = false;
                        bufferIndex = 0;
                    }
                }
                else if (bufferIndex == Packet.MAX_PACKET_SIZE) // Received twenty bytes, check endsequence
                {
                    if (packetBuffer[Packet.MAX_PACKET_SIZE - 1] != Packet.END_SEQUENCE)
                    {
                        //TODO Send exception
                        Debug.WriteLine(String.Format("Packet had bad end sequence: {0}", PacketToStringArray(packetBuffer)));
                        isReceivingPacket = false;
                        bufferIndex = 0;
                    }
                    else
                    {
                        try
                        {
                            if (Packet.Validate(packetBuffer))
                            {
                                Packet p = new Packet(packetBuffer.ToArray());
                                //Debug.WriteLine(p.ToString());
                                PacketReceived(p);
                            }
                            else
                            {
                                Debug.WriteLine(String.Format("Packet checksum mismatch: {0}", PacketToStringArray(packetBuffer)));
                            }

                        }
                        catch (ArgumentException ex)
                        {
                            //Bad Packet, Discard
                            System.Diagnostics.Debug.WriteLine(ex);
                        }
                        isReceivingPacket = false;
                        bufferIndex = 0;
                    }
                }

            }
        }

        public void SendPacket(Packet p, bool Retransmission = false)
        {
            if (serial.IsOpen && p.IsGoodToSend())
            {
                if (!Retransmission)
                {
                    if (p.Data.ExpectsAcknowledgement)
                    {
                        uint ackNumber = NextAcknowlegdementNumber();
                        p.Data.SetAckNumber(ackNumber);
                        lock (sentPacketsLockObject)
                        {
                            sentPackets.Add(new SentPacket(p, ackNumber));
                        }
                    }
                }
                Debug.WriteLine(String.Format("Sending Packet: {0}", PacketToStringArray(p)));
                BytesSent += Packet.MAX_PACKET_SIZE;
                serial.SendByteArray(p.ToByteArray());
            }
            else
            {
                Debug.WriteLine("Packet could not be sent.");
            }

            RaisePropertyChanged("BytesInTBuffer");
            RaisePropertyChanged("UnacknowlegdedPackets");

        }

        string PacketToStringArray(Packet p)
        {
            return PacketToStringArray(p.ToByteArray());
        }
        string PacketToStringArray(byte[] p)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in p)
            {
                sb.Append("0x");
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }
            return sb.ToString();
        }

        #region Protocol Methods

        public void ModeSwitch(FlightMode NewMode, FlightMode FallbackMode = FlightMode.None)
        {
            Packet p = new Packet(MessageType.ModeSwitch)
            {
                Data = new ModeSwitchData(NewMode, FallbackMode)
            };
            SendPacket(p);
        }

        public void Acknowledge(uint Number = 0)
        {
            Packet p = new Packet(MessageType.Acknowledge)
            {
                Data = new AcknowledgeData(Number)
            };
            SendPacket(p);
        }

        public void Telemetry(ushort BatteryVoltage, FlightMode FlightMode, short Phi, short Theta, short P, short Q, short R, ushort LoopTime)
        {
            Packet p = new Packet(MessageType.Telemetry)
            {
                Data = new TelemetryData(BatteryVoltage, FlightMode, Phi, Theta, P, Q, R, LoopTime)
            };
            SendPacket(p);
        }

        public void RemoteControl(ushort Lift, short Roll, short Pitch, short Yaw)
        {
            Packet p = new Packet(MessageType.RemoteControl)
            {
                Data = new RemoteControlData(Lift, Roll, Pitch, Yaw)
            };
            SendPacket(p);
        }

        /*
        //Parameter messages (0x40-0x9F)
        SetControllerRollPID = 0x40, ///Expects Acknowledgement
        SetControllerPitchPID = 0x41, ///Expects Acknowledgement
        SetControllerYawPID = 0x42, ///Expects Acknowledgement
        SetControllerHeightPID = 0x43, ///Expects Acknowledgement
        SetMessageFrequencies = 0x44, ///Expects Acknowledgement. TelemetryFrequency, RemoteControlFrequency and LoopFreqency

        //Reserved for future use (0xA0-0xDF)

        //Exceptions, system commands and other failure mode related stuff (0xF0 - 0xFD)
        Reset = 0xFB, ///Expects Acknolegdement. Resets the Embedded System
        Kill = 0xFC, ///Expects Acknolegdement. Kills all activity
        Exception = 0xFD ///Expects no Acknolegdement. Reports exception to peer.*/

        #endregion

        #region IDisposable members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {                
                PacketTimer.Stop();
                // free managed resources
                if (serial != null)
                {
                    serial.Dispose();
                }
                Debug.WriteLine(String.Format("Bytes Received: {0}", BytesReceived), "CommunicationInterface");
                Debug.WriteLine(String.Format("Bytes Sent: {0}", BytesSent), "CommunicationInterface");
            }
        }
        #endregion

        #region Event members
        void PacketReceived(Packet p)
        {
            OnPacketReceived(new PacketReceivedEventArgs(p));
        }

        protected virtual void OnPacketReceived(PacketReceivedEventArgs e)
        {
            if (PacketReceivedEvent != null)
            {
                PacketReceivedEvent(this, e);
            }
        }
        #endregion
    }
}
