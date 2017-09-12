using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication {
    public class PacketReceivedEventArgs : EventArgs {
        public readonly Packet ReceivedPacket;
        public PacketReceivedEventArgs(Packet _ReceivedPacket) {
            ReceivedPacket = _ReceivedPacket;
        }
    }
}
