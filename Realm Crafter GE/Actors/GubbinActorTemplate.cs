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
using RenderingServices;

namespace RealmCrafter
{
    public class GubbinActorTemplate
    {
        public Actor Actor;
        public byte Gender; // 0 - male, 1 - female

        public string Emitter = "";
        public ushort MeshID = 65535;

        public bool UseLight = false;
        public float LightRadius = 0.0f;
        public Vector3 LightColor = new Vector3(1, 1, 1);
        public string LightFunction = "";

        public GubbinAnimationType AnimationType = GubbinAnimationType.PreAnimated;
        public ushort AnimationStartFrame = 0;
        public ushort AnimationEndFrame = 0;

        public string AssignedBoneName = "";
        public Vector3 Position = new Vector3();
        public Vector3 Scale = new Vector3(1, 1, 1);
        public Vector3 Rotation = new Vector3();
    }
}
