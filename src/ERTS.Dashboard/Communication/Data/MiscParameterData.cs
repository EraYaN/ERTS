using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication.Data
{
    class MiscParameterData : PackageData
    {
        ushort panicDecrement;
        ushort rcInterval;
        ushort logDivider;
        ushort batteryThreshold;
        ushort telemetryDivider;
        uint ackNumber;

        public ushort PanicDecrement {
            get { return panicDecrement; }
        }
        public ushort RCInterval {
            get { return rcInterval; }
        }
        public ushort LogDivider {
            get { return logDivider; }
        }
        public ushort BatteryThreshold {
            get { return batteryThreshold; }
        }
        public ushort TelemetryDivider {
            get { return telemetryDivider; }
        }
        public uint AckNumber {
            get { return ackNumber; }
        }

        public MiscParameterData(ushort PanicDecrement, ushort RCInterval, ushort LogDivider, ushort BatteryThreshold, ushort TelemetryDivider, uint AckNumber = 0)
        {
            panicDecrement = PanicDecrement;
            rcInterval = RCInterval;
            logDivider = LogDivider;
            batteryThreshold = BatteryThreshold;
            telemetryDivider = TelemetryDivider;
            ackNumber = AckNumber;
        }
        public MiscParameterData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");

            ackNumber = BitConverter.ToUInt32(data, 0);
            panicDecrement = BitConverter.ToUInt16(data, 4);
            rcInterval = BitConverter.ToUInt16(data, 6);
            logDivider = BitConverter.ToUInt16(data, 8);
            batteryThreshold = BitConverter.ToUInt16(data, 10);
            telemetryDivider = BitConverter.ToUInt16(data, 12);
        }
        public MiscParameterData()
        {
            throw new NotSupportedException();
        }

        public override int Length => 5 * sizeof(ushort) + sizeof(uint);

        public override bool ExpectsAcknowledgement => (true);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] data = new byte[Length];

            Buffer.BlockCopy(BitConverter.GetBytes(ackNumber), 0, data, 0, sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(panicDecrement), 0, data, 4, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(rcInterval), 0, data, 6, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(logDivider), 0, data, 8, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(batteryThreshold), 0, data, 10, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(telemetryDivider), 0, data, 12, sizeof(ushort));
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            ackNumber = AckNumber;
        }

    }
}
