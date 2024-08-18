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
using System.Media;
using System.Threading;
using System.Windows.Forms;
using RealmCrafter_GE.Utilities.Fullscreen;
using System;

namespace RealmCrafter_GE.RC_Crash_Handler
{
    public class ThreadExceptionHandler
    {
        public void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                HandleTaskBar.showTaskBar();
                Program.GE.Hide();
                Application.Idle -= new EventHandler(Program.GE.m_ZoneRender.WorldRender_MainLoop);
                Application.Idle -= new EventHandler(Program.GE.m_GubbinEditor.Application_Idle);

                CrashManager CM = new CrashManager(e);
                SystemSounds.Hand.Play();
                CM.ShowDialog();
                CM.Dispose();
                Program.GE.Dispose();
            }
            catch(Exception E)
            {
                MessageBox.Show("The crash handler, crashed. Giving up.");
                MessageBox.Show(E.StackTrace);
                MessageBox.Show(E.Source);
                Application.Exit();
            }
        }
    }
}