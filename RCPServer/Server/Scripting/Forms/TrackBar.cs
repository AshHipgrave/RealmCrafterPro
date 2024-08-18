using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Checkbox
    /// </summary>
    public class TrackBar : Control
    {
#region Member Variables
        int barValue = 0;
        int minimum = 0;
        int maximum = 10;
        int tickFrequency = 1;

        /// <summary>
        /// Client checked or unchecked the control.
        /// </summary>
        public event global::Scripting.Forms.EventHandler ValueChanged;
#endregion

#region Properties
        /// <summary>
        /// Gets or sets
        /// </summary>
        public int Value
        {
            get { return barValue; }
            set
            {
                barValue = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "VALU", barValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public int Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "MINM", minimum));
                }
            }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public int Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "MAXM", maximum));
                }
            }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public int TickFrequency
        {
            get { return tickFrequency; }
            set
            {
                tickFrequency = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "TCKF", tickFrequency));
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
            writer.Write(minimum);
            writer.Write(maximum);
            writer.Write(tickFrequency);
            writer.Write(barValue);

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
            minimum = reader.ReadInt32();
            maximum = reader.ReadInt32();
            tickFrequency = reader.ReadInt32();
            barValue = reader.ReadInt32();

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
            Pa.Write("CTRB", false);

            WriteInternalProperties(ref Pa);

            Pa.Write("MINM", false);
            Pa.Write(minimum);

            Pa.Write("MAXM", false);
            Pa.Write(maximum);

            Pa.Write("TCKF", false);
            Pa.Write(tickFrequency);

            Pa.Write("VALU", false);
            Pa.Write(barValue);

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
            ValueChanged = null;
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
            if (control as TrackBar == null)
                return;

            TrackBar C = control as TrackBar;
            string EventName = reader.ReadString(4);

            if (EventName.Equals("VALU"))
            {
                int NewCheck = reader.ReadInt32();

                // Event must not be thrown if it didn't really change.
                if (NewCheck != C.barValue)
                {
                    C.barValue = NewCheck;

                    if (C.ValueChanged != null)
                        C.ValueChanged.Invoke(control, new FormEventArgs());
                }
            }
        }
        #endregion



    }
}
