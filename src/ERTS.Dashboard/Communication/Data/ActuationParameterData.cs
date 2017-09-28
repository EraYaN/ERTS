using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication.Data
{
    class ActuationParameterData : PackageData
    {
        ushort ratePitchRollLift;
        ushort rateYaw;
        ushort motorMin;
        ushort motorMax;
        uint ackNumber;

        public ushort RatePitchRollLift {
            get { return ratePitchRollLift; }
        }
        public ushort RateYaw {
            get { return rateYaw; }
        }
        public ushort MotorMin {
            get { return motorMin; }
        }
        public ushort MotorMax {
            get { return motorMax; }
        }
        public uint AckNumber {
            get { return ackNumber; }
        }

        public ActuationParameterData(ushort RatePitchRollLift, ushort RateYaw, ushort MotorMin, ushort MotorMax, uint AckNumber)
        {
            ratePitchRollLift = RatePitchRollLift;
            rateYaw = RateYaw;
            motorMin = MotorMin;
            motorMax = MotorMax;
            ackNumber = AckNumber;
        }
        public ActuationParameterData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");

            ackNumber = BitConverter.ToUInt32(data, 0);
            ratePitchRollLift = BitConverter.ToUInt16(data, 4);
            rateYaw = BitConverter.ToUInt16(data, 6);
            motorMin = BitConverter.ToUInt16(data, 8);
            motorMax = BitConverter.ToUInt16(data, 10);
        }
        public ActuationParameterData()
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
            Buffer.BlockCopy(BitConverter.GetBytes(ratePitchRollLift), 0, data, 4, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(rateYaw), 0, data, 6, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(motorMin), 0, data, 8, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(motorMax), 0, data, 10, sizeof(ushort));
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            ackNumber = AckNumber;
        }

    }
}
