using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication.Data
{
    
    class ControllerParameterData : PackageData
    {
        ushort pYaw;
        ushort pHeight;
        ushort p1PitchRoll;
        ushort p2PitchRoll;
        uint ackNumber;

        public ushort PYaw {
            get { return pYaw; }
        }
        public ushort PHeight {
            get { return pHeight; }
        }
        public ushort P1PitchRoll {
            get { return p1PitchRoll; }
        }
        public ushort P2PitchRoll {
            get { return p2PitchRoll; }
        }
        public uint AckNumber {
            get { return ackNumber; }
        }

        public ControllerParameterData(ushort PYaw, ushort PHeight, ushort P1PitchRoll, ushort P2PitchRoll, uint AckNumber)
        {
            pYaw = PYaw;
            pHeight = PHeight;
            p1PitchRoll = P1PitchRoll;
            p2PitchRoll = P2PitchRoll;
            ackNumber = AckNumber;
        }
        public ControllerParameterData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");

            ackNumber = BitConverter.ToUInt32(data, 0);
            pYaw = BitConverter.ToUInt16(data, 4);
            pHeight = BitConverter.ToUInt16(data, 6);
            p1PitchRoll = BitConverter.ToUInt16(data, 8);
            p2PitchRoll = BitConverter.ToUInt16(data, 10);
        }
        public ControllerParameterData()
        {
            throw new NotSupportedException();
        }

        public override int Length => 4 * sizeof(ushort) + sizeof(uint);

        public override bool ExpectsAcknowledgement => (true);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] data = new byte[Length];

            Buffer.BlockCopy(BitConverter.GetBytes(ackNumber), 0, data, 0, sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(pYaw), 0, data, 4, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(pHeight), 0, data, 6, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(p1PitchRoll), 0, data, 8, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(p2PitchRoll), 0, data, 10, sizeof(ushort));
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            ackNumber = AckNumber;
        }

    }
}
