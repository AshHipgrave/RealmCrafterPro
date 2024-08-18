using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI ListBox
    /// </summary>
    public class ListBox : Control, IPropertyArrayBase<string>
    {
        #region Member Variables
        int selectedIndex = -1;
        List<string> items = new List<string>();
        PropertyArray<string> itemsProperty;

        /// <summary>
        /// Client selected a new item.
        /// </summary>
        public event global::Scripting.Forms.EventHandler SelectedIndexChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ListBox()
        {
            itemsProperty = new PropertyArray<string>(this, items);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the box is checked.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                if (AllocID > 0) // Has network instance
                {
                    PacketWriter Writer = PropertyHelper.Create(AllocID, "SLIN", selectedIndex);
                    UpdateProperty(Writer);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the selected item
        /// </summary>
        public string SelectedValue
        {
            get { return selectedIndex > 0 ? items[selectedIndex] : ""; }
            set
            {
                if (selectedIndex < 0)
                    throw new ArgumentOutOfRangeException("SelectedValue", "SelectedIndex was never set!");

                items[selectedIndex] = value;
                if (AllocID > 0) // Has network instance
                {
                    PacketWriter Writer = PropertyHelper.Create(AllocID, "SLVA", value);
                    UpdateProperty(Writer);
                }
            }
        }

        /// <summary>
        /// Gets the list of items
        /// </summary>
        public PropertyArray<string> Items
        {
            get { return itemsProperty; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="index"></param>
        public void UpdateIndexedProperty(List<string> items, int index)
        {
            if (AllocID > 0)
            {
                PacketWriter Writer = PropertyHelper.Create(AllocID, "ITVA", index);
                Writer.Write(items[index], true);
                UpdateProperty(Writer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="values"></param>
        public void AddProperty(List<string> items, string[] values)
        {
            PacketWriter Writer = PropertyHelper.Create(AllocID, "ADIT", values.Length);

            for (int i = 0; i < values.Length; ++i)
            {
                Writer.Write(values[i], true);
                items.Add(values[i]);
            }

            if (AllocID > 0)
            {
                UpdateProperty(Writer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="index"></param>
        public void RemoveProperty(List<string> items, int index)
        {
            items.RemoveAt(index);

            if (selectedIndex >= 0 && selectedIndex >= index)
                --selectedIndex;

            if (AllocID > 0)
            {
                PacketWriter Writer = PropertyHelper.Create(AllocID, "DLIT", index);
                UpdateProperty(Writer);
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
            writer.Write((ushort)items.Count);

            foreach (string S in items)
                writer.Write(S, true);

            writer.Write((ushort)selectedIndex);

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
            ushort Count = reader.ReadUInt16();

            items.Clear();
            for (int i = 0; i < Count; ++i)
                items.Add(reader.ReadString());

            selectedIndex = reader.ReadUInt16();

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
            Pa.Write("CLIS", false);

            WriteInternalProperties(ref Pa);

            Pa.Write("ADIT", false);
            Pa.Write(items.Count);

            foreach (string S in items)
            {
                Pa.Write(S, true);
            }

            Pa.Write("SLIN", false);
            Pa.Write(selectedIndex);

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
            SelectedIndexChanged = null;
            itemsProperty = null;
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
            if (control as ListBox == null)
                return;

            ListBox C = control as ListBox;
            string EventName = reader.ReadString(4);

            if (EventName.Equals("SLIN"))
            {
                int NewIndex = reader.ReadInt32();

                // Event must not be thrown if it didn't really change.
                if (NewIndex != C.selectedIndex)
                {
                    C.selectedIndex = NewIndex;

                    if (C.SelectedIndexChanged != null)
                        C.SelectedIndexChanged.Invoke(control, new FormEventArgs());
                }
            }
        }
        #endregion

    }
}
