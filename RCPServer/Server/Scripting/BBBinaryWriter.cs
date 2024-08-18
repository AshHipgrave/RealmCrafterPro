using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Scripting
{
    /// <summary>
    /// Blitz friendly binary writer
    /// </summary>
    public class BBBinaryWriter : BinaryWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public BBBinaryWriter(Stream stream)
            : base(stream)
        {

        }

        /// <summary>
        /// Write a string prefixed with a 32-bit length header.
        /// </summary>
        /// <param name="data"></param>
        public void WriteBBString(string data)
        {
            byte[] Bytes = ASCIIEncoding.ASCII.GetBytes(data);
            Write((int)(Bytes.Length));
            Write(Bytes);
        }

    }
}
