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

        /// <summary>
        /// The lower battery cutoff voltage
        /// </summary>
        [DefaultValue(1050)]
        public int BatteryThreshold {
            get;
            set;
        }

        /// <summary>
        /// The decrement of motor value per loop time while in panic mode.
        /// </summary>
        [DefaultValue(1)]
        public int PanicDecrement {
            get;
            set;
        }
        
        /// <summary>
        /// The interval in RC Control messages in milliseconds
        /// </summary>
        [DefaultValue(40)]
        public int RCInterval {
            get;
            set;
        }
        /// <summary>
        /// The divider for telemetry transmissions
        /// </summary>
        [DefaultValue(20)]
        public int TelemetryDivider {
            get;
            set;
        }
        /// <summary>
        /// The divider for logging values to flash
        /// </summary>
        [DefaultValue(10)]
        public int LogDivider {
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

        /// <summary>
        /// The deadzone for the lift axis
        /// </summary>
        [DefaultValue(1.0)]
        public double LiftDeadzone {
            get;
            set;
        }

        /// <summary>
        /// The deadzone for the roll axis
        /// </summary>
        [DefaultValue(5.0)]
        public double RollDeadzone {
            get;
            set;
        }

        /// <summary>
        /// The deadzone for the pitch axis
        /// </summary>
        [DefaultValue(5.0)]
        public double PitchDeadzone {
            get;
            set;
        }

        /// <summary>
        /// The deadzone for the yaw axis
        /// </summary>
        [DefaultValue(5.0)]
        public double YawDeadzone {
            get;
            set;
        }

        /// <summary>
        /// The start value for PYaw
        /// </summary>
        [DefaultValue(20.0)]
        public double StartPYaw {
            get;
            set;
        }

        /// <summary>
        /// The start value for PHeight
        /// </summary>
        [DefaultValue(3000.0)]
        public double StartPHeight {
            get;
            set;
        }

        /// <summary>
        /// The start value for P1RollPitch
        /// </summary>
        [DefaultValue(33.0)]
        public double StartP1RollPitch {
            get;
            set;
        }

        /// <summary>
        /// The start value for P2RollPitch
        /// </summary>
        [DefaultValue(21.0)]
        public double StartP2RollPitch {
            get;
            set;
        }

        /// <summary>
        /// The start value for PLift
        /// </summary>
        [DefaultValue(1.0)]
        public double StartPLift {
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
            if (Property == "BatteryThreshold" || Property == "")
            {
                BatteryThreshold = (int)(TypeDescriptor.GetProperties(this)["BatteryThreshold"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "PanicDecrement" || Property == "")
            {
                PanicDecrement = (int)(TypeDescriptor.GetProperties(this)["PanicDecrement"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "RCInterval" || Property == "")
            {
                RCInterval = (int)(TypeDescriptor.GetProperties(this)["RCInterval"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "TelemetryDivider" || Property == "")
            {
                TelemetryDivider = (int)(TypeDescriptor.GetProperties(this)["TelemetryDivider"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "LogDivider" || Property == "")
            {
                LogDivider = (int)(TypeDescriptor.GetProperties(this)["LogDivider"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
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
            if (Property == "LiftDeadzone" || Property == "")
            {
                LiftDeadzone = (double)(TypeDescriptor.GetProperties(this)["LiftDeadzone"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "RollDeadzone" || Property == "")
            {
                RollDeadzone = (double)(TypeDescriptor.GetProperties(this)["RollDeadzone"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "PitchDeadzone" || Property == "")
            {
                PitchDeadzone = (double)(TypeDescriptor.GetProperties(this)["PitchDeadzone"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "YawDeadzone" || Property == "")
            {
                YawDeadzone = (double)(TypeDescriptor.GetProperties(this)["YawDeadzone"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "StartPYaw" || Property == "")
            {
                StartPYaw = (double)(TypeDescriptor.GetProperties(this)["StartPYaw"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "StartPHeight" || Property == "")
            {
                StartPHeight = (double)(TypeDescriptor.GetProperties(this)["StartPHeight"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "StartP1RollPitch" || Property == "")
            {
                StartP1RollPitch = (double)(TypeDescriptor.GetProperties(this)["StartP1RollPitch"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "StartP2RollPitch" || Property == "")
            {
                StartP2RollPitch = (double)(TypeDescriptor.GetProperties(this)["StartP2RollPitch"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
            }
            if (Property == "StartPLift" || Property == "")
            {
                StartPLift = (double)(TypeDescriptor.GetProperties(this)["StartPLift"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
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
            try { BatteryThreshold = info.GetInt32("BatteryThreshold"); } catch { SetDefaults("BatteryThreshold"); }
            try { PanicDecrement = info.GetInt32("PanicDecrement"); } catch { SetDefaults("PanicDecrement"); }
            try { RCInterval = info.GetInt32("RCInterval"); } catch { SetDefaults("RCInterval"); }
            try { TelemetryDivider = info.GetInt32("TelemetryDivider"); } catch { SetDefaults("TelemetryDivider"); }
            try { LogDivider = info.GetInt32("LogDivider"); } catch { SetDefaults("LogDivider"); }
            try { PacketCheckResendInterval = info.GetInt32("PacketCheckResendInterval"); } catch { SetDefaults("PacketCheckResendInterval"); }
            try { PacketResendInterval = info.GetInt32("PacketResendInterval"); } catch { SetDefaults("PacketResendInterval"); }
            try { PacketRetransmissionCount = info.GetInt32("PacketRetransmissionCount"); } catch { SetDefaults("PacketRetransmissionCount"); }
            try { KillAfterRetransmissionFail = info.GetBoolean("KillAfterRetransmissionFail"); } catch { SetDefaults("KillAfterRetransmissionFail"); }
            try { LiftDeadzone = info.GetDouble("LiftDeadzone"); } catch { SetDefaults("LiftDeadzone"); }
            try { RollDeadzone = info.GetDouble("RollDeadzone"); } catch { SetDefaults("RollDeadzone"); }
            try { PitchDeadzone = info.GetDouble("PitchDeadzone"); } catch { SetDefaults("PitchDeadzone"); }
            try { YawDeadzone = info.GetDouble("YawDeadzone"); } catch { SetDefaults("YawDeadzone"); }
            try { StartPYaw = info.GetDouble("StartPYaw"); } catch { SetDefaults("StartPYaw"); }
            try { StartPHeight = info.GetDouble("StartPHeight"); } catch { SetDefaults("StartPHeight"); }
            try { StartP1RollPitch = info.GetDouble("StartP1RollPitch"); } catch { SetDefaults("StartP1RollPitch"); }
            try { StartP2RollPitch = info.GetDouble("StartP2RollPitch"); } catch { SetDefaults("StartP2RollPitch"); }
            try { StartPLift = info.GetDouble("StartPLift"); } catch { SetDefaults("StartPLift"); }

        }
        [SecurityPermission(SecurityAction.Demand,
        SerializationFormatter = true)]

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ComPort", ComPort);
            info.AddValue("BaudRate", BaudRate);
            info.AddValue("BatteryThreshold", BatteryThreshold);
            info.AddValue("PanicDecrement", PanicDecrement);
            info.AddValue("RCInterval", RCInterval);
            info.AddValue("TelemetryDivider", TelemetryDivider);
            info.AddValue("LogDivider", LogDivider);
            info.AddValue("PacketCheckResendInterval", PacketCheckResendInterval);
            info.AddValue("PacketResendInterval", PacketResendInterval);
            info.AddValue("PacketRetransmissionCount", PacketRetransmissionCount);
            info.AddValue("KillAfterRetransmissionFail", KillAfterRetransmissionFail);
            info.AddValue("LiftDeadzone", LiftDeadzone);
            info.AddValue("RollDeadzone", RollDeadzone);
            info.AddValue("PitchDeadzone", PitchDeadzone);
            info.AddValue("YawDeadzone", YawDeadzone);
            info.AddValue("StartPYaw", StartPYaw);
            info.AddValue("StartPHeight", StartPHeight);
            info.AddValue("StartP1RollPitch", StartP1RollPitch);
            info.AddValue("StartP2RollPitch", StartP2RollPitch);
            info.AddValue("StartPLift", StartPLift);
        }
    }
}
