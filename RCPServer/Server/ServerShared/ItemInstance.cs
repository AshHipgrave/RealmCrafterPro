using System;
using System.Collections.Generic;
using System.Text;
using RCPServer;
namespace RCPServer
{

    public partial class ItemInstance : Scripting.ItemInstance
    {
        Item item = null;
        Attributes attributes = new Attributes();
        int itemHealth = 0;
        int assignment = 0;
        ActorInstance assignTo = null;
        int assignId = 0;
        int allocId = 0;
        int sentToClient = 0;

        public void Serialize(Scripting.PacketWriter Pa)
        {
            if (Item != null)
                Pa.Write((ushort)item.ID);
            else
                Pa.Write((ushort)65535);

            attributes.Serialize(Pa);
            Pa.Write(itemHealth);
            Pa.Write(assignment);
            if (assignTo != null)
                Pa.Write(assignTo.GCLID);
            else
                Pa.Write(0);
            Pa.Write(assignId);
            Pa.Write(allocId);
            Pa.Write(sentToClient);

            if(sentToClient > 0)
                SentToClient.Remove(this);
        }

        public static ItemInstance Deserialize(Scripting.PacketReader Pa, Scripting.PacketWriter ReAllocator)
        {
            if (Pa.ReadByte() == 0)
                return null;

            ItemInstance I = new ItemInstance();
            ushort ItemID = Pa.ReadUInt16();
            if (ItemID != 65535)
                I.item = Item.Items[ItemID];

            I.attributes.Deserialize(Pa);
            I.itemHealth = Pa.ReadInt32();
            I.assignment = Pa.ReadInt32();
            uint GCLID = Pa.ReadUInt32();
            I.assignId = Pa.ReadInt32();
            int tAllocID = Pa.ReadInt32();
            I.sentToClient = Pa.ReadInt32();

            ActorInstance AI = ActorInstance.FromGCLID(GCLID);
            if (AI != null)
                I.assignTo = AI;

            if(I.sentToClient > 0)
                SentToClient.Add(I);

            ReAllocator.Write((byte)'I');
            ReAllocator.Write(tAllocID);
            ReAllocator.Write(I.allocId);

            return I;
        }

        public Item Item
        {
            get { return item; }
            set { item = value; }
        }

        public Attributes Attributes
        {
            get { return attributes; }
        }

        public int ItemHealth
        {
            get { return itemHealth; }
            set { itemHealth = value; }
        }

        public int Assignment
        {
            get { return assignment; }
            set { assignment = value; }
        }

        public int AssignID
        {
            get { return assignId; }
            set { assignId = value; }
        }

        public int AllocID
        {
            get { return allocId; }
            set { allocId = value; }
        }

        public ActorInstance AssignTo
        {
            get { return assignTo; }
            set { assignTo = value; }
        }

        public ItemInstance()
        {
            ++LastAllocID;
            allocId = LastAllocID;
        }

        public bool Identical(ItemInstance other)
        {
            if (other == null)
                return false;

            if (Item != other.Item)
                return false;

            if (ItemHealth != other.ItemHealth)
                return false;

            for (int i = 0; i < Attributes.Value.Length; ++i)
                if (Attributes.Value[i] != other.Attributes.Value[i])
                    return false;

            return true;
        }

        public ItemInstance CopyItemInstance()
        {
            ItemInstance I = new ItemInstance();
            I.Item = Item;
            I.ItemHealth = ItemHealth;

            for (int i = 0; i < Attributes.Value.Length; ++i)
                I.Attributes.Value[i] = Attributes.Value[i];

            return I;
        }

        public byte[] ToArray()
        {
            byte[] StrBytes = new byte[83];

            byte[] ItemID = BitConverter.GetBytes((ushort)(item.ID));
            StrBytes[0] = ItemID[0]; StrBytes[1] = ItemID[1];

            for (int i = 0; i < attributes.Value.Length; ++i)
            {
                byte[] ValueByte = BitConverter.GetBytes((ushort)(attributes.Value[i] + 5000));
                StrBytes[(i * 2) + 2] = ValueByte[0];
                StrBytes[(i * 2) + 3] = ValueByte[1];
            }

            StrBytes[82] = (byte)(itemHealth);

            return StrBytes;
        }

        public void SendingToClient()
        {
            if (sentToClient == 0)
                SentToClient.Add(this);
            ++sentToClient;
        }

        public void ClientInstanceRemoved()
        {
            --sentToClient;
            if (sentToClient == 0)
                SentToClient.Remove(this);
        }


        protected static int LastAllocID = 0;
        protected static List<ItemInstance> SentToClient = new List<ItemInstance>();

        public static ItemInstance FromAllocID(int id)
        {
            foreach (ItemInstance I in SentToClient)
                if (I.AllocID == id)
                    return I;
            return null;
        }

        public static ItemInstance CreateItemInstance(Item item)
        {
            ItemInstance I = new ItemInstance();
            I.Item = item;
            I.ItemHealth = 100;

            for (int i = 0; i < I.Attributes.Maximum.Length; ++i)
                I.Attributes.Value[i] = I.Item.Attributes.Value[i];

            return I;
        }

        public static int ItemInstanceStringLength()
        {
            return 83;
        }

        public static ItemInstance FromArray(byte[] StringArray)
        {
            if (StringArray.Length < ItemInstanceStringLength())
                return null;

            // NOTE THIS NEEDS TO BE CHANGED LATER TO UINT32 - Ben
            uint ID = (uint)(BitConverter.ToUInt16(StringArray, 2));

            if (Item.Items[ID] != null)
            {
                ItemInstance I = CreateItemInstance(Item.Items[ID]);
                int Offset = 2;
                for (int j = 0; j < 40; ++j)
                {
                    I.Attributes.Value[j] = (int)(BitConverter.ToUInt16(StringArray, Offset)) - 5000;
                    Offset += 2;
                }
                I.ItemHealth = (int)(StringArray[Offset]);

                return I;
            }
            else
            {
                Log.WriteLine("Warning: Item (" + ID.ToString() + ") was removed from an actor as it no longer exists!");
                int Offset = 2;
                for (int j = 0; j < 40; ++j)
                {
                    //(int)(BitConverter.ToUInt16(StringArray, Offset));
                    Offset += 2;
                }
                //(int)(StringArray[Offset]);

                return null;
            }

            
        }
    }
}
