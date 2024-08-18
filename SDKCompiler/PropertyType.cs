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
using System.Collections.Generic;
using System.Text;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public enum PropertyType
    {
        Bool = 0,
        Byte = 1,
        Int16 = 2,
        Int32 = 3,
        Single = 4,
        Vector2 = 5,
        Vector3 = 6,
        Color = 7,
        String = 8,
        StringArray = 9,
        PositionType = 10,
        SizeType = 11,
        TextAlign = 12
    }

    public enum TextAlign
    {
        Default = -1,
        Left = 0,
        Center = 1,
        Right = 2,
        Top = 0,
        Middle = 1,
        Bottom = 2
    }

    public enum SizeType
    {
        Absolute = 0,
        Relative = 1
    }

    public enum PositionType
    {
        Absolute = 0,
        Relative = 1,
        Centered = 2
    }

}
