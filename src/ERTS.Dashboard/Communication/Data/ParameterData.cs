using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication.Data
{
    public class ParameterData : PackageData
    {
        ushort _b; // Divider for lift, pitch and roll
        ushort _d; // Divider for yaw

        /// <summary>
        /// Divider for lift, pitch and roll
        /// </summary>
        public ushort B {
            get { return _b; }
        }
        /// <summary>
        /// Divider for yaw
        /// </summary>
        public ushort D {
            get { return _d; }
        }

        public ParameterData(ushort b, ushort d)
        {
            _b = b;
            _d = d;
        }

        public ParameterData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");

            _b = BitConverter.ToUInt16(data, 0);
            _d = BitConverter.ToUInt16(data, sizeof(ushort) * 1);
        }

        public ParameterData()
        {
            throw new NotSupportedException();
        }

        public override int Length => 2 * sizeof(ushort);

        public override bool ExpectsAcknowledgement => (false);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {

            byte[] data = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(_b), 0, data, sizeof(ushort) * 0, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(_d), 0, data, sizeof(ushort) * 1, sizeof(ushort));
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            throw new NotImplementedException();
        }
    }
}
