using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Math
{
    /// <summary>
    /// Three dimensional vector with an internal sector representation
    /// </summary>
    public struct SectorVector
    {
        /// <summary>
        /// Internal data
        /// </summary>
        private float x, y, z;
        private ushort sectorX, sectorZ;

        /// <summary>
        /// Get the X component
        /// </summary>
        public float X { get { return x; } set { x = value; FixValues(); } }

        /// <summary>
        /// Get the Y component
        /// </summary>
        public float Y { get { return y; } set { y = value; FixValues(); } }

        /// <summary>
        /// Get the Z component
        /// </summary>
        public float Z { get { return z; } set { z = value; FixValues(); } }

        /// <summary>
        /// Get the X Sector
        /// </summary>
        public ushort SectorX { get { return sectorX; } set { sectorX = value; } }

        /// <summary>
        /// Get the Z Sector
        /// </summary>
        public ushort SectorZ { get { return sectorZ; } set { sectorZ = value; } }

        /// <summary>
        /// Construct a new vector with default parameters.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public SectorVector(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
            sectorX = 0;
            sectorZ = 0;

            FixValues();
        }

        /// <summary>
        /// Construct a new vector with default sector parameter.
        /// </summary>
        /// <param name="SectorX"></param>
        /// <param name="SectorZ"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public SectorVector(ushort SectorX, ushort SectorZ, float X, float Y, float Z)
        {
            sectorX = SectorX;
            sectorZ = SectorZ;
            x = X;
            y = Y;
            z = Z;

            FixValues();
        }

        /// <summary>
        /// Fix sector values to be within the square sector range.
        /// </summary>
        public void FixValues()
        {
            if (X > 1000000) //Fix a stack overflow.
            {
                X = 0;
                Z = 0;
            }

            while (X > SectorSize)
            {
                ++sectorX;
                X -= SectorSize;
            }

            while (X < 0.0f && sectorX > 0)
            {
                --sectorX;
                X += SectorSize;
            }

            while (Z > SectorSize)
            {
                ++sectorZ;
                Z -= SectorSize;
            }

            while (Z < 0.0f && sectorZ > 0)
            {
                --sectorZ;
                Z += SectorSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SectorVector operator +(SectorVector a, SectorVector b)
        {
            return new SectorVector(
                (ushort)(a.sectorX + b.sectorX),
                (ushort)(a.sectorZ + b.sectorZ),
                a.x + b.x,
                a.y + b.y,
                a.z + b.z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SectorVector operator -(SectorVector a, SectorVector b)
        {
            int XDifference = (int)a.sectorX - (int)b.sectorX;
            int ZDifference = (int)a.sectorZ - (int)b.sectorZ;

            // Handle the unsigned error
            if (XDifference < 0)
            {
                b.x = (((float)XDifference) * SectorSize) - b.x;
                XDifference = 0;
            }

            if (ZDifference < 0)
            {
                b.z = (((float)ZDifference) * SectorSize) - b.z;
                ZDifference = 0;
            }

            return new SectorVector(
                (ushort)XDifference,
                (ushort)ZDifference,
                a.x - b.x,
                a.y - b.y,
                a.z - b.z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(SectorVector a, SectorVector b)
        {
            if (a.sectorX != b.sectorX)
                return false;

            if (a.sectorZ != b.sectorZ)
                return false;

            if (System.Math.Abs(a.x - b.x) > 0.0001f)
                return false;
            if (System.Math.Abs(a.z - b.z) > 0.0001f)
                return false;
            if (System.Math.Abs(a.y - b.y) > 0.0001f)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(SectorVector))
                return base.Equals(obj);


            SectorVector a = this;
            SectorVector b = (SectorVector)obj;

            if (a.sectorX != b.sectorX)
                return false;

            if (a.sectorZ != b.sectorZ)
                return false;

            if (System.Math.Abs(a.x - b.x) > 0.0001f)
                return false;
            if (System.Math.Abs(a.z - b.z) > 0.0001f)
                return false;
            if (System.Math.Abs(a.y - b.y) > 0.0001f)
                return false;

            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(SectorVector a, SectorVector b)
        {
            if (a.sectorX != b.sectorX)
                return true;

            if (a.sectorZ != b.sectorZ)
                return true;

            if (System.Math.Abs(a.x - b.x) > 0.0001f)
                return true;
            if (System.Math.Abs(a.z - b.z) > 0.0001f)
                return true;
            if (System.Math.Abs(a.y - b.y) > 0.0001f)
                return true;

            return false;
        }

        /// <summary>
        /// Check if two sectorised positions are within a defined square area.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="width"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public bool WithinSectorDimension(SectorVector b, float width, float depth)
        {
            SectorVector DistVec = this - b;
            float XDist = (((float)DistVec.SectorX) * SectorVector.SectorSize) + DistVec.X;
            float ZDist = (((float)DistVec.SectorZ) * SectorVector.SectorSize) + DistVec.Z;

            // If negative, then we aren't inside
            if (XDist < 0.0f || ZDist < 0.0f)
                return false;

            // If the distance from square origin is great than square dimensions, then we aren't inside
            if (XDist > width || ZDist > depth)
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return sectorX.ToString() + ":" + x.ToString() + ", " + y.ToString() + ", " + sectorZ.ToString() + ":" + z.ToString();
        }

        /// <summary>
        /// Converts object to a Vector3
        /// </summary>
        /// <returns></returns>
        public Vector3 ToVector3()
        {
            return new Vector3(x + (SectorSize * (float)sectorX), y, z + (SectorSize * (float)sectorZ));
        }

        /// <summary>
        /// 
        /// </summary>
        public static SectorVector Zero = new SectorVector(0, 0, 0);

        /// <summary>
        /// Sector size
        /// </summary>
        public const float SectorSize = 768.0f;
    }
}
