using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Button
    /// </summary>
    public class Button : Control
    {
#region Member Variables
        TextAlign align = TextAlign.Default;
        TextAlign valign = TextAlign.Default;
        bool useBorder = true;
        
        string upImage = "";
        string hoverImage = "";
        string downImage = "";

        /// <summary>
        /// Client Left-Clicked the control.
        /// </summary>
        public event global::Scripting.Forms.EventHandler Click;

        /// <summary>
        /// Client Right-Clicked the control.
        /// </summary>
        public event global::Scripting.Forms.EventHandler RightClick;
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
        /// Gets or sets whether a border is used around this control. Specifically useful for image buttons.
        /// </summary>
        public bool UseBorder
        {
            get { return useBorder; }
            set
            {
                useBorder = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "BRDR", useBorder));
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to an external image to be displayed when the mouse is not over the control.
        /// </summary>
        /// <remarks>
        /// This field is not required for default skinned elements, but its useful to display an image inside a button.
        /// </remarks>
        public string UpImage
        {
            get { return upImage; }
            set
            {
                upImage = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "UPIM", upImage));
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to an external image to be displayed when the mouse is over the control.
        /// </summary>
        /// <remarks>
        /// This field is not required for default skinned elements, but its useful to display an image inside a button.
        /// </remarks>
        public string HoverImage
        {
            get { return hoverImage; }
            set
            {
                hoverImage = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "HOIM", hoverImage));
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to an external image to be displayed when the mouse is clicking the control.
        /// </summary>
        /// <remarks>
        /// This field is not required for default skinned elements, but its useful to display an image inside a button.
        /// </remarks>
        public string DownImage
        {
            get { return downImage; }
            set
            {
                downImage = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "DOIM", downImage));
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
            writer.Write((byte)(useBorder ? 1 : 0));
            writer.Write(upImage, true);
            writer.Write(downImage, true);
            writer.Write(hoverImage, true);
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
            useBorder = reader.ReadByte() > 0;
            upImage = reader.ReadString();
            downImage = reader.ReadString();
            hoverImage = reader.ReadString();
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
            Pa.Write("CBUT", false);

            WriteInternalProperties(ref Pa);

            Pa.Write("BRDR", false);
            Pa.Write((byte)(useBorder ? 1 : 0));

            if (upImage.Length > 0)
            {
                Pa.Write("UPIM", false);
                Pa.Write(upImage, true);
            }

            if (downImage.Length > 0)
            {
                Pa.Write("DOIM", false);
                Pa.Write(downImage, true);
            }

            if (hoverImage.Length > 0)
            {
                Pa.Write("HOIM", false);
                Pa.Write(hoverImage, true);
            }

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
            Click = null;
            RightClick = null;
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
            if (control as Button == null)
                return;

            Button B = control as Button;
            string EventName = reader.ReadString(4);

            if (EventName.Equals("LCLK") && B.Click != null)
                B.Click.Invoke(control, new FormEventArgs());
            if (EventName.Equals("RCLK") && B.RightClick != null)
                B.RightClick.Invoke(control, new FormEventArgs());
        }
#endregion


    }
}
