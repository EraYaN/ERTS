﻿using ERTS.Dashboard.Communication.Enumerations;
using System;

namespace ERTS.Dashboard.Communication.Data
{
    public class TelemetryData : PackageData
    {
        ushort batteryVoltage; //12 bits
        FlightMode flightMode; // 4 bits
        short phi;
        short theta;
        short p;
        short q;
        short r;
        ushort loopTime;

        public ushort BatteryVoltage {
            get { return batteryVoltage; }
        }
        public FlightMode FlightMode {
            get { return flightMode; }
        }
        public short Phi {
            get { return phi; }
        }
        public short Theta {
            get { return theta; }
        }
        public short P {
            get { return p; }
        }
        public short Q {
            get { return q; }
        }
        public short R {
            get { return r; }
        }
        public ushort LoopTime {
            get { return loopTime; }
        }


        public TelemetryData(ushort BatteryVoltage, FlightMode FlightMode, short Phi, short Theta, short P, short Q, short R, ushort LoopTime)
        {
            batteryVoltage = BatteryVoltage;
            flightMode = FlightMode;
            phi = Phi;
            theta = Theta;
            p = P;
            q = Q;
            r = R;
            loopTime = LoopTime;
        }

        public TelemetryData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");

            ushort tmp = BitConverter.ToUInt16(data, 0);

            batteryVoltage = (ushort)((tmp & 0xFFF0) >> 4);
            if (!Enum.IsDefined(typeof(FlightMode), tmp & 0x000F))
                throw new ArgumentException("Data contains unrecognized FlightMode.", "data");

            flightMode = (FlightMode)(tmp & 0x000F);

            phi = BitConverter.ToInt16(data, sizeof(short));
            theta = BitConverter.ToInt16(data, sizeof(short) * 2);
            p = BitConverter.ToInt16(data, sizeof(short) * 3);
            q = BitConverter.ToInt16(data, sizeof(short) * 4);
            r = BitConverter.ToInt16(data, sizeof(short) * 5);
            loopTime = BitConverter.ToUInt16(data, sizeof(ushort) * 6);
        }

        public TelemetryData()
        {
            throw new NotSupportedException();
        }

        public override int Length => 6 * sizeof(ushort) + sizeof(FlightMode);

        public override bool ExpectsAcknowledgement => (false);

        public override bool IsValid()
        {
            return flightMode != FlightMode.None;
        }

        public override byte[] ToByteArray()
        {

            byte[] data = new byte[Length];
            //batteryVoltage and flightMode
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)(((batteryVoltage & 0xFFF) << 4) | ((byte)flightMode & 0xF))), 0, data, 0, sizeof(ushort));
            //phi
            Buffer.BlockCopy(BitConverter.GetBytes(phi), 0, data, sizeof(ushort), sizeof(ushort));
            //theta
            Buffer.BlockCopy(BitConverter.GetBytes(theta), 0, data, sizeof(ushort) * 2, sizeof(ushort));
            //rotations
            Buffer.BlockCopy(BitConverter.GetBytes(theta), 0, data, sizeof(ushort) * 3, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(theta), 0, data, sizeof(ushort) * 4, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(theta), 0, data, sizeof(ushort) * 5, sizeof(ushort));
            //loopTime
            Buffer.BlockCopy(BitConverter.GetBytes(loopTime), 0, data, sizeof(ushort) * 6, sizeof(ushort));
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            throw new NotImplementedException();
        }
    }
}
