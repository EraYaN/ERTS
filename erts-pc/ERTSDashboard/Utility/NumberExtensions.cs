using System;

namespace ERTS.Dashboard.Utility
{
    public static class NumberExtensions
    {
        /// <summary>
		/// This methods makes sure the value lies between the min and max parameters
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="val">The value</param>
		/// <param name="min">The minimum value</param>
		/// <param name="max">The maximum value</param>
		/// <returns>The clamped value</returns>
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        /// <summary>
		/// This methods make the value when between upperBound and lowerBound
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="val">The value</param>
		/// <param name="lowerBound">The lower limit of the deadzone</param>
		/// <param name="upperBound">The upper limit of the deadzone</param>
		/// <returns>The value with the deadzone applied</returns>
		public static T Deadzone<T>(this T val, T lowerBound, T upperBound) where T : IComparable<T>
        {
            dynamic r = 0;
            if (val.CompareTo(upperBound) < 0 && val.CompareTo(lowerBound) > 0) return r;
            else return val;
        }

        /// <summary>
        /// Converts a little endian byte sequence to a single precision floating point array
        /// </summary>
        /// <param name="array">Source byte array</param>
        /// <returns></returns>
        public static float[] ConvertByteToFloat(byte[] array)
        {
            float[] floatArr = new float[array.Length / 4];
            for (int i = 0; i < floatArr.Length; i++)
            {
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(array, i * 4, 4);
                }
                floatArr[i] = BitConverter.ToSingle(array, i * 4);
            }
            return floatArr;
        }
    }
}
