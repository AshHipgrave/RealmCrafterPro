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
// Realm Crafter ClientZones module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port November 2006

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using RealmCrafter_GE;
using RenderingServices;
using IrrlichtSound;
using System.Windows.Forms;
using System.ComponentModel;

namespace RealmCrafter.ClientZone
{
    // Delegate for setting loading bar progress
    public delegate void LoadProgressUpdate(int Value);

    // A single zone
    public class Zone
    {
        // Maximum value for fog far distance
        public const float MaxFogFar = 3000f;

        #region Zone settings
        public string Name;
        //public Entity Cloud, Stars;
        //public ushort SkyTexID, CloudTexID, StormCloudTexID, StarsTexID;
        public byte FogR, FogG, FogB;
        public float FogNear, FogFar;
        public bool Outdoors;
        public byte AmbientR, AmbientG, AmbientB;
        public float DefaultLightPitch, DefaultLightYaw;
        public ushort LoadingTexID, LoadingMusicID;
        public ushort MapMarkerTexID, MapTexID;
        public float SlopeRestrict;

        public string EnvironmentName = "default";
        // world setup
        //public ushort WorldSetupRainSoundId, WorldSetupWindSoundId, WorldSetupSnowTextureId, WorldSetupRainTextureId;
        //public ushort[] WorldSetupStromSoundId;
        //public bool DefaultZone;
        #endregion

        #region Lists of zone objects
        public List<ZoneObject>
            Sceneries = new List<ZoneObject>(),
            Waters = new List<ZoneObject>(),
            ColBoxes = new List<ZoneObject>(),
            Emitters = new List<ZoneObject>(),
            Terrains = new List<ZoneObject>(),
            SoundZones = new List<ZoneObject>(),
            Lights = new List<ZoneObject>(),
            MenuControls = new List<ZoneObject>();
        #endregion

        private static Entity StoredCamera;
        private static bool StoredDisplayItems;

        // Constructor
        public Zone(string ZoneName)
        {
            // Default settings
            Name = ZoneName;
            //SkyTexID = 65535;
            //CloudTexID = 65535;
            //StormCloudTexID = 65535;
            //StarsTexID = 65535;
            LoadingTexID = 65535;
            LoadingMusicID = 65535;
            SlopeRestrict = 0.6f;
            FogNear = 800f;
            FogFar = 2500f;
            FogR = 200;
            FogG = 200;
            FogB = 200;
            AmbientR = 100;
            AmbientG = 100;
            AmbientB = 100;
            Outdoors = true;
            DefaultLightPitch = 0f;
            DefaultLightYaw = 0f;
            //WorldSetupStromSoundId = new ushort[3];

            // Create sky spheres
            //Stars = Entity.LoadMesh(@"Data\Meshes\Sky Sphere.b3d");
            //Stars.Shader = Shaders.Sky;
            //Stars.Order = 4;
            //Stars.FX = 1 + 8;
            //Stars.Scale(500f, 100f, 500f);
            //Cloud = Entity.LoadMesh(@"Data\Meshes\Sky Sphere.b3d");
            //Cloud.Shader = Shaders.Clouds;
            //Cloud.Order = 1;
            //Cloud.FX = 1 + 8;
            //Cloud.Scale(500f, 100f, 500f);
            //Cloud.Visible = false;
        }

