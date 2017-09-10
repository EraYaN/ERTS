using ERTS.Dashboard.Communication.Enumerations;
using System;

namespace ERTS.Dashboard.Communication.Data
{
    public class ModeSwitchData : PackageData
    {
        FlightMode newMode;
        FlightMode fallbackMode;
        uint ackNumber;

        public FlightMode NewMode {
            get { return newMode; }
        }
        public FlightMode FallbackMode {
            get { return fallbackMode; }
        }
        public uint AckNumber {
            get { return ackNumber; }
        }

        public ModeSwitchData(FlightMode NewMode, FlightMode FallbackMode = FlightMode.None, uint AckNumber = 0)
        {
            newMode = NewMode;
            fallbackMode = FallbackMode;
            ackNumber = AckNumber;
        }
        
        public ModeSwitchData(byte[] data)
        {
            if(data.Length<Length)
                throw new ArgumentException("Data is not long enough.", "data");
            if (!Enum.IsDefined(typeof(FlightMode), data[0]))
                throw new ArgumentException("Data contains unrecognized new FlightMode.", "data");
            if (!Enum.IsDefined(typeof(FlightMode), data[1]))
                throw new ArgumentException("Data contains unrecognized fallback FlightMode.", "data");

            newMode = (FlightMode)data[0];
            fallbackMode = (FlightMode)data[1];
            ackNumber = BitConverter.ToUInt32(data, 2);
        }
        public ModeSwitchData()
        {
            throw new NotSupportedException();
        }

        public override int Length => 2 * sizeof(FlightMode) + sizeof(int);        

        public override bool ExpectsAcknowledgement => (true);

        public override bool IsValid()
        {
            return newMode != FlightMode.None;
        }

        public override byte[] ToByteArray()
        {
            byte[] data = new byte[Length];
            data[0] = (byte)newMode;
            data[1] = (byte)fallbackMode;
            Buffer.BlockCopy(BitConverter.GetBytes(ackNumber), 0, data, sizeof(FlightMode) *2, sizeof(int));
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            ackNumber = AckNumber;
        }
    }
}
