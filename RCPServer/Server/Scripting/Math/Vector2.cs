using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Math
{
    /// <summary>
    /// Three Dimensional Vector
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// Internal data
        /// </summary>
        private float x, y;

        /// <summary>
        /// Get the X component
        /// </summary>
        public float X { get { return x; } set { x = value; } }

        /// <summary>
        /// Get the Y component
        /// </summary>
        public float Y { get { return y; } set { y = value; } }

        /// <summary>
        /// Construct a new vector with default parameters
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public Vector2(float X, float Y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Construct a new vector with default parameters
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public Vector2(double X, double Y)
        {
            x = (float)X;
            y = (float)Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return x.ToString() + ", " + y.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector2 Zero = new Vector2(0, 0);
    }
}
