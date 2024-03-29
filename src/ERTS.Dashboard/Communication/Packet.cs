﻿using ERTS.Dashboard.Communication.Data;
using ERTS.Dashboard.Communication.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication {
    public class Packet {
        public const ushort START_SEQUENCE = 0xFEFF;
        public const int MAX_PACKET_SIZE = 20;
        public const int HEADER_SIZE = 5;
        public const int FOOTER_SIZE = 1;
        public const int DATA_SIZE = MAX_PACKET_SIZE - HEADER_SIZE - FOOTER_SIZE;
        public const byte END_SEQUENCE = 0xFF;
        
        public ushort StartSequence;
        public MessageType Type;
        public ushort Checksum;
        public PackageData Data;
        public byte EndSequence;

        public byte[] ToByteArray(bool SetChecksum = true) {
            byte[] packed = new byte[MAX_PACKET_SIZE];
            Array.Clear(packed, 0, DATA_SIZE);

            packed[0] = (byte)((StartSequence & 0xFF00) >> 8);
            packed[1] = (byte)(StartSequence & 0x00FF);                      
            packed[2] = (byte)Type;
            packed[3] = 0;
            packed[4] = 0;
            Buffer.BlockCopy(Data.ToByteArray(), 0, packed, HEADER_SIZE, Math.Min(DATA_SIZE,Data.Length));
            packed[MAX_PACKET_SIZE - 1] = EndSequence;
            if (SetChecksum)
            {
                Checksum = GlobalData.crc.calculate_crc16(packed, MAX_PACKET_SIZE);
                Buffer.BlockCopy(BitConverter.GetBytes(Checksum), 0, packed, 3, sizeof(ushort));
            }

            return packed;
        }

        public Packet() {
            StartSequence = START_SEQUENCE;
            Type = MessageType.Unknown;
            Checksum = 0;
            Data = null;
            EndSequence = END_SEQUENCE;
        }

        public Packet(MessageType type)
        {
            StartSequence = START_SEQUENCE;
            Type = type;
            Checksum = 0;
            Data = null;
            EndSequence = END_SEQUENCE;
        }

        public Packet(byte[] PacketData) {
            StartSequence = BitConverter.ToUInt16(PacketData, 0);
            Type = (MessageType)(PacketData[2]);
            Checksum = BitConverter.ToUInt16(PacketData, 3);
            byte[] dataSegment = new ArraySegment<byte>(PacketData,HEADER_SIZE,DATA_SIZE).ToArray();
            switch (Type)
            {
                case MessageType.ModeSwitch:
                    Data = new ModeSwitchData(dataSegment);
                    break;
                case MessageType.Acknowledge:
                    Data = new AcknowledgeData(dataSegment);
                    break;
                case MessageType.Telemetry:
                    Data = new TelemetryData(dataSegment);
                    break;
                case MessageType.RemoteControl:
                    Data = new RemoteControlData(dataSegment);
                    break;
                case MessageType.FlashData:
                    Data = new FlashData(dataSegment);
                    break;
                //case MessageType.ActuationParameters:                    
                case MessageType.ControllerParameters:                    
                case MessageType.MiscParameters:                                  
                case MessageType.Reset:                    
                case MessageType.Kill:
                    throw new NotImplementedException();
                case MessageType.Exception:
                    Data = new ExceptionData(dataSegment);
                    break;
                default:
                    throw new ArgumentException("Data does not contain valid message type.","PacketData");

            }
            EndSequence = PacketData[MAX_PACKET_SIZE - 1];
        }
        public bool IsGoodToSend()
        {
            return Type != MessageType.Unknown && Checksum == 0 && Data.IsValid();
        }

        public void ResetForRetransmission()
        {
            Checksum = 0;
        }

        public bool Validate()
        {
            byte[] PacketData = ToByteArray(false);
            ushort cs = GlobalData.crc.calculate_crc16(PacketData, MAX_PACKET_SIZE);
            
            return cs == Checksum;
        }
        public static bool Validate(byte[] OriginalPacketData) {
            byte[] PacketData = OriginalPacketData.ToArray();
            ushort packet_cs = BitConverter.ToUInt16(PacketData,3);
            PacketData[3] = 0; //checksum to be updated later
            PacketData[4] = 0; //checksum to be updated later
            ushort cs = GlobalData.crc.calculate_crc16(PacketData, MAX_PACKET_SIZE);
            return cs == packet_cs; 
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Packet Type: {0}\n", Type.ToString());
            sb.AppendFormat("Checksum: {0:X04}\n", Checksum);
            sb.AppendFormat("Is Valid: {0}\n",Validate()? "Yes" : "No");
            sb.Append(Data.ToString());
            sb.Append("Raw Data:\n");
            foreach (byte b in Data.ToByteArray())
            {
                sb.Append(b.ToString("X2"));
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
