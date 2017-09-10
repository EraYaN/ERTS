using EraYaN.Serial;
using System;
using System.Diagnostics;
using System.Linq;

namespace ERTS.Dashboard.Communication
{
    public class CommunicationInterface : IDisposable {
        SerialInterface serial;
        string serialPort;
        int baudRate;
        public event EventHandler<PacketReceivedEventArgs> PacketReceivedEvent;

        bool isReceivingPacket = false;
        int bufferIndex = 0;
        byte[] packetBuffer = new byte[Packet.DATA_SIZE];

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

        public CommunicationInterface(string SerialPort, int BaudRate) {
            serialPort = SerialPort;
            baudRate = BaudRate;
            serial = new SerialInterface(SerialPort, BaudRate);
            serial.SerialDataEvent += com_SerialDataEvent;
            if (serial.OpenPort() != 0) {
                Debug.WriteLine(serial.lastError);
            }
        }

        void com_SerialDataEvent(object sender, SerialDataEventArgs e) {            
            if (isReceivingPacket == false) {
                isReceivingPacket = true;
                packetBuffer[bufferIndex] = e.Data;
                bufferIndex++;                
            } else {
                packetBuffer[bufferIndex] = e.Data;
                bufferIndex++;
                if (bufferIndex == 2) // Received two bytes, check starting sequence
                {
                    if(packetBuffer[0]!=((Packet.START_SEQUENCE & 0xFF00) >> 8) || packetBuffer[1] != ((Packet.START_SEQUENCE & 0x00FF)))
                    {
                        //TODO Send exception
                        isReceivingPacket = false;
                        bufferIndex = 0;
                    }
                }
                if (bufferIndex == 3) // Received three bytes, check MessageType
                {
                    if (!Enum.IsDefined(typeof(MessageType), packetBuffer[2]))
                    {
                        //TODO Send exception
                        isReceivingPacket = false;
                        bufferIndex = 0;
                    }
                }
                if (bufferIndex == Packet.MAX_PACKET_SIZE) // Received twenty bytes, check endsequence
                {
                    if (packetBuffer[Packet.MAX_PACKET_SIZE-1] != Packet.END_SEQUENCE)
                    {
                        //TODO Send exception
                        isReceivingPacket = false;
                        bufferIndex = 0;
                    } else
                    {
                        try
                        {
                            PacketReceived(new Packet(packetBuffer.ToArray()));
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

        public void SendPacket(Packet p) {
            if (serial.IsOpen) {
                serial.SendByteArray(p.ToByteArray());
            }
        }

        #region Protocol Methods
        /*public void OK() {
            Packet p = new Packet();
            p.Type = MessageType.OK;
            p.ID = 0;
            p.DataLength = 0;
            p.Data = new byte[0] { };
            SendPacket(p);
        }*/
        
        #endregion

        #region IDisposable members
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // free managed resources
                if (serial != null) {
                    serial.Dispose();
                }
            }
        }
        #endregion

        #region Event members
        void PacketReceived(Packet p) {
            OnPacketReceived(new PacketReceivedEventArgs(p));
        }

        protected virtual void OnPacketReceived(PacketReceivedEventArgs e) {
            if (PacketReceivedEvent != null) {
                PacketReceivedEvent(this, e);
            }
        }
        #endregion
    }
}
