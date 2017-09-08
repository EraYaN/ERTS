using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTSDashboard.Utility
{
    /// <summary>
	/// A custom event used for higher-level (String) serial messaging.
	/// </summary>
    public class InputEventArgs : EventArgs
    {
        /// <summary>
        /// The data to be saved in this eventArgs
        /// </summary>
        public readonly String Data;
        /// <summary>
        /// Wrapper for serial data event
        /// </summary>
        /// <param name="_Data">The actual data of this event</param>
        /// <param name="e">Embedded/inner SerialDataEventArgs in this SerialDataEventArgs</param>
        public InputEventArgs(String _Data)
        {
            Data = _Data;
        }
    }
}
