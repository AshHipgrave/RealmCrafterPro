using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Math
{
    /// <summary>
    /// Three Dimensional Vector
    /// </summary>
    public struct Vector3
    {
        /// <summary>
        /// Internal data
        /// </summary>
        private float x, y, z;

        /// <summary>
        /// Get the X component
        /// </summary>
        public float X { get { return x; } set { x = value; } }

        /// <summary>
        /// Get the Y component
        /// </summary>
        public float Y { get { return y; } set { y = value; } }

        /// <summary>
        /// Get the Z component
        /// </summary>
        public float Z { get { return z; } set { z = value; } }

        /// <summary>
        /// Construct a new vector with default parameters
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public Vector3(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        /// <summary>
        /// Construct a new vector with default parameters
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public Vector3(double X, double Y, double Z)
        {
            x = (float)X;
            y = (float)Y;
            z = (float)Z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return x.ToString() + ", " + y.ToString() + ", " + z.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 Zero = new Vector3(0, 0, 0);
    }
}
