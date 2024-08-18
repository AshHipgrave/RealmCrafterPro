using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Form
    /// </summary>
    public class Form : Control
    {
#region Member Variables
        bool closeButton = true;
        bool modal = false;

        /// <summary>
        /// Form Closed Event.
        /// </summary>
        /// <remarks>
        /// Event triggered when a user click the "Close" button on the form. After this event the form remains active but with the Visibility
        /// property set to <i>false</i>. If the close button is meant to dispose of the form, then remember to handle this correctly.
        /// </remarks>
        public event global::Scripting.Forms.EventHandler Closed;
#endregion

#region Properties
        /// <summary>
        /// Gets or sets whether a 'close' button is visible on the top of a form.
        /// </summary>
        public bool CloseButton
        {
            get { return closeButton; }
            set
            {
                closeButton = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "CBTN", closeButton));
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the modal property of this control (see: Remarks).
        /// </summary>
        /// <remarks>
        /// A 'Modal' form is one which remains topmost of all other controls. If a modal form is active, all other GUI input is disabled.
        /// </remarks>
        public bool Modal
        {
            get { return modal; }
            set
            {
                modal = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "MODL", modal));
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
            writer.Write((byte)(closeButton ? 1 : 0));
            writer.Write((byte)(modal ? 1 : 0));

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
            closeButton = reader.ReadByte() > 0;
            modal = reader.ReadByte() > 0;

            base.Deserialize(reader, master, actor, clientControls);
        }

        /// <summary>
        /// <b>DO NOT USE IN SCRIPTS.</b>
        /// </summary>
        public override PacketWriter ControlPropertyPacket(bool isRoot)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write("CWND", false);

            WriteInternalProperties(ref Pa);

            Pa.Write("CBTN", false);
            Pa.Write((byte)(closeButton ? 1 : 0));

            Pa.Write("MODL", false);
            Pa.Write((byte)(modal ? 1 : 0));

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
            Closed = null;
        }
#endregion

#region Events
        /// <summary>
        /// <b>DO NOT USE IN SCRIPTS.</b>
        /// </summary>
        public static void ProcessEvent(Control control, PacketReader reader)
        {
            
            if (control as Form == null)
                return;
            
            Form W = control as Form;
            string EventName = reader.ReadString(4);
            
            if (EventName.Equals("CLOS") && W.Closed != null)
                W.Closed.Invoke(control, new FormEventArgs());
        }
#endregion


    }
}
