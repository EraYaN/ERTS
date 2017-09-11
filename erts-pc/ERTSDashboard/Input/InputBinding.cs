using MicroMvvm;
using System;
using System.Diagnostics;

namespace ERTS.Dashboard.Input
{
    [Serializable]
    public class InputBinding : ObservableObject
    {
        ControlActuator controlActuator;
        
        public ControlActuator Control {
            get { return controlActuator; }
        }

        [field: NonSerialized]
        public event EventHandler<BindingActuatedEventArgs> BindingActuatedEvent;

        public InputBinding(ControlActuator ControlActuator)
        {
            controlActuator = ControlActuator;
            GlobalData.input.InputEvent += InputEventFilterHandler;            
        }
        
        #region Event Handler
        private void InputEventFilterHandler(object sender, InputEventArgs e)
        {
            if (e.DeviceGuid != controlActuator.DeviceGuid)
                return;
            if (e.StateUpdate.RawOffset != controlActuator.RawOffset)
                return;

            Debug.WriteLine(String.Format("Got input for {0} by {1}, actuating binding.", controlActuator.Name, controlActuator.ControlDisplayName));
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
