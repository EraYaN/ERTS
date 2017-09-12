using System;

namespace EraYaN.Serial
{
    /// <summary>
    /// A simple interface for the SerialInterface.
    /// </summary>
    interface ISerial {
        event EventHandler<SerialDataEventArgs> SerialDataEvent;
    }
}
