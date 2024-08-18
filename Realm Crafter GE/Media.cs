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
// Realm Crafter Media module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port September 2006

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using RenderingServices;
using IrrlichtSound;

namespace RealmCrafter
{
    public class cMeshLOD
    {
        public ushort MeshMediumID, MeshLowID;
        public float distHigh, distMedium, distLow;
    };

    internal static class Media
    {
        #region Members
        private static uint[] LoadedTextures = new uint[65535];
        private static Sound[] LoadedSounds = new Sound[65535];
        public static float[] LoadedMeshScales = new float[65535];

        public static cMeshLOD[] LoadedMeshLOD = new cMeshLOD[65535];

        public static float[] LoadedMeshX = new float[65535],
                              LoadedMeshY = new float[65535],
                              LoadedMeshZ = new float[65535];

        public static byte[] MeshFlags = new byte[65535];
        public static Dictionary<string, object>[] LoadedParameters = new Dictionary<string, object>[65535];

        public static ushort[] LoadedMeshShaders = new ushort[65535];
        public static ushort[] LoadedTextureFlags = new ushort[65535];
        public static FileStream LockedMeshes, LockedTextures, LockedSounds, LockedMusic;
        public static float DefaultVolume = 1f;
        #endregion

        #region Lock/unlock methods
        public static FileStream LockMeshes()
        {
            LockedMeshes = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open, FileAccess.ReadWrite,
                                          FileShare.Read);
            return LockedMeshes;
        }

