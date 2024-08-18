using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Scripting
{
    /// <summary>
    /// Blitz friendly binary reader.
    /// </summary>
    public class BBBinaryReader : BinaryReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public BBBinaryReader(Stream stream)
            : base(stream)
        {

        }

        /// <summary>
        /// Read a string which is prefixed by a 32-bit length header.
        /// </summary>
        /// <returns></returns>
        public string ReadBBString()
        {
            int Len = ReadInt32();
            byte[] Data = ReadBytes(Len);
            return System.Text.ASCIIEncoding.ASCII.GetString(Data);
        }

        /// <summary>
        /// Check whether the read position has reached the end of the stream.
        /// </summary>
        public bool Eof
        {
            get { return BaseStream.Position >= BaseStream.Length - 1; }
        }
    }
}
