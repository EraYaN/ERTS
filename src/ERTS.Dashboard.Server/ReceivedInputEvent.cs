using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Server
{
    /// <summary>
    /// A custom event used for the input HTTP server
    /// </summary>
    public class ReceivedInputEventArgs : EventArgs
    {
        /// <summary>
        /// The lift value between 0.0 and 1.0
        /// </summary>
        public readonly double Lift;
        /// <summary>
        /// The pitch value between -1.0 and 1.0
        /// </summary>
        public readonly double Pitch;
        /// <summary>
        /// The roll value between -1.0 and 1.0
        /// </summary>
        public readonly double Roll;
        /// <summary>
        /// The yaw value between -1.0 and 1.0
        /// </summary>
        public readonly double Yaw;
        /// <summary>
        /// Wrapper for serial data event
        /// </summary>
        /// <param name="data">The data byte</param>
        public ReceivedInputEventArgs(double lift, double pitch, double roll, double yaw)
        {
            Lift = lift;
            Pitch = pitch;
            Roll = roll;
            Yaw = yaw;
        }
    }
}
