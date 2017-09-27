using MicroMvvm;
using System;
using System.Diagnostics;
using System.Linq;

namespace ERTS.Dashboard.Input
{
    [Serializable]
    public class InputBinding : ObservableObject
    {
        ControlActuator[] controlActuators;
        public readonly string Name;

        public ControlActuator[] Controls {
            get { return controlActuators; }
        }

        [field: NonSerialized]
        public event EventHandler<BindingActuatedEventArgs> BindingActuatedEvent;

        public InputBinding(ControlActuator[] ControlActuator, string _Name="")
        {
            Name = _Name;
            controlActuators = ControlActuator;
            GlobalData.input.InputEvent += InputEventFilterHandler;            
        }
        
        #region Event Handler
        private void InputEventFilterHandler(object sender, InputEventArgs e)
        {
            if (!e.InputEngaged)
                return;
            if (!controlActuators.Any(cA => cA.DeviceGuid == e.DeviceGuid && cA.RawOffset == e.StateUpdate.RawOffset))
                return;

            ControlActuator activatedKey = controlActuators.First(cA => cA.DeviceGuid == e.DeviceGuid && cA.RawOffset == e.StateUpdate.RawOffset);

            //Debug.WriteLine(String.Format("Got input for {0} by {1}, actuating binding.", Name, activatedKey.ControlDisplayName));
            BindingActuated(e);
        }
        #endregion

        #region Event Source
        void BindingActuated(InputEventArgs InnerEvent)
        {
            OnBindingActuated(new BindingActuatedEventArgs(InnerEvent));
        }

        protected virtual void OnBindingActuated(BindingActuatedEventArgs e)
        {
            if (BindingActuatedEvent != null)
            {
                BindingActuatedEvent(this, e);
            }
        }
        #endregion

    }
}
