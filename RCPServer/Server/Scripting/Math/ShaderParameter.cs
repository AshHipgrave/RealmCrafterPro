using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Math
{
    /// <summary>
    /// Shader Parameter value structure
    /// </summary>
    public struct ShaderParameter
    {
        Vector4 dataHeld;
        ShaderParameterType parameterType;

        /// <summary>
        /// Constructor for 1D vector (float).
        /// </summary>
        /// <param name="x"></param>
        public ShaderParameter(float x)
        {
            dataHeld = new Vector4(x, 0, 0, 0);
            parameterType = ShaderParameterType.Float;
        }

        /// <summary>
        /// Constructor for 2D vector.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ShaderParameter(float x, float y)
        {
            dataHeld = new Vector4(x, y, 0, 0);
            parameterType = ShaderParameterType.Vector2;
        }

        /// <summary>
        /// Constructor for 3D vector.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public ShaderParameter(float x, float y, float z)
        {
            dataHeld = new Vector4(x, y, z, 0);
            parameterType = ShaderParameterType.Vector3;
        }

        /// <summary>
        /// Constructor for 4D vector.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public ShaderParameter(float x, float y, float z, float w)
        {
            dataHeld = new Vector4(x, y, z, w);
            parameterType = ShaderParameterType.Vector4;
        }

        /// <summary>
        /// Constructor for parameter related to time.
        /// </summary>
        /// <remarks>
        /// The time based constructor makes the client carry out timing calculations for the shader.
        /// 
        /// The 'Time' type will begin counting from 0 endlessly.
        /// The 'TimeReset' type will reset the value to resetTo after endTime is reached.
        /// The 'TimeLoop' type will reset the value to 0 after endTime is reached.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="endTime"></param>
        /// <param name="resetTo"></param>
        public ShaderParameter(ShaderParameterType type, float endTime, float resetTo)
        {
            if (type != ShaderParameterType.Time
                && type != ShaderParameterType.TimeReset
                && type != ShaderParameterType.TimeLoop)
                throw new ArgumentException("Input enumeration 'type' must be of type Time, TimeReset or TimeLoop for ShaderParameter(ShaderParameterType, float, float)");

            parameterType = type;
            dataHeld = new Vector4(endTime, resetTo, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public ShaderParameter(ShaderParameterType type, float x, float y, float z, float w)
        {
            parameterType = type;
            dataHeld = new Vector4(x, y, z, w);
        }


        /// <summary>
        /// Get parameter data as float.
        /// </summary>
        /// <returns></returns>
        public float GetParameterFloat()
        {
            return dataHeld.X;
        }

        /// <summary>
        /// Get parameter data as Vector2.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetParameterVector2()
        {
            return new Vector2(dataHeld.X, dataHeld.Y);
        }

        /// <summary>
        /// Get parameter data as Vector3.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetParameterVector3()
        {
            return new Vector3(dataHeld.X, dataHeld.Y, dataHeld.Z);
        }

        /// <summary>
        /// Get parameter data as Vector4.
        /// </summary>
        /// <returns></returns>
        public Vector4 GetParameterVector4()
        {
            return dataHeld;
        }

        /// <summary>
        /// Get the type of this parameter.
        /// </summary>
        /// <returns></returns>
        public ShaderParameterType GetParameterType()
        {
            return parameterType;
        }
    }
}
