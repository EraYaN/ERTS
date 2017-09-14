using ERTS.Dashboard.Communication;
using CRCLib;
using ERTS.Dashboard.Input;
using ERTS.Dashboard.Control;

namespace ERTS.Dashboard {
    /// <summary>
    /// The static GlobalData class.
    /// </summary>
    public static class GlobalData
    {
		/// <summary>
		/// Configuration unit
		/// </summary>
		static public Settings cfg;
        /// <summary>
        /// Serial communication interface
        /// </summary>
        static public CommunicationInterface com;
        /// <summary>
        /// Input manager class
        /// </summary>
        static public InputManager input;
        static public void InitInputManager()
        {
            input = new InputManager();
            input.StartThread();
        }
        /// <summary>
        /// Connects all inputs to the controller and other modules.
        /// </summary>
        static public PatchBox patchbox;
        static public void InitPatchBox()
        {
            patchbox = new PatchBox();
        }
        /// <summary>
        /// CRCLib class (holds tons of tables for CRC calculation)
        /// </summary>
        static public crclib crc;
        static public void InitCRC()
        {
            crc = new crclib();
        }
        /// <summary>
        /// Controller class, implements all timers and behaviour of the PC side of the control software.
        /// </summary>
        static public Controller ctr;
        static public void InitController()
        {
            ctr = new Controller();
            return;
        }
        /// <summary>
        /// Misc data bindings structure used for visualization
        /// </summary>
        //public static Databindings db = new Databindings();
        
        static public void Dispose()
        {
            if (cfg != null)
                cfg = null;
            if (com != null)
                com.Dispose();
            if (input != null)
                input.Dispose();
            if (patchbox != null)
                patchbox = null;
            if (crc != null)
                crc = null;
            if (ctr != null)
                ctr.Dispose();
            return;
        }
    }
}
