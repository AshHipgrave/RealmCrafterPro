//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
// Realm Crafter Inventories module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port October 2006

using System;

namespace RealmCrafter
{
    // Inventory class
    public class Inventory
    {
        // Slot constants
        public enum SlotType
        {
            Weapon = 1,
            Shield = 2,
            Hat = 3,
            Chest = 4,
            Hand = 5,
            Belt = 6,
            Legs = 7,
            Feet = 8,
            Ring = 9,
            Amulet = 10,
            Backpack = 11
        } ;

        public enum SlotIndex
        {
            Weapon = 0,
            Shield = 1,
            Hat = 2,
            Chest = 3,
            Hand = 4,
            Belt = 5,
            Legs = 6,
            Feet = 7,
            Ring1 = 8,
            Ring2 = 9,
            Ring3 = 10,
            Ring4 = 11,
            Amulet1 = 12,
            Amulet2 = 13,
            Backpack = 14
        } ;

        // Items held in this inventory
        public ItemInstance[] Items = new ItemInstance[50];
        public int[] Amounts = new int[50];

        // Gets the overall armour level for an inventory
        public int GetArmouryLevel()
        {
            int AP = 0;
            for (int i = (int) SlotIndex.Shield; i <= (int) SlotIndex.Feet; ++i)
            {
                if (Items[i] != null)
                {
                    if (Items[i].Item.IType == Item.ItemType.Armour && Items[i].ItemHealth > 0)
                    {
                        AP += Items[i].Item.ArmourLevel;
                    }
                }
            }
            return AP;
        }
    }
}