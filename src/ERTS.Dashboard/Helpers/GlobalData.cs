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
            if (File.Exists("cfg.bin"))
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    using (Stream stream = new FileStream("cfg.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        cfg = (Settings)formatter.Deserialize(stream);
                    }
                }
                catch
                {
                    cfg = new Settings();
                }
                finally
                {
                }
            }
            else
            {
                cfg = new Settings();
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
                if (!String.IsNullOrEmpty(cfg.Comport) && cfg.BaudRate > 0)
                {
                    com = new CommunicationInterface(cfg.Comport, cfg.BaudRate);
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
                    Debug.WriteLine("COM Port or Baud Rate not valid. OPen Configuration and restart.");
                    return false;
                }
            }
            else
            {
                Debug.WriteLine("CRCLib and Configuration must exist before starting the CommunicationInterface.");
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
                Debug.WriteLine("PatchBox, InputManager and CommunicationInterface must all exist before starting the Controller.");
                return false;
            }
        }
        static public void InitStageOne(IntPtr WindowHandle)
        {
            if (InitConfiguration())
                Debug.WriteLine("Started Configuration.");
            else
                Debug.WriteLine("Starting Configuration failed.");
            if (InitCRCLib())
                Debug.WriteLine("Started CRCLib.");
            else
                Debug.WriteLine("Starting CRCLib failed.");           
            if (InitInputManager())
                Debug.WriteLine("Started InputManager.");
            else
                Debug.WriteLine("Starting InputManager failed.");
            if (InitPatchBox())
            {
                input.AquireDevices(patchbox.DeviceGuids, WindowHandle);
                Debug.WriteLine("Started PatchBox.");
            }
            else
            {
                Debug.WriteLine("Starting PatchBox failed.");
            }
            return;
        }
        static public void InitStageTwo()
        {
            
            if (InitCommunicationInterface())
                Debug.WriteLine("Started CommunicationInterface.");
            else
                Debug.WriteLine("Starting CommunicationInterface failed.");            
            if (InitController())
                Debug.WriteLine("Started Controller.");
            else
                Debug.WriteLine("Starting Controller failed.");
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
