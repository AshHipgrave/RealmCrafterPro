using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Scripting.Math;


namespace Scripting.Forms
{
    /// <summary>
    /// Scripted GUI Base-Class
    /// </summary>
    public class Control
    {
#region Member Variables
        /// <summary>
        /// 
        /// </summary>
        protected Vector2 location = new Vector2(0, 0);

        /// <summary>
        /// 
        /// </summary>
        protected Vector2 size = new Vector2(0, 0);

        /// <summary>
        /// 
        /// </summary>
        protected bool enabled = true;

        /// <summary>
        /// 
        /// </summary>
        protected bool visible = true;

        /// <summary>
        /// 
        /// </summary>
        protected string name = "";

        /// <summary>
        /// 
        /// </summary>
        protected string text = "";

        /// <summary>
        /// 
        /// </summary>
        protected bool backColorSet = false;

        /// <summary>
        /// 
        /// </summary>
        protected Color backColor = Color.White;

        /// <summary>
        /// 
        /// </summary>
        protected bool foreColorSet = false;

        /// <summary>
        /// 
        /// </summary>
        protected Color foreColor = Color.White;

        /// <summary>
        /// 
        /// </summary>
        protected int skin = 1;

        /// <summary>
        /// 
        /// </summary>
        protected PositionType positionType = PositionType.Absolute;

        /// <summary>
        /// 
        /// </summary>
        protected SizeType sizeType = SizeType.Absolute;

        // When a network transfer takes place, this will be filled with the topmost control in the tree.
        /// <summary>
        /// Master control as sent to client.
        /// </summary>
        protected Control clientMaster = null;

        /// <summary>
        /// Internal AllocID.
        /// </summary>
        protected int allocID = 0;

        /// <summary>
        /// Actor form is associated with.
        /// </summary>
        protected Scripting.Actor actor = null;

        /// <summary>
        /// Internal parent.
        /// </summary>
        protected Control parent;

        /// <summary>
        /// Internal controls list.
        /// </summary>
        protected List<Control> controls = new List<Control>();
#endregion

#region Properties
        /// <summary>
        /// Gets the child controls of this object.
        /// </summary>
        public List<Control> Controls
        {
            get { return controls; }
        }

        /// <summary>
        /// Gets the root control in the controls tree.
        /// </summary>
        /// <remarks>
        /// Note: This will be null until GUI is sent to the client.
        /// </remarks>
        public Control ClientMaster
        {
            get { return clientMaster; }
        }

        /// <summary>
        /// Gets the Allocation Index of this object.
        /// </summary>
        /// <remarks>
        /// This will be 0 until a control is synced with the client; at which point it will become the identification
        /// of the control on the client and server machines.
        /// </remarks>
        public int AllocID
        {
            get { return allocID; }
        }

        /// <summary>
        /// Gets or sets the controls parent.
        /// </summary>
        /// <remarks>
        /// Note: It is not possible to set a new parent after the GUI tree is sent to a client. A System.Exception will be thrown if this is attempted.
        /// </remarks>
        public Control Parent
        {
            get { return parent; }
            set
            {
                if (clientMaster != null)
                    throw new Exception("Cannot modify control hierarchy after it has been sent to a client");
                else
                    parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the control name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "NAME", name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the control text.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "TEXT", text));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the control can be interacted with.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "ENBL", enabled));
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the control.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "VSBL", visible));
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the control in relation to its parent.
        /// </summary>
        public Math.Vector2 Location
        {
            get { return location; }
            set
            {
                location = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "LCTN", positionType, location));
                }
            }
        }

        /// <summary>
        /// Gets or sets the 'position type' of this control.
        /// </summary>
        public PositionType PositionType
        {
            get { return positionType; }
            set
            {
                positionType = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "LCTN", positionType, location));
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the control.
        /// </summary>
        public Math.Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "SIZE", sizeType, size));
                }
            }
        }

        /// <summary>
        /// Gets or sets the 'size type' of this control.
        /// </summary>
        public SizeType SizeType
        {
            get { return sizeType; }
            set
            {
                sizeType = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "SIZE", sizeType, size));
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the control.
        /// </summary>
        /// <remarks>
        /// By default, no color is set. This will allow the client to use the default color defined in the skin.
        /// </remarks>
        public System.Drawing.Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value; backColorSet = true;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "BCOL", backColor));
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        /// <remarks>
        /// By default, no color is set. This will allow the client to use the default color defined in the skin.
        /// </remarks>
        public System.Drawing.Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value; foreColorSet = true;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "FCOL", foreColor));
                }
            }
        }

        /// <summary>
        /// Gets or sets the skin index of the control.
        /// </summary>
        /// <remarks>
        /// Note: This command is currently unimplemented on the client side and will likely fail.
        /// </remarks>
        public int Skin
        {
            get { return skin; }
            set
            {
                skin = value;
                if (AllocID > 0) // Has network instance
                {
                    UpdateProperty(PropertyHelper.Create(AllocID, "SKIN", skin));
                }
            }
        }

        /// <summary>
        /// Get the actor to which this dialog is bound.
        /// </summary>
        public Actor Actor
        {
            get { return actor; }
        }
