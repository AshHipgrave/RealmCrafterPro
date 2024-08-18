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
// Realm Crafter Animations module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port October 2006

using System;
using System.IO;
using RenderingServices;
using System.Windows.Forms;

namespace RealmCrafter
{
    // Animation set
    public class AnimSet
    {
        // Fixed animations used by the client
        public enum Anim
        {
            Walk = 149,
            Run = 148,
            SwimIdle = 147,
            SwimSlow = 146,
            SwimFast = 145,
            RideIdle = 144,
            RideWalk = 143,
            RideRun = 142,
            DefaultAttack = 141,
            RightAttack = 140,
            TwoHandAttack = 139,
            StaffAttack = 138,
            DefaultParry = 137,
            RightParry = 136,
            TwoHandParry = 135,
            StaffParry = 134,
            ShieldParry = 133,
            LastHit = 132,
            FirstHit = 130,
            LastDeath = 129,
            FirstDeath = 127,
            Jump = 126,
            Idle = 125,
            Yawn = 124,
            LookRound = 123,
            SitDown = 122,
            SitIdle = 121,
            StandUp = 120,
            StrafeLeft = 119,
            StrafeRight = 118
        } ;

        // Index
        public const int TotalAnimationSets = 20000;
        public static AnimSet[] Index = new AnimSet[TotalAnimationSets];

        // Members
        public string Name;
        public int ID;
        public string[] AnimName = new string[150];
        public ushort[] AnimStart = new ushort[150];
        public ushort[] AnimEnd = new ushort[150];
        public float[] AnimSpeed = new float[150];

        // Linked list
        public static AnimSet FirstAnimSet;
        public AnimSet NextAnimSet;

        // Constructor
        public AnimSet()
        {
            for (int i = 0; i < TotalAnimationSets; ++i)
            {
                if (Index[i] == null)
                {
                    Index[i] = this;
                    ID = i;
                    Name = "New animation set";
                    for (int j = 0; j < 150; ++j)
                    {
                        AnimName[j] = "";
                        AnimStart[j] = 0;
                        AnimEnd[j] = 0;
                        AnimSpeed[j] = 1f;
                    }
                    AnimName[149] = "Walk";
                    AnimName[148] = "Run";
                    AnimName[147] = "Swim idle";
                    AnimName[146] = "Swim slow";
                    AnimName[145] = "Swim fast";
                    AnimName[144] = "Ride idle";
                    AnimName[143] = "Ride walk";
                    AnimName[142] = "Ride run";
                    AnimName[141] = "Default attack";
                    AnimName[140] = "Right hand attack";
                    AnimName[139] = "Two hand attack";
                    AnimName[138] = "Staff attack";
                    AnimName[137] = "Default parry";
                    AnimName[136] = "Right hand parry";
                    AnimName[135] = "Two hand parry";
                    AnimName[134] = "Staff parry";
                    AnimName[133] = "Shield parry";
                    AnimName[132] = "Hit 1";
                    AnimName[131] = "Hit 2";
                    AnimName[130] = "Hit 3";
                    AnimName[129] = "Death 1";
                    AnimName[128] = "Death 2";
                    AnimName[127] = "Death 3";
                    AnimName[126] = "Jump";
                    AnimName[125] = "Idle";
                    AnimName[124] = "Yawn";
                    AnimName[123] = "Look around";
                    AnimName[122] = "Sit down";
                    AnimName[121] = "Sit idle";
                    AnimName[120] = "Stand up";
                    AnimName[119] = "Strafe Left";
                    AnimName[118] = "Strafe Right";
                    NextAnimSet = FirstAnimSet;
                    FirstAnimSet = this;
                    return;
                }
            }
            throw new AnimSetException("Maximum number of animation sets already created!");
        }

        private AnimSet(bool DoNotAssignID)
        {
            Name = "New animation set";
            for (int j = 0; j < 150; ++j)
            {
                AnimName[j] = "";
                AnimStart[j] = 0;
                AnimEnd[j] = 0;
                AnimSpeed[j] = 1f;
            }
            AnimName[149] = "Walk";
            AnimName[148] = "Run";
            AnimName[147] = "Swim idle";
            AnimName[146] = "Swim slow";
            AnimName[145] = "Swim fast";
            AnimName[144] = "Ride idle";
            AnimName[143] = "Ride walk";
            AnimName[142] = "Ride run";
            AnimName[141] = "Default attack";
            AnimName[140] = "Right hand attack";
            AnimName[139] = "Two hand attack";
            AnimName[138] = "Staff attack";
            AnimName[137] = "Default parry";
            AnimName[136] = "Right hand parry";
            AnimName[135] = "Two hand parry";
            AnimName[134] = "Staff parry";
            AnimName[133] = "Shield parry";
            AnimName[132] = "Hit 1";
            AnimName[131] = "Hit 2";
            AnimName[130] = "Hit 3";
            AnimName[129] = "Death 1";
            AnimName[128] = "Death 2";
            AnimName[127] = "Death 3";
            AnimName[126] = "Jump";
            AnimName[125] = "Idle";
            AnimName[124] = "Yawn";
            AnimName[123] = "Look around";
            AnimName[122] = "Sit down";
            AnimName[121] = "Sit idle";
            AnimName[120] = "Stand up";
            AnimName[119] = "Strafe Left";
            AnimName[118] = "Strafe Right";
            NextAnimSet = FirstAnimSet;
            FirstAnimSet = this;
            return;
        }

        // Removes this item from the index and linked list
        public void Delete()
        {
            Index[ID] = null;
            AnimSet A = FirstAnimSet;
            if (A == this)
            {
                FirstAnimSet = NextAnimSet;
            }
            else
            {
                while (A != null)
                {
                    if (A.NextAnimSet == this)
                    {
                        A.NextAnimSet = NextAnimSet;
                        NextAnimSet = null;
                        break;
                    }
                    A = A.NextAnimSet;
                }
            }
        }

