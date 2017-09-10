using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication.Data
{
    public class RemoteControlData : PackageData
    {
        ushort lift; //Throttle
        short roll; //Aileron
        short pitch; //Elevator
        short yaw; //Rudder
        
        /// <summary>
        /// The Throttle control signal
        /// </summary>
        public ushort Lift {
            get { return lift; }
        }
        /// <summary>
        /// The Aileron control signal
        /// </summary>
        public short Roll {
            get { return roll; }
        }
        /// <summary>
        /// The Elevator control signal
        /// </summary>
        public short Pitch {
            get { return pitch; }
        }
        /// <summary>
        /// The Rudder control signal
        /// </summary>
        public short Yaw {
            get { return yaw; }
        }
        public RemoteControlData(ushort Lift, short Roll, short Pitch, short Yaw)
        {
            lift = Lift;
            roll = Roll;
            pitch = Pitch;
            yaw = Yaw;
        }

        public RemoteControlData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");

            lift = BitConverter.ToUInt16(data, 0);
            roll = BitConverter.ToInt16(data, sizeof(ushort) * 1);
            pitch = BitConverter.ToInt16(data, sizeof(ushort) * 2);
            yaw = BitConverter.ToInt16(data, sizeof(ushort) * 3);
        }

        public RemoteControlData()
        {
            throw new NotSupportedException();
        }

        public override int Length => 4 * sizeof(ushort);

        public override bool ExpectsAcknowledgement => (false);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {

            byte[] data = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(lift), 0, data, sizeof(ushort) * 0, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(roll), 0, data, sizeof(ushort) * 1, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(pitch), 0, data, sizeof(ushort) * 2, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(yaw), 0, data, sizeof(ushort) * 3, sizeof(ushort));
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            throw new NotImplementedException();
        }
    }
}
