using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Panel
    /// </summary>
    public class Panel : Control
    {
#region Member Variables
        string image = "";
        Math.Vector2 minTexCoord = new Math.Vector2(0, 0);
        Math.Vector2 maxTexCoord = new Math.Vector2(1, 1);
#endregion

#region Properties
        /// <summary>
        /// Gets or sets the path to the background image of this control.
        /// </summary>
        public string Image
        {
            get { return image; }
            set
            {
                image = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "IMAG", image));
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimal texture coordinate of the image (default: 0, 0).
        /// </summary>
        public Math.Vector2 MinTexCoord
        {
            get { return minTexCoord; }
            set
            {
                minTexCoord = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "MITX", minTexCoord));
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximal texture coordinate of the image (default: 1, 1).
        /// </summary>
        public Math.Vector2 MaxTexCoord
        {
            get { return maxTexCoord; }
            set
            {
                maxTexCoord = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "MATX", maxTexCoord));
                }
            }
        }
#endregion

#region Network Sync/Lifetime
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(PacketWriter writer)
        {
            writer.Write(image, true);
            writer.Write(minTexCoord.X);
            writer.Write(minTexCoord.Y);
            writer.Write(maxTexCoord.X);
            writer.Write(maxTexCoord.Y);

            base.Serialize(writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="master"></param>
        /// <param name="actor"></param>
        /// <param name="clientControls"></param>
        public override void Deserialize(PacketReader reader, Control master, Scripting.Actor actor, LinkedList<Control> clientControls)
        {
            minTexCoord = new Math.Vector2();
            maxTexCoord = new Math.Vector2();

            image = reader.ReadString();
            minTexCoord.X = reader.ReadSingle();
            minTexCoord.Y = reader.ReadSingle();
            maxTexCoord.X = reader.ReadSingle();
            maxTexCoord.Y = reader.ReadSingle();

            base.Deserialize(reader, master, actor, clientControls);
        }

        /// <summary>
        /// <b>DO NOT USE IN SCRIPTS.</b>
        /// </summary>
        /// <param name="isRoot"></param>
        /// <returns></returns>
        public override PacketWriter ControlPropertyPacket(bool isRoot)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write("CPIC", false);

            WriteInternalProperties(ref Pa);

            if (image.Length > 0)
            {
                Pa.Write("IMAG", false);
                Pa.Write(image, true);
            }

            Pa.Write("MITX", false);
            Pa.Write(minTexCoord.X);
            Pa.Write(minTexCoord.Y);

            Pa.Write("MATX", false);
            Pa.Write(maxTexCoord.X);
            Pa.Write(maxTexCoord.Y);

            Pa.Write("CHLD", false);
            Pa.Write((ushort)Controls.Count);

            foreach (Control C in Controls)
            {
                PacketWriter PaC = C.ControlPropertyPacket(false);
                byte[] Buff = PaC.ToArray();
                Pa.Write(Buff.Length);
                Pa.Write(Buff);
            }

            Pa.Write("ENDL", false);

            return Pa;
        }

        /// <summary>
        /// <b>DO NOT USE IN SCRIPTS.</b>
        /// </summary>
        protected override void Orphan()
        {
        }
        #endregion

        #region Event Handling
        /// <summary>
        /// <b>DO NOT USE IN SCRIPTS.</b>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="reader"></param>
        public static void ProcessEvent(Control control, PacketReader reader)
        {
            if (control as Panel == null)
                return;

            Panel B = control as Panel;
            string EventName = reader.ReadString();
        }
#endregion



    }
}
