using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Server
{
    /// <summary>
    /// A custom event used for the input HTTP server
    /// </summary>
    public class ClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// The connected remote host
        /// </summary>
        public readonly IPAddress Remote;

        /// <summary>
        /// The event for when a client connects
        /// </summary>
        /// <param name="remote">The remote host</param>
        public ClientConnectedEventArgs(IPAddress remote)
        {
            Remote = remote;            
        }
    }
}
