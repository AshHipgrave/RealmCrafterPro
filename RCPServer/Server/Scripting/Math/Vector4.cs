using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Math
{
    /// <summary>
    /// Four Dimensional Vector
    /// </summary>
    public struct Vector4
    {
        /// <summary>
        /// Internal data
        /// </summary>
        private float x, y, z, w;

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
        /// Get the W component
        /// </summary>
        public float W { get { return w; } set { w = value; } }

        /// <summary>
        /// Construct a new vector with default parameters
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="W"></param>
        public Vector4(float X, float Y, float Z, float W)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }

        /// <summary>
        /// Construct a new vector with default parameters
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="W"></param>
        public Vector4(double X, double Y, double Z, double W)
        {
            x = (float)X;
            y = (float)Y;
            z = (float)Z;
            w = (float)W;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ", " + w.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector4 Zero = new Vector4(0, 0, 0, 0);
    }
}
