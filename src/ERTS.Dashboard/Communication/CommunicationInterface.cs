using EraYaN.Serial;
using ERTS.Dashboard.Communication.Data;
using ERTS.Dashboard.Communication.Enumerations;
using ERTS.Dashboard.Helpers;
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

        MultimediaTimer PacketTimer;
        Timer BandwitdhTimer;

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
        Stopwatch BandwidthStopwatch;
        int _lastBytesReceived = 0;
        int _bytesReceived = 0;
        public int BytesReceived {
            get {
                return _bytesReceived;
            }
            set {
                if (_bytesReceived != value)
                {
                    _bytesReceived = value;
                    //RaisePropertyChanged("BytesReceived");
                }
            }
        }
        int _lastBytesSent = 0;
        int _bytesSent = 0;
        public int BytesSent {
            get {
                return _bytesSent;
            }
            set {
                if (_bytesSent != value)
                {
                    _bytesSent = value;
                    //RaisePropertyChanged("BytesSent");
                }
            }
        }
        int _lastPacketsReceived = 0;
        int _packetsReceived = 0;
        public int PacketsReceived {
            get {
                return _packetsReceived;
            }
            set {
                if (_packetsReceived != value)
                {
                    _packetsReceived = value;
                    //RaisePropertyChanged("PacketsReceived");
                }
            }
        }
        int _lastPacketsSent = 0;
        int _packetsSent = 0;
        public int PacketsSent {
            get {
                return _packetsSent;
            }
            set {
                if (_packetsSent != value)
                {
                    _packetsSent = value;
                    //RaisePropertyChanged("PacketsSent");
                }
            }
        }
        double _receivedBandwidth = 0;
        public double ReceivedBandwidth {
            get {
                return _receivedBandwidth;
            }
            set {
                if (_receivedBandwidth != value)
                {
                    _receivedBandwidth = value;
                    RaisePropertyChanged("ReceivedBandwidth");
                }
            }
        }
        double _sentBandwidth = 0;
        public double SentBandwidth {
            get {
                return _sentBandwidth;
            }
            set {
                if (_sentBandwidth != value)
                {
                    _sentBandwidth = value;
                    RaisePropertyChanged("SentBandwidth");
                }
            }
        }
        double _packetsReceivedPerSecond = 0;
        public double PacketsReceivedPerSecond {
            get {
                return _packetsReceivedPerSecond;
            }
            set {
                if (_packetsReceivedPerSecond != value)
                {
                    _packetsReceivedPerSecond = value;
                    RaisePropertyChanged("PacketsReceivedPerSecond");
                }
            }
        }
        double _packetsSentPerSecond = 0;
        public double PacketsSentPerSecond {
            get {
                return _packetsSentPerSecond;
            }
            set {
                if (_packetsSentPerSecond != value)
                {
                    _packetsSentPerSecond = value;
                    RaisePropertyChanged("PacketsSentPerSecond");
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
            BandwidthStopwatch = new Stopwatch();
            PacketTimer = new MultimediaTimer(GlobalData.cfg.PacketCheckResendInterval);
            PacketTimer.Elapsed += PacketTimer_Elapsed;
            PacketTimer.Start();
            BandwitdhTimer = new Timer(1000);
            BandwitdhTimer.Elapsed += BandwitdhTimer_Elapsed;
            BandwitdhTimer.Start();
            BandwidthStopwatch.Start();
        }

        private void BandwitdhTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            BandwidthStopwatch.Stop();
            double seconds = BandwidthStopwatch.Elapsed.TotalSeconds;
            BandwidthStopwatch.Restart();
            ReceivedBandwidth = (_bytesReceived - _lastBytesReceived) / seconds;
            SentBandwidth = (_bytesSent - _lastBytesSent) / seconds;

            PacketsReceivedPerSecond = (_packetsReceived - _lastPacketsReceived) / seconds;
            PacketsSentPerSecond = (_packetsSent - _lastPacketsSent) / seconds;

            _lastBytesSent = _bytesSent;
            _lastBytesReceived = _bytesReceived;
            _lastPacketsSent = _packetsSent;
            _lastPacketsReceived = _packetsReceived;
        }

        private void PacketTimer_Elapsed(object sender, EventArgs e)
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
                //Debug.WriteLine(String.Format("Sending Packet: {0}", PacketToStringArray(p)));
                PacketsSent++;
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
                PacketTimer.Dispose();
                BandwitdhTimer.Stop();
                BandwitdhTimer.Dispose();
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
            PacketsReceived++;
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
