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
// Realm Crafter Environment3D module by Rob W (rottbott@hotmail.com)
// Original version December 2004, C# port November 2006

using System;
using System.IO;
using RenderingServices;
using RealmCrafter_GE;

namespace RealmCrafter
{
    // Collision types
    public enum CollisionType
    {
        None = 0,
        Sphere = 1,
        Box = 2,
        Triangle = 3,
        Actor = 4,
        Player = 5,
        ActorTri1 = 7,
        ActorTri2 = 8,
        PickableNone = 9
    } ;

    public class Environment3D
    {
        protected static int TimeH, TimeM, TimeFactor;
        protected static int TimeS = 0, TimeUpdate = 0;
        protected static Entity Camera;

        protected static ClientZone.Zone CurrentZone;

        public static void Hide()
        {
            SDKNet.SDKInvoker.EnvHide();
        }

        public static void Show()
        {
            SDKNet.SDKInvoker.EnvShow();
        }

        static float fogNear = 500.0f, fogFar = 1000.0f;
        public static float FogNear
        {
            get { return fogNear; }
            set
            {
                fogNear = value;
                SDKNet.SDKInvoker.FogChange(fogNear, fogFar);
            }
        }

        public static float FogFar
        {
            get { return fogFar; }
            set
            {
                fogFar = value;
                SDKNet.SDKInvoker.FogChange(fogNear, fogFar);
            }
        }


        public static void InitializeEnvironment3D(Entity camera)
        {
            Camera = camera;

            // Setup all the defaults
            CurrentZone = null;

            TimeH = 16;
            TimeM = 0;
            TimeFactor = 10;

            SDKNet.SDKInvoker.Start(
                System.IO.Path.Combine(System.Environment.CurrentDirectory, "SDK.dll"),
                RenderWrapper.GetBBDXCommandData(),
                Camera.Handle, GE.GUIManager.GetHandle());
        }


        public static void Update(float deltaTime)
        {


            SDKNet.SDKInvoker.SetEnvTime(TimeH, TimeM, 0.0f);
            SDKNet.SDKInvoker.UpdateEnvironment(deltaTime);
            Camera.CameraRange(1.5f, RenderWrapper.bbdx2_GetFogFar() + 10.0f);

            if (RealmCrafter_GE.GE.TerrainManager != null)
                RealmCrafter_GE.GE.TerrainManager.SetAmbientLight(System.Drawing.Color.FromArgb(RenderWrapper.bbdx2_GetAmbientR(), RenderWrapper.bbdx2_GetAmbientG(), RenderWrapper.bbdx2_GetAmbientB()));
            if (RealmCrafter_GE.Program.Manager != null)
                RealmCrafter_GE.Program.Manager.SetAmbientLight(System.Drawing.Color.FromArgb(RenderWrapper.bbdx2_GetAmbientR(), RenderWrapper.bbdx2_GetAmbientG(), RenderWrapper.bbdx2_GetAmbientB()));
        }

        public static void SetCurrentZone(ClientZone.Zone newZone, string envName)
        {
            CurrentZone = newZone;
            SDKNet.SDKInvoker.ChangeEnvironment(envName, newZone.Name);
        }


        public static void SetTime(int timeH, int timeM)
        {
            TimeH = timeH;
            TimeM = timeM;
        }


        public static void SetViewDistance(float near, float far, bool ForceSkyChange)
        {
            FogNear = near;
            FogFar = far;
        }

    }
}