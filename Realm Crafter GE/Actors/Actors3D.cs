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
// Realm Crafter Actors 3D module by Rob W (rottbott@hotmail.com)
// Original version November 2004, C# port October 2006
using RealmCrafter_GE;

namespace RealmCrafter
{
    using RenderingServices;
    using RottParticles;
    using System;
    using System.IO;
    using System.Collections.Generic;

    // Actors3D class
    public class Actors3D
    {
        // Currently set camera entity
        public static Entity Camera;

        // Gubbin joints map
        public static string[] GubbinJoints = new string[6];

        // Other options
        public static int HideNametags = 0;
        public static bool DisableCollisions = false;

        public static bool LoadGubbinNames()
        {
            BinaryReader F = Blitz.ReadFile(@"Data\Game Data\Gubbins.dat");
            if (F == null)
            {
                return false;
            }

            for (int i = 0; i < 6; ++i)
            {
                GubbinJoints[i] = Blitz.ReadString(F);
            }

            F.Close();

            return true;
        }

        public static bool SaveGubbinNames()
        {
            BinaryWriter F = Blitz.WriteFile(@"Data\Game Data\Gubbins.dat");
            if (F == null)
            {
                return false;
            }

            for (int i = 0; i < 6; ++i)
            {
                Blitz.WriteString(GubbinJoints[i], F);
            }

            F.Close();

            return true;
        }

