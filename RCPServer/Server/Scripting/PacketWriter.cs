using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Helper class to concatenate binary data for the purpose of sending packets or eventually storing raw data.
    /// 
    /// The ToArray() Method will obtain a byte array of the contained data.
    /// </summary>
    /// <remarks>
    /// If a PacketWriter is used for a packet which has multiple destinations, call ToArray() once before transmission and operate
    /// only on the returned data; this will prevent the class instancing multiple arrays.
    /// </remarks>
    public class PacketWriter
    {
        private byte[] data = new byte[1024];
        private int location = 0;
        private int size = 0;

        /// <summary>
        /// Check that there is enough memory to perform a write operation and expand automatically on failure.
        /// </summary>
        /// <param name="length">Size of data that will be written.</param>
        protected void VerifyWriteSize(int length)
        {
            // Room enough to poke value
            if (location + length < data.Length)
                return;

            // Not enough room, increase size by 1024 bytes
            byte[] NewData = new byte[data.Length + 1024];
            data.CopyTo(NewData, 0);
            data = NewData;

            // Recurse incase the length is bigger than the 1024 that we just added
            VerifyWriteSize(length);
        }

        /// <summary>
        /// 
        /// </summary>
        public PacketWriter()
        {

        }

        /// <summary>
        /// Reset write points to 'empty' packet.
        /// </summary>
        public void Clear()
        {
            location = 0;
            size = 0;
        }

        /// <summary>
        /// Write a signed 32-bit integer.
        /// </summary>
        /// <param name="value"></param>
        public void Write(Int32 value)
        {
            VerifyWriteSize(4);

            BitConverter.GetBytes(value).CopyTo(data, location);
            location += 4;
            size += 4;
        }

        /// <summary>
        /// Write an unsigned 32-bit integer.
        /// </summary>
        /// <param name="value"></param>
        public void Write(UInt32 value)
        {
            VerifyWriteSize(4);

            BitConverter.GetBytes(value).CopyTo(data, location);
            location += 4;
            size += 4;
        }

        /// <summary>
        /// Write a floating point number.
        /// </summary>
        /// <param name="value"></param>
        public void Write(float value)
        {
            VerifyWriteSize(4);

            BitConverter.GetBytes(value).CopyTo(data, location);
            location += 4;
            size += 4;
        }

        /// <summary>
        /// Write an unsigned 16-bit integer (ushort).
        /// </summary>
        /// <param name="value"></param>
        public void Write(UInt16 value)
        {
            VerifyWriteSize(2);

            BitConverter.GetBytes(value).CopyTo(data, location);
            location += 2;
            size += 2;
        }

        /// <summary>
        /// Write an unsigned 8-bit integer (byte).
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte value)
        {
            VerifyWriteSize(1);

            BitConverter.GetBytes(value).CopyTo(data, location);
            location += 1;
            size += 1;
        }

        /// <summary>
        /// Write a signed 8-bit integer (sbyte).
        /// </summary>
        /// <param name="value"></param>
        public void Write(sbyte value)
        {
            VerifyWriteSize(1);

            BitConverter.GetBytes(value).CopyTo(data, location);
            location += 1;
            size += 1;
        }

        /// <summary>
        /// Write a string.
        /// </summary>
        /// <remarks>
        /// useBBLength will prefix the string with a 32-bit signed integer representing the length of the internal data.
        /// 
        /// Data is written as ASCII.
        /// </remarks>
        /// <param name="value"></param>
        /// <param name="useBBLength">Prefix data with length.</param>
        public void Write(string value, bool useBBLength)
        {
            if(useBBLength)
                Write(value.Length);

            VerifyWriteSize(value.Length);

            ASCIIEncoding.ASCII.GetBytes(value).CopyTo(data, location);
            location += value.Length;
            size += value.Length;
        }

        /// <summary>
        /// Write byte array.
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        /// <summary>
        /// Write a byte array from an offset within the array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public void Write(byte[] value, int offset)
        {
            Write(value, offset, value.Length - offset);
        }


        /// <summary>
        /// Write a byte array from an offset with a variable length.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Write(byte[] value, int offset, int length)
        {
            VerifyWriteSize(length);

            for (int i = 0; i < length; ++i)
            {
                data[i + location] = value[i + offset];
            }

            location += length;
            size += length;
        }

        /// <summary>
        /// Write a SectorVector
        /// </summary>
        /// <param name="vector"></param>
        public void Write(Math.SectorVector vector)
        {
            VerifyWriteSize(16);
            Write(vector.SectorX);
            Write(vector.SectorZ);
            Write(vector.X);
            Write(vector.Y);
            Write(vector.Z);
        }

        /// <summary>
        /// Convert the internal data to a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            byte[] Out = new byte[size];
            for (int i = 0; i < size; ++i)
                Out[i] = data[i];

            return Out;
        }

        /// <summary>
        /// Get or Set the write location on the memory stream.
        /// </summary>
        public int Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// Get the length of the internal data.
        /// </summary>
        public int Length
        {
            get { return size; }
        }
    }
}
