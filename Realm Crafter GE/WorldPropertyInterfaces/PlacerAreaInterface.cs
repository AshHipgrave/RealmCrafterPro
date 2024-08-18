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
using System.Collections;
using System.Collections.Generic;
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

    public class PlacerAreaPropertyInterface
    {
        // Internal use
        private TreePlacerArea _Object;
        private int _InvSize;

        public PlacerAreaPropertyInterface(TreePlacerArea Object)
        {
            _Object = Object;
        }


        #region Property interface

        [CategoryAttribute("Area Properties")]
        public int NodeCount
        {
            get { return _Object.Nodes.Count; }
            set
            {
                if (value < 3)
                {
                    MessageBox.Show("Error: You can't have less than three edges!");
                    return;
                }

                if (value > _Object.Nodes.Count)
                {
                    int ToAdd = value - _Object.Nodes.Count;


                    float LX = _Object.Nodes[NodeCount - 1].EN.X();
                    float LZ = _Object.Nodes[NodeCount - 1].EN.Z();
                    float FX = _Object.Nodes[0].EN.X();
                    float FZ = _Object.Nodes[0].EN.Z();

                    float IntervalX = (FX - LX) / Convert.ToSingle(ToAdd + 1);
                    float IntervalZ = (FZ - LZ) / Convert.ToSingle(ToAdd + 1);

                    for (int i = 0; i < ToAdd; ++i)
                    {
                        LX += IntervalX;
                        LZ += IntervalZ;

                        _Object.AddPlacerNode(LX, LZ, false);
                    }

                    _Object.RebuildAll();
                }

                if (value < _Object.Nodes.Count)
                {
                    _Object.RemoveNodes(_Object.Nodes.Count - value);
                }
            }
        }

        [CategoryAttribute("Area Properties")]
        public int RandomSeed
        {
            get { return _Object.seed; }
            set
            {
                _Object.seed = value;
                _Object.RebuildAll();
            }
        }

        [CategoryAttribute("Area Properties")]
        public float Coverage
        {
            get { return _Object.coverage; }
            set
            {
                _Object.coverage = value;
                _Object.RebuildAll();
            }
        }

        [Category("Area Properties"), EditorAttribute(typeof(TreeCollectionTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<TreePlacerType> TreeTypes
        {
            get { return _Object.PlacementTypes; }
            set { _Object.PlacementTypes = value; }
        }

        [Category("Area Builder"), EditorAttribute(typeof(TreePlacerAreaFinalizer), typeof(System.Drawing.Design.UITypeEditor))]
        public TreePlacerArea Finalize
        {
            get { return _Object; }
            set { }
        }
        #endregion
    }

}
