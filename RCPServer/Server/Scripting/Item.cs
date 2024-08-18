using System;
using System.Collections.Generic;
using System.Text;


namespace Scripting
{
    /// <summary>
    /// Instances of this class hold Default data for items. A static array of Items is held in this class. This is
    /// a list of default items. 
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Gets Item ID as assigned in GE.
        /// </summary>
        public virtual uint IDNum { get { return 0; } }

        /// <summary>
        /// Gets Item's name.
        /// </summary>
        public virtual string ItemName { get { return string.Empty; } }

        /// <summary>
        /// Gets Exclusive Race name
        /// </summary>
        public virtual string Race { get { return string.Empty; } }

        /// <summary>
        /// Gets Exclusive Class name
        /// </summary>
        public virtual string Class { get { return string.Empty; } }

        /// <summary>
        /// Gets Name of script that is called for this item.
        /// </summary>
        public virtual string ScriptFile { get { return string.Empty; } }

        /// <summary>
        /// Gets Name of method that is called for item when it is used.
        /// </summary>
        public virtual string MethodName { get { return string.Empty; } }   
  
        /// <summary>
        /// Gets Value assigned in GE for this item.
        /// </summary>
        public virtual int Price { get { return 0; } }

        /// <summary>
        /// Gets Mass of Item as assigned in GE.
        /// </summary>
        public virtual int Weight { get { return 0; } }

        /// <summary>
        /// Gets whether Item takes damage.
        /// </summary>
        public virtual bool IsDamagable { get { return false; } }



        /// <summary>
        /// Gets damage of this Item as a weapon. As a assigned in GE.
        /// </summary>
        public virtual int Damage { get { return 0; } }

        /// <summary>
        /// Gets weapon damage type. This is the index of the Resistance related to this item. 
        /// </summary>
        public virtual int DamageType { get { return 0; } }

        /// <summary>
        /// Gets weapon type. E.g. Bow, Sword etc.
        /// </summary>
        public virtual WeaponType WeaponType { get { return 0; } }

        /// <summary>
        /// Gets the range of this item. If it's a ranged weapon.
        /// </summary>
        public virtual float WeaponRange { get { return 0; } }

        /// <summary>
        /// Gets the Armour Level of this item.
        /// </summary>
        public virtual int Armour { get { return 0; } }

        /// <summary>
        /// Gets the length of time the Eat Effect of this item occurs for.
        /// </summary>
        public virtual uint EatEffectsTime { get { return 0; } }

        /// <summary>
        /// Gets the MiscData string. As assigned in GE.
        /// </summary>
        public virtual string Misc { get { return string.Empty; } }  
        
        /// <summary>
        /// Gets whether item is stackable.
        /// </summary>  
        public virtual bool IsStackable { get { return false; } }

        /// <summary>
        /// Gets the slot which this item is placed in.
        /// </summary>
        public virtual ItemSlot GetSlot() { return 0; }

        /// <summary>
        /// Get Item type. This is how it is used.
        /// </summary>
        /// <returns></returns>
        public virtual ItemTypes GetItemType() { return 0; }


        /// <summary>
        /// Get Item's attribute value for given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual int GetAttributeValue(int index) { return 0; }

        /// <summary>
        /// Get Item's attribute max value for given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual int GetAttributeMaxValue(int index) { return 0; }

        /// <summary>
        /// Returns the item using the given Name. Returns null if no item is found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Item FindDefaultItem(string name)
        {
            foreach (Item i in DefaultItems)
                if (i.ItemName == name)
                    return i;

            return null;
        }

        /// <summary>
        /// Returns the item using given ID. Returns null if no item is found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item FindDefaultItem(int id)
        {
            foreach (Item i in DefaultItems)
                if (i.IDNum == id)
                    return i;

            return null;
        }

        /// <summary>
        /// Gets a list of all Default Items created within GE.
        /// </summary>
        public static Item[] DefaultItems;



   
    }
}