        public static bool UnlockMeshes()
        {
            if (LockedMeshes != null)
            {
                LockedMeshes.Close();
                LockedMeshes = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static FileStream LockTextures()
        {
            LockedTextures = new FileStream(@"Data\Game Data\Textures.dat", System.IO.FileMode.Open,
                                            FileAccess.ReadWrite, FileShare.Read);
            return LockedTextures;
        }

        public static bool UnlockTextures()
        {
            if (LockedTextures != null)
            {
                LockedTextures.Close();
                LockedTextures = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static FileStream LockSounds()
        {
            LockedSounds = new FileStream(@"Data\Game Data\Sounds.dat", System.IO.FileMode.Open, FileAccess.ReadWrite,
                                          FileShare.Read);
            return LockedSounds;
        }

        public static bool UnlockSounds()
        {
            if (LockedSounds != null)
            {
                LockedSounds.Close();
                LockedSounds = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static FileStream LockMusic()
        {
            LockedMusic = new FileStream(@"Data\Game Data\Music.dat", System.IO.FileMode.Open, FileAccess.ReadWrite,
                                         FileShare.Read);
            return LockedMusic;
        }

        public static bool UnlockMusic()
        {
            if (LockedMusic != null)
            {
                LockedMusic.Close();
                LockedMusic = null;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Load/unload media methods
        public static Entity GetMesh(int ID)
        {
            if (ID >= 65535 || ID < 0)
            {
                // error ID
                return null;
            }
            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Find data address in file index
            F.BaseStream.Seek(1+ID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMeshes == null)
                {
                    F.Close();
                }
                return null;
            }

            // Read in mesh data
            F.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
            byte Flags = F.ReadByte();
            LoadedMeshScales[ID] = F.ReadSingle();
            LoadedMeshX[ID] = F.ReadSingle();
            LoadedMeshY[ID] = F.ReadSingle();
            LoadedMeshZ[ID] = F.ReadSingle();
            LoadedMeshShaders[ID] = F.ReadUInt16();
            MeshFlags[ID] = Flags;
            // LOD
            if (LoadedMeshLOD[ID] == null) LoadedMeshLOD[ID] = new cMeshLOD();
            LoadedMeshLOD[ID].distHigh = F.ReadSingle();
            LoadedMeshLOD[ID].MeshMediumID = F.ReadUInt16();
            LoadedMeshLOD[ID].distMedium = F.ReadSingle();
            LoadedMeshLOD[ID].MeshLowID = F.ReadUInt16();
            LoadedMeshLOD[ID].distLow = F.ReadSingle();

            string Name = Blitz.ReadString(F);

            byte IsAnim = (byte) (Flags & 1);
            bool CalcTangents = (Flags & 2) > 0;
            bool Reflect = (Flags & 4) > 0;
            bool Refract = (Flags & 8) > 0;

            // Done with database file
            if (LockedMeshes == null)
            {
                F.Close();
            }

            // Load the mesh
            Entity LoadedMesh = Entity.LoadMesh(@"Data\Meshes\" + Name, null, IsAnim == 1);
            if (LoadedMesh == null)
            {
                return null;
            }

            LoadedMesh.Scale(LoadedMeshScales[ID], LoadedMeshScales[ID], LoadedMeshScales[ID]);
            LoadedMesh.IsAnim = IsAnim;
            LoadedMesh.Flags = Flags;
            // LOD

            if (LoadedMeshLOD[ID].distHigh > 0.0f && LoadedMeshLOD[ID].distMedium > 0.0f && LoadedMeshLOD[ID].distLow > 0.0f
                && LoadedMeshLOD[ID].distMedium > LoadedMeshLOD[ID].distHigh && LoadedMeshLOD[ID].distLow > LoadedMeshLOD[ID].distMedium)
            {
                LoadedMesh.distLOD_High = LoadedMeshLOD[ID].distHigh;
                LoadedMesh.distLOD_Medium = LoadedMeshLOD[ID].distMedium;
                LoadedMesh.MeshLOD_Medium = LoadedMeshLOD[ID].MeshMediumID;
                LoadedMesh.distLOD_Low = LoadedMeshLOD[ID].distLow;
                LoadedMesh.MeshLOD_Low = LoadedMeshLOD[ID].MeshLowID;
            }

            if (LoadedParameters[ID] == null)
            {
                LoadedParameters[ID] = new Dictionary<string, object>();
            }
            LoadedMesh.Parameters = LoadedParameters[ID];
            LoadedMesh.ResendParameters();

            if (CalcTangents && IsAnim == 0)
            {
                LoadedMesh.UpdateNormals();
                LoadedMesh.UpdateHardwareBuffers();
                if (RenderWrapper.CalculateB3DTangents(LoadedMesh.Handle) == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Could not generate tangents on mesh: " + Name);
                }
            }

            if (LoadedMeshShaders[ID] < 65535)
            {
                RealmCrafter_GE.ShaderProfile Pr = RealmCrafter_GE.ShaderManager.GetProfile(LoadedMeshShaders[ID]);

                if (Pr != null)
                {
                    RenderWrapper.EntityProfile(LoadedMesh.Handle, Pr.Handle);
                }
                else
                {
                    LoadedMesh.Shader = RenderingServices.Shaders.LitObject1;

                    if (IsAnim == 0 && RealmCrafter_GE.ShaderManager.DefaultStatic != null)
                    {
                        RenderWrapper.EntityProfile(LoadedMesh.Handle,
                                                    RealmCrafter_GE.ShaderManager.DefaultStatic.Handle);
                    }

                    if (IsAnim == 1 && RealmCrafter_GE.ShaderManager.DefaultAnimated != null)
                    {
                        RenderWrapper.EntityProfile(LoadedMesh.Handle,
                                                    RealmCrafter_GE.ShaderManager.DefaultAnimated.Handle);
                    }
                }
            }
            else
            {
                LoadedMesh.Shader = RenderingServices.Shaders.LitObject1;

                if (IsAnim == 0 && RealmCrafter_GE.ShaderManager.DefaultStatic != null)
                {
                    RenderWrapper.EntityProfile(LoadedMesh.Handle, RealmCrafter_GE.ShaderManager.DefaultStatic.Handle);
                }

                if (IsAnim == 1 && RealmCrafter_GE.ShaderManager.DefaultAnimated != null)
                {
                    RenderWrapper.EntityProfile(LoadedMesh.Handle, RealmCrafter_GE.ShaderManager.DefaultAnimated.Handle);
                }
            }

            LoadedMesh.ID = (ushort) ID;
            return LoadedMesh;
        }

        public static uint GetTexture(int ID, bool Copy)
        {
            if (ID == 65535)
            {
                return Render.LoadTexture("BBDX2GETDEFAULTTEXTURE.int");
            }

            // Texture is not already cached
            if (LoadedTextures[ID] == 0)
            {
                // Open index file
                BinaryReader F;
                if (LockedTextures == null)
                {
                    FileStream FStream = new FileStream(@"Data\Game Data\Textures.dat", System.IO.FileMode.Open);
                    F = new BinaryReader(FStream);
                }
                else
                {
                    F = new BinaryReader(LockedTextures);
                }

                // Find data address in file index
                F.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
                int DataAddress = F.ReadInt32();
                if (DataAddress == 0)
                {
                    if (LockedTextures == null)
                    {
                        F.Close();
                    }
                    return 0;
                }

                // Read in texture data
                F.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
                LoadedTextureFlags[ID] = F.ReadUInt16();
                string Name = Blitz.ReadString(F);

                // Done with database file
                if (LockedTextures == null)
                {
                    F.Close();
                }

                // Load the texture
                LoadedTextures[ID] = Render.LoadTexture(@"Data\Textures\" + Name, LoadedTextureFlags[ID]);
            }

            // Copy texture
            if (Copy)
            {
                Render.GrabTexture(LoadedTextures[ID]);
            }
            return LoadedTextures[ID];
        }

        public static Sound GetSound(int ID)
        {
            // Already loaded
            if (LoadedSounds[ID] != null)
            {
                return LoadedSounds[ID];
            }

            // Open index file
            BinaryReader F;
            if (LockedSounds == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Sounds.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedSounds);
            }

            // Find data address in file index
            F.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedSounds == null)
                {
                    F.Close();
                }
                return null;
            }

            // Read in sound data
            F.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
            byte Is3D = F.ReadByte();
            string Name = Blitz.ReadString(F);

            // Done with database file
            if (LockedSounds == null)
            {
                F.Close();
            }

            // Load the sound
            try
            {
                LoadedSounds[ID] = Sound.Load(@"Data\Sounds\" + Name);
                return LoadedSounds[ID];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void UnloadTexture(int ID)
        {
            if (ID < 0)
                return;

            if (ID == 65535)
                return;

            if (LoadedTextures[ID] != 0)
            {
                Render.FreeTexture(LoadedTextures[ID]);
                LoadedTextures[ID] = 0;
            }
        }

        public static void UnloadSound(int ID)
        {
            if (ID == 65535)
                return;

            // Sound is loaded
            if (LoadedSounds[ID] != null)
            {
                //LoadedSounds[ID].Dispose();
                //LoadedSounds[ID] = null;
            }
        }
        #endregion

        #region Get name methods
        public static string GetMeshName(int ID)
        {
            return GetMeshName(ID, true);
        }

        public static string GetMeshName(int ID, bool appendAnim)
        {
            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Find data address in file index
            F.BaseStream.Seek(1+ID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMeshes == null)
                {
                    F.Close();
                }
                return "";
            }

            // Read in mesh data
            F.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
            byte IsAnim = (byte) (F.ReadByte() & 1);
            F.BaseStream.Position += 34; // 18 + 16(LOD)
            string Name = Blitz.ReadString(F);

            // Done
            if (LockedMeshes == null)
            {
                F.Close();
            }

            if(appendAnim)
                return Name + IsAnim.ToString();
            return Name;
        }

        public static string GetTextureName(int ID)
        {
            if (ID == 65535)
                return "";

            // Open index file
            BinaryReader F;
            if (LockedTextures == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Textures.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedTextures);
            }

            // Find data address in file index
            F.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedTextures == null)
                {
                    F.Close();
                }
                return "";
            }

            // Read in mesh data
            F.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
            ushort Flags = F.ReadUInt16();
            string Name = Blitz.ReadString(F);

            // Done
            if (LockedTextures == null)
            {
                F.Close();
            }
            return Name + Blitz.StrFromInt(Flags, 1);
        }

        public static string GetSoundName(int ID)
        {
            // Open index file
            BinaryReader F;
            if (LockedSounds == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Sounds.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedSounds);
            }

            // Find data address in file index
            F.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedSounds == null)
                {
                    F.Close();
                }
                return "";
            }

            // Read in mesh data
            F.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
            byte Is3D = F.ReadByte();
            string Name = Blitz.ReadString(F);

            // Done
            if (LockedSounds == null)
            {
                F.Close();
            }
            return Name + Is3D.ToString();
        }

        public static string GetMusicName(int ID)
        {
            // Open index file
            BinaryReader F;
            if (LockedMusic == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Music.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMusic);
            }

            // Find data address in file index
            F.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMusic == null)
                {
                    F.Close();
                }
                return "";
            }

