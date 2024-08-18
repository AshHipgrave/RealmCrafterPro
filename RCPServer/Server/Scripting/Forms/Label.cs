using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Label
    /// </summary>
    public class Label : Control
    {
#region Member Variables
        TextAlign align = TextAlign.Default;
        TextAlign valign = TextAlign.Default;
        bool multiline = false;
        bool inlineStringProcessing = true;
        Math.Vector2 scissorWindow = new Math.Vector2();
        bool forceScissoring = false;
        Math.Vector2 scrollOffset = new Math.Vector2();
#endregion

#region Properties
        /// <summary>
        /// Gets or sets the Horizontal Alignment of the internal text label of this control.
        /// </summary>
        public global::Scripting.Forms.TextAlign Align
        {
            get { return align; }
            set
            {
                align = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "HALN", (byte)align));
                }
            }
        }

        /// <summary>
        /// Gets or sets the Vertical Alignment of the internal text label of this control.
        /// </summary>
        public global::Scripting.Forms.TextAlign VAlign
        {
            get { return valign; }
            set
            {
                valign = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "VALN", (byte)valign));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this label renders on multiple lines.
        /// </summary>
        public bool Multiline
        {
            get { return multiline; }
            set
            {
                multiline = value;
                if (AllocID > 0) // Has network instance
                {
                    byte Flags = 0;
                    Flags += (byte)(multiline ? 1 : 0);
                    Flags += (byte)(inlineStringProcessing ? 2 : 0);
                    Flags += (byte)(forceScissoring ? 4 : 0);
                    UpdateProperty(PropertyHelper.Create(AllocID, "FLAG", Flags));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether escape sequences for colors and effects are processed.
        /// </summary>
        public bool InlineStringProcessing
        {
            get { return inlineStringProcessing; }
            set
            {
                inlineStringProcessing = value;
                if (AllocID > 0) // Has network instance
                {
                    byte Flags = 0;
                    Flags += (byte)(multiline ? 1 : 0);
                    Flags += (byte)(inlineStringProcessing ? 2 : 0);
                    Flags += (byte)(forceScissoring ? 4 : 0);
                    UpdateProperty(PropertyHelper.Create(AllocID, "FLAG", Flags));
                }
            }
        }

        /// <summary>
        /// Gets or sets the use of a scissoring window around the control.
        /// </summary>
        public bool ForceScissoring
        {
            get { return forceScissoring; }
            set
            {
                forceScissoring = value;
                if (AllocID > 0) // Has network instance
                {
                    byte Flags = 0;
                    Flags += (byte)(multiline ? 1 : 0);
                    Flags += (byte)(inlineStringProcessing ? 2 : 0);
                    Flags += (byte)(forceScissoring ? 4 : 0);
                    UpdateProperty(PropertyHelper.Create(AllocID, "FLAG", Flags));
                }
            }
        }

        /// <summary>
        /// Gets or sets the scissor region (if ForceScissoring is enabled).
        /// </summary>
        public Math.Vector2 ScissorRegion
        {
            get { return scissorWindow; }
            set
            {
                scissorWindow = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "SCWN", scissorWindow));
                }
            }
        }

        /// <summary>
        /// Gets or sets the drawing offset for text, useful for scrolling text boxes.
        /// </summary>
        public Math.Vector2 ScrollOffset
        {
            get { return scrollOffset; }
            set
            {
                scrollOffset = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "SCOF", scrollOffset));
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
            byte Flags = 0;
            Flags += (byte)(multiline ? 1 : 0);
            Flags += (byte)(inlineStringProcessing ? 2 : 0);
            Flags += (byte)(forceScissoring ? 4 : 0);
            writer.Write(Flags);

            writer.Write(scissorWindow.X);
            writer.Write(scissorWindow.Y);

            writer.Write(scrollOffset.X);
            writer.Write(scrollOffset.Y);

            writer.Write((byte)align);
            writer.Write((byte)valign);
            
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
            byte Flags = reader.ReadByte();
            multiline = (Flags & 1) > 0;
            inlineStringProcessing = (Flags & 2) > 0;
            forceScissoring = (Flags & 4) > 0;

            scissorWindow = new Math.Vector2();
            scrollOffset = new Math.Vector2();

            scissorWindow.X = reader.ReadSingle();
            scissorWindow.Y = reader.ReadSingle();

            scrollOffset.X = reader.ReadSingle();
            scrollOffset.Y = reader.ReadSingle();

            align = (TextAlign)reader.ReadByte();
            valign = (TextAlign)reader.ReadByte();

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
            Pa.Write("CLBL", false);

            WriteInternalProperties(ref Pa);

            byte Flags = 0;
            Flags += (byte)(multiline ? 1 : 0);
            Flags += (byte)(inlineStringProcessing ? 2 : 0);
            Flags += (byte)(forceScissoring ? 4 : 0);
            Pa.Write("FLAG", false);
            Pa.Write(Flags);

            Pa.Write("SCWN", false);
            Pa.Write(scissorWindow.X);
            Pa.Write(scissorWindow.Y);

            Pa.Write("SCOF", false);
            Pa.Write(scrollOffset.X);
            Pa.Write(scrollOffset.Y);

            if (align != TextAlign.Default)
            {
                Pa.Write("HALN", false);
                Pa.Write((byte)align);
            }

            if (valign != TextAlign.Default)
            {
                Pa.Write("VALN", false);
                Pa.Write((byte)valign);
            }

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
            if (control as Label == null)
                return;

            Label B = control as Label;
            string EventName = reader.ReadString();
        }
#endregion


    }
}
