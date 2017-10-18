using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication.Data
{
    class MiscParameterData : PackageData
    {
        byte funcRaw;
        byte funcLogging;
        byte funcWireless;
        uint ackNumber;

        public byte FuncRaw {
            get { return funcRaw; }
        }
        public byte FuncLogging {
            get { return funcLogging; }
        }
        public byte FuncWireless {
            get { return funcWireless; }
        }

        public MiscParameterData(byte FuncRaw, byte FuncLogging, byte FuncWireless, uint AckNumber = 0)
        {
            funcRaw = FuncRaw;
            funcLogging = FuncLogging;
            funcWireless = FuncWireless;
            
            ackNumber = AckNumber;
        }
        public MiscParameterData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");

            ackNumber = BitConverter.ToUInt32(data, 0);
            funcRaw = data[4];
            funcLogging = data[5];
            funcWireless = data[6];
        }
        public MiscParameterData()
        {
            throw new NotSupportedException();
        }

        public override int Length => 3 * sizeof(byte) + sizeof(uint);

        public override bool ExpectsAcknowledgement => (true);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] data = new byte[Length];

            Buffer.BlockCopy(BitConverter.GetBytes(ackNumber), 0, data, 0, sizeof(uint));
            data[4] = funcRaw;
            data[5] = funcLogging;
            data[6] = funcWireless;
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            ackNumber = AckNumber;
        }

    }
}
