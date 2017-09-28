using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;

namespace ERTS.Dashboard.Configuration
{
    //TODO Implementent this based on actual application settings. (Configuration.settings file)
    [Serializable]
    public class Settings : ISerializable
    {
        public const string CFG_FILE = "cfg.bin";
        [DefaultValue("COM3")]
        public string ComPort {
            get;
            set;
        }
        [DefaultValue(115200)]
        public int BaudRate {
            get;
            set;
        }
        //The target loop time on the Embedded System in microseconds
        [DefaultValue(1000)]
        public int TargetLoopTime {
            get;
            set;
        }
        /// <summary>
        /// The interval in RC Control messages in milliseconds
        /// </summary>
        [DefaultValue(20)]
        public int RCInterval {
            get;
            set;
        }
        /// <summary>
        /// The interval between telemetry submissions in milliseconds
        /// </summary>
        [DefaultValue(1000)]
        public int TelemetryInterval {
            get;
            set;
        }
        /// <summary>
        /// The interval between checking for needed retransmissions in milliseconds
        /// </summary>
        [DefaultValue(50)]
        public int PacketCheckResendInterval {
            get;
            set;
        }
        /// <summary>
        /// The interval between packet retransmissions in milliseconds
        /// </summary>
        [DefaultValue(500)]
        public int PacketResendInterval {
            get;
            set;
        }
        /// <summary>
        /// The amount of times a packet should be retransmitted.
        /// </summary>
        [DefaultValue(50)]
        public int PacketRetransmissionCount {
            get;
            set;
        }

        /// <summary>
        /// Setting to kill the quad after 50 transmission fails.
        /// </summary>
        [DefaultValue(true)]
        public bool KillAfterRetransmissionFail {
            get;
            set;
        }

        public void SetDefaults(String Property = "")
        {
            if (Property == "ComPort" || Property == "")
            {
                ComPort = (String)(TypeDescriptor.GetProperties(this)["ComPort"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "BaudRate" || Property == "")
            {
                BaudRate = (int)(TypeDescriptor.GetProperties(this)["BaudRate"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "TargetLoopTime" || Property == "")
            {
                TargetLoopTime = (int)(TypeDescriptor.GetProperties(this)["TargetLoopTime"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "RCInterval" || Property == "")
            {
                RCInterval = (int)(TypeDescriptor.GetProperties(this)["RCInterval"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "TelemetryInterval" || Property == "")
            {
                TelemetryInterval = (int)(TypeDescriptor.GetProperties(this)["TelemetryInterval"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "PacketCheckResendInterval" || Property == "")
            {
                PacketCheckResendInterval = (int)(TypeDescriptor.GetProperties(this)["PacketCheckResendInterval"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "PacketResendInterval" || Property == "")
            {
                PacketResendInterval = (int)(TypeDescriptor.GetProperties(this)["PacketResendInterval"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "PacketRetransmissionCount" || Property == "")
            {
                PacketResendInterval = (int)(TypeDescriptor.GetProperties(this)["PacketRetransmissionCount"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "KillAfterRetransmissionFail" || Property == "")
            {
                KillAfterRetransmissionFail = (bool)(TypeDescriptor.GetProperties(this)["KillAfterRetransmissionFail"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
        }

        public Settings(bool defaults = false)
        {
            if(defaults)
                SetDefaults();
        }

        public void Save()
        {
            System.Diagnostics.Debug.WriteLine("Saving configuration to disk", "Settings");
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(CFG_FILE, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                formatter.Serialize(stream, this);
                System.Diagnostics.Debug.WriteLine("Saved configuration to disk.", "Settings");
            }
        }
        protected Settings(SerializationInfo info, StreamingContext context)
        {
            try { ComPort = info.GetString("ComPort"); } catch { SetDefaults("ComPort"); }
            try { BaudRate = info.GetInt32("BaudRate"); } catch { SetDefaults("BaudRate"); }
            try { TargetLoopTime = info.GetInt32("TargetLoopTime"); } catch { SetDefaults("TargetLoopTime"); }
            try { RCInterval = info.GetInt32("RCInterval"); } catch { SetDefaults("RCInterval"); }
            try { TelemetryInterval = info.GetInt32("TelemetryInterval"); } catch { SetDefaults("TelemetryInterval"); }
            try { PacketCheckResendInterval = info.GetInt32("PacketCheckResendInterval"); } catch { SetDefaults("PacketCheckResendInterval"); }
            try { PacketResendInterval = info.GetInt32("PacketResendInterval"); } catch { SetDefaults("PacketResendInterval"); }
            try { PacketRetransmissionCount = info.GetInt32("PacketRetransmissionCount"); } catch { SetDefaults("PacketRetransmissionCount"); }
            try { KillAfterRetransmissionFail = info.GetBoolean("KillAfterRetransmissionFail"); } catch { SetDefaults("KillAfterRetransmissionFail"); }

        }
        [SecurityPermissionAttribute(SecurityAction.Demand,
        SerializationFormatter = true)]

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ComPort", ComPort);
            info.AddValue("BaudRate", BaudRate);
            info.AddValue("TargetLoopTime", TargetLoopTime);
            info.AddValue("RCInterval", RCInterval);
            info.AddValue("TelemetryInterval", TelemetryInterval);
            info.AddValue("PacketCheckResendInterval", PacketCheckResendInterval);
            info.AddValue("PacketResendInterval", PacketResendInterval);
            info.AddValue("PacketRetransmissionCount", PacketRetransmissionCount);
            info.AddValue("KillAfterRetransmissionFail", KillAfterRetransmissionFail);
        }
    }
}