            // Read in mesh data
            F.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
            string Name = Blitz.ReadString(F);

            // Done
            if (LockedMusic == null)
            {
                F.Close();
            }
            return Name;
        }
        #endregion

        // Create a new blank media database
        public static void CreateDatabase(string Filename)
        {
            FileStream F = new FileStream(Filename, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(F);
            BW.Write((byte)255); // Flag for New Format
            for (int i = 0; i < 65535; ++i) BW.Write((int)0);
            BW.Close();
        }

        #region Add/remove from database methods
        public static void ConvertMeshDatabase()
        {
            FileStream FO = new FileStream(@"Data\Game Data\Meshes.dat", FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            BinaryReader BR = new BinaryReader(FO);
            if (BR.ReadByte() == 255) { BR.Close(); FO.Close(); return; }   // Check Database New Format

            FileStream FN = new FileStream(@"Data\Game Data\Meshes_.dat", FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FN);

            BW.Write((byte)255); // Flag for New Format
            for (int i = 0; i < 65535; ++i) BW.Write( (int)0 );
            BW.Flush();
            
            long Length;
            byte Flags;
            float Scale, X, Y, Z;
            ushort ShaderID;
            string Name;
            for (int ID = 0; ID < 65535; ++ID)
            {
                // Find data address in file index
                BR.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
                int DataAddress = BR.ReadInt32();
                if (DataAddress == 0) continue;

                // Move to DataAddress
                BR.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
                // Read in mesh data
                    Flags = BR.ReadByte();
                    Scale = BR.ReadSingle();
                    X = BR.ReadSingle();
                    Y = BR.ReadSingle();
                    Z = BR.ReadSingle();
                    ShaderID = BR.ReadUInt16();
                    Name = Blitz.ReadString(BR);
                
                // Write into new MeshDatabase
                    Length = BW.BaseStream.Length;
                    BW.BaseStream.Seek(1+ID*4, SeekOrigin.Begin);
                    BW.Write((int)Length);
                    BW.BaseStream.Seek(Length, SeekOrigin.Begin);

                    BW.Write(Flags);
                    BW.Write(Scale);                //  Scale
                    BW.Write(X);                    //  X
                    BW.Write(Y);                    //  Y
                    BW.Write(Z);                    //  Z
                    BW.Write(ShaderID);             //  ShaderID  + 19

                    BW.Write(0f);                   //  DistToMedium_LOD
                    BW.Write((ushort)65535);        //  IdMeshLOD_Medium
                    BW.Write(0f);                   //  DistToLow_LOD
                    BW.Write((ushort)65535);        //  IdMeshLOD_Low
                    BW.Write(0f);                   //  DistToHide_LOD  + 16 = 35

                    Blitz.WriteString(Name, BW);   // MeshName
                    BW.Flush();
            }

            BR.Close(); BW.Close();
            FO.Close(); FN.Close();

            File.Delete(@"Data\Game Data\Meshes.dat");
            File.Move(@"Data\Game Data\Meshes_.dat", @"Data\Game Data\Meshes.dat");
        }

        public static int AddMeshToDatabase(string Filename, bool IsAnim)
        {
            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Check all existing entries to make sure this file isn't already there
            bool MIsAnim;
            string MFilename;
            long Length = F.BaseStream.Length;
            F.BaseStream.Seek(1+65535 * 4, SeekOrigin.Begin);
            while (F.BaseStream.Position < Length)
            {
                MIsAnim = F.ReadBoolean();
                F.BaseStream.Position += 34;
                MFilename = Blitz.ReadString(F).ToUpper();
                if (MFilename.Contains( Filename.ToUpper()) && MIsAnim == IsAnim)
                {
                    if (LockedMeshes == null)
                    {
                        F.Close();
                    }
                    return -1;
                }
            }

            // Find the first free ID
            int DataAddress;
            F.BaseStream.Seek(1, SeekOrigin.Begin);
            for (int ID = 0; ID < 65535; ++ID)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress == 0)
                {
                    // Write new data to database
                    BinaryWriter FW = new BinaryWriter(F.BaseStream);
                    FW.BaseStream.Seek(1+ID * 4, SeekOrigin.Begin);
                    FW.Write((int) Length);
                    Length = FW.BaseStream.Length;
                    FW.BaseStream.Seek(Length, SeekOrigin.Begin);
                    FW.Write(IsAnim);
                    FW.Write(1f);               //  Scale
                    FW.Write(0f);               //  X
                    FW.Write(0f);               //  Y
                    FW.Write(0f);               //  Z
                    FW.Write((ushort)65535);    //  ShaderID  + 19
                    
                    FW.Write(0f);               //  DistToMedium_LOD
                    FW.Write((ushort)65535);    //  IdMeshLOD_Medium
                    FW.Write(0f);               //  DistToLow_LOD
                    FW.Write((ushort)65535);    //  IdMeshLOD_Low
                    FW.Write(0f);               //  DistToHide_LOD  + 16 = 35

                    Blitz.WriteString(Filename, FW);    // MeshName

                    // Return new ID
                    if (LockedMeshes == null)
                    {
                        F.Close();
                    }
                    return ID;
                }
            }

            // No free ID found
            if (LockedMeshes == null)
            {
                F.Close();
            }
            return -1;
        }

        public static int AddTextureToDatabase(string Filename, ushort Flags)
        {
            LockTextures();

            // Open index file
//             BinaryReader F;
//             if (LockedTextures == null)
//             {
//                 FileStream FStream = new FileStream(@"Data\Game Data\Textures.dat", System.IO.FileMode.Open);
//                 F = new BinaryReader(FStream);
//             }
//             else
//             {
//                 F = new BinaryReader(LockedTextures);
//             }
// 
//             // Check all existing entries to make sure this file isn't already there
//             ushort TFlags;
//             string TFilename;
//             long Length = F.BaseStream.Length;
//             F.BaseStream.Seek(65535 * 4, SeekOrigin.Begin);
//             while (F.BaseStream.Position < Length)
//             {
//                 TFlags = F.ReadUInt16();
//                 TFilename = Blitz.ReadString(F).ToUpper();
//                 if (TFilename == Filename.ToUpper() && TFlags == Flags)
//                 {
//                     if (LockedTextures == null)
//                     {
//                         F.Close();
//                     }
//                     System.Windows.Forms.MessageBox.Show(TFilename + " = " + Filename.ToUpper());
//                     return -2;
//                 }
//             }

            for (int i = 0; i < 65535; ++i)
            {
                string Name = GetTextureName(i);
                if (Name.Length > 0)
                {
                    Name = Name.Substring(0, Name.Length - 1);
                    if (Name.Equals(Filename, StringComparison.CurrentCultureIgnoreCase))
                    {
                        UnlockTextures();
                        return -2;
                    }
                }
            }

            BinaryReader F = new BinaryReader(LockedTextures);
            long Length = F.BaseStream.Length;

            // Find the first free ID
            int DataAddress;
            F.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int ID = 0; ID < 65535; ++ID)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress == 0)
                {
                    // Write new data to database
                    BinaryWriter FW = new BinaryWriter(F.BaseStream);
                    FW.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
                    FW.Write((int)Length);
                    Length = FW.BaseStream.Length;
                    FW.BaseStream.Seek(Length, SeekOrigin.Begin);
                    FW.Write(Flags);
                    Blitz.WriteString(Filename, FW);

                    // Return new ID
                    if (LockedTextures == null)
                    {
                        F.Close();
                    }
                    else
                    {
                        UnlockTextures();
                    }
                    return ID;
                }
            }

            // No free ID found
