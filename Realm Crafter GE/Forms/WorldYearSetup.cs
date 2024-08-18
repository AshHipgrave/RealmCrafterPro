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
using System.Windows.Forms;
using Environment=RealmCrafter.Environment;

namespace RealmCrafter_GE.Forms
{
    public partial class WorldYearSetup : Form
    {
        public WorldYearSetup()
        {
            InitializeComponent();
            WorldYearTimeCompression.Value = Environment.TimeFactor;
            WorldYearCurrentYear.Value = Environment.Year;
            WorldYearCurrentDay.Value = Environment.Day + 1;
        }

        private void WorldYearSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void WorldYearTimeCompression_ValueChanged(object sender, EventArgs e)
        {
            if (Environment.TimeFactor != WorldYearTimeCompression.Value)
            {
                Environment.TimeFactor = (int) WorldYearTimeCompression.Value;
                Program.GE.SetWorldSavedStatus(false);
            }
        }

        private void WorldYearCurrentYear_ValueChanged(object sender, EventArgs e)
        {
            if (Environment.Year != WorldYearCurrentYear.Value)
            {
                Environment.Year = (int) WorldYearCurrentYear.Value;
                Program.GE.SetWorldSavedStatus(false);
            }
        }

        private void WorldYearCurrentDay_ValueChanged(object sender, EventArgs e)
        {
            int NewDay = (int) WorldYearCurrentDay.Value - 1;
            if (Environment.Day != NewDay)
            {
                Environment.Day = NewDay;
                Program.GE.SetWorldSavedStatus(false);
            }
        }

    }
}