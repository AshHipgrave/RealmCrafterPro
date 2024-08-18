using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    partial class ItemInstance
    {
        public override int Armor
        {
            get
            {
                return item.ArmourLevel;
            }
        }

        public override int Damage
        {
            get
            {
                return item.WeaponDamage;
            }
        }

        public override int DamageType
        {
            get
            {
                return item.WeaponDamageType;
            }
        }

        public override int Health
        {
            get
            {
                return itemHealth;
            }
            set
            {
                itemHealth = value;

                // If it belongs to a human player, tell them the new health
                if (assignTo == null)
                    return;

                for (int i = 0; i <= 45; ++i)
                {
                    if (assignTo.Inventory.Items[i] == this)
                    {
                        Scripting.PacketWriter Pa = new Scripting.PacketWriter();
                        Pa.Write(assignTo.GCLID);
                        Pa.Write((byte)'H');
                        Pa.Write((byte)i);
                        Pa.Write((byte)itemHealth);

                        RCEnet.Send(assignTo.RNID, MessageTypes.P_InventoryUpdate, Pa.ToArray(), true);

                        return;
                    }
                }

            }
        }

        public override uint ID
        {
            get
            {
                return item.ID;
            }
        }

        public override int Mass
        {
            get
            {
                return item.Mass;
            }
        }

        public override string MiscData
        {
            get
            {
                return item.MiscData;
            }
        }

        public override string Name
        {
            get
            {
                return item.Name;
            }
        }

        public override float Range
        {
            get
            {
                return item.Range;
            }
        }

        public override int Value
        {
            get
            {
                return item.Value;
            }
        }

        public override Scripting.WeaponType WeaponType
        {
            get
            {
                return item.WeaponType;
            }
        }

        public override int GetAttribute(string name)
        {
            int Attr = Attribute.FindAttribute(name);
            if (Attr > -1)
                return Attributes.Value[Attr];
            return 0;
        }

    }
}
