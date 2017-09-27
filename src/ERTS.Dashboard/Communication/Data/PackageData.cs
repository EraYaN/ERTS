using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication.Data
{
    //Implemented all Parameter classes.
    public abstract class PackageData
    {
        public PackageData()
        {        
        }
        public PackageData(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Length in bytes
        /// </summary>
        public abstract int Length {
            get;
        }
        /// <summary>
        /// Length in bytes
        /// </summary>
        public abstract bool ExpectsAcknowledgement {
            get;
        }
        /// <summary>
        /// Set the number to be used for acknowledgement tracking.
        /// </summary>
        public abstract void SetAckNumber(uint AckNumber);
        /// <summary>
        /// Build the binary representation of this data with <see cref="Length"/>.
        /// </summary>
        /// <returns>Byte array representing this data.</returns>
        public abstract byte[] ToByteArray();
        /// <summary>
        /// Checks if the data is valid.
        /// </summary>
        public abstract bool IsValid();
        
    }
}
