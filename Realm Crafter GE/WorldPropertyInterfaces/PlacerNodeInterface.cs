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
/* This class interfaces the property grid with the selected scenery object
 * retrieving information from the Scenery object class
 * Author: Shane Smith, Aug 2008
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using RealmCrafter;
using RealmCrafter.ClientZone;
using RealmCrafter.ServerZone;
using RealmCrafter_GE.Property_Interfaces;
using RealmCrafter_GE.Property_Interfaces.PropertyUIs;
using RenderingServices;

namespace RealmCrafter_GE.Property_Interfaces
{
    /*Properties
     * Entity - Location, Rotation, Scale
     * MeshID, TextureID, AnimationMode
     * CatchRain // Generates a CatchPlane when loaded in client
     * Lightmap
     * NameScript
     * Collision
     */

    public class PlacerNodePropertyInterface
    {
        // Internal use
        private TreePlacerNode _Object;
        private int _InvSize;

        public PlacerNodePropertyInterface(TreePlacerNode Object)
        {
            _Object = Object;
        }


        #region Property interface


        #region Entity handling

        #region Location
        [CategoryAttribute("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }
            set { _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z()); _Object.UpdateTransform(); }
        }

        [CategoryAttribute("Location")]
        public float LocY
        {
            get { return _Object.EN.Y(); }
            set { _Object.EN.Position(_Object.EN.X(), value, _Object.EN.Z()); _Object.UpdateTransform(); }
        }

        [CategoryAttribute("Location")]
        public float LocZ
        {
            get { return _Object.EN.Z(); }
            set { _Object.EN.Position(_Object.EN.X(), _Object.EN.Y(), value); _Object.UpdateTransform(); }
        }
        #endregion

        #endregion

        #endregion
    }

}
