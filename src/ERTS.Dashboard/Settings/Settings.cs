using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;

namespace ERTS.Dashboard {
    //TODO Implementent this based on actual application settings. (Settings.settings file)
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
        }
        [SecurityPermissionAttribute(SecurityAction.Demand,
        SerializationFormatter = true)]

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Comport", Comport);
            info.AddValue("BaudRate", BaudRate);           
        }
    }
}
