using ERTS.Dashboard.Communication.Enumerations;
using System;
using System.Linq;
using System.Text;

namespace ERTS.Dashboard.Communication.Data
{
    public class ExceptionData : PackageData
    {
        public const int MAX_MESSAGE_LENGTH = 13;
        ExceptionType exceptionType;
        string message;


        public ExceptionType ExceptionType {
            get { return exceptionType; }
        }

        public string Message {
            get { return message; }
        }

        public ExceptionData(ExceptionType ExceptionType, string Message = "")
        {
            exceptionType = ExceptionType;
            message = Message.Substring(0, MAX_MESSAGE_LENGTH);
        }

        public ExceptionData(byte[] data)
        {
            if (data.Length < Length)
                throw new ArgumentException("Data is not long enough.", "data");
            if (!Enum.IsDefined(typeof(ExceptionType), data[0]))
                throw new ArgumentException("Data contains unrecognized ExceptionType.", "data");

            exceptionType = (ExceptionType)data[0];
            message = Encoding.ASCII.GetString(new ArraySegment<byte>(data, 1, MAX_MESSAGE_LENGTH).ToArray());
        }
        public ExceptionData()
        {
            throw new NotSupportedException();
        }

        public override int Length => sizeof(ExceptionType) + MAX_MESSAGE_LENGTH;

        public override bool ExpectsAcknowledgement => (false);

        public override bool IsValid()
        {
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] data = new byte[Length];
            data[0] = (byte)exceptionType;
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(message), 0, data, sizeof(ExceptionType), MAX_MESSAGE_LENGTH);
            return data;
        }

        public override void SetAckNumber(uint AckNumber)
        {
            throw new NotImplementedException();
        }
    }
}