        // Load, unload or save this zone
        public static Zone Load(string ZoneName, Entity Cam, bool DisplayItems, LoadProgressUpdate ProgUpdate)
        {
            
            StoredCamera = Cam;
            StoredDisplayItems = DisplayItems;
            // Open file and lock media databases
            BinaryReader F = Blitz.ReadFile(@"Data\Areas\" + ZoneName + ".dat");
            if (F == null)
            {
                return null;
            }
            Media.LockMeshes();
            Media.LockTextures();
            if (ProgUpdate != null)
            {
                ProgUpdate(0);
            }

            // Create new zone object
            Zone Z = new Zone(ZoneName);
            

            #region Create and display loading screen if necessary
            uint Tex;
            Entity LoadQuad = null;
            Z.LoadingTexID = F.ReadUInt16();
            Z.LoadingMusicID = F.ReadUInt16();
            if (Z.LoadingTexID < 65535)
            {
                Tex = Media.GetTexture(Z.LoadingTexID, false);
                if (Tex != 0)
                {
                    LoadQuad = Entity.CreateSAQuad();
                    LoadQuad.SAQuadLayout(0f, 0f, 1f, 1f);
                    LoadQuad.Texture(Tex);
                    Media.UnloadTexture(Z.LoadingTexID);
                    Render.RenderWorld();
                }
            }
            #endregion



            // Read in zone data and load all objects


            #region Environment
            
             int SkyTexID = F.ReadUInt16();
             int CloudTexID = F.ReadUInt16();
             int StormCloudTexID = F.ReadUInt16();
             int StarsTexID = F.ReadUInt16();
// 
//             Environment3D.SetSkyTextures(CloudTexID, StarsTexID, SkyTexID);

            Z.FogR = F.ReadByte();
            Z.FogG = F.ReadByte();
            Z.FogB = F.ReadByte();
            Z.FogNear = F.ReadSingle();
            Z.FogFar = F.ReadSingle();
            if (Z.FogFar > MaxFogFar)
            {
                Z.FogFar = MaxFogFar;
            }
            
            #endregion

            #region Texture sky spheres
            //if (Z.CloudTexID < 65535)
            //{
            //    Tex = Media.GetTexture(Z.CloudTexID, false);
            //    if (Tex != 0)
            //    {
            //        Z.Cloud.Texture(Tex, 0);
            //        Z.Stars.Texture(Tex, 0);
            //    }
            //    Z.Cloud.AlphaNoSolid(0.4f);
            //}
            //if (Z.StarsTexID < 65535)
            //{
            //    Tex = Media.GetTexture(Z.StarsTexID, false);
            //    if (Tex != 0)
            //    {
            //        Z.Stars.Texture(Tex, 1);
            //        Z.Cloud.Texture(Tex, 1);
            //    }
            //    Z.Stars.AlphaNoSolid(1f);
            //}
            //else
            //{
            //    Z.Stars.AlphaNoSolid(0f);
            //}
            #endregion

            #region Other environmental settings
            //Z.MapMarkerTexID = F.ReadUInt16();
            Z.MapTexID = F.ReadUInt16();
            Z.Outdoors = F.ReadBoolean();
            Z.AmbientR = F.ReadByte();
            Z.AmbientG = F.ReadByte();
            Z.AmbientB = F.ReadByte();
            Z.DefaultLightPitch = F.ReadSingle();
            Z.DefaultLightYaw = F.ReadSingle();
            Z.SlopeRestrict = F.ReadSingle();
            #endregion

            if (ProgUpdate != null)
            {
                ProgUpdate(5);
            }

            // Temporary loading variables
            float PosX, PosY, PosZ, Pitch, Yaw, Roll, ScaleX, ScaleY, ScaleZ;

            #region Load scenery objects
            ushort Sceneries = F.ReadUInt16();
            bool bIsNewVersion = true;
            for (int i = 0; i < Sceneries; ++i)
            {
                ushort MeshID = F.ReadUInt16();
                Scenery S = new Scenery(Z, MeshID);
                PosX = F.ReadSingle();
                PosY = F.ReadSingle();
                PosZ = F.ReadSingle();
                Pitch = F.ReadSingle();
                Yaw = F.ReadSingle();
                Roll = F.ReadSingle();
                ScaleX = F.ReadSingle();
                ScaleY = F.ReadSingle();
                ScaleZ = F.ReadSingle();
                S.AnimationMode = F.ReadByte();
                S.SceneryID = F.ReadByte();
                S.TextureID = F.ReadUInt16();

                byte Flags = F.ReadByte();
                S.CatchRain = (Flags & 1) > 0;
                S.Interactive = (Flags & 2) > 0;

                // LOD
                ushort MeshLOD_Medium, MeshLOD_Low;
                float distLOD_High, distLOD_Medium, distLOD_Low;
                CollisionType CollisionMode;

                 MeshLOD_Low = MeshLOD_Medium = 65535;
                distLOD_High = distLOD_Medium = distLOD_Low = 0;

                Byte IsNewVersion = F.ReadByte();
                bIsNewVersion = (IsNewVersion == 255);
                if (IsNewVersion == 255)
                {
                    distLOD_High = F.ReadSingle();
                    MeshLOD_Medium = F.ReadUInt16();
                    distLOD_Medium = F.ReadSingle();
                    MeshLOD_Low = F.ReadUInt16();
                    distLOD_Low = F.ReadSingle();

                    CollisionMode = (CollisionType)F.ReadByte();
                    if ( (CollisionType)CollisionMode == CollisionType.Triangle )
                        S.CollisionMeshID = F.ReadUInt16();
                }
                else
                    CollisionMode = (CollisionType)IsNewVersion;

                S.Lightmap = Blitz.ReadString(F);
                S.RCTE = Blitz.ReadString(F);

                // If mesh loaded successfully
                if (S.EN != null)
                {
                    // Transform
                    S.EN.Position(PosX, PosY, PosZ);
                    S.EN.Rotate(Pitch, Yaw, Roll);
                    S.EN.Scale(ScaleX, ScaleY, ScaleZ);

	                 if (bIsNewVersion)
	                 {
	                    RenderWrapper.EntityShadowLevel(S.EN.Handle, 2);

                        if (distLOD_Low == 0)
                            distLOD_Low = 10000;
                        if (distLOD_Medium == 0)
                            distLOD_Medium = 10000;
                        if (distLOD_High == 0)
                            distLOD_High = 10000;

	                    // LOD
	                    S.EN.distLOD_High = distLOD_High;
	                    S.EN.MeshLOD_Medium = MeshLOD_Medium;
	                    S.EN.distLOD_Medium = distLOD_Medium;
	                    S.EN.MeshLOD_Low = MeshLOD_Low;
	                    S.EN.distLOD_Low = distLOD_Low;
	
	                    //RenderWrapper.EntityShadowShader(S.EN.Handle, Shaders.DefaultAnimatedDepthShader);
	                 }

                    // Apply lightmap
                    if (S.Lightmap != "")
                    {
                        uint LMap = Render.LoadTexture(S.Lightmap);
                        //Render.TextureCoords(LMap, 1);
                        S.EN.Texture(LMap, 1);
                        Render.FreeTexture(LMap);
                        //S.EN.Shader = Shaders.LitObject2;
                    }

                    // Rexture
                    if (S.TextureID < 65535)
                    {
                        Tex = Media.GetTexture(S.TextureID, false);
                        if (Tex != 0)
                        {
                            S.EN.Texture(Tex);
                        }
                    }

                    // Animation
                    if (!DisplayItems && (S.AnimationMode == 1 || S.AnimationMode == 2))
                    {
                        S.EN.Animate(S.AnimationMode);
                    }

                    // Set collision/picking
                    if (CollisionMode != CollisionType.None)
                    {
                        Collision.EntityType(S.EN, (byte) CollisionMode);
                        if (CollisionMode == CollisionType.Sphere)
                        {
                            float MaxLengthX = S.EN.MeshWidth * ScaleX;
                            float MaxLengthZ = S.EN.MeshDepth * ScaleZ;
                            if (MaxLengthZ > MaxLengthX)
                            {
                                MaxLengthX = MaxLengthZ;
                            }
                            Collision.EntityRadius(S.EN, MaxLengthX / 2f, (S.EN.MeshHeight * ScaleY) / 2f);
                            Collision.EntityPickMode(S.EN, PickMode.Sphere);
                        }
                        else if (CollisionMode == CollisionType.Triangle)
                        {
                            Collision.SetCollisionMesh(S.EN);
                            Collision.EntityPickMode(S.EN, PickMode.Polygon);
                        }
                        else if (CollisionMode == CollisionType.Box)
                        {
                            float Width = S.EN.MeshWidth * ScaleX;
                            float Height = S.EN.MeshHeight * ScaleY;
                            float Depth = S.EN.MeshDepth * ScaleZ;
                            Collision.EntityBox(S.EN, Width / -2f, Height / -2f, Depth / -2f, Width, Height, Depth);
                            Collision.EntityPickMode(S.EN, PickMode.Polygon);
                        }
                        Collision.ResetEntity(S.EN);
                    }
                    else if (DisplayItems)
                    {
                        Collision.EntityType(S.EN, (byte) CollisionType.PickableNone);
                        Collision.SetCollisionMesh(S.EN);
                        Collision.EntityPickMode(S.EN, PickMode.Polygon);
                    }

                    // Create catch plane if required
                    if (!DisplayItems && S.CatchRain)
                    {
                        MeshMinMaxVertices MMV = new MeshMinMaxVertices(S.EN, Pitch, Yaw, Roll, ScaleX, ScaleY, ScaleZ);
                        CatchPlane CP = new CatchPlane();
                        CP.Y = MMV.MaxY + PosY;
                        CP.MinX = MMV.MinX + PosX;
                        CP.MinZ = MMV.MinZ + PosZ;
                        CP.MaxX = MMV.MaxX + PosX;
                        CP.MaxZ = MMV.MaxZ + PosZ;
                    }
                }
                    // Mesh did not load successfully
                else
                {
                    // Remove from list
                    Z.Sceneries.Remove(S);
                }

                // RottNet update

                // Loading bar update every alternate object
                if ((i % 2) == 0 && ProgUpdate != null)
                {
                    ProgUpdate(5 + ((40 * i) / Sceneries));
                }
            }
            #endregion

            #region Load water areas
            ushort Waters = F.ReadUInt16();
            for (int i = 0; i < Waters; ++i)
            {
                Water W = new Water(Z);
                if (bIsNewVersion)
                {
                	W.ShaderID = ShaderManager.GetShader(F.ReadInt32());
                	W.EN.Shader = W.ShaderID;
               }
               else
               {
                	W.TextureID[0] = F.ReadUInt16();
                	W.TexHandle[0] = Media.GetTexture(W.TextureID[0], false);
                	W.TexHandle[1] = Render.LoadTexture(@"Data\Textures\Water\Shader Water.png");
               }

                W.TexScale = F.ReadSingle();
                PosX = F.ReadSingle();
                PosY = F.ReadSingle();
                PosZ = F.ReadSingle();

                if ((i == 0) && bIsNewVersion) RenderingServices.RenderWrapper.SetWaterLevel(PosY);

                ScaleX = F.ReadSingle();
                ScaleZ = F.ReadSingle();
                W.Red = F.ReadByte();
                W.Green = F.ReadByte();
                W.Blue = F.ReadByte();
                W.Alpha = F.ReadByte();
                W.EN.Scale(ScaleX, 1, ScaleZ);
                W.EN.Position(PosX, PosY, PosZ);

                for (int t = 0; t < 4; ++t)
                {
                    if (bIsNewVersion)
                    {
                    	 	W.TextureID[t] = F.ReadUInt16();
                    		W.TexHandle[t] = Media.GetTexture((int)(W.TextureID[t]), false);
                    }
                    if (W.TexHandle[t] != 0)
                    {
                        Render.ScaleTexture(W.TexHandle[t], 1f / W.TexScale, 1f / W.TexScale);
                        W.EN.Texture(W.TexHandle[t], t);
                    }
                }

                float Alpha = (float) (W.Alpha) / 100f;
                if (Alpha > 1f)
                {
                    Alpha = 1f;
                }
                W.EN.AlphaNoSolid(Alpha);
                // If in the client and the player actor instance is of a walking only type, create collision box here
                if (!DisplayItems && ActorInstance.Me != null)
                {
                    if (ActorInstance.Me.Actor.EType == Actor.Environment.Walk)
                    {
                        Collision.EntityType(W.EN, (byte) CollisionType.Box);
                        ScaleX = (float) Math.Abs(ScaleX / 2.0);
                        ScaleZ = (float) Math.Abs(ScaleZ / 2.0);
                        Collision.EntityBox(W.EN, -ScaleX, -1000f, -ScaleZ, ScaleX * 2f, 2000f, ScaleZ * 2f);
                    }
                }

                int nParams = (bIsNewVersion? F.ReadInt32() : 0);
                for (int iParam = 0; iParam < nParams; ++iParam)
                {
                    string MName = F.ReadString(); 
                    byte ttype = F.ReadByte();
                    switch(ttype) 
                    {
                        case 1: 
                            {
                                Vector1 v = new Vector1(); v.X = F.ReadSingle();
                                RenderingServices.RenderWrapper.EntityConstantFloat(W.EN.Handle, MName, v.X);
                                break;
                            }
                        case 2: {
                                Vector2 v = new Vector2(); v.X = F.ReadSingle(); v.Y = F.ReadSingle();
                                RenderingServices.RenderWrapper.EntityConstantFloat2(W.EN.Handle, MName, v.X, v.Y);
                                break;
                            }
                        case 3:
                            {
                                Vector3 v = new Vector3(); v.X = F.ReadSingle(); v.Y = F.ReadSingle(); v.Z = F.ReadSingle();
                                RenderingServices.RenderWrapper.EntityConstantFloat3(W.EN.Handle, MName, v.X, v.Y, v.Z);
                                break;
                            }
                        case 4:
                            {
                                Vector4 v = new Vector4(); v.X = F.ReadSingle(); v.Y = F.ReadSingle(); v.Z = F.ReadSingle(); v.W = F.ReadSingle();
                                RenderingServices.RenderWrapper.EntityConstantFloat4(W.EN.Handle, MName, v.X, v.Y, v.Z, v.W);
                                break;
                            }
                    }
                }
            }           



            #endregion

            if (ProgUpdate != null)
            {
                ProgUpdate(60);
            }

            #region Load collision boxes
            ushort ColBoxes = F.ReadUInt16();
            for (int i = 0; i < ColBoxes; ++i)
            {
                ColBox C = new ColBox(Z, DisplayItems);
                PosX = F.ReadSingle();
                PosY = F.ReadSingle();
                PosZ = F.ReadSingle();
                Pitch = F.ReadSingle();
                Yaw = F.ReadSingle();
                Roll = F.ReadSingle();
                ScaleX = F.ReadSingle();
                ScaleY = F.ReadSingle();
                ScaleZ = F.ReadSingle();
                C.EN.Position(PosX, PosY, PosZ);
                C.EN.Rotate(Pitch, Yaw, Roll);
                C.EN.Scale(ScaleX, ScaleY, ScaleZ);
            }
            #endregion

            if (ProgUpdate != null)
            {
                ProgUpdate(65);
            }

            #region Load emitters
            ushort Emitters = F.ReadUInt16();
            for (int i = 0; i < Emitters; ++i)
            {
                string ConfigName = Blitz.ReadString(F);
                Emitter E = new Emitter(Z, ConfigName, Cam, DisplayItems);
                // Read emitter data
                E.TextureID = F.ReadUInt16();
                PosX = F.ReadSingle();
                PosY = F.ReadSingle();
                PosZ = F.ReadSingle();
                Pitch = F.ReadSingle();
                Yaw = F.ReadSingle();
                Roll = F.ReadSingle();
                // If config loaded successfully, create the emitter
                if (E.Config != null)
                {
                    uint Texture = Media.GetTexture(E.TextureID, true);
                    if (Texture != 0)
                    {
                        E.Config.ChangeTexture(Texture, false);
                    }
                    Entity EmitterEN = RottParticles.General.CreateEmitter(E.Config);
                    EmitterEN.Parent(E.EN, false);
                    E.EN.Position(PosX, PosY, PosZ);
                    E.EN.Rotate(Pitch, Yaw, Roll);
                }
                    // Failed to load config
                else
                {
                    if (!DisplayItems)
                    {
                        System.Windows.Forms.MessageBox.Show("Could not load emitter: " + ConfigName, "Error");
                    }
                    E.EN.Free();
                    Z.Emitters.Remove(E);
                }
            }
            #endregion

            if (ProgUpdate != null)
            {
                ProgUpdate(80);
            }

            #region Load basic mesh terrains
            ushort Terrains = F.ReadUInt16();
            //if (Terrains > 0)
//                throw new Exception("TODO: JB - Clean up this section!");

            //RCTTerrain T = new RCTTerrain(Z);
            //T.Path = @".\Data\Terrains\Test.te";
            //T.Terrain = GE.TerrainManager.CreateT1(2048);//GE.TerrainManager.LoadT1(T.Path);
            //T.Terrain.Tag = new List<Program.TerrainTagItem>();

            for (int i = 0; i < Terrains; ++i)
            {
                

                int Len = F.ReadInt32();
                byte[] Buf = new byte[Len];
                F.Read(Buf, 0, Len);
                string TempPath = System.Text.ASCIIEncoding.ASCII.GetString(Buf);
                RealmCrafter.RCT.RCTerrain TempTerrain = GE.TerrainManager.LoadT1(TempPath);
                NGUINet.NVector3 TempPosition = new NGUINet.NVector3(F.ReadSingle(), F.ReadSingle(), F.ReadSingle());

                if (TempTerrain == null)
                    continue;

                RCTTerrain T = new RCTTerrain(Z);
                T.Terrain = TempTerrain;
                T.Path = TempPath;
                T.Terrain.Tag = new List<Program.TerrainTagItem>();
                T.Terrain.SetPosition(TempPosition);
            }

            #endregion

            if (ProgUpdate != null)
            {
                ProgUpdate(95);
            }

            #region Load sound zones
            ushort SoundZones = F.ReadUInt16();
            for (int i = 0; i < SoundZones; ++i)
            {
                SoundZone SZ = new SoundZone(Z, DisplayItems);
                // Transform
                PosX = F.ReadSingle();
                PosY = F.ReadSingle();
                PosZ = F.ReadSingle();
                ScaleX = F.ReadSingle();
                SZ.EN.Scale(ScaleX, ScaleX, ScaleX);
                SZ.EN.Position(PosX, PosY, PosZ);
                // Sound options
                SZ.SoundID = F.ReadUInt16();
                SZ.MusicID = F.ReadUInt16();
                SZ.RepeatTime = F.ReadInt32();
                SZ.Volume = F.ReadByte();
                // Load sound
                if (!DisplayItems)
                {
                    if (SZ.SoundID < 65535)
                    {
                        SZ.LoadedSound = Media.GetSound(SZ.SoundID);
                        string SoundName = Media.GetSoundName(SZ.SoundID);
                        SZ.Is3D = (Blitz.IntFromStr(SoundName.Substring(SoundName.Length - 1)) != 0);
                    }
                    else if (SZ.MusicID < 65535)
                    {
                        SZ.MusicFilename = @"Data\Music\" + Media.GetMusicName(SZ.MusicID);
                    }
                }
            }
            #endregion

            #region Load dynamic lights
            if (F.BaseStream.Position + 1 < F.BaseStream.Length)
            {
                ushort Lights = F.ReadUInt16();
                for (int i = 0; i < Lights; ++i)
                {
                    // Get Version
                    byte Version = F.ReadByte();

                    switch (Version)
                    {
                        case 1:
                            {
                                Light L = new Light(Z, DisplayItems);

                                L.EN.Position(F.ReadSingle(), F.ReadSingle(), F.ReadSingle());
                                L.Red = (byte)F.ReadInt32();
                                L.Green = (byte)F.ReadInt32();
                                L.Blue = (byte)F.ReadInt32();
                                L.Radius = F.ReadSingle();

                                L.Handle.Color(L.Red, L.Green, L.Blue);
                                L.Handle.Radius(L.Radius);
                                L.Handle.Position(L.EN.X(), L.EN.Y(), L.EN.Z());

                                L.UpdateLines();

                                Collision.EntityType(L.EN, (byte)CollisionType.Triangle);
                                Collision.SetCollisionMesh(L.EN);
                                Collision.EntityPickMode(L.EN, PickMode.Polygon);

                                break;
                            }
                        case 2:
                            {
                                Light L = new Light(Z, DisplayItems);

                                L.EN.Position(F.ReadSingle(), F.ReadSingle(), F.ReadSingle());
                                L.Red = (byte)F.ReadInt32();
                                L.Green = (byte)F.ReadInt32();
                                L.Blue = (byte)F.ReadInt32();
                                L.Radius = F.ReadSingle();

                                int Len = F.ReadInt32();
                                byte[] Buf = new byte[Len];
                                F.Read(Buf, 0, Len);
                                string TempName = System.Text.ASCIIEncoding.ASCII.GetString(Buf);

                                foreach (LightFunction LF in LightFunctionList.Functions)
                                {
                                    if (LF.Name.Equals(TempName, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        L.Function = LF;
                                        break;
                                    }
                                }

                                L.Handle.Color(L.Red, L.Green, L.Blue);
                                L.Handle.Radius(L.Radius);
                                L.Handle.Position(L.EN.X(), L.EN.Y(), L.EN.Z());

                                L.UpdateLines();

                                Collision.EntityType(L.EN, (byte)CollisionType.Triangle);
                                Collision.SetCollisionMesh(L.EN);
                                Collision.EntityPickMode(L.EN, PickMode.Polygon);

                                break;
                            }
                    }
                }
            }
            #endregion

            #region Load world setup (Marian Voicu) (Edited by Jared Belkus)

            int RainSoundId = 65535;
            int StormSoundId0 = 65535;
            int StormSoundId1 = 65535;
            int StormSoundId2 = 65535;
            int WindSoundId = 65535;
            int SnowTextureId = 65535;
            int RainTextureId = 65535;
            int SkyProfile = 65535;
            int CloudProfile = 65535;
            string EnvName = "default";

            try
            {
                RainSoundId = F.ReadUInt16();
                StormSoundId0 = F.ReadUInt16();
                StormSoundId1 = F.ReadUInt16();
                StormSoundId2 = F.ReadUInt16();
                WindSoundId = F.ReadUInt16();
                SnowTextureId = F.ReadUInt16();
                RainTextureId = F.ReadUInt16();
                SkyProfile = F.ReadUInt16();
                CloudProfile = F.ReadUInt16();

                int Len = F.ReadInt32();
                EnvName = System.Text.ASCIIEncoding.ASCII.GetString(F.ReadBytes(Len));

                ushort ControlCnt = F.ReadUInt16();
                for (int i = 0; i < ControlCnt; ++i)
                {
                    MenuControl Mc = new MenuControl(Z, DisplayItems);
                    Mc.ControlType = (MenuControlType)F.ReadByte();
                    Mc.EN.Position(F.ReadSingle(), F.ReadSingle(), F.ReadSingle());
                    Mc.Update();

                    Collision.EntityType(Mc.EN, (byte)CollisionType.Triangle);
                    Collision.SetCollisionMesh(Mc.EN);
                    Collision.EntityPickMode(Mc.EN, PickMode.Polygon);
                }
            }
            catch (System.IO.EndOfStreamException)
            {
            }

//             Environment3D.SetRainSound(RainSoundId);
//             Environment3D.SetWindSound(WindSoundId);
//             Environment3D.SetThunderSounds(StormSoundId0, StormSoundId1, StormSoundId2);
//             Environment3D.SetRainTexture(RainTextureId);
//             Environment3D.SetSnowTexture(SnowTextureId);
//             Environment3D.SetSkyProfiles(SkyProfile, CloudProfile);
            

            #endregion

            Z.EnvironmentName = EnvName;
            Environment3D.SetCurrentZone(Z, Z.EnvironmentName);
            Environment3D.FogNear = Z.FogNear;
            Environment3D.FogFar = Z.FogFar;
            Environment3D.Update(1.0f);

            if (File.Exists(@"Data\Areas\" + ZoneName + ".ltz"))
                Program.CurrentTreeZone = Program.Manager.LoadZone(@"Data\Areas\" + ZoneName + ".ltz");
            else
                Program.CurrentTreeZone = Program.Manager.CreateZone();
            if (Program.CurrentTreeZone == null)
                throw new Exception("Couldn't load TreeZone for " + ZoneName + "!");

            // Free loading screen
            if (LoadQuad != null)
            {
                LoadQuad.Free();
            }

            // Unlock media databases and close file
            Media.UnlockMeshes();
            Media.UnlockTextures();
            F.Close();



            if (ProgUpdate != null)
            {
                ProgUpdate(100);
            }

            // todo: The data storage mechanism needs to be augmented so that it can store the
            // value of the Default Zone checkbox (in the Weather group on the Zone Setup tab
            // on the World tab). When that's done and the client has been modified to use it,
            // add code here to read it and set the ClientZone's DefaultZone property
            // accordingly.



            // Done, return zone handle
            return Z;
        }

        public bool Save()
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                BinaryWriter F = Blitz.WriteFile(@"Data\Areas\" + Name + ".dat");
                if (F == null)
                {
                    return false;
                }

                // Loading screen
                F.Write(LoadingTexID);
                F.Write(LoadingMusicID);

                // Environment
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));

                F.Write(FogR);
                F.Write(FogG);
                F.Write(FogB);
                F.Write(FogNear);
                F.Write(FogFar);

                //F.Write(MapMarkerTexID);
                F.Write(MapTexID);
                F.Write(Outdoors);
                F.Write(AmbientR);
                F.Write(AmbientG);
                F.Write(AmbientB);
                F.Write(DefaultLightPitch);
                F.Write(DefaultLightYaw);
                F.Write(SlopeRestrict);

                int i;

                // Scenery
                Scenery S;
                F.Write((ushort) Sceneries.Count);
                for (i = 0; i < Sceneries.Count; ++i)
                {
                    S = (Scenery) Sceneries[i];
                    F.Write(S.MeshID);
                    F.Write(S.EN.X());
                    F.Write(S.EN.Y());
                    F.Write(S.EN.Z());
                    F.Write(S.EN.Pitch());
                    F.Write(S.EN.Yaw());
                    F.Write(S.EN.Roll());
                    F.Write(S.EN.ScaleX());
                    F.Write(S.EN.ScaleY());
                    F.Write(S.EN.ScaleZ());
                    F.Write(S.AnimationMode);
                    F.Write(S.SceneryID);
                    F.Write(S.TextureID);

                    // Remove interactive flag is namescript is null
                    if (string.IsNullOrEmpty(S.NameScript))
                        S.Interactive = false;

                    byte Flags = 0;
                    Flags += (byte)(S.CatchRain ? 1 : 0);
                    Flags += (byte)(S.Interactive ? 2 : 0);

                    F.Write(Flags);
                    // LOD
                    F.Write((byte)255);   // Is New Version
                    F.Write(S.EN.distLOD_High);
                    F.Write(S.EN.MeshLOD_Medium);
                    F.Write(S.EN.distLOD_Medium);
                    F.Write(S.EN.MeshLOD_Low);
                    F.Write(S.EN.distLOD_Low);
                    //F.Write(S.NameScript);
                    if ((CollisionType) Collision.EntityType(S.EN) != CollisionType.PickableNone)
                    {
                        F.Write((byte) Collision.EntityType(S.EN));
                        if ((CollisionType)Collision.EntityType(S.EN) == CollisionType.Triangle) F.Write(S.CollisionMeshID);
                    }
                    else
                    {
                        F.Write((byte) 0);
                    }
                    Blitz.WriteString(S.Lightmap, F);
                    Blitz.WriteString(S.RCTE, F); // Extra data for RTCE
                }

                // Water
                Water W;
                F.Write((ushort) Waters.Count);
                for (i = 0; i < Waters.Count; ++i)
                {
                    W = (Water) Waters[i];
                    F.Write( ShaderManager.GetShaderID(W.ShaderID) );
                    F.Write(W.TexScale);
                    F.Write(W.EN.X(true));
                    F.Write(W.EN.Y(true));
                    F.Write(W.EN.Z(true));
                    F.Write(W.EN.ScaleX(true));
                    F.Write(W.EN.ScaleZ(true));
                    F.Write(W.Red);
                    F.Write(W.Green);
                    F.Write(W.Blue);
                    F.Write(W.Alpha);

                    for (int t = 0; t < 4; ++t)
                       F.Write((short)W.TextureID[t]);

                    int nParams = RenderingServices.RenderWrapper.GetNodeParameterCount(W.EN.Handle);
                    F.Write(nParams);
                    for (int iParam = 0; iParam < nParams; ++iParam)
                    {
                        string MName = RenderingServices.RenderWrapper.GetNodeParameterName(W.EN.Handle, iParam);
                        object v = RenderingServices.RenderWrapper.GetEntityParameterValue(W.EN.Handle, MName);
                        F.Write(MName);
                             if (v.GetType() == typeof(Vector1)) { F.Write((Byte)1); F.Write(((Vector1)v).X); }
                        else if (v.GetType() == typeof(Vector2)) { F.Write((Byte)2); F.Write(((Vector2)v).X); F.Write(((Vector2)v).Y); }
                        else if (v.GetType() == typeof(Vector3)) { F.Write((Byte)3); F.Write(((Vector3)v).X); F.Write(((Vector3)v).Y); F.Write(((Vector3)v).Z); }
                        else if (v.GetType() == typeof(Vector4)) { F.Write((Byte)4); F.Write(((Vector4)v).X); F.Write(((Vector4)v).Y); F.Write(((Vector4)v).Z); F.Write(((Vector4)v).W); }
                        else F.Write((Byte)0);
                    }

                }

                // Collision boxes
                ColBox CB;
                F.Write((ushort) ColBoxes.Count);
                for (i = 0; i < ColBoxes.Count; ++i)
                {
                    CB = (ColBox) ColBoxes[i];
                    F.Write(CB.EN.X());
                    F.Write(CB.EN.Y());
                    F.Write(CB.EN.Z());
                    F.Write(CB.EN.Pitch());
                    F.Write(CB.EN.Yaw());
                    F.Write(CB.EN.Roll());
                    F.Write(CB.EN.ScaleX());
                    F.Write(CB.EN.ScaleY());
                    F.Write(CB.EN.ScaleZ());
                }

                // Emitters
                Emitter E;
                F.Write((ushort) Emitters.Count);
                for (i = 0; i < Emitters.Count; ++i)
                {
                    E = (Emitter) Emitters[i];
                    Blitz.WriteString(E.Config.Name, F);
                    F.Write(E.TextureID);
                    F.Write(E.EN.X());
                    F.Write(E.EN.Y());
                    F.Write(E.EN.Z());
                    F.Write(E.EN.Pitch());
                    F.Write(E.EN.Yaw());
                    F.Write(E.EN.Roll());
                }

                // Terrains
                RCTTerrain T;
                F.Write((ushort)Terrains.Count);
                for (i = 0; i < Terrains.Count; ++i)
                {
                    T = (RCTTerrain)Terrains[i];
                    F.Write(Convert.ToInt32(T.Path.Length));
                    F.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(T.Path));
                    GE.TerrainManager.SaveT1(T.Terrain, T.Path);
                    F.Write(T.Terrain.GetPosition().X);
                    F.Write(T.Terrain.GetPosition().Y);
                    F.Write(T.Terrain.GetPosition().Z);
                }

                // Sound zones
                SoundZone SZ;
                F.Write((ushort) SoundZones.Count);
                for (i = 0; i < SoundZones.Count; ++i)
                {
                    SZ = (SoundZone) SoundZones[i];
                    F.Write(SZ.EN.X(true));
                    F.Write(SZ.EN.Y(true));
                    F.Write(SZ.EN.Z(true));
                    F.Write(SZ.EN.ScaleX(true));
                    F.Write(SZ.SoundID);
                    F.Write(SZ.MusicID);
                    F.Write(SZ.RepeatTime);
                    F.Write(SZ.Volume);
                }

                // Lights
                Light L;
                F.Write((ushort) Lights.Count);
                for (i = 0; i < Lights.Count; ++i)
                {
                    L = (Light) Lights[i];

                    // Write supported version
                    F.Write((byte) 2);

                    // Position
                    F.Write(L.EN.X());
                    F.Write(L.EN.Y());
                    F.Write(L.EN.Z());

                    // Color
                    F.Write((int) L.Red);
                    F.Write((int) L.Green);
                    F.Write((int) L.Blue);

                    // Radius
                    F.Write(L.Radius);

                    string Func = "";
                    if (L.Function != null)
                        Func = L.Function.Name;

                    F.Write(Convert.ToInt32(Func.Length));
                    F.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(Func));
                }

                // World Setup (Marian Voicu) (Edited by Jared)
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                F.Write(Convert.ToUInt16(65535));
                // end (MV)

                F.Write(EnvironmentName.Length);
                F.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(EnvironmentName));

                // Menu controllers
                F.Write((ushort)MenuControls.Count);
                foreach (MenuControl Mc in MenuControls)
                {
                    F.Write((byte)Mc.ControlType);
                    F.Write(Mc.EN.X());
                    F.Write(Mc.EN.Y());
                    F.Write(Mc.EN.Z());
                }


                // Trees
                if (Program.CurrentTreeZone != null)
                {
                    Program.Manager.SaveZone(Program.CurrentTreeZone, @"Data\Areas\" + Name + ".ltz");
                }

                F.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Unload()
        {
            // Free all entities
            FreeList(Sceneries);
            FreeList(Waters);
            FreeList(ColBoxes);
            FreeList(Emitters);
            FreeList(Terrains);
            FreeList(SoundZones);
            FreeList(Lights);
            FreeList(MenuControls);
            Sceneries.Clear();
            Waters.Clear();
            ColBoxes.Clear();
            Emitters.Clear();
            Terrains.Clear();
            SoundZones.Clear();
            Lights.Clear();
            MenuControls.Clear();

            // Trees
            if (Program.CurrentTreeZone != null)
            {
                Program.CurrentTreeZone.Destroy();
                Program.CurrentTreeZone = null;
            }
        }

        // Shows/hides all 3D entities in this zone
        public void SetVisible(bool Visible)
        {
            SetListVisible(Sceneries, Visible);
            SetListVisible(Waters, Visible);
            SetListVisible(ColBoxes, Visible);
            SetListVisible(Emitters, Visible);
            SetListVisible(Terrains, Visible);
            SetListVisible(SoundZones, Visible);
            SetListVisible(Lights, Visible);
            SetListVisible(MenuControls, Visible);
            if (Visible)
                Environment3D.Show();
            else
                Environment3D.Hide();
            for (int i = 0; i < Emitters.Count; ++i)
            {
                ZoneObject Z = Emitters[i];
                if (Z.EN != null)
                {
                    RottParticles.General.ShowEmitter(Z.EN.GetChild(1), Visible);
                }
            }
        }

        public void ReloadEmitters()
        {
            /*
             * 
             * Emitter E = new Emitter(Z, ConfigName, Cam, DisplayItems);
                // Read emitter data
                E.TextureID = F.ReadUInt16();
                PosX = F.ReadSingle();
                PosY = F.ReadSingle();
                PosZ = F.ReadSingle();
                Pitch = F.ReadSingle();
                Yaw = F.ReadSingle();
                Roll = F.ReadSingle();
                // If config loaded successfully, create the emitter
                if (E.Config != null)
                {
                    uint Texture = Media.GetTexture(E.TextureID, true);
                    if (Texture != 0)
                    {
                        E.Config.ChangeTexture(Texture, false);
                    }
                    Entity EmitterEN = RottParticles.General.CreateEmitter(E.Config);
                    EmitterEN.Parent(E.EN, false);
                    E.EN.Position(PosX, PosY, PosZ);
                    E.EN.Rotate(Pitch, Yaw, Roll);
                }
             */
            //List<ZoneObject> TempEmitters = new List<ZoneObject>();
            int EmitterCount = Emitters.Count;
            string[] ConfigNames = new string[EmitterCount];
            ushort[] TextureID = new ushort[EmitterCount];
            float[] PosX = new float[EmitterCount];
            float[] PosY = new float[EmitterCount];
            float[] PosZ = new float[EmitterCount];
            float[] Pitch = new float[EmitterCount];
            float[] Yaw = new float[EmitterCount];
            float[] Roll = new float[EmitterCount];
            for (int i = 0; i < Emitters.Count; i++)
            {
                 Emitter F = (Emitter) Emitters[i];
                ConfigNames[i] = F.ConfigName;
                 TextureID[i] = F.TextureID;
                 PosX[i] = Emitters[i].EN.X();
                 PosY[i] = Emitters[i].EN.Y();
                 PosZ[i] = Emitters[i].EN.Z();
                 Pitch[i] = Emitters[i].EN.Pitch();
                 Yaw[i] = Emitters[i].EN.Yaw();
                 Roll[i] = Emitters[i].EN.Roll();
            }

            FreeList(Emitters);
            Emitters.Clear();
            for (int i = 0; i < EmitterCount; i++)
            {
                Emitter E = new Emitter(this, ConfigNames[i], StoredCamera, StoredDisplayItems);
                if (E.Config != null)
                {
                    uint Texture = Media.GetTexture(TextureID[i], true);
                    if (Texture != 0)
                    {
                        E.Config.ChangeTexture(Texture, false);
                    }
                    Entity EmitterEN = RottParticles.General.CreateEmitter(E.Config);
                    EmitterEN.Parent(E.EN, false);
                    E.EN.Position(PosX[i], PosY[i], PosZ[i]);
                    E.EN.Rotate(Pitch[i], Yaw[i], Roll[i]);

                }
            }

        }
        // Traverses a linked list of ZoneObjects and sets the visible state of their 3D entity
        public void SetListVisible(List<ZoneObject> List, bool Visible)
        {
            for (int i = 0; i < List.Count; ++i)
            {
                ZoneObject Z = List[i];
                if (Z.EN != null)
                {
                    Z.EN.Visible = Visible;
                }
            }

            if (Visible)
            {
                RenderWrapper.SetRenderSolidCallbackRT(0, GE.TerrainManager.GetRenderCallback());
                RenderWrapper.SetRenderShadowDepthCallback(0, GE.TerrainManager.GetRenderDepthCallback());
            }
            else
            {
                RenderWrapper.SetRenderSolidCallbackRT(0, IntPtr.Zero);
                RenderWrapper.SetRenderShadowDepthCallback(0, IntPtr.Zero);
            }
        }

        // Traverses a linked list of ZoneObjects and frees their 3D entities
        private void FreeList(List<ZoneObject> List)
        {
            for (int i = 0; i < List.Count; ++i)
            {
                ZoneObject Z = List[i];
                Z.Freeing();
                if (Z.EN != null)
                {
                    if (Z is Emitter)
                    {
                        RottParticles.General.FreeEmitter(Z.EN.GetChild(1), true, false);
                    }
                    Z.EN.Free();
                }
                if (Z is RCTTerrain)
                {

                    RCTTerrain T = (RCTTerrain)Z;
                    GE.TerrainManager.Destroy(T.Terrain);
                    
                    T.Path = "";
                    if (Program.GE.m_TerrainEditor.Terrain == T.Terrain)
                    {
                        if (Program.GE.m_TerrainEditor.EditorMode)
                            Program.GE.m_TerrainEditor.EditorModeButton_Click(null, EventArgs.Empty);
                        //Program.GE.m_TerrainEditor.Terrain = null;
                        //Program.GE.m_TerrainEditor.EditorMode = false;
                        //Program.GE.m_TerrainEditor.RefreshEditorButton(0);
                    }
                    T.Terrain = null;
                }
            }
        }

        // Removes a ZoneObject from this zone
        public void RemoveObject(ZoneObject Obj)
        {
            if (Obj is Scenery)
            {
                Sceneries.Remove(Obj);
            }
            else if (Obj is Water)
            {
                Waters.Remove(Obj);
            }
            else if (Obj is ColBox)
            {
                ColBoxes.Remove(Obj);
            }
            else if (Obj is Emitter)
            {
                Emitters.Remove(Obj);
            }
            else if (Obj is RCTTerrain)
            {
                Terrains.Remove(Obj);
            }
            else if (Obj is SoundZone)
            {
                SoundZones.Remove(Obj);
            }
            else if (Obj is Light)
            {
                (Obj as Light).Handle.Free();
                Lights.Remove(Obj);
            }
            else if (Obj is MenuControl)
            {
                MenuControls.Remove(Obj);
            }
        }

        // Sets the camera view distance for this zone
        public void SetViewDistance(Entity Cam, float Near, float Far)
        {
            FogNear = Near;
            FogFar = Far;
            Cam.CameraRange(1.5f, Far + 10f);
            Render.FogRange(Near, Far);
            Environment3D.SetViewDistance(Near, Far, true);
            //float Scale = Far - 10f;
            //Sky.Scale(Scale, Scale, Scale);
            //Stars.Scale(Scale, Scale, Scale);
            //Cloud.Scale(Scale, Scale, Scale);
        }
    }

    // Base class for all zone objects
    public class ZoneObject
    {
        // 3D entity
        private Entity PrivateEN;

        [Browsable(false)]
        public Entity EN
        {
            get { return PrivateEN; }
            set
            {
                PrivateEN = value;
                if (PrivateEN != null)
                {
                    PrivateEN.ExtraData = this;
                }
            }
        }

        // Locked and cannot be edited
        public bool Locked;

        // Overriden by objects which store information in the server zone object
        public virtual void UpdateServerVersion(RealmCrafter.ServerZone.Zone CurrentServerZone)
        {
        }

        // Overridder by objects which don't use BBDX entities
        public virtual void UpdateTransform()
        {
        }

        public virtual void UpdateTransformTool() { }

        public virtual void Freeing() { }

        public virtual void MoveInit() { }
        public virtual void ScaleInit() { }
        public virtual void RotateInit() { }

        public virtual void Moving(object sender, EventArgs e) { }
        public virtual void Moved(object sender, EventArgs e) { }
        public virtual void Scaling(object sender, EventArgs e) { }
        public virtual void Scaled(object sender, EventArgs e) { }
        public virtual void Rotating(object sender, EventArgs e) { }
        public virtual void Rotated(object sender, EventArgs e) { }


        // Linked lists
        public ZoneObject Next;

        // Constructor
        public ZoneObject()
        {
            Locked = false;
        }
    }

    // Scenery mesh object
    public class Scenery : ZoneObject
    {
        public byte SceneryID; // Set by user, used for scenery ownerships
        public ushort MeshID, TextureID; // TextureID only used to override automatically loaded textures
        public byte AnimationMode; // 0 = none, 1 = constant loop, 2 = constant ping-pong, 3 = when selected
        public bool CatchRain; // Generates a CatchPlane when loaded in client
        public bool Interactive;
        public string Lightmap;
        public string RCTE;
        public string NameScript;

        public ushort CollisionMeshID;
        public Entity MoverPivot;

        // Constructor
        public Scenery(Zone Z, ushort Mesh) : base()
        {
            // Add to list
            Z.Sceneries.Add(this);

            // Default values
            SceneryID = 0;
            TextureID = 65535;
            AnimationMode = 0;
            CatchRain = false;
            Interactive = false;
            Lightmap = "";
            RCTE = "";
            NameScript = "";

            MoverPivot = Entity.CreatePivot();

            // Load mesh
            MeshID = CollisionMeshID = Mesh;
            if (MeshID < 65535)
            {
                EN = Media.GetMesh(Mesh);
                if (EN != null)
                {
                    float Scale = Media.LoadedMeshScales[MeshID] * 0.05f;
                    EN.Scale(Scale, Scale, Scale);
                    EN.RenderMask = 1;
                }
            }
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void ScaleInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new ScaleTool(MoverPivot, new EventHandler(Scaling), new EventHandler(Scaled));
            (Program.Transformer as ScaleTool).Multiplier = 0.05f;
        }

        public override void RotateInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new RotateTool(MoverPivot, new EventHandler(Rotating), new EventHandler(Rotated));
            
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            Program.GE.SetWorldSavedStatus(false);
        }


        public override void Scaling(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Scale(MoverPivot.ScaleX() / 200.0f, MoverPivot.ScaleY() / 200.0f, MoverPivot.ScaleZ() / 200.0f);
            Program.GE.SetWorldSavedStatus(false);
        }

        public override void Rotating(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Rotate(MoverPivot.Pitch(), MoverPivot.Yaw(), MoverPivot.Roll());
            Program.GE.SetWorldSavedStatus(false);
        }

    }

