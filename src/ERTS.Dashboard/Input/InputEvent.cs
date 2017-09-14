using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Input
{
    /// <summary>
	/// A custom event used for InputManager to give the event to all InputBindingInstances.
	/// </summary>
    public class InputEventArgs : EventArgs
    {
        /// <summary>
        /// The data to be saved in this eventArgs
        /// </summary>
        public readonly IStateUpdate StateUpdate;
        public readonly Guid DeviceGuid;
        public readonly bool InputEngaged;
        /// <summary>
        /// Wrapper for input data event
        /// </summary>
        /// <param name="_UpdateData">The device update structure.</param>
        /// <param name="_DeviceGuid">The device GUID.</param>
        public InputEventArgs(IStateUpdate _StateUpdate, Guid _DeviceGuid, bool _InputEngaged = true)
        {
            StateUpdate = _StateUpdate;
            DeviceGuid = _DeviceGuid;
            InputEngaged = _InputEngaged;
        }
    }
}
