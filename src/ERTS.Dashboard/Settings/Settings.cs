using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;

namespace ERTS.Dashboard.Configuration {
    //TODO Implementent this based on actual application settings. (Configuration.settings file)
    [Serializable]
    public class Settings : ISerializable {
        const string CFG_FILE = "cfg.bin";
        [System.ComponentModel.DefaultValueAttribute("COM4")]
        public string Comport {
            get;
            set;
        }
        [System.ComponentModel.DefaultValueAttribute(115200)]
        public int BaudRate {
            get;
            set;
        }
        //The target loop time on the Embedded System in microseconds
        [System.ComponentModel.DefaultValueAttribute(1000)]
        public int TargetLoopTime {
            get;
            set;
        }
        /// <summary>
        /// The interval in RC Control messages in milliseconds
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(20)]
        public int RCInterval {
            get;
            set;
        }
        /// <summary>
        /// The interval between telemetry submissions in milliseconds
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(1000)]
        public int TelemetryInterval {
            get;
            set;
        }

        public Settings() {

        }

        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(CFG_FILE, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                formatter.Serialize(stream, this);
            }
        }
        protected Settings(SerializationInfo info, StreamingContext context) {
            try { Comport = info.GetString("Comport"); } catch { }
            try { BaudRate = info.GetInt32("BaudRate"); } catch { }
            try { TargetLoopTime = info.GetInt32("TargetLoopTime"); } catch { }
            try { RCInterval = info.GetInt32("RCInterval"); } catch { }
            try { TelemetryInterval = info.GetInt32("TelemetryInterval"); } catch { }
        }
        [SecurityPermissionAttribute(SecurityAction.Demand,
        SerializationFormatter = true)]

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Comport", Comport);
            info.AddValue("BaudRate", BaudRate);
            info.AddValue("TargetLoopTime", BaudRate);
            info.AddValue("RCInterval", BaudRate);
            info.AddValue("TelemetryInterval", BaudRate);
        }
    }
}
