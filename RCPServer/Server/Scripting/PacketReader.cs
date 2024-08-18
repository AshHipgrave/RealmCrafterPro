using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Helper class to read through a byte array. This is generally used with network packets but is also useful to read stored binary streams.
    /// </summary>
    public class PacketReader
    {
        private byte[] data = null;
        private int location = 0;

        /// <summary>
        /// Constructor with originating byte data to read from.
        /// </summary>
        /// <remarks>
        /// The data sent to the constructor is copied.
        /// </remarks>
        /// <param name="packet"></param>
        public PacketReader(byte[] packet)
        {
            // Copy input data to internal array
            data = new byte[packet.Length];
            packet.CopyTo(data, 0);
        }

        /// <summary>
        /// Get the internal data handle of this object.
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            return data;
        }

        /// <summary>
        /// Read a 32-bit signed integer.
        /// </summary>
        /// <returns></returns>
        public Int32 ReadInt32()
        {
            location += 4;
            return BitConverter.ToInt32(data, location - 4);
        }

        /// <summary>
        /// Read a 32-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public UInt32 ReadUInt32()
        {
            location += 4;
            return BitConverter.ToUInt32(data, location - 4);
        }

        /// <summary>
        /// Read a 32-bit floating point number.
        /// </summary>
        /// <returns></returns>
        public Single ReadSingle()
        {
            location += 4;
            return BitConverter.ToSingle(data, location - 4);
        }

        /// <summary>
        /// Read a 16-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public UInt16 ReadUInt16()
        {
            location += 2;
            return BitConverter.ToUInt16(data, location - 2);
        }

        /// <summary>
        /// Read an 8-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            location += 1;
            return data[location - 1];
        }

        /// <summary>
        /// Read an 8-bit signed integer.
        /// </summary>
        /// <returns></returns>
        public sbyte ReadSByte()
        {
            location += 1;
            return (sbyte)(data[location - 1]);
        }



        /// <summary>
        /// Read a byte array of the given length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int length)
        {
            byte[] Bytes = new byte[length];
            for (int i = 0; i < length; ++i)
                Bytes[i] = data[i + location];
            location += length;

            return Bytes;
        }

        /// <summary>
        /// Read a BB string (prefixed with a 32-bit integer).
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            int Length = ReadInt32();
            location += Length;
            return ASCIIEncoding.ASCII.GetString(data, location - Length, Length);
        }

        /// <summary>
        /// Read an ASCII string of the given length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ReadString(int length)
        {
            location += length;
            return ASCIIEncoding.ASCII.GetString(data, location - length, length);
        }

        /// <summary>
        /// Read a sector vector
        /// </summary>
        /// <returns></returns>
        public Math.SectorVector ReadSectorVector()
        {
            ushort SectorX = ReadUInt16();
            ushort SectorZ = ReadUInt16();
            float X = ReadSingle();
            float Y = ReadSingle();
            float Z = ReadSingle();

            return new Math.SectorVector(SectorX, SectorZ, X, Y, Z);
        }

        /// <summary>
        /// Peek the next integer without advancing the read location.
        /// </summary>
        /// <returns></returns>
        public Int32 PeekInt32()
        {
            return BitConverter.ToInt32(data, location);
        }

        /// <summary>
        /// Peek the next float without advancing the read location.
        /// </summary>
        /// <returns></returns>
        public Single PeekSingle()
        {
            return BitConverter.ToSingle(data, location);
        }

        /// <summary>
        /// Peek the next integer without advancing the read location.
        /// </summary>
        /// <returns></returns>
        public UInt16 PeekUInt16()
        {
            return BitConverter.ToUInt16(data, location);
        }

        /// <summary>
        /// Peek the next integer without advancing the read location.
        /// </summary>
        /// <returns></returns>
        public UInt32 PeekUInt32()
        {
            return BitConverter.ToUInt32(data, location);
        }

        /// <summary>
        /// Peek the next integer without advancing the read location.
        /// </summary>
        /// <returns></returns>
        public byte PeekByte()
        {
            return data[location];
        }

        /// <summary>
        /// Peek the next integer without advancing the read location.
        /// </summary>
        /// <returns></returns>
        public sbyte PeekSByte()
        {
            return (sbyte)(data[location]);
        }

        /// <summary>
        /// Peek the next string without advancing the read location.
        /// </summary>
        /// <returns></returns>
        public string PeekString()
        {
            int Length = ReadInt32();
            return ASCIIEncoding.ASCII.GetString(data, location, Length);
        }

        /// <summary>
        /// Peek the next string without advancing the read location.
        /// </summary>
        /// <returns></returns>
        public string PeekString(int length)
        {
            return ASCIIEncoding.ASCII.GetString(data, location, length);
        }

        /// <summary>
        /// Get or Set the read location.
        /// </summary>
        public int Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// Get the length of readable data.
        /// </summary>
        public int Length
        {
            get { return data.Length; }
        }

    }
}
