using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Represents and instance of an item that an actor possesses.
    /// </summary>
    public class ItemInstance
    {
        /// <summary>
        /// Get the armor amount of this item.
        /// </summary>
        public virtual int Armor { get { return 0; } }

        /// <summary>
        /// Get the damage that this item deals
        /// </summary>
        public virtual int Damage { get { return 0; } }

        /// <summary>
        /// Get the type of damage the item deals.
        /// </summary>
        public virtual int DamageType { get { return 0; } }

        /// <summary>
        /// Get or Set the remaining health of this item.
        /// </summary>
        public virtual int Health { get { return 0; } set { } }

        /// <summary>
        /// Get the ItemID (As seen in the Items Tab of the Game Editor).
        /// </summary>
        public virtual uint ID { get { return 0; } }

        /// <summary>
        /// Get the mass of the item.
        /// </summary>
        public virtual int Mass { get { return 0; } }

        /// <summary>
        /// Get the additional data as provided in the Items Tab of the Game Editor.
        /// </summary>
        public virtual string MiscData { get { return ""; } }

        /// <summary>
        /// Get the name of the item.
        /// </summary>
        public virtual string Name { get { return ""; } }

        /// <summary>
        /// Get the range of the item (if it is a weapon).
        /// </summary>
        public virtual float Range { get { return 0; } }

        /// <summary>
        /// Get the value of this item.
        /// </summary>
        public virtual int Value { get { return 0; } }

        /// <summary>
        /// Get the type of this item if it is a weapon.
        /// </summary>
        public virtual WeaponType WeaponType { get { return 0; } }

        /// <summary>
        /// Get the value of a specific attribute that this item can affect.
        /// </summary>
        /// <param name="name">Name of the attribute to find.</param>
        /// <returns>Attribute value as specified in the Items Tab of the Game Editor.</returns>
        public virtual int GetAttribute(string name) { return 0; }
    }
}
