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
using System.IO;
using System.Windows.Forms;

namespace RealmCrafter_GE
{
    class DuplicateZoneTerrain
    {
        public static void Duplicate(string newZoneName, string oldZoneName)
        {
            File.Copy(@"Data\Areas\" + newZoneName + ".dat", @"Data\Areas\" + newZoneName + ".dat2", true);

            BinaryWriter O = Blitz.WriteFile(@"Data\Areas\" + newZoneName + ".dat");
            BinaryReader F = Blitz.ReadFile(@"Data\Areas\" + newZoneName + ".dat2");
            if (F == null)
                return;

            O.Write(F.ReadUInt16());
            O.Write(F.ReadUInt16());

            O.Write(F.ReadUInt16());
            O.Write(F.ReadUInt16());
            O.Write(F.ReadUInt16());
            O.Write(F.ReadUInt16());

            O.Write(F.ReadByte());
            O.Write(F.ReadByte());
            O.Write(F.ReadByte());
            O.Write(F.ReadSingle());
            O.Write(F.ReadSingle());


            O.Write(F.ReadUInt16());
            O.Write(F.ReadBoolean());
            O.Write(F.ReadByte());
            O.Write(F.ReadByte());
            O.Write(F.ReadByte());
            O.Write(F.ReadSingle());
            O.Write(F.ReadSingle());
            O.Write(F.ReadSingle());

            #region Load scenery objects
            ushort Sceneries = F.ReadUInt16();
            O.Write(Sceneries);

            for (int i = 0; i < Sceneries; ++i)
            {
                O.Write(F.ReadUInt16());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadByte());
                O.Write(F.ReadByte());
                O.Write(F.ReadUInt16());
                O.Write(F.ReadBoolean());
                O.Write(F.ReadByte());
                Blitz.WriteString(Blitz.ReadString(F), O);
                Blitz.WriteString(Blitz.ReadString(F), O);
            }
            #endregion

            #region Load water areas
            ushort Waters = F.ReadUInt16();
            O.Write(Waters);
            for (int i = 0; i < Waters; ++i)
            {
                O.Write(F.ReadUInt16());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadByte());
                O.Write(F.ReadByte());
                O.Write(F.ReadByte());
                O.Write(F.ReadByte());
            }
            #endregion

            #region Load collision boxes
            ushort ColBoxes = F.ReadUInt16();
            O.Write(ColBoxes);
            for (int i = 0; i < ColBoxes; ++i)
            {
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
            }
            #endregion

            #region Load emitters
            ushort Emitters = F.ReadUInt16();
            O.Write(Emitters);
            for (int i = 0; i < Emitters; ++i)
            {
                Blitz.WriteString(Blitz.ReadString(F), O);
                O.Write(F.ReadUInt16());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
            }
            #endregion

            #region Load basic mesh terrains
            ushort Terrains = F.ReadUInt16();
            O.Write(Terrains);

            for (int i = 0; i < Terrains; ++i)
            {
                string Filename = Blitz.ReadString(F);
                Filename = Filename.Replace(oldZoneName, newZoneName);

                Blitz.WriteString(Filename, O);

                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
            }
            #endregion

            #region Load sound zones
            ushort SoundZones = F.ReadUInt16();
            O.Write(SoundZones);
            for (int i = 0; i < SoundZones; ++i)
            {
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadSingle());
                O.Write(F.ReadUInt16());
                O.Write(F.ReadUInt16());
                O.Write(F.ReadInt32());
                O.Write(F.ReadByte());
            }
            #endregion

            #region Load dynamic lights
            if (F.BaseStream.Position + 1 < F.BaseStream.Length)
            {
                ushort Lights = F.ReadUInt16();
                O.Write(Lights);
                for (int i = 0; i < Lights; ++i)
                {
                    // Get Version
                    byte Version = F.ReadByte();
                    O.Write(Version);

                    switch (Version)
                    {
                        case 1:
                            {
                                O.Write(F.ReadSingle());
                                O.Write(F.ReadSingle());
                                O.Write(F.ReadSingle());
                                O.Write(F.ReadInt32());
                                O.Write(F.ReadInt32());
                                O.Write(F.ReadInt32());
                                O.Write(F.ReadSingle());
                                break;
                            }
                        case 2:
                            {
                                O.Write(F.ReadSingle());
                                O.Write(F.ReadSingle());
                                O.Write(F.ReadSingle());
                                O.Write(F.ReadInt32());
                                O.Write(F.ReadInt32());
                                O.Write(F.ReadInt32());
                                O.Write(F.ReadSingle());
                                Blitz.WriteString(Blitz.ReadString(F), O);

                                break;
                            }
                    }
                }
            }
            #endregion

            #region Load world setup (Marian Voicu) (Edited by Jared Belkus)

            ushort RainSoundId = 65535;
            ushort StormSoundId0 = 65535;
            ushort StormSoundId1 = 65535;
            ushort StormSoundId2 = 65535;
            ushort WindSoundId = 65535;
            ushort SnowTextureId = 65535;
            ushort RainTextureId = 65535;
            ushort SkyProfile = 65535;
            ushort CloudProfile = 65535;

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

            }
            catch (System.IO.EndOfStreamException)
            {
            }

            O.Write(RainSoundId);
            O.Write(WindSoundId);
            O.Write(StormSoundId0);
            O.Write(StormSoundId1);
            O.Write(StormSoundId2);
            O.Write(RainTextureId);
            O.Write(SnowTextureId);
            O.Write(SkyProfile);
            O.Write(CloudProfile);

            #endregion

            F.Close();
            O.Close();

            File.Delete(@"Data\Areas\" + newZoneName + ".dat2");

        }
    }
}