    // Tree holder object
    public class Tree : ZoneObject
    {
        public LTNet.TreeInstance Instance;
        public uint ActiveID;
        public TreeNode Node;

        public Tree(LTNet.TreeInstance instance, Entity en)
        {
            Instance = instance;
            EN = en;
            Node  = new TreeNode("Loading...");
            Node.Tag = this;
        }

        public override void UpdateTransform()
        {
            if (Instance == null || EN == null)
                return;

            float x = EN.ScaleX();
            float y = EN.ScaleY();
            float z = EN.ScaleZ();



            //Instance.SetScale(x, y, z);
            Instance.SetPosition(EN.X(), EN.Y(), EN.Z());
            
            //Instance.SetScale(EN.ScaleX(), EN.ScaleY(), EN.ScaleZ());
            Instance.SetRotation(EN.Pitch(), EN.Yaw(), EN.Roll());
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            Program.Transformer = new MoveTool(EN, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void ScaleInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            //Program.Transformer = new ScaleTool(EN, new EventHandler(Scaling), new EventHandler(Scaled));
        }

        public override void RotateInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            Program.Transformer = new RotateTool(EN, new EventHandler(Rotating), new EventHandler(Rotated));
        }

        public override void Moved(object sender, EventArgs e)
        {
            EN.Position(Instance.X(), Instance.Y(), Instance.Z());
            Program.GE.SetWorldSavedStatus(false);
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (EN == null || Instance == null)
                return;

            Instance.SetPosition(EN.X(), EN.Y(), EN.Z());
        }

