using ERTS.Dashboard.Communication;
using CRCLib;

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
        /// CRCLib class (holds tons of tables for CRC calculation)
        /// </summary>
        static public crclib crc;
        static public void InitCRC()
        {
            crc = new crclib();
        }
        /// <summary>
        /// GUI visualization
        /// </summary>
        //static public Visualization vis;
        /// <summary>
        /// Controller class
        /// </summary>
        //static public Controller ctr;
        /// <summary>
        /// Misc data bindings structure used for visualization
        /// </summary>
        //public static Databindings db = new Databindings();
		
		//public static Observer obsvr;		
    }
}
