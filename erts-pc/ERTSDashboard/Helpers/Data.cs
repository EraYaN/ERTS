using ERTS.Dashboard.Communication;

namespace ERTS.Dashboard {
    /// <summary>
    /// The static Data class.
    /// </summary>
    public static class Data
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
        static public InputManager input = new InputManager();
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
        public static Databindings db = new Databindings();
		
		//public static Observer obsvr;		
    }
}