//             if (LockedTextures == null)
//             {
//                 F.Close();
//             }
            UnlockTextures();
            return -1;
        }

        public static int LoadTextureToDatabase(string Filename, ushort Flags)
        {
            // Open index file
            BinaryReader F;
            if (LockedTextures == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Textures.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedTextures);
            }

            // Check all existing entries to make sure this file isn't already there
            ushort TFlags;
            string TFilename;
            long Length = F.BaseStream.Length;
            F.BaseStream.Seek(65535 * 4, SeekOrigin.Begin);
            int mID = 0;
            while (F.BaseStream.Position < Length)
            {
                TFlags = F.ReadUInt16();
                TFilename = Blitz.ReadString(F).ToUpper();
                if (TFilename == Filename.ToUpper() && TFlags == Flags)
                {
                    if (LockedTextures == null)
                    {
                        F.Close();
                    }
                    return mID;
                }
                mID++;
            }

            // Find the first free ID
            int DataAddress;
            F.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int ID = 0; ID < 65535; ++ID)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress == 0)
                {
                    // Write new data to database
                    BinaryWriter FW = new BinaryWriter(F.BaseStream);
                    FW.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
                    FW.Write((int)Length);
                    Length = FW.BaseStream.Length;
                    FW.BaseStream.Seek(Length, SeekOrigin.Begin);
                    FW.Write(Flags);
                    Blitz.WriteString(Filename, FW);

                    // Return new ID
                    if (LockedTextures == null)
                    {
                        F.Close();
                    }
                    return ID;
                }
            }

            // No free ID found
            if (LockedTextures == null)
            {
                F.Close();
            }
            return -1;
        }


        public static int AddSoundToDatabase(string Filename, bool Is3D)
        {
            // Open index file
            BinaryReader F;
            if (LockedSounds == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Sounds.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedSounds);
            }

            // Check all existing entries to make sure this file isn't already there
            bool SIs3D;
            string SFilename;
            long Length = F.BaseStream.Length;
            F.BaseStream.Seek(65535 * 4, SeekOrigin.Begin);
            while (F.BaseStream.Position < Length)
            {
                SIs3D = F.ReadBoolean();
                SFilename = Blitz.ReadString(F).ToUpper();
                if (SFilename == Filename.ToUpper() && SIs3D == Is3D)
                {
                    if (LockedSounds == null)
                    {
                        F.Close();
                    }
                    return -1;
                }
            }

            // Find the first free ID
            int DataAddress;
            F.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int ID = 0; ID < 65535; ++ID)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress == 0)
                {
                    // Write new data to database
                    BinaryWriter FW = new BinaryWriter(F.BaseStream);
                    FW.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
                    FW.Write((int)Length);
                    Length = FW.BaseStream.Length;
                    FW.BaseStream.Seek(Length, SeekOrigin.Begin);
                    FW.Write(Is3D);
                    Blitz.WriteString(Filename, FW);

                    // Return new ID
                    if (LockedSounds == null)
                    {
                        F.Close();
                    }
                    return ID;
                }
            }

            // No free ID found
            if (LockedSounds == null)
            {
                F.Close();
            }
            return -1;
        }

        public static int AddMusicToDatabase(string Filename)
        {
            // Open index file
            BinaryReader F;
            if (LockedMusic == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Music.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMusic);
            }

            // Check all existing entries to make sure this file isn't already there
            string MFilename;
            long Length = F.BaseStream.Length;
            F.BaseStream.Seek(65535 * 4, SeekOrigin.Begin);
            while (F.BaseStream.Position < Length)
            {
                try
                {
                    MFilename = Blitz.ReadString(F).ToUpper();
                }
                catch (Exception e)
                {
                    MFilename = "";
                }

                if (MFilename == Filename.ToUpper())
                {
                    if (LockedMusic == null)
                    {
                        F.Close();
                    }
                    return -1;
                }
            }

            // Find the first free ID
            int DataAddress;
            F.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int ID = 0; ID < 65535; ++ID)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress == 0)
                {
                    // Write new data to database
                    BinaryWriter FW = new BinaryWriter(F.BaseStream);
                    FW.BaseStream.Seek(ID * 4, SeekOrigin.Begin);
                    FW.Write(Length);
                    Length = FW.BaseStream.Length;
                    FW.BaseStream.Seek(Length, SeekOrigin.Begin);
                    Blitz.WriteString(Filename, FW);

                    // Return new ID
                    if (LockedMusic == null)
                    {
                        F.Close();
                    }
                    return ID;
                }
            }

            // No free ID found
            if (LockedMusic == null)
            {
                F.Close();
            }
            return -1;
        }

        public static void RemoveMeshFromDatabase(int ID)
        {
            // Open index file
            bool Locked = false;
            FileStream FStream;
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
                Locked = true;
            }

            // Read in the data address for every ID except this one
            int DataAddress;
            LoadedMediaData FirstMediaData = null, L = null;
            F.BaseStream.Seek(1, SeekOrigin.Begin);
            for (int i = 0; i < 65535; ++i)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress > 0 && i != ID)
                {
                    L = new LoadedMediaData();
                    L.ID = (ushort) i;
                    L.DataAddress = DataAddress;
                    L.Next = FirstMediaData;
                    FirstMediaData = L;
                }
            }

            // Read in the actual data for each existing ID
            L = FirstMediaData;
            while (L != null)
            {
                F.BaseStream.Seek(L.DataAddress, SeekOrigin.Begin);
                L.ExtraData = F.ReadByte();
                L.Scale = F.ReadSingle();
                L.X = F.ReadSingle();
                L.Y = F.ReadSingle();
                L.Z = F.ReadSingle();
                L.Shader = F.ReadUInt16();
                if (L.MeshLOD == null) L.MeshLOD = new cMeshLOD();
                L.MeshLOD.distHigh = F.ReadSingle();
                L.MeshLOD.MeshMediumID = F.ReadUInt16();
                L.MeshLOD.distMedium = F.ReadSingle();
                L.MeshLOD.MeshLowID = F.ReadUInt16();
                L.MeshLOD.distLow = F.ReadSingle();
                L.Name = Blitz.ReadString(F);

                L = L.Next;
            }

            // Clear and reopen the database for writing
            F.Close();
            CreateDatabase(@"Data\Game Data\Meshes.dat");
            FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open, FileAccess.ReadWrite,
                                     FileShare.Read);

            // Write everything back out to the database
            BinaryWriter FW = new BinaryWriter(FStream);
            long FileEnd = 1+65535 * 4;
            L = FirstMediaData;
            while (L != null)
            {
                FW.BaseStream.Seek(1+L.ID * 4, SeekOrigin.Begin);
                FW.Write((int) FileEnd);
                FW.BaseStream.Seek(FileEnd, SeekOrigin.Begin);
                FW.Write((byte) L.ExtraData);
                FW.Write(L.Scale);
                FW.Write(L.X);
                FW.Write(L.Y);
                FW.Write(L.Z);
                FW.Write(L.Shader);

                FW.Write(L.MeshLOD.distHigh);
                FW.Write(L.MeshLOD.MeshMediumID);
                FW.Write(L.MeshLOD.distMedium);
                FW.Write(L.MeshLOD.MeshLowID);
                FW.Write(L.MeshLOD.distLow);

                Blitz.WriteString(L.Name, FW);
                FileEnd = FW.BaseStream.Position;

                L = L.Next;
            }

            // Restore locked state
            if (!Locked)
            {
                FW.Close();
            }
            else
            {
                LockedMeshes = FStream;
            }
        }

        public static void RemoveTextureFromDatabase(int ID)
        {
            UnloadTexture(ID);

            // Open index file
            bool Locked = false;
            FileStream FStream;
            BinaryReader F;
            if (LockedTextures == null)
            {
                FStream = new FileStream(@"Data\Game Data\Textures.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                Locked = true;
                F = new BinaryReader(LockedTextures);
            }

            // Read in the data address for every ID except this one
            int DataAddress;
            LoadedMediaData FirstMediaData = null, L = null;
            F.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < 65535; ++i)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress > 0 && i != ID)
                {
                    L = new LoadedMediaData();
                    L.ID = (ushort) i;
                    L.DataAddress = DataAddress;
                    L.Next = FirstMediaData;
                    FirstMediaData = L;
                }
            }

            // Read in the actual data for each existing ID
            L = FirstMediaData;
            while (L != null)
            {
                F.BaseStream.Seek(L.DataAddress, SeekOrigin.Begin);
                L.ExtraData = F.ReadUInt16();
                L.Name = Blitz.ReadString(F);

                L = L.Next;
            }

            // Clear and reopen the database for writing
            F.Close();
            CreateDatabase(@"Data\Game Data\Textures.dat");
            FStream = new FileStream(@"Data\Game Data\Textures.dat", System.IO.FileMode.Open, FileAccess.ReadWrite,
                                     FileShare.Read);
            BinaryWriter FW = new BinaryWriter(FStream);

            // Write everything back out to the database
            long FileEnd = 65535 * 4;
            L = FirstMediaData;
            while (L != null)
            {
                FW.BaseStream.Seek(L.ID * 4, SeekOrigin.Begin);
                FW.Write((int) FileEnd);
                FW.BaseStream.Seek(FileEnd, SeekOrigin.Begin);
                FW.Write((ushort) L.ExtraData);
                Blitz.WriteString(L.Name, FW);
                FileEnd = FW.BaseStream.Position;

                L = L.Next;
            }

            // Restore locked state if necessary
            if (!Locked)
            {
                FW.Close();
            }
            else
            {
                LockedTextures = FStream;
            }
        }

        public static void RemoveSoundFromDatabase(int ID)
        {
            UnloadSound(ID);

            // Open index file
            FileStream FStream;
            bool Locked = false;
            BinaryReader F;
            if (LockedSounds == null)
            {
                FStream = new FileStream(@"Data\Game Data\Sounds.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                Locked = true;
                F = new BinaryReader(LockedSounds);
            }

            // Read in the data address for every ID except this one
            int DataAddress;
            LoadedMediaData FirstMediaData = null, L = null;
            F.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < 65535; ++i)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress > 0 && i != ID)
                {
                    L = new LoadedMediaData();
                    L.ID = (ushort) i;
                    L.DataAddress = DataAddress;
                    L.Next = FirstMediaData;
                    FirstMediaData = L;
                }
            }

            // Read in the actual data for each existing ID
            L = FirstMediaData;
            while (L != null)
            {
                F.BaseStream.Seek(L.DataAddress, SeekOrigin.Begin);
                L.ExtraData = F.ReadByte();
                L.Name = Blitz.ReadString(F);

                L = L.Next;
            }

            // Clear and reopen the database for writing
            F.Close();
            CreateDatabase(@"Data\Game Data\Sounds.dat");
            FStream = new FileStream(@"Data\Game Data\Sounds.dat", System.IO.FileMode.Open, FileAccess.ReadWrite,
                                     FileShare.Read);
            BinaryWriter FW = new BinaryWriter(FStream);

            // Write everything back out to the database
            long FileEnd = 65535 * 4;
            L = FirstMediaData;
            while (L != null)
            {
                FW.BaseStream.Seek(L.ID * 4, SeekOrigin.Begin);
                FW.Write((int) FileEnd);
                FW.BaseStream.Seek(FileEnd, SeekOrigin.Begin);
                FW.Write((byte) L.ExtraData);
                Blitz.WriteString(L.Name, FW);
                FileEnd = FW.BaseStream.Position;

                L = L.Next;
            }

            // Restore locked state if necessary
            if (!Locked)
            {
                FW.Close();
            }
            else
            {
                LockedSounds = FStream;
            }
        }

        public static void RemoveMusicFromDatabase(int ID)
        {
            // Open index file
            bool Locked = false;
            FileStream FStream;
            BinaryReader F;
            if (LockedMusic == null)
            {
                FStream = new FileStream(@"Data\Game Data\Music.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                Locked = true;
                F = new BinaryReader(LockedMusic);
            }

            // Read in the data address for every ID except this one
            int DataAddress;
            LoadedMediaData FirstMediaData = null, L = null;
            F.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < 65535; ++i)
            {
                DataAddress = F.ReadInt32();
                if (DataAddress > 0 && i != ID)
                {
                    L = new LoadedMediaData();
                    L.ID = (ushort) i;
                    L.DataAddress = DataAddress;
                    L.Next = FirstMediaData;
                    FirstMediaData = L;
                }
            }

            // Read in the actual data for each existing ID
            L = FirstMediaData;
            while (L != null)
            {
                F.BaseStream.Seek(L.DataAddress, SeekOrigin.Begin);
                L.Name = Blitz.ReadString(F);

                L = L.Next;
            }

            // Clear and reopen the database for writing
            F.Close();
            CreateDatabase(@"Data\Game Data\Music.dat");
            FStream = new FileStream(@"Data\Game Data\Music.dat", System.IO.FileMode.Open, FileAccess.ReadWrite,
                                     FileShare.Read);
            BinaryWriter FW = new BinaryWriter(FStream);

            // Write everything back out to the database
            long FileEnd = 65535 * 4;
            L = FirstMediaData;
            while (L != null)
            {
                FW.BaseStream.Seek(L.ID * 4, SeekOrigin.Begin);
                FW.Write((int) FileEnd);
                FW.BaseStream.Seek(FileEnd, SeekOrigin.Begin);
                Blitz.WriteString(L.Name, FW);
                FileEnd = FW.BaseStream.Position;

                L = L.Next;
            }

            // Restore locked state if necessary
            if (!Locked)
            {
                FW.Close();
            }
            else
            {
                LockedMusic = FStream;
            }
        }
        #endregion

        // Change mesh settings
        public static void SetMeshScale(ushort MediaID, float Scale)
        {
            LoadedMeshScales[MediaID] = Scale;

            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Find data address in file index
            F.BaseStream.Seek(1+MediaID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMeshes == null)
                {
                    F.Close();
                }
                return;
            }

            // Write new scale float
            BinaryWriter FW = new BinaryWriter(F.BaseStream);
            FW.BaseStream.Seek(DataAddress + 1, SeekOrigin.Begin);
            FW.Write(Scale);

            // Done with database file
            if (LockedMeshes == null)
            {
                F.Close();
            }
        }

        public static void SetMeshLOD(ushort MediaID, byte iLod, ushort LodID)  // 0:Medium 1:Low
        {
            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Find data address in file index
            F.BaseStream.Seek(1+MediaID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMeshes == null) F.Close();
                return;
            }

            // Write new scale float
            BinaryWriter FW = new BinaryWriter(F.BaseStream);

            int MoveToLod = 19 + 4 + ((iLod == 0) ? 0 : 6); // 19 + 4(distToMedium)
            FW.BaseStream.Seek(DataAddress + MoveToLod, SeekOrigin.Begin); 
            if ( iLod == 0 ) LoadedMeshLOD[MediaID].MeshMediumID = LodID;
            else             LoadedMeshLOD[MediaID].MeshLowID = LodID;
            FW.Write(LodID);

            // Done with database file
            if (LockedMeshes == null) F.Close();
        }

        public static void SetMeshLOD_Distance(ushort MediaID, byte iLod, float Distance)  // 0:High, 1:Medium 2:Low
        {
            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Find data address in file index
            F.BaseStream.Seek(1+MediaID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMeshes == null) F.Close();
                return;
            }

            // Write new scale float
            BinaryWriter FW = new BinaryWriter(F.BaseStream);
                                   // High           // Medium         // Low
            int MoveToLod = 19 + ((iLod == 0) ? 0 : ((iLod == 1) ? 6 : ((iLod == 2) ? 12 : 0))); 
            FW.BaseStream.Seek(DataAddress + MoveToLod, SeekOrigin.Begin);
            if (iLod == 0) LoadedMeshLOD[MediaID].distHigh = Distance;
            else if (iLod == 1) LoadedMeshLOD[MediaID].distMedium = Distance;
            else if (iLod == 2) LoadedMeshLOD[MediaID].distLow = Distance;
            FW.Write(Distance);

            // Done with database file
            if (LockedMeshes == null) F.Close();
        }

        public static void SetMeshOffset(ushort MediaID, float X, float Y, float Z)
        {
            LoadedMeshX[MediaID] = X;
            LoadedMeshY[MediaID] = Y;
            LoadedMeshZ[MediaID] = Z;

            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Find data address in file index
            F.BaseStream.Seek(1+MediaID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMeshes == null)
                {
                    F.Close();
                }
                return;
            }

            // Write new offset floats
            BinaryWriter FW = new BinaryWriter(F.BaseStream);
            FW.BaseStream.Seek(DataAddress + 5, SeekOrigin.Begin);
            FW.Write(X);
            FW.Write(Y);
            FW.Write(Z);

            // Done with database file
            if (LockedMeshes == null)
            {
                F.Close();
            }
        }

        public static void SetMeshShader(ushort MediaID, ushort Shader)
        {
            LoadedMeshShaders[MediaID] = Shader;

            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Find data address in file index
            F.BaseStream.Seek(1+MediaID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMeshes == null)
                {
                    F.Close();
                }
                return;
            }

            // Write new shader ushort
            BinaryWriter FW = new BinaryWriter(F.BaseStream);
            FW.BaseStream.Seek(DataAddress + 17, SeekOrigin.Begin);
            FW.Write(Shader);
            FW.Close();

            // Done with database file
            if (LockedMeshes == null)
            {
                F.Close();
            }
        }

        public static void SetMeshFlags(ushort MediaID, byte Flags)
        {
            MeshFlags[MediaID] = Flags;

            // Open index file
            BinaryReader F;
            if (LockedMeshes == null)
            {
                FileStream FStream = new FileStream(@"Data\Game Data\Meshes.dat", System.IO.FileMode.Open);
                F = new BinaryReader(FStream);
            }
            else
            {
                F = new BinaryReader(LockedMeshes);
            }

            // Find data address in file index
            F.BaseStream.Seek(1+MediaID * 4, SeekOrigin.Begin);
            int DataAddress = F.ReadInt32();
            if (DataAddress == 0)
            {
                if (LockedMeshes == null)
                {
                    F.Close();
                }
                return;
            }

            // Write new shader ushort
            BinaryWriter FW = new BinaryWriter(F.BaseStream);
            FW.BaseStream.Seek(DataAddress, SeekOrigin.Begin);
            FW.Write(Flags);
            FW.Close();

            // Done with database file
            if (LockedMeshes == null)
            {
                F.Close();
            }
        }

        // Scales a mesh entity to be a certain size without altering the mesh (works on animated meshes)
        public static void SizeEntity(Entity EN, float Width, float Height, float Depth, bool Uniform)
        {
            // Find mesh edges
            MeshMinMaxVertices MMV = new MeshMinMaxVertices(EN);
            float MWidth = MMV.MaxX - MMV.MinX;
            float MHeight = MMV.MaxY - MMV.MinY;
            float MDepth = MMV.MaxZ - MMV.MinZ;
            MMV = null;

            // Scale
            if (!Uniform)
            {
                EN.Scale(Width / MWidth, Height / MHeight, Depth / MDepth);
            }
            else
            {
                float XScale = Width / MWidth;
                float YScale = Height / MHeight;
                float ZScale = Depth / MDepth;

                if (YScale < XScale)
                {
                    XScale = YScale;
                }
                if (ZScale < XScale)
                {
                    XScale = ZScale;
                }

                EN.Scale(XScale, XScale, XScale);
            }
        }

        public static void LoadEntityParameters()
        {
            XmlTextReader X = null;

            try
            {
                X = new XmlTextReader("Data\\Game Data\\ShaderParameters.xml");

                int EntityID = -1;

                while (X.Read())
                {
                    // <entity> node
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "entity")
                    {
                        EntityID = Convert.ToInt32(X.GetAttribute("id"));
                    }

                    // </entity> node
                    if (X.NodeType == XmlNodeType.EndElement && X.Name.ToLower() == "entity")
                    {
                        EntityID = -1;
                    }

                    // <parameter> node
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "parameter" && EntityID > -1 &&
                        EntityID < LoadedParameters.Length - 1)
                    {
                        string Name = X.GetAttribute("name");
                        string Type = X.GetAttribute("type").ToLower();
                        string Value = X.GetAttribute("value");
                        object O = null;

                        if (Type.Equals("vector1"))
                        {
                            O = new Vector1OptionsConverter().ConvertFrom(null, null, Value);
                        }
                        if (Type.Equals("vector2"))
                        {
                            O = new Vector2OptionsConverter().ConvertFrom(null, null, Value);
                        }
                        if (Type.Equals("vector3"))
                        {
                            O = new Vector3OptionsConverter().ConvertFrom(null, null, Value);
                        }
                        if (Type.Equals("vector4"))
                        {
                            O = new Vector4OptionsConverter().ConvertFrom(null, null, Value);
                        }

                        if (LoadedParameters[EntityID] == null)
                        {
                            LoadedParameters[EntityID] = new Dictionary<string, object>();
                        }

                        if (O != null)
                        {
                            LoadedParameters[EntityID].Add(Name, O);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Eek an error
                System.Windows.Forms.MessageBox.Show("Error: " + e.Message, "LoadEntityParameters",
                                                     System.Windows.Forms.MessageBoxButtons.OK,
                                                     System.Windows.Forms.MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            finally
            {
                // Cleanup
                X.Close();
            }
        }

        public static void SaveEntityParameters()
        {
            XmlTextWriter X = null;

            try
            {
                X = new XmlTextWriter("Data\\Game Data\\ShaderParameters.xml", System.Text.Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                X.WriteStartDocument();
                X.WriteStartElement("parameters");

                for (int i = 0; i < 65535; ++i)
                {
                    if (LoadedParameters[i] != null && LoadedParameters[i].Count > 0)
                    {
                        X.WriteStartElement("entity");
                        X.WriteStartAttribute("id");
                        X.WriteString(i.ToString());
                        X.WriteEndAttribute();

                        foreach (KeyValuePair<string, object> Kp in LoadedParameters[i])
                        {
                            X.WriteStartElement("parameter");

                            X.WriteStartAttribute("name");
                            X.WriteString(Kp.Key);
                            X.WriteEndAttribute();

                            X.WriteStartAttribute("type");
                            if (Kp.Value.GetType() == typeof(Vector1))
                            {
                                X.WriteString("vector1");
                            }
                            if (Kp.Value.GetType() == typeof(Vector2))
                            {
                                X.WriteString("vector2");
                            }
                            if (Kp.Value.GetType() == typeof(Vector3))
                            {
                                X.WriteString("vector3");
                            }
                            if (Kp.Value.GetType() == typeof(Vector4))
                            {
                                X.WriteString("vector4");
                            }
                            X.WriteEndAttribute();

                            X.WriteStartAttribute("value");
                            if (Kp.Value.GetType() == typeof(Vector1))
                            {
                                X.WriteString(Kp.Value.ToString());
                            }
                            if (Kp.Value.GetType() == typeof(Vector2))
                            {
                                X.WriteString(Kp.Value.ToString());
                            }
                            if (Kp.Value.GetType() == typeof(Vector3))
                            {
                                X.WriteString(Kp.Value.ToString());
                            }
                            if (Kp.Value.GetType() == typeof(Vector4))
                            {
                                X.WriteString(Kp.Value.ToString());
                            }
                            X.WriteEndAttribute();

                            X.WriteEndElement();
                        }

                        X.WriteEndElement();
                    }
                }

                // Close and clean
                X.WriteEndElement();
                X.WriteEndDocument();
                X.Flush();
            }
            catch (Exception e)
            {
                // Eek an error
                System.Windows.Forms.MessageBox.Show("Error: " + e.Message, "SaveEntityParameters",
                                                     System.Windows.Forms.MessageBoxButtons.OK,
                                                     System.Windows.Forms.MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            finally
            {
                // Cleanup
                X.Close();
            }
        }
    }

    public class LoadedMediaData
    {
        public ushort ID;
        public string Name;
        public int DataAddress, ExtraData;
        public float X, Y, Z, Scale;
        public ushort Shader;
        public cMeshLOD MeshLOD;
        public LoadedMediaData Next;
    }

    // Retrieves the min/max vertex positions of a mesh or mesh heirarchy
    public class MeshMinMaxVertices
    {
        // Members
        public float MinX, MinY, MinZ;
        public float MaxX, MaxY, MaxZ;

        // Constructor for a normal mesh
        public MeshMinMaxVertices(Entity EN)
        {
            if (EN.Class == "MESH")
            {
                int Children = EN.CountChildren();
                int Surfaces = EN.CountSurfaces();
                int Vertices;
                uint Surf;
                float X, Y, Z;
                for (int i = 1; i <= Surfaces; ++i)
                {
                    Surf = EN.GetSurface(i);
                    Vertices = Entity.CountVertices(Surf);
                    for (int j = 0; j < Vertices; ++j)
                    {
                        X = Entity.VertexX(Surf, j);
                        Y = Entity.VertexY(Surf, j);
                        Z = Entity.VertexZ(Surf, j);
                        if (X < MinX)
                        {
                            MinX = X;
                        }
                        else if (X > MaxX)
                        {
                            MaxX = X;
                        }
                        if (Y < MinY)
                        {
                            MinY = Y;
                        }
                        else if (Y > MaxY)
                        {
                            MaxY = Y;
                        }
                        if (Z < MinZ)
                        {
                            MinZ = Z;
                        }
                        else if (Z > MaxZ)
                        {
                            MaxZ = Z;
                        }
                    }
                }
                for (int i = 1; i <= Children; ++i)
                {
                    MeshMinMaxVertices ChildResult = new MeshMinMaxVertices(EN.GetChild(i));
                    if (ChildResult.MinX < MinX)
                    {
                        MinX = ChildResult.MinX;
                    }
                    if (ChildResult.MinY < MinY)
                    {
                        MinY = ChildResult.MinY;
                    }
                    if (ChildResult.MinZ < MinZ)
                    {
                        MinZ = ChildResult.MinZ;
                    }
                    if (ChildResult.MaxX > MaxX)
                    {
                        MaxX = ChildResult.MaxX;
                    }
                    if (ChildResult.MaxY > MaxY)
                    {
                        MaxY = ChildResult.MaxY;
                    }
                    if (ChildResult.MaxZ > MaxZ)
                    {
                        MaxZ = ChildResult.MaxZ;
                    }
                    ChildResult = null;
                }
            }
        }

        // Constructor for a transformed mesh
        public MeshMinMaxVertices(Entity EN, float Pitch, float Yaw, float Roll, float ScaleX, float ScaleY,
                                  float ScaleZ)
        {
            if (EN.Class == "MESH")
            {
                int Children = EN.CountChildren();
                int Surfaces = EN.CountSurfaces();
                int Vertices;
                uint Surf;
                float X, Y, Z;
                for (int i = 1; i <= Surfaces; ++i)
                {
                    Surf = EN.GetSurface(i);
                    Vertices = Entity.CountVertices(Surf);
                    for (int j = 0; j < Vertices; ++j)
                    {
                        X = Entity.VertexX(Surf, j) * ScaleX;
                        Y = Entity.VertexY(Surf, j) * ScaleY;
                        Z = Entity.VertexZ(Surf, j) * ScaleZ;
                        Entity P = Entity.CreatePivot();
                        P.Rotate(Pitch, Yaw, Roll);
                        Entity.TFormPoint(X, Y, Z, P, null);
                        X = Entity.TFormedX();
                        Y = Entity.TFormedY();
                        Z = Entity.TFormedZ();
                        P.Free();

                        if (X < MinX)
                        {
                            MinX = X;
                        }
                        else if (X > MaxX)
                        {
                            MaxX = X;
                        }
                        if (Y < MinY)
                        {
                            MinY = Y;
                        }
                        else if (Y > MaxY)
                        {
                            MaxY = Y;
                        }
                        if (Z < MinZ)
                        {
                            MinZ = Z;
                        }
                        else if (Z > MaxZ)
                        {
                            MaxZ = Z;
                        }
                    }
                }
                for (int i = 1; i <= Children; ++i)
                {
                    MeshMinMaxVertices ChildResult = new MeshMinMaxVertices(EN.GetChild(i), Pitch, Yaw, Roll, ScaleX,
                                                                            ScaleY, ScaleZ);
                    if (ChildResult.MinX < MinX)
                    {
                        MinX = ChildResult.MinX;
                    }
                    if (ChildResult.MinY < MinY)
                    {
                        MinY = ChildResult.MinY;
                    }
                    if (ChildResult.MinZ < MinZ)
                    {
                        MinZ = ChildResult.MinZ;
                    }
                    if (ChildResult.MaxX > MaxX)
                    {
                        MaxX = ChildResult.MaxX;
                    }
                    if (ChildResult.MaxY > MaxY)
                    {
                        MaxY = ChildResult.MaxY;
                    }
                    if (ChildResult.MaxZ > MaxZ)
                    {
                        MaxZ = ChildResult.MaxZ;
                    }
                    ChildResult = null;
                }
            }
        }
    }

    public class linkOfPanel
    {
        public System.Windows.Forms.Panel panel;
        public linkOfPanel nextPanel;

        public linkOfPanel(System.Windows.Forms.Panel p)
        {
            nextPanel = null;
            panel = p;
        }

        public void AddPanel(System.Windows.Forms.Panel p)
        {
            //panel = p;
            linkOfPanel temp = this;
            while (temp.nextPanel != null)
            {
                temp = temp.nextPanel;
            }
            linkOfPanel newLinkOfPanel = new linkOfPanel(p);
            temp.nextPanel = newLinkOfPanel;
        }

        public void RemovePanels()
        {
            this.nextPanel = null;
            GC.Collect();
        }
    }
}