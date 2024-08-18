using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Button
    /// </summary>
    public class ItemButton : Control
    {
        #region Member Variables
        string backgroundImage = "";
        ushort itemID = 65535;
        ushort spellID = 65535;
        ushort itemAmount = 0;
        byte timer = 0;
        bool canDragFromInventory = false;
        bool canDragFromSpells = false;
        bool canDragToInventory = false;

        /// <summary>
        /// Client Left-Clicked the control.
        /// </summary>
        public event global::Scripting.Forms.EventHandler Click;

        /// <summary>
        /// Client Right-Clicked the control.
        /// </summary>
        public event global::Scripting.Forms.EventHandler RightClick;

        /// <summary>
        /// Client dragged the contents of this box to his inventory
        /// </summary>
        public event global::Scripting.Forms.EventHandler DraggedToInventory;

        /// <summary>
        /// Client placed an item in this box from his inventory
        /// </summary>
        public event global::Scripting.Forms.EventHandler DraggedFromInventory;

        /// <summary>
        /// Client placed a spell in this box from his spell book.
        /// </summary>
        public event global::Scripting.Forms.EventHandler DraggedFromSpells;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the Timer property which represents the greyed out 'clock' style icon over the button. (0-100 range).
        /// </summary>
        public int Timer
        {
            get { return (int)timer; }
            set
            {
                int T = value;
                if (T > 100)
                    T = 100;
                if (T < 0)
                    T = 0;

                timer = (byte)T;
                if (AllocID > 0)
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "TIME", timer));
                }
            }
        }

        /// <summary>
        /// Gets or Sets the path to the texture file used as a background to this button.
        /// </summary>
        public string BackgroundImage
        {
            get { return backgroundImage; }
            set
            {
                backgroundImage = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "BGIM", backgroundImage));
                }
            }
        }

        /// <summary>
        /// Gets or Sets the ID of the item currently held in this control (is 65535 if a spell is held).
        /// </summary>
        public ushort ItemID
        {
            get { return itemID; }
            set
            {
                itemID = value;
                spellID = 65535;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "ITID", itemID));
                }
            }
        }

        /// <summary>
        /// Gets Item class for Item held in this ItemButton. Returns null if no Item is placed in it. 
        /// </summary>
        public Item Item
        {
            get
            {
                foreach (Item It in Item.DefaultItems)
                {
                    if (It.IDNum == ItemID)
                        return It;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or Sets the amount of items held in this control (is 0 if a spell is held).
        /// </summary>
        public ushort ItemAmount
        {
            get { return itemAmount; }
            set
            {
                itemAmount = value;
                spellID = 65535;

                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "ITAM", itemAmount));
                }
            }
        }

        /// <summary>
        /// Gets or Sets the ID of the current spell being held (is 65535 if an item is held).
        /// </summary>
        public ushort SpellID
        {
            get { return spellID; }
            set
            {
                spellID = value;
                itemID = 65535;
                itemAmount = 0;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "SPELL", spellID));
                }
            }
        }

        /// <summary>
        /// Gets or Sets whether the client can drag an item from this control to their inventory
        /// </summary>
        public bool CanDragToInventory
        {
            get { return canDragToInventory; }
            set
            {
                canDragToInventory = value;
                if (AllocID > 0) // Has network instance
                {
                    byte Flags = 0;
                    Flags += (byte)(canDragToInventory ? 1 : 0);
                    Flags += (byte)(canDragFromInventory ? 2 : 0);
                    Flags += (byte)(canDragFromSpells ? 4 : 0);
                    UpdateProperty(PropertyHelper.Create(AllocID, "FLAG", Flags));
                }
            }
        }

        /// <summary>
        /// Gets or Sets whether this control can be overridden by a placement from the client inventory.
        /// </summary>
        public bool CanDragFromInventory
        {
            get { return canDragFromInventory; }
            set
            {
                canDragFromInventory = value;
                if (AllocID > 0) // Has network instance
                {
                    byte Flags = 0;
                    Flags += (byte)(canDragToInventory ? 1 : 0);
                    Flags += (byte)(canDragFromInventory ? 2 : 0);
                    Flags += (byte)(canDragFromSpells ? 4 : 0);
                    UpdateProperty(PropertyHelper.Create(AllocID, "FLAG", Flags));
                }
            }
        }

        /// <summary>
        /// Gets or Sets whether this control can be overridden by a placement from the client spell book.
        /// </summary>
        public bool CanDragFromSpells
        {
            get { return canDragFromSpells; }
            set
            {
                canDragFromSpells = value;
                if (AllocID > 0) // Has network instance
                {
                    byte Flags = 0;
                    Flags += (byte)(canDragToInventory ? 1 : 0);
                    Flags += (byte)(canDragFromInventory ? 2 : 0);
                    Flags += (byte)(canDragFromSpells ? 4 : 0);
                    UpdateProperty(PropertyHelper.Create(AllocID, "FLAG", Flags));
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
            writer.Write(timer);
            writer.Write(itemID);
            writer.Write(itemAmount);
            writer.Write(spellID);
            writer.Write((byte)backgroundImage.Length);
            writer.Write(backgroundImage, false);
            byte Flags = 0;
            Flags += (byte)(canDragToInventory ? 1 : 0);
            Flags += (byte)(canDragFromInventory ? 2 : 0);
            Flags += (byte)(canDragFromSpells ? 4 : 0);
            writer.Write(Flags);

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
            timer = reader.ReadByte();
            itemID = reader.ReadUInt16();
            itemAmount = reader.ReadUInt16();
            spellID = reader.ReadUInt16();
            backgroundImage = reader.ReadString(reader.ReadByte());

            byte Flags = reader.ReadByte();
            canDragToInventory = ((Flags & 1) > 0);
            canDragFromInventory = ((Flags & 2) > 0);
            canDragFromSpells = ((Flags & 4) > 0);

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
            Pa.Write("ZITB", false);

            WriteInternalProperties(ref Pa);

            if (timer > 0)
            {
                Pa.Write("TIME", false);
                Pa.Write(timer);
            }

            if (backgroundImage.Length > 0)
            {
                Pa.Write("BGIM", false);
                Pa.Write(backgroundImage, true);
            }

            if (itemID != 65535)
            {
                Pa.Write("ITEM", false);
                Pa.Write(itemID);
                Pa.Write(itemAmount);
            }
            else if (spellID != 65535)
            {
                Pa.Write("SPELL", false);
                Pa.Write(spellID);
            }

            byte Flags = 0;
            Flags += (byte)(canDragToInventory ? 1 : 0);
            Flags += (byte)(canDragFromInventory ? 2 : 0);
            Flags += (byte)(canDragFromSpells ? 4 : 0);

            Pa.Write("FLAG", false);
            Pa.Write(Flags);

            if (Controls.Count > 0)
            {
                Pa.Write("CHLD", false);
                Pa.Write((ushort)Controls.Count);

                foreach (Control C in Controls)
                {
                    PacketWriter PaC = C.ControlPropertyPacket(false);
                    byte[] Buff = PaC.ToArray();
                    Pa.Write(Buff.Length);
                    Pa.Write(Buff);
                }
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
            if (control as ItemButton == null)
                return;

            ItemButton B = control as ItemButton;
            string EventName = reader.ReadString(4);

            if (EventName.Equals("LCLK") && B.Click != null)
                B.Click.Invoke(control, new FormEventArgs());
            if (EventName.Equals("RCLK") && B.RightClick != null)
                B.RightClick.Invoke(control, new FormEventArgs());
            if (B.canDragToInventory && EventName.Equals("TOIN") && B.DraggedToInventory != null)
                B.DraggedToInventory.Invoke(control, new FormEventArgs());

            if (B.canDragFromInventory && EventName.Equals("FRIN"))
            {
                B.itemID = reader.ReadUInt16();
                B.itemAmount = reader.ReadUInt16();
                B.spellID = 65535;

                if (B.DraggedFromInventory != null)
                    B.DraggedFromInventory.Invoke(control, new FormEventArgs());
            }

            if (B.canDragFromSpells && EventName.Equals("FRSP"))
            {
                B.itemID = 65535;
                B.itemAmount = 0;
                B.spellID = reader.ReadUInt16();

                if (B.DraggedFromSpells != null)
                    B.DraggedFromSpells.Invoke(control, new FormEventArgs());
            }
        }
        #endregion


    }
}
