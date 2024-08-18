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
/* 
 * This class interfaces the property grid with the selected Light object
 * retrieving information from the Light object class
 * Author: Shane Smith, Aug 2008
 */
using System.ComponentModel;

namespace RealmCrafter_GE.Property_Interfaces
{
    public class LightPropertyInterface
    {
        private RealmCrafter.ClientZone.Light _Object;

        public LightPropertyInterface(RealmCrafter.ClientZone.Light Object)
        {
            _Object = Object;
        }

        #region Entity handling

        #region Location
        [CategoryAttribute("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }
            set
            {
                _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z());
                _Object.Handle.Position(value, _Object.EN.Y(), _Object.EN.Z());
            }
        }

        [CategoryAttribute("Location")]
        public float LocY
        {
            get { return _Object.EN.Y(); }
            set
            {
                _Object.EN.Position(_Object.EN.X(), value, _Object.EN.Z());
                _Object.Handle.Position(_Object.EN.X(), value, _Object.EN.Z());
            }
        }

        [CategoryAttribute("Location")]
        public float LocZ
        {
            get { return _Object.EN.Z(); }
            set
            {
                _Object.EN.Position(_Object.EN.X(), _Object.EN.Y(), value);
                _Object.Handle.Position(_Object.EN.X(), _Object.EN.Y(), value);
            }
        }
        #endregion

        #endregion

        #region Lighting properties
        [CategoryAttribute("Color")]
        public byte Red
        {
            get { return _Object.Red; }
            set
            {
                _Object.Red = value;
                _Object.Handle.Color(_Object.Red, _Object.Green, _Object.Blue);
                _Object.UpdateLines();
            }
        }

        [CategoryAttribute("Color")]
        public byte Green
        {
            get { return _Object.Green; }
            set
            {
                _Object.Green = value;
                _Object.Handle.Color(_Object.Red, _Object.Green, _Object.Blue);
                _Object.UpdateLines();
            }
        }

        [CategoryAttribute("Color")]
        public byte Blue
        {
            get { return _Object.Blue; }
            set
            {
                _Object.Blue = value;
                _Object.Handle.Color(_Object.Red, _Object.Green, _Object.Blue);
                _Object.UpdateLines();
            }
        }

        [CategoryAttribute("Radius")]
        public float Radius
        {
            get { return _Object.Radius; }
            set
            {
                _Object.Radius = value;
                _Object.Handle.Radius(_Object.Radius);
                _Object.UpdateLines();
            }
        }
        #endregion
    }
}