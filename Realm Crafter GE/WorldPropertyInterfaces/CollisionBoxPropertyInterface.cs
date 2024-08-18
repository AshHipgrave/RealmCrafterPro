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
 * This class interfaces the property grid with the selected Collision Box object
 * retrieving information from the Collision Box object class
 * Author: Shane Smith, Aug 2008
 */

using System.ComponentModel;
using RealmCrafter.ClientZone;

namespace RealmCrafter_GE.Property_Interfaces
{
    /// <summary>
    /// Collision Box Property Interface
    /// </summary>
    public class CollisionBoxPropertyInterface
    {
        /*Properties
         * Entity - Location, Rotation, Scale
         */
        private ColBox _Object;

        public CollisionBoxPropertyInterface(ColBox Object)
        {
            _Object = Object;
        }

        #region Entity handling

        #region Location
        [CategoryAttribute("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }
            set { _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z()); }
        }

        [CategoryAttribute("Location")]
        public float LocY
        {
            get { return _Object.EN.Y(); }
            set { _Object.EN.Position(_Object.EN.X(), value, _Object.EN.Z()); }
        }

        [CategoryAttribute("Location")]
        public float LocZ
        {
            get { return _Object.EN.Z(); }
            set { _Object.EN.Position(_Object.EN.X(), _Object.EN.Y(), value); }
        }
        #endregion

        #region Scale
        [CategoryAttribute("Scale")]
        public float ScaleX
        {
            get { return _Object.EN.ScaleX() * 20; }

            set { _Object.EN.Scale(value / 20, _Object.EN.ScaleY(), _Object.EN.ScaleZ()); }
        }

        [CategoryAttribute("Scale")]
        public float ScaleY
        {
            get { return _Object.EN.ScaleY() * 20; }

            set { _Object.EN.Scale(_Object.EN.ScaleX(), value / 20, _Object.EN.ScaleZ()); }
        }

        [CategoryAttribute("Scale")]
        public float ScaleZ
        {
            get { return _Object.EN.ScaleZ() * 20; }

            set { _Object.EN.Scale(_Object.EN.ScaleX(), _Object.EN.ScaleY(), value / 20); }
        }
        #endregion

        #region Rotation
        [CategoryAttribute("Rotation")]
        public float Pitch
        {
            get { return _Object.EN.Pitch(); }

            set { _Object.EN.Rotate(value, _Object.EN.Yaw(), _Object.EN.Roll()); }
        }

        [CategoryAttribute("Rotation")]
        public float Roll
        {
            get { return _Object.EN.Roll(); }

            set { _Object.EN.Rotate(_Object.EN.Pitch(), _Object.EN.Yaw(), value); }
        }

        [CategoryAttribute("Rotation")]
        public float Yaw
        {
            get { return _Object.EN.Yaw(); }

            set { _Object.EN.Rotate(_Object.EN.Pitch(), value, _Object.EN.Roll()); }
        }
        #endregion

        #endregion
    }
}