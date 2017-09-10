using System;
using System.Collections.Generic;
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
        public byte[] Data;
        public byte EndSequence;

        public byte[] ToByteArray(bool SetChecksum = true) {
            byte[] packed = new byte[MAX_PACKET_SIZE];            

            packed[0] = (byte)(StartSequence & 0xFF00 >> 8);
            packed[1] = (byte)(StartSequence & 0x00FF);
            packed[2] = (byte)Type;
            packed[3] = 0;
            packed[4] = 0;
            Buffer.BlockCopy(Data, 0, packed, 6, DATA_SIZE);
            packed[MAX_PACKET_SIZE - 1] = EndSequence;
            if (SetChecksum)
            {
                Checksum = GlobalData.crc.calculate_crc16(packed, MAX_PACKET_SIZE);

                packed[3] = (byte)(Checksum & 0xFF00 >> 8);
                packed[4] = (byte)(Checksum & 0x00FF);
            }

            return packed;
        }

        public Packet() {
            StartSequence = START_SEQUENCE;
            Data = Enumerable.Repeat((byte)0x0, DATA_SIZE).ToArray();
            EndSequence = END_SEQUENCE;
        }

        public Packet(byte[] PacketData) {
            StartSequence = (ushort)((PacketData[0]) << 8 | PacketData[1]);
            Type = (MessageType)(PacketData[2]);
            Checksum = (ushort)((PacketData[3]) << 8 | PacketData[4]);
            Buffer.BlockCopy(PacketData, HEADER_SIZE, Data, 0, DATA_SIZE);
            EndSequence = PacketData[MAX_PACKET_SIZE - 1];
        }
        public bool Validate()
        {
            byte[] PacketData = ToByteArray(false);
            ushort cs = GlobalData.crc.calculate_crc16(PacketData, MAX_PACKET_SIZE);
            return cs == Checksum;
        }
        public static bool Validate(byte[] PacketData) {
            ushort packet_cs = (ushort)((PacketData[3] << 8) | PacketData[4]);
            PacketData[3] = 0; //checksum to be updated later
            PacketData[4] = 0; //checksum to be updated later
            ushort cs = GlobalData.crc.calculate_crc16(PacketData, MAX_PACKET_SIZE);
            return cs == packet_cs; 
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Packet Type: {0}\n", Type.ToString());
            sb.AppendFormat("Checksum: {0:X04}", Checksum);
            sb.AppendFormat("Is Valid: {0}",Validate()? "Yes" : "No");
            sb.AppendFormat("Data: {0}", BitConverter.ToString(Data,0,DATA_SIZE).Replace("-"," "));
            return sb.ToString();
        }
    }
}