        public static void UpdatePreviewFrames(ActorInstance AI, AnimSet A, int Sequence)
        {
            if (AI == null)
                return;
            if (AI.EN == null)
                return;

            RenderWrapper.bbdx2_ReExtractAnimSeq(((Entity)AI.EN).Handle, AI.AnimSeqs[Sequence], A.AnimStart[Sequence], A.AnimEnd[Sequence]);

        }

        // Plays an animation on an actor instance
        public static void PlayAnimation(ActorInstance AI, int Mode, float Speed, int Sequence, bool FixedSpeed)
        {
            // Get relevant animation set
            AnimSet A;
            if (AI.Gender == 0)
            {
                A = Index[AI.Actor.MaleAnimationSet];
            }
            else
            {
                A = Index[AI.Actor.FemaleAnimationSet];
            }

            if (Sequence == -1)
            {
                ((Entity)AI.EN).Animate(Mode, 0, 0);
            }

            //Get out of bound sequence
            if ((Sequence > A.AnimEnd.GetUpperBound(0)) || (Sequence < A.AnimStart.GetLowerBound(0)))
            {
                AnimSet.PlayAnimation(AI, 1, 0.5f, (int) AnimSet.Anim.Idle);
                ((Entity) AI.EN).Animate(Mode, 0, 0);
                MessageBox.Show("Animation sequence value is out of bounds", "Error");
                return;
            }

            // AnimSet is null
            if (A == null)
            {
                AnimSet.PlayAnimation(AI, 1, 0.5f, (int) AnimSet.Anim.Idle);
                ((Entity) AI.EN).Animate(Mode, 0, 0);
                MessageBox.Show("Animation is null", "Error");
                return;
            }
            // Invalid frames
            if (A.AnimEnd[Sequence] == 0 || A.AnimStart[Sequence] > A.AnimEnd[Sequence])
            {
                //AnimSet.PlayAnimation(AI, 1, 0.5f, (int) AnimSet.Anim.Idle);
                ((Entity) AI.EN).Animate(Mode, 0, 0);
                //MessageBox.Show("Animation frames are invalid:\nStart: " + A.AnimStart[Sequence].ToString() + "\nEnd: " + A.AnimEnd[Sequence].ToString(), "Error");
                return;
            }

            // Adjust speed if required and play the animation
            if (FixedSpeed)
            {
                int Length = A.AnimEnd[Sequence] - A.AnimStart[Sequence];
                if (Length < 1)
                {
                    Length = 1;
                }
                Speed *= (float) Length;
            }
            Speed *= A.AnimSpeed[Sequence];

            ((Entity) AI.EN).Animate(1, Speed, AI.AnimSeqs[Sequence]);
        }


        public static void PlayAnimation(ActorInstance AI, int Mode, float Speed, int Sequence)
        {
            PlayAnimation(AI, Mode, Speed, Sequence, true);
        }

        // Searches for an animation with a specific name in this set
        public int Find(string Name)
        {
            Name = Name.ToUpper();
            for (int i = 0; i < 150; ++i)
            {
                if (AnimName[i].ToUpper() == Name)
                {
                    return i;
                }
            }
            return -1;
        }

        // Loads all animation sets from a file
        public static int Load(string Filename)
        {
            BinaryReader F = Blitz.ReadFile(Filename);
            if (F == null)
            {
                return -1;
            }

            int Sets = 0;
            long FileLength = F.BaseStream.Length;

            while (F.BaseStream.Position < FileLength)
            {
                AnimSet A = new AnimSet(true);
                A.ID = F.ReadUInt16();
                Index[A.ID] = A;
                A.Name = Blitz.ReadString(F);
                for (int i = 0; i < 150; ++i)
                {
                    A.AnimName[i] = Blitz.ReadString(F);
                    A.AnimStart[i] = F.ReadUInt16();
                    A.AnimEnd[i] = F.ReadUInt16();
                    A.AnimSpeed[i] = F.ReadSingle();
                }


                if (string.IsNullOrEmpty(A.AnimName[(int)AnimSet.Anim.StrafeLeft]))
                    A.AnimName[(int)AnimSet.Anim.StrafeLeft] = "Strafe Left";
                if (string.IsNullOrEmpty(A.AnimName[(int)AnimSet.Anim.StrafeRight]))
                    A.AnimName[(int)AnimSet.Anim.StrafeRight] = "Strafe Right";
                

                Sets++;
            }


            F.Close();
            return Sets;
        }

        // Saves all animation sets to a file
        public static bool Save(string Filename)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                BinaryWriter F = Blitz.WriteFile(Filename);
                if (F == null)
                {
                    return false;
                }

                AnimSet A = FirstAnimSet;
                while (A != null)
                {
                    F.Write((ushort) A.ID);
                    Blitz.WriteString(A.Name, F);
                    for (int i = 0; i < 150; ++i)
                    {
                        Blitz.WriteString(A.AnimName[i], F);
                        F.Write(A.AnimStart[i]);
                        F.Write(A.AnimEnd[i]);
                        F.Write(A.AnimSpeed[i]);
                    }

                    A = A.NextAnimSet;
                }

                F.Close();

                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetAnimInt(string strAnim)
        {
            int j = 0;
            for (; j <= (int) Anim.Walk && AnimName[j] != strAnim; j++)
            {
                ;
            }
            return j;
        }
    }

    //  Animation set creation exception
    internal class AnimSetException : Exception
    {
        public AnimSetException(string Message) : base(Message)
        {
        }
    }
}