        // Loads the 3D models for an actor instance
        public static bool LoadActorInstance3D(ActorInstance A, float Scale, bool SkipAttachments)
        {
            A.Actor.Radius = 0f;
            Entity CollisionEN = Entity.CreatePivot();

            // CollisionEN.Position(5f, 0f, 5f);
            A.CollisionEN = CollisionEN;
            MeshMinMaxVertices MMV = null;
            AnimSet ActorAnimSet = null;

            // Main mesh and textures
            if (A.Gender == 0)
            {
                ActorAnimSet = AnimSet.Index[A.Actor.MaleAnimationSet];

                // Main mesh
                Entity EN = Media.GetMesh(A.Actor.MaleMesh);
                A.EN = EN;
                if (EN == null)
                {
                    CollisionEN.Free();
                    return false;
                }

                RenderWrapper.EntityShadowLevel(EN.Handle, 3);
                if (RenderWrapper.GetEntityShadowShader(EN.Handle) == 0)
                    RenderWrapper.EntityShadowShader(EN.Handle, Shaders.DefaultAnimatedDepthShader);

                Scale = Scale * Media.LoadedMeshScales[A.Actor.MaleMesh] * A.Actor.Scale;
                EN.Parent(CollisionEN, true);
                MMV = new MeshMinMaxVertices(EN);
                EN.Position(0f, (MMV.MaxY - MMV.MinY) * -0.5f, 0f);

                // Textures
                uint BodyTex = A.BodyTex;
                uint FaceTex = A.FaceTex;

                ActorTextureSet BodySet = new ActorTextureSet(65535, 65535, 65535, 65535);
                ActorTextureSet FaceSet = new ActorTextureSet(65535, 65535, 65535, 65535);


                if (FaceTex < A.Actor.MaleFaceIDs.Length)
                    FaceSet = A.Actor.MaleFaceIDs[FaceTex];
                if (BodyTex < A.Actor.MaleBodyIDs.Length)
                    BodySet = A.Actor.MaleBodyIDs[BodyTex];

                // Multiple surfaces -- use face and body textures
                if (EN.CountSurfaces() > 1)
                {
                    bool SurfacesBackwards = false;
                    string FirstSurfaceTex = EN.SurfaceTexture(0, 0).ToUpper();

                    // If the name of the first surface contains the word "HEAD", the surfaces are backwards
                    if (FirstSurfaceTex.Contains("HEAD"))
                    {
                        SurfacesBackwards = true;
                    }

                        // If 'HEAD' isn't found at all, check if the assigned texture is in fact one of the face textures
                    else if (!EN.SurfaceTexture(1, 0).ToUpper().Contains("HEAD"))
                    {
                        for (int i = 0; i < A.Actor.MaleFaceIDs.Length; ++i)
                        {
                            if (A.Actor.MaleFaceIDs[i].Tex0 < 65535)
                            {
                                string Name2 = Media.GetTextureName(A.Actor.MaleFaceIDs[i].Tex0).ToUpper();
                                if (!string.IsNullOrEmpty(Name2) && Name2 == FirstSurfaceTex)
                                {
                                    SurfacesBackwards = true;
                                    break;
                                }
                            }
                        }
                    }

                    // Perform the texturing
                    if (SurfacesBackwards)
                    {

                        if (BodySet.Tex0 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex0, false), 0, 1);
                        if (BodySet.Tex1 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex1, false), 1, 1);
                        if (BodySet.Tex2 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex2, false), 2, 1);
                        if (BodySet.Tex3 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex3, false), 3, 1);
                        if (FaceSet.Tex0 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex0, false), 0, 0);
                        if (FaceSet.Tex1 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex1, false), 1, 0);
                        if (FaceSet.Tex2 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex2, false), 2, 0);
                        if (FaceSet.Tex3 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex3, false), 3, 0);
                    }
                    else
                    {
                        if (BodySet.Tex0 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex0, false), 0, 0);
                        if (BodySet.Tex1 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex1, false), 1, 0);
                        if (BodySet.Tex2 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex2, false), 2, 0);
                        if (BodySet.Tex3 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex3, false), 3, 0);
                        if (FaceSet.Tex0 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex0, false), 0, 1);
                        if (FaceSet.Tex1 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex1, false), 1, 1);
                        if (FaceSet.Tex2 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex2, false), 2, 1);
                        if (FaceSet.Tex3 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex3, false), 3, 1);
                    }
                }
                    // One surface -- use body texture only
                else
                {
                    if (BodySet.Tex0 < 65535)
                        EN.Texture(Media.GetTexture(BodySet.Tex0, false), 0, 0);
                    if (BodySet.Tex1 < 65535)
                        EN.Texture(Media.GetTexture(BodySet.Tex1, false), 1, 0);
                    if (BodySet.Tex2 < 65535)
                        EN.Texture(Media.GetTexture(BodySet.Tex2, false), 2, 0);
                    if (BodySet.Tex3 < 65535)
                        EN.Texture(Media.GetTexture(BodySet.Tex3, false), 3, 0);
                }
                
                // Beard
                if(A.Beard < A.Actor.Beards.Count)
                    ShowGubbinSet(A, A.Actor.Beards[A.Beard], A.BeardENs);

                // Get radius
                float MaxLength = EN.MeshWidth;
                if (EN.MeshDepth > MaxLength)
                {
                    MaxLength = EN.MeshDepth;
                }
                A.Actor.Radius = MaxLength * Media.LoadedMeshScales[A.Actor.MaleMesh] * A.Actor.Scale * 0.5f;
            }
            else
            {
                ActorAnimSet = AnimSet.Index[A.Actor.FemaleAnimationSet];
                // Main mesh
                Entity EN = Media.GetMesh(A.Actor.FemaleMesh);
                A.EN = EN;
                if (EN == null)
                {
                    CollisionEN.Free();
                    return false;
                }
                Scale = Scale * Media.LoadedMeshScales[A.Actor.FemaleMesh] * A.Actor.Scale;
                EN.Parent(CollisionEN, true);
                MMV = new MeshMinMaxVertices(EN);
                EN.Position(0f, (MMV.MaxY - MMV.MinY) * -0.5f, 0f);
                // Textures
                uint BodyTex = A.BodyTex;
                uint FaceTex = A.FaceTex;

                ActorTextureSet BodySet = new ActorTextureSet(65535, 65535, 65535, 65535);
                ActorTextureSet FaceSet = new ActorTextureSet(65535, 65535, 65535, 65535);


                if (FaceTex < A.Actor.FemaleFaceIDs.Length)
                    FaceSet = A.Actor.FemaleFaceIDs[FaceTex];
                if (BodyTex < A.Actor.FemaleBodyIDs.Length)
                    BodySet = A.Actor.FemaleBodyIDs[BodyTex];

                // Multiple surfaces -- use face and body textures
                if (EN.CountSurfaces() > 1 && FaceTex != 0)
                {
                    bool SurfacesBackwards = false;
                    // Get texture name of first surface
                    string SurfaceName = EN.SurfaceTexture(0, 0).ToUpper();
                    // If it contains the word "HEAD", the surfaces are backwards
                    if (SurfaceName.Contains("HEAD"))
                    {
                        SurfacesBackwards = true;
                    }
                        // Otherwise, check if the assigned texture is in fact one of the face textures
                    else
                    {
                        for (int i = 0; i < A.Actor.FemaleFaceIDs.Length; ++i)
                        {
                            if (A.Actor.FemaleFaceIDs[i].Tex0 < 65535)
                            {
                                string Name2 = Media.GetTextureName(A.Actor.FemaleFaceIDs[i].Tex0).ToUpper();
                                if (!string.IsNullOrEmpty(Name2))
                                {
                                    SurfacesBackwards = true;
                                    break;
                                }
                            }
                        }
                    }
                    // Perform the texturing
                    if (SurfacesBackwards)
                    {
                        if (BodySet.Tex0 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex0, false), 0, 1);
                        if (BodySet.Tex1 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex1, false), 1, 1);
                        if (BodySet.Tex2 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex2, false), 2, 1);
                        if (BodySet.Tex3 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex3, false), 3, 1);
                        if (FaceSet.Tex0 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex0, false), 0, 0);
                        if (FaceSet.Tex1 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex1, false), 1, 0);
                        if (FaceSet.Tex2 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex2, false), 2, 0);
                        if (FaceSet.Tex3 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex3, false), 3, 0);
                    }
                    else
                    {
                        if (BodySet.Tex0 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex0, false), 0, 0);
                        if (BodySet.Tex1 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex1, false), 1, 0);
                        if (BodySet.Tex2 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex2, false), 2, 0);
                        if (BodySet.Tex3 < 65535)
                            EN.Texture(Media.GetTexture(BodySet.Tex3, false), 3, 0);
                        if (FaceSet.Tex0 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex0, false), 0, 1);
                        if (FaceSet.Tex1 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex1, false), 1, 1);
                        if (FaceSet.Tex2 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex2, false), 2, 1);
                        if (FaceSet.Tex3 < 65535)
                            EN.Texture(Media.GetTexture(FaceSet.Tex3, false), 3, 1);
                    }
                }
                    // One surface -- use body texture only
                else
                {
                    if (BodySet.Tex0 < 65535)
                        EN.Texture(Media.GetTexture(BodySet.Tex0, false), 0, 0);
                    if (BodySet.Tex1 < 65535)
                        EN.Texture(Media.GetTexture(BodySet.Tex1, false), 1, 0);
                    if (BodySet.Tex2 < 65535)
                        EN.Texture(Media.GetTexture(BodySet.Tex2, false), 2, 0);
                    if (BodySet.Tex3 < 65535)
                        EN.Texture(Media.GetTexture(BodySet.Tex3, false), 3, 0);
                }
                // Get radius
                float MaxLength = EN.MeshWidth;
                if (EN.MeshDepth > MaxLength)
                {
                    MaxLength = EN.MeshDepth;
                }
                A.Actor.Radius = MaxLength * Media.LoadedMeshScales[A.Actor.FemaleMesh] * A.Actor.Scale * 0.5f;
            }

            // Animations
            ((Entity) A.EN).Shader = Shaders.Animated;
            if (ActorAnimSet != null)
            {
                for (int i = 0; i < 150; ++i)
                {
                    if (ActorAnimSet.AnimEnd[i] > 0)
                    {
                        A.AnimSeqs[i] = ((Entity) A.EN).ExtractAnimSeq(ActorAnimSet.AnimStart[i],
                                                                       ActorAnimSet.AnimEnd[i]);
                    }
                }
            }

            // Scale
            CollisionEN.Scale(Scale, Scale, Scale);

            if (!SkipAttachments)
            {
                // Attached emitters
                CreateEntityEmitters((Entity) A.EN);

                // Hair
                SetActorHat(A, null);

                // Shadow

                // Collision

                // Debug code to display the collision radius

                // Items
                // UpdateActorItems(A);

                // Nametag
                // if (HideNametags != 1)
                // CreateActorNametag(A);
            }

            // Type handle

            return true;
        }

        public static bool LoadActorInstance3D(ActorInstance A, float Scale)
        {
            return LoadActorInstance3D(A, Scale, false);
        }

        public static bool LoadActorInstance3D(ActorInstance A)
        {
            return LoadActorInstance3D(A, 1f, false);
        }

        public static void FreeActorInstance3D(ActorInstance A)
        {
            if (A.GubbinEN != null)
                Actors3D.HideGubbins(A);

            HideGubbinSet(A, A.BeardENs);
            HideGubbinSet(A, A.HatENs);
            HideGubbinSet(A, A.ShieldENs);
            HideGubbinSet(A, A.WeaponENs);
            HideGubbinSet(A, A.ChestENs);

            if (A.ShadowEN != null)
            {
                FreeEntityEmitters((Entity) A.ShadowEN);
                ((Entity) A.ShadowEN).Free();
                A.ShadowEN = null;
            }

            if (A.NametagEN != null)
            {
                FreeEntityEmitters((Entity) A.NametagEN);
                ((Entity) A.NametagEN).Free();
                A.NametagEN = null;
            }

            if (A.EN != null)
            {
                FreeEntityEmitters((Entity) A.EN);
                ((Entity) A.EN).Free();
                A.EN = null;
            }

            ((Entity) A.CollisionEN).Free();
            A.CollisionEN = null;
        }

        public static void SafeFreeActorInstance(ActorInstance A)
        {
            FreeActorInstance3D(A);
            A.Dispose();
        }

        // Load 3D models for actor attachments
        public static void SetActorHat(ActorInstance A, List<GubbinTemplate> templates)
        {
            HideGubbinSet(A, A.HatENs);

            // No templates even set (unequip)
            if (templates == null)
            {
                if (A.Gender == 0)
                {
                    if(A.Hair < A.Actor.MaleHairs.Count)
                        templates = A.Actor.MaleHairs[A.Hair];
                }
                else
                {
                    if(A.Hair < A.Actor.FemaleHairs.Count)
                    templates = A.Actor.FemaleHairs[A.Hair];
                }
            }

            if (templates != null)
                ShowGubbinSet(A, templates, A.HatENs);
        }

        // Shows/hides a gubbin on an actor instance
        public static void ShowGubbins(ActorInstance A)
        {
            ShowGubbinSet(A, A.Actor.DefaultGubbins, A.GubbinEN);
        }

        public static void HideGubbins(ActorInstance A)
        {
            HideGubbinSet(A, A.GubbinEN);
        }

        public static void ShowGubbinSet(ActorInstance A, List<GubbinTemplate> templates, List<GubbinPreviewInstance> previews)
        {
            foreach (GubbinTemplate T in templates)
            {
                GubbinActorTemplate AT = null;

                foreach (GubbinActorTemplate tAT in T.ActorTemplates)
                {
                    if (tAT.Actor == A.Actor && tAT.Gender == A.Gender)
                    {
                        AT = tAT;
                        break;
                    }
                }

                // No definition matches this actor
                if (AT == null)
                    continue;

                // Create a preview instance, and set it up
                GubbinPreviewInstance P = new GubbinPreviewInstance(AT);

                // Get bone attachment
                Entity Bone = null;
                if (A.EN != null && A.EN is Entity)
                    Bone = ((Entity)A.EN).FindChild(AT.AssignedBoneName);

                // No bone, so avoid
                if (Bone == null)
                    continue;

                // Mesh
                if (AT.MeshID < 65535)
                {
                    P.Mesh = Media.GetMesh(AT.MeshID);

                    if (P.Mesh != null)
                    {
                        if (AT.AnimationType == GubbinAnimationType.PreAnimated)
                        {
                            int Sequence = P.Mesh.ExtractAnimSeq(AT.AnimationStartFrame, AT.AnimationEndFrame);
                            P.Mesh.Animate(1, 1.0f, Sequence);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Gubbin '" + T.Name + "' Is using Inherited animation which is not supported!");
                        }


                        P.Mesh.Parent(Bone, false);

                        P.Mesh.Position(AT.Position.X, AT.Position.Y, AT.Position.Z);
                        P.Mesh.Scale(AT.Scale.X, AT.Scale.Y, AT.Scale.Z);
                        P.Mesh.Rotate(AT.Rotation.X, AT.Rotation.Y, AT.Rotation.Z);
                    }
                }
                
                // Emitter
                if (!string.IsNullOrEmpty(AT.Emitter))
                {
                    EmitterConfig Config = RottParticles.EmitterConfig.Load(@"Data\Emitter Configs\" + AT.Emitter + ".rpc", Program.GE.Camera, 0);
                    if (Config != null)
                    {
                        uint Tex = Media.GetTexture(Config.DefaultTextureID, false);
                        if (Tex != 0)
                        {
                            Config.ChangeTexture(Tex);
                        }

                        P.EmitterEN = RottParticles.General.CreateEmitter(Config);

                        if (P.EmitterEN != null)
                        {
                            P.EmitterEN.Parent(Bone, false);
                            P.EmitterEN.Position(AT.Position.X, AT.Position.Y, AT.Position.Z);
                            P.EmitterEN.Scale(AT.Scale.X * 10.0f, AT.Scale.Y * 10.0f, AT.Scale.Z * 10.0f);
                            P.EmitterEN.Rotate(AT.Rotation.X, AT.Rotation.Y, AT.Rotation.Z);
                        }
                    }
                }

                // Light
                if (AT.UseLight)
                {
                    P.Light = RenderingServices.Light.CreatePointLight();
                    P.Light.Color(
                        (int)(AT.LightColor.X * 255.0f),
                        (int)(AT.LightColor.Y * 255.0f),
                        (int)(AT.LightColor.Z * 255.0f));
                    P.Light.Radius(AT.LightRadius);
                    P.Light.Position(AT.Position.X, AT.Position.Y, AT.Position.Z);
                }

                // Add to list
                previews.Add(P);
            }
        }

        public static void HideGubbinSet(ActorInstance A, List<GubbinPreviewInstance> previews)
        {
            foreach (GubbinPreviewInstance P in previews)
            {
                if (P.Mesh != null)
                {
                    P.Mesh.Free();
                    P.Mesh = null;
                }

                if (P.EmitterEN != null)
                {
                    RottParticles.General.FreeEmitter(P.EmitterEN, true, true);
                    P.EmitterEN = null;
                }

                if (P.Light != null)
                {
                    P.Light.Free();
                    P.Light = null;
                }
            }

            previews.Clear();
        }

        // Load/free emitters attached to a mesh (RECURSIVE)
        public static void CreateEntityEmitters(Entity EN)
        {
            int ChildCount = EN.CountChildren();
            for (int i = 1; i <= ChildCount; ++i)
            {
                Entity CE = EN.GetChild(i);
                CreateEntityEmitters(CE);
                string Name = CE.Name;
                if (Name.Length > 4 && Name.ToUpper().Substring(0, 2) == "E_")
                {
                    Name = Name.Substring(2);
                    int Pos = Name.IndexOf("_");
                    if (Pos >= 0)
                    {
                        string ConfigName = Name.Substring(0, Pos);
                        ushort TextureID = ushort.Parse(Name.Substring(Pos + 1));
                        uint Texture = Media.GetTexture(TextureID, true);
                        if (Texture != 0)
                        {
                            EmitterConfig Config = EmitterConfig.Load(@"Data\Emitter Configs\" + ConfigName + ".rpc",
                                                                      Camera, Texture);
                            if (Config != null)
                            {
                                Entity EmitterEN = General.CreateEmitter(Config);
                                if (EmitterEN != null)
                                {
                                    EmitterEN.Parent(CE, false);
                                }
                                else
                                {
                                    Config.Free(true);
                                }
                            }
                        }

                        Media.UnloadTexture(TextureID);
                    }
                }
            }
        }

        public static void FreeEntityEmitters(Entity EN)
        {
            for (int i = 1; i <= EN.CountChildren(); ++i)
            {
                Entity CE = EN.GetChild(i);
                FreeEntityEmitters(CE);
                if (CE.ExtraData is RottParticles.Emitter)
                {
                    General.FreeEmitter(CE, true, true);
                }
            }
        }
    }
}