        public override void Scaled(object sender, EventArgs e)
        {
            Program.GE.SetWorldSavedStatus(false);
        }

        public override void Scaling(object sender, EventArgs e)
        {
            //if (EN == null || Instance == null)
            //    return;

            //Instance.SetScale(EN.ScaleX(), EN.ScaleY(), EN.ScaleZ());
        }

        public override void Rotated(object sender, EventArgs e)
        {
            Program.GE.SetWorldSavedStatus(false);
        }

        public override void Rotating(object sender, EventArgs e)
        {
            if (EN == null || Instance == null)
                return;

            Instance.SetRotation(EN.Pitch(), EN.Yaw(), EN.Roll());
        }
    }

    public class TreePlacerNode : ZoneObject
    {
        public TreePlacerArea Parent;
        public TreeNode TN;

        public TreePlacerNode(TreePlacerArea parent)
        {
            TN = new TreeNode("Placer Node");
            TN.Tag = this;
            Parent = parent;
            EN = Entity.CreatePivot();// Program.TreeCapsule.Copy();
            //EN.Visible = true;
            //EN.Shader = Program.PhysicsEditorShader;
            //EN.Alpha(0.5f);
            //Collision.EntityBox(EN, 0, 0, 0, 1.0f, 10, 10.0f);
            Collision.EntityPickMode(EN, PickMode.Polygon);
            //EN.Scale(0.1f, 10, 0.1f);

            uint Desc = RenderWrapper.bbdx2_CreatePhysicsDesc(1, EN.Handle);
            
            RenderWrapper.bbdx2_AddBox(Desc,
                0,
                0,
                0,
                2,
                10,
                2,
                -1.0f);

            RenderWrapper.bbdx2_ClosePhysicsDesc(Desc);

        }

