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
// Realm Crafter Environment module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port November 2006

using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RealmCrafter
{
    public static partial class Environment
    {

        // Year

        public static int Year, Day, TimeH, TimeM, TimeFactor = 10;
        private static int TimeUpdate;
        public static bool HourChanged = true, MinuteChanged = true;

        // Load/save environment settings
        public static bool LoadEnvironment()
        {
            BinaryReader F = Blitz.ReadFile(@"Data\Server Data\Environment.dat");
            if (F == null)
            {
                return false;
            }

            Year = F.ReadInt32();
            Day = F.ReadInt32();
            TimeH = F.ReadInt32();
            TimeM = F.ReadInt32();
            TimeFactor = F.ReadInt32();

            F.Close();
            TimeUpdate = System.Environment.TickCount;
            return true;
        }

        public static bool SaveEnvironment()
        {
            return SaveEnvironment(false);
        }

        public static bool SaveEnvironment(bool FullSave)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                BinaryWriter F;
                if (FullSave)
                {
                    F = Blitz.WriteFile(@"Data\Server Data\Environment.dat");
                }
                else
                {
                    FileStream FStream = new FileStream(@"Data\Server Data\Environment.dat", System.IO.FileMode.Open,
                                                        FileAccess.Write);
                    F = new BinaryWriter(FStream);
                }
                if (F == null)
                {
                    return false;
                }
                F.Write(Year);
                F.Write(Day);
                F.Write(TimeH);
                F.Write(TimeM);
                F.Write(TimeFactor);

                F.Close();
                return true;
            }
            else
            {
                return false;
            }
        }


        // Advances time of day
        public static void UpdateEnvironment()
        {
            MinuteChanged = false;
            HourChanged = false;

            // Advance by one minute
            int Ticks = System.Environment.TickCount;
            if (Ticks - TimeUpdate > 60000 / TimeFactor)
            {
                TimeUpdate = Ticks;
                TimeM++;
                MinuteChanged = true;
                // Advance by one hour
                if (TimeM > 59)
                {
                    TimeH++;
                    TimeM = 0;
                    HourChanged = true;
                    // Advance by one day
                    if (TimeH > 23)
                    {
                        TimeH = 0;
                        Day++;
                    }
                }
            }
        }

        // Returns the delta (in minutes) between two times
        public static int TimeDelta(int StartH, int StartM, int EndH, int EndM)
        {
            if (StartH == EndH) // Start and end are in the same hour
            {
                return EndM - StartM;
            }
            else if (StartH < EndH) // Start hour is before end hour
            {
                return (60 - StartM) + EndM + (60 * (EndH - (StartH + 1)));
            }
            else // Start hour is after end hour (i.e. it spans two days)
            {
                return (60 - StartM) + EndM + (60 * (24 - (StartH + 1))) + (60 * EndH);
            }
        }

    }
}