#endregion

#region Network Sync
        /// <summary>
        /// Serialize basic information
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Serialize(PacketWriter writer)
        {
            writer.Write(allocID);
            writer.Write((byte)positionType);
            writer.Write(location.X);
            writer.Write(location.Y);
            writer.Write((byte)sizeType);
            writer.Write(size.X);
            writer.Write(size.Y);
            writer.Write((byte)(enabled ? 1 : 0));
            writer.Write((byte)(visible ? 1 : 0));
            writer.Write(name, true);
            writer.Write(text, true);
            writer.Write((byte)(backColorSet ? 1 : 0));
            writer.Write(backColor.ToArgb());
            writer.Write((byte)(foreColorSet ? 1 : 0));
            writer.Write(foreColor.ToArgb());
            writer.Write(skin);
            writer.Write((ushort)controls.Count);

            foreach (Control C in controls)
            {
                C.Serialize(writer);
            }
        }

        /// <summary>
        /// Deserialize basic information
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="master"></param>
        /// <param name="actor"></param>
        /// <param name="clientControls"></param>
        public virtual void Deserialize(PacketReader reader, Control master, Scripting.Actor actor, LinkedList<Control> clientControls)
        {
            this.actor = actor;
            clientMaster = master;
            clientControls.AddLast(this);

            location = new Vector2();
            size = new Vector2();

            allocID = reader.ReadInt32();
            positionType = (PositionType)reader.ReadByte();
            location.X = reader.ReadSingle();
            location.Y = reader.ReadSingle();
            sizeType = (SizeType)reader.ReadByte();
            size.X = reader.ReadSingle();
            size.Y = reader.ReadSingle();
            enabled = reader.ReadByte() > 0;
            visible = reader.ReadByte() > 0;
            name = reader.ReadString();
            text = reader.ReadString();
            backColorSet = reader.ReadByte() > 0;
            backColor = Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            foreColorSet = reader.ReadByte() > 0;
            foreColor = Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            skin = reader.ReadInt32();

            ushort ControlCount = reader.ReadUInt16();
            if (ControlCount != controls.Count)
                throw new Exception("Serialized ControlCount does not match stored script count!");

            foreach (Control C in controls)
            {
                C.Deserialize(reader, master, actor, clientControls);
            }
        }

        /// <summary>
        /// Method called when this (master) control is being closed.
        /// </summary>
        /// <param name="forZoning">If true, then the actor is simply zoning (serializing) rather than logging off.</param>
        public virtual void Closing(bool forZoning)
        {

        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        protected void UpdateProperty(PacketWriter writer)
        {
            if (actor != null)
                actor.PostPropertyPacket(writer);
        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        public PacketWriter GetDeletePacket()
        {
            PacketWriter Pa = new PacketWriter();

            if (clientMaster != null)
                Pa.Write(clientMaster.allocID);
            else
                Pa.Write(allocID);

            return Pa;
        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        public PacketWriter ControlPropertyPacket()
        {
            return ControlPropertyPacket(true);
        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        public virtual PacketWriter ControlPropertyPacket(bool isRoot)
        {
            throw new Exception("Control must override ControlPropertyPacket()");
        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        protected void WriteInternalProperties(ref PacketWriter writer)
        {
            writer.Write("ALID", false);
            writer.Write(allocID);
            writer.Write("LCTN", false);
            writer.Write((byte)positionType);
            writer.Write(location.X);
            writer.Write(location.Y);
            writer.Write("SIZE", false);
            writer.Write((byte)sizeType);
            writer.Write(size.X);
            writer.Write(size.Y);
            writer.Write("ENBL", false);
            writer.Write((byte)(enabled ? 1 : 0));
            writer.Write("VSBL", false);
            writer.Write((byte)(visible ? 1 : 0));
            writer.Write("NAME", false);
            writer.Write(name, true);
            writer.Write("TEXT", false);
            writer.Write(text, true);
            if (backColorSet)
            {
                writer.Write("BCOL", false);
                writer.Write(backColor.A);
                writer.Write(backColor.R);
                writer.Write(backColor.G);
                writer.Write(backColor.B);
            }
            if (foreColorSet)
            {
                writer.Write("FCOL", false);
                writer.Write(foreColor.A);
                writer.Write(foreColor.R);
                writer.Write(foreColor.G);
                writer.Write(foreColor.B);
            }
            writer.Write("SKIN", false);
            writer.Write(skin);
        }
#endregion

#region Network Objects
        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        protected void RecurseSetMaster(Control master, Scripting.Actor actor, ref int allocIDs, LinkedList<Control> clientControls)
        {
            ++allocIDs;
            allocID = allocIDs;
            this.actor = actor;
            clientMaster = master;

            clientControls.AddLast(this);

            foreach (Control C in controls)
            {
                C.RecurseSetMaster(master, actor, ref allocIDs, clientControls);
            }
        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        protected virtual void Orphan()
        {

        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        protected void RecurseClientInstanceRemoved(LinkedList<Control> clientControls)
        {
            parent = null;
            actor = null;
            clientControls.Remove(this);
            foreach (Control C in controls)
            {
                C.RecurseClientInstanceRemoved(clientControls);
            }
            controls.Clear();
            Orphan();
        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        public void SendingToClient(Scripting.Actor actor, ref int allocIDs, LinkedList<Control> clientControls)
        {
            if (clientMaster != null)
                throw new Exception("Cannot send this control hierarchy to more than one client");

            RecurseSetMaster(this, actor, ref allocIDs, clientControls);
        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        public void ClientInstanceRemoved(LinkedList<Control> clientControls)
        {
            RecurseClientInstanceRemoved(clientControls);
        }
#endregion


#region Static Members
        static Dictionary<Type, Scripting.Forms.FormProcessEventHandler> CustomControlHandlers = new Dictionary<Type, Scripting.Forms.FormProcessEventHandler>();

        /// <summary>
        /// Register an event processor method to capture client form events targeted at the given type.
        /// </summary>
        /// <param name="type">Type of object (descended from Scripting.Forms.Control) to listen for.</param>
        /// <param name="handler">Delegate to invoke when an event is captured.</param>
        public static void RegisterFormProcessEventHandler(Type type, Scripting.Forms.FormProcessEventHandler handler)
        {
            if (handler == null)
                return;

            if (!CustomControlHandlers.ContainsKey(type))
                CustomControlHandlers.Add(type, handler);
        }

        /// <summary>
        /// <b>DO NOT USE FROM SCRIPTS.</b>
        /// </summary>
        public static void ProcessEvent(LinkedList<Scripting.Forms.Control> clientControls, PacketReader reader)
        {
            int AllocID = reader.ReadInt32();

            foreach (Control Ctrl in clientControls)
            {
                if (Ctrl.AllocID == AllocID)
                {
                    Type Check = Ctrl.GetType();

                    while (Check != typeof(object) && Check != null)
                    {
                        if (CustomControlHandlers.ContainsKey(Check))
                        {
                            CustomControlHandlers[Check].Invoke(Ctrl, reader);
                        }

                        Check = Check.BaseType;
                    }

                    return;
                }
            }

        }
#endregion

    }
}