using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication
{
    class SentPacket
    {
        Packet p;
        DateTime timestamp;
        List<uint> ackNumbers;
        uint numberOfTries;

        public Packet Packet {
            get { return p; }
        }
        public uint NumberOfTries {
            get { return numberOfTries; }
        }

        public DateTime Timestamp {
            get { return timestamp; }
        }

        public List<uint> AckNumbers {
            get { return ackNumbers; }
        }

        public bool HasAckNumber(uint AckNumber)
        {
            return AckNumbers.Contains(AckNumber);
        }

        public SentPacket(Packet P, uint AckNumber)
        {
            p = P;
            ackNumbers = new List<uint>();
            ackNumbers.Add(AckNumber);
            timestamp = DateTime.Now;
            numberOfTries = 1;
        }

        public void PrepareForNextTry(uint NewAckNumber)
        {
            p.Data.SetAckNumber(NewAckNumber);
            p.ResetForRetransmission();
            ackNumbers.Add(NewAckNumber);
            timestamp = DateTime.Now;
            numberOfTries++;
        }
    }
}
