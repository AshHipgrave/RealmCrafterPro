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
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;

namespace RCTTest
{
    public class GrassTypePropertyInterface
    {
        RealmCrafter.RCT.GrassType type;
        EventHandler changedEvent = null;

        public GrassTypePropertyInterface(RealmCrafter.RCT.GrassType setType, EventHandler ev)
        {
            type = setType;
            changedEvent = ev;
        }

        [CategoryAttribute("Main"), Description("Name of this grass type")]
        public string Name
        {
            get { return type.Name; }
            set { type.Name = value; changedEvent.Invoke(type, EventArgs.Empty); }
        }

        [CategoryAttribute("Appearance"), Description("Texture file applied to plane")]
        [Editor(typeof(RealmCrafter_GE.Property_Interfaces.cBrowseProperty), typeof(System.Drawing.Design.UITypeEditor))]
        public string Texture
        {
            get { return type.Texture; }
            set { type.Texture = System.IO.Path.Combine(@"Data\Textures\", value); changedEvent.Invoke(type, EventArgs.Empty); }
        }

        [CategoryAttribute("Appearance"), Description("X Scale of the grass in units")]
        public float ScaleX
        {
            get
            {
                float T = type.Scale.X;
                return T;
            }

            set
            {
                type.Scale = new NGUINet.NVector2(value, ScaleY);
                changedEvent.Invoke(type, EventArgs.Empty);
            }
        }

        [CategoryAttribute("Appearance"), Description("Y Scale of the grass in units")]
        public float ScaleY
        {
            get
            {
                float T = type.Scale.Y;
                return T;
            }

            set
            {
                type.Scale = new NGUINet.NVector2(ScaleX, value);
                changedEvent.Invoke(type, EventArgs.Empty);
            }
        }

        [CategoryAttribute("Appearance"), Description("Amount of grass to fill a given area. At 1.0, GrassCount = Area/Scale.")]
        public float Coverage
        {
            get
            {
                return type.Coverage;
            }
            set
            {
                type.Coverage = value; changedEvent.Invoke(type, EventArgs.Empty);
            }
        }

        [CategoryAttribute("Appearance"), Description("Y Offset of plane incase it is not centered.")]
        public float Offset
        {
            get
            {
                return type.Offset;
            }
            set { type.Offset = value; changedEvent.Invoke(type, EventArgs.Empty); }
        }

        [CategoryAttribute("Appearance"), Description("Random height change on different blades.")]
        public float HeightVariance
        {
            get
            {
                return type.HeightVariance;
            }
            set { type.HeightVariance = value; changedEvent.Invoke(type, EventArgs.Empty); }
        }

        
    }
}
