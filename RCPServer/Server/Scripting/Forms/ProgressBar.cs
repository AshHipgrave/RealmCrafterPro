using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI ProgressBar
    /// </summary>
    public class ProgressBar : Control
    {
#region Member Variables
        int barValue = 0;
#endregion

#region Properties
        /// <summary>
        /// Gets or sets progress bar value (between 0-100).
        /// </summary>
        public int Value
        {
            get { return barValue; }
            set
            {
                int NewValue = value;
                if (NewValue < 0)
                    NewValue = 0;
                if (NewValue > 100)
                    NewValue = 100;

                if (NewValue == barValue)
                    return;

                barValue = NewValue;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "VALU", barValue));
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
            Pa.Write("CPRG", false);

            WriteInternalProperties(ref Pa);

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
            if (control as ProgressBar == null)
                return;

            ProgressBar B = control as ProgressBar;
            string EventName = reader.ReadString();
        }
        #endregion
    }
}
