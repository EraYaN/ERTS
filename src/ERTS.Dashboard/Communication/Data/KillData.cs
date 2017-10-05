using System;

namespace ERTS.Dashboard.Communication.Data
{
    public class KillData : PackageData
    {
        uint ackNumber;

        public uint AckNumber {
            get { return ackNumber; }
        }

        public KillData(uint AckNumber = 0)
        {
            ackNumber = AckNumber;
        }

        public KillData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");            

            ackNumber = BitConverter.ToUInt32(data,0);
        }

        public override int Length => sizeof(int);

        public override bool ExpectsAcknowledgement => (true);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] data = BitConverter.GetBytes(ackNumber);            
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            ackNumber = AckNumber;
        }
    }
}
