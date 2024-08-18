using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RCPServer
{
    public class FileChunk
    {
        public BinaryReader Reader;
        public string Name;
        public byte Version;
        public long Offset;
        public long Length;

        public void Skip()
        {
            Reader.BaseStream.Position = Offset + Length;
        }

        public void GoTo()
        {
            Reader.BaseStream.Position = Offset;
        }

        public static FileChunk ReadChunk(BinaryReader f)
        {
            FileChunk Chunk = new FileChunk();

            Chunk.Reader = f;
            Chunk.Offset = f.BaseStream.Position + 9;
            Chunk.Name = Encoding.ASCII.GetString(f.ReadBytes(4));
            Chunk.Version = f.ReadByte();
            Chunk.Length = f.ReadInt32();

            return Chunk;
        }
    }
}
