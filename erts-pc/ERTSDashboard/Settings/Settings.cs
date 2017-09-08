using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace ERTS.Dashboard {
    //TODO Implementent this based on actual application settings. (Settings.settings file)
    [Serializable]
    public class Settings : ISerializable {
        [System.ComponentModel.DefaultValueAttribute("COM4")]
        public string Comport {
            get;
            set;
        }
        [System.ComponentModel.DefaultValueAttribute(921600)]
        public int BaudRate {
            get;
            set;
        }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool InterpolateOnSave {
            get;
            set;
        }

        [System.ComponentModel.DefaultValueAttribute(1.0)]
        public double TickTime {
            get;
            set;
        }
        [System.ComponentModel.DefaultValueAttribute(1)]
        public int UpdateTickInterval {
            get;
            set;
        }

        public Settings() {

        }
        protected Settings(SerializationInfo info, StreamingContext context) {
            try { Comport = info.GetString("Comport"); } catch { }
            try { BaudRate = info.GetInt32("BaudRate"); } catch { }
            try { InterpolateOnSave = info.GetBoolean("InterpolateOnSave"); } catch { }
            try { TickTime = info.GetDouble("TickTime"); } catch { }
            try { UpdateTickInterval = info.GetInt32("UpdateTickInterval"); } catch { }
        }
        [SecurityPermissionAttribute(SecurityAction.Demand,
        SerializationFormatter = true)]

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Comport", Comport);
            info.AddValue("BaudRate", BaudRate);
            info.AddValue("InterpolateOnSave", InterpolateOnSave);            
            info.AddValue("TickTime", TickTime);
            info.AddValue("UpdateTickInterval", UpdateTickInterval);
        }
    }
}
