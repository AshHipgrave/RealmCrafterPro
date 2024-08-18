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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RealmCrafter
{
    public partial class LightFunctionTimeEditorForm : Form
    {
        LightFunctionTime LightTime = null;

        public LightFunctionTimeEditorForm()
        {
            InitializeComponent();
            TopLevel = false;
        }

        public object SelectedObject
        {
            get
            {
                LightTime = new LightFunctionTime();

                if (GameTimerOption.Checked)
                    LightTime.UseGameTime = true;

                LightTime.TimeH = Convert.ToInt32(GameTimeHour.Value);
                LightTime.TimeM = Convert.ToInt32(GameTimeMinute.Value);
                LightTime.TimeMS = Convert.ToInt32(WaitTimeMS.Value);

                return LightTime;
            }
            set
            {
                if (!(value is LightFunctionTime))
                    return;

                LightTime = value as LightFunctionTime;

                WaitTimerOption.Checked = !LightTime.UseGameTime;
                GameTimerOption.Checked = LightTime.UseGameTime;

                WaitTimeMS.Value = LightTime.TimeMS;
                GameTimeHour.Value = LightTime.TimeH;
                GameTimeMinute.Value = LightTime.TimeM;
                
            }
        }
    }
}