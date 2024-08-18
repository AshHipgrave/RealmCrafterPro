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
using System;
using System.IO;
using System.Windows.Forms;

namespace RealmCrafter
{
    public class Character
    {
        // Index
        public static Character[] Index = new Character[1024];
        // Members
        public int ID;
        public string Name;
        public ushort MeshID;
        public string ExclusiveRace, ExclusiveClass;
        public ushort SoundID;

        public float CharacterOffsetX,
                     CharacterOffsetY,
                     CharacterOffsetZ,
                     CharacterRotationX,
                     CharacterRotationY,
                     CharacterRotationZ,
                     CharacterScale = 1;

        // Linked list
        public static Character FirstCharacter;
        public Character NextCharacter;
        // Constructor
        public Character()
        {
            for (int i = 0; i < 1024; ++i)
            {
                if (Index[i] == null)
                {
                    Index[i] = this;
                    ID = i;
                    Name = "New character";
                    MeshID = 65535;
                    SoundID = 65535;
                    ExclusiveRace = "(None - available to all races)";
                    ExclusiveClass = "(None - available to all class)";
                    NextCharacter = FirstCharacter;
                    FirstCharacter = this;
                    return;
                }
            }
            throw new CharacterException("Maximum number of characters already created!");
        }

        private Character(bool DoNotAssignID)
        {
            Name = "New character";
            MeshID = 65535;
            SoundID = 65535;
            ExclusiveRace = "(None - available to all races)";
            ExclusiveClass = "(None - available to all class)";
            NextCharacter = FirstCharacter;
            FirstCharacter = this;
            return;
        }

        // Removes this instance from the index and linked list
        public void Delete()
        {
            Index[ID] = null;
            Character A = FirstCharacter;
            if (A == this)
            {
                FirstCharacter = NextCharacter;
            }
            else
            {
                while (A != null)
                {
                    if (A.NextCharacter == this)
                    {
                        A.NextCharacter = NextCharacter;
                        NextCharacter = null;
                        break;
                    }
                    A = A.NextCharacter;
                }
            }
        }
    }

    public class CharacterException : Exception
    {
        public CharacterException(string Message) : base(Message)
        {
        }
    }
}