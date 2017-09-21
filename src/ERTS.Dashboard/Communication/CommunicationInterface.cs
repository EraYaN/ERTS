using EraYaN.Serial;
using ERTS.Dashboard.Communication.Data;
using ERTS.Dashboard.Communication.Enumerations;
using MicroMvvm;
using System;
using System.Diagnostics;
using System.Linq;

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
        }

        void com_SerialDataEvent(object sender, SerialDataEventArgs e)
        {
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
                    if(e.Data != 0xFE)
                        Debug.WriteLine(String.Format("Looking for packet start with {0:X4}", lastTwoBytes));
                }
            }
            else
            {
                packetBuffer[bufferIndex] = e.Data;
                bufferIndex++;
                if (bufferIndex == 2) // Received two bytes, check starting sequence
                {
                    if (packetBuffer[0] != ((Packet.START_SEQUENCE & 0xFF00) >> 8) || packetBuffer[1] != ((Packet.START_SEQUENCE & 0x00FF)))
                    {
                        //TODO Send exception
                        Debug.WriteLine("Packet did not have correct header.");
                        isReceivingPacket = false;
                        bufferIndex = 0;
                    }
                }
                if (bufferIndex == 3) // Received three bytes, check MessageType
                {
                    if (!Enum.IsDefined(typeof(MessageType), packetBuffer[2]))
                    {
                        //TODO Send exception
                        Debug.WriteLine("Packet had bad type.");
                        isReceivingPacket = false;
                        bufferIndex = 0;
                    }
                }
                if (bufferIndex == Packet.MAX_PACKET_SIZE) // Received twenty bytes, check endsequence
                {
                    if (packetBuffer[Packet.MAX_PACKET_SIZE - 1] != Packet.END_SEQUENCE)
                    {
                        //TODO Send exception
                        Debug.WriteLine("Packet had bad end sequence.");
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
                                Debug.WriteLine("Packet checksum mismatch.");
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

        public void SendPacket(Packet p)
        {            
            if (serial.IsOpen && p.IsGoodToSend())
            {
                serial.SendByteArray(p.ToByteArray());
            }
            RaisePropertyChanged("BytesInTBuffer");
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
                // free managed resources
                if (serial != null)
                {
                    serial.Dispose();
                }
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
