using ERTS.Dashboard.Communication.Enumerations;
using System;
using System.Linq;
using System.Text;
using ERTS.Dashboard.Communication;

namespace ERTS.Dashboard.Communication.Data
{
    public class FlashData : PackageData
    {
        public const int MAX_DATA_LENGTH = Packet.DATA_SIZE - 2;
        ushort sequenceNumber;
        byte[] flashData;


        public ushort SequenceNumber {
            get { return sequenceNumber; }
        }

        public byte[] FlashData {
            get { return flashData; }
        }

        public FlashData(ushort SequenceNumber, byte[] FlashData)
        {
            sequenceNumber = SequenceNumber;
            if (FlashData.Length > MAX_DATA_LENGTH)
            {
                throw new ArgumentException(String.Format("Data can only be {0} bytes long.", Packet.DATA_SIZE - 2), "Data");
            }
            flashData = FlashData;
        }

        public FlashData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");            

            sequenceNumber = BitConverter.ToUInt16(data, 0);
            flashData = new ArraySegment<byte>(data, 2, MAX_DATA_LENGTH).ToArray());
        }
        public FlashData()
        {
            throw new NotSupportedException();
        }

        public override int Length => sizeof(ushort) + MAX_DATA_LENGTH;

        public override bool ExpectsAcknowledgement => (false);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] data = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(sequenceNumber), 0, data, sizeof(ushort) * 0, sizeof(ushort));
            Buffer.BlockCopy(flashData, 0, data, sizeof(ushort), MAX_DATA_LENGTH);
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            throw new NotImplementedException();
        }
    }
}
