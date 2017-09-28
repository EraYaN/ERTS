using ERTS.Dashboard.Communication;
using ERTS.Dashboard.Configuration;
using CRCLib;
using ERTS.Dashboard.Input;
using ERTS.Dashboard.Control;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Diagnostics;

namespace ERTS.Dashboard
{
    /// <summary>
    /// The static GlobalData class.
    /// </summary>
    public static class GlobalData
    {
        /// <summary>
        /// Configuration unit
        /// </summary>
        static public Settings cfg;
        static public bool InitConfiguration()
        {
            if (File.Exists(Settings.CFG_FILE))
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    using (Stream stream = new FileStream(Settings.CFG_FILE, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        cfg = (Settings)formatter.Deserialize(stream);
                        Debug.WriteLine("Loaded configuration from disk.", "DATA");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Configuration could not be read. Restored default settings.", "DATA");
                    cfg = new Settings(true);
                }
                finally
                {
                }
            }
            else
            {
                Debug.WriteLine("Configuration not found. Restored default settings.", "DATA");
                cfg = new Settings(true);
            }
            return cfg != null;

        }
        /// <summary>
        /// Serial communication interface
        /// </summary>
        static public CommunicationInterface com;
        static public bool InitCommunicationInterface()
        {
            if (crc != null && cfg != null)
            {
                if (!String.IsNullOrEmpty(cfg.ComPort) && cfg.BaudRate > 0)
                {
                    com = new CommunicationInterface(cfg.ComPort, cfg.BaudRate);
                    if (!com.IsOpen)
                    {
                        com.Dispose();
                        com = null;
                        return false;
                    }
                    return com.IsOpen;
                }
                else
                {
                    Debug.WriteLine("COM Port or Baud Rate not valid. Open Configuration and restart.", "DATA");
                    return false;
                }
            }
            else
            {
                Debug.WriteLine("CRCLib and Configuration must exist before starting the CommunicationInterface.", "DATA");
                return false;
            }
        }
        /// <summary>
        /// Input manager class
        /// </summary>
        static public InputManager input;
        static public bool InitInputManager()
        {
            input = new InputManager();
            input.StartThread();
            return true;
        }
        /// <summary>
        /// Connects all inputs to the controller and other modules.
        /// </summary>
        static public PatchBox patchbox;
        static public bool InitPatchBox()
        {
            if (input != null)
                patchbox = new PatchBox();
            return true;
        }
        /// <summary>
        /// CRCLib class (holds tons of tables for CRC calculation)
        /// </summary>
        static public crclib crc;
        static public bool InitCRCLib()
        {
            crc = new crclib();
            return true;
        }
        /// <summary>
        /// Controller class, implements all timers and behaviour of the PC side of the control software.
        /// </summary>
        static public Controller ctr;
        static public bool InitController()
        {
            if (patchbox != null && input != null && com != null)
            {
                ctr = new Controller();
                return true;
            }
            else
            {
                Debug.WriteLine("PatchBox, InputManager and CommunicationInterface must all exist before starting the Controller.", "DATA");
                return false;
            }
        }
        static public void InitStageOne(IntPtr WindowHandle)
        {
            if (InitConfiguration())
                Debug.WriteLine("Started Configuration.","DATA");
            else
                Debug.WriteLine("Starting Configuration failed.", "DATA");
            if (InitCRCLib())
                Debug.WriteLine("Started CRCLib.", "DATA");
            else
                Debug.WriteLine("Starting CRCLib failed.", "DATA");           
            if (InitInputManager())
                Debug.WriteLine("Started InputManager.", "DATA");
            else
                Debug.WriteLine("Starting InputManager failed.", "DATA");
            if (InitPatchBox())
            {
                input.AquireDevices(patchbox.DeviceGuids, WindowHandle);
                Debug.WriteLine("Started PatchBox.", "DATA");
            }
            else
            {
                Debug.WriteLine("Starting PatchBox failed.", "DATA");
            }
            return;
        }
        static public void InitStageTwo()
        {
            
            if (InitCommunicationInterface())
                Debug.WriteLine("Started CommunicationInterface.", "DATA");
            else
                Debug.WriteLine("Starting CommunicationInterface failed.", "DATA");            
            if (InitController())
                Debug.WriteLine("Started Controller.", "DATA");
            else
                Debug.WriteLine("Starting Controller failed.", "DATA");
            return;
        }
        static public void Dispose()
        {

            if (ctr != null)
            {
                ctr.Dispose();
                ctr = null;
            }
            if (com != null)
            {
                com.Dispose();
                com = null;
            }
            if (patchbox != null)
                patchbox = null;
            if (input != null)
            {
                input.Dispose();
                input = null;
            }
            if (crc != null)
                crc = null;
            if (cfg != null)
            {
                cfg.Save();
                cfg = null;
            }
            return;
        }
    }
}
