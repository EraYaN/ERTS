using System;

namespace EraYaN.Serial
{
    /// <summary>
    /// A custom event used for higher-level (String) serial messaging.
    /// </summary>
    public class SerialDataEventArgs : EventArgs {
        /// <summary>
        /// The data to be saved in this eventArgs
        /// </summary>
        public readonly byte Data;
        /// <summary>
        /// Wrapper for serial data event
        /// </summary>
        /// <param name="data">The data byte</param>
        public SerialDataEventArgs(byte data) {
            Data = data;
        }
    }
}
