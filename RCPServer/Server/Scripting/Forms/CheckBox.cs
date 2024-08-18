using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Checkbox
    /// </summary>
    public class CheckBox : Control
    {
        #region Member Variables
        bool isChecked = false;

        /// <summary>
        /// Client checked or unchecked the control.
        /// </summary>
        public event global::Scripting.Forms.EventHandler CheckedChange;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the box is checked.
        /// </summary>
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "CHCK", isChecked));
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
            writer.Write((byte)(isChecked ? 1 : 0));
            
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
            isChecked = reader.ReadByte() > 0;

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
            Pa.Write("CCHK", false);

            WriteInternalProperties(ref Pa);

            Pa.Write("CHCK", false);
            Pa.Write((byte)(isChecked ? 1 : 0));

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
            CheckedChange = null;
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
            if (control as CheckBox == null)
                return;

            CheckBox C = control as CheckBox;
            string EventName = reader.ReadString(4);

            if (EventName.Equals("CHCK"))
            {
                bool NewCheck = reader.ReadByte() > 0;

                // Event must not be thrown if it didn't really change.
                if (NewCheck != C.isChecked)
                {
                    C.isChecked = NewCheck;

                    if (C.CheckedChange != null)
                        C.CheckedChange.Invoke(control, new FormEventArgs());
                }
            }
        }
        #endregion
    }
}
