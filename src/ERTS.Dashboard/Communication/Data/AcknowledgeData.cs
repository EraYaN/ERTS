using System;

namespace ERTS.Dashboard.Communication.Data
{
    public class AcknowledgeData : PackageData
    {
        uint number;

        public uint Number {
            get { return number; }
        }

        public AcknowledgeData(uint Number = 0)
        {
            number = Number;
        }

        public AcknowledgeData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");            

            number = BitConverter.ToUInt32(data,0);
        }
        public AcknowledgeData()
        {
            throw new NotSupportedException();
        }

        public override int Length => sizeof(int);

        public override bool ExpectsAcknowledgement => (false);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] data = BitConverter.GetBytes(number);            
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            throw new NotImplementedException();
        }
    }
}
