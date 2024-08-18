using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Textbox
    /// </summary>
    public class TextBox : Control
    {
#region Member Variables
        string passwordCharacter = "";
        string validationExpression = "";
#endregion

#region Properties
        /// <summary>
        /// Gets or sets the character that will substitute the client input for use with passwords.
        /// </summary>
        public string PasswordCharacter
        {
            get { return passwordCharacter; }
            set
            {
                passwordCharacter = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "PACH", passwordCharacter));
                }
            }
        }

        /// <summary>
        /// Gets or sets the regular expression used to validate the controls content.
        /// </summary>
        /// <remarks>
        /// If only a certain input type is required (such as numbers only), it is possible to define a regular expression to be tested as the client is typing.
        /// </remarks>
        public string ValidationExpression
        {
            get { return validationExpression; }
            set
            {
                validationExpression = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "VAEX", validationExpression));
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
            writer.Write(validationExpression, true);
            writer.Write(passwordCharacter, true);

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
            validationExpression = reader.ReadString();
            passwordCharacter = reader.ReadString();

            base.Deserialize(reader, master, actor, clientControls);
        }

        /// <summary>
        /// <b>DO NOT USE IN SCRIPTS.</b>
        /// </summary>
        public override PacketWriter ControlPropertyPacket(bool isRoot)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write("CTXT", false);

            WriteInternalProperties(ref Pa);

            Pa.Write("VAEX", false);
            Pa.Write(validationExpression, true);

            Pa.Write("PACH", false);
            Pa.Write(passwordCharacter, true);

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
        public static void ProcessEvent(Control control, PacketReader reader)
        {
            if (control as TextBox == null)
                return;

            TextBox T = control as TextBox;
            string EventName = reader.ReadString(4);

            if (EventName.Equals("TXTC"))
            {
                T.text = reader.ReadString();

//                 if (T.TextChanged != null)
//                 {
//                     T.TextChanged.Invoke(control, new FormEventArgs());
//                 }
            }
        }
#endregion


    }
}
