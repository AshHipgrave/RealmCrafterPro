using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Forms
{
    /// <summary>
    /// Property Helped to assist in network property transmission.
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, int value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write(value);

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, ushort value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write(value);

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, float value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write(value);

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, string value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write(value, true);

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, bool value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write((byte)(value ? 1 : 0));

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, byte value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write(value);

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, System.Drawing.Color value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write(value.A);
            Writer.Write(value.R);
            Writer.Write(value.G);
            Writer.Write(value.B);

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, global::Scripting.Math.Vector2 value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write(value.X);
            Writer.Write(value.Y);

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="postype"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, global::Scripting.Forms.PositionType postype, global::Scripting.Math.Vector2 value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write((byte)postype);
            Writer.Write(value.X);
            Writer.Write(value.Y);

            return Writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocID"></param>
        /// <param name="name"></param>
        /// <param name="sizetype"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PacketWriter Create(int allocID, string name, global::Scripting.Forms.SizeType sizetype, global::Scripting.Math.Vector2 value)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Write(allocID);
            Writer.Write(name, false);
            Writer.Write((byte)sizetype);
            Writer.Write(value.X);
            Writer.Write(value.Y);

            return Writer;
        }
    }
}
