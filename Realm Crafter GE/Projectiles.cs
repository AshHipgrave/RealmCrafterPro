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
// Realm Crafter Projectiles module by Rob W (rottbott@hotmail.com)
// Original version October 2005, C# port October 2006

using System;
using System.IO;

namespace RealmCrafter
{
    // Projectile class
    public class Projectile
    {
        // Index
        public static Projectile[] Index = new Projectile[10000];

        // Members
        public int ID;
        public string Name;
        public ushort MeshID;
        public string Emitter1, Emitter2;
        public ushort Emitter1TexID, Emitter2TexID;
        public bool Homing;
        public byte HitChance, Speed;
        public ushort Damage, DamageType;

        // Linked list
        public static Projectile FirstProjectile;
        public Projectile NextProjectile;

        // Constructor
        public Projectile()
        {
            for (int i = 0; i < 10000; ++i)
            {
                if (Index[i] == null)
                {
                    Index[i] = this;
                    ID = i;
                    Name = "New projectile";
                    MeshID = 65535;
                    Emitter1 = "";
                    Emitter2 = "";
                    Emitter1TexID = 65535;
                    Emitter2TexID = 65535;
                    Homing = false;
                    HitChance = 1;
                    Speed = 1;

                    NextProjectile = FirstProjectile;
                    FirstProjectile = this;
                    return;
                }
            }
            throw new ProjectileException("Maximum number of projectiles already created!");
        }

        private Projectile(bool DoNotAssignID)
        {
            Name = "New projectile";
            MeshID = 65535;
            Emitter1 = "";
            Emitter2 = "";
            Emitter1TexID = 65535;
            Emitter2TexID = 65535;
            Homing = false;
            HitChance = 1;
            Speed = 1;

            NextProjectile = FirstProjectile;
            FirstProjectile = this;
            return;
        }

        // Finds a projectile by name
        public static Projectile Find(string Name)
        {
            Name = Name.ToUpper();
            Projectile P = FirstProjectile;
            while (P != null)
            {
                if (P.Name.ToUpper() == Name)
                {
                    return P;
                }
                P = P.NextProjectile;
            }
            return null;
        }

        // Removes this projectile from the index and linked list
        public void Delete()
        {
            Index[ID] = null;
            Projectile P = FirstProjectile;
            if (P == this)
            {
                FirstProjectile = NextProjectile;
            }
            else
            {
                while (P != null)
                {
                    if (P.NextProjectile == this)
                    {
                        P.NextProjectile = NextProjectile;
                        NextProjectile = null;
                        break;
                    }
                    P = P.NextProjectile;
                }
            }
        }

        // Load/save projectiles from/to file
        public static int Load(string Filename)
        {
            BinaryReader F = Blitz.ReadFile(Filename);
            if (F == null)
            {
                return -1;
            }

            int Projectiles = 0;
            long FileLength = F.BaseStream.Length;

            while (F.BaseStream.Position < FileLength)
            {
                Projectile P = new Projectile(true);
                P.ID = F.ReadUInt16();
                Index[P.ID] = P;
                P.Name = Blitz.ReadString(F);
                P.MeshID = F.ReadUInt16();
                P.Emitter1 = Blitz.ReadString(F);
                P.Emitter2 = Blitz.ReadString(F);
                P.Emitter1TexID = F.ReadUInt16();
                P.Emitter2TexID = F.ReadUInt16();
                P.Homing = F.ReadBoolean();
                P.HitChance = F.ReadByte();
                P.Damage = F.ReadUInt16();
                P.DamageType = F.ReadUInt16();
                P.Speed = F.ReadByte();

                Projectiles++;
            }

            F.Close();
            return Projectiles;
        }

        public static bool Save(string Filename)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                BinaryWriter F = Blitz.WriteFile(Filename);
                if (F == null)
                {
                    return false;
                }

                Projectile P = FirstProjectile;
                while (P != null)
                {
                    F.Write((ushort) P.ID);
                    Blitz.WriteString(P.Name, F);
                    F.Write(P.MeshID);
                    Blitz.WriteString(P.Emitter1, F);
                    Blitz.WriteString(P.Emitter2, F);
                    F.Write(P.Emitter1TexID);
                    F.Write(P.Emitter2TexID);
                    F.Write(P.Homing);
                    F.Write(P.HitChance);
                    F.Write(P.Damage);
                    F.Write(P.DamageType);
                    F.Write(P.Speed);

                    P = P.NextProjectile;
                }

                F.Close();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // Projectile creation exception
    public class ProjectileException : Exception
    {
        public ProjectileException(string Message) : base(Message)
        {
        }
    }
}