        public void Remove()
        {
            if (Program.Transformer != null && Program.Transformer.HasParent(EN))
            {
                Program.Transformer.Free();
                Program.Transformer = null;
            }

            if (EN != null)
                EN.Free();
            EN = null;
        }

        public override void UpdateTransform()
        {
            float X = EN.X();
            float Z = EN.Z();

            Entity Picked = Collision.LinePick(X, 50000, Z, 0, -100000, 0, 0);
            float Y = 0.0f;
            if (Picked != null)
            {
                Y = Collision.PickedY();

                foreach (TreePlacerNode Node in Parent.Nodes)
                {
                    if (Picked.Handle == Node.EN.Handle)
                    {
                        Y -= 5.0f;
                        break;
                    }
                }
            }

            EN.Position(X, Y, Z);

            Parent.RebuildAll();
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            Program.Transformer = new MoveTool(EN, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void ScaleInit()
        {

        }

        public override void RotateInit()
        {

        }

        public override void Moved(object sender, EventArgs e)
        {
            UpdateTransform();
        }

        public override void Moving(object sender, EventArgs e)
        {
            Parent.Rebuild();
        }
    }

    public class TreePlacerType
    {
        public StoredTree Tree;
        bool randomizeRotation = true;
        float heightVariance = 0.0f;

        public TreePlacerType(StoredTree tree)
        {
            this.Tree = tree;
        }

        public override string ToString()
        {
            return Tree.ToString();
        }

        [CategoryAttribute("Tree Properties")]
        public bool RandomizeRotation
        {
            get { return randomizeRotation; }
            set { randomizeRotation = value; }
        }

        [CategoryAttribute("Tree Properties")]
        public float HeightVariance
        {
            get { return heightVariance; }
            set { heightVariance = value; }
        }
    }

    public class TreePlacerArea : ZoneObject
    {
        public List<TreePlacerNode> Nodes = new List<TreePlacerNode>();
        public List<TreePlacerType> PlacementTypes = new List<TreePlacerType>();
        List<Line3D> Lines = new List<Line3D>();
        List<Entity> PlaceMarkers = new List<Entity>();
        public TreeNode TN;

        public int seed = 1;
        public float coverage = 0.1f;

        public TreePlacerArea(Zone Z, float x, float z)
        {
            TN = new TreeNode("Tree Placer");
            TN.Tag = this;
            EN = Entity.CreatePivot();


            AddPlacerNode(x - 10, z + 10, false);
            AddPlacerNode(x + 10, z + 10, false);
            AddPlacerNode(x + 10, z - 10, false);
            AddPlacerNode(x - 10, z - 10, true);

        }

        public void Hide()
        {
            foreach (Entity E in PlaceMarkers)
            {
                E.Visible = false;
            }
        }

        public void Show()
        {
            foreach (Entity E in PlaceMarkers)
            {
                E.Visible = true;
            }
        }

        public void AddPlacerNode(float x, float z, bool rebuild)
        {
            TreePlacerNode Node = new TreePlacerNode(this);
            Nodes.Add(Node);
            TN.Nodes.Add(Node.TN);

            Entity Picked = Collision.LinePick(x, 50000, z, 0, -100000, 0, 0);
            float Y = 0.0f;
            if(Picked != null)
                Y = Collision.PickedY();

            Node.EN.Position(x, Y, z);

            if (rebuild)
                RebuildAll();
        }

        public void RemoveNodes(int count)
        {
            int NodeCount = Nodes.Count;
            for (int i = NodeCount - 1; i > NodeCount - (count + 1); --i)
            {
                TreePlacerNode Node = Nodes[i];
                TN.Nodes.Remove(Node.TN);
                Node.Remove();
                Nodes.Remove(Node);
            }

            RebuildAll();
        }

        public void Rebuild()
        {
            foreach (Line3D L in Lines)
            {
                L.Free();
            }
            Lines.Clear();

            if (Nodes.Count < 2)
                return;

            TreePlacerNode LastNode = Nodes[Nodes.Count - 1];
            foreach (TreePlacerNode Node in Nodes)
            {
                if (LastNode != null && Node != null && LastNode.EN != null && Node.EN != null)
                {
                    Line3D Top = new Line3D(LastNode.EN.X(), LastNode.EN.Y() - 5, LastNode.EN.Z(),
                        Node.EN.X(), Node.EN.Y() - 5, Node.EN.Z(), false);

                    Line3D Bottom = new Line3D(LastNode.EN.X(), LastNode.EN.Y() + 5, LastNode.EN.Z(),
                        Node.EN.X(), Node.EN.Y() + 5, Node.EN.Z(), false);

                    Line3D Slice = new Line3D(Node.EN.X(), Node.EN.Y() + 5, Node.EN.Z(),
                        Node.EN.X(), Node.EN.Y() - 5, Node.EN.Z(), false);

                    Top.SetColor(255, 0, 0);
                    Bottom.SetColor(255, 0, 0);
                    Slice.SetColor(255, 0, 0);

                    Lines.Add(Top);
                    Lines.Add(Bottom);
                    Lines.Add(Slice);

                }

                LastNode = Node;
            }

        }

        public void RebuildAll()
        {
            Rebuild();

            foreach (Entity E in PlaceMarkers)
            {
                if(E != null)
                    E.Free();
            }
            PlaceMarkers.Clear();

            Random RandomPos = new Random(seed);
            Random RandomType = new Random(seed);

            Vector2 Min = new Vector2();
            Vector2 Max = new Vector2();
            Vector2 Range = new Vector2();
            bool Set = false;

            foreach (TreePlacerNode Node in Nodes)
            {
                if (Node != null && Node.EN != null)
                {
                    Vector3 Position = new Vector3(Node.EN.X(), Node.EN.Y(), Node.EN.Z());

                    if (Set == false)
                    {
                        Min.X = Position.X;
                        Min.Y = Position.Z;

                        Max.X = Min.X;
                        Max.Y = Min.Y;
                        Set = true;
                    }

                    if (Position.X < Min.X)
                        Min.X = Position.X;
                    if (Position.Z < Min.Y)
                        Min.Y = Position.Z;
                    if (Position.X > Max.X)
                        Max.X = Position.X;
                    if (Position.Z > Max.Y)
                        Max.Y = Position.Z;
                }
            }

            Range.X = Max.X - Min.X;
            Range.Y = Max.Y - Min.Y;

            for (float X = Min.X; X < Max.X; X += (1.0f / coverage))
            {
                for (float Z = Min.Y; Z < Max.Y; Z += (1.0f / coverage))
                {
                    float RandomX = X + (Convert.ToSingle(RandomPos.NextDouble()) * (1.0f / coverage));
                    float RandomZ = Z + (Convert.ToSingle(RandomPos.NextDouble()) * (1.0f / coverage));

                    Vector2 RandomP = new Vector2(RandomX, RandomZ);
                    if (IsPointInside(RandomP))
                    {
                        Entity E = Program.TreeCapsule.Copy();
                        E.Visible = true;
                        E.Shader = Program.PhysicsEditorShader;
                        E.Alpha(0.5f);
                        E.Scale(1f, 4, 1f);

                        Entity Picked = Collision.LinePick(RandomX, 50000, RandomZ, 0, -100000, 0, 0);
                        float Y = 0.0f;
                        if (Picked != null)
                            Y = Collision.PickedY();

                        E.Position(RandomX, Y + 5.0f, RandomZ);
                        PlaceMarkers.Add(E);
                    }

                }
            }
        }

        public void BuildFinal()
        {
            if (PlacementTypes.Count == 0)
            {
                MessageBox.Show("Error: This tree placer has no trees assigned to it!");
                return;
            }
            Rebuild();

            Random RandomPos = new Random(seed);
            Random RandomType = new Random(seed);
            Random Scaler = new Random();

            Vector2 Min = new Vector2();
            Vector2 Max = new Vector2();
            Vector2 Range = new Vector2();
            bool Set = false;

            foreach (TreePlacerNode Node in Nodes)
            {
                Vector3 Position = new Vector3(Node.EN.X(), Node.EN.Y(), Node.EN.Z());

                if (Set == false)
                {
                    Min.X = Position.X;
                    Min.Y = Position.Z;

                    Max.X = Min.X;
                    Max.Y = Min.Y;
                    Set = true;
                }

                if (Position.X < Min.X)
                    Min.X = Position.X;
                if (Position.Z < Min.Y)
                    Min.Y = Position.Z;
                if (Position.X > Max.X)
                    Max.X = Position.X;
                if (Position.Z > Max.Y)
                    Max.Y = Position.Z;
            }

            Range.X = Max.X - Min.X;
            Range.Y = Max.Y - Min.Y;

            Program.CurrentTreeZone.Lock();

            for (float X = Min.X; X < Max.X; X += (1.0f / coverage))
            {
                for (float Z = Min.Y; Z < Max.Y; Z += (1.0f / coverage))
                {
                    float RandomX = X + (Convert.ToSingle(RandomPos.NextDouble()) * (1.0f / coverage));
                    float RandomZ = Z + (Convert.ToSingle(RandomPos.NextDouble()) * (1.0f / coverage));

                    Vector2 RandomP = new Vector2(RandomX, RandomZ);
                    if (IsPointInside(RandomP))
                    {
                        float Yaw = 0.0f;
                        float ScaleY = 1.0f;
                        TreePlacerType TType = PlacementTypes[RandomType.Next(0, PlacementTypes.Count)];

                        if (TType.RandomizeRotation)
                            Yaw = Convert.ToSingle(Scaler.NextDouble()) * 360.0f;

                        float Offset = Convert.ToSingle(TType.HeightVariance);
                        ScaleY += (Convert.ToSingle(Scaler.NextDouble()) * Offset) - (Offset * 0.5f);

                        Entity Picked = Collision.LinePick(RandomX, 50000, RandomZ, 0, -100000, 0, 0);
                        float Y = 0.0f;
                        if (Picked != null)
                            Y = Collision.PickedY();
                        LTNet.TreeInstance I = Program.CurrentTreeZone.AddTreeInstance(TType.Tree.LTType, RandomX, Y, RandomZ);
                        I.SetScale(1.0f, ScaleY, 1.0f);
                        I.SetRotation(0, Yaw, 0);
                    }
                }
            }

            Program.CurrentTreeZone.Unlock();

            foreach (TreePlacerNode Node in Nodes)
            {
                Node.Remove();
            }
            Nodes.Clear();
            foreach (Line3D Line in Lines)
            {
                Line.Free();
            }
            Lines.Clear();
            foreach (Entity E in PlaceMarkers)
            {
                E.Free();
            }
            PlaceMarkers.Clear();

            TN.Parent.Nodes.Remove(TN);
            Program.GE.m_ZoneList.ZoneObjectListCheck(this, false);
            Program.GE.m_ZoneList.AddObjectsCount();
            Program.GE.m_propertyWindow.ObjectProperties.SelectedObject = null;
            Program.GE.SetWorldSavedStatus(false);
        }

        public void Remove()
        {
            foreach (TreePlacerNode Node in Nodes)
            {
                Node.Remove();
            }
            Nodes.Clear();
            foreach (Line3D Line in Lines)
            {
                Line.Free();
            }
            Lines.Clear();
            foreach (Entity E in PlaceMarkers)
            {
                E.Free();
            }
            PlaceMarkers.Clear();

            TN.Parent.Nodes.Remove(TN);
            Program.GE.m_ZoneList.ZoneObjectListCheck(this, false);
            Program.GE.m_ZoneList.AddObjectsCount();
            Program.GE.m_propertyWindow.ObjectProperties.SelectedObject = null;
            Program.GE.SetWorldSavedStatus(false);
        }

        bool InsidePolygon(Vector2[] polygon, Vector2 p)
        {
            int counter = 0;
            int i;
            double xinters;
            Vector2 p1, p2;
            int n = polygon.Length;

            p1 = polygon[0];
            for (i = 1; i <= n; i++)
            {
                p2 = polygon[i % n];

                if (p.Y > Math.Min(p1.Y, p2.Y))
                {

                    if (p.Y <= Math.Max(p1.Y, p2.Y))
                    {

                        if (p.X <= Math.Max(p1.X, p2.X))
                        {

                            if (p1.Y != p2.Y)
                            {
                                xinters = (p.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                                if (p1.X == p2.X || p.X <= xinters)
                                    counter++;
                            }
                        }
                    }
                }

                p1 = p2;
            }

            if (counter % 2 == 0)
                return (false);
            else
                return (true);
        }
        public bool IsPointInside(Vector2 point)
        {
            Vector2[] Poly = new Vector2[Nodes.Count];

            for (int i = 0; i < Nodes.Count; ++i)
                Poly[i] = new Vector2(Nodes[i].EN.X(), Nodes[i].EN.Z());

            return InsidePolygon(Poly, point);

        }

        public override string ToString()
        {
            return "Click to Finalize";
        }
    }

    // Water plane object
    public class Water : ZoneObject
    {
        public uint ShaderID;
        public byte Red, Green, Blue, Alpha;
        public float TexScale;
        public int[] TextureID = new int[4];
        public uint[] TexHandle = new uint[4];
        public Entity MoverPivot;

        // Used by editor only
        public RealmCrafter.ServerZone.WaterArea ServerWater;

        public override void UpdateServerVersion(RealmCrafter.ServerZone.Zone CurrentServerZone)
        {
            ServerWater.X = EN.X() - (EN.ScaleX() * 0.5f);
            ServerWater.Z = EN.Z() - (EN.ScaleZ() * 0.5f);
            ServerWater.Y = EN.Y();
            ServerWater.Width = EN.ScaleX();
            ServerWater.Depth = EN.ScaleZ();
        }

        // Creates a water plane mesh
        public static Entity CreatePlane()
        {
            Entity Out = Entity.CreateMesh();
            uint Surf = Out.CreateSurface();
            Entity.AddVertex(Surf, -0.5f, 0f, -0.5f, 0f, 0f);
            Entity.AddVertex(Surf, 0.5f, 0f, -0.5f, 1f, 0f);
            Entity.AddVertex(Surf, 0.5f, 0f, 0.5f, 1f, 1f);
            Entity.AddVertex(Surf, -0.5f, 0f, 0.5f, 0f, 1f);
            Entity.AddTriangle(Surf, 0, 2, 1);
            Entity.AddTriangle(Surf, 0, 3, 2);
            Entity.AddTriangle(Surf, 1, 2, 0);
            Entity.AddTriangle(Surf, 2, 3, 0);
            Out.UpdateNormals();
            Out.UpdateHardwareBuffers();
            Out.AlphaState = true;
            return Out;
        }

        // Constructor
        public Water(Zone Z) : base()
        {
            // Add to list
            Z.Waters.Add(this);

            // Default values
            for (int i=0; i<4; ++i)
            {
                TextureID[i] = 65535;
                TexHandle[i] = 0;
            }
            Red = 0;
            Green = 80;
            Blue = 200;
            Alpha = 50;
            TexScale = 1f;
            
            // Create mesh
            ShaderID = ShaderManager.GetShader("Precomp_CubeMap_Reflection");

            EN = CreatePlane();
            EN.Shader = ShaderID;
            EN.Scale(16f, 1f, 16f);
            EN.Alpha(50f / 100f);
            EN.RenderMask = 1;
            EN.FX = 32;
            Collision.EntityType(EN, (byte) CollisionType.Box);

            MoverPivot = Entity.CreatePivot();
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            UpdateServerVersion(Program.GE.CurrentServerZone);
            Program.GE.SetWorldSavedStatus(false);
        }
    }

    // Invisible collision box object
    public class ColBox : ZoneObject
    {
        public Entity MoverPivot;

        // Constructor
        public ColBox(Zone Z, bool MakeVisible) : base()
        {
            // Add to list
            Z.ColBoxes.Add(this);

            // Create mesh
            EN = Entity.CreateCube();
            EN.Shader = Shaders.FullbrightAlpha;
            Collision.EntityType(EN, (byte) CollisionType.Box);
            Collision.SetCollisionMesh(EN);
            EN.RenderMask = 16;
            if (MakeVisible)
            {
                Collision.EntityPickMode(EN, PickMode.Polygon);
                EN.AlphaNoSolid(0.4f);
                EN.AlphaState = true;
            }
            else
            {
                EN.AlphaNoSolid(0f);
            }
            EN.Scale(5f, 5f, 5f);

            MoverPivot = Entity.CreatePivot();
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void ScaleInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new ScaleTool(MoverPivot, new EventHandler(Scaling), new EventHandler(Scaled));
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
        }


        public override void Scaling(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Scale(MoverPivot.ScaleX() / 200.0f, MoverPivot.ScaleY() / 200.0f, MoverPivot.ScaleZ() / 200.0f);
        }
    }

    // Particles emitter object
    public class Emitter : ZoneObject
    {
        public string ConfigName;
        public RottParticles.EmitterConfig Config;
        public ushort TextureID;

        public Entity MoverPivot;

        // Constructor
        public Emitter(Zone Z, string ConfigName, Entity Cam, bool MakeVisible) : base()
        {
            // Add to list
            Z.Emitters.Add(this);

            // Default values
            this.ConfigName = ConfigName;
            if (!string.IsNullOrEmpty(ConfigName))
            {
                Config = RottParticles.EmitterConfig.Load(@"Data\Emitter Configs\" + ConfigName + ".rpc", Cam, 0);
            }
            TextureID = Config.DefaultTextureID;

            // Create mesh
            if (MakeVisible)
            {
                EN = Entity.CreateSphere();
                EN.Shader = Shaders.FullbrightAlpha;
                EN.Color(255, 255, 255);
                EN.AlphaNoSolid(0.5f);
                EN.RenderMask = 16;
                //Collision.EntityType(EN, (byte) CollisionType.Sphere);
                //Collision.EntityRadius(EN, 2.5f, 2.5f);
                //Collision.EntityPickMode(EN, PickMode.Sphere);
                EN.Scale(2.5f, 2.5f, 2.5f);
                Collision.EntityType(EN, (byte)CollisionType.Triangle);
                Collision.SetCollisionMesh(EN);
                Collision.EntityPickMode(EN, PickMode.Polygon);
            }
            else
            {
                EN = Entity.CreatePivot();
            }

            MoverPivot = Entity.CreatePivot();
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            Program.GE.SetWorldSavedStatus(false);
        }
    }

    // Terrain object
    public class Terrain : ZoneObject
    {
        public const int ChunkDetail = 32;
        public ushort BaseTexID, DetailTexID;
        public float DetailTexScale;
        public int Detail;
        public bool Morph, Shading;
        public uint DetailTex;
        public int GridSize;
        public float[,] Heights; // Only stored in editor, not game

        // Constructor
        public Terrain(Zone Z) : base()
        {
            // Add to list
            Z.Terrains.Add(this);

            // Default values
            DetailTexScale = 15f;
            Detail = 2000;
            DetailTexID = 65535;
            Morph = true;
            Shading = false;
        }
    }
    
    // RCT Object
    public class RCTTerrain : ZoneObject
    {
        public string Path;
        public RealmCrafter.RCT.RCTerrain Terrain;

        // Constructor
        public RCTTerrain(Zone Z)
            : base()
        {
            // Add to list
            Z.Terrains.Add(this);

            // Defaults
            Path = "";
            Terrain = null;
        }
    }

    // Sound zone object
    public class SoundZone : ZoneObject
    {
        public ushort SoundID, MusicID; // Can be one or the other
        public int RepeatTime; // Time in seconds to wait before repeating the sound, -1 to play once only
        public byte Volume; // Percentage 1%-100%
        public Sound LoadedSound;
        public string MusicFilename;
        public bool Is3D;
        public Channel Channel; // For updating the sound zone in the client
        public uint Timer;
        public float Fade;

        public Entity MoverPivot;

        // Constructor
        public SoundZone(Zone Z, bool MakeVisible) : base()
        {
            // Add to list
            Z.SoundZones.Add(this);

            // Default values
            SoundID = 65535;
            MusicID = 65535;
            Volume = 100;

            // Create mesh
            if (MakeVisible)
            {
                EN = Entity.CreateSphere();
                EN.Shader = Shaders.FullbrightAlpha;
                EN.AlphaNoSolid(0.5f);
                EN.AlphaState = true;
                EN.RenderMask = 16;
                Collision.EntityType(EN, (byte) CollisionType.Sphere);
                Collision.SetCollisionMesh(EN);
                Collision.EntityPickMode(EN, PickMode.Polygon);
            }
            else
            {
                EN = Entity.CreatePivot();
            }
            EN.Scale(10f, 10f, 10f);

            MoverPivot = Entity.CreatePivot();
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void ScaleInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new ScaleTool(MoverPivot, new EventHandler(Scaling), new EventHandler(Scaled));
            ScaleTool ST = Program.Transformer as ScaleTool;
            ST.AllowScaleY = false;
            ST.AllowScaleZ = false;
            ST.CopyXToY = true;
            ST.CopyXToZ = true;
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            Program.GE.SetWorldSavedStatus(false);
        }


        public override void Scaling(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Scale(MoverPivot.ScaleX() / 200.0f, MoverPivot.ScaleX() / 200.0f, MoverPivot.ScaleX() / 200.0f);
            Program.GE.SetWorldSavedStatus(false);
        }
    }

    // Dynamic light object
    public class Light : ZoneObject
    {
        public float Radius;
        public byte Red, Green, Blue;
        public bool Flicker, NightOnly;
        public RealmCrafter_GE.LineCircle SurroundXZ, SurroundXY, SurroundZY;
        public RenderingServices.Light Handle;
        public LightFunction Function = null;

        public Entity MoverPivot;

        // Constructor
        public Light(Zone Z, bool MakeVisible) : base()
        {
            // Add to list
            if(Z != null)
                Z.Lights.Add(this);

            // Default values
            Radius = 50f;
            Red = 255;
            Green = 255;
            Blue = 255;
            Flicker = false;
            NightOnly = false;

            // Create mesh
            if (MakeVisible)
            {
                EN = Entity.CreateSphere();
                EN.Shader = Shaders.FullbrightAlpha;
                EN.Alpha(0.5f);
                EN.RenderMask = 16;
                SurroundXZ = new RealmCrafter_GE.LineCircle(40, Radius / 2.0f, 1, EN);
                SurroundXY = new RealmCrafter_GE.LineCircle(40, Radius / 2.0f, 0, EN);
                SurroundZY = new RealmCrafter_GE.LineCircle(40, Radius / 2.0f, 2, EN);
            }
            else
            {
                EN = Entity.CreatePivot();
            }
            //EN.Scale(2f, 2f, 2f);

            Handle = RenderingServices.Light.CreatePointLight();
            Handle.Color(Red, Green, Blue);
            Handle.Radius(Radius);

            MoverPivot = Entity.CreatePivot();
        }

        public override void UpdateServerVersion(RealmCrafter.ServerZone.Zone CurrentServerZone)
        {
            base.UpdateServerVersion(CurrentServerZone);

            Handle.Position(EN.X(), EN.Y(), EN.Z());
            Handle.Color(Red, Green, Blue);

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            UpdateLines();
        }

        public void UpdateLines()
        {
            if (SurroundXY != null)
            {
                SurroundXY.Size = Radius / 2.0f;
                SurroundXY.Color = System.Drawing.Color.FromArgb(Red, Green, Blue);
            }
            if (SurroundXZ != null)
            {
                SurroundXZ.Size = Radius / 2.0f;
                SurroundXZ.Color = System.Drawing.Color.FromArgb(Red, Green, Blue);
            }
            if (SurroundZY != null)
            {
                SurroundZY.Size = Radius / 2.0f;
                SurroundZY.Color = System.Drawing.Color.FromArgb(Red, Green, Blue);
            }
            MoverPivot.Scale(Radius, Radius, Radius);
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(Radius * 200.0f, Radius * 200.0f, Radius * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(Radius * 200.0f, Radius * 200.0f, Radius * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void ScaleInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(Radius * 200.0f, Radius * 200.0f, Radius * 200.0f);

            Program.Transformer = new ScaleTool(MoverPivot, new EventHandler(Scaling), new EventHandler(Scaled));
            ScaleTool ST = Program.Transformer as ScaleTool;
            //ST.AllowScaleY = false;
            //ST.AllowScaleZ = false;
            ST.CopyXToZ = true;
            ST.CopyXToY = true;
            ST.Multiplier = 0.5f;
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            UpdateLines();
            Program.GE.SetWorldSavedStatus(false);
        }


        public override void Scaling(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            Radius = MoverPivot.ScaleX() / 200.0f;
            UpdateLines();
            Program.GE.SetWorldSavedStatus(false);
        }
    }

    // A 'type' of a menu control
    public enum MenuControlType
    {
        CameraPosition = 0,
        CameraFocus = 1,
        ActorSpawn = 2
    }

    // Menu control object
    public class MenuControl : ZoneObject
    {
        public MenuControlType ControlType = MenuControlType.CameraPosition;

        float Radius = 4.0f;
        public Entity MoverPivot;
        public NGUINet.NLabel Label;

        // Constructor
        public MenuControl(Zone Z, bool MakeVisible)
            : base()
        {
            // Add to list
            Z.MenuControls.Add(this);

            // Create mesh
            if (MakeVisible)
            {
                EN = Entity.CreateSphere();
                EN.Shader = Shaders.FullbrightAlpha;
                EN.Alpha(0.5f);
                EN.RenderMask = 16;
                Collision.EntityType(EN, (byte)CollisionType.Triangle);
                Collision.SetCollisionMesh(EN);
                Collision.EntityPickMode(EN, PickMode.Polygon);


                Label = GE.GUIManager.CreateLabel("PORTALNAME", new NGUINet.NVector2(0, 0), new NGUINet.NVector2(0, 0));
                Label.Visible = false;
            }
            else
            {
                EN = Entity.CreatePivot();
            }
            //EN.Scale(2f, 2f, 2f);

            MoverPivot = Entity.CreatePivot();
        }

        public void Update()
        {
            if (Label != null)
                Label.Text = ControlType.ToString();
        }

        public void UpdateLabel()
        {
            UpdateLabel(false);
        }

        public void UpdateLabel(bool hideonly)
        {
            Label.Visible = false;

            if (hideonly)
                return;

            if (Program.GE.RenderingPanelCurrentIndex != -3)
                return;
            if (Program.GE.RenderToggleEditor.Checked)
                return;

            float PHeight = 1.2f * 0.6f;

            float DistX = Program.GE.Camera.X() - EN.X();
            float DistY = Program.GE.Camera.Y() - EN.Y() + PHeight;
            float DistZ = Program.GE.Camera.Z() - EN.Z();
            float Dist = Math.Abs(DistX * DistX + DistY * DistY + DistZ * DistZ);

            if (Dist < 500 * 500)
            {
                RenderWrapper.bbdx2_ManagedProjectVector3(EN.X(), EN.Y() + PHeight, EN.Z());

                float PrX = RenderWrapper.bbdx2_ProjectedX();
                float PrY = RenderWrapper.bbdx2_ProjectedY();
                float PrZ = RenderWrapper.bbdx2_ProjectedZ();


                PrX -= (Label.InternalWidth * 0.5f);

                Label.Location = new NGUINet.NVector2(PrX, PrY);


                if (PrZ > 0.1f && PrZ < 1.0f && PrX > 0.0f && PrX < 1.0f && PrY > 0.0f && PrY < 1.0f)
                    Label.Visible = true;
            }
        }

        public override void UpdateServerVersion(RealmCrafter.ServerZone.Zone CurrentServerZone)
        {
            base.UpdateServerVersion(CurrentServerZone);

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
            if (Label != null)
                GE.GUIManager.Destroy(Label);
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(Radius * 200.0f, Radius * 200.0f, Radius * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(Radius * 200.0f, Radius * 200.0f, Radius * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            Program.GE.SetWorldSavedStatus(false);
        }

    }

    // Generated from scenery objects to prevent rain/snow particles falling through
    public class CatchPlane
    {
        public float MinX, MinZ, MaxX, MaxZ, Y;
    }
}