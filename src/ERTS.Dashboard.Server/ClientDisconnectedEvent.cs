using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Server
{
    /// <summary>
    /// A custom event used for the inp[ut HTTP server
    /// </summary>
    public class ClientDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// The disconnected remote host
        /// </summary>
        public readonly IPAddress Remote;

        /// <summary>
        /// The event for when a client disconnects
        /// </summary>
        /// <param name="remote">The remote host</param>
        public ClientDisconnectedEventArgs(IPAddress remote)
        {
            Remote = remote;
        }
    }